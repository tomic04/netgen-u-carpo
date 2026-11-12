using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Samo citanje baze spin. Naredba koja nije SELECT se odbija prije slanja.
    /// </summary>
    public static class Citanje
    {
        public static string Spoj
        {
            get
            {
                string e = Environment.GetEnvironmentVariable("SPIN_SQL");
                if (!string.IsNullOrWhiteSpace(e))
                    return e.IndexOf("TrustServerCertificate", StringComparison.OrdinalIgnoreCase) >= 0
                        ? e
                        : e.Trim().TrimEnd(';') + ";TrustServerCertificate=True";
                return "Server=localhost;Database=spin;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=15;ApplicationIntent=ReadOnly";
            }
        }

        public static bool JeCitanje(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return false;
            string s = sql.Trim();
            if (!s.StartsWith("select", StringComparison.OrdinalIgnoreCase)
                && !s.StartsWith("with", StringComparison.OrdinalIgnoreCase))
                return false;
            string l = " " + s.ToLowerInvariant() + " ";
            string[] zabranjeno =
            {
                " insert ", " update ", " delete ", " merge ", " drop ", " alter ",
                " truncate ", " exec ", " execute ", " into ", " grant ", " revoke "
            };
            for (int i = 0; i < zabranjeno.Length; i++)
            {
                if (l.Contains(zabranjeno[i]))
                    return false;
            }
            return true;
        }

        public sealed class Shema
        {
            public bool Uspjeh;
            public string Greska;
            public readonly List<string> Imena = new List<string>();
            public readonly List<string> Tipovi = new List<string>();
            public int Redova;
            public bool SortkeyNeopada = true;
        }

        public static Shema Select(string sql, int maxRedova)
        {
            var r = new Shema();
            if (!JeCitanje(sql))
            {
                string razlog;
                if (!SmijeProceduru(sql, out razlog))
                {
                    r.Greska = razlog ?? "odbio sam naredbu koja nije cisto citanje";
                    return r;
                }
            }
            try
            {
                using (var conn = new SqlConnection(Spoj))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = 60;
                        using (var rd = cmd.ExecuteReader())
                        {
                            for (int i = 0; i < rd.FieldCount; i++)
                            {
                                r.Imena.Add(rd.GetName(i));
                                r.Tipovi.Add(rd.GetFieldType(i).Name);
                            }
                            int sort = -1;
                            for (int i = 0; i < r.Imena.Count; i++)
                            {
                                if (string.Equals(r.Imena[i], "sortkey", StringComparison.OrdinalIgnoreCase))
                                    sort = i;
                            }
                            object prosli = null;
                            while (rd.Read())
                            {
                                r.Redova++;
                                if (sort >= 0 && r.SortkeyNeopada && !rd.IsDBNull(sort))
                                {
                                    object sad = rd.GetValue(sort);
                                    IComparable cmp = prosli as IComparable;
                                    if (cmp != null && sad != null && cmp.CompareTo(sad) > 0)
                                        r.SortkeyNeopada = false;
                                    prosli = sad;
                                }
                                if (maxRedova > 0 && r.Redova >= maxRedova)
                                    break;
                            }
                        }
                    }
                }
                r.Uspjeh = true;
            }
            catch (Exception ex)
            {
                r.Greska = ex.GetType().Name + ": " + ex.Message;
            }
            return r;
        }

        public static string PrviTekst(string sql, out string greska)
        {
            greska = null;
            if (!JeCitanje(sql))
            {
                greska = "odbio sam naredbu koja nije cisto citanje";
                return null;
            }
            try
            {
                using (var conn = new SqlConnection(Spoj))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = 30;
                        object o = cmd.ExecuteScalar();
                        if (o == null || o is DBNull)
                            return "";
                        return Convert.ToString(o);
                    }
                }
            }
            catch (Exception ex)
            {
                greska = ex.GetType().Name + ": " + ex.Message;
                return null;
            }
        }

        public static string TipoviStupaca(string tablica, IList<string> imena)
        {
            var sb = new StringBuilder();
            sb.Append("select c.name, ty.name from sys.columns c join sys.types ty on c.user_type_id = ty.user_type_id where c.object_id = object_id('dbo.");
            sb.Append(tablica.Replace("'", ""));
            sb.Append("') and c.name in (");
            for (int i = 0; i < imena.Count; i++)
            {
                if (i > 0)
                    sb.Append(",");
                sb.Append("'");
                sb.Append(imena[i].Replace("'", ""));
                sb.Append("'");
            }
            sb.Append(")");
            return sb.ToString();
        }

        public static List<string> Stupac(string sql, out string greska)
        {
            var lista = new List<string>();
            greska = null;
            if (!JeCitanje(sql))
            {
                greska = "odbio sam naredbu koja nije cisto citanje";
                return null;
            }
            try
            {
                using (var conn = new SqlConnection(Spoj))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = 30;
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                lista.Add(rd.IsDBNull(0) ? "" : Convert.ToString(rd.GetValue(0)));
                                if (lista.Count >= 20)
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                greska = ex.GetType().Name + ": " + ex.Message;
                return null;
            }
            return lista;
        }

        /// <summary>
        /// Null znači da izvršni tekst nema upis ni ugniježđeni poziv.
        /// Komentar se ne izvršava. WITH EXECUTE AS je opcija modula, ne poziv.
        /// NetGen ovaj filtar nema: ExecSql šalje tekst koji nema @@ (Class1.vb 113–118).
        /// </summary>
        public static string RazlogTijela(string def)
        {
            if (string.IsNullOrWhiteSpace(def))
                return "nema tijela";
            string t = SkiniKomentare(def);
            t = Regex.Replace(t, @"^\s*(create|alter)\s+(proc|procedure|function|view)\b", " ", RegexOptions.IgnoreCase);
            t = Regex.Replace(t, @"\bwith\s+execute\s+as\s+[A-Za-z_][A-Za-z0-9_]*", " ", RegexOptions.IgnoreCase);
            t = Regex.Replace(t, @"\bexecute\s+as\s+(owner|caller|self)\b", " ", RegexOptions.IgnoreCase);
            t = Regex.Replace(t, @"\bexecute\s+as\s+user\s*=\s*N?'[^']*'", " ", RegexOptions.IgnoreCase);
            string[] kljuc = { "insert", "update", "delete", "merge", "drop", "alter", "truncate", "exec", "execute", "into" };
            for (int i = 0; i < kljuc.Length; i++)
            {
                if (Regex.IsMatch(t, @"\b" + kljuc[i] + @"\b", RegexOptions.IgnoreCase))
                    return "kljucna rijec " + kljuc[i];
            }
            return null;
        }

        private static string SkiniKomentare(string def)
        {
            string t = Regex.Replace(def, @"/\*.*?\*/", " ", RegexOptions.Singleline);
            return Regex.Replace(t, @"--.*?$", " ", RegexOptions.Multiline);
        }

        /// <summary>
        /// Jedan poziv procedure, bez točka-zareza. Tijelo mora proći RazlogTijela.
        /// Inače false i razlog ostaje null (nije poziv) ili opisuje tijelo.
        /// </summary>
        private static bool SmijeProceduru(string sql, out string razlog)
        {
            razlog = null;
            string ime;
            if (!JedanPoziv(sql, out ime))
                return false;
            string greska;
            string def = PrviTekst("select object_definition(object_id(N'dbo." + ime + "'))", out greska);
            if (def == null)
            {
                razlog = greska ?? "nema tijela";
                return false;
            }
            razlog = RazlogTijela(def);
            return razlog == null;
        }

        private static bool JedanPoziv(string sql, out string ime)
        {
            ime = null;
            string s = (sql ?? "").Trim();
            if (s.Length == 0 || s.IndexOf(';') >= 0 || s.IndexOf("--", StringComparison.Ordinal) >= 0 || s.IndexOf("/*", StringComparison.Ordinal) >= 0)
                return false;
            int i = 0;
            Preskoci(s, ref i);
            string w = Rijec(s, ref i);
            if (Isto(w, "exec") || Isto(w, "execute"))
            {
                Preskoci(s, ref i);
                w = Rijec(s, ref i);
            }
            if (Isto(w, "dbo") && i < s.Length && s[i] == '.')
            {
                i++;
                w = Rijec(s, ref i);
            }
            if (!ImeProcedure(w) || ZabranjenoIme(w))
                return false;
            ime = w;
            Preskoci(s, ref i);
            while (i < s.Length)
            {
                if (s[i] != '@')
                    return false;
                i++;
                string par = Rijec(s, ref i);
                if (!ImeProcedure(par))
                    return false;
                Preskoci(s, ref i);
                if (i >= s.Length || s[i] != '=')
                    return false;
                i++;
                Preskoci(s, ref i);
                if (!Vrijednost(s, ref i))
                    return false;
                Preskoci(s, ref i);
                if (i < s.Length && s[i] == ',')
                {
                    i++;
                    Preskoci(s, ref i);
                    continue;
                }
                break;
            }
            return i >= s.Length;
        }

        private static bool Vrijednost(string s, ref int i)
        {
            if (i >= s.Length)
                return false;
            if (s[i] == 'N' || s[i] == 'n')
            {
                if (i + 1 < s.Length && s[i + 1] == '\'')
                    i++;
            }
            if (s[i] == '\'')
            {
                i++;
                while (i < s.Length)
                {
                    if (s[i] == '\'' && i + 1 < s.Length && s[i + 1] == '\'')
                    {
                        i += 2;
                        continue;
                    }
                    if (s[i] == '\'')
                    {
                        i++;
                        return true;
                    }
                    i++;
                }
                return false;
            }
            int p = i;
            string w = Rijec(s, ref i);
            if (Isto(w, "null"))
                return true;
            i = p;
            if (s[i] == '-' || s[i] == '+')
                i++;
            bool broj = false;
            while (i < s.Length && ((s[i] >= '0' && s[i] <= '9') || s[i] == '.'))
            {
                broj = true;
                i++;
            }
            return broj;
        }

        private static bool ImeProcedure(string w)
        {
            if (string.IsNullOrEmpty(w))
                return false;
            if (!(char.IsLetter(w[0]) || w[0] == '_'))
                return false;
            for (int i = 1; i < w.Length; i++)
            {
                char c = w[i];
                if (!(char.IsLetterOrDigit(c) || c == '_'))
                    return false;
            }
            return true;
        }

        private static bool ZabranjenoIme(string w)
        {
            if (w.StartsWith("xp_", StringComparison.OrdinalIgnoreCase))
                return true;
            return Isto(w, "select") || Isto(w, "with") || Isto(w, "update") || Isto(w, "insert")
                || Isto(w, "delete") || Isto(w, "merge") || Isto(w, "drop") || Isto(w, "alter")
                || Isto(w, "truncate") || Isto(w, "into") || Isto(w, "grant") || Isto(w, "revoke")
                || Isto(w, "create") || Isto(w, "declare") || Isto(w, "set")
                || Isto(w, "sp_executesql");
        }

        private static void Preskoci(string s, ref int i)
        {
            while (i < s.Length && char.IsWhiteSpace(s[i]))
                i++;
        }

        private static string Rijec(string s, ref int i)
        {
            int p = i;
            while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_'))
                i++;
            if (i == p)
                return "";
            return s.Substring(p, i - p);
        }

        private static bool Isto(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
    }
}

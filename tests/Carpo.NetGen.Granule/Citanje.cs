using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

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
                    return e;
                return "Server=localhost;Database=spin;Integrated Security=True;Connect Timeout=15;ApplicationIntent=ReadOnly";
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
                r.Greska = "odbio sam naredbu koja nije cisto citanje";
                return r;
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
    }
}

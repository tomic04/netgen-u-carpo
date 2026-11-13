using System;
using System.Collections.Generic;
using System.Text;

namespace Carpo.NetGen.Grid
{
    /// <summary>
    /// Prije slanja na jupiter2019/jupiter2022. Prva riječ (insert/update/delete)
    /// propušta <c>spKntZakljucivanjeTem @action=1</c>. Zato se gleda cijeli tekst
    /// i, za EXEC, tijelo iz sys.sql_modules.
    /// </summary>
    public sealed class SqlZastita
    {
        public bool Pise;
        public string Razlog = "";
        public readonly List<string> Procedure = new List<string>();
    }

    public static class ZastitaSql
    {
        private static readonly HashSet<string> Naredbe = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "select", "with", "declare", "set", "if", "begin", "end", "while", "return",
            "print", "waitfor", "use", "goto", "throw", "raiserror", "dbcc", "open",
            "fetch", "close", "commit", "rollback", "break", "continue", "create",
            "insert", "update", "delete", "drop", "alter", "truncate", "merge",
            "grant", "revoke", "checkpoint", "backup", "restore", "kill", "from",
            "where", "and", "or", "not", "into", "values", "table", "proc", "procedure",
            "function", "view", "index", "trigger", "as", "on", "join", "inner", "left",
            "right", "outer", "cross", "group", "order", "by", "having", "union", "all",
            "case", "when", "then", "else", "top", "distinct", "exec", "execute"
        };

        /// <summary>Tekst koji bismo poslali izravno. DELETE/UPDATE/DROP/TRUNCATE/MERGE/ALTER/GRANT/REVOKE uvijek. CREATE osim CREATE TABLE #temp. INSERT osim u #temp.</summary>
        public static SqlZastita Tekst(string sql)
        {
            return Analiza(sql, false);
        }

        /// <summary>Tijelo procedure. INSERT/UPDATE/DELETE/MERGE/TRUNCATE/DROP nad tablicom koja nije #temp ili @varijabla.</summary>
        public static SqlZastita Tijelo(string definicija)
        {
            return Analiza(definicija, true);
        }

        private static SqlZastita Analiza(string sql, bool tijelo)
        {
            var z = new SqlZastita();
            if (string.IsNullOrWhiteSpace(sql))
                return z;
            var kod = new StringBuilder();
            var literali = new List<string>();
            Izvuci(sql, kod, literali);
            for (int i = 0; i < literali.Count; i++)
            {
                var unutra = Analiza(literali[i], tijelo);
                if (unutra.Pise && !z.Pise)
                {
                    z.Pise = true;
                    z.Razlog = unutra.Razlog;
                }
            }
            string[] dijelovi = kod.ToString().Split(';');
            for (int d = 0; d < dijelovi.Length; d++)
            {
                List<string> tokeni = Tokeni(dijelovi[d]);
                if (tokeni.Count == 0)
                    continue;
                Pisi(tokeni, tijelo, z);
                Procedura(tokeni, z);
            }
            return z;
        }

        private static void Izvuci(string sql, StringBuilder kod, List<string> literali)
        {
            int i = 0;
            while (i < sql.Length)
            {
                char c = sql[i];
                if (c == '-' && i + 1 < sql.Length && sql[i + 1] == '-')
                {
                    while (i < sql.Length && sql[i] != '\n')
                        i++;
                    kod.Append(' ');
                    continue;
                }
                if (c == '/' && i + 1 < sql.Length && sql[i + 1] == '*')
                {
                    i += 2;
                    while (i + 1 < sql.Length && !(sql[i] == '*' && sql[i + 1] == '/'))
                        i++;
                    if (i + 1 < sql.Length)
                        i += 2;
                    else
                        i = sql.Length;
                    kod.Append(' ');
                    continue;
                }
                if (c == '\'')
                {
                    i++;
                    var lit = new StringBuilder();
                    while (i < sql.Length)
                    {
                        if (sql[i] == '\'' && i + 1 < sql.Length && sql[i + 1] == '\'')
                        {
                            lit.Append('\'');
                            i += 2;
                            continue;
                        }
                        if (sql[i] == '\'')
                        {
                            i++;
                            break;
                        }
                        lit.Append(sql[i]);
                        i++;
                    }
                    literali.Add(lit.ToString());
                    kod.Append(" '' ");
                    continue;
                }
                kod.Append(c);
                i++;
            }
        }

        private static List<string> Tokeni(string sql)
        {
            var t = new List<string>();
            int i = 0;
            while (i < sql.Length)
            {
                char c = sql[i];
                if (char.IsWhiteSpace(c) || c == '(' || c == ')' || c == ',' || c == '=' || c == '+' || c == '-' || c == '*' || c == '/' || c == '<' || c == '>' || c == '!' || c == ';')
                {
                    if (c == '(')
                        t.Add("(");
                    i++;
                    continue;
                }
                t.Add(ProcitajIme(sql, ref i));
            }
            return t;
        }

        private static string ProcitajIme(string sql, ref int i)
        {
            var sb = new StringBuilder();
            while (i < sql.Length)
            {
                if (sql[i] == '[')
                {
                    int j = sql.IndexOf(']', i + 1);
                    if (j < 0)
                        j = sql.Length - 1;
                    sb.Append(sql.Substring(i, j - i + 1));
                    i = j + 1;
                }
                else
                {
                    int k = i;
                    while (k < sql.Length)
                    {
                        char d = sql[k];
                        if (char.IsWhiteSpace(d) || d == '(' || d == ')' || d == ',' || d == '=' || d == '+' || d == '-' || d == '*' || d == '/' || d == '<' || d == '>' || d == '!' || d == ';' || d == '[' || d == '.')
                            break;
                        k++;
                    }
                    if (k == i)
                        break;
                    sb.Append(sql.Substring(i, k - i));
                    i = k;
                }
                if (i < sql.Length && sql[i] == '.')
                {
                    sb.Append('.');
                    i++;
                    continue;
                }
                break;
            }
            return sb.ToString();
        }

        private static void Pisi(List<string> tokeni, bool tijelo, SqlZastita z)
        {
            for (int i = 0; i < tokeni.Count; i++)
            {
                string w = tokeni[i].ToLowerInvariant();
                if (w == "delete" || w == "update" || w == "insert" || w == "merge" || w == "truncate" || w == "drop" || w == "create" || w == "alter" || w == "grant" || w == "revoke")
                {
                    string cilj = Cilj(tokeni, i, w);
                    if (Zabranjeno(w, cilj, tijelo))
                    {
                        z.Pise = true;
                        if (z.Razlog.Length == 0)
                            z.Razlog = w + (cilj.Length == 0 ? "" : " " + cilj);
                        return;
                    }
                }
                if (w == "into" && !JeInsertIliMerge(tokeni, i))
                {
                    string cilj = SljedeciIdent(tokeni, i + 1);
                    if (!Privremena(cilj))
                    {
                        z.Pise = true;
                        if (z.Razlog.Length == 0)
                            z.Razlog = "into " + cilj;
                        return;
                    }
                }
            }
        }

        private static bool JeInsertIliMerge(List<string> tokeni, int into)
        {
            for (int i = into - 1; i >= 0; i--)
            {
                string w = tokeni[i].ToLowerInvariant();
                if (w == "(" || w == ")")
                    continue;
                return w == "insert" || w == "merge";
            }
            return false;
        }

        private static bool Zabranjeno(string naredba, string cilj, bool tijelo)
        {
            bool priv = Privremena(cilj);
            if (naredba == "insert")
                return !priv;
            if (naredba == "create")
                return !priv;
            if (naredba == "merge" || naredba == "truncate" || naredba == "drop" || naredba == "delete" || naredba == "update")
                return !(tijelo && priv);
            return true;
        }

        private static string Cilj(List<string> tokeni, int i, string naredba)
        {
            int j = i + 1;
            if (j < tokeni.Count)
            {
                string s = tokeni[j].ToLowerInvariant();
                if (naredba == "delete" && s == "from")
                    j++;
                else if (naredba == "insert" && s == "into")
                    j++;
                else if (naredba == "merge" && s == "into")
                    j++;
                else if (naredba == "truncate" && s == "table")
                    j++;
                else if (naredba == "drop" && (s == "table" || s == "proc" || s == "procedure" || s == "view" || s == "index"))
                    j++;
                else if (naredba == "create" && (s == "table" || s == "proc" || s == "procedure" || s == "view" || s == "index"))
                {
                    if (s != "table")
                        return "schema";
                    j++;
                }
                else if (naredba == "update" && s == "top")
                    j += 2;
            }
            return SljedeciIdent(tokeni, j);
        }

        private static string SljedeciIdent(List<string> tokeni, int j)
        {
            while (j < tokeni.Count && tokeni[j] == "(")
                j++;
            if (j >= tokeni.Count)
                return "";
            return tokeni[j];
        }

        private static bool Privremena(string cilj)
        {
            if (string.IsNullOrEmpty(cilj))
                return false;
            string[] dijelovi = cilj.Split('.');
            for (int i = 0; i < dijelovi.Length; i++)
            {
                string d = dijelovi[i].Trim();
                if (d.StartsWith("[", StringComparison.Ordinal) && d.EndsWith("]", StringComparison.Ordinal) && d.Length >= 2)
                    d = d.Substring(1, d.Length - 2);
                if (d.StartsWith("#", StringComparison.Ordinal) || d.StartsWith("@", StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        private static void Procedura(List<string> tokeni, SqlZastita z)
        {
            if (!Naredbe.Contains(tokeni[0]) && tokeni[0] != "''" && tokeni[0] != "(" && !tokeni[0].StartsWith("@", StringComparison.Ordinal))
                DodajProceduru(tokeni[0], z);
            for (int i = 0; i < tokeni.Count; i++)
            {
                if (!string.Equals(tokeni[i], "exec", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(tokeni[i], "execute", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (i + 1 >= tokeni.Count)
                    continue;
                string sljedeci = tokeni[i + 1];
                if (sljedeci == "(" || sljedeci.StartsWith("@", StringComparison.Ordinal))
                {
                    z.Pise = true;
                    if (z.Razlog.Length == 0)
                        z.Razlog = "dinamički EXEC";
                    continue;
                }
                DodajProceduru(sljedeci, z);
            }
        }

        private static void DodajProceduru(string ime, SqlZastita z)
        {
            if (ime == "''" || ime == "(" || Naredbe.Contains(ime))
                return;
            string cisto = OcisitiIme(ime);
            if (cisto.Length == 0 || Naredbe.Contains(cisto))
                return;
            if (!Sadrzi(z.Procedure, cisto))
                z.Procedure.Add(cisto);
        }

        public static string OcisitiIme(string ime)
        {
            if (ime == null)
                return "";
            string[] dijelovi = ime.Split('.');
            string zadnji = dijelovi[dijelovi.Length - 1].Trim();
            if (zadnji.StartsWith("[", StringComparison.Ordinal) && zadnji.EndsWith("]", StringComparison.Ordinal) && zadnji.Length >= 2)
                zadnji = zadnji.Substring(1, zadnji.Length - 2);
            return zadnji;
        }

        private static bool Sadrzi(List<string> lista, string ime)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if (string.Equals(lista[i], ime, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}

using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Poziv
{
    /// <summary>
    /// ASSEMBLYLOAD. Form1.vb 4584–4623 (shell), 6602–6661 (s dvotočkom),
    /// 6681–6727 (Rafalno bez dvotočke), Form2.vb 7424–7456 (dijalog).
    /// Ime sklopa je tekst lijevo od zadnje točke. CreateInstance dobije cijelo ime.
    /// totalgen ima pet argumenata. Povrat se ne čita. Pad Load ili CreateInstance
    /// ide u log i prekida ostatak lanca (Exit Sub).
    /// </summary>
    public static class AssemblyPoziv
    {
        public sealed class Ishod
        {
            public bool Prekinuto;
            public bool Pozvan;
            public string Sklop = "";
            public string Tip = "";
            public readonly string[] Argumenti = new string[5];
        }

        public static string TipIz(string naredba)
        {
            string s = (naredba ?? "").Trim();
            string oznaka = "ASSEMBLYLOAD(";
            int a = s.IndexOf(oznaka, StringComparison.Ordinal);
            if (a < 0)
                return "";
            s = s.Substring(a + oznaka.Length);
            s = s.Replace(")", "");
            return s.Trim();
        }

        public static string SklopIz(string tip)
        {
            string t = tip ?? "";
            int z = t.LastIndexOf('.');
            if (z <= 0)
                return "";
            return t.Substring(0, z);
        }

        /// <summary>
        /// Stupci s imenom. Janus preskače stupac 0 jer mu je DataMember prazan (Form1.vb 2557–2576).
        /// Zadnja četiri znaka koja su broj različit od nule otpadnu (Val).
        /// </summary>
        public static string ParametriReda(DataRow red)
        {
            if (red == null || red.Table == null)
                return "";
            var sb = new StringBuilder();
            for (int i = 0; i < red.Table.Columns.Count; i++)
            {
                string ime = (red.Table.Columns[i].ColumnName ?? "").Trim();
                if (ime.Length == 0)
                    continue;
                if (red[i] == null || red[i] == DBNull.Value)
                    continue;
                if (ime.Length >= 4)
                {
                    int sirina;
                    if (int.TryParse(ime.Substring(ime.Length - 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out sirina) && sirina != 0)
                        ime = ime.Substring(0, ime.Length - 4);
                }
                ime = ime.Replace(" ", "_").Replace("!", "");
                string v = Convert.ToString(red[i], CultureInfo.InvariantCulture) ?? "";
                decimal broj;
                if (decimal.TryParse(v, NumberStyles.Number, CultureInfo.CurrentCulture, out broj)
                    || decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out broj))
                    v = v.Replace(",", ".");
                sb.Append("@").Append(ime).Append("='").Append(v).Append("' , ");
            }
            return sb.ToString();
        }

        public static string Konekcija(string put, string sirova)
        {
            string t = sirova ?? "";
            if (string.Equals(put, "rafalno", StringComparison.Ordinal))
                return t;
            if (t.ToLowerInvariant().IndexOf("driver", StringComparison.Ordinal) >= 0)
                return t;
            return "Driver={SQL Server};" + t;
        }

        public static Ishod Pozovi(string naredba, string put, string id, string operater, string konekcija, string zadnjiSql, string parametriGrida, string parametriSvi)
        {
            var i = new Ishod();
            i.Tip = TipIz(naredba);
            i.Sklop = SklopIz(i.Tip);
            string p = put ?? "shell";
            string cs = Konekcija(p, konekcija);
            i.Argumenti[0] = id ?? "";
            i.Argumenti[1] = operater ?? "";
            i.Argumenti[2] = cs;
            if (p == "dijalog")
            {
                i.Argumenti[3] = parametriSvi ?? "";
                i.Argumenti[4] = parametriSvi ?? "";
            }
            else if (p == "rafalno")
            {
                i.Argumenti[3] = zadnjiSql ?? "";
                i.Argumenti[4] = "";
            }
            else
            {
                i.Argumenti[3] = zadnjiSql ?? "";
                i.Argumenti[4] = parametriGrida ?? "";
            }
            if (i.Sklop.Length == 0 || i.Tip.Length == 0)
            {
                i.Prekinuto = true;
                NgDnevnik.Greska("Greška kod Assembly.Load(" + i.Sklop + ") : nema točke u imenu");
                return i;
            }
            Assembly a;
            try
            {
                a = Assembly.Load(i.Sklop);
            }
            catch (Exception ex)
            {
                i.Prekinuto = true;
                NgDnevnik.Greska("Greška kod Assembly.Load(" + i.Sklop + ") : " + ex.Message);
                return i;
            }
            object o;
            try
            {
                o = a.CreateInstance(i.Tip);
                if (o == null)
                    throw new InvalidOperationException("CreateInstance je vratio null");
            }
            catch (Exception ex)
            {
                i.Prekinuto = true;
                NgDnevnik.Greska("Greška kod CreateInstance (" + i.Tip + ") : " + ex.Message);
                return i;
            }
            if (p == "kolon")
                NgDnevnik.Akcija("zovem totalgen u asembliju ...");
            try
            {
                o.GetType().InvokeMember(
                    "totalgen",
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    o,
                    new object[] { i.Argumenti[0], i.Argumenti[1], i.Argumenti[2], i.Argumenti[3], i.Argumenti[4] });
                i.Pozvan = true;
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("nakon totalgen u asembliju " + ex.Message);
            }
            return i;
        }
    }
}

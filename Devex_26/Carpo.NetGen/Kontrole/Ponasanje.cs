using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Carpo.NetGen.Kontrole
{
    /// <summary>
    /// Ponašanje kontrola dijaloga onako kako Form2 i Form3 rade, uključujući tipfelere.
    /// </summary>
    public static class Ponasanje
    {
        public static string DatumPocetni(string raw, int godina, int mjesec)
        {
            if (raw == null)
                return null;
            string t = raw.ToLowerInvariant();
            if (t == "firstdayofyear")
                return "01.01." + godina.ToString(CultureInfo.InvariantCulture);
            if (t == "firstdayoflastyear")
                return "01.01." + (godina - 1).ToString(CultureInfo.InvariantCulture);
            if (t == "firstdayofmonth")
                return "01." + mjesec.ToString(CultureInfo.InvariantCulture) + "." + godina.ToString(CultureInfo.InvariantCulture);
            if (t == "firstdayoflastmonth")
                return "01." + (mjesec - 1).ToString(CultureInfo.InvariantCulture) + "." + godina.ToString(CultureInfo.InvariantCulture);
            return t;
        }

        public static bool AllowEmptyJeTrue(string s)
        {
            return s != null && s.Trim().ToLowerInvariant() == "true";
        }

        /// <summary>SHOWCHECKBOX bez CHECKED ostavi Checked=false, parametar je null. Form2.vb 2276–2291, 3522–3566.</summary>
        public static string DatumParametar(bool showCheckBox, string checkedAtribut, DateTime vrijednost)
        {
            bool cekiran = true;
            if (showCheckBox)
                cekiran = false;
            if (checkedAtribut != null)
            {
                string t = checkedAtribut.Trim().ToLowerInvariant();
                cekiran = t == "true" || t == "1";
            }
            if (!cekiran)
                return "null";
            return "'" + vrijednost.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "'";
        }

        public static string BrojParametar(string tekst)
        {
            if (tekst == null || tekst.Trim().Length == 0)
                return "0";
            return tekst.Trim().Replace(",", ".");
        }

        /// <summary>cur i number dijele isti NumerBox. Nema posebnog formata valute. Form2.vb 2071.</summary>
        public static string DxBroj()
        {
            return "TextEdit;Width=100;RightToLeft=Yes;HAlignment=Near";
        }

        public static string Dx(string grana)
        {
            if (grana == "text")
                return "TextEdit;Width=300";
            if (grana == "number")
                return DxBroj();
            if (grana == "search")
                return "ButtonEdit;Width=300;Tag=0";
            if (grana == "date")
                return "DateEdit;Width=100;Format=Short;AllowNull=false";
            if (grana == "combo")
                return "LookUpEdit;Width=300;ValueMember=stupac0;DisplayMember=stupac1";
            if (grana == "check")
                return "CheckEdit;Width=100;AutoSize=true";
            if (grana == "radio")
                return "CheckEdit;Width=200;Radio=true";
            if (grana == "multidrop")
                return "CheckedComboBoxEdit;Width=300";
            if (grana == "label")
                return "LabelControl;AutoSize=true;BackColor=Transparent";
            if (grana == "group")
                return "GroupControl;BackColor=Transparent";
            if (grana == "panel")
                return "PanelControl;BackColor=Transparent";
            if (grana == "statusstrip")
                return "StatusStrip;Visible=true";
            return "";
        }

        public static string ComboParametar(bool allowEmpty, bool nemaOdabira)
        {
            if (allowEmpty && nemaOdabira)
                return "null";
            if (nemaOdabira)
                return "1";
            return "";
        }

        public static bool MultidropUbacujePrazanRed()
        {
            return false;
        }

        public static string StatusTekst(string text1)
        {
            return (text1 ?? "").ToUpperInvariant();
        }

        public static bool LabelSaljeParametar()
        {
            return false;
        }

        /// <summary>Atribut koji se ne traži ostaje nepročitan. TREE33, WUDTH, GRUOPBYBOXVISIBLE, DEFALUT.</summary>
        public static string Procitano(IDictionary<string, string> atributi, string trazeno)
        {
            if (atributi == null || trazeno == null)
                return null;
            string v;
            if (atributi.TryGetValue(trazeno, out v))
                return v;
            return null;
        }

        public static string ComboOsvjezavanje(string vrijednostKolone, bool allowEmpty)
        {
            if (vrijednostKolone == null)
                return "";
            if (vrijednostKolone.Trim().ToLowerInvariant() != "refresh")
                return "vrijednost";
            if (allowEmpty)
                return "refresh+allowempty";
            return "refresh";
        }

        public static string TrazilicaOblik(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return "prazno";
            string s = sql.Trim();
            if (s.StartsWith("sp", StringComparison.OrdinalIgnoreCase))
                return "sp";
            if (s.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                return "select";
            return "hash";
        }

        /// <summary>Form3.vb 99–133. Prazan tekst ne gradi SQL.</summary>
        public static string TrazilicaSql(string parametri, string trazeni)
        {
            if (string.IsNullOrWhiteSpace(parametri))
                return "";
            string p = parametri.Trim();
            string oblik = TrazilicaOblik(p);
            if (oblik == "sp" || oblik == "select")
                return p;
            string tekst = (trazeni ?? "").Trim().Replace("'", "");
            if (tekst.Length == 0)
                return "";
            string[] tar2 = p.Split('#');
            string tablica = tar2[0];
            var polja = new StringBuilder();
            var sort = new StringBuilder();
            for (int i = 1; i < tar2.Length; i++)
            {
                string komad = tar2[i] ?? "";
                if (i > 1)
                    sort.Append(komad.Replace("!", "").Replace("%", "")).Append(", ");
                polja.Append(komad.Replace("!", "").Replace("%", "").Replace("desc", "").Replace("asc", "")).Append(", ");
            }
            if (polja.Length < 2 || sort.Length < 2)
                return "";
            string poljaSql = polja.ToString(0, polja.Length - 2);
            string sortSql = sort.ToString(0, sort.Length - 2);
            int kosa = poljaSql.IndexOf("/", StringComparison.OrdinalIgnoreCase);
            if (kosa >= 0 && kosa >= 3)
                poljaSql = poljaSql.Substring(0, kosa - 3);
            kosa = sortSql.IndexOf("/", StringComparison.OrdinalIgnoreCase);
            if (kosa >= 0 && kosa >= 3)
                sortSql = sortSql.Substring(0, kosa - 3);

            string[] rijeci = tekst.Split(' ');
            var sstr = new StringBuilder();
            string separator = "";
            string wstr = "";
            for (int i = 0; i < rijeci.Length; i++)
            {
                var pom = new StringBuilder("(");
                for (int j = 2; j < tar2.Length; j++)
                {
                    string komad = tar2[j] ?? "";
                    if (komad.IndexOf("!", StringComparison.OrdinalIgnoreCase) >= 0)
                        continue;
                    if (komad.IndexOf("/", StringComparison.Ordinal) >= 0)
                    {
                        wstr = komad.Replace("/", "");
                        continue;
                    }
                    string cist = komad.Trim().Replace("desc", "").Replace("asc", "");
                    string lijevo = cist.StartsWith("%", StringComparison.Ordinal) ? "%" : "";
                    string desno = cist.EndsWith("%", StringComparison.Ordinal) ? "%" : "";
                    string stupac = cist.Replace("%", "");
                    pom.Append(stupac).Append(" like '").Append(lijevo).Append(rijeci[i]).Append(desno).Append("' or ");
                }
                if (pom.Length < 4)
                    continue;
                sstr.Append(separator).Append(pom.ToString(0, pom.Length - 3)).Append(" ) ");
                separator = " and ";
            }
            return "select distinct " + poljaSql + " from " + tablica + " where " + sstr + " " + wstr + " order by " + sortSql;
        }

        public sealed class AssemblyIshod
        {
            public string Id = "";
            public string Operater = "";
            public string Konekcija = "";
            public string ZadnjiSql = "";
            public string ParametriGrida = "";
            public bool Prekinuto;
            public int Argumenata;
        }

        /// <summary>Shell totalgen: id, operater, konekcija, zadnji SQL, parametri grida. Form1.vb 4599–4623.</summary>
        public static AssemblyIshod AssemblyShell(string id, string operater, string konekcija, string zadnjiSql, string parametri, bool loadUspio, bool instancaUspjela)
        {
            var i = new AssemblyIshod();
            i.Id = id ?? "";
            i.Operater = operater ?? "";
            i.Konekcija = konekcija ?? "";
            i.ZadnjiSql = zadnjiSql ?? "";
            i.ParametriGrida = parametri ?? "";
            i.Argumenata = 5;
            if (!loadUspio || !instancaUspjela)
                i.Prekinuto = true;
            return i;
        }
    }
}

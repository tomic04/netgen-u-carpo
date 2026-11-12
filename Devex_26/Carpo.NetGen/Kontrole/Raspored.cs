using System;
using System.Collections.Generic;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Kontrole
{
    /// <summary>
    /// Form2.SrediKontrolu (2924–3023). TOP/LEFT/WIDTH/HEIGHT dijele se s 14.
    /// WUDTH se ne čita. TOP 0 ili natpis koji završava na * slaže kontrolu i lijevi natpis.
    /// </summary>
    public sealed class Mjesto
    {
        public int Vrh;
        public int Lijevo;
        public int Sirina;
        public int Visina;
        public string Tekst;
        public bool Natpis;
        public int NatpisVrh;
        public int NatpisLijevo;
        public string NatpisTekst;
        public bool ImaUkljuceno;
        public bool Ukljuceno;
        public bool ImaVidljivo;
        public bool Vidljivo;
        public bool Cekirano;
    }

    public static class Raspored
    {
        public static string Editor(NgGrana grana)
        {
            if (grana == NgGrana.Text)
                return "TextEdit";
            if (grana == NgGrana.Number)
                return "SpinEdit";
            if (grana == NgGrana.Search)
                return "ButtonEdit";
            if (grana == NgGrana.Date)
                return "DateEdit";
            if (grana == NgGrana.Combo)
                return "LookUpEdit";
            if (grana == NgGrana.MultiDrop)
                return "CheckedComboBoxEdit";
            if (grana == NgGrana.Check || grana == NgGrana.Radio)
                return "CheckEdit";
            if (grana == NgGrana.Label || grana == NgGrana.StatusStrip)
                return "LabelControl";
            if (grana == NgGrana.Group)
                return "GroupControl";
            if (grana == NgGrana.Panel)
                return "PanelControl";
            if (grana == NgGrana.Button || grana == NgGrana.Help)
                return "SimpleButton";
            if (grana == NgGrana.PictureBox)
                return "PictureEdit";
            if (grana == NgGrana.WebBrowser)
                return "WebBrowser";
            if (grana == NgGrana.Grid || grana == NgGrana.DxGrid)
                return "GridControl";
            if (grana == NgGrana.Tree)
                return "TreeList";
            if (grana == NgGrana.SplitContainer)
                return "SplitContainerControl";
            return "";
        }

        public static int Broj(string tekst)
        {
            if (string.IsNullOrEmpty(tekst))
                return 0;
            int n;
            if (!int.TryParse(tekst.Trim(), out n))
                return 0;
            return n;
        }

        public static void Velicina(NgAttributeBag atributi, out int sirina, out int visina)
        {
            sirina = 0;
            visina = 0;
            if (atributi == null)
                return;
            if (atributi.Ima("WIDTH"))
                sirina = MasterGrid.VbJedinica(Broj(atributi.Vrijednost("WIDTH")));
            if (atributi.Ima("HEIGHT"))
                visina = MasterGrid.VbJedinica(Broj(atributi.Vrijednost("HEIGHT")));
        }

        public static Mjesto Izracunaj(NgGrana grana, NgAttributeBag a, ref int zadnji)
        {
            var m = new Mjesto();
            string caption = a == null ? "" : (a.Vrijednost("CAPTION") ?? "");
            caption = caption.Trim();
            if (grana == NgGrana.Radio)
                m.Tekst = caption;
            else if (a != null && a.Ima("DEFAULT"))
                m.Tekst = (a.Vrijednost("DEFAULT") ?? "").Trim();
            else
                m.Tekst = caption;
            m.Tekst = SrediTekst(m.Tekst);
            if (a != null && a.Ima("DEFAULT"))
            {
                string d = (a.Vrijednost("DEFAULT") ?? "").Trim().ToLowerInvariant();
                m.Cekirano = d == "1" || d == "true";
            }

            int vrh = a != null && a.Ima("TOP") ? MasterGrid.VbJedinica(Broj(a.Vrijednost("TOP"))) : 0;
            int lijevo = a != null && a.Ima("LEFT") ? MasterGrid.VbJedinica(Broj(a.Vrijednost("LEFT"))) : 0;
            if (a != null && a.Ima("WIDTH"))
                m.Sirina = MasterGrid.VbJedinica(Broj(a.Vrijednost("WIDTH")));
            if (a != null && a.Ima("HEIGHT"))
                m.Visina = MasterGrid.VbJedinica(Broj(a.Vrijednost("HEIGHT")));

            bool slaze = grana != NgGrana.Group && grana != NgGrana.Label && grana != NgGrana.Panel;
            if (slaze && (vrh == 0 || caption.EndsWith("*", StringComparison.Ordinal)))
            {
                if (caption.EndsWith("*", StringComparison.Ordinal))
                    zadnji = vrh;
                else
                {
                    zadnji = zadnji + 30;
                    if (zadnji == 30)
                        zadnji = 10;
                }
                if (lijevo == 0)
                    lijevo = 140;
                vrh = zadnji;
                if (grana != NgGrana.Check && grana != NgGrana.Button)
                {
                    m.Natpis = true;
                    m.NatpisVrh = zadnji + 4;
                    string natpis = caption.EndsWith("*", StringComparison.Ordinal)
                        ? caption.Substring(0, caption.Length - 1).Trim()
                        : caption;
                    natpis = SrediTekst(natpis);
                    m.NatpisTekst = natpis;
                    if (caption.EndsWith("*", StringComparison.Ordinal))
                        m.NatpisLijevo = lijevo - (natpis.Length * 5) - 15;
                    else
                        m.NatpisLijevo = lijevo - 130;
                }
            }
            else if (slaze)
                zadnji = vrh;

            m.Vrh = vrh;
            m.Lijevo = lijevo;
            if (a != null && a.Ima("ENABLED"))
            {
                bool? z = Zastavica(a.Vrijednost("ENABLED"));
                if (z.HasValue)
                {
                    m.ImaUkljuceno = true;
                    m.Ukljuceno = z.Value;
                }
            }
            if (a != null && a.Ima("VISIBLE"))
            {
                bool? z = Zastavica(a.Vrijednost("VISIBLE"));
                if (z.HasValue)
                {
                    m.ImaVidljivo = true;
                    m.Vidljivo = z.Value;
                }
            }
            return m;
        }

        public static string SrediTekst(string tekst)
        {
            return (tekst ?? "").Replace("_", "").Replace("!!", "\r\n");
        }

        /// <summary>FormBorderStyle kao WinForms broj. Nepoznat ili prazan ostaje prazan tekst.</summary>
        public static string Rub(string vrijednost)
        {
            int n;
            if (vrijednost == null || !int.TryParse(vrijednost.Trim(), out n))
                return "";
            if (n == 0)
                return "None";
            if (n == 1)
                return "FixedSingle";
            if (n == 2)
                return "Fixed3D";
            if (n == 3)
                return "FixedDialog";
            if (n == 4)
                return "Sizable";
            if (n == 5)
                return "FixedToolWindow";
            if (n == 6)
                return "SizableToolWindow";
            return "";
        }

        /// <summary>
        /// Ključevi forme koju gumb otvara. Natpis, /14, tab i tekst kontrole kao Form2.SrediKontrolu.
        /// AcceptButton se ne postavlja (Form2.vb 3367).
        /// </summary>
        public static void KljuceviForme(NgForm forma, Dictionary<string, string> s)
        {
            if (forma == null || s == null)
                return;
            string naslov = forma.Atributi == null ? "" : (forma.Atributi.Vrijednost("CAPTION") ?? "");
            s["formaNaslov"] = naslov.Trim();
            int sirina;
            int visina;
            Velicina(forma.Atributi, out sirina, out visina);
            s["formaSirina"] = sirina.ToString();
            s["formaVisina"] = visina.ToString();
            s["acceptButton"] = "";
            s["tab"] = TabRed(forma);
            string onload = (forma.OnLoad ?? "").Trim();
            if (forma.Kontrole == null)
                return;
            for (int i = 0; i < forma.Kontrole.Count; i++)
            {
                NgControl k = forma.Kontrole[i];
                if (k == null || string.IsNullOrEmpty(k.Name))
                    continue;
                int z = 0;
                Mjesto m = Izracunaj(k.Grana, k.Atributi, ref z);
                string prefiks = "kontrola." + k.Name + ".";
                s[prefiks + "tekst"] = m.Tekst ?? "";
                if (k.Atributi != null && k.Atributi.Ima("TOP"))
                    s[prefiks + "top"] = m.Vrh.ToString();
                if (k.Atributi != null && k.Atributi.Ima("LEFT"))
                    s[prefiks + "left"] = m.Lijevo.ToString();
                if (k.Atributi != null && k.Atributi.Ima("SIZE"))
                    s[prefiks + "velicina"] = (k.Atributi.Vrijednost("SIZE") ?? "").Trim();
                if (OnLoadPuni(onload, k.Name) && (k.Grana == NgGrana.Text || k.Grana == NgGrana.Number || k.Grana == NgGrana.Date || k.Grana == NgGrana.Search))
                    s[prefiks + "izOnLoada"] = "true";
            }
        }

        public static string TabRed(NgForm forma)
        {
            if (forma == null)
                return "";
            int cur = -1;
            int brojac = 0;
            var red = new List<string>();
            var indeks = new List<int>();
            var poredak = new List<int>();
            int n = 0;
            if (forma.Kontrole != null)
            {
                for (int i = 0; i < forma.Kontrole.Count; i++)
                {
                    NgControl k = forma.Kontrole[i];
                    if (k == null || string.IsNullOrEmpty(k.Name))
                        continue;
                    bool ukljuceno = Ukljuceno(k.Atributi, "ENABLED");
                    bool vidljivo = Ukljuceno(k.Atributi, "VISIBLE");
                    int idx;
                    bool uRedu;
                    int tab;
                    if (k.Atributi != null && k.Atributi.Ima("TABINDEX") && int.TryParse((k.Atributi.Vrijednost("TABINDEX") ?? "").Trim(), out tab))
                    {
                        idx = tab;
                        uRedu = tab >= 0;
                    }
                    else
                    {
                        cur = cur + 1;
                        idx = cur;
                        uRedu = true;
                    }
                    if (!ukljuceno)
                        uRedu = false;
                    if (uRedu)
                    {
                        red.Add(k.Name);
                        indeks.Add(idx);
                        poredak.Add(n);
                        n++;
                    }
                    if (BrojiTab(k.Grana) && ukljuceno && vidljivo)
                        brojac++;
                }
            }
            if (forma.Gumbi != null)
            {
                for (int i = 0; i < forma.Gumbi.Count; i++)
                {
                    string natpis = forma.Gumbi[i].Natpis ?? "";
                    if (natpis.Length == 0)
                        continue;
                    red.Add(natpis);
                    indeks.Add(brojac + i + 1);
                    poredak.Add(n);
                    n++;
                }
            }
            for (int a = 0; a < red.Count; a++)
            {
                for (int b = a + 1; b < red.Count; b++)
                {
                    if (indeks[b] < indeks[a] || (indeks[b] == indeks[a] && poredak[b] < poredak[a]))
                    {
                        string ts = red[a];
                        red[a] = red[b];
                        red[b] = ts;
                        int ti = indeks[a];
                        indeks[a] = indeks[b];
                        indeks[b] = ti;
                        int tp = poredak[a];
                        poredak[a] = poredak[b];
                        poredak[b] = tp;
                    }
                }
            }
            return string.Join(",", red.ToArray());
        }

        private static bool OnLoadPuni(string onload, string ime)
        {
            if (string.IsNullOrEmpty(onload) || string.IsNullOrEmpty(ime))
                return false;
            string s = onload.ToLowerInvariant();
            if (s.IndexOf("select", StringComparison.Ordinal) != 0 && s.TrimStart().IndexOf("select", StringComparison.Ordinal) != 0)
                return false;
            s = s.TrimStart();
            if (s.StartsWith("select *", StringComparison.Ordinal) || s.StartsWith("select*", StringComparison.Ordinal))
                return true;
            return ImeUSql(s, ime.ToLowerInvariant());
        }

        private static bool ImeUSql(string sql, string ime)
        {
            int od = 0;
            while (od < sql.Length)
            {
                int i = sql.IndexOf(ime, od, StringComparison.Ordinal);
                if (i < 0)
                    return false;
                bool prije = i == 0 || !Slovo(sql[i - 1]);
                int kraj = i + ime.Length;
                bool poslije = kraj >= sql.Length || !Slovo(sql[kraj]);
                if (prije && poslije)
                    return true;
                od = i + 1;
            }
            return false;
        }

        private static bool Slovo(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_';
        }

        private static bool Ukljuceno(NgAttributeBag a, string ime)
        {
            if (a == null || !a.Ima(ime))
                return true;
            bool? z = Zastavica(a.Vrijednost(ime));
            return !z.HasValue || z.Value;
        }

        private static bool BrojiTab(NgGrana grana)
        {
            return grana != NgGrana.Label && grana != NgGrana.Group && grana != NgGrana.Panel;
        }

        private static bool? Zastavica(string v)
        {
            if (v == null)
                return null;
            string t = v.Trim().ToLowerInvariant();
            if (t == "true")
                return true;
            if (t == "false")
                return false;
            return null;
        }
    }
}

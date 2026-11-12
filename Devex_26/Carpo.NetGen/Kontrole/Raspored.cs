using System;
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

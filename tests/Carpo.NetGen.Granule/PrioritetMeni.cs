using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Read-only popis ssMeni asm:NetGenDLL.loader / frm i rang po stablu, detailu i raznolikosti.
    /// </summary>
    public static class PrioritetMeni
    {
        public static string Tekst;

        private static readonly string[] Pokrivene =
        {
            "OznakaKL.xml", "JeziciNG.xml", "Statusi.xml", "KntBlaZak.xml", "PrijemniList.xml",
            "Kulture.xml", "KarticaPartnera.xml", "KUPNG.xml", "KntDospjeleStavke.xml",
            "MPInventura.xml", "FakPar.xml", "MPNarIzdSkl.xml", "DobUgovori.xml",
            "MPNGPregDokMAIL.xml", "KUPDostKN.xml", "KUPDostKnNGBvg.xml", "MPBlagajna.xml",
            "AOPNovi.xml", "RatArkod.xml", "Narudzbe.xml", "RobaAtributiUredi.xml"
        };

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string greska;
            string sql = "select Parametar from ssMeni where Parametar like 'asm:%'";
            List<string> svi = Citanje.StupacDo(sql, 2000, out greska);
            if (svi == null)
            {
                Tekst = "PRIORITET greska " + greska;
                l.Add(Qa.O("Prioritet", "MENI-01", "ssMeni loader/frm", "dbo.ssMeni", greska));
                return l;
            }

            var redovi = new List<string>();
            for (int i = 0; i < svi.Count; i++)
            {
                string p = svi[i] ?? "";
                string low = p.ToLowerInvariant();
                if (low.StartsWith("asm:netgendll.loader:") || low.StartsWith("asm:netgendll.frm:"))
                    redovi.Add(p);
            }

            var indeks = Indeks(Path.Combine(root, "definitions", "netgen"));
            var rang = new List<Red>();
            for (int i = 0; i < redovi.Count; i++)
            {
                string par = redovi[i];
                string fajl = ImeFajla(par);
                string put = null;
                indeks.TryGetValue(fajl.ToLowerInvariant(), out put);
                var r = new Red();
                r.Parametar = par;
                r.Fajl = fajl;
                r.Putanja = put;
                r.Pokrivana = Pokrivena(fajl);
                if (put != null)
                    Ocijeni(put, r);
                rang.Add(r);
            }
            rang.Sort(Usporedi);

            var sb = new StringBuilder();
            int loader = 0;
            int frm = 0;
            for (int i = 0; i < rang.Count; i++)
            {
                if (rang[i].Parametar.ToLowerInvariant().Contains(".frm:"))
                    frm++;
                else
                    loader++;
            }
            sb.AppendLine("PRIORITET n=" + rang.Count + " loader=" + loader + " frm=" + frm);
            int nepokr = 0;
            for (int i = 0; i < rang.Count; i++)
            {
                Red r = rang[i];
                if (!r.Pokrivana)
                    nepokr++;
                sb.Append("PRIORITET");
                sb.Append('\t');
                sb.Append(i + 1);
                sb.Append('\t');
                sb.Append(r.Pokrivana ? "da" : "ne");
                sb.Append('\t');
                sb.Append(r.Stablo ? "stablo" : "-");
                sb.Append('\t');
                sb.Append(r.Detail ? "detail" : "-");
                sb.Append('\t');
                sb.Append(r.Bod);
                sb.Append('\t');
                sb.Append(r.Fajl);
                sb.Append('\t');
                sb.Append(r.Parametar);
                sb.Append('\t');
                sb.Append(r.Elementi);
                sb.Append('\t');
                sb.Append(r.Putanja ?? "nema-xml");
                sb.AppendLine();
            }
            sb.AppendLine("PRIORITET nepokriveno=" + nepokr);
            Tekst = sb.ToString();

            l.Add(Qa.Z("Prioritet", "MENI-01", "ssMeni NetGenDLL loader+frm redova " + rang.Count + " (loader " + loader + ", frm " + frm + ")", "dbo.ssMeni"));
            l.Add(Qa.Z("Prioritet", "MENI-02", "Nepokrivenih checklistom " + nepokr, "docs/granule"));
            return l;
        }

        private static int Usporedi(Red a, Red b)
        {
            int c = (b.Stablo && b.Detail ? 1 : 0) - (a.Stablo && a.Detail ? 1 : 0);
            if (c != 0)
                return c;
            c = (b.Stablo ? 1 : 0) - (a.Stablo ? 1 : 0);
            if (c != 0)
                return c;
            c = (b.Detail ? 1 : 0) - (a.Detail ? 1 : 0);
            if (c != 0)
                return c;
            c = b.Bod - a.Bod;
            if (c != 0)
                return c;
            return string.Compare(a.Fajl, b.Fajl, StringComparison.OrdinalIgnoreCase);
        }

        private static bool Pokrivena(string fajl)
        {
            for (int i = 0; i < Pokrivene.Length; i++)
            {
                if (string.Equals(Pokrivene[i], fajl, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static string ImeFajla(string parametar)
        {
            int c = parametar.LastIndexOf(':');
            string rest = c >= 0 ? parametar.Substring(c + 1) : parametar;
            int s = Math.Max(rest.LastIndexOf('\\'), rest.LastIndexOf('/'));
            if (s >= 0)
                rest = rest.Substring(s + 1);
            return rest.Trim();
        }

        private static Dictionary<string, string> Indeks(string korijen)
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists(korijen))
                return d;
            string[] svi = Directory.GetFiles(korijen, "*.xml", SearchOption.AllDirectories);
            for (int i = 0; i < svi.Length; i++)
            {
                string ime = Path.GetFileName(svi[i]);
                if (!d.ContainsKey(ime))
                    d[ime] = svi[i];
            }
            return d;
        }

        private static void Ocijeni(string put, Red r)
        {
            string t = Rijetke.Procitaj(put);
            string low = t.ToLowerInvariant();
            bool treeAttr = ImaAtribut(low, "tree=") && !low.Contains("tree=\"\"") && !low.Contains("tree=''");
            bool combo = ImaAtribut(low, "combo=");
            r.Stablo = (treeAttr && VrijednostNijePrazna(low, "tree")) || (combo && VrijednostNijePrazna(low, "combo"));
            r.Detail = VrijednostNijePrazna(low, "detailgrid");
            var el = new List<string>();
            if (r.Stablo)
                el.Add("stablo");
            if (r.Detail)
                el.Add("detail");
            Dodaj(el, low, "setdefaults=", "setdefaults");
            Dodaj(el, low, "treetranslate=", "treetranslate");
            Dodaj(el, low, "buttons=", "buttons");
            Dodaj(el, low, "onload=\":", "onload-forma");
            if (low.Contains("printmaster") || low.Contains("printrepx"))
                el.Add("print");
            if (low.Contains("repx/"))
                el.Add("repx");
            if (low.Contains("assemblyload"))
                el.Add("assemblyload");
            if (low.Contains("multiselect"))
                el.Add("multiselect");
            if (low.Contains("foreach"))
                el.Add("foreach");
            if (low.Contains("type=\"search\"") || low.Contains("type=\"trazilica\"") || low.Contains("type='search'"))
                el.Add("search");
            if (low.Contains("type=\"combo\"") || low.Contains("type='combo'"))
                el.Add("combo");
            if (low.Contains("type=\"date\"") || low.Contains("type=\"datetime\"") || low.Contains("type='date'"))
                el.Add("date");
            if (low.Contains("type=\"number\"") || low.Contains("type=\"cur\"") || low.Contains("type='number'"))
                el.Add("number");
            if (low.Contains("type=\"check\"") || low.Contains("type=\"checkbox\"") || low.Contains("type='check'"))
                el.Add("check");
            if (low.Contains("type=\"radio\"") || low.Contains("type='radio'"))
                el.Add("radio");
            if (low.Contains("type=\"multidrop\"") || low.Contains("type='multidrop'"))
                el.Add("multidrop");
            if (low.Contains("type=\"grid\"") || low.Contains("type='grid'"))
                el.Add("grid-dijalog");
            if (low.Contains("type=\"tree\"") || low.Contains("type='tree'"))
                el.Add("tree-dijalog");
            if (low.Contains("nextform="))
                el.Add("nextform");
            if (low.Contains("dbclipboard="))
                el.Add("clipboard");
            r.Bod = el.Count + (r.Stablo && r.Detail ? 10 : 0);
            r.Elementi = string.Join(",", el.ToArray());
        }

        private static void Dodaj(List<string> el, string low, string kljuc, string ime)
        {
            if (low.Contains(kljuc))
                el.Add(ime);
        }

        private static bool ImaAtribut(string low, string kljuc)
        {
            return low.Contains(kljuc);
        }

        private static bool VrijednostNijePrazna(string low, string ime)
        {
            int i = 0;
            string kljuc = ime + "=\"";
            while ((i = low.IndexOf(kljuc, i, StringComparison.Ordinal)) >= 0)
            {
                int q = low.IndexOf('"', i + kljuc.Length);
                if (q > i + kljuc.Length)
                    return true;
                i += kljuc.Length;
            }
            return false;
        }

        private sealed class Red
        {
            public string Parametar;
            public string Fajl;
            public string Putanja;
            public bool Pokrivana;
            public bool Stablo;
            public bool Detail;
            public int Bod;
            public string Elementi = "";
        }
    }
}

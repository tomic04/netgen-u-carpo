using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Corpus
{
    /// <summary>
    /// Ucitava svaki NetGen XML iz zadanih mapa. Pad jednog fajla ne prekida ostale.
    /// Invarijante (OznakaKL, JeziciNG, HIGHT, TYPE grana, UTF-16) jesu greska parsera i vrate kod 1.
    /// </summary>
    internal static class Program
    {
        private static readonly Dictionary<string, NgGrana> OcekivanaGrana = NapraviOcekivano();

        private static int Main(string[] args)
        {
            var mape = new List<string>();
            string izvjestaj = null;
            string tsv = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--izvjestaj" && i + 1 < args.Length)
                {
                    izvjestaj = args[++i];
                    continue;
                }
                if (args[i] == "--tsv" && i + 1 < args.Length)
                {
                    tsv = args[++i];
                    continue;
                }
                mape.Add(args[i]);
            }

            if (mape.Count == 0)
            {
                Console.Error.WriteLine("Upotreba: Carpo.NetGen.Corpus.exe <mapa> [mapa...] [--izvjestaj datoteka] [--tsv datoteka]");
                return 2;
            }

            var fajlovi = new List<string>();
            for (int i = 0; i < mape.Count; i++)
            {
                if (!Directory.Exists(mape[i]))
                {
                    Console.Error.WriteLine("Nema mape: " + mape[i]);
                    return 2;
                }
                fajlovi.AddRange(Directory.GetFiles(mape[i], "*.xml", SearchOption.AllDirectories));
            }
            fajlovi.Sort(StringComparer.OrdinalIgnoreCase);

            int parsed = 0;
            int failed = 0;
            int input = 0;
            int forme = 0;
            int stablo = 0;
            int detail = 0;
            int utf16 = 0;
            int utf16Pao = 0;
            var padovi = new List<string>();
            var invarijante = new List<string>();
            var tsvRedovi = new List<string>();
            tsvRedovi.Add("putanja\tstatus\tforme\tkontrole\tgrane\tstablo\tdetail\tgreska");

            bool vidjenOznaka = false;
            bool vidjenJezici = false;

            for (int i = 0; i < fajlovi.Count; i++)
            {
                string putanja = fajlovi[i];
                string imeFajla = Path.GetFileName(putanja);
                bool jeOznaka = string.Equals(imeFajla, "OznakaKL.xml", StringComparison.OrdinalIgnoreCase);
                bool jeJezici = string.Equals(imeFajla, "JeziciNG.xml", StringComparison.OrdinalIgnoreCase);
                if (jeOznaka)
                    vidjenOznaka = true;
                if (jeJezici)
                    vidjenJezici = true;
                bool bom16 = JeUtf16(putanja);
                if (bom16)
                    utf16++;

                NgLoadResult r;
                try
                {
                    r = NetGenXmlReader.Ucitaj(putanja);
                }
                catch (Exception ex)
                {
                    r = NgLoadResult.Neuspjelo(putanja, NetGenXmlReader.Poruka(putanja, ex.Message));
                }

                if (!r.Uspjeh)
                {
                    failed++;
                    if (bom16)
                        utf16Pao++;
                    string razlog = JedanRed(r.Greska);
                    padovi.Add(putanja + "\t" + razlog);
                    if (jeOznaka || jeJezici)
                        invarijante.Add(putanja + "\t" + razlog);
                    tsvRedovi.Add(Tsv(putanja, "greska", 0, 0, "", "ne", "ne", razlog));
                    continue;
                }

                parsed++;
                var p = r.Projekt;
                input += p.BrojKontrola();
                forme += p.Forme.Count;
                if (p.Raspored.ImaStablo)
                    stablo++;
                if (p.Raspored.ImaDetail)
                    detail++;

                string greskeInvarijante = Provjeri(putanja, p);
                if (greskeInvarijante != null)
                    invarijante.Add(putanja + "\t" + greskeInvarijante);

                if (jeOznaka)
                {
                    string o = ProvjeriOznaka(p);
                    if (o != null)
                        invarijante.Add(putanja + "\t" + o);
                }
                if (jeJezici)
                {
                    string j = ProvjeriJezici(p);
                    if (j != null)
                        invarijante.Add(putanja + "\t" + j);
                }

                tsvRedovi.Add(Tsv(
                    putanja,
                    "ok",
                    p.Forme.Count,
                    p.BrojKontrola(),
                    Grane(p),
                    p.Raspored.ImaStablo ? "da" : "ne",
                    p.Raspored.ImaDetail ? "da" : "ne",
                    ""));
            }

            if (!vidjenOznaka)
                invarijante.Add("(korpus)\tNema OznakaKL.xml");
            if (!vidjenJezici)
                invarijante.Add("(korpus)\tNema JeziciNG.xml");
            if (utf16Pao > 0)
                invarijante.Add("(korpus)\tUTF-16 BOM nije prosao XmlDocument.Load: " + utf16Pao.ToString());
            if (fajlovi.Count == 0)
                invarijante.Add("(korpus)\tNema XML fajlova");

            var sb = new StringBuilder();
            sb.AppendLine("PARSED=" + parsed.ToString());
            sb.AppendLine("FAILED=" + failed.ToString());
            sb.AppendLine("FILES=" + fajlovi.Count.ToString());
            sb.AppendLine("INPUT=" + input.ToString());
            sb.AppendLine("FORME=" + forme.ToString());
            sb.AppendLine("STABLO=" + stablo.ToString());
            sb.AppendLine("DETAIL=" + detail.ToString());
            sb.AppendLine("UTF16=" + utf16.ToString());
            sb.AppendLine("OBRAZAC=" + Carpo.NetGen.Shell.ShellLayout.Obrazac);
            sb.AppendLine("INVARIJANTE=" + invarijante.Count.ToString());
            sb.AppendLine("FAILED_LIST=");
            for (int i = 0; i < padovi.Count; i++)
                sb.AppendLine(padovi[i]);
            sb.AppendLine("INVARIANT_LIST=");
            for (int i = 0; i < invarijante.Count; i++)
                sb.AppendLine(invarijante[i]);

            string tekst = sb.ToString();
            Console.Write(tekst);
            if (!string.IsNullOrEmpty(izvjestaj))
            {
                OsigurajMapu(izvjestaj);
                File.WriteAllText(izvjestaj, tekst, new UTF8Encoding(false));
            }
            if (!string.IsNullOrEmpty(tsv))
            {
                OsigurajMapu(tsv);
                File.WriteAllLines(tsv, tsvRedovi.ToArray(), new UTF8Encoding(false));
            }

            return invarijante.Count == 0 ? 0 : 1;
        }

        private static string Provjeri(string putanja, NgProject p)
        {
            var problemi = new List<string>();
            if (p.Raspored == null || p.Raspored.ObrazacIme != Carpo.NetGen.Shell.ShellLayout.Obrazac)
                problemi.Add("raspored nije " + Carpo.NetGen.Shell.ShellLayout.Obrazac);

            Dictionary<string, int> xmlHist;
            int xmlInput;
            try
            {
                var doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.Load(putanja);
                xmlHist = Histogram(doc);
                xmlInput = doc.GetElementsByTagName("INPUT").Count;
            }
            catch (Exception ex)
            {
                return "ponovni Load za usporedbu: " + ex.Message;
            }

            if (p.BrojElemenata("INPUT") != xmlInput || p.BrojKontrola() != xmlInput)
                problemi.Add("INPUT model=" + p.BrojKontrola().ToString() + " elementi=" + p.BrojElemenata("INPUT").ToString() + " xml=" + xmlInput.ToString());

            var modelHist = p.HistogramAtributa();
            if (!IstiHistogram(xmlHist, modelHist, problemi))
                problemi.Add("histogram atributa se ne poklapa (HIGHT/HEIGHT nisu smjeli promijeniti ime)");

            ProvjeriGrane(p, problemi);

            if (problemi.Count == 0)
                return null;
            return string.Join("; ", problemi.ToArray());
        }

        private static void ProvjeriGrane(NgProject p, List<string> problemi)
        {
            for (int i = 0; i < p.Forme.Count; i++)
                ProvjeriListu(p.Forme[i].Kontrole, problemi);
            ProvjeriListu(p.KontroleIzvanForme, problemi);
        }

        private static void ProvjeriListu(List<NgControl> lista, List<string> problemi)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                var k = lista[i];
                if (string.IsNullOrEmpty(k.TypeRaw))
                {
                    if (k.Grana != NgGrana.Nijedna)
                        problemi.Add("prazan TYPE dobio granu " + k.Grana.ToString());
                    continue;
                }
                string kljuc = k.TypeRaw.Trim().ToLowerInvariant();
                NgGrana ocekivano;
                if (!OcekivanaGrana.TryGetValue(kljuc, out ocekivano))
                    continue;
                if (k.Grana != ocekivano)
                    problemi.Add("TYPE '" + k.TypeRaw + "' grana " + k.Grana.ToString() + " umjesto " + ocekivano.ToString());
            }
        }

        private static string ProvjeriOznaka(NgProject p)
        {
            var problemi = new List<string>();
            if (p.Forme.Count != 1 || p.Forme[0].Ime != "FRMEDIT")
                problemi.Add("ocekuje se jedna forma FRMEDIT");
            var k = p.NadjiKontrolu("oznaka2");
            if (k == null || k.Grana != NgGrana.Text)
                problemi.Add("oznaka2 nije text");
            var forma = p.NadjiFormu("FRMEDIT");
            if (forma == null || !forma.OnSubmit.TrimStart().StartsWith("update", StringComparison.OrdinalIgnoreCase))
                problemi.Add("ONSUBMIT ne pocinje s update");
            if (!p.OnLoad.TrimStart().StartsWith("select", StringComparison.OrdinalIgnoreCase))
                problemi.Add("STARTUPSTORA nije prezivio (nema ONLOAD)");
            if (problemi.Count == 0)
                return null;
            return "OznakaKL: " + string.Join("; ", problemi.ToArray());
        }

        private static string ProvjeriJezici(NgProject p)
        {
            var problemi = new List<string>();
            var g = p.NadjiGrupu("MASTERCOLUMNS");
            if (g == null || g.ClearColums == null)
                problemi.Add("CLEARCOLUMS nije procitan");
            else if (!string.Equals(g.ClearColums, "false", StringComparison.OrdinalIgnoreCase))
                problemi.Add("CLEARCOLUMS='" + g.ClearColums + "'");

            bool vidjenDvaG = false;
            if (g != null)
            {
                for (int i = 0; i < g.Kolone.Count; i++)
                {
                    var c = g.Kolone[i];
                    if (!c.ImaDvostrukoG)
                        continue;
                    vidjenDvaG = true;
                    if (c.Atributi.Ima("AGREGATEFUNCTION"))
                        continue;
                    if (c.AgregateFunction != null)
                        problemi.Add("AGGREGATEFUNCTION (dva G) vezan je na agregat kod " + (c.Name ?? ""));
                }
            }
            if (!vidjenDvaG)
                problemi.Add("nema kolone s AGGREGATEFUNCTION (dva G)");
            if (problemi.Count == 0)
                return null;
            return "JeziciNG: " + string.Join("; ", problemi.ToArray());
        }

        private static bool IstiHistogram(Dictionary<string, int> xml, Dictionary<string, int> model, List<string> problemi)
        {
            bool ok = true;
            foreach (var kv in xml)
            {
                int n;
                model.TryGetValue(kv.Key, out n);
                if (n != kv.Value)
                {
                    ok = false;
                    if (problemi.Count < 8)
                        problemi.Add("atribut " + kv.Key + " xml=" + kv.Value.ToString() + " model=" + n.ToString());
                }
            }
            foreach (var kv in model)
            {
                if (!xml.ContainsKey(kv.Key))
                {
                    ok = false;
                    if (problemi.Count < 8)
                        problemi.Add("model ima visak atributa " + kv.Key + "=" + kv.Value.ToString());
                }
            }
            return ok;
        }

        private static Dictionary<string, int> Histogram(XmlDocument doc)
        {
            var d = new Dictionary<string, int>(StringComparer.Ordinal);
            var cvorovi = doc.GetElementsByTagName("*");
            for (int i = 0; i < cvorovi.Count; i++)
            {
                var el = (XmlElement)cvorovi[i];
                foreach (XmlAttribute a in el.Attributes)
                {
                    int n;
                    d.TryGetValue(a.Name, out n);
                    d[a.Name] = n + 1;
                }
            }
            return d;
        }

        private static string Grane(NgProject p)
        {
            var d = new Dictionary<string, int>(StringComparer.Ordinal);
            DodajGrane(p.KontroleIzvanForme, d);
            for (int i = 0; i < p.Forme.Count; i++)
                DodajGrane(p.Forme[i].Kontrole, d);
            var sb = new StringBuilder();
            foreach (var kv in d)
            {
                if (sb.Length > 0)
                    sb.Append(',');
                sb.Append(kv.Key);
                sb.Append('=');
                sb.Append(kv.Value.ToString());
            }
            return sb.ToString();
        }

        private static void DodajGrane(List<NgControl> lista, Dictionary<string, int> d)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                string key = lista[i].Grana.ToString();
                int n;
                d.TryGetValue(key, out n);
                d[key] = n + 1;
            }
        }

        private static void OsigurajMapu(string putanja)
        {
            string dir = Path.GetDirectoryName(Path.GetFullPath(putanja));
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
        }

        private static bool JeUtf16(string putanja)
        {
            byte[] b = new byte[2];
            using (var fs = File.OpenRead(putanja))
            {
                if (fs.Read(b, 0, 2) < 2)
                    return false;
            }
            return (b[0] == 0xFF && b[1] == 0xFE) || (b[0] == 0xFE && b[1] == 0xFF);
        }

        private static string JedanRed(string tekst)
        {
            if (tekst == null)
                return "";
            return tekst.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
        }

        private static string Tsv(string putanja, string status, int forme, int kontrole, string grane, string stablo, string detail, string greska)
        {
            return putanja + "\t" + status + "\t" + forme.ToString() + "\t" + kontrole.ToString()
                + "\t" + (grane ?? "") + "\t" + stablo + "\t" + detail + "\t" + (greska ?? "");
        }

        private static Dictionary<string, NgGrana> NapraviOcekivano()
        {
            var d = new Dictionary<string, NgGrana>(StringComparer.Ordinal);
            d["b_utton"] = NgGrana.Nijedna;
            d["t1ext"] = NgGrana.Nijedna;
            d["bxutton"] = NgGrana.Nijedna;
            d["g_rid"] = NgGrana.Nijedna;
            d["b1utton"] = NgGrana.Nijedna;
            d["buttton"] = NgGrana.Nijedna;
            d["datetiem"] = NgGrana.Date;
            d["tekst"] = NgGrana.Nijedna;
            d["picture"] = NgGrana.Nijedna;
            d["int"] = NgGrana.Nijedna;
            d["datetime"] = NgGrana.Date;
            d["cmeditvb6.curmedit"] = NgGrana.Number;
            d["vb.checkbox"] = NgGrana.Check;
            d["checkbox"] = NgGrana.Check;
            d["vb.textbox"] = NgGrana.Text;
            d["tmedit2vb6.textmedit2"] = NgGrana.Text;
            d["cbex3.comboboxex3"] = NgGrana.Combo;
            d["buttonnevrijedi"] = NgGrana.Button;
            d["label"] = NgGrana.Label;
            d["button"] = NgGrana.Button;
            d["text"] = NgGrana.Text;
            d["number"] = NgGrana.Number;
            d["combo"] = NgGrana.Combo;
            d["date"] = NgGrana.Date;
            d["grid"] = NgGrana.Grid;
            d["dxgrid"] = NgGrana.DxGrid;
            d["search"] = NgGrana.Search;
            d["frame"] = NgGrana.Group;
            d["check"] = NgGrana.Check;
            d["group"] = NgGrana.Group;
            d["radio"] = NgGrana.Radio;
            d["trazilica"] = NgGrana.Search;
            d["multidrop"] = NgGrana.MultiDrop;
            d["tree"] = NgGrana.Tree;
            d["panel"] = NgGrana.Panel;
            d["webbrowser"] = NgGrana.WebBrowser;
            d["picturebox"] = NgGrana.PictureBox;
            d["statusstrip"] = NgGrana.StatusStrip;
            d["help"] = NgGrana.Help;
            return d;
        }
    }
}

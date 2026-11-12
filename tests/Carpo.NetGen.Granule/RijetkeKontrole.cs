using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Rijetke kontrole: tree na dijalogu, webbrowser, picturebox, NEXTFORM,
    /// clipboard, SETDEFAULTS, TREETRANSLATE, grid na formi.
    /// </summary>
    internal static class Rijetke
    {
        public static readonly string[] TreeXml =
        {
            "Agronom granule/ManagerCjenika.xml",
            "Agronom granule/AOPNovi.xml",
            "BIOVEGA/MPDodKlasaTransportno.xml",
            "BIOVEGA/MPOptimalneZaliheNEWNC0.xml",
            "BIOVEGA/JOPPD.xml",
            "BIOVEGA/MPRobaNarPakovanje.xml",
            "BIOVEGA/MPOptimalneZaliheNEWv2.xml",
            "BIOVEGA/MPOptimalneZaliheNEW.xml",
            "BIOVEGA/AOPNovi.xml",
            "BIOVEGA/RobaWebKlase.xml",
            "BIOVEGA/MPDodKlasaMinKol.xml"
        };

        public static readonly string[] WebXml =
        {
            "Agronom granule/RatArkod.xml",
            "Agronom granule/STStadoNG.xml",
            "BIOVEGA/RatArkod.xml"
        };

        public static readonly string[] PicXml =
        {
            "Agronom granule/HRCKadManagerVanjski.xml",
            "BIOVEGA/RobaAtributiUredi.xml"
        };

        public static readonly string[] PicImena =
        {
            "BlaBSUnos.xml",
            "BlaBSUnosPN.xml",
            "BlaBSUnosFISK.xml"
        };

        public static bool ImaTip(string tekst, string tip)
        {
            if (tekst == null)
                return false;
            return tekst.IndexOf("TYPE=\"" + tip + "\"", StringComparison.OrdinalIgnoreCase) >= 0
                || tekst.IndexOf("TYPE='" + tip + "'", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string Procitaj(string putanja)
        {
            byte[] raw = File.ReadAllBytes(putanja);
            if (raw.Length >= 2 && raw[0] == 0xFF && raw[1] == 0xFE)
                return System.Text.Encoding.Unicode.GetString(raw);
            if (raw.Length >= 3 && raw[0] == 0xEF && raw[1] == 0xBB && raw[2] == 0xBF)
                return System.Text.Encoding.UTF8.GetString(raw, 3, raw.Length - 3);
            try
            {
                return new System.Text.UTF8Encoding(false, true).GetString(raw);
            }
            catch (DecoderFallbackException)
            {
                return System.Text.Encoding.GetEncoding(1250).GetString(raw);
            }
        }

        public static void K(List<Stavka> l, string granula, string xml, string id, string opis, string izvor, string kljuc, string ocek)
        {
            l.Add(Qa.Kljuc(granula, id, opis, izvor, xml, "otvori", "", kljuc, ocek));
        }
    }

    public static class RijetkeKontrole
    {
        public const string Ime = "Rijetke";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            l.Add(Svi(root, "RIJ-01", Rijetke.TreeXml, "tree")
                ? Qa.Z(Ime, "RIJ-01", "11 XML-ova ima INPUT TYPE=tree", "netgen/Form2.vb:2558")
                : Qa.P(Ime, "RIJ-01", "katalog stabla", "netgen/Form2.vb:2558", "nedostaje"));
            l.Add(Svi(root, "RIJ-02", Rijetke.WebXml, "webbrowser")
                ? Qa.Z(Ime, "RIJ-02", "3 XML-a imaju webbrowser", "netgen/Form2.vb:1414")
                : Qa.P(Ime, "RIJ-02", "katalog web", "netgen/Form2.vb:1414", "nedostaje"));
            l.Add(Svi(root, "RIJ-03", Rijetke.PicXml, "picturebox") && SviPoImenu(root, Rijetke.PicImena, "picturebox")
                ? Qa.Z(Ime, "RIJ-03", "5 XML-ova ima picturebox", "netgen/Form2.vb:2707")
                : Qa.P(Ime, "RIJ-03", "katalog slike", "netgen/Form2.vb:2707", "nedostaje"));

            string aop = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "AOPNovi.xml");
            NgProject pa = Ucitaj(l, "RIJ-10", aop);
            if (pa != null)
            {
                NgControl t = NadjiGranu(pa, NgGrana.Tree);
                string sql = t == null ? "" : (Kontrole.A(t, "SQL") ?? "");
                int dv = sql.IndexOf(':');
                string lijevo = dv < 0 ? "" : sql.Substring(0, dv).ToLowerInvariant();
                l.Add(t != null && t.Name == "leftgrid" && lijevo.Contains("tree") && sql.EndsWith("LIJEVIGRIDFORMA", StringComparison.Ordinal)
                    ? Qa.Z(Ime, "RIJ-11", "leftgrid: lijevo od : sadrzi tree, desno je LIJEVIGRIDFORMA", "netgen/Form2.vb:2575-2579,2825")
                    : Qa.P(Ime, "RIJ-11", "AOP stablo", "netgen/Form2.vb:2578", sql));
            }

            string webk = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "RobaWebKlase.xml");
            NgProject pw = Ucitaj(l, "RIJ-12", webk);
            if (pw != null)
            {
                NgForm roba = pw.NadjiFormu("ROBA");
                NgControl st = Kontrole.Nadji(roba, "KlRobaWebKlasaId");
                NgControl g1 = Kontrole.Nadji(roba, "klasa_da");
                l.Add(st != null && st.Grana == NgGrana.Tree && string.Equals(st.TypeRaw, "TREE", StringComparison.Ordinal)
                    && (Kontrole.A(st, "SQL") ?? "").IndexOf("spKlRobaWebKlase_Tree:PUNI", StringComparison.Ordinal) >= 0
                    ? Qa.Z(Ime, "RIJ-13", "TYPE=TREE je stablo. Tag je stupac 3 bez :PUNI. Parametar je cijeli tag", "netgen/Form2.vb:2558,2864,3667-3671")
                    : Qa.P(Ime, "RIJ-13", "RobaWebKlase", "netgen/Form2.vb:2864", st == null ? "" : st.TypeRaw));
                l.Add(g1 != null && g1.Grana == NgGrana.Grid && string.Equals(Kontrole.A(g1, "CHECKBOXVISIBLE"), "FALSE", StringComparison.OrdinalIgnoreCase)
                    ? Qa.Z(Ime, "RIJ-14", "Grid na dijalogu: CHECKBOXVISIBLE FALSE sakrije stupac 1", "netgen/Form2.vb:1623,1752-1757")
                    : Qa.P(Ime, "RIJ-14", "grid dijalog", "netgen/Form2.vb:1754", ""));
            }

            string ark = Path.Combine(root, "definitions", "netgen", "Agronom granule", "RatArkod.xml");
            NgProject pr = Ucitaj(l, "RIJ-15", ark);
            if (pr != null)
            {
                NgForm f = pr.NadjiFormu("WEBBROWSER");
                NgControl wb = Kontrole.Nadji(f, "wb");
                l.Add(wb != null && wb.Grana == NgGrana.WebBrowser && Kontrole.A(wb, "URL") == null
                    && (Ispis.Atr(f, "ONLOAD") ?? "").IndexOf("preglednik.arkod.hr", StringComparison.Ordinal) >= 0
                    ? Qa.Z(Ime, "RIJ-16", "Nema URL. Adresa je kolona wb iz ONLOAD. URL bi isao ToUpper", "netgen/Form2.vb:1419-1433")
                    : Qa.P(Ime, "RIJ-16", "webbrowser", "netgen/Form2.vb:1431", ""));
            }

            string pic = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "RobaAtributiUredi.xml");
            NgProject pp = Ucitaj(l, "RIJ-17", pic);
            if (pp != null)
            {
                NgControl sl = NadjiIme(pp, "slikalocation");
                l.Add(sl != null && sl.Grana == NgGrana.PictureBox && Kontrole.A(sl, "LOCATION") == null
                    ? Qa.Z(Ime, "RIJ-18", "picturebox bez LOCATION: slika je kolona slikalocation. http zove Load", "netgen/Form2.vb:2710-2726")
                    : Qa.P(Ime, "RIJ-18", "picturebox", "netgen/Form2.vb:2710", ""));
            }
            string bla = NadjiPoImenu(Path.Combine(root, "definitions", "netgen"), "BlaBSUnos.xml");
            if (bla != null)
            {
                string tx = Rijetke.Procitaj(bla);
                l.Add(Rijetke.ImaTip(tx, "picturebox") && tx.IndexOf("LOCATION=", StringComparison.OrdinalIgnoreCase) >= 0
                    ? Qa.Z(Ime, "RIJ-19", "LOCATION ide u ImageLocation nakon ToUpper", "netgen/Form2.vb:2710")
                    : Qa.P(Ime, "RIJ-19", "LOCATION", "netgen/Form2.vb:2710", ""));
            }

            string dob = Path.Combine(root, "definitions", "netgen", "Agronom granule", "DobUgovori.xml");
            NgProject pd = Ucitaj(l, "RIJ-20", dob);
            if (pd != null)
            {
                NgForm ins = pd.NadjiFormu("INSERT_FORM");
                l.Add(Ispis.Atr(ins, "NEXTFORM") == "IZMJENI_FORM"
                    ? Qa.Z(Ime, "RIJ-21", "NEXTFORM nakon OK otvara IZMJENI_FORM. Dozvola pita ime prve forme", "netgen/Form2.vb:4431-4459")
                    : Qa.P(Ime, "RIJ-21", "NEXTFORM", "netgen/Form2.vb:4431", Ispis.Atr(ins, "NEXTFORM") ?? ""));
            }

            string nar = Path.Combine(root, "definitions", "netgen", "Agronom granule", "Narudzbe.xml");
            NgProject pn = Ucitaj(l, "RIJ-22", nar);
            if (pn != null)
            {
                string tt = pn.Atributi.Ima("TREETRANSLATE") ? (pn.Atributi.Vrijednost("TREETRANSLATE") ?? "") : "";
                NgForm unos = pn.NadjiFormu("UnosStavkeNar");
                l.Add(tt.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                    ? Qa.Z(Ime, "RIJ-23", "TREETRANSLATE samo na podlozi, stupac 2. Dijalog stablo ne prevodi. Ikona cvora je 0", "netgen/Form1.vb:4461-4464; netgen/Form1.designer.vb:575-579")
                    : Qa.P(Ime, "RIJ-23", "TREETRANSLATE", "netgen/Form1.vb:4461", tt));
                l.Add(Ispis.Atr(unos, "NEXTFORM") == "UnosStavkeNar"
                    ? Qa.Z(Ime, "RIJ-24", "NEXTFORM na isto ime vrti formu dok je OK", "netgen/Form2.vb:4431")
                    : Qa.P(Ime, "RIJ-24", "NEXTFORM sebe", "netgen/Form2.vb:4431", Ispis.Atr(unos, "NEXTFORM") ?? ""));
            }

            string jez = Path.Combine(root, "definitions", "netgen", "Agronom granule", "JeziciNG.xml");
            NgProject pj = Ucitaj(l, "RIJ-25", jez);
            if (pj != null)
            {
                string sd = pj.Atributi.Ima("SETDEFAULTS") ? (pj.Atributi.Vrijednost("SETDEFAULTS") ?? "") : "";
                l.Add(sd.Length > 0 && sd.IndexOf('#') < 0
                    ? Qa.Z(Ime, "RIJ-26", "SETDEFAULTS bez # je jedan ExecSql. Sa # svaki segment posebno", "netgen/Form1.vb:3862-3866,3945-3962")
                    : Qa.P(Ime, "RIJ-26", "SETDEFAULTS", "netgen/Form1.vb:3862", ""));
            }

            string dos = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KntDospjeleStavke.xml");
            NgProject pk = Ucitaj(l, "RIJ-27", dos);
            if (pk != null)
            {
                NgForm main = pk.NadjiFormu("MAINFORM");
                l.Add(string.Equals(Ispis.Atr(main, "DBCLIPBOARD"), "TRUE", StringComparison.OrdinalIgnoreCase)
                    ? Qa.Z(Ime, "RIJ-28", "DBCLIPBOARD TRUE cita NetGenDBClipboard. Ime filter pali clipboard osim ako je false", "netgen/Form2.vb:1099-1103,1170-1176")
                    : Qa.P(Ime, "RIJ-28", "clipboard", "netgen/Form2.vb:1170", Ispis.Atr(main, "DBCLIPBOARD") ?? ""));
            }

            l.Add(Stablo.Meni(Ime, "RIJ-30", "AOPNovi.xml"));
            l.Add(Stablo.Meni(Ime, "RIJ-31", "RatArkod.xml"));
            l.Add(Stablo.Meni(Ime, "RIJ-32", "Narudzbe.xml"));
            l.Add(Stablo.Meni(Ime, "RIJ-33", "RobaWebKlase.xml"));

            if (pa != null)
                Rijetke.K(l, Ime, aop, "RIJ-40", "Dijalog stablo: tag je stupac 3, bez :forma. yearmonth na dijalogu ne crta", "netgen/Form2.vb:2576-2577,2842,2864", "tree.dijalog.tag", "stupac3");
            if (pr != null)
                Rijetke.K(l, Ime, ark, "RIJ-41", "webbrowser Navigate. URL atribut je ToUpper. refresh nije URL", "netgen/Form2.vb:1419-1433", "web.navigate", "onload-kolona");
            if (pp != null)
                Rijetke.K(l, Ime, pic, "RIJ-42", "picturebox StretchImage. http zove Load. LOCATION je ToUpper", "netgen/Form2.vb:2710-2726", "pic.stretch", "stretch");
            return l;
        }

        private static string NadjiPoImenu(string korijen, string ime)
        {
            foreach (string p in Directory.GetFiles(korijen, ime, SearchOption.AllDirectories))
                return p;
            return null;
        }

        private static bool SviPoImenu(string root, string[] imena, string tip)
        {
            string net = Path.Combine(root, "definitions", "netgen");
            for (int i = 0; i < imena.Length; i++)
            {
                string p = NadjiPoImenu(net, imena[i]);
                if (p == null || !Rijetke.ImaTip(Rijetke.Procitaj(p), tip))
                    return false;
            }
            return true;
        }

        private static bool Svi(string root, string id, string[] rel, string tip)
        {
            for (int i = 0; i < rel.Length; i++)
            {
                string p = Path.Combine(root, "definitions", "netgen", rel[i].Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(p))
                    return false;
                if (!Rijetke.ImaTip(Rijetke.Procitaj(p), tip))
                    return false;
            }
            return true;
        }

        private static NgProject Ucitaj(List<Stavka> l, string id, string xml)
        {
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, id, "XML postoji", xml, "nema"));
                return null;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.O(Ime, id, "Parser nije ucitao " + Path.GetFileName(xml), "netgen/Form1.vb:3263", g));
                return null;
            }
            l.Add(Qa.Z(Ime, id, Path.GetFileName(xml) + " se ucita", "netgen/Form1.vb:3263"));
            return p;
        }

        private static NgControl NadjiGranu(NgProject p, NgGrana g)
        {
            for (int i = 0; i < p.Forme.Count; i++)
            {
                for (int j = 0; j < p.Forme[i].Kontrole.Count; j++)
                {
                    if (p.Forme[i].Kontrole[j].Grana == g)
                        return p.Forme[i].Kontrole[j];
                }
            }
            return null;
        }

        private static NgControl NadjiIme(NgProject p, string ime)
        {
            for (int i = 0; i < p.Forme.Count; i++)
            {
                NgControl c = Kontrole.Nadji(p.Forme[i], ime);
                if (c != null)
                    return c;
            }
            return null;
        }
    }
}

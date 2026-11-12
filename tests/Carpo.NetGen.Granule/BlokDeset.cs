using System;
using System.Collections.Generic;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class DrzaveNg
    {
        public const string Ime = "DrzaveNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "DRZ-01", root, "definitions", "netgen", "BIOVEGA", "DrzaveNG.xml");
            if (p == null)
                return l;
            l.Add((p.OnLoad ?? "").StartsWith("spdrzaveQuery @action=0", StringComparison.Ordinal)
                && Pet.GumbNaredba(p, 1, "DrzaveUnesi#refreshmaster")
                && Pet.GumbNaredba(p, 5, "PRINTREPXMASTER")
                ? Qa.Z(Ime, "DRZ-02", "Ulaz je upit. Unesi osvjezi master. Ispis je PRINTREPXMASTER", "netgen/Form1.vb:3879,4951")
                : Qa.P(Ime, "DRZ-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            NgForm f = p.NadjiFormu("DrzaveUnesi");
            string zatvori = f == null ? "" : GridNext.OnClick(f, "buttonCLOSE", "OK");
            string spremi = f == null ? "" : GridNext.OnClick(f, "buttonOK", "spDrzaveAzur @action=0");
            l.Add(f != null && Pet.AttrF(f, "NEXTFORM") == "Dodaj" && p.NadjiFormu("Dodaj") == null
                && zatvori.Trim() == "OK" && spremi.IndexOf("@action=0", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "DRZ-03", "Odustani je OK. Spremi je action 0. NEXTFORM Dodaj nema formu", "netgen/Form2.vb:6888,4430")
                : Qa.P(Ime, "DRZ-03", "unos", "netgen/Form2.vb:1052", zatvori));
            return l;
        }
    }

    public static class ZupanijeNg
    {
        public const string Ime = "ZupanijeNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "ZUP-01", root, "definitions", "netgen", "BIOVEGA", "ZupanijeNG.xml");
            if (p == null)
                return l;
            NgForm f = p.NadjiFormu("ZupanijeUnesi");
            string zatvori = f == null ? "" : GridNext.OnClick(f, "buttonCLOSE", "OK");
            l.Add((p.OnLoad ?? "").StartsWith("spZupanijeQuery @action=0", StringComparison.Ordinal)
                && Pet.GumbNaredba(p, 5, "PRINTREPXMASTER")
                && f != null && Pet.AttrF(f, "NEXTFORM") == "Dodaj" && zatvori.Trim() == "OK"
                ? Qa.Z(Ime, "ZUP-02", "Ulaz je upit. Odustani je OK i trazi formu Dodaj", "netgen/Form1.vb:3879")
                : Qa.P(Ime, "ZUP-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            return l;
        }
    }

    public static class PdvPromjenaNew
    {
        public const string Ime = "PDVPromjenaNEW";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PDN-01", root, "definitions", "netgen", "BIOVEGA", "PDVPromjenaNEW.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":FILTERFORM" && (p.Tree ?? "").IndexOf("spTreeKlMasterRoba:KLASIFIKACIJA", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PDN-02", "Ulaz je FILTERFORM. Stablo je store prema KLASIFIKACIJA", "netgen/Form1.vb:3879,4350")
                : Qa.P(Ime, "PDN-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            NgForm brisi = p.NadjiFormu("DELETEFORM");
            NgForm sve = p.NadjiFormu("OBRISISVE");
            l.Add(brisi != null && (brisi.OnSubmit ?? "").IndexOf("action = 55", StringComparison.Ordinal) >= 0
                && (brisi.OnSubmit ?? "").IndexOf("!!", StringComparison.Ordinal) >= 0
                && sve != null && (sve.OnSubmit ?? "").IndexOf("action = 555", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PDN-03", "Brisanje jedne robe je action 55 s !!. Sve najave su action 555", "netgen/Form1.vb:5449")
                : Qa.P(Ime, "PDN-03", "brisanje", "netgen/Form1.vb:5449", brisi == null ? "" : brisi.OnSubmit));
            return l;
        }
    }

    public static class MpNarPredis
    {
        public const string Ime = "MPNarPredis";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "MNP-01", root, "definitions", "netgen", "BIOVEGA", "MPNarPredis.xml");
            if (p == null)
                return l;
            l.Add(string.IsNullOrEmpty(p.OnLoad)
                && (p.Tree ?? "").IndexOf("Danas:DANAS_FORM", StringComparison.Ordinal) >= 0
                && (p.DetailGrid ?? "").IndexOf("@id=@@id", StringComparison.Ordinal) >= 0
                && (p.Buttons ?? "").IndexOf("vrstadokid-MaloprodajaGla", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "MNP-02", "Nema ONLOAD. Stablo pocinje Danas. Ispis je vrstadokid-", "netgen/Form1.vb:3879,4350,5215")
                : Qa.P(Ime, "MNP-02", "tree", "netgen/Form1.vb:4350", p.Tree ?? ""));
            return l;
        }
    }

    public static class NacinOtpremeNg
    {
        public const string Ime = "NacinOtpremeNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "NOT-01", root, "definitions", "netgen", "BIOVEGA", "NacinOtpremeNG.xml");
            if (p == null)
                return l;
            NgForm f = p.NadjiFormu("NacinOtpremeUnesi");
            string spremi = f == null ? "" : GridNext.OnClick(f, "buttonOK", "OKStay");
            l.Add((p.OnLoad ?? "").StartsWith("spProNacinIsporukeQuery @action = 0", StringComparison.Ordinal)
                && Pet.GumbNaredba(p, 1, "NacinOtpremeUnesi#refreshmaster")
                && spremi.IndexOf("OKStay", StringComparison.Ordinal) >= 0
                && Pet.AttrF(f, "NEXTFORM") == "Dodaj"
                ? Qa.Z(Ime, "NOT-02", "Ulaz je upit. Spremi ostaje zbog OKStay. Sljedeca forma je Dodaj", "netgen/Form2.vb:6884")
                : Qa.P(Ime, "NOT-02", "unos", "netgen/Form2.vb:6884", spremi));
            return l;
        }
    }

    public static class GridNext
    {
        public const string Ime = "GridNext";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            Grid(l, root, "GRD-01", "Agronom granule", "ServisPrava.xml", "MAINFORM", "master", "spServisPravaQueryNG @action=2", "");
            Grid(l, root, "GRD-02", "Agronom granule", "ServisPrava.xml", "MAINFORM", "detail", "@@master", "");
            Grid(l, root, "GRD-03", "Agronom granule", "RekapitulacijaUI.xml", "Parametri", "strskl", "skladisteid", "");
            Grid(l, root, "GRD-04", "Agronom granule", "ManagerCjenika.xml", "MAINFORM", "g_master", "@@find", "");
            Grid(l, root, "GRD-05", "BIOVEGA", "KomStanjeSkl.xml", "MAINFORM", "skladisteid", "viSkladistaPravaIzvjestaji", "");
            Grid(l, root, "GRD-06", "BIOVEGA", "MPPonudaNaruciRazliku.xml", "FILTERFORM", "povezinar", "spmpPonudaNaruciRazliku @action = 2", "TRUE");
            Grid(l, root, "GRD-07", "BIOVEGA", "IspisNaljepnica.xml", "MAINFORM", "detalji", "@@glava", "");
            Grid(l, root, "GRD-08", "BIOVEGA", "MPAnalBrStavaka.xml", "FILTERFORM", "stavke", "##tmpanalstavke", "");
            Grid(l, root, "GRD-09", "BIOVEGA", "MPPregArtNG2.xml", "FILTERFORM", "grdrobaOdabir", "spMPPregArtng @action = 6", "FALSE");
            Grid(l, root, "GRD-10", "BIOVEGA", "ProNotpL.xml", "MAINFORM", "stavke", "pronotpdet", "");
            Next(l, root, "NXF-01", "BIOVEGA", "KomStanjeSkl.xml", "MAINFORM", "ButtonKartica:KARTICA", "");
            Next(l, root, "NXF-02", "BIOVEGA", "MPPSNG.xml", "UPDATE_FORM0", ":@@UPDATE_FORM", "");
            Next(l, root, "NXF-03", "Agronom granule", "KntKompVerizne.xml", "DODAJ_FORM", "IZMJENI_FORM", "@action=0");
            Next(l, root, "NXF-04", "BIOVEGA", "MPNarIzdSkl.xml", "INSERTFORM", "UPDATE_FORM", "spMPInsertGlaIdetNG");
            Next(l, root, "NXF-05", "BIOVEGA", "DrzaveNG.xml", "DrzaveUnesi", "Dodaj", "");
            Next(l, root, "NXF-06", "BIOVEGA", "RobaAtributiUredi.xml", "AZURFORM", "AZURFORM", "");
            Next(l, root, "NXF-07", "BIOVEGA", "NabStoNarucitiBiovegaNG.xml", "NARUDZBA", "Naruceno", "");
            Next(l, root, "NXF-08", "Agronom granule", "MER_ZahtjevZaNabavkuNG.xml", "INSERT", "UPDATE", "");
            return l;
        }

        private static void Grid(List<Stavka> l, string root, string id, string folder, string fajl, string forma, string kontrola, string sqlDio, string skip)
        {
            NgProject p = Pet.Ucitaj(l, Ime, id, root, "definitions", "netgen", folder, fajl);
            if (p == null)
                return;
            NgForm f = p.NadjiFormu(forma);
            NgControl c = f == null ? null : Nadji(f, kontrola);
            if (c == null)
            {
                l.Add(Qa.P(Ime, id, kontrola, fajl, "nema grida"));
                return;
            }
            string sql = Attr(c, "SQL");
            string sk = Attr(c, "SKIPDEFAULTLOAD");
            bool ok = sql.IndexOf(sqlDio, StringComparison.Ordinal) >= 0 && sk.Equals(skip, StringComparison.OrdinalIgnoreCase);
            l.Add(ok
                ? Qa.Z(Ime, id, kontrola + " SQL i SKIPDEFAULTLOAD odgovaraju NetGen pravilu", "netgen/Form2.vb:1635-1646")
                : Qa.P(Ime, id, kontrola, "netgen/Form2.vb:1643", sk + " | " + sql));
        }

        private static void Next(List<Stavka> l, string root, string id, string folder, string fajl, string forma, string sljedeca, string submitDio)
        {
            NgProject p = Pet.Ucitaj(l, Ime, id, root, "definitions", "netgen", folder, fajl);
            if (p == null)
                return;
            NgForm f = p.NadjiFormu(forma);
            if (f == null)
            {
                l.Add(Qa.P(Ime, id, forma, fajl, "nema forme"));
                return;
            }
            bool ok = Pet.AttrF(f, "NEXTFORM") == sljedeca;
            if (ok && submitDio.Length > 0)
                ok = (f.OnSubmit ?? "").IndexOf(submitDio, StringComparison.Ordinal) >= 0;
            l.Add(ok
                ? Qa.Z(Ime, id, forma + " NEXTFORM je " + sljedeca, "netgen/Form2.vb:1052,4431")
                : Qa.P(Ime, id, forma, "netgen/Form2.vb:1052", Pet.AttrF(f, "NEXTFORM")));
        }

        private static NgControl Nadji(NgForm f, string ime)
        {
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                if (string.Equals(f.Kontrole[i].Name, ime, StringComparison.OrdinalIgnoreCase))
                    return f.Kontrole[i];
            }
            return null;
        }

        private static string Attr(NgControl c, string ime)
        {
            foreach (var kv in c.Atributi.Stavke)
            {
                if (string.Equals(kv.Key, ime, StringComparison.OrdinalIgnoreCase))
                    return kv.Value ?? "";
            }
            return "";
        }

        public static string OnClick(NgForm f, string ime, string trag)
        {
            string pogodak = null;
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                NgControl c = f.Kontrole[i];
                if (!string.Equals(c.Name, ime, StringComparison.OrdinalIgnoreCase))
                    continue;
                string oc = Attr(c, "ONCLICK");
                if (oc.IndexOf(trag, StringComparison.Ordinal) < 0 && oc.Trim() != trag)
                    continue;
                pogodak = oc;
            }
            return pogodak ?? "";
        }
    }
}

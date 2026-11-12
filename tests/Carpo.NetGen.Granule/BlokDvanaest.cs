using System;
using System.Collections.Generic;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class KodRad1
    {
        public const string Ime = "KOD_RAD1";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "RAD-01", root, "definitions", "netgen", "BIOVEGA", "KOD_RAD1.xml");
            if (p == null)
                return l;
            NgForm f = p.NadjiFormu("IZMJENA_RAD1_STAVKE");
            string azur = "";
            if (f != null)
            {
                for (int i = 0; i < f.Kontrole.Count; i++)
                {
                    string oc = "";
                    foreach (var kv in f.Kontrole[i].Atributi.Stavke)
                    {
                        if (string.Equals(kv.Key, "ONCLICK", StringComparison.OrdinalIgnoreCase))
                            oc = kv.Value ?? "";
                    }
                    if (oc.IndexOf("update_stavke_rad1", StringComparison.Ordinal) >= 0)
                        azur = oc;
                }
            }
            l.Add(string.IsNullOrEmpty(p.OnLoad) && Pet.Attr(p, "COMMANDTIMEOUT") == "600"
                && Pet.Attr(p, "TOTALROWVISIBLE").Equals("false", StringComparison.OrdinalIgnoreCase)
                && azur.IndexOf("spKOD_RAD1_Azur", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "RAD-02", "Nema ONLOAD. Timeout je 600. Izmjena stavke zove spKOD_RAD1_Azur", "netgen/Form1.vb:3879,3463")
                : Qa.P(Ime, "RAD-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            return l;
        }
    }

    public static class MpPredispIz
    {
        public const string Ime = "MPPredispIz";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PDI-01", root, "definitions", "netgen", "BIOVEGA", "MPPredispIz.xml");
            if (p == null)
                return l;
            l.Add(string.IsNullOrEmpty(p.OnLoad)
                && (p.Tree ?? "").IndexOf("Danas:DANAS_FORM", StringComparison.Ordinal) >= 0
                && (p.Tree ?? "").IndexOf("spMPTreeNG", StringComparison.Ordinal) >= 0
                && p.MasterDblClick == "UPDATE_FORM"
                && (p.Buttons ?? "").IndexOf("vrstadokid-MaloprodajaGla", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PDI-02", "Nema ONLOAD. Stablo ima Danas i store. Ispis je vrstadokid-", "netgen/Form1.vb:3879,4350,5215")
                : Qa.P(Ime, "PDI-02", "tree", "netgen/Form1.vb:4350", p.Tree ?? ""));
            return l;
        }
    }

    public static class MpIzduMp
    {
        public const string Ime = "MPIzduMPZaprimi";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "IZD-01", root, "definitions", "netgen", "BIOVEGA", "MPIzduMPZaprimi.xml");
            if (p == null)
                return l;
            bool lista = false;
            for (int i = 0; i < p.PreskoceniGumbi.Count; i++)
                if (p.PreskoceniGumbi[i] == "CMD7CAPTION")
                    lista = true;
            NgForm ins = p.NadjiFormu("INSERTFORM");
            l.Add(string.IsNullOrEmpty(p.OnLoad) && (p.Tree ?? "").IndexOf("Danas:DANAS_FORM", StringComparison.Ordinal) >= 0
                && (p.Tree ?? "").IndexOf("spMPTree @action=1", StringComparison.Ordinal) >= 0
                && p.MasterDblClick == "UPDATE_FORM"
                && (p.Popup ?? "").IndexOf("UPDATEFORM", StringComparison.Ordinal) >= 0
                && lista && Pet.GumbNaredba(p, 6, "ISPIS_FORM")
                && ins != null && Pet.AttrF(ins, "NEXTFORM") == "UPDATE_FORM"
                ? Qa.Z(Ime, "IZD-02", "Lista je skrivena. Dvostruki klik je UPDATE_FORM, popup je UPDATEFORM. Unos ide na UPDATE_FORM", "netgen/Form1.vb:3320,4435")
                : Qa.P(Ime, "IZD-02", "tree", "netgen/Form1.vb:4350", p.MasterDblClick ?? ""));
            return l;
        }
    }

    public static class CarinskaTarifaNg
    {
        public const string Ime = "CarinskaTarifaNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "CAR-01", root, "definitions", "netgen", "BIOVEGA", "CarinskaTarifaNG.xml");
            if (p == null)
                return l;
            NgForm u = p.NadjiFormu("CarinskaTarifaUnesi");
            NgForm b = p.NadjiFormu("CarinskaTarifaBrisanje");
            string odustani = u == null ? "" : GridNext.OnClick(u, "buttonCLOSE", "ok");
            string brisi = "";
            if (b != null)
            {
                for (int i = 0; i < b.Kontrole.Count; i++)
                {
                    foreach (var kv in b.Kontrole[i].Atributi.Stavke)
                    {
                        if (string.Equals(kv.Key, "ONCLICK", StringComparison.OrdinalIgnoreCase)
                            && (kv.Value ?? "").IndexOf("@action=2", StringComparison.Ordinal) >= 0)
                            brisi = kv.Value ?? "";
                    }
                }
            }
            l.Add((p.OnLoad ?? "").StartsWith("spCarinskaTarifaQuery @action=0", StringComparison.Ordinal)
                && Pet.GumbNaredba(p, 1, "CarinskaTarifaUnesi#refreshmaster")
                && Pet.GumbNaredba(p, 5, "PRINTREPXMASTER")
                && u != null && Pet.AttrF(u, "NEXTFORM") == "Dodaj" && p.NadjiFormu("Dodaj") == null
                && odustani.Trim() == "ok" && brisi.IndexOf("#OK", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "CAR-02", "Ulaz je upit. Odustani na unosu je ok. Brisanje je action 2. Forma Dodaj ne postoji", "netgen/Form2.vb:6888,4430")
                : Qa.P(Ime, "CAR-02", "unos", "netgen/Form2.vb:1052", odustani));
            return l;
        }
    }

    public static class MpZakljPerioda
    {
        public const string Ime = "MPZakljPerioda";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "ZKP-01", root, "definitions", "netgen", "BIOVEGA", "MPZakljPerioda.xml");
            if (p == null)
                return l;
            NgForm f = p.NadjiFormu("IZMJENIFORM");
            l.Add((p.OnLoad ?? "").IndexOf("as prazno", StringComparison.Ordinal) >= 0
                && Pet.Attr(p, "COMMANDTIMEOUT") == "600"
                && p.MasterDblClick == "MPKARTICA"
                && f != null && (f.OnSubmit ?? "").IndexOf("@action=8", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "ZKP-02", "Ulaz je select stupca prazno. Dvostruki klik je MPKARTICA. Izmjena je action 8", "netgen/Form1.vb:3879,3463")
                : Qa.P(Ime, "ZKP-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            return l;
        }
    }
}

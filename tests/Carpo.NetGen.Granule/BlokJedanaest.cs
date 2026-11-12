using System;
using System.Collections.Generic;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class MpEurParam
    {
        public const string Ime = "MPEurParam";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "EUR-01", root, "definitions", "netgen", "BIOVEGA", "MPEurParam.xml");
            if (p == null)
                return l;
            NgForm f = p.NadjiFormu("UPDATEFORM");
            string spremi = f == null ? "" : GridNext.OnClick(f, "ok", "action=2");
            l.Add(p.OnLoad == ":UPDATEFORM" && (p.Buttons ?? "").IndexOf("UPDATEFORM", StringComparison.Ordinal) >= 0
                && spremi.IndexOf("#Ok", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "EUR-02", "Ulaz je UPDATEFORM. Spremi je action 2 pa Ok", "netgen/Form1.vb:3879")
                : Qa.P(Ime, "EUR-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            return l;
        }
    }

    public static class MpAzurPubVars
    {
        public const string Ime = "MPAzurPubVars";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "PUB-01", root, "definitions", "netgen", "BIOVEGA", "MPAzurPubVars.xml");
            if (p == null)
                return l;
            NgForm o = p.NadjiFormu("OBNOVI");
            l.Add(p.OnLoad == "spMPAzurPubvars @action = 2" && Pet.GumbNaredba(p, 1, "OBNOVI")
                && o != null && Pet.AttrF(o, "FILLEXPLORER").Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                && (o.OnSubmit ?? "").IndexOf("@action = 1", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "PUB-02", "Ulaz je action 2. Obnovi je action 1 i puni master", "netgen/Form1.vb:3879,5445")
                : Qa.P(Ime, "PUB-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            return l;
        }
    }

    public static class MpTrgovinaPortal
    {
        public const string Ime = "MPTrgovinaPortal";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "POR-01", root, "definitions", "netgen", "BIOVEGA", "MPTrgovinaPortal.xml");
            if (p == null)
                return l;
            NgForm k = p.NadjiFormu("KASAINSERT_FORM");
            l.Add(string.IsNullOrEmpty(p.OnLoad) && p.MasterDblClick == ":TRGOVINAFORM"
                && Pet.Attr(p, "COMMANDTIMEOUT") == "300"
                && k != null && (k.OnSubmit ?? "").IndexOf("spMPKase @Action=3", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "POR-02", "Nema ONLOAD. Dvostruki klik otvara TRGOVINAFORM. Unos kase je Action 3", "netgen/Form1.vb:3879,4862,3463")
                : Qa.P(Ime, "POR-02", "dbl", "netgen/Form1.vb:4862", p.MasterDblClick ?? ""));
            return l;
        }
    }

    public static class MpUvjetiKase
    {
        public const string Ime = "MPUvjetiKase";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "UVK-01", root, "definitions", "netgen", "BIOVEGA", "MPUvjetiKase.xml");
            if (p == null)
                return l;
            NgForm f = p.NadjiFormu("FILTERFORM");
            NgForm d = p.NadjiFormu("DELETEFORM");
            string brisi = d == null ? "" : GridNext.OnClick(d, "ok", "action=5");
            l.Add(p.OnLoad == ":FILTERFORM" && Pet.Attr(p, "CHECKBOXVISIBLE") == "TRUE"
                && f != null && Pet.AttrF(f, "FILLEXPLORER").Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                && brisi.IndexOf("#Ok", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "UVK-02", "Ulaz je filter. Checkbox ostaje. Brisanje je action 5 pa Ok", "netgen/Form1.vb:3879,2043")
                : Qa.P(Ime, "UVK-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            return l;
        }
    }

    public static class ObrazacGranula
    {
        public const string Ime = "Obrazac";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "OBR-01", root, "definitions", "netgen", "BIOVEGA", "Obrazac.xml");
            if (p == null)
                return l;
            NgForm osv = p.NadjiFormu("REFRESHTREEFORM");
            l.Add(string.IsNullOrEmpty(p.OnLoad)
                && (p.Tree ?? "").IndexOf("spObrazacTree @action='obrazac':STATUS", StringComparison.Ordinal) >= 0
                && osv != null && Pet.AttrF(osv, "FILLEXPLORER").Equals("onlytree", StringComparison.OrdinalIgnoreCase)
                && (p.Buttons ?? "").IndexOf("OBRAZAC(@obrazacid=0)", StringComparison.Ordinal) >= 0
                && Pet.Attr(p, "COMMANDTIMEOUT") == "600"
                ? Qa.Z(Ime, "OBR-02", "Stablo je store. Osvjezi je onlytree. Unos zove OBRAZAC s parametrom", "netgen/Form1.vb:4350,5475")
                : Qa.P(Ime, "OBR-02", "tree", "netgen/Form1.vb:4350", p.Tree ?? ""));
            return l;
        }
    }
}

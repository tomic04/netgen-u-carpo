using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class RekapitulacijaUi
    {
        public const string Ime = "RekapitulacijaUI";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string sklad = Path.Combine(root, "definitions", "netgen", "SKLAD", "RekapitulacijaUI.xml");
            if (!File.Exists(sklad))
                l.Add(Qa.O(Ime, "REK-00", "SKLAD\\RekapitulacijaUI.xml nije u repou", "docs/granule/Prioritet.md", sklad));
            NgProject p = Pet.Ucitaj(l, Ime, "REK-01", root, "definitions", "netgen", "Agronom granule", "RekapitulacijaUI.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":Parametri#refreshmaster" && Pet.Attr(p, "COMMANDTIMEOUT") == "180"
                && Pet.GumbNaredba(p, 4, "PRINTREPXMASTER")
                ? Qa.Z(Ime, "REK-02", "Ulaz je Parametri pa refreshmaster. Ispis je PRINTREPXMASTER. Timeout je 180", "netgen/Form1.vb:3879,4570,4945,3463")
                : Qa.P(Ime, "REK-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            return l;
        }
    }

    public static class ServisPrava
    {
        public const string Ime = "ServisPrava";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "SPR-01", root, "definitions", "netgen", "Agronom granule", "ServisPrava.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":MAINFORM" && Pet.Attr(p, "DETAILDBLCLICK") == ":UPDATE" && p.NadjiFormu("MAINFORM") != null
                ? Qa.Z(Ime, "SPR-02", "Ulaz je MAINFORM. Dvostruki klik detalja je UPDATE", "netgen/Form1.vb:3879,3390")
                : Qa.P(Ime, "SPR-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(p.NadjiFormu("FILTER") != null
                ? Qa.Z(Ime, "SPR-03", "Element FILTER je forma", "netgen/Form2.vb:957")
                : Qa.O(Ime, "SPR-03", "Element FILTER jos nije forma", "netgen/Form2.vb:957", "FILTER je strukturni"));
            return l;
        }
    }

    public static class MpInvRazlike
    {
        public const string Ime = "MPInvRazlike";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "IRA-01", root, "definitions", "netgen", "BIOVEGA", "MPInvRazlike.xml");
            if (p == null)
                return l;
            bool lista = false;
            for (int i = 0; i < p.PreskoceniGumbi.Count; i++)
                if (p.PreskoceniGumbi[i] == "CMD7CAPTION")
                    lista = true;
            l.Add(string.IsNullOrEmpty(p.OnLoad) && (p.Tree ?? "").IndexOf("Danas:DANAS_FORM", StringComparison.Ordinal) >= 0
                && lista && Pet.GumbNaredba(p, 6, "vrstadokid-MaloprodajaGla")
                ? Qa.Z(Ime, "IRA-02", "Nema ONLOAD. Lista je skrivena. Ispis je vrstadokid-", "netgen/Form1.vb:3879,3320,5215")
                : Qa.P(Ime, "IRA-02", "tree", "netgen/Form1.vb:3879", p.Tree ?? ""));
            l.Add(Pet.GumbNaredba(p, 2, "INSERTFORM@@indvrdok")
                ? Qa.Z(Ime, "IRA-03", "Dodaj trazi INSERTFORM@@indvrdok, ne INSERTFORM", "netgen/Form1.vb:3326")
                : Qa.P(Ime, "IRA-03", "dodaj", "netgen/Form1.vb:3326", Pet.OpisGumba(p, 2)));
            l.Add((p.Popup ?? "").IndexOf("Stornoraj#STORNO", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "IRA-04", "Stornoraj i STORNO nemaju dvotocku i nisu stavka izbornika", "netgen/Form1.vb:3564-3566")
                : Qa.P(Ime, "IRA-04", "popup", "netgen/Form1.vb:3564", p.Popup ?? ""));
            return l;
        }
    }

    public static class MpNarFFirma
    {
        public const string Ime = "MPNarFFirma";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "NFF-01", root, "definitions", "netgen", "BIOVEGA", "MPNarFFirma.xml");
            if (p == null)
                return l;
            bool lista = false;
            for (int i = 0; i < p.PreskoceniGumbi.Count; i++)
                if (p.PreskoceniGumbi[i] == "CMD7CAPTION")
                    lista = true;
            string tree = p.Tree ?? "";
            l.Add(string.IsNullOrEmpty(p.OnLoad) && tree.IndexOf("Danas:DANAS_FORM", StringComparison.Ordinal) >= 0
                && (p.DetailGrid ?? "").IndexOf("@id=@@id", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "NFF-02", "Nema ONLOAD. Detail veze @@id", "netgen/Form1.vb:3879,3373")
                : Qa.P(Ime, "NFF-02", "detail", "netgen/Form1.vb:3373", p.DetailGrid ?? ""));
            l.Add(lista && Pet.GumbNaredba(p, 6, "vrstadokid-propongla") && Pet.Attr(p, "COMMANDTIMEOUT") == "300"
                ? Qa.Z(Ime, "NFF-03", "Lista je skrivena. Ispis je vrstadokid-propongla. Timeout je 300", "netgen/Form1.vb:3320,5215,3463")
                : Qa.P(Ime, "NFF-03", "ispis", "netgen/Form1.vb:5215", Pet.OpisGumba(p, 6)));
            return l;
        }
    }

    public static class LvKatalozi
    {
        public const string Ime = "LVKatalozi";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "LVK-01", root, "definitions", "netgen", "BIOVEGA", "LVKatalozi.xml");
            if (p == null)
                return l;
            NgForm osv = p.NadjiFormu("REFRESHTREEFORM");
            l.Add(string.IsNullOrEmpty(p.OnLoad) && (p.Tree ?? "").IndexOf("spLVKatalozi_Tree @action='tree':STATUS", StringComparison.Ordinal) >= 0
                && osv != null && Pet.AttrF(osv, "FILLEXPLORER").Equals("onlytree", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "LVK-02", "Nema ONLOAD. Stablo je store. Osvjezi drvo je onlytree", "netgen/Form1.vb:4350,5475")
                : Qa.P(Ime, "LVK-02", "tree", "netgen/Form1.vb:4350", p.Tree ?? ""));
            l.Add(Pet.Attr(p, "COMMANDTIMEOUT") == "600" && Pet.Attr(p, "TOTALROWVISIBLE").Equals("false", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "LVK-03", "Timeout je 600. false gasi sumu", "netgen/Form1.vb:3463,2049")
                : Qa.P(Ime, "LVK-03", "timeout", "netgen/Form1.vb:3463", Pet.Attr(p, "COMMANDTIMEOUT")));
            return l;
        }
    }

    public static class Nedostaje79
    {
        private static readonly string[] Imena = new string[]
        {
            "IzvKlasePartneriParstru.xml", "jesteliznali.xml", "KntOpomene.xml", "KntRasporedTros.xml",
            "KonfigSrvRN.xml", "ManGadTip.xml", "newswebsite.xml", "OrgShemaKonf.xml",
            "OsobeOperateri.xml", "PubVarsNG.xml", "ServisJamstvo3.xml", "Spojka.xml",
            "Upitnici.xml", "VoziloFinRekapNG.xml"
        };

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            for (int i = 0; i < Imena.Length; i++)
            {
                string fajl = Imena[i];
                string id = "N79-" + (i + 1).ToString("00");
                bool ima = File.Exists(Path.Combine(root, "definitions", "netgen", "BIOVEGA", fajl))
                    || File.Exists(Path.Combine(root, "definitions", "netgen", fajl))
                    || File.Exists(Path.Combine(root, "definitions", "netgen", "Agronom granule", fajl));
                if (!ima)
                    l.Add(Qa.O("Nedostaje79", id, fajl + " nije u definitions/netgen", "docs/granule/Prioritet.md", "rang 79-92"));
                else
                    l.Add(Qa.Z("Nedostaje79", id, "XML je stigao", fajl));
            }
            return l;
        }
    }

    public static class ComboDijalog
    {
        public const string Ime = "ComboDijalog";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            K(l, root, "CMB-01", "BIOVEGA", "MPInventura.xml", "sortpoid", "FALSE", "select 1 as sortpoid", "");
            K(l, root, "CMB-02", "BIOVEGA", "MPInventura.xml", "trgovinaid", "FALSE", "spMPDajTrgovina @operateriid=@@operateriid", "refresh");
            K(l, root, "CMB-03", "BIOVEGA", "MPInventura.xml", "osobeid", "FALSE", "@@trgovinaid", "");
            K(l, root, "CMB-04", "Agronom granule", "MPRealizNG.xml", "trgovinaid", "true", "@@operateriid", "");
            K(l, root, "CMB-05", "Agronom granule", "MPKntNacinPlacAzur.xml", "svi", "FOLSE", "select 2 as svi", "");
            K(l, root, "CMB-06", "Agronom granule", "DobUgovori.xml", "DobUgovoriVrsteId", "TRUE", "DobUgovoriVrste", "TRANSLATE");
            return l;
        }

        private static void K(List<Stavka> l, string root, string id, string folder, string fajl, string kontrola, string prazno, string sqlDio, string dodatno)
        {
            NgProject p = Pet.Ucitaj(l, Ime, id, root, "definitions", "netgen", folder, fajl);
            if (p == null)
                return;
            NgControl c = Nadji(p, kontrola);
            if (c == null)
            {
                l.Add(Qa.P(Ime, id, kontrola, fajl, "nema kontrole"));
                return;
            }
            string ae = "";
            foreach (var kv in c.Atributi.Stavke)
                if (string.Equals(kv.Key, "ALLOWEMPTY", StringComparison.OrdinalIgnoreCase))
                    ae = kv.Value ?? "";
            string sql = "";
            string oc = "";
            string tr = "";
            foreach (var kv in c.Atributi.Stavke)
            {
                if (string.Equals(kv.Key, "SQL", StringComparison.OrdinalIgnoreCase))
                    sql = kv.Value ?? "";
                if (string.Equals(kv.Key, "ONCLICK", StringComparison.OrdinalIgnoreCase))
                    oc = kv.Value ?? "";
                if (string.Equals(kv.Key, "TRANSLATE", StringComparison.OrdinalIgnoreCase))
                    tr = kv.Value ?? "";
            }
            bool ok = ae == prazno && sql.IndexOf(sqlDio, StringComparison.Ordinal) >= 0;
            if (dodatno == "refresh")
                ok = ok && oc.IndexOf("refresh", StringComparison.OrdinalIgnoreCase) >= 0;
            if (dodatno == "TRANSLATE")
                ok = ok && tr.Equals("true", StringComparison.OrdinalIgnoreCase);
            l.Add(ok
                ? Qa.Z(Ime, id, kontrola + " ALLOWEMPTY=" + prazno + " i SQL odgovaraju NetGen pravilu", "netgen/Form2.vb:2402-2411,3455-3464")
                : Qa.P(Ime, id, kontrola, "netgen/Form2.vb:2403", ae + " | " + sql));
        }

        private static NgControl Nadji(NgProject p, string ime)
        {
            for (int i = 0; i < p.Forme.Count; i++)
            {
                NgForm f = p.Forme[i];
                for (int j = 0; j < f.Kontrole.Count; j++)
                {
                    if (string.Equals(f.Kontrole[j].Name, ime, StringComparison.OrdinalIgnoreCase))
                        return f.Kontrole[j];
                }
            }
            return null;
        }
    }
}

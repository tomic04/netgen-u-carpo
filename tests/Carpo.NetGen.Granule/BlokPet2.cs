using System;
using System.Collections.Generic;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class MpTrgOpr
    {
        public const string Ime = "MPTrgOprNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "TRG-01", root, "definitions", "netgen", "BIOVEGA", "MPTrgOprNG.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":FILTERFORM"
                ? Qa.Z(Ime, "TRG-02", "Ulaz otvara FILTERFORM", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "TRG-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(p.DetailGrid != null && p.DetailGrid.IndexOf("@id=@@id", StringComparison.Ordinal) >= 0
                && p.MasterDblClick == "IZMJENA"
                ? Qa.Z(Ime, "TRG-03", "Detail veze @@id. Dvostruki klik je IZMJENA", "netgen/Form1.vb:3373,6935")
                : Qa.P(Ime, "TRG-03", "detail", "netgen/Form1.vb:3373", p.DetailGrid ?? ""));
            l.Add(Pet.GumbNaredba(p, 1, "FILTERFORM") && Pet.GumbNaredba(p, 5, "SVI")
                && (p.Buttons ?? "").IndexOf("Pregled svih:SVI", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "TRG-04", "BUTTONS i CMD nose iste naredbe, od Parametri do Pregled svih", "netgen/Form1.vb:3770-3796")
                : Qa.P(Ime, "TRG-04", "BUTTONS", "netgen/Form1.vb:3770", p.Buttons ?? ""));
            NgForm f = p.NadjiFormu("FILTERFORM");
            l.Add(f != null && Pet.AttrF(f, "FILLEXPLORER") == "TRUE" && Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "TRG-05", "FILTERFORM true zamijeni master. Checkbox je ugasen", "netgen/Form1.vb:5445-5452,2043")
                : Qa.P(Ime, "TRG-05", "filter", "netgen/Form1.vb:5445", f == null ? "" : Pet.AttrF(f, "FILLEXPLORER")));
            return l;
        }
    }

    public static class MpKase
    {
        public const string Ime = "MPKase";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "KSE-01", root, "definitions", "netgen", "BIOVEGA", "MPKase.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad != null && p.OnLoad.IndexOf("spMPKase @action=1", StringComparison.Ordinal) >= 0
                && p.DetailGrid != null && p.DetailGrid.IndexOf("@trgovinaid=@@id", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KSE-02", "ONLOAD je master SQL. Detail veze trgovinaid na @@id", "netgen/Form1.vb:3879-3882,3373")
                : Qa.P(Ime, "KSE-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            l.Add(Pet.Attr(p, "DETAILDBLCLICK") == "UPDATE_FORM"
                ? Qa.Z(Ime, "KSE-03", "Dvostruki klik detaila otvara UPDATE_FORM", "netgen/Form1.vb:3390,7201-7202")
                : Qa.P(Ime, "KSE-03", "DETAILDBLCLICK", "netgen/Form1.vb:3390", Pet.Attr(p, "DETAILDBLCLICK")));
            bool lista = false;
            for (int i = 0; i < p.PreskoceniGumbi.Count; i++)
                if (p.PreskoceniGumbi[i] == "CMD7CAPTION")
                    lista = true;
            l.Add(lista && Pet.GumbNaredba(p, 5, "ZAGLAVLJE") && Pet.Attr(p, "DETCHECKBOXVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "KSE-04", "Lista nema dvotocku. DETCHECKBOXVISIBLE je FALSE", "netgen/Form1.vb:3320,3461")
                : Qa.P(Ime, "KSE-04", "CMD7", "netgen/Form1.vb:3320", string.Join(",", p.PreskoceniGumbi.ToArray())));
            l.Add(Pet.Attr(p, "POPUPDETAIL").IndexOf("Izmjeni:UPDATEFORM", StringComparison.Ordinal) >= 0
                && p.NadjiFormu("UPDATEFORM") == null && p.NadjiFormu("UPDATE_FORM") != null
                ? Qa.Z(Ime, "KSE-05", "POPUPDETAIL puni izbornik detalja. UPDATEFORM nije UPDATE_FORM", "netgen/Form1.vb:3386,3702-3750,7225")
                : Qa.P(Ime, "KSE-05", "POPUPDETAIL", "netgen/Form1.vb:3386", Pet.Attr(p, "POPUPDETAIL")));
            return l;
        }
    }

    public static class FaktureMinMax
    {
        public const string Ime = "FaktureMinMaxDat";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "FMD-01", root, "definitions", "netgen", "BIOVEGA", "FaktureMinMaxDat.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":MAINFORM" && Pet.GumbNaredba(p, 1, "MAINFORM")
                ? Qa.Z(Ime, "FMD-02", "Ulaz i gumb otvaraju MAINFORM", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "FMD-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            NgForm f = p.NadjiFormu("MAINFORM");
            l.Add(f != null && Pet.AttrF(f, "MAXIMIZEBOX") == "FALSE"
                ? Qa.Z(Ime, "FMD-03", "MAXIMIZEBOX FALSE gasi povecanje dijaloga", "netgen/Form2.vb:1031")
                : Qa.P(Ime, "FMD-03", "MAXIMIZEBOX", "netgen/Form2.vb:1031", f == null ? "" : Pet.AttrF(f, "MAXIMIZEBOX")));
            l.Add(f != null && f.OnLoad != null && f.OnLoad.IndexOf("popuni_datume", StringComparison.Ordinal) >= 0 && Ima(f, "#OK")
                ? Qa.Z(Ime, "FMD-04", "ONLOAD puni datume. U redu salje promjenu i #OK", "netgen/Form2.vb:3531-3532")
                : Qa.P(Ime, "FMD-04", "datumi", "netgen/Form2.vb:3531", f == null ? "" : f.OnLoad));
            return l;
        }

        private static bool Ima(NgForm f, string dio)
        {
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                string s = f.Kontrole[i].Atributi.Vrijednost("ONCLICK") ?? "";
                if (s.IndexOf(dio, StringComparison.Ordinal) >= 0)
                    return true;
            }
            return false;
        }
    }

    public static class KntKonfSheme
    {
        public const string Ime = "KntKonfShemeMatTem";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "KSM-01", root, "definitions", "netgen", "BIOVEGA", "KntKonfShemeMatTem.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == "spKntKonfShemeMatTem 1"
                ? Qa.Z(Ime, "KSM-02", "ONLOAD je poziv s pozicijskim argumentom 1", "netgen/Form1.vb:3879-3882")
                : Qa.P(Ime, "KSM-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(Pet.GumbNaredba(p, 1, "INSERTFORM") && Pet.GumbNaredba(p, 4, "DELETEFORM") && p.MasterDblClick == "UPDATEFORM"
                ? Qa.Z(Ime, "KSM-03", "Gumbi su unos, izmjena, kopija i brisanje. Dvostruki klik je UPDATEFORM", "netgen/Form1.vb:3314,6935")
                : Qa.P(Ime, "KSM-03", "CMD", "netgen/Form1.vb:3314", Pet.OpisGumba(p, 2)));
            l.Add(Pet.Attr(p, "PAGEORIENTATION") == "LANDSCAPE" && Pet.NemaRedni(p, 5)
                ? Qa.Z(Ime, "KSM-04", "LANDSCAPE nema gumb ispisa pa se orijentacija ne primijeni", "netgen/Form1.vb:5930,4951")
                : Qa.P(Ime, "KSM-04", "orijentacija", "netgen/Form1.vb:5930", Pet.Attr(p, "PAGEORIENTATION")));
            return l;
        }
    }

    public static class SklMinMax
    {
        public const string Ime = "SklMinMaxDat";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "SMD-01", root, "definitions", "netgen", "BIOVEGA", "SklMinMaxDat.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":MAINFORM" && Pet.GumbNaredba(p, 1, "MAINFORM")
                ? Qa.Z(Ime, "SMD-02", "Ulaz i gumb otvaraju MAINFORM", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "SMD-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            NgForm f = p.NadjiFormu("MAINFORM");
            l.Add(f != null && Pet.AttrF(f, "MAXIMIZEBOX") == "FALSE"
                ? Qa.Z(Ime, "SMD-03", "MAXIMIZEBOX FALSE gasi povecanje dijaloga", "netgen/Form2.vb:1031")
                : Qa.P(Ime, "SMD-03", "MAXIMIZEBOX", "netgen/Form2.vb:1031", f == null ? "" : Pet.AttrF(f, "MAXIMIZEBOX")));
            bool ok = false;
            if (f != null)
            {
                for (int i = 0; i < f.Kontrole.Count; i++)
                {
                    string s = f.Kontrole[i].Atributi.Vrijednost("ONCLICK") ?? "";
                    if (s.IndexOf("promijeni_datume", StringComparison.Ordinal) >= 0 && s.IndexOf("#OK", StringComparison.Ordinal) >= 0)
                        ok = true;
                }
            }
            l.Add(f != null && f.OnLoad != null && f.OnLoad.IndexOf("spSktMinMaxDatum", StringComparison.Ordinal) >= 0 && ok
                ? Qa.Z(Ime, "SMD-04", "ONLOAD puni datume. U redu salje promjenu i #OK", "netgen/Form2.vb:3531-3532")
                : Qa.P(Ime, "SMD-04", "datumi", "netgen/Form2.vb:3531", f == null ? "" : f.OnLoad));
            return l;
        }
    }
}

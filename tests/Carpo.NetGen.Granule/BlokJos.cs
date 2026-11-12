using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class VrCjenik
    {
        public const string Ime = "VrCjenik";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string mer = Path.Combine(root, "definitions", "netgen", "MER", "vrcjenik.xml");
            if (!File.Exists(mer))
                l.Add(Qa.O(Ime, "VRC-00", "MER\\vrcjenik.xml nije u repou", "docs/granule/Prioritet.md", mer));
            else
                l.Add(Qa.Z(Ime, "VRC-00", "MER putanja postoji", mer));
            NgProject p = Pet.Ucitaj(l, Ime, "VRC-01", root, "definitions", "netgen", "Agronom granule", "VrCjenik.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad != null && p.OnLoad.IndexOf("spVrCjenikQuery_NG @action = 0", StringComparison.Ordinal) >= 0
                && p.MasterDblClick == "IZMJENA_VRC"
                ? Qa.Z(Ime, "VRC-02", "ONLOAD je master SQL. Dvostruki klik je IZMJENA_VRC", "netgen/Form1.vb:3879-3882,6935")
                : Qa.P(Ime, "VRC-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            l.Add(Pet.GumbNaredba(p, 1, "UNOS_VRC") && Pet.GumbNaredba(p, 5, "KOPIRAJ_VRC") && Pet.NemaRedni(p, 4)
                ? Qa.Z(Ime, "VRC-03", "Gumbi 1, 2, 3 i 5. CMD4 ne postoji", "netgen/Form1.vb:3314-3329")
                : Qa.P(Ime, "VRC-03", "CMD", "netgen/Form1.vb:3314", Pet.OpisGumba(p, 4)));
            l.Add(Pet.Attr(p, "DETCHECKBOXVISIBLE") == "FALSE" && Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE"
                && string.IsNullOrEmpty(p.DetailGrid)
                ? Qa.Z(Ime, "VRC-04", "DETCHECKBOXVISIBLE je FALSE, a DETAILGRID nema pa se detail ne crta", "netgen/Form1.vb:3461,2400-2406")
                : Qa.P(Ime, "VRC-04", "det", "netgen/Form1.vb:2402", Pet.Attr(p, "DETCHECKBOXVISIBLE")));
            NgForm unos = p.NadjiFormu("UNOS_VRC");
            l.Add(unos != null && Pet.AttrF(unos, "REFRESHAFTERCHILD") == "TRUE"
                ? Qa.Z(Ime, "VRC-05", "REFRESHAFTERCHILD na formi se ne cita. Cita se na projektu i na kontroli", "netgen/Form1.vb:3396; netgen/Form2.vb:3058")
                : Qa.P(Ime, "VRC-05", "refresh", "netgen/Form2.vb:3058", unos == null ? "" : Pet.AttrF(unos, "REFRESHAFTERCHILD")));
            return l;
        }
    }

    public static class InstrumentiPlacanja
    {
        public const string Ime = "InstrumentiPlacanjaNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "INS-01", root, "definitions", "netgen", "BIOVEGA", "InstrumentiPlacanjaNG.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad != null && p.OnLoad.IndexOf("spKntNacinPlacNG @action='Grid'", StringComparison.Ordinal) >= 0
                && p.MasterDblClick == ":INSTRUMPLACUPDATE"
                ? Qa.Z(Ime, "INS-02", "ONLOAD je master SQL. Dvostruki klik skine vodecu dvotocku i otvori INSTRUMPLACUPDATE", "netgen/Form1.vb:3879,4862-4867")
                : Qa.P(Ime, "INS-02", "onload", "netgen/Form1.vb:4862", p.MasterDblClick));
            l.Add(Pet.GumbNaredba(p, 1, "INSTRUMPLACINSERT#refreshmaster") && Pet.GumbNaredba(p, 5, "PRINTREPXMASTER")
                ? Qa.Z(Ime, "INS-03", "Unesi je forma pa refreshmaster. Ispis je PRINTREPXMASTER", "netgen/Form1.vb:4570,4945-4948")
                : Qa.P(Ime, "INS-03", "CMD", "netgen/Form1.vb:4570", Pet.OpisGumba(p, 1)));
            l.Add(Pet.Attr(p, "HELPKOD") == "MER252"
                ? Qa.Z(Ime, "INS-04", "HELPKOD MER252 pokaze ButtonHELP i F1 pokrene JupiterHelp.exe", "netgen/Form1.vb:3491-3494,7792-7799")
                : Qa.P(Ime, "INS-04", "HELPKOD", "netgen/Form1.vb:3491", Pet.Attr(p, "HELPKOD")));
            l.Add(p.Atributi.Vrijednost("SETDEFAULTS") != null && p.Atributi.Vrijednost("SETDEFAULTS").IndexOf("tkosmomi", StringComparison.Ordinal) >= 0
                && Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE" && Pet.Attr(p, "GROUPBYBOXVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "INS-05", "SETDEFAULTS puni tkosmomi. Checkbox je ugasen, group-by kutija je skrivena", "netgen/Form1.vb:3958,2043,3350")
                : Qa.P(Ime, "INS-05", "setdefault", "netgen/Form1.vb:3958", ""));
            return l;
        }
    }

    public static class MatTem
    {
        public const string Ime = "MatTem";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "MAT-01", root, "definitions", "netgen", "BIOVEGA", "MatTem.xml");
            if (p == null)
                return l;
            l.Add(p.OnLoad == ":FILTERFORM" && Pet.Attr(p, "COMMANDTIMEOUT") == "300" && Pet.Attr(p, "PAGEORIENTATION") == "LANDSCAPE"
                ? Qa.Z(Ime, "MAT-02", "Ulaz otvara FILTERFORM. Timeout je 300. LANDSCAPE ne dira PRINTMASTER", "netgen/Form1.vb:3879,3463,4951,5930")
                : Qa.P(Ime, "MAT-02", "onload", "netgen/Form1.vb:3879", p.OnLoad));
            l.Add(Pet.GumbNaredba(p, 1, "FILTERFORM") && Pet.GumbNaredba(p, 2, "PROKNJIZIFORM") && Pet.GumbNaredba(p, 3, "PRINTMASTER") && Pet.NemaRedni(p, 4)
                ? Qa.Z(Ime, "MAT-03", "Parametri, Proknjizi, Ispis. Razmak iza imena forme skine Trim", "netgen/Form1.vb:3326,4951")
                : Qa.P(Ime, "MAT-03", "CMD", "netgen/Form1.vb:3326", Pet.OpisGumba(p, 2)));
            NgForm f = p.NadjiFormu("FILTERFORM");
            l.Add(f != null && Pet.AttrF(f, "FILLEXPLORER") == "TRUE" && f.OnSubmit != null && f.OnSubmit.EndsWith("!!", StringComparison.Ordinal)
                ? Qa.Z(Ime, "MAT-04", "FILLEXPLORER true zamijeni master. !! gasi lijepljenje parametara", "netgen/Form1.vb:5445-5452")
                : Qa.P(Ime, "MAT-04", "filter", "netgen/Form1.vb:5449", f == null ? "" : f.OnSubmit));
            NgForm pro = p.NadjiFormu("PROKNJIZIFORM");
            l.Add(pro != null && pro.OnSubmit != null && pro.OnSubmit.StartsWith("spkntMatknjizenjequeryNG", StringComparison.Ordinal)
                ? Qa.Z(Ime, "MAT-05", "Proknjizi pocinje sa sp pa se niz parametara zalijepi", "netgen/Form2.vb:3378-3388")
                : Qa.P(Ime, "MAT-05", "proknjizi", "netgen/Form2.vb:3378", pro == null ? "" : pro.OnSubmit));
            return l;
        }
    }

    public static class MpPsNg
    {
        public const string Ime = "MPPSNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "MPSNG-01", root, "definitions", "netgen", "BIOVEGA", "MPPSNG.xml");
            if (p == null)
                return l;
            string tree = p.Tree ?? "";
            l.Add(tree.StartsWith("Granula >>> PS u MP:#", StringComparison.Ordinal)
                && tree.IndexOf("Danas:DANAS_FORM", StringComparison.Ordinal) >= 0
                && tree.IndexOf("spMPTreeNG @action=1:", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "MPSNG-02", "Prvi segment nema dvotocku i ne postane cvor. Store ide u DazovaIzStore", "netgen/Form1.vb:4298,4347-4356")
                : Qa.P(Ime, "MPSNG-02", "tree", "netgen/Form1.vb:4347", tree));
            l.Add(p.DetailGrid != null && p.DetailGrid.IndexOf("@id=@@id", StringComparison.Ordinal) >= 0
                && p.MasterDblClick == "UPDATE_FORM"
                ? Qa.Z(Ime, "MPSNG-03", "Detail veze @@id. Dvostruki klik je UPDATE_FORM", "netgen/Form1.vb:3373,6935")
                : Qa.P(Ime, "MPSNG-03", "detail", "netgen/Form1.vb:3373", p.DetailGrid ?? ""));
            bool lista = false;
            for (int i = 0; i < p.PreskoceniGumbi.Count; i++)
                if (p.PreskoceniGumbi[i] == "CMD7CAPTION")
                    lista = true;
            l.Add(lista && Pet.GumbNaredba(p, 6, "vrstadokid-MaloprodajaGla") && Pet.GumbNaredba(p, 8, "KONTROLAPS")
                ? Qa.Z(Ime, "MPSNG-04", "Lista nema dvotocku. Ispis je vrstadokid-. Kontrola otvara KONTROLAPS", "netgen/Form1.vb:3320,5215")
                : Qa.P(Ime, "MPSNG-04", "CMD", "netgen/Form1.vb:3320", Pet.OpisGumba(p, 6) + " " + string.Join(",", p.PreskoceniGumbi.ToArray())));
            string on2 = Pet.Attr(p, "ONLOAD2");
            l.Add(Pet.Attr(p, "AUTOFITCOLUMNS") == "False" && Pet.Attr(p, "COMMANDTIMEOUT") == "300"
                && on2.IndexOf("'Dodaj'", StringComparison.Ordinal) >= 0
                && Pet.Attr(p, "DETTOTALROWVISIBLE") == "FALSE"
                ? Qa.Z(Ime, "MPSNG-05", "AUTOFITCOLUMNS se ne cita. ONLOAD2 gleda natpis gumba. Timeout je 300", "netgen/Form1.vb:3914-3921,3463")
                : Qa.P(Ime, "MPSNG-05", "autofit", "netgen/Form1.vb:3914", Pet.Attr(p, "AUTOFITCOLUMNS")));
            l.Add(p.Popup != null && p.Popup.IndexOf("Izmjeni:UPDATEFORM", StringComparison.Ordinal) >= 0
                && p.NadjiFormu("UPDATEFORM") == null && p.NadjiFormu("UPDATE_FORM") != null
                ? Qa.Z(Ime, "MPSNG-06", "Popup trazi UPDATEFORM. Forma je UPDATE_FORM i dijalog se ne otvori", "netgen/Form1.vb:5369-5370")
                : Qa.P(Ime, "MPSNG-06", "popup", "netgen/Form1.vb:3385", p.Popup ?? ""));
            return l;
        }
    }

    public static class MpStanje
    {
        public const string Ime = "MPStanje";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "MPS-01", root, "definitions", "netgen", "BIOVEGA", "MPStanje.xml");
            if (p == null)
                return l;
            l.Add(string.IsNullOrEmpty(p.OnLoad) && string.IsNullOrEmpty(p.Buttons) && p.MasterDblClick == "KARTICA"
                && p.NadjiFormu("KARTICA") != null
                ? Qa.Z(Ime, "MPS-02", "Nema ONLOAD ni gumba. Dvostruki klik otvara KARTICA", "netgen/Form1.vb:3879,6935")
                : Qa.P(Ime, "MPS-02", "ulaz", "netgen/Form1.vb:3879", p.MasterDblClick));
            NgForm kart = p.NadjiFormu("KARTICA");
            l.Add(kart != null && Pet.AttrF(kart, "SKIPDEFAULTLOAD") == "TRUE" && Pet.Attr(p, "COMMANDTIMEOUT") == "300"
                ? Qa.Z(Ime, "MPS-03", "SKIPDEFAULTLOAD TRUE ne puni grid odmah. Timeout je 300", "netgen/Form2.vb:1635-1646; netgen/Form1.vb:3463")
                : Qa.P(Ime, "MPS-03", "skip", "netgen/Form2.vb:1643", kart == null ? "" : Pet.AttrF(kart, "SKIPDEFAULTLOAD")));
            NgForm main = p.NadjiFormu("MAINFORM");
            string inc = main == null ? "" : Pet.AttrF(main, "INCLUDE");
            string kartica = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPKarticaRobe.xml");
            if (inc == "MPKarticaRobe.xml" && !File.Exists(kartica))
                l.Add(Qa.O(Ime, "MPS-04", "INCLUDE trazi MPKarticaRobe.xml u istom folderu. Datoteke nema, cvorovi se ne spajaju", "netgen/Form2.vb:844-934", kartica));
            else if (inc == "MPKarticaRobe.xml")
                l.Add(Qa.Z(Ime, "MPS-04", "INCLUDE datoteka je stigla", kartica));
            else
                l.Add(Qa.P(Ime, "MPS-04", "INCLUDE", "netgen/Form2.vb:847", inc));
            l.Add(Pet.Attr(p, "CHECKBOXVISIBLE") == "FALSE" && string.IsNullOrEmpty(p.Tree)
                ? Qa.Z(Ime, "MPS-05", "Checkbox mastera je ugasen. TREE je prazan i stablo se sklanja", "netgen/Form1.vb:2043,4310-4316")
                : Qa.P(Ime, "MPS-05", "grid", "netgen/Form1.vb:2043", Pet.Attr(p, "CHECKBOXVISIBLE")));
            return l;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Granule
{
    public static class OdobreniKalo
    {
        public const string Ime = "OdobreniKaloNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "OKA-01", root, "definitions", "netgen", "BIOVEGA", "OdobreniKaloNG.xml");
            if (p == null)
                return l;
            NgForm kl = p.NadjiFormu("KlMaster");
            l.Add(string.IsNullOrEmpty(p.OnLoad) && p.Tree == "spTreeKlMasterRoba:KlMaster"
                && kl != null && Pet.AttrF(kl, "FILLEXPLORER") == "TRUE"
                && (kl.OnSubmit ?? "").IndexOf("@@node", StringComparison.Ordinal) >= 0
                && Pet.NemaRedni(p, 1)
                ? Qa.Z(Ime, "OKA-02", "Nema ONLOAD ni gumba. Stablo je store, KlMaster puni master preko @@node", "netgen/Form1.vb:4350,5445")
                : Qa.P(Ime, "OKA-02", "tree", "netgen/Form1.vb:4350", p.Tree ?? ""));
            NgProject a = Pet.Ucitaj(l, Ime, "OKA-03", root, "definitions", "netgen", "Agronom granule", "OdobreniKaloNG.xml");
            if (a == null)
                return l;
            NgForm kl2 = a.NadjiFormu("KlMaster");
            l.Add(a.Tree == p.Tree && kl2 != null && kl2.OnSubmit == kl.OnSubmit
                ? Qa.Z(Ime, "OKA-04", "Agronom ima isti TREE i isti ONSUBMIT", "definitions/netgen/Agronom granule/OdobreniKaloNG.xml")
                : Qa.P(Ime, "OKA-04", "agronom", "netgen/Form1.vb:4350", a.Tree ?? ""));
            return l;
        }
    }

    public static class KupStatN
    {
        public const string Ime = "KUPStatN";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "KST-01", root, "definitions", "netgen", "Agronom granule", "KUPStatN.xml");
            if (p == null)
                return l;
            l.Add((p.OnLoad ?? "").StartsWith("select kupstatusid", StringComparison.Ordinal)
                && Pet.GumbNaredba(p, 1, "INSERTFORM") && Pet.GumbNaredba(p, 3, "DELETEFORM")
                && p.MasterDblClick == "UPDATEFORM"
                ? Qa.Z(Ime, "KST-02", "Ulaz je select. Unesi, obrisi i dvostruki klik otvaraju forme", "netgen/Form1.vb:3879,3319")
                : Qa.P(Ime, "KST-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            NgForm brisi = p.NadjiFormu("DELETEFORM");
            l.Add((p.Popup ?? "").IndexOf("PREGLEDOPFORM", StringComparison.Ordinal) >= 0
                && brisi != null && (brisi.OnSubmit ?? "").IndexOf("@action=2", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KST-03", "Popup ima dvotocku. Brisanje je spKUPStatusAzurN @action=2", "netgen/Form1.vb:3564,5509")
                : Qa.P(Ime, "KST-03", "brisanje", "netgen/Form1.vb:5509", brisi == null ? "" : brisi.OnSubmit));
            return l;
        }
    }

    public static class BisnodeExport
    {
        public const string Ime = "BisnodeExport";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            NgProject p = Pet.Ucitaj(l, Ime, "BIS-01", root, "definitions", "netgen", "Agronom granule", "BisnodeExport.xml");
            if (p == null)
                return l;
            NgForm f = p.NadjiFormu("PARAM_FORM");
            string oc = f == null ? "" : OnclickDijalog.OnClick(f, "buttonOK", "#OK");
            l.Add(p.OnLoad == ":PARAM_FORM" && f != null
                && Pet.AttrF(f, "FILLEXPLORER").Equals("true", StringComparison.OrdinalIgnoreCase)
                && oc == "#OK"
                ? Qa.Z(Ime, "BIS-02", "Ulaz je PARAM_FORM. #OK i FILLEXPLORER true pune master", "netgen/Form2.vb:6821,6888")
                : Qa.P(Ime, "BIS-02", "onload", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            NgForm put = p.NadjiFormu("PATH");
            string upis = put == null ? "" : OnclickDijalog.OnClick(put, "button1", "update Pubvars");
            l.Add(Pet.GumbNaredba(p, 2, "Export") && upis.IndexOf("update Pubvars", StringComparison.Ordinal) >= 0
                && upis.IndexOf("#OK", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "BIS-03", "EXPORT otvara Export. Putanja prije OK pise update Pubvars", "netgen/Form2.vb:7509,3319")
                : Qa.P(Ime, "BIS-03", "putanja", "netgen/Form2.vb:7509", upis));
            return l;
        }
    }

    public static class Nedostaje96
    {
        private static readonly string[] Imena = new string[]
        {
            "kategorija.xml", "NacinObracunaZadatka.xml", "PovijestCjenika.xml", "PregledKnj.xml",
            "proces.xml", "PTOsobeCjenik.xml", "ptutrvrstarada.xml", "PTVrstaDok.xml",
            "SktSerBrojAzur.xml", "StatusProjekta.xml", "statusZadatka.xml", "TecajnaListaNG.xml",
            "VrstaProjekt.xml"
        };

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            for (int i = 0; i < Imena.Length; i++)
            {
                string fajl = Imena[i];
                string id = "N96-" + (i + 1).ToString("00");
                bool ima = File.Exists(Path.Combine(root, "definitions", "netgen", "BIOVEGA", fajl))
                    || File.Exists(Path.Combine(root, "definitions", "netgen", fajl))
                    || File.Exists(Path.Combine(root, "definitions", "netgen", "Agronom granule", fajl));
                if (!ima)
                    l.Add(Qa.O("Nedostaje96", id, fajl + " nije u definitions/netgen", "docs/granule/Prioritet.md", "rang 98-111"));
                else
                    l.Add(Qa.Z("Nedostaje96", id, "XML je stigao", fajl));
            }
            return l;
        }
    }

    public static class OnclickDijalog
    {
        public const string Ime = "OnclickDijalog";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            Tocno(l, root, "ONK-01", "Agronom granule", "ServisPrava.xml", "MAINFORM", "ButtonNG27", "CLOSE", "", "");
            Tocno(l, root, "ONK-02", "BIOVEGA", "MPePotvrdeNarudzbe.xml", "FILTERFORM", "ButtonNG26", "cancel", "TRUE", "");
            Tocno(l, root, "ONK-03", "BIOVEGA", "ProNotpL.xml", "MAINFORM", "button1", "OK", "", "");
            Tocno(l, root, "ONK-04", "Agronom granule", "ZakljucivanjeTemNG.xml", "MAINFORM", "buttonOK", ":Zakljucavanje", "", "");
            Tocno(l, root, "ONK-05", "BIOVEGA", "KntOPZ-STAT-1.xml", "UPIT", "buttonOK", "OK", "TRUE", "spKntOPZSTAT");
            Sadrzi(l, root, "ONK-06", "BIOVEGA", "KntBrutoBilancaNG.xml", "FILTER_FORM", "ok", "dbo.spKntBrutoBilanca_Pregled_Tree", "#OK", "ONLYTREE", "");
            Tocno(l, root, "ONK-07", "Agronom granule", "BisnodeExport.xml", "PARAM_FORM", "buttonOK", "#OK", "true", "spBisnodeExportQuery");
            Sadrzi(l, root, "ONK-08", "BIOVEGA", "SklMinMaxDat.xml", "MAINFORM", "cmdDodaj", "spSktMinMaxDatum", "#OK", "", "");
            Sadrzi(l, root, "ONK-09", "BIOVEGA", "KntPDVObrasciEU.xml", "FILTER_FORM", "ok", "update dbo.KntPDVObrasciEU", "#OK", "TRUE", "");
            Sadrzi(l, root, "ONK-10", "Agronom granule", "MPKasaImport.xml", "FILTERFORM", "ok", "BLAGAJNAENABLE(", "#OK", "TRUE", "spMPKasaCopy");
            return l;
        }

        private static void Tocno(List<Stavka> l, string root, string id, string folder, string fajl, string forma, string kontrola, string onclick, string fill, string submit)
        {
            ProvjeriJedan(l, root, id, folder, fajl, forma, kontrola, onclick, null, null, fill, submit);
        }

        private static void Sadrzi(List<Stavka> l, string root, string id, string folder, string fajl, string forma, string kontrola, string dio, string kraj, string fill, string submit)
        {
            ProvjeriJedan(l, root, id, folder, fajl, forma, kontrola, null, dio, kraj, fill, submit);
        }

        private static void ProvjeriJedan(List<Stavka> l, string root, string id, string folder, string fajl, string forma, string kontrola, string tocno, string dio, string kraj, string fill, string submit)
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
            string oc = OnClick(f, kontrola, tocno ?? dio);
            bool ok = oc != null;
            if (ok && tocno != null)
                ok = oc.Trim() == tocno;
            if (ok && dio != null)
                ok = oc.IndexOf(dio, StringComparison.Ordinal) >= 0;
            if (ok && kraj != null)
                ok = oc.TrimEnd().EndsWith(kraj, StringComparison.Ordinal);
            if (ok && fill.Length > 0)
                ok = Pet.AttrF(f, "FILLEXPLORER").Equals(fill, StringComparison.OrdinalIgnoreCase);
            if (ok && submit.Length > 0)
                ok = (f.OnSubmit ?? "").IndexOf(submit, StringComparison.Ordinal) >= 0;
            l.Add(ok
                ? Qa.Z(Ime, id, kontrola + " ONCLICK odgovara NetGen pravilu", "netgen/Form2.vb:6221,6808,6888,7031")
                : Qa.P(Ime, id, kontrola, "netgen/Form2.vb:2634", oc ?? "nema"));
        }

        public static string OnClick(NgForm f, string ime, string trag)
        {
            string pogodak = null;
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                NgControl c = f.Kontrole[i];
                if (!string.Equals(c.Name, ime, StringComparison.OrdinalIgnoreCase))
                    continue;
                string oc = Vrijednost(c, "ONCLICK");
                if (trag != null && oc.IndexOf(trag, StringComparison.Ordinal) < 0 && oc.Trim() != trag)
                    continue;
                pogodak = oc;
            }
            return pogodak;
        }

        private static string Vrijednost(NgControl c, string ime)
        {
            foreach (var kv in c.Atributi.Stavke)
            {
                if (string.Equals(kv.Key, ime, StringComparison.OrdinalIgnoreCase))
                    return kv.Value ?? "";
            }
            return "";
        }
    }
}

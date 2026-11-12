using System;
using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Jezik komandi i dijalozi. Pravila su iz Form1.NovaGenerickaForma / NovaGenerickaForma1
    /// i Form2.Rafalno / PokupiParametre / Button1_Click. Kuka bez kljuca = otvoreno.
    /// </summary>
    internal static class Komande
    {
        public const string Dml = "INSERTDELETEUPDATESELECT";

        public static bool Prvih6JeDml(string sql)
        {
            if (sql == null)
                return false;
            string s = sql.Trim();
            if (s.Length < 6)
                return false;
            return Dml.IndexOf(s.Substring(0, 6).ToUpperInvariant(), StringComparison.Ordinal) >= 0;
        }

        public static string[] Segmenti(string naredba)
        {
            if (naredba == null)
                return new string[0];
            return naredba.Replace("~", "#").Split('#');
        }

        public static NgControl Nadji(NgForm f, string ime)
        {
            if (f == null)
                return null;
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                if (string.Equals(f.Kontrole[i].Name, ime, StringComparison.OrdinalIgnoreCase))
                    return f.Kontrole[i];
            }
            return null;
        }

        public static void K(List<Stavka> l, string granula, string xml, string id, string opis, string izvor, string kljuc, string ocek)
        {
            l.Add(Qa.Kljuc(granula, id, opis, izvor, xml, "otvori", "", kljuc, ocek));
        }
    }

    public static class Kulture
    {
        public const string Ime = "Kulture";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "Kulture.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "KUL-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "KUL-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "KUL-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(p.OnLoad.StartsWith("spKultureNGQuery", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KUL-02", "Master je spKultureNGQuery, ne spKulture iz Carpo prepisa", "netgen/Form1.vb:3368")
                : Qa.P(Ime, "KUL-02", "Stari spKultureNGQuery", "netgen/Form1.vb:3368", p.OnLoad));
            l.Add(p.Gumbi.Count >= 3 && p.Gumbi[0].Naredba == "INSERT" && p.Gumbi[1].Naredba == "UPDATE" && p.Gumbi[2].Naredba == "DELETE"
                ? Qa.Z(Ime, "KUL-03", "CMD1-3 otvaraju INSERT, UPDATE, DELETE", "netgen/Form1.vb:3314-3316,5667")
                : Qa.P(Ime, "KUL-03", "CMD forme", "netgen/Form1.vb:3314", p.Gumbi.Count == 0 ? "" : p.Gumbi[0].Naredba));

            NgForm ins = p.NadjiFormu("INSERT");
            NgForm upd = p.NadjiFormu("UPDATE");
            NgForm del = p.NadjiFormu("DELETE");
            l.Add(ins != null && !Komande.Prvih6JeDml(ins.OnSubmit) && ins.OnSubmit.TrimEnd().EndsWith(",", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KUL-04", "INSERT ONSUBMIT pocinje sa sp, pa se parametri lijepe na kraj", "netgen/Form2.vb:3378-3388; netgen/Form1.vb:5514")
                : Qa.P(Ime, "KUL-04", "Lijepljenje parametara", "netgen/Form2.vb:3378", ins == null ? "" : ins.OnSubmit));
            l.Add(upd != null && upd.OnSubmit.IndexOf("@@ID", StringComparison.Ordinal) >= 0 && upd.OnLoad.IndexOf("@@ID", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KUL-05", "UPDATE nosi @@ID u ONSUBMIT i ONLOAD", "netgen/Form2.vb:1039-1042,2880")
                : Qa.P(Ime, "KUL-05", "UPDATE @@ID", "netgen/Form2.vb:2880", ""));
            NgControl cijenaIns = Komande.Nadji(ins, "Cijena");
            NgControl cijenaUpd = Komande.Nadji(upd, "Cijena");
            l.Add(cijenaIns != null && cijenaIns.Grana == NgGrana.Text && cijenaUpd != null && cijenaUpd.Grana == NgGrana.Number
                ? Qa.Z(Ime, "KUL-06", "Cijena je text na unosu i number na izmjeni", "netgen/Form2.vb:1996,2071")
                : Qa.P(Ime, "KUL-06", "Tip Cijena", "netgen/Form2.vb:2071", ""));
            NgControl search = Komande.Nadji(ins, "robaid");
            string sql = search == null ? "" : (search.Atributi.Vrijednost("SQL") ?? "");
            l.Add(search != null && search.Grana == NgGrana.Search && sql.StartsWith("roba#robaid#", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KUL-07", "Tražilica je tablica#id#kolone, ne sp/select", "netgen/Form3.vb:99-108")
                : Qa.P(Ime, "KUL-07", "Search SQL", "netgen/Form3.vb:102", sql));
            NgControl combo = Komande.Nadji(ins, "orgshemaid");
            l.Add(combo != null && combo.Grana == NgGrana.Combo
                && string.Equals(combo.Atributi.Vrijednost("ALLOWEMPTY"), "TRUE", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KUL-08", "Combo ALLOWEMPTY=TRUE ubaci prazan red, ne salje 1", "netgen/Form2.vb:2402-2411,3456")
                : Qa.P(Ime, "KUL-08", "ALLOWEMPTY", "netgen/Form2.vb:2403", ""));
            NgControl ok = Komande.Nadji(ins, "buttonOK");
            string klik = ok == null ? "" : (ok.Atributi.Vrijednost("ONCLICK") ?? "");
            l.Add(klik.IndexOf("spRaiserror", StringComparison.OrdinalIgnoreCase) >= 0 && klik.TrimEnd().EndsWith("#OK", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KUL-09", "Gumb je spRaiserror pa #OK. Greska prekida lanac prije OK", "netgen/Form2.vb:7510-7516,6888")
                : Qa.P(Ime, "KUL-09", "spRaiserror#OK", "netgen/Form2.vb:7510", klik));
            NgControl zatvori = Komande.Nadji(ins, "buttonCLOSE");
            l.Add(zatvori != null && string.Equals(zatvori.Atributi.Vrijednost("ONCLICK"), "CLOSE", StringComparison.Ordinal)
                ? Qa.Z(Ime, "KUL-10", "Odustani je CLOSE = Button2, OKClick false", "netgen/Form2.vb:7033-7034,3400")
                : Qa.P(Ime, "KUL-10", "CLOSE", "netgen/Form2.vb:7033", ""));
            NgForm par = p.NadjiFormu("PARAMETRI");
            NgControl dodaj = Komande.Nadji(par, "ButtonNG8");
            string dklik = dodaj == null ? "" : (dodaj.Atributi.Vrijednost("ONCLICK") ?? "");
            l.Add(dklik.IndexOf("as focus", StringComparison.OrdinalIgnoreCase) >= 0 && dklik.IndexOf("as Naziv", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KUL-11", "Dodaj parametra: focus kolona i refresh kontrole Naziv i stavke", "netgen/Form2.vb:4627-4632,4974-4998")
                : Qa.P(Ime, "KUL-11", "focus/refresh", "netgen/Form2.vb:4629", dklik));
            l.Add(Stablo.Otvori(Ime, "KUL-12", "Kulture.xml", root));

            Komande.K(l, Ime, xml, "KUL-20", "Prazan number ide kao 0, prazan text kao navodnici", "netgen/Form2.vb:3601,3655", "prazanBroj", "0");
            Komande.K(l, Ime, xml, "KUL-21", "Tražilica šalje Tag (id), ne tekst. Prazan tag je null", "netgen/Form2.vb:3626-3632", "searchParam", "tag");
            Komande.K(l, Ime, xml, "KUL-22", "Search SQL roba: id=robaid, LIKE na userid i naziv, ! stupac samo sort", "netgen/Form3.vb:102-126", "searchWhere", "roba");
            Komande.K(l, Ime, xml, "KUL-23", "Apostrof u tekstu postaje ¸¸ pa '' u SQL", "netgen/Form2.vb:3655; netgen/Class1.vb:850", "apostrof", "''");
            Komande.K(l, Ime, xml, "KUL-24", "spRaiserror bez #OK u tekstu poruke ne guta grešku i ne izvrši sljedeći #OK", "netgen/Class1.vb:187-189; netgen/Form2.vb:7511-7515", "raiserrorPrekida", "true");
            return l;
        }
    }

    public static class KarticaPartnera
    {
        public const string Ime = "KarticaPartnera";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KarticaPartnera.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "KAR-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "KAR-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "KAR-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(p.OnLoad.StartsWith(":", StringComparison.Ordinal) && p.OnLoad.IndexOf("FILTERFORM", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "KAR-02", "ONLOAD=:FILTERFORM otvara formu, ne ide u SQL", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "KAR-02", "ONLOAD forma", "netgen/Form1.vb:3879", p.OnLoad));
            NgForm f = p.NadjiFormu("FILTERFORM");
            l.Add(f != null && f.OnSubmit.Contains("!!") && !Komande.Prvih6JeDml(f.OnSubmit.Replace("!!", ""))
                ? Qa.Z(Ime, "KAR-03", "ONSUBMIT zavrsava s !! pa se niz @ime= ne lijepi", "netgen/Form1.vb:5449-5450")
                : Qa.P(Ime, "KAR-03", "!! gasi lijepljenje", "netgen/Form1.vb:5449", f == null ? "" : f.OnSubmit));
            l.Add(f != null && string.Equals(f.Atributi.Vrijednost("FILLEXPLORER"), "TRUE", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KAR-04", "FILLEXPLORER=TRUE: ONSUBMIT bez !! postane master upit", "netgen/Form1.vb:5445-5452")
                : Qa.P(Ime, "KAR-04", "FILLEXPLORER", "netgen/Form1.vb:5445", ""));
            NgControl dat = Komande.Nadji(f, "datod");
            l.Add(dat != null && dat.Grana == NgGrana.Date && string.Equals(dat.Atributi.Vrijednost("DEFAULT"), "firstdayofyear", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KAR-05", "Datum default firstdayofyear je 01.01. tekuce godine", "netgen/Form2.vb:2254-2255")
                : Qa.P(Ime, "KAR-05", "firstdayofyear", "netgen/Form2.vb:2254", ""));
            NgControl multi = Komande.Nadji(f, "kntplanaid");
            l.Add(multi != null && multi.Grana == NgGrana.MultiDrop
                ? Qa.Z(Ime, "KAR-06", "multidrop šalje označene id-eve odvojene tildom", "netgen/Form2.vb:3441-3447")
                : Qa.P(Ime, "KAR-06", "multidrop", "netgen/Form2.vb:3446", multi == null ? "" : multi.Grana.ToString()));
            NgControl chk = Komande.Nadji(f, "AnalitikaPS");
            l.Add(chk != null && chk.Grana == NgGrana.Check
                ? Qa.Z(Ime, "KAR-07", "check ide kao CheckState", "netgen/Form2.vb:1460,3424-3427")
                : Qa.P(Ime, "KAR-07", "check", "netgen/Form2.vb:3424", ""));
            NgControl val = Komande.Nadji(f, "DevValuteId");
            l.Add(val != null && string.Equals(val.Atributi.Vrijednost("ALLOWEMPTY"), "true", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KAR-08", "ALLOWEMPTY=true je case-insensitive", "netgen/Form2.vb:2403")
                : Qa.P(Ime, "KAR-08", "ALLOWEMPTY true", "netgen/Form2.vb:2403", ""));
            NgControl prikazi = Komande.Nadji(f, "ok");
            string klik = prikazi == null ? "" : (prikazi.Atributi.Vrijednost("ONCLICK") ?? "");
            l.Add(klik.IndexOf("+focus(partneriid)", StringComparison.OrdinalIgnoreCase) >= 0 && klik.IndexOf("##OK", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "KAR-09", "+focus u poruci i prazan segment ## pa OK", "netgen/Class1.vb:202-204; netgen/Form2.vb:6821")
                : Qa.P(Ime, "KAR-09", "focus i ##", "netgen/Class1.vb:202", klik));
            l.Add(f.Ime.IndexOf("filter", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "KAR-10", "Ime forme sadrzi filter: DB clipboard je ukljucen osim DBCLIPBOARD=false", "netgen/Form2.vb:1102-1104")
                : Qa.P(Ime, "KAR-10", "filter clipboard", "netgen/Form2.vb:1102", f.Ime));
            bool ispis = false;
            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                if (p.Gumbi[i].Naredba.StartsWith("PRINTMASTER", StringComparison.OrdinalIgnoreCase))
                    ispis = true;
            }
            l.Add(ispis
                ? Qa.Z(Ime, "KAR-11", "CMD3 PRINTMASTER ide u isti IspisGRIDREPX kao PRINTREPXMASTER", "netgen/Form1.vb:4951-4955")
                : Qa.P(Ime, "KAR-11", "PRINTMASTER", "netgen/Form1.vb:4951", ""));
            l.Add(Stablo.Otvori(Ime, "KAR-12", "KarticaPartnera.xml", root));

            Komande.K(l, Ime, xml, "KAR-20", "Datum ide kao yyyyMMdd u navodnicima", "netgen/Form2.vb:3531-3532", "datumFormat", "yyyyMMdd");
            Komande.K(l, Ime, xml, "KAR-21", "!! u captionu je novi red, na ONSUBMIT gasi lijepljenje", "netgen/Form2.vb:2943; netgen/Form1.vb:5449", "uzvicnik", "dva-znacenja");
            Komande.K(l, Ime, xml, "KAR-22", "+FOCUS(ime) vraca fokus i ne gasi grešku ako nema +OK", "netgen/Class1.vb:187-204", "focusNeGuta", "true");
            Komande.K(l, Ime, xml, "KAR-23", "Prazan segment ## ne preskače sljedeći OK", "netgen/Form2.vb:6821-6822", "prazanSegment", "nastavi");
            Komande.K(l, Ime, xml, "KAR-24", "repx naredba s #CLOSE zatvara formu nakon ispisa", "netgen/Form2.vb:7033; netgen/Form1.vb:4276", "repxClose", "CLOSE");
            return l;
        }
    }

    public static class KupNg
    {
        public const string Ime = "KUPNG";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KUPNG.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "KUP-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "KUP-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "KUP-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            NgButton cmd2 = null;
            NgButton cmd5 = null;
            NgButton cmd6 = null;
            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                if (p.Gumbi[i].Redni == 2) cmd2 = p.Gumbi[i];
                if (p.Gumbi[i].Redni == 5) cmd5 = p.Gumbi[i];
                if (p.Gumbi[i].Redni == 6) cmd6 = p.Gumbi[i];
            }
            string n2 = cmd2 == null ? "" : cmd2.Naredba;
            string[] seg = Komande.Segmenti(n2);
            bool imaRefresh = false;
            for (int i = 0; i < seg.Length; i++)
            {
                if (seg[i].ToUpperInvariant().Contains("REFRESHMASTERGRID"))
                    imaRefresh = true;
            }
            l.Add(n2.IndexOf('~') >= 0 && imaRefresh
                ? Qa.Z(Ime, "KUP-02", "Tilda u CMD postaje #, pa REFRESHMASTERGRID stvarno osvježi zadnji SQL", "netgen/Form1.vb:4558,4570-4577")
                : Qa.P(Ime, "KUP-02", "Tilda i REFRESHMASTERGRID", "netgen/Form1.vb:4558", n2));
            l.Add(cmd5 != null && cmd5.Naredba.StartsWith("MULTISELECT", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KUP-03", "Ime koje pocinje s MULTISELECT puni TotalGenMultiSelectTemp", "netgen/Form1.vb:4932-5018")
                : Qa.P(Ime, "KUP-03", "MULTISELECT", "netgen/Form1.vb:4932", cmd5 == null ? "" : cmd5.Naredba));
            l.Add(cmd6 != null && cmd6.Naredba.StartsWith("MULTIBLUE", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "KUP-04", "MULTIBLUE uzima plave (SelectedItems), ne cekirane", "netgen/Form1.vb:5022-5032")
                : Qa.P(Ime, "KUP-04", "MULTIBLUE", "netgen/Form1.vb:5022", cmd6 == null ? "" : cmd6.Naredba));
            l.Add(Stablo.Otvori(Ime, "KUP-05", "KUPNG.xml", root));

            Komande.K(l, Ime, xml, "KUP-10", "Temp tablica: delete po suser_sname i po operateriid, pa insert id, host, suser, operater", "netgen/Form1.vb:4934-5018", "multiTemp", "delete-dva-pa-insert");
            Komande.K(l, Ime, xml, "KUP-11", "FOREACHSELECTED na dijalogu vrti cekirane retke grida i sljedeci segment", "netgen/Form2.vb:7040-7042,8337-8354", "foreachSelected", "cekirani");
            Komande.K(l, Ime, xml, "KUP-12", "FOREACHROW vrti sve retke grida. Na podlozi Form1 petlja je komentar", "netgen/Form2.vb:7046-7048,8310-8327; netgen/Form1.vb:6843-6846", "foreachRowDijalog", "zivo");
            Komande.K(l, Ime, xml, "KUP-13", "FOREACHBLUEROW vrti SelectedItems redom od 0", "netgen/Form2.vb:7043-7045,8363-8394", "foreachBlue", "plavi");
            Komande.K(l, Ime, xml, "KUP-14", "Radio šalje 1 ili 0", "netgen/Form2.vb:3432-3435", "radio", "1-0");
            Komande.K(l, Ime, xml, "KUP-15", "REFRESHCONTROL(ime) radi select 'refresh' as ime", "netgen/Form2.vb:6911-6915", "refreshControl", "select refresh");
            Komande.K(l, Ime, xml, "KUP-16", "CANCEL i CLOSE su Button2. CLOSEPARENT jos postavi bCloseParent", "netgen/Form2.vb:7031-7037", "cancelClose", "button2");
            Komande.K(l, Ime, xml, "KUP-17", "OKSTAY ne zatvara formu, OK zatvara", "netgen/Form2.vb:6884-6890", "okStay", "ne-zatvara");
            return l;
        }
    }
}

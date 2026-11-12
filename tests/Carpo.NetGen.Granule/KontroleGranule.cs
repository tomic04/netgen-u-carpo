using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using Carpo.NetGen.Kontrole;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// InStr redoslijed TYPE iz Form2.ProcitajXML (1256-2707) i atributi kontrola.
    /// Kuka bez kljuca je otvorena. Kriva grana parsera je pad.
    /// </summary>
    internal static class Kontrole
    {
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

        public static int Broj(NgForm f, NgGrana g)
        {
            if (f == null)
                return 0;
            int n = 0;
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                if (f.Kontrole[i].Grana == g)
                    n++;
            }
            return n;
        }

        public static string A(NgControl c, string ime)
        {
            if (c == null || c.Atributi == null || !c.Atributi.Ima(ime))
                return null;
            return c.Atributi.Vrijednost(ime) ?? "";
        }

        public static bool AllowTrue(string s)
        {
            return s != null && s.Trim().ToLowerInvariant() == "true";
        }

        /// <summary>
        /// Form2.vb 2252-2267. DEFAULT se spusti na mala slova. Samo cetiri firstday* se prepisuju.
        /// </summary>
        public static string DatumPocetni(string raw, int godina, int mjesec)
        {
            if (raw == null)
                return null;
            string t = raw.ToLowerInvariant();
            if (t == "firstdayofyear")
                return "01.01." + godina.ToString();
            if (t == "firstdayoflastyear")
                return "01.01." + (godina - 1).ToString();
            if (t == "firstdayofmonth")
                return "01." + mjesec.ToString() + "." + godina.ToString();
            if (t == "firstdayoflastmonth")
                return "01." + (mjesec - 1).ToString() + "." + godina.ToString();
            return t;
        }

        public static string OblikTrazilice(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return "prazno";
            string s = sql.Trim();
            if (s.StartsWith("sp", StringComparison.OrdinalIgnoreCase))
                return "sp";
            if (s.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                return "select";
            return "hash";
        }

        public static void K(List<Stavka> l, string granula, string xml, string id, string opis, string izvor, string kljuc, string ocek)
        {
            l.Add(Qa.Kljuc(granula, id, opis, izvor, xml, "otvori", "", kljuc, ocek));
        }

        public static Stavka Objekti(string granula, string id, params string[] imena)
        {
            var sb = new StringBuilder();
            sb.Append("select name from sys.objects where name in (");
            for (int i = 0; i < imena.Length; i++)
            {
                if (i > 0)
                    sb.Append(",");
                sb.Append("'");
                sb.Append(imena[i].Replace("'", ""));
                sb.Append("'");
            }
            sb.Append(")");
            string greska;
            List<string> redovi = Citanje.Stupac(sb.ToString(), out greska);
            if (redovi == null)
                return Qa.O(granula, id, "sys.objects (samo citanje)", "spin", greska);
            if (redovi.Count < imena.Length)
                return Qa.O(granula, id, "nisu svi objekti na spin", "sys.objects", string.Join(",", redovi.ToArray()));
            return Qa.Z(granula, id, "objekti postoje: " + string.Join(",", redovi.ToArray()), "sys.objects");
        }
    }

    public static class KontrolePravila
    {
        public const string Ime = "Kontrole";

        public static List<Stavka> Provjeri()
        {
            var l = new List<Stavka>();
            l.Add(Grana("KON-01", "checkbox", NgGrana.Check, "checkbox sadrzi check", "netgen/Form2.vb:1460"));
            l.Add(Grana("KON-02", "vb.checkbox", NgGrana.Check, "vb.checkbox sadrzi check", "netgen/Form2.vb:1460"));
            l.Add(Grana("KON-03", "CMEditVB6.CurMEdit", NgGrana.Number, "cur bilo gdje je NumerBox", "netgen/Form2.vb:2071"));
            l.Add(Grana("KON-04", "datetiem", NgGrana.Date, "datetiem sadrzi date", "netgen/Form2.vb:2232"));
            l.Add(Grana("KON-05", "datetime", NgGrana.Date, "datetime sadrzi date", "netgen/Form2.vb:2232"));
            l.Add(Grana("KON-06", "VB.textbox", NgGrana.Text, "vb.textbox sadrzi text", "netgen/Form2.vb:1996"));
            l.Add(Grana("KON-07", "b_utton", NgGrana.Nijedna, "b_utton nema button", "netgen/Form2.vb:2614"));
            l.Add(Grana("KON-08", "picture", NgGrana.Nijedna, "picture nema picturebox", "netgen/Form2.vb:2707"));
            l.Add(Grana("KON-09", "frame", NgGrana.Group, "frame je GroupBox", "netgen/Form2.vb:1354"));
            l.Add(Grana("KON-10", "group", NgGrana.Group, "group je isti GroupBox", "netgen/Form2.vb:1354"));
            l.Add(Grana("KON-11", "dxgrid", NgGrana.DxGrid, "dxgrid prije grid", "netgen/Form2.vb:1579,1623"));
            l.Add(Grana("KON-12", "SEARCH", NgGrana.Search, "SEARCH bez obzira na slova", "netgen/Form2.vb:2140"));
            l.Add(Grana("KON-13", "statusstrip", NgGrana.StatusStrip, "statusstrip je prva grana", "netgen/Form2.vb:1256"));
            l.Add(Grana("KON-14", "panel", NgGrana.Panel, "panel", "netgen/Form2.vb:1384"));
            l.Add(Grana("KON-15", "multidrop", NgGrana.MultiDrop, "multidrop", "netgen/Form2.vb:2478"));
            l.Add(Grana("KON-16", "radio", NgGrana.Radio, "radio", "netgen/Form2.vb:1509"));
            l.Add(Grana("KON-17", "trazilica", NgGrana.Search, "trazilica je search", "netgen/Form2.vb:2140"));

            string mjesec = Kontrole.DatumPocetni("firstdayofmonth", 2026, 11);
            l.Add(mjesec == "01.11.2026"
                ? Qa.Z(Ime, "KON-20", "firstdayofmonth je 01. + mjesec + . + godina, bez vodece nule", "netgen/Form2.vb:2260-2261")
                : Qa.P(Ime, "KON-20", "firstdayofmonth", "netgen/Form2.vb:2260", mjesec));
            string today = Kontrole.DatumPocetni("TODAY", 2026, 11);
            l.Add(today == "today"
                ? Qa.Z(Ime, "KON-21", "TODAY nije kljucna rijec. Spusti se na mala slova i ostane literal", "netgen/Form2.vb:2253-2267")
                : Qa.P(Ime, "KON-21", "TODAY", "netgen/Form2.vb:2253", today));
            string sij = Kontrole.DatumPocetni("firstdayoflastmonth", 2026, 1);
            l.Add(sij == "01.0.2026"
                ? Qa.Z(Ime, "KON-22", "firstdayoflastmonth u sijecnju da mjesec 0", "netgen/Form2.vb:2263-2264")
                : Qa.P(Ime, "KON-22", "firstdayoflastmonth", "netgen/Form2.vb:2263", sij));
            l.Add(Kontrole.DatumPocetni("firstdayofyear", 2026, 6) == "01.01.2026" && Kontrole.DatumPocetni("firstdayoflastyear", 2026, 6) == "01.01.2025"
                ? Qa.Z(Ime, "KON-23", "firstdayofyear i firstdayoflastyear", "netgen/Form2.vb:2254-2258")
                : Qa.P(Ime, "KON-23", "firstdayofyear", "netgen/Form2.vb:2254", ""));
            l.Add(Kontrole.AllowTrue("TRUE") && Kontrole.AllowTrue("true") && !Kontrole.AllowTrue("") && !Kontrole.AllowTrue("FALSE")
                ? Qa.Z(Ime, "KON-24", "ALLOWEMPTY je tocno true, prazan string nije", "netgen/Form2.vb:2403")
                : Qa.P(Ime, "KON-24", "ALLOWEMPTY", "netgen/Form2.vb:2403", ""));
            l.Add(Kontrole.OblikTrazilice("spMPRobaZaMPNG @trazi='@@searchboxrobaid'") == "sp"
                && Kontrole.OblikTrazilice("select r.robaid from roba r") == "select"
                && Kontrole.OblikTrazilice("dbo.spNesto @a=1") == "hash"
                && Kontrole.OblikTrazilice("partneri#partneriid#naziv%") == "hash"
                ? Qa.Z(Ime, "KON-25", "Trazilica: sp/select ostaje SQL. dbo.sp nije sp. Inace tablica#", "netgen/Form3.vb:99-102")
                : Qa.P(Ime, "KON-25", "oblik trazilice", "netgen/Form3.vb:99", ""));
            return l;
        }

        private static Stavka Grana(string id, string type, NgGrana ocek, string opis, string izvor)
        {
            NgGrana g = NgType.Odredi(type);
            if (g == ocek)
                return Qa.Z(Ime, id, opis + " (" + type + ")", izvor);
            return Qa.P(Ime, id, opis, izvor, type + " -> " + g.ToString());
        }
    }

    public static class KntDospjeleStavke
    {
        public const string Ime = "KntDospjeleStavke";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "KntDospjeleStavke.xml");
            NgProject p = Ucitaj(l, "DOS-01", xml);
            if (p == null)
                return l;
            NgForm f = p.NadjiFormu("MAINFORM");
            NgControl status = Kontrole.Nadji(f, "s1");
            NgControl frame = Kontrole.Nadji(f, "FrameNG9");
            NgControl konto = Kontrole.Nadji(f, "kntplanaid");
            NgControl klpid = Kontrole.Nadji(f, "klpid");
            NgControl mjesta = Kontrole.Nadji(f, "mjestaid");
            NgControl zup = Kontrole.Nadji(f, "zupanijeid");
            NgControl partner = Kontrole.Nadji(f, "partneriid");
            NgControl iznos = Kontrole.Nadji(f, "iznos");
            NgControl datum = Kontrole.Nadji(f, "datumod");

            l.Add(status != null && status.Grana == NgGrana.StatusStrip && Kontrole.A(status, "TEXT1") != null && Kontrole.A(status, "TEXT3") == null
                ? Qa.Z(Ime, "DOS-02", "statusstrip TEXT1 postoji, TEXT3 ne. Tag je velikim slovima, OERN na treci", "netgen/Form2.vb:1256-1260")
                : Qa.P(Ime, "DOS-02", "statusstrip", "netgen/Form2.vb:1256", ""));
            l.Add(frame != null && frame.Grana == NgGrana.Group
                ? Qa.Z(Ime, "DOS-03", "frame je GroupBox", "netgen/Form2.vb:1354")
                : Qa.P(Ime, "DOS-03", "frame", "netgen/Form2.vb:1354", ""));
            l.Add(klpid != null && klpid.Grana == NgGrana.Combo && Kontrole.AllowTrue(Kontrole.A(klpid, "ALLOWEMPTY")) && Kontrole.A(klpid, "DATAFIELD") == null
                ? Qa.Z(Ime, "DOS-04", "combo ALLOWEMPTY true, nema DATAFIELD. Vrijednost je stupac 0", "netgen/Form2.vb:2370,2403-2411,2430-2431")
                : Qa.P(Ime, "DOS-04", "ALLOWEMPTY", "netgen/Form2.vb:2403", ""));
            string lost = Kontrole.A(zup, "ONLOSTFOCUS") ?? "";
            string sqlMj = Kontrole.A(mjesta, "SQL") ?? "";
            l.Add(lost.IndexOf("refresh", StringComparison.OrdinalIgnoreCase) >= 0 && lost.IndexOf("mjestaid", StringComparison.OrdinalIgnoreCase) >= 0
                && sqlMj.IndexOf("@@zupanijeid", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "DOS-05", "zupanijeid ONLOSTFOCUS vrati refresh za mjestaid, SQL djeteta cita @@zupanijeid", "netgen/Form2.vb:5905,7510,5158-5182")
                : Qa.P(Ime, "DOS-05", "zavisni combo", "netgen/Form2.vb:5158", lost));
            string psql = Kontrole.A(partner, "SQL") ?? "";
            l.Add(partner != null && partner.Grana == NgGrana.Search && Kontrole.OblikTrazilice(psql) == "hash" && psql.IndexOf("!Sifra", StringComparison.Ordinal) >= 0 && psql.IndexOf("Naziv%", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "DOS-06", "search je tablica#id#kolone. % LIKE, ! nije u WHERE", "netgen/Form3.vb:99-133")
                : Qa.P(Ime, "DOS-06", "search hash", "netgen/Form3.vb:102", psql));
            l.Add(konto != null && konto.Grana == NgGrana.MultiDrop
                ? Qa.Z(Ime, "DOS-07", "multidrop salje id1~id2", "netgen/Form2.vb:2478,3446")
                : Qa.P(Ime, "DOS-07", "multidrop", "netgen/Form2.vb:2478", ""));
            l.Add(iznos != null && iznos.Grana == NgGrana.Number && Kontrole.A(iznos, "DECIMALS") == "2" && Kontrole.A(iznos, "DEFAULT") == null && Kontrole.A(iznos, "FORMATMASK") == null
                ? Qa.Z(Ime, "DOS-08", "number DECIMALS=2, nema DEFAULT. Prazno salje 0", "netgen/Form2.vb:2084-2086,3601")
                : Qa.P(Ime, "DOS-08", "number", "netgen/Form2.vb:3601", ""));
            l.Add(datum != null && datum.Grana == NgGrana.Date && Kontrole.A(datum, "DEFAULT") == null && Kontrole.A(datum, "SHOWCHECKBOX") == null
                ? Qa.Z(Ime, "DOS-09", "datum bez DEFAULT i bez SHOWCHECKBOX. Nije today", "netgen/Form2.vb:2252,2275,3531")
                : Qa.P(Ime, "DOS-09", "datum", "netgen/Form2.vb:2252", ""));
            l.Add(Stablo.Meni(Ime, "DOS-10", "KntDospjeleStavke.xml"));
            l.Add(DospjeleNaSpinu());
            Kontrole.K(l, Ime, xml, "DOS-20", "statusstrip TEXT1 u tag velikim slovima, @@rowcount i dalje se nadje", "netgen/Form2.vb:1258; netgen/Class1.vb:865; netgen/Form2.vb:3818", "status.s1", "TEXT1-upper");
            Kontrole.K(l, Ime, xml, "DOS-21", "zupanijeid refresh ponovo puni mjestaid i vrati ALLOWEMPTY red", "netgen/Form2.vb:5158-5176", "combo.refresh.mjestaid", "refresh+allowempty");
            Kontrole.K(l, Ime, xml, "DOS-22", "prazan iznos ide kao 0", "netgen/Form2.vb:3601", "number.iznos", "0");
            Kontrole.K(l, Ime, xml, "DOS-23", "multidrop kntplanaid je id1~id2 bez razmaka", "netgen/Form2.vb:3446", "multidrop.kntplanaid", "tilde");
            return l;
        }

        private static Stavka DospjeleNaSpinu()
        {
            const string izvor = "netgen/Form2.vb:2430-2431,2503-2504";
            string[] problemi = new string[4];
            problemi[0] = Stupci("dbo.spKntPopuniKontroleNG @Action = 'konto'", false, "1", "1000");
            problemi[1] = Stupci("dbo.spKntPopuniKontroleNG @Action = 'klpartneri'", true, "", "");
            problemi[2] = Stupci("spOrgShemaQuery @action=0", false, "1", "Test shema");
            string[] greska = new string[1];
            DataTable grid = ComboDijalog.Ucitaj("dbo.spKntOtvStavkeQueryNG", false, delegate(string s)
            {
                string e;
                DataTable t = Citanje.Tablica(s, out e);
                greska[0] = e;
                return t;
            });
            if (grid == null || grid.Rows.Count == 0)
                problemi[3] = greska[0] ?? "grid nije vratio red";
            else
                problemi[3] = null;
            for (int i = 0; i < problemi.Length; i++)
            {
                if (problemi[i] != null)
                    return Qa.P(Ime, "DOS-11", "Carpo veze stupac 0 i 1 kao NetGen i pozove procedure", izvor, problemi[i]);
            }
            return Qa.Z(Ime, "DOS-11", "konto 1/1000, prazan red na klasi, shema 1/Test shema, grid ima red", izvor);
        }

        private static string Stupci(string sql, bool prazan, string ocek0, string ocek1)
        {
            string[] g = new string[1];
            DataTable dt = ComboDijalog.Ucitaj(sql, prazan, delegate(string s)
            {
                string e;
                DataTable t = Citanje.Tablica(s, out e);
                g[0] = e;
                return t;
            });
            if (dt == null || dt.Rows.Count == 0 || dt.Columns.Count < 2)
                return sql + " " + (g[0] ?? "nema stupca 1");
            string a = Convert.ToString(dt.Rows[0][0]);
            string b = Convert.ToString(dt.Rows[0][1]);
            if (a != ocek0 || b != ocek1)
                return sql + " stupac0=[" + a + "] stupac1=[" + b + "]";
            return null;
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
                l.Add(Qa.P(Ime, id, "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return null;
            }
            l.Add(Qa.Z(Ime, id, "Projekt se ucita", "netgen/Form1.vb:3263"));
            return p;
        }
    }

    public static class MPInventura
    {
        public const string Ime = "MPInventura";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "MPInventura.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "INV-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "INV-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "INV-01", "Projekt se ucita", "netgen/Form1.vb:3263"));

            NgForm filter = p.NadjiFormu("FILTERFORM");
            NgControl od = Kontrole.Nadji(filter, "datumod");
            NgControl dok = Kontrole.Nadji(filter, "datumdo");
            string defOd = Kontrole.A(od, "DEFAULT");
            l.Add(od != null && defOd == "firstdayofmonth" && Kontrole.DatumPocetni(defOd, 2026, 11) == "01.11.2026"
                ? Qa.Z(Ime, "INV-02", "datumod DEFAULT=firstdayofmonth", "netgen/Form2.vb:2260-2261")
                : Qa.P(Ime, "INV-02", "firstdayofmonth", "netgen/Form2.vb:2260", defOd ?? ""));
            l.Add(dok != null && Kontrole.A(dok, "DEFAULT") == null && Kontrole.A(dok, "DEFALUT") != null
                ? Qa.Z(Ime, "INV-03", "datumdo ima DEFALUT, ne DEFAULT. today se ne primijeni", "netgen/Form2.vb:2252")
                : Qa.P(Ime, "INV-03", "DEFALUT", "netgen/Form2.vb:2252", ""));

            NgForm unos = p.NadjiFormu("INSERTROBANE_FORM");
            NgControl act = Kontrole.Nadji(unos, "datumact");
            string defAct = Kontrole.A(act, "DEFAULT");
            l.Add(defAct == "TODAY" && Kontrole.DatumPocetni(defAct, 2026, 11) == "today"
                ? Qa.Z(Ime, "INV-04", "DEFAULT=TODAY nije kljucna rijec, literal today", "netgen/Form2.vb:2253-2267")
                : Qa.P(Ime, "INV-04", "TODAY", "netgen/Form2.vb:2253", defAct ?? ""));

            NgForm ins = p.NadjiFormu("INSERTFORM");
            NgControl trg = Kontrole.Nadji(ins, "trgovinaid");
            NgControl sort = Kontrole.Nadji(ins, "sortpoid");
            NgControl chk = Kontrole.Nadji(ins, "knjustv");
            NgControl hash = Kontrole.Nadji(ins, "partneriid");
            string onclick = Kontrole.A(trg, "ONCLICK") ?? "";
            l.Add(!Kontrole.AllowTrue(Kontrole.A(trg, "ALLOWEMPTY")) && onclick.IndexOf("refresh", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "INV-05", "ALLOWEMPTY FALSE i ONCLICK refresh osobeid/skladisteid", "netgen/Form2.vb:2403,8497-8506,5158")
                : Qa.P(Ime, "INV-05", "refresh combo", "netgen/Form2.vb:5158", onclick));
            string sortSql = Kontrole.A(sort, "SQL") ?? "";
            l.Add(sort != null && sortSql.IndexOf("1 as sortpoid", StringComparison.Ordinal) >= 0 && !Kontrole.AllowTrue(Kontrole.A(sort, "ALLOWEMPTY"))
                ? Qa.Z(Ime, "INV-06", "bez ALLOWEMPTY, SelectedValue=1 pogodi prvi red", "netgen/Form2.vb:3459-3464")
                : Qa.P(Ime, "INV-06", "SelectedValue 1", "netgen/Form2.vb:3459", ""));
            l.Add(chk != null && chk.Grana == NgGrana.Check && string.Equals(chk.TypeRaw, "check", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "INV-07", "check salje CheckState", "netgen/Form2.vb:1460,3424-3427")
                : Qa.P(Ime, "INV-07", "check", "netgen/Form2.vb:1460", chk == null ? "" : chk.TypeRaw));
            string hsql = Kontrole.A(hash, "SQL") ?? "";
            l.Add(Kontrole.OblikTrazilice(hsql) == "hash" && hsql.IndexOf("#!adresa", StringComparison.Ordinal) >= 0 && hsql.IndexOf("%naziv%", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "INV-08", "vipartneri# : % LIKE, ! izvan WHERE", "netgen/Form3.vb:102-133")
                : Qa.P(Ime, "INV-08", "hash search", "netgen/Form3.vb:118", hsql));

            NgForm grupa = p.NadjiFormu("INSERTGROUP_FORM");
            NgControl reg = Kontrole.Nadji(grupa, "mpregijeId");
            NgControl md = Kontrole.Nadji(grupa, "str");
            l.Add(Kontrole.AllowTrue(Kontrole.A(reg, "ALLOWEMPTY")) && (Kontrole.A(reg, "ONCLICK") ?? "").IndexOf("str", StringComparison.OrdinalIgnoreCase) >= 0
                && md != null && md.Grana == NgGrana.MultiDrop
                ? Qa.Z(Ime, "INV-09", "combo refresh puni multidrop. ALLOWEMPTY na multidropu se ne primjenjuje kao na combu", "netgen/Form2.vb:2495,2500,4813-4821")
                : Qa.P(Ime, "INV-09", "multidrop refresh", "netgen/Form2.vb:4813", ""));

            NgForm kart = p.NadjiFormu("MPKARTICA");
            NgControl rad = Kontrole.Nadji(kart, "rbtnKumulativ");
            l.Add(rad != null && rad.Grana == NgGrana.Radio
                ? Qa.Z(Ime, "INV-10", "radio salje 1 ili 0", "netgen/Form2.vb:1509,3432-3435")
                : Qa.P(Ime, "INV-10", "radio", "netgen/Form2.vb:3432", ""));

            NgForm trazi = p.NadjiFormu("TRAZI");
            NgControl kol = Kontrole.Nadji(trazi, "kolicinastvarna");
            NgControl cij = Kontrole.Nadji(trazi, "cijena");
            NgControl sp = Kontrole.Nadji(trazi, "robaid");
            l.Add(kol != null && Kontrole.A(kol, "DECIMALS") == "4" && cij != null && Kontrole.A(cij, "DECIMALS") == null
                ? Qa.Z(Ime, "INV-11", "DECIMALS samo kad atribut postoji. Bez njega OERN", "netgen/Form2.vb:2085-2086")
                : Qa.P(Ime, "INV-11", "DECIMALS", "netgen/Form2.vb:2086", ""));
            l.Add(Kontrole.OblikTrazilice(Kontrole.A(sp, "SQL") ?? "") == "sp"
                ? Qa.Z(Ime, "INV-12", "robaid SQL pocinje sa sp, ostaje taj tekst", "netgen/Form3.vb:99-100")
                : Qa.P(Ime, "INV-12", "sp search", "netgen/Form3.vb:99", Kontrole.A(sp, "SQL") ?? ""));

            l.Add(Stablo.Meni(Ime, "INV-13", "MPInventura.xml"));
            string ocekOd = Kontrole.DatumPocetni("firstdayofmonth", DateTime.Today.Year, DateTime.Today.Month);
            Kontrole.K(l, Ime, xml, "INV-20", "datumod je prvi dan ovog mjeseca, format 01.m.gggg", "netgen/Form2.vb:2260-2261", "date.datumod", ocekOd);
            Kontrole.K(l, Ime, xml, "INV-21", "DEFALUT=today se ignorira", "netgen/Form2.vb:2252", "date.datumdo", "nema-default");
            Kontrole.K(l, Ime, xml, "INV-22", "radio rbtnKumulativ je 1 ili 0", "netgen/Form2.vb:3432-3435", "radio.rbtnKumulativ", "1-0");
            return l;
        }
    }

    public static class FakPar
    {
        public const string Ime = "FakPar";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "FakPar.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "FAK-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "FAK-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "FAK-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            l.Add(p.Atributi.Ima("GRUOPBYBOXVISIBLE") && !p.Atributi.Ima("GROUPBYBOXVISIBLE")
                ? Qa.Z(Ime, "FAK-02", "GRUOPBYBOXVISIBLE nije GROUPBYBOXVISIBLE", "netgen/Form1.vb:3350")
                : Qa.P(Ime, "FAK-02", "tipfeler grupe", "netgen/Form1.vb:3350", ""));

            NgForm f = p.NadjiFormu("INSERTFORM");
            NgControl cur = Kontrole.Nadji(f, "Rabat");
            NgControl uc = Kontrole.Nadji(f, "Ucestalost");
            NgControl grup = Kontrole.Nadji(f, "grupiranje");
            NgControl ziro = Kontrole.Nadji(f, "parziroid");
            NgControl chk = Kontrole.Nadji(f, "PoSkladistu");
            NgControl txt = Kontrole.Nadji(f, "Ugovor");
            NgControl sea = Kontrole.Nadji(f, "partneriid");
            l.Add(cur != null && cur.Grana == NgGrana.Number && cur.TypeRaw == "CMEditVB6.CurMEdit" && Kontrole.A(cur, "DECIMALS") == "2"
                ? Qa.Z(Ime, "FAK-03", "CMEditVB6.CurMEdit je number jer sadrzi cur", "netgen/Form2.vb:2071")
                : Qa.P(Ime, "FAK-03", "cur", "netgen/Form2.vb:2071", cur == null ? "" : cur.TypeRaw));
            l.Add(uc != null && uc.Grana == NgGrana.Number && Kontrole.A(uc, "DECIMALS") == "0" && Kontrole.A(uc, "FORMATMASK") == null
                ? Qa.Z(Ime, "FAK-04", "nema FORMATMASK. Prazan broj je 0", "netgen/Form2.vb:2084-2086,3601")
                : Qa.P(Ime, "FAK-04", "decimals 0", "netgen/Form2.vb:2086", ""));
            l.Add(grup != null && !Kontrole.AllowTrue(Kontrole.A(grup, "ALLOWEMPTY")) && Kontrole.A(grup, "ALLOWEMPTY") == ""
                ? Qa.Z(Ime, "FAK-05", "ALLOWEMPTY prazan nije true. SelectedValue=1 pogodi id 1", "netgen/Form2.vb:2403,3459")
                : Qa.P(Ime, "FAK-05", "prazan ALLOWEMPTY", "netgen/Form2.vb:2403", Kontrole.A(grup, "ALLOWEMPTY") ?? "null"));
            l.Add(ziro != null && Kontrole.AllowTrue(Kontrole.A(ziro, "ALLOWEMPTY"))
                ? Qa.Z(Ime, "FAK-06", "parziroid ALLOWEMPTY TRUE ubaci null red", "netgen/Form2.vb:2403-2411")
                : Qa.P(Ime, "FAK-06", "ALLOWEMPTY true", "netgen/Form2.vb:2403", ""));
            l.Add(chk != null && chk.Grana == NgGrana.Check && chk.TypeRaw == "checkbox"
                ? Qa.Z(Ime, "FAK-07", "checkbox je check, parametar je CheckState", "netgen/Form2.vb:1460,3424-3427")
                : Qa.P(Ime, "FAK-07", "checkbox", "netgen/Form2.vb:1460", chk == null ? "" : chk.TypeRaw));
            l.Add(txt != null && txt.Grana == NgGrana.Text && txt.TypeRaw == "VB.textbox"
                ? Qa.Z(Ime, "FAK-08", "VB.textbox je text. Prazno je ''", "netgen/Form2.vb:1996,3655")
                : Qa.P(Ime, "FAK-08", "vb.textbox", "netgen/Form2.vb:1996", txt == null ? "" : txt.TypeRaw));
            string ssql = Kontrole.A(sea, "SQL") ?? "";
            l.Add(sea != null && sea.Grana == NgGrana.Search && Kontrole.OblikTrazilice(ssql) == "hash" && ssql.IndexOf("sifra%", StringComparison.Ordinal) >= 0 && !Kontrole.AllowTrue(Kontrole.A(sea, "ALLOWEMPTY"))
                ? Qa.Z(Ime, "FAK-09", "SEARCH hash. ALLOWEMPTY na searchu se ne cita", "netgen/Form2.vb:2140; netgen/Form3.vb:102")
                : Qa.P(Ime, "FAK-09", "search", "netgen/Form3.vb:102", ssql));
            l.Add(Stablo.Otvori(Ime, "FAK-10", "FakPar.xml", root));
            l.Add(Kontrole.Objekti(Ime, "FAK-11", "spFakPartneriUvjetiQuery"));
            Kontrole.K(l, Ime, xml, "FAK-20", "CurMEdit prazan ide kao 0, ne kao tekst", "netgen/Form2.vb:2071,3601", "number.Rabat", "0");
            Kontrole.K(l, Ime, xml, "FAK-21", "checkbox PoSkladistu salje CheckState", "netgen/Form2.vb:3424-3427", "check.PoSkladistu", "checkstate");
            return l;
        }
    }

    public static class MPNarIzdSkl
    {
        public const string Ime = "MPNarIzdSkl";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPNarIzdSkl.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "NAR-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "NAR-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "NAR-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            NgForm kop = p.NadjiFormu("KOPIRAJ");
            NgControl dt = Kontrole.Nadji(kop, "datum");
            NgControl dok = Kontrole.Nadji(kop, "datumdok");
            l.Add(dt != null && dt.TypeRaw == "datetiem" && dt.Grana == NgGrana.Date
                ? Qa.Z(Ime, "NAR-02", "datetiem sadrzi date i nastane datum", "netgen/Form2.vb:2232")
                : Qa.P(Ime, "NAR-02", "datetiem", "netgen/Form2.vb:2232", dt == null ? "" : dt.TypeRaw + "/" + dt.Grana));
            l.Add(dok != null && dok.TypeRaw == "datetime" && dok.Grana == NgGrana.Date
                ? Qa.Z(Ime, "NAR-03", "datetime je ista grana kao date", "netgen/Form2.vb:2232")
                : Qa.P(Ime, "NAR-03", "datetime", "netgen/Form2.vb:2232", ""));

            NgForm don = p.NadjiFormu("DONOSAUTOMATSKI");
            l.Add(Kontrole.Broj(don, NgGrana.Panel) >= 2
                ? Qa.Z(Ime, "NAR-04", "dva panela, i bez NAME", "netgen/Form2.vb:1384")
                : Qa.P(Ime, "NAR-04", "panel", "netgen/Form2.vb:1384", Kontrole.Broj(don, NgGrana.Panel).ToString()));
            bool dock = false;
            if (don != null)
            {
                for (int i = 0; i < don.Kontrole.Count; i++)
                {
                    if (don.Kontrole[i].Grana == NgGrana.Panel && Kontrole.A(don.Kontrole[i], "DOCK") == "1")
                        dock = true;
                }
            }
            l.Add(dock
                ? Qa.Z(Ime, "NAR-05", "DOCK=1 je DockStyle.Top", "netgen/Form2.vb:1407")
                : Qa.P(Ime, "NAR-05", "DOCK", "netgen/Form2.vb:1407", ""));
            NgControl grp = Kontrole.Nadji(don, "bottomgroupbox");
            NgControl pak = Kontrole.Nadji(don, "pakovanje");
            NgControl md = Kontrole.Nadji(don, "dodklasa");
            l.Add(grp != null && grp.Grana == NgGrana.Group && Kontrole.A(grp, "DOCK") == "2"
                ? Qa.Z(Ime, "NAR-06", "group je GroupBox, DOCK=2 je Bottom", "netgen/Form2.vb:1354,1376")
                : Qa.P(Ime, "NAR-06", "group", "netgen/Form2.vb:1354", ""));
            l.Add(pak != null && pak.Grana == NgGrana.Check && pak.Atributi.Ima("WUDTH") && !pak.Atributi.Ima("WIDTH")
                ? Qa.Z(Ime, "NAR-07", "WUDTH nije WIDTH. Sirina se ne mijenja", "netgen/Form2.vb:2949")
                : Qa.P(Ime, "NAR-07", "WUDTH", "netgen/Form2.vb:2949", ""));
            l.Add(md != null && md.Grana == NgGrana.MultiDrop && Kontrole.A(md, "ALLOWEMPTY") == "FALSE"
                ? Qa.Z(Ime, "NAR-08", "ALLOWEMPTY na multidropu se procita i ne ubaci red", "netgen/Form2.vb:2499-2504")
                : Qa.P(Ime, "NAR-08", "multidrop ALLOWEMPTY", "netgen/Form2.vb:2500", ""));
            l.Add(Stablo.Meni(Ime, "NAR-09", "MPNarIzdSkl.xml"));
            Kontrole.K(l, Ime, xml, "NAR-20", "datetiem se crta kao datum", "netgen/Form2.vb:2232", "date.datum", "date");
            Kontrole.K(l, Ime, xml, "NAR-21", "WUDTH se ignorira", "netgen/Form2.vb:2949", "width.pakovanje", "nema");
            return l;
        }
    }

    public static class DobUgovori
    {
        public const string Ime = "DobUgovori";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "DobUgovori.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "DOB-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "DOB-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "DOB-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            NgForm ins = p.NadjiFormu("INSERT_FORM");
            NgControl dat = Kontrole.Nadji(ins, "datugovorvrijedido");
            NgControl vrsta = Kontrole.Nadji(ins, "DobUgovoriVrsteId");
            l.Add(dat != null && string.Equals(Kontrole.A(dat, "SHOWCHECKBOX"), "TRUE", StringComparison.OrdinalIgnoreCase) && Kontrole.A(dat, "CHECKED") == null && Kontrole.A(dat, "DEFAULT") == null
                ? Qa.Z(Ime, "DOB-02", "SHOWCHECKBOX bez CHECKED ugasi kvacicu i salje null", "netgen/Form2.vb:2275-2278,2295,3560-3567")
                : Qa.P(Ime, "DOB-02", "SHOWCHECKBOX", "netgen/Form2.vb:2277", ""));
            l.Add(vrsta != null && Kontrole.AllowTrue(Kontrole.A(vrsta, "ALLOWEMPTY")) && string.Equals(Kontrole.A(vrsta, "TRANSLATE"), "true", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "DOB-03", "TRANSLATE prevodi stupac prikaza, ne vrijednost", "netgen/Form2.vb:2420-2424")
                : Qa.P(Ime, "DOB-03", "TRANSLATE", "netgen/Form2.vb:2420", ""));

            NgForm stavka = p.NadjiFormu("UNESI_STAVKU_FORM");
            NgControl roba = Kontrole.Nadji(stavka, "robaid");
            NgControl rad = Kontrole.Nadji(stavka, "rad2");
            NgControl grp = Kontrole.Nadji(stavka, "GroupNG0");
            if (grp == null)
            {
                for (int i = 0; i < p.Forme.Count && grp == null; i++)
                    grp = Kontrole.Nadji(p.Forme[i], "GroupNG0");
            }
            NgControl strip = Kontrole.Nadji(stavka, "s1");
            l.Add(Kontrole.OblikTrazilice(Kontrole.A(roba, "SQL") ?? "") == "sp"
                ? Qa.Z(Ime, "DOB-04", "spTraziRoba ostaje SQL, @@searchboxrobaid je tekst", "netgen/Form3.vb:99-100; netgen/Form2.vb:3626-3630")
                : Qa.P(Ime, "DOB-04", "sp search", "netgen/Form3.vb:99", ""));
            string klik = Kontrole.A(rad, "ONCLICK") ?? "";
            l.Add(rad != null && rad.Grana == NgGrana.Radio && klik.IndexOf("refresh", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "DOB-05", "radio ONCLICK refresh combo. Parametar radija je 1 ili 0", "netgen/Form2.vb:1509,3432,5158")
                : Qa.P(Ime, "DOB-05", "radio refresh", "netgen/Form2.vb:3432", ""));
            l.Add(grp != null && grp.Grana == NgGrana.Group && string.Equals(grp.TypeRaw, "group", StringComparison.OrdinalIgnoreCase)
                ? Qa.Z(Ime, "DOB-06", "group i frame su isti GroupBox", "netgen/Form2.vb:1354")
                : Qa.P(Ime, "DOB-06", "group", "netgen/Form2.vb:1354", grp == null ? "" : grp.TypeRaw));
            l.Add(strip != null && strip.Grana == NgGrana.StatusStrip && Kontrole.A(strip, "TEXT1") == "@@version" && Kontrole.A(strip, "TEXT2") == null
                ? Qa.Z(Ime, "DOB-07", "TEXT1=@@version, TEXT2 fali", "netgen/Form2.vb:1258-1260")
                : Qa.P(Ime, "DOB-07", "statusstrip", "netgen/Form2.vb:1258", Kontrole.A(strip, "TEXT1") ?? ""));
            l.Add(Stablo.Meni(Ime, "DOB-08", "DobUgovori.xml"));
            Kontrole.K(l, Ime, xml, "DOB-20", "SHOWCHECKBOX bez CHECKED salje null", "netgen/Form2.vb:2277-2278,3566", "date.datugovorvrijedido", "null");
            Kontrole.K(l, Ime, xml, "DOB-21", "TRANSLATE prevodi samo prikaz", "netgen/Form2.vb:2420-2424", "combo.translate", "stupac1");
            return l;
        }
    }

    public static class MpNgPregDokMail
    {
        public const string Ime = "MPNGPregDokMAIL";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPNGPregDokMAIL.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "MAIL-01", "XML postoji", xml, "nema"));
                return l;
            }
            string g;
            NgProject p = Qa.Ucitaj(xml, out g);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "MAIL-01", "Projekt se ucita", "netgen/Form1.vb:3263", g));
                return l;
            }
            l.Add(Qa.Z(Ime, "MAIL-01", "Projekt se ucita", "netgen/Form1.vb:3263"));
            string pop = p.Popup ?? "";
            l.Add(pop.IndexOf("ASSEMBLYLOAD(RepxToPdfToMail.StartProgram)", StringComparison.Ordinal) >= 0 && pop.IndexOf('~') < 0
                ? Qa.Z(Ime, "MAIL-02", "POPUPMASTER bez tilde ide u NovaGenerickaForma, ne u Rafalno", "netgen/Form1.vb:5743-5760,4584")
                : Qa.P(Ime, "MAIL-02", "ASSEMBLYLOAD", "netgen/Form1.vb:4584", pop));
            l.Add(string.IsNullOrEmpty(p.Tree) && p.Atributi.Ima("TREE33") && !p.Atributi.Ima("TREE")
                ? Qa.Z(Ime, "MAIL-03", "TREE33 nije TREE. Stablo se ne crta iz tog atributa", "netgen/Form1.vb:3387")
                : Qa.P(Ime, "MAIL-03", "TREE33", "netgen/Form1.vb:3387", p.Tree ?? ""));
            l.Add(p.OnLoad != null && p.OnLoad.TrimStart().StartsWith(":", StringComparison.Ordinal)
                ? Qa.Z(Ime, "MAIL-04", "ONLOAD=:FILTERFORM je forma, ne SQL", "netgen/Form1.vb:3879-3880")
                : Qa.P(Ime, "MAIL-04", "ONLOAD dvotocka", "netgen/Form1.vb:3879", p.OnLoad ?? ""));
            NgForm fil = p.NadjiFormu("FILTERFORM");
            string clip = fil == null ? null : (fil.Atributi.Ima("DBCLIPBOARD") ? fil.Atributi.Vrijednost("DBCLIPBOARD") : null);
            bool imaCarpo = false;
            if (fil != null)
            {
                for (int i = 0; i < fil.Kontrole.Count; i++)
                {
                    if (string.Equals(fil.Kontrole[i].Name, "CarpoForm", StringComparison.OrdinalIgnoreCase))
                        imaCarpo = true;
                }
            }
            l.Add(clip != null && clip.Trim().ToLowerInvariant() == "false" && !imaCarpo
                ? Qa.Z(Ime, "MAIL-05", "DBCLIPBOARD=false gasi clipboard iako ime sadrzi filter. Nema CarpoForm", "netgen/Form2.vb:1102-1104; docs/funkcionalnosti/poziv-carpo.md")
                : Qa.P(Ime, "MAIL-05", "nije Carpo subforma", "netgen/Form2.vb:1102", clip ?? ""));
            NgControl vrst = Kontrole.Nadji(fil, "vrstapromid");
            NgControl sea = Kontrole.Nadji(fil, "partneriid");
            l.Add(vrst != null && (Kontrole.A(vrst, "ONLOSTFOCUS") ?? "").IndexOf("vrstadokid", StringComparison.OrdinalIgnoreCase) >= 0
                && Kontrole.OblikTrazilice(Kontrole.A(sea, "SQL") ?? "") == "hash"
                ? Qa.Z(Ime, "MAIL-06", "filter: zavisni combo i search vipartneri#", "netgen/Form2.vb:7510,5158; netgen/Form3.vb:102")
                : Qa.P(Ime, "MAIL-06", "filter kontrole", "netgen/Form2.vb:5158", ""));
            l.Add(Stablo.Meni(Ime, "MAIL-07", "MPNGPregDokMAIL.xml"));
            Kontrole.K(l, Ime, xml, "MAIL-20", "totalgen(id, operater, konekcija, zadnji SQL mastera, parametri grida). Povrat se ne cita", "netgen/Form1.vb:4599-4623", "assemblyload.args", "shell-pet");
            Kontrole.K(l, Ime, xml, "MAIL-21", "Load ili CreateInstance padne: CB log i Exit Sub", "netgen/Form1.vb:4600-4611", "assemblyload.greska", "prekini");
            return l;
        }
    }
}

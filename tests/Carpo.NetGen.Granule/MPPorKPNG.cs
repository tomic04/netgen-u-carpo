using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// MPPorKPNG: filter prije grida, datum, combo, broj.
    /// Ocekivanje je iz Form1/Form2, ne iz Carpo.NetGen.
    /// spMPPoravnajKP se ne salje. jupiter2022 se ne otvara.
    /// </summary>
    public static class MPPorKPNG
    {
        public const string Ime = "MPPorKPNG";
        public const string Naslov = ">Poravnavanje KP u maloprodaji ";
        public const string Popup = "Filter:FILTERFORM#Izmjeni:IZMJENIIZNOS#Zaključi:ZAKLJUCI#Otključaj:OTKLJUCAJ";

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "BIOVEGA", "MPPorKPNG.xml");
            string kopija = Path.Combine(root, "NetGenBot", "granule", "Biovega", "MPPorKPNG.xml");
            if (!File.Exists(xml))
            {
                l.Add(Qa.P(Ime, "MPK-01", "XML postoji", xml, "nema datoteke"));
                return l;
            }
            if (File.Exists(kopija))
            {
                l.Add(IstiHash(xml, kopija)
                    ? Qa.Z(Ime, "MPK-00", "NetGenBot kopija je isti sadrzaj", "definitions/netgen/BIOVEGA/MPPorKPNG.xml")
                    : Qa.P(Ime, "MPK-00", "NetGenBot kopija je isti sadrzaj", kopija, "SHA-256 se razlikuje"));
            }

            string greska;
            NgProject p = Qa.Ucitaj(xml, out greska);
            if (p == null)
            {
                l.Add(Qa.P(Ime, "MPK-01", "Projekt se ucita", "netgen/Form1.vb:3263", greska));
                return l;
            }

            l.Add(Qa.Jednako(Ime, "MPK-01", "Naslov ima vodeci > i razmak na kraju. NAME nije naslov", "netgen/Form1.vb:3522-3528", Naslov, p.Naslov));
            l.Add(Qa.Jednako(Ime, "MPK-02", "ONLOAD je :FILTERFORM, master se ne puni na ulazu", "netgen/Form1.vb:3879-3880", ":FILTERFORM", p.OnLoad));
            l.Add(string.IsNullOrEmpty(p.Tree) && string.IsNullOrEmpty(p.DetailGrid)
                ? Qa.Z(Ime, "MPK-03", "Nema TREE ni DETAILGRID. Prazno stablo se skloni", "netgen/Form1.vb:4310-4316,2111")
                : Qa.P(Ime, "MPK-03", "Nema TREE ni DETAILGRID", "netgen/Form1.vb:3373", (p.Tree ?? "") + "/" + (p.DetailGrid ?? "")));
            l.Add(Qa.Jednako(Ime, "MPK-04", "Dvostruki klik otvara IZMJENIIZNOS", "netgen/Form1.vb:3388-3389", "IZMJENIIZNOS", p.MasterDblClick));
            l.Add(Qa.Jednako(Ime, "MPK-05", "Popup nema Listu ni ispis", "netgen/Form1.vb:5743", Popup, p.Popup));
            l.Add(Gumbi(p));
            l.Add(Preskoceni(p));
            l.Add(!p.Atributi.Ima("CHECKBOXVISIBLE") && !p.Atributi.Ima("TOTALROWVISIBLE") && !p.Atributi.Ima("GROUPBYBOXVISIBLE")
                ? Qa.Z(Ime, "MPK-08", "Checkbox, total i group box nisu u XML-u. Checkbox stupac i total ostaju ukljuceni, group box ostaje Janus True", "netgen/Form1.vb:2043-2053,3094,3350")
                : Qa.P(Ime, "MPK-08", "atributi grida", "netgen/Form1.vb:2043", "XML ih ima"));
            l.Add(string.IsNullOrEmpty(p.Permisio)
                ? Qa.Z(Ime, "MPK-09", "PERMISIO nije u XML-u, default ostaje true", "netgen/Form1.vb:3153,3414")
                : Qa.P(Ime, "MPK-09", "PERMISIO", "netgen/Form1.vb:3153", p.Permisio));

            l.AddRange(Forme(p));
            l.AddRange(Runtime(xml));
            return l;
        }

        private static Stavka Gumbi(NgProject p)
        {
            if (p.Gumbi.Count != 5)
                return Qa.P(Ime, "MPK-06", "Pet gumba podloge, CMD6 prazan", "netgen/Form1.vb:3314-3330", "gumbi=" + p.Gumbi.Count);
            bool ok = p.Gumbi[0].Redni == 1 && p.Gumbi[0].Natpis == "Parametri" && p.Gumbi[0].Naredba == "FILTERFORM"
                && p.Gumbi[1].Redni == 2 && p.Gumbi[1].Natpis == "Dodaj" && p.Gumbi[1].Naredba == "DODAJIZNOS"
                && p.Gumbi[2].Redni == 3 && p.Gumbi[2].Natpis == "Izmjeni" && p.Gumbi[2].Naredba == "IZMJENIIZNOS"
                && p.Gumbi[3].Redni == 4 && p.Gumbi[3].Natpis == "Zaključi" && p.Gumbi[3].Naredba == "ZAKLJUCI"
                && p.Gumbi[4].Redni == 5 && p.Gumbi[4].Natpis == "Otključaj" && p.Gumbi[4].Naredba == "OTKLJUCAJ";
            return ok
                ? Qa.Z(Ime, "MPK-06", "Gumbi 1-5 lome se na prvoj dvotocki", "netgen/Form1.vb:3314-3330")
                : Qa.P(Ime, "MPK-06", "Gumbi 1-5", "netgen/Form1.vb:3314", p.Gumbi[0].Natpis + "/" + p.Gumbi[4].Naredba);
        }

        private static Stavka Preskoceni(NgProject p)
        {
            bool ima7 = false;
            bool ima8 = false;
            for (int i = 0; i < p.PreskoceniGumbi.Count; i++)
            {
                if (p.PreskoceniGumbi[i] == "CMD7CAPTION")
                    ima7 = true;
                if (p.PreskoceniGumbi[i] == "CMD8CAPTION")
                    ima8 = true;
            }
            for (int i = 0; i < p.Gumbi.Count; i++)
            {
                if (p.Gumbi[i].Redni == 7 || p.Gumbi[i].Redni == 8)
                    return Qa.P(Ime, "MPK-07", "Lista i Print dok. nisu gumbi", "netgen/Form1.vb:3320-3333", "redni " + p.Gumbi[i].Redni);
            }
            return ima7 && ima8
                ? Qa.Z(Ime, "MPK-07", "CMD7 i CMD8 nemaju dvotocku: gumb ostaje prazan i skriven", "netgen/Form1.vb:3320-3333,3834-3835")
                : Qa.P(Ime, "MPK-07", "CMD7 i CMD8 preskoceni", "netgen/Form1.vb:3320", string.Join(",", p.PreskoceniGumbi.ToArray()));
        }

        private static List<Stavka> Forme(NgProject p)
        {
            var l = new List<Stavka>();
            NgForm filter = p.NadjiFormu("FILTERFORM");
            NgForm dodaj = p.NadjiFormu("DODAJIZNOS");
            NgForm izm = p.NadjiFormu("IZMJENIIZNOS");
            NgForm zak = p.NadjiFormu("ZAKLJUCI");
            NgForm otk = p.NadjiFormu("OTKLJUCAJ");
            if (filter == null || dodaj == null || izm == null || zak == null || otk == null)
            {
                l.Add(Qa.P(Ime, "MPK-10", "Pet formi", "XML", "nedostaje forma"));
                return l;
            }

            l.Add(Qa.Jednako(Ime, "MPK-10", "FILTERFORM FILLEXPLORER je TRUE", "netgen/Form1.vb:5445-5452", "TRUE", Atr(filter, "FILLEXPLORER")));
            l.Add(filter.OnSubmit.StartsWith("spMPPoravnajKP @action = 1,", StringComparison.Ordinal)
                ? Qa.Z(Ime, "MPK-11", "Filter ONSUBMIT pocinje sa sp, parametri se lijepe", "netgen/Form2.vb:3378-3388")
                : Qa.P(Ime, "MPK-11", "filter ONSUBMIT", "netgen/Form2.vb:1039", filter.OnSubmit));
            l.Add(filter.Gumbi.Count == 2 && filter.Gumbi[0].Natpis == "Pokaži" && filter.Gumbi[0].Naredba == ""
                && filter.Gumbi[1].Natpis == "Odustani"
                ? Qa.Z(Ime, "MPK-12", "CMD1 i CMD2 na formi su cijeli natpis", "netgen/Form2.vb:1061-1066")
                : Qa.P(Ime, "MPK-12", "gumbi filtera", "netgen/Form2.vb:1061", filter.Gumbi.Count.ToString()));

            l.Add(Atr(dodaj, "FILLEXPLORER") == ""
                ? Qa.Z(Ime, "MPK-13", "DODAJIZNOS ima prazan FILLEXPLORER: ExecSql pa zadnji master, ne novi upit", "netgen/Form1.vb:5509-5519")
                : Qa.P(Ime, "MPK-13", "FILLEXPLORER dodaj", "netgen/Form1.vb:5509", Atr(dodaj, "FILLEXPLORER")));
            l.Add(dodaj.OnSubmit.IndexOf("@action=2", StringComparison.Ordinal) >= 0
                && izm.OnSubmit.IndexOf("@action=4", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "MPK-14", "Unos je action 2, izmjena action 4, oba lijepe parametre", "netgen/Form2.vb:3378-3388")
                : Qa.P(Ime, "MPK-14", "ONSUBMIT unos/izmjena", "netgen/Form2.vb:1039", dodaj.OnSubmit));

            l.Add(zak.OnSubmit.StartsWith("spMPPoravnajKP 5,", StringComparison.Ordinal)
                && otk.OnSubmit.StartsWith("spMPPoravnajKP 6,", StringComparison.Ordinal)
                ? Qa.Z(Ime, "MPK-15", "Zakljucak i otkljucaj su pozicijski 5 i 6, ne @action", "XML")
                : Qa.P(Ime, "MPK-15", "ONSUBMIT zakljucak", "XML", zak.OnSubmit));
            l.Add(zak.OnLoad.IndexOf("spRaiserror", StringComparison.Ordinal) >= 0
                && izm.OnLoad.IndexOf("spRaiserror", StringComparison.Ordinal) >= 0
                && otk.OnLoad.IndexOf("spRaiserror", StringComparison.Ordinal) >= 0
                ? Qa.Z(Ime, "MPK-16", "ONLOAD s spRaiserror ne otvara formu. Test proceduru ne salje", "netgen/Form2.vb:1199,4429")
                : Qa.P(Ime, "MPK-16", "ONLOAD raiserror", "netgen/Form2.vb:1199", zak.OnLoad));

            NgControl dat = Nadji(dodaj, "datumdok");
            NgControl iznos = Nadji(dodaj, "mpiznos");
            NgControl osoba = Nadji(dodaj, "osobeid");
            NgControl trg = Nadji(dodaj, "TrgovinaId");
            NgControl nap = Nadji(dodaj, "napomena");
            l.Add(dat != null && dat.Grana == NgGrana.Date && AtrC(dat, "VALUE") == "DATE1" && !dat.Atributi.Ima("DEFAULT") && AtrC(dat, "CHECKBOX") == "FALSE" && !dat.Atributi.Ima("SHOWCHECKBOX")
                ? Qa.Z(Ime, "MPK-20", "VALUE=DATE1 se ne cita. DEFAULT nema, ostaje danas. CHECKBOX nije SHOWCHECKBOX", "netgen/Form2.vb:2251-2278")
                : Qa.P(Ime, "MPK-20", "datumdok", "netgen/Form2.vb:2251", dat == null ? "nema" : AtrC(dat, "VALUE")));
            string tab = dat == null ? "" : (dat.Atributi.Vrijednost("TABINDEX") ?? "");
            l.Add(tab.Trim() == "1" && tab != "1"
                ? Qa.Z(Ime, "MPK-21", "TABINDEX datumdok je 1 s razmakom (tab iz fajla). VB ga cita kao 1, ne kao -2", "netgen/Form2.vb:3027-3030")
                : Qa.P(Ime, "MPK-21", "TABINDEX razmak", "netgen/Form2.vb:3028", tab.Replace("\t", "\\t").Replace(" ", "\\s")));
            l.Add(nap != null && AtrC(nap, "TABINDEX") == "1"
                ? Qa.Z(Ime, "MPK-22", "napomena ima cisti TABINDEX 1", "netgen/Form2.vb:3028-3030")
                : Qa.P(Ime, "MPK-22", "TABINDEX napomena", "netgen/Form2.vb:3028", nap == null ? "" : AtrC(nap, "TABINDEX")));
            l.Add(iznos != null && iznos.Grana == NgGrana.Number && AtrC(iznos, "DECIMALS") == "4" && !iznos.Atributi.Ima("VALUE")
                ? Qa.Z(Ime, "MPK-23", "Broj krene od 0, DECIMALS=4 je DecimalDigits", "netgen/Form2.vb:2084-2088")
                : Qa.P(Ime, "MPK-23", "mpiznos", "netgen/Form2.vb:2086", iznos == null ? "" : iznos.Grana.ToString()));
            l.Add(osoba != null && osoba.Grana == NgGrana.Combo && AtrC(osoba, "ALLOWEMPTY").Trim().ToLowerInvariant() == "true"
                && trg != null && !trg.Atributi.Ima("ALLOWEMPTY")
                ? Qa.Z(Ime, "MPK-24", "osobeid dozvoljava prazno. TrgovinaId ne, pa pokusa SelectedValue=1", "netgen/Form2.vb:2402-2411,3455-3468")
                : Qa.P(Ime, "MPK-24", "ALLOWEMPTY", "netgen/Form2.vb:2402", ""));

            NgControl od = Nadji(filter, "datumod");
            NgControl ddo = Nadji(filter, "datumdo");
            NgControl st = Nadji(filter, "status");
            l.Add(od != null && AtrC(od, "VALUE") == "DATE2" && ddo != null && AtrC(ddo, "VALUE") == "DATE"
                ? Qa.Z(Ime, "MPK-25", "Filter VALUE DATE2 i DATE se ne citaju. Oba ostanu danas", "netgen/Form2.vb:2251-2267")
                : Qa.P(Ime, "MPK-25", "filter datumi", "netgen/Form2.vb:2251", ""));
            l.Add(st != null && st.Grana == NgGrana.Combo && AtrC(st, "SQL").IndexOf("union all", StringComparison.OrdinalIgnoreCase) >= 0
                ? Qa.Z(Ime, "MPK-26", "Status je combo iz union all, stupac 0 vrijednost, stupac 1 natpis", "netgen/Form2.vb:2427-2431")
                : Qa.P(Ime, "MPK-26", "status SQL", "netgen/Form2.vb:2388", ""));

            NgControl trIzm = Nadji(izm, "TrgovinaId");
            l.Add(trIzm != null && string.Equals(AtrC(trIzm, "ENABLED"), "False", StringComparison.Ordinal)
                ? Qa.Z(Ime, "MPK-27", "Izmjena gasi trgovinu. TabStop je false", "netgen/Form2.vb:2951,3046")
                : Qa.P(Ime, "MPK-27", "ENABLED trgovina", "netgen/Form2.vb:2951", trIzm == null ? "" : AtrC(trIzm, "ENABLED")));

            l.Add(Qa.Jednako(Ime, "MPK-28", "Sirina unosa je 7500/14", "netgen/Form2.vb:1093", Qa.Px("7500").ToString(), Qa.Px(Atr(dodaj, "WIDTH")).ToString()));
            l.Add(Qa.Jednako(Ime, "MPK-29", "Sirina datuma je 1500/14", "netgen/Form2.vb:2949", Qa.Px("1500").ToString(), Qa.Px(AtrC(dat, "WIDTH")).ToString()));
            return l;
        }

        private static List<Stavka> Runtime(string xml)
        {
            var l = new List<Stavka>();
            K(l, xml, "MPK-30", "FILTERFORM true zamijeni master SQL-om ONSUBMIT i parametara", "netgen/Form1.vb:5445-5452", "gumb", "Parametri", "fillExplorer", "true");
            K(l, xml, "MPK-31", "Prazan FILLEXPLORER izvrsi ONSUBMIT pa ponovi zadnji master", "netgen/Form1.vb:5509-5519", "gumb", "Dodaj", "fillExplorer", "prazan");
            K(l, xml, "MPK-40", "datumdok i napomena dijele indeks 1. Red je datum pa napomena", "netgen/Form2.vb:3027-3030", "gumb", "Dodaj", "tab", "datumdok,napomena");
            K(l, xml, "MPK-41", "Datum nije DATE1. U parametru je danas kao yyyyMMdd, broj je 0", "netgen/Form2.vb:2251-2267,3530-3532,2084", "gumb", "Dodaj", "datumParam", "yyyyMMdd");
            l.Add(Qa.Mora(Ime, "MPK-42", "Checkbox stupac je vidljiv, total je ukljucen, sort klikom ostaje", "netgen/Form1.vb:2043-2053,3401-3404", xml, "otvori", "", "selectorVidljiv", "true"));
            K(l, xml, "MPK-43", "SQL filtera ide u NgDnevnik, ne u CBLog", "docs/funkcionalnosti/logging.md", "gumb", "Parametri", "dnevnik", "NetGen SQL");
            l.Add(Izgled("MPK-50", "Row header, prazni retci, HeaderLines, font i obrub su Janus krom. Kontrola je DevExpress.", "netgen/Form1.designer.vb:640-655; netgen/Class1.vb:1000,1364"));
            l.Add(Izgled("MPK-51", "Gumbi su u ribbonu. Sirina 79 i razmak 8 se ne traze.", "netgen/Form1.vb:3312,3845-3846"));
            return l;
        }

        private static void K(List<Stavka> l, string xml, string id, string opis, string izvor, string akcija, string cilj, string kljuc, string ocekivano)
        {
            l.Add(Qa.Kljuc(Ime, id, opis, izvor, xml, akcija, cilj, kljuc, ocekivano));
        }

        private static Stavka Izgled(string id, string opis, string izvor)
        {
            return Qa.Z(Ime, id, "izgled – Carpo. " + opis, izvor);
        }

        private static NgControl Nadji(NgForm f, string ime)
        {
            if (f == null)
                return null;
            for (int i = 0; i < f.Kontrole.Count; i++)
            {
                if (f.Kontrole[i].Name == ime)
                    return f.Kontrole[i];
            }
            return null;
        }

        private static string Atr(NgForm f, string ime)
        {
            return f.Atributi.Vrijednost(ime) ?? "";
        }

        private static string AtrC(NgControl c, string ime)
        {
            if (c == null)
                return "";
            return c.Atributi.Vrijednost(ime) ?? "";
        }

        private static bool IstiHash(string a, string b)
        {
            using (var sha = SHA256.Create())
            {
                byte[] ha = sha.ComputeHash(File.ReadAllBytes(a));
                byte[] hb = sha.ComputeHash(File.ReadAllBytes(b));
                if (ha.Length != hb.Length)
                    return false;
                for (int i = 0; i < ha.Length; i++)
                {
                    if (ha[i] != hb[i])
                        return false;
                }
                return true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;

// Nad svim tekstovima korpusa (atributi s naredbama/SQL-om) zove čiste C# parsere Carpo.NetGen i bilježi
// iznimke i pozive dulje od 2 s. Pokretanje: mono Fuzz.exe <definitions/netgen>
class KorpusParseri
{
    static readonly Dictionary<string, int> greske = new Dictionary<string, int>();
    static readonly Dictionary<string, string> primjer = new Dictionary<string, string>();
    static int pozivi;

    static string Read(string p)
    {
        byte[] b = File.ReadAllBytes(p);
        if (b.Length >= 2 && ((b[0] == 0xFF && b[1] == 0xFE) || (b[0] == 0xFE && b[1] == 0xFF)))
            return Encoding.Unicode.GetString(b, 2, b.Length - 2);
        if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF)
            return Encoding.UTF8.GetString(b, 3, b.Length - 3);
        try { return new UTF8Encoding(false, true).GetString(b); }
        catch { return Encoding.GetEncoding(1250).GetString(b); }
    }

    static void Probaj(string ime, string ulaz, Action a)
    {
        pozivi++;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        Exception greska = null;
        var t = new Thread(() => { try { a(); } catch (Exception ex) { greska = ex; } });
        t.IsBackground = true;
        t.Start();
        if (!t.Join(3000))
        {
            Zabiljezi(ime + " PETLJA", ulaz);
            try { t.Abort(); } catch { }
            return;
        }
        if (greska != null)
            Zabiljezi(ime + " " + greska.GetType().Name + ": " + Kratko(greska.Message), ulaz);
        else if (sw.ElapsedMilliseconds > 500)
            Zabiljezi(ime + " SPORO " + sw.ElapsedMilliseconds + "ms", ulaz);
    }

    static string Kratko(string s) { s = (s ?? "").Replace("\r", " ").Replace("\n", " "); return s.Length > 90 ? s.Substring(0, 90) : s; }

    static void Zabiljezi(string kljuc, string ulaz)
    {
        int n; greske.TryGetValue(kljuc, out n); greske[kljuc] = n + 1;
        if (!primjer.ContainsKey(kljuc)) primjer[kljuc] = Kratko(ulaz);
    }

    static void Parseri(string tekst, string atribut)
    {
        var k = new SqlKontekst();
        k.Id = "5"; k.OperaterId = "7"; k.Node = "3"; k.ParametriSaGrida = "@id='5' , @naziv='A,B' , @datum='14.11.2026. 0:00:00' , ";
        var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        cb["imereporta1="] = "repx/rptX"; cb["datumdo="] = "14.11.2026"; cb["kolicina="] = "1,5";
        if (Environment.GetEnvironmentVariable("SAMO") == "rafal")
        {
            Probaj("RafalNaredba.Refresh/Dock", tekst, () => { RafalNaredba.RefreshKontrola(tekst); RafalNaredba.Dock(tekst); RafalNaredba.DockKontrola(tekst); });
            Probaj("RafalNaredba.Eval", tekst, () => { RafalNaredba.EvalLijevo(tekst); RafalNaredba.EvalDesno(tekst); });
            Probaj("RafalNaredba.Upozorenje", tekst, () => { RafalNaredba.UpozorenjeParam(tekst); RafalNaredba.UpozorenjeTekst(tekst); RafalNaredba.OtkazImaPoruku(tekst); RafalNaredba.OtkazParam(tekst); RafalNaredba.OtkazTekst(tekst); });
            Probaj("RafalNaredba.Datoteka", tekst, () => { RafalNaredba.Datoteka(tekst); RafalNaredba.DatotekaPut(tekst); RafalNaredba.DatotekaSql(tekst); });
            Probaj("RafalNaredba.Ostalo", tekst, () => { RafalNaredba.Slopp(tekst); RafalNaredba.StartSearch(tekst); RafalNaredba.CreateObject(tekst); RafalNaredba.AlertNastavi("1", tekst); RafalNaredba.CancelBoxStani("1", tekst); RafalNaredba.MasterSljedeci(tekst); });
            return;
        }
        Probaj("ZastitaSql.Tekst", tekst, () => ZastitaSql.Tekst(tekst));
        Probaj("ZastitaSql.Tijelo", tekst, () => ZastitaSql.Tijelo(tekst));
        Probaj("SqlTekst.Sredi", tekst, () => SqlTekst.Sredi(tekst, k));
        Probaj("SqlTekst.Sredi2", tekst, () => SqlTekst.Sredi2(tekst, k));
        Probaj("SqlTekst.Odluka", tekst, () => SqlTekst.Odluka(tekst));
        Probaj("SqlTekst.ZaGumb", tekst, () => SqlTekst.ZaGumb(tekst, k));
        Probaj("SrediPoslije.Zamijeni", tekst, () => SrediPoslije.Zamijeni(tekst, k.ParametriSaGrida, k, cb, false));
        Probaj("SrediPoslije.Zamijeni-ispis", tekst, () => SrediPoslije.Zamijeni(tekst, k.ParametriSaGrida, k, cb, true));
        Probaj("SrediPoslije.Petlja-prazno", tekst, () => SrediPoslije.Petlja(tekst, "", null, false));
        Probaj("SrediPoslije.Kolone", tekst, () => SrediPoslije.Kolone(tekst, (g, c) => "null"));
        Probaj("DijalogTok.ZamijeniTokene", tekst, () => DijalogTok.ZamijeniTokene(tekst, "5", cb));
        Probaj("DijalogTok.Koraci", tekst, () => DijalogTok.Koraci(tekst));
        Probaj("DijalogTok.Gumb", tekst, () => DijalogTok.Gumb(null, 1, tekst, false));
        Probaj("Lanac.Segmenti", tekst, () => Lanac.Segmenti(tekst));
        Probaj("Lanac.Lijepi", tekst, () => Lanac.Lijepi(tekst, "@a=1,"));
        Probaj("Lanac.OsvjezavaZadnjiSql", tekst, () => Lanac.OsvjezavaZadnjiSql(tekst));
        Probaj("Lanac.IspisSegment", tekst, () => Lanac.IspisSegment(tekst));
        Probaj("Lanac.OnLoadLanac", tekst, () => Lanac.OnLoadLanac(tekst));
        Probaj("Lanac.GumbIzCase", tekst, () => Lanac.GumbIzCase(tekst, "x"));
        Probaj("Lanac.Izvrsi", tekst, () => Lanac.Izvrsi(tekst, true, null));
        Probaj("Lanac.Izvrsi-podloga", tekst, () => Lanac.Izvrsi(tekst, false, "greska #OK"));
        Probaj("GumbTok.Akcija", tekst, () => GumbTok.Akcija(null, tekst));
        Probaj("Form1Rafal.Vrsta", tekst, () => Form1Rafal.Vrsta(tekst));
        Probaj("Form1Rafal.Pripremi", tekst, () => Form1Rafal.Pripremi(tekst));
        Probaj("Form1Rafal.OpenFormIme", tekst, () => Form1Rafal.OpenFormIme(tekst));
        Probaj("Form1Rafal.IsEmptyPreskace", tekst, () => Form1Rafal.IsEmptyPreskace(tekst));
        Probaj("Form1Rafal.BojaSql", tekst, () => Form1Rafal.BojaSql(tekst));
        Probaj("Form1Rafal.EditStupciSuUpit", tekst, () => Form1Rafal.EditStupciSuUpit(tekst));
        Probaj("StupciNaredba.Primijeni", tekst, () => StupciNaredba.Primijeni(tekst));
        Probaj("Izraz.Izracunaj", tekst, () => Izraz.Izracunaj(tekst));
        Probaj("PorukaNaredba.Tekst", tekst, () => PorukaNaredba.Tekst(tekst));
        Probaj("ForeachNaredba", tekst, () => { ForeachNaredba.Vrsta(tekst); ForeachNaredba.Grid(tekst); ForeachNaredba.Ponovi(tekst, new[] { "1", "2" }); });
        Probaj("KlikNaredba.Kontrola", tekst, () => KlikNaredba.Kontrola(tekst));
        Probaj("OtvoriNaredba.Ime", tekst, () => OtvoriNaredba.Ime(tekst));
        Probaj("FiskalNaredba", tekst, () => { FiskalNaredba.Tablica(tekst); FiskalNaredba.Argument(tekst); });
        Probaj("ProstorNaredba", tekst, () => { ProstorNaredba.Je(tekst); ProstorNaredba.Argument(tekst); });
        Probaj("PraznoNaredba", tekst, () => { PraznoNaredba.Je(tekst); PraznoNaredba.PreskociSljedeci(tekst); });
        Probaj("StupacVrijednost.Je", tekst, () => StupacVrijednost.Je(tekst));
        Probaj("BlagajnaNaredba", tekst, () => BlagajnaNaredba.Upit(BlagajnaNaredba.Id(tekst)));
        Probaj("RafalNaredba.sve", tekst, () =>
        {
            RafalNaredba.RefreshKontrola(tekst); RafalNaredba.Dock(tekst); RafalNaredba.DockKontrola(tekst);
            RafalNaredba.MasterSljedeci(tekst); RafalNaredba.MasterPrethodni(tekst); RafalNaredba.EvalLijevo(tekst); RafalNaredba.EvalDesno(tekst);
            RafalNaredba.UpozorenjeParam(tekst); RafalNaredba.UpozorenjeTekst(tekst); RafalNaredba.OtkazImaPoruku(tekst); RafalNaredba.OtkazParam(tekst); RafalNaredba.OtkazTekst(tekst);
            RafalNaredba.Slopp(tekst); RafalNaredba.Datoteka(tekst); RafalNaredba.DatotekaPut(tekst); RafalNaredba.DatotekaSql(tekst);
            RafalNaredba.StartSearch(tekst); RafalNaredba.CreateObject(tekst); RafalNaredba.AlertNastavi("1", tekst); RafalNaredba.CancelBoxStani("1", tekst);
        });
        Probaj("PosaljiServer.Vrsta", tekst, () => PosaljiServer.Vrsta(tekst));
        Probaj("Pokretanje.Je", tekst, () => Pokretanje.Je(tekst));
        Probaj("BojaNaredba.Sql", tekst, () => BojaNaredba.Sql(tekst));
        Probaj("IspisZaglavlje", tekst, () => { Carpo.NetGen.Print.IspisZaglavlje.Desno(tekst, cb); Carpo.NetGen.Print.IspisZaglavlje.Podnozje(tekst, cb); });
        Probaj("IspisMotora.JeIspisImena", tekst, () => Carpo.NetGen.Print.IspisMotora.JeIspisImena(tekst));
        Probaj("Lanac.SearchParametar", tekst, () => { Lanac.SearchParametar(tekst); Lanac.SearchTablica(tekst); });
        Probaj("ClipboardNg.IzSetDefaults", tekst, () => Carpo.NetGen.Kontrole.ClipboardNg.IzSetDefaults(tekst, k, null));
    }

    static string Dekod(string v)
    {
        return v.Replace("&apos;", "'").Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&#13;", "\r").Replace("&#10;", "\n").Replace("&amp;", "&");
    }

    static void Main(string[] args)
    {
        var atributi = new Regex("\\b(ONCLICK|ONSUBMIT|ONLOAD|ONLOAD2|ONLOADED|ONLOSTFOCUS|ONGOTFOCUS|ONDBLCLICK|ONCELLEDITED|ONCELLEDITEDDET|BEFORECELLEDITED|ONCLOSED|ONESCKEY|ONENTERKEY|AFTERREFRESH|SQL|SETDEFAULTS|DETAILGRID|TREE|CMD\\d+CAPTION|CMD\\d+|MASTERDBLCLICK|DETAILDBLCLICK|POPUP\\w*|EDITCOLUMNS|PAGEHEADER\\w*|PAGEFOOTER\\w*|CAPTION|TEXT\\d|STORA|STARTUPSTORA|NEXTFORM|HOTKEY)\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);
        int tekstova = 0;
        var vidjeni = new HashSet<string>();
        foreach (string p in Directory.GetFiles(args[0], "*.xml", SearchOption.AllDirectories))
        {
            string t = Read(p);
            foreach (Match m in atributi.Matches(t))
            {
                string v = Dekod(m.Groups[2].Value);
                if (v.Trim().Length == 0 || !vidjeni.Add(m.Groups[1].Value.ToUpperInvariant() + "\u0001" + v))
                    continue;
                tekstova++;
                Parseri(v, m.Groups[1].Value);
                foreach (string seg in v.Split('#'))
                    if (seg.Trim().Length > 0 && seg != v && vidjeni.Add("SEG\u0001" + seg))
                        Parseri(seg, "segment");
            }
        }
        Console.WriteLine("tekstova=" + tekstova + " poziva=" + pozivi + " vrsta gresaka=" + greske.Count);
        foreach (var g in greske)
            Console.WriteLine(g.Value + "\t" + g.Key + "\t" + primjer[g.Key]);
    }
}

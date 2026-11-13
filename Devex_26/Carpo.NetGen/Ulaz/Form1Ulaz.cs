using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Print;

namespace Carpo.NetGen.Ulaz
{
    /// <summary>
    /// Pravila iz Form1.vb koja port još nije imao: alatna traka, tipke, dozvole,
    /// ulaz, spoj, jezik, prostor, izbornik. Fiskalni URL ostaje testni (R109).
    /// </summary>
    public static class Form1Ulaz
    {
        public const int PorukaZatvaranje = 0x7D0;
        public const int PorukaAktivno = 0x7D1;
        public const int PorukaNeaktivno = 0x7D2;
        public const int WmDisplayChange = 0x7E;
        public const int WmDpiChanged = 0x2E0;
        public const string ProdukcijskiCis = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService";

        public static bool TrebaPoravnati(int poruka)
        {
            return poruka == WmDisplayChange || poruka == WmDpiChanged;
        }

        /// <summary>Form1.vb 211–215. Samo maksimiziran prozor.</summary>
        public static bool ResizeAkoJeMaksimiziran(bool maksimiziran)
        {
            return maksimiziran;
        }

        /// <summary>Form1.vb 233–248. Samo vidljivi gumbi 1–5.</summary>
        public static string AlatnaTraka(bool[] vidljiv, string[] natpis, bool[] ukljucen)
        {
            string t = "<buttons>";
            for (int i = 0; i < 5; i++)
            {
                bool v = vidljiv != null && i < vidljiv.Length && vidljiv[i];
                if (!v)
                    continue;
                string cap = natpis != null && i < natpis.Length ? (natpis[i] ?? "") : "";
                bool en = ukljucen != null && i < ukljucen.Length && ukljucen[i];
                t += "<button type=\"button\" name=\"button" + (i + 1).ToString(CultureInfo.InvariantCulture)
                    + "\" caption=\"" + cap + "\" icon=\" \" enabled=\"" + (en ? "True" : "False")
                    + "\" visible=\"True\" />";
            }
            return t + "</buttons>";
        }

        /// <summary>Form1.vb 251–260. button1–button5, inače prazno.</summary>
        public static string KlikAlatne(string ime)
        {
            if (ime == "button1" || ime == "button2" || ime == "button3" || ime == "button4" || ime == "button5")
                return ime;
            return "";
        }

        public sealed class TipkaIshod
        {
            public bool Zatvori;
            public bool Pomoc;
            public bool Parametri;
            public bool Jezici;
        }

        /// <summary>Form1.vb 1350–1392. Escape zatvara. F1 samo uz HELPKOD. Ctrl+F11 parametri. Ctrl+F9 jezici.</summary>
        public static TipkaIshod Tipka(string tipka, bool control, string helpKod)
        {
            var i = new TipkaIshod();
            string k = tipka ?? "";
            if (string.Equals(k, "Escape", StringComparison.OrdinalIgnoreCase))
                i.Zatvori = true;
            if (string.Equals(k, "F1", StringComparison.OrdinalIgnoreCase))
            {
                if ((helpKod ?? "").Trim().Length > 0)
                {
                    i.Pomoc = true;
                    return i;
                }
            }
            if (control && string.Equals(k, "F11", StringComparison.OrdinalIgnoreCase))
                i.Parametri = true;
            if (control && string.Equals(k, "F9", StringComparison.OrdinalIgnoreCase))
                i.Jezici = true;
            return i;
        }

        /// <summary>Form1.vb 1298–1309. ONCLOSED ide u Rafalno. Greška ide u poruku.</summary>
        public static string Zatvoreno(string onClosed)
        {
            return onClosed ?? "";
        }

        /// <summary>Form1.vb 1694–1704. Prvi stupac prvog reda. Inače 0.</summary>
        public static string SqlOperatera(string korisnik, string lozinka)
        {
            return "select * from ssoperateri where naziv ='" + (korisnik ?? "").Trim() + "' and password='" + (lozinka ?? "").Trim() + "'";
        }

        public static int OperaterIz(DataTable tablica, bool greska)
        {
            if (greska || tablica == null || tablica.Rows.Count == 0 || tablica.Columns.Count == 0)
                return 0;
            object v = tablica.Rows[0][0];
            if (v == null || v == DBNull.Value)
                return 0;
            int n;
            if (int.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out n))
                return n;
            return 0;
        }

        public sealed class UlazArg
        {
            public string Xml = "";
            public string Server = "";
            public string Baza = "";
            public string DbKorisnik = "";
            public string DbLozinka = "";
            public string AppKorisnik = "";
            public string AppLozinka = "";
            public string Operater = "";
            public string Web = "";
            public string Jezik = "";
            public string Proxy = "";
        }

        /// <summary>Form1.vb 1707–1774. Prazan -xml padne na startup put.</summary>
        public static UlazArg Ulaz(string[] argumenti, string startup)
        {
            var u = new UlazArg();
            if (argumenti != null)
            {
                for (int i = 0; i < argumenti.Length; i++)
                {
                    string a = (argumenti[i] ?? "").ToLowerInvariant();
                    string sljedeci = i + 1 < argumenti.Length ? (argumenti[i + 1] ?? "") : "";
                    if (a == "-xml")
                        u.Xml = sljedeci.Replace("\"", "");
                    else if (a == "-s")
                        u.Server = sljedeci;
                    else if (a == "-d")
                        u.Baza = sljedeci;
                    else if (a == "-uid")
                        u.DbKorisnik = sljedeci;
                    else if (a == "-pwd")
                        u.DbLozinka = sljedeci;
                    else if (a == "-u")
                        u.AppKorisnik = sljedeci;
                    else if (a == "-p")
                        u.AppLozinka = sljedeci;
                    else if (a == "-o")
                        u.Operater = sljedeci;
                    else if (a == "-w")
                        u.Web = sljedeci;
                    else if (a == "-l")
                        u.Jezik = sljedeci;
                    else if (a == "-y")
                        u.Proxy = sljedeci;
                }
            }
            if (u.Xml.Length == 0)
                u.Xml = startup ?? "";
            return u;
        }

        /// <summary>Form1.vb 1176–1177 i 1563–1568.</summary>
        public static string ImeAplikacije(string xmlPut)
        {
            string p = xmlPut ?? "";
            int z = p.LastIndexOf('\\');
            string ime = z < 0 ? p : p.Substring(z + 1);
            return "NetGenDLL-" + ime;
        }

        public static string Spoj(string server, string baza, string login, string lozinka, string aplikacija)
        {
            if (string.IsNullOrEmpty(login))
                return "Server=" + (server ?? "") + ";Database=" + (baza ?? "") + ";Trusted_Connection=true;Application Name=" + (aplikacija ?? "") + ";";
            return "Server=" + (server ?? "") + ";Database=" + (baza ?? "") + ";UID=" + login + ";PWD=" + (lozinka ?? "") + ";Application Name=" + (aplikacija ?? "") + ";";
        }

        public static string SpojJupiterXml(string server, string login, string lozinka, string aplikacija)
        {
            return Spoj(server, "JupiterXML", login, lozinka, aplikacija);
        }

        /// <summary>Form1.vb 1670–1674. Oba stabla skrivena gase karticu.</summary>
        public static bool TabVidljiv(bool stablo1, bool stablo2)
        {
            return stablo1 || stablo2;
        }

        /// <summary>Form1.vb 2665–2691. True izlazi iz petlje. False ostaje dok ne dođe True.</summary>
        public static bool DozvolaKomande(string kontrola, DataTable tablica)
        {
            bool smije = true;
            string cilj = (kontrola ?? "").Replace(":", "").Trim();
            if (tablica == null || tablica.Rows.Count == 0 || !tablica.Columns.Contains("functionname") || !tablica.Columns.Contains("smije"))
                return true;
            for (int i = 0; i < tablica.Rows.Count; i++)
            {
                string fn = Convert.ToString(tablica.Rows[i]["functionname"], CultureInfo.InvariantCulture) ?? "";
                if (!string.Equals(fn.Trim(), cilj, StringComparison.OrdinalIgnoreCase))
                    continue;
                object s = tablica.Rows[i]["smije"];
                if (s == null || s == DBNull.Value)
                    continue;
                bool vrijednost;
                if (s is bool)
                    vrijednost = (bool)s;
                else
                {
                    string t = Convert.ToString(s, CultureInfo.InvariantCulture) ?? "";
                    if (string.Equals(t, "true", StringComparison.OrdinalIgnoreCase) || t == "1")
                        vrijednost = true;
                    else if (string.Equals(t, "false", StringComparison.OrdinalIgnoreCase) || t == "0")
                        vrijednost = false;
                    else
                        continue;
                }
                smije = vrijednost;
                if (vrijednost)
                    break;
            }
            return smije;
        }

        /// <summary>Form1.vb 2694–2707. Ime je datoteka iza zadnje kose crte.</summary>
        public static string SqlDozvole(string xmlPut, int operater)
        {
            string p = xmlPut ?? "";
            int z = p.LastIndexOf('\\');
            string ime = (z < 0 ? p : p.Substring(z + 1)).Trim();
            return "select *,1 as sort from ssPermission where objectname='" + ime + "' and ((OperateriID = " + operater.ToString(CultureInfo.InvariantCulture)
                + ") or (GrupeID = (select top 1 GrupeId from ssOperateri where OperateriId = " + operater.ToString(CultureInfo.InvariantCulture)
                + ") )) union select *,2 as sort from ssPermission where objectname='" + ime + "' and OperateriID = 0 and grupeid=0 order by sort ,isgrupa desc,grupeid,operateriid";
        }

        /// <summary>Form1.vb 2509–2534.</summary>
        public static string MapaRasporeda()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SPIN Informatica\\Jupiter Software\\Grid Settings\\";
        }

        public sealed class JezikMp
        {
            public string SqlLogin = "select isnull(jezik,'') as jezik from jupitersystem..loginimages where [LOGIN] = suser_sname()";
            public string SqlKasa = "select jezik from MPKase";
            public string Jezik = "";
            public bool Inicijalizira;
        }

        /// <summary>Form1.vb 1398–1471. hrv ne inicijalizira. Return prije TotalGenLang.</summary>
        public static JezikMp JezikIz(bool prevoditelj, string vecJezik, string izLogina, bool loginImaRed, string izKase, bool kasaImaRed)
        {
            var j = new JezikMp();
            if (!prevoditelj)
                return j;
            j.Jezik = vecJezik ?? "";
            if (j.Jezik.Length == 0 && loginImaRed)
                j.Jezik = izLogina ?? "";
            if (j.Jezik.Length == 0 && kasaImaRed)
                j.Jezik = izKase ?? "";
            if (j.Jezik.Trim().Length > 0 && j.Jezik.Trim().ToLowerInvariant() != "hrv")
                j.Inicijalizira = true;
            return j;
        }

        public static string SqlMultilingual()
        {
            return " select * from Pubvars where VarIme = 'Multilingual'";
        }

        public static string SqlPismo(string jezik)
        {
            return " select * from jezici where oznaka = '" + (jezik ?? "").Trim() + "'";
        }

        /// <summary>Form1.vb 1016–1054. Redovi se slažu unatrag, pa je prvi zadnji dodani.</summary>
        public static string DajCb(string verzija, string server, string baza, string operater, string xml, string node, string id, string[][] cb)
        {
            string sss = "";
            if (cb != null)
            {
                for (int j = 0; j < cb.Length; j++)
                {
                    string ime = cb[j] == null || cb[j].Length == 0 ? "" : (cb[j][0] ?? "");
                    string vrijednost = cb[j] == null || cb[j].Length < 2 ? "" : (cb[j][1] ?? "");
                    if (ime.Trim().Length == 0)
                        continue;
                    sss += j.ToString(CultureInfo.InvariantCulture) + ".  @@" + ime + vrijednost + "\r\n";
                }
            }
            string t = "---------------------------\r\n";
            t = "@@id= " + (id ?? "") + "\r\n\r\n" + t;
            t = "@@node= " + (node ?? "") + "\r\n\r\n" + t;
            t = "XML = " + (xml ?? "") + "\r\n\r\n" + t;
            t = "NetGen DLL v" + (verzija ?? "") + "\r\nServer=" + (server ?? "") + "\r\nDB=" + (baza ?? "") + "\r\n @@operateriid=" + (operater ?? "") + "\r\n\r\n" + t;
            return t + sss;
        }

        /// <summary>Form1.vb 1057–1063. Indeks 0 završi prvi.</summary>
        public static string DajSql(string[] cbLog)
        {
            string t = "";
            if (cbLog == null)
                return t;
            for (int i = cbLog.Length - 1; i >= 0; i--)
                t = (cbLog[i] ?? "") + "\r\n\r\n" + t;
            return t;
        }

        /// <summary>Form1.vb 1066–1068. Tipfeler PARAMATERI je u izvoru.</summary>
        public static string DajParametre(string parametri)
        {
            return "PARAMATERISAMASTERIDETAILGRIDA=" + (parametri ?? "");
        }

        public static string TekstIznimke(string trag)
        {
            return trag ?? "";
        }

        public sealed class ProstorIshod
        {
            public bool Izlaz;
            public string Poruka = "";
            public bool Slo;
            public string Url = "";
            public string Timeout = "";
            public bool Salje;
            public string Sql = "";
            public string SqlXml = "";
        }

        /// <summary>Form1.vb 849–934. Slanje ide samo na testni CIS.</summary>
        public static ProstorIshod Prostor(string trgovinaId, DataTable postavke)
        {
            var i = new ProstorIshod();
            string id = (trgovinaId ?? "").Trim();
            i.Sql = "select * from mpfiskalizacija where partneriid=(select partneriid from trgovina where trgovinaid=" + id + " )";
            i.SqlXml = "spMPXMLProstorZahtijev @trgovinaid=" + id;
            if (postavke == null || postavke.Rows.Count == 0)
            {
                i.Izlaz = true;
                i.Poruka = "Nema ništa u MPFISKALIZACIJA !!!";
                return i;
            }
            DataRow r = postavke.Rows[0];
            string url = Polje(r, "url");
            if (url.ToLowerInvariant().IndexOf("fu.gov.si", StringComparison.Ordinal) >= 0)
            {
                i.Slo = true;
                i.Izlaz = true;
                return i;
            }
            if (url.Trim().Length == 0)
                url = ProdukcijskiCis;
            string timeout = Polje(r, "timeout");
            if (timeout.Trim().Length == 0)
                timeout = "5000";
            string pfx = Polje(r, "pfx");
            string loz = Polje(r, "lozinkazapfx");
            if (pfx.Trim().Length == 0)
            {
                i.Izlaz = true;
                i.Poruka = "Nije definiran PFX !";
                i.Url = url;
                return i;
            }
            if (loz.Trim().Length == 0)
            {
                i.Izlaz = true;
                i.Poruka = "Nije definirana lozinka za  PFX !";
                i.Url = url;
                return i;
            }
            i.Url = url;
            i.Timeout = timeout;
            i.Salje = FiskalTok.JeTest(url);
            return i;
        }

        /// <summary>Form1.vb 2232–2255.</summary>
        public static string BrojStavki(int master, bool masterTotal, int detail, bool detailTotal, string opis, string jezik)
        {
            int m = masterTotal ? master - 1 : master;
            int d = detailTotal ? detail - 1 : detail;
            if (m < 0)
                m = 0;
            if (d < 0)
                d = 0;
            return Prevod.Tekst("Pronađeno", jezik) + " : " + m.ToString(CultureInfo.InvariantCulture) + " / " + d.ToString(CultureInfo.InvariantCulture) + "   " + (opis ?? "");
        }

        public static string NaslovStavki(int detail, bool detailTotal)
        {
            int d = detailTotal ? detail - 1 : detail;
            if (d < 0)
                d = 0;
            return "Stavke (" + d.ToString(CultureInfo.InvariantCulture) + ")";
        }

        /// <summary>Form1.vb 2189–2226. Broj se ne prevodi. Stupac 0 se preskače.</summary>
        public static string[] PrevediStupce(string popis, string[] imena, string[] vrijednosti, string jezik)
        {
            var izlaz = new string[vrijednosti == null ? 0 : vrijednosti.Length];
            if (vrijednosti != null)
            {
                for (int i = 0; i < vrijednosti.Length; i++)
                    izlaz[i] = vrijednosti[i];
            }
            if ((popis ?? "").Trim().Length == 0 || imena == null)
                return izlaz;
            string kljuc = "," + popis + ",";
            for (int i = 1; i < izlaz.Length && i < imena.Length; i++)
            {
                string ime = (imena[i] ?? "").Trim();
                if (ime.Length >= 4)
                {
                    int sirina;
                    if (int.TryParse(ime.Substring(ime.Length - 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out sirina) && sirina != 0)
                        ime = ime.Substring(0, ime.Length - 4);
                }
                if (kljuc.ToLowerInvariant().IndexOf("," + ime.Trim().ToLowerInvariant() + ",", StringComparison.Ordinal) < 0)
                    continue;
                string v = izlaz[i] ?? "";
                decimal broj;
                if (decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out broj)
                    || decimal.TryParse(v, NumberStyles.Number, CultureInfo.CurrentCulture, out broj))
                    continue;
                izlaz[i] = Prevod.Tekst(v.Trim(), jezik);
            }
            return izlaz;
        }

        public sealed class StupacFormat
        {
            public int Sirina;
            public string Format = "";
            public bool Desno;
            public string Natpis = "";
            public bool Sakriven;
        }

        /// <summary>Form1.vb 1777–1822. Zadnje 4 znamenke natpisa dijele se s 14.</summary>
        public static StupacFormat FormatStupca(string tip, string natpis)
        {
            var s = new StupacFormat();
            string t = (tip ?? "").ToLowerInvariant();
            string c = (natpis ?? "").Trim();
            if (t.IndexOf("int", StringComparison.Ordinal) >= 0)
            {
                s.Sirina = 34;
                s.Format = "##0";
                s.Desno = true;
            }
            if (t.IndexOf("decimal", StringComparison.Ordinal) >= 0 || t.IndexOf("double", StringComparison.Ordinal) >= 0)
            {
                s.Sirina = 44;
                s.Format = "#,##0.00";
                s.Desno = true;
            }
            if (t.IndexOf("string", StringComparison.Ordinal) >= 0)
                s.Sirina = 200;
            if (t.IndexOf("date", StringComparison.Ordinal) >= 0)
            {
                s.Sirina = 100;
                s.Format = "dd.MM.yyyy";
            }
            if (c.Length >= 4)
            {
                int n;
                if (int.TryParse(c.Substring(c.Length - 4), NumberStyles.Integer, CultureInfo.InvariantCulture, out n) && n != 0)
                {
                    s.Sirina = (int)Math.Round(n / 14.0, MidpointRounding.ToEven);
                    c = c.Substring(0, c.Length - 4);
                }
            }
            s.Natpis = c.Replace(";", "\r\n");
            if (s.Natpis.Length >= 2 && s.Natpis.Substring(s.Natpis.Length - 2, 2).ToUpperInvariant() == "ID")
                s.Sakriven = true;
            return s;
        }

        /// <summary>Form1.vb 2447–2463.</summary>
        public static string DmsNaslov(int redova)
        {
            return "DMS (" + redova.ToString(CultureInfo.InvariantCulture) + ")";
        }

        /// <summary>Form1.vb 4127–4134. Put koji sadrži .xml gasi prvo stablo.</summary>
        public static bool PrvoStablo(string put)
        {
            return (put ?? "").Trim().ToLowerInvariant().IndexOf(".xml", StringComparison.Ordinal) < 0;
        }

        /// <summary>Form1.vb 4088–4122. Korijen je red kojem je stupac 1 null. Natpis je stupac 2.</summary>
        public static List<string> IzbornikBaza(DataTable sve)
        {
            var korijen = new List<string>();
            if (sve == null)
                return korijen;
            for (int i = 0; i < sve.Rows.Count; i++)
            {
                if (sve.Rows[i][1] == DBNull.Value || sve.Rows[i][1] == null)
                    korijen.Add(Convert.ToString(sve.Rows[i][2], CultureInfo.InvariantCulture) ?? "");
            }
            return korijen;
        }

        public static string SqlIzbornika(object roditelj)
        {
            if (roditelj == null)
                return "select * from netgenmenu";
            return "select * from netgenmenu where parentid=" + Convert.ToString(roditelj, CultureInfo.InvariantCulture);
        }

        /// <summary>Form1.vb 4037–4060. Prva stavka je XML-ovi / BUILDIN 1.</summary>
        public static List<string> IzbornikMape(string mapa)
        {
            var l = new List<string>();
            l.Add("XML-ovi");
            l.Add("BUILDIN 1 : XMLFile1");
            if (string.IsNullOrEmpty(mapa) || !Directory.Exists(mapa))
                return l;
            string[] datoteke = Directory.GetFiles(mapa, "*.xml");
            Array.Sort(datoteke, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < datoteke.Length; i++)
            {
                string cap = NatpisProjekta(datoteke[i]);
                if (cap.Length > 0)
                    l.Add(cap);
            }
            return l;
        }

        private static string NatpisProjekta(string put)
        {
            try
            {
                var d = new XmlDocument();
                d.Load(put);
                XmlNode n = d.SelectSingleNode("/PROJECT/CAPTION");
                if (n == null)
                    return "";
                return n.InnerText ?? "";
            }
            catch
            {
                return "";
            }
        }

        public sealed class CvorMeni
        {
            public string Natpis = "";
            public string Tag = "";
            public readonly List<CvorMeni> Djeca = new List<CvorMeni>();
        }

        /// <summary>Form1.vb 4227–4231. http ide kroz kosu crtu, disk kroz obrnutu.</summary>
        public static string PutMeniXml(string put)
        {
            string p = put ?? "";
            if (p.Trim().ToLowerInvariant().IndexOf("http", StringComparison.Ordinal) >= 0)
                return p + "/menu.xml";
            return p + "\\menu.xml";
        }

        /// <summary>Form1.vb 4236–4282. Natpis je caption. Tag je put plus xml.</summary>
        public static CvorMeni MeniIzXml(string xml, string put)
        {
            var korijen = new CvorMeni();
            if (string.IsNullOrEmpty(xml))
                return null;
            var d = new XmlDocument();
            d.LoadXml(xml);
            if (d.DocumentElement == null || d.DocumentElement.Attributes["caption"] == null)
                return null;
            korijen.Natpis = d.DocumentElement.Attributes["caption"].Value ?? "";
            if (!DodajCvorove(d.DocumentElement, korijen, put))
                return null;
            return korijen;
        }

        /// <summary>Form1.vb 5544–5563. Usporedba je trim i mala slova.</summary>
        public static int NadjiPoTekstu(string trazeno, string[] tekstovi)
        {
            string t = (trazeno ?? "").Trim().ToLowerInvariant();
            if (tekstovi == null)
                return -1;
            for (int i = 0; i < tekstovi.Length; i++)
            {
                if (t == (tekstovi[i] ?? "").Trim().ToLowerInvariant())
                    return i;
            }
            return -1;
        }

        /// <summary>Form1.vb 5556–5661. Lijevo od prve dvotočke je čvor, desno forma.</summary>
        public static void RastaviTag(string tag, out string cvor, out string forma)
        {
            cvor = "";
            forma = "";
            string t = tag ?? "";
            int z = t.IndexOf(':');
            if (z < 0)
                return;
            cvor = t.Substring(0, z);
            forma = t.Substring(z + 1);
        }

        /// <summary>Form1.vb 5566–5582. Bez reda je 0. Bez stupca id ide stupac 2. Prazan tekst ostaje.</summary>
        public static string Id(bool imaRedova, bool idGreska, string idCelija, string stupac2)
        {
            if (!imaRedova)
                return "0";
            if (!idGreska)
                return (idCelija ?? "").Trim();
            return (stupac2 ?? "").Trim();
        }

        /// <summary>Form1.vb 5588–5599. Prazan ili null id padne na stupac 0, pa na 0.</summary>
        public static string DxId(bool imaRedova, bool idLose, string idCelija, string stupac0)
        {
            if (!imaRedova)
                return "0";
            string v = idLose ? (stupac0 ?? "") : (idCelija ?? "");
            if (v.Length == 0)
                return "0";
            return v;
        }

        /// <summary>Form1.vb 6250–6267. Prefiks je GridMaster. DX sprema xml, Janus gxl.</summary>
        public static string DatotekaRasporeda(string xmlPut, string posebno, bool viseFormi, string aktivna, string jezik, bool dx)
        {
            string p = xmlPut ?? "";
            int z = p.LastIndexOf('\\');
            string ime = z < 0 ? p : p.Substring(z);
            ime = ime.Replace("\\", "").Trim().Replace(".xml", "").Trim();
            if ((posebno ?? "").Length > 0)
                ime = posebno;
            string forma = viseFormi ? (aktivna ?? "") : "";
            string ext = dx ? ".xml" : ".gxl";
            return "GridMaster" + ime + forma + "-" + (jezik ?? "") + ext;
        }

        private static bool DodajCvorove(XmlNode cvor, CvorMeni roditelj, string put)
        {
            if (cvor == null || !cvor.HasChildNodes)
                return true;
            string sep = (put ?? "").Trim().ToLowerInvariant().IndexOf("http", StringComparison.Ordinal) >= 0 ? "/" : "\\";
            for (int i = 0; i < cvor.ChildNodes.Count; i++)
            {
                XmlNode x = cvor.ChildNodes[i];
                if (x.NodeType != XmlNodeType.Element)
                    continue;
                if (x.Attributes == null || x.Attributes["caption"] == null || x.Attributes["xml"] == null)
                    return false;
                var t = new CvorMeni();
                t.Natpis = x.Attributes["caption"].Value ?? "";
                t.Tag = (put ?? "") + sep + (x.Attributes["xml"].Value ?? "");
                roditelj.Djeca.Add(t);
                if (!DodajCvorove(x, t, put))
                    return false;
            }
            return true;
        }

        private static string Polje(DataRow red, string ime)
        {
            if (red == null || red.Table == null || !red.Table.Columns.Contains(ime) || red[ime] == DBNull.Value || red[ime] == null)
                return "";
            return Convert.ToString(red[ime], CultureInfo.InvariantCulture) ?? "";
        }
    }
}

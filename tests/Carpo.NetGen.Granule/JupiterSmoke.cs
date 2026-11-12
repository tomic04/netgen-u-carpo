using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Otvaranje NetGen granula na jupiter2019 i jupiter2022. Jedna granula, jedna konekcija.
    /// Prije EXEC-a tijelo procedure iz sys.sql_modules, rekurzivno. Ništa se ne briše.
    /// </summary>
    internal static class JupiterSmoke
    {
        private const int Timeout = 30;

        private static readonly string[][] Baze = new string[][]
        {
            new[] { "jupiter2019", "Biovega", "422" },
            new[] { "jupiter2019", "CentarTehnike", "174" },
            new[] { "jupiter2019", "Setcor", "152" },
            new[] { "jupiter2019", "Enna", "167" },
            new[] { "jupiter2019", "Ireks", "172" },
            new[] { "jupiter2022", "Panissimo", "311" },
            new[] { "jupiter2022", "Tusak", "309" },
            new[] { "jupiter2022", "CentarTehnike", "177" },
            new[] { "jupiter2022", "Setcor", "124" }
        };

        public static int Pokreni(string root, string izlaz)
        {
            var sb = new StringBuilder();
            sb.Append("# Smoke Jupiter\n\n");
            sb.Append("Samo put otvaranja: ONLOAD, master, stablo, detail, combo i SQL na filteru. ");
            sb.Append("Nema gumba, nema ONSUBMIT, nema ispisa, nema fiskalizacije.\n\n");
            sb.Append("Prije ovog runnera smoke na jupiter2019 i jupiter2022 nije pokrenut. Nijedna procedura nije izvršena.\n\n");
            sb.Append("`Application Name=CarpoNetGen-smoke`, `CommandTimeout` 30 s, `Pooling=false`, jedna granula pa zatvorena konekcija. Operater je 7, `@@node` je `null`.\n\n");
            sb.Append("Zaštita ne gleda samo prvu riječ. `spKntZakljucivanjeTem @action=1` i `spKntPreknjizavanjeNG @action=2` su poziv procedure. ");
            sb.Append("Prije EXEC-a ide `SELECT` iz `sys.sql_modules`, i rekurzivno za procedure koje tijelo zove. ");
            sb.Append("INSERT/UPDATE/DELETE/MERGE/TRUNCATE/DROP nad tablicom koja nije `#temp` ili `@varijabla` se ne pokreće (`procedura-piše`). ");
            sb.Append("Izravni tekst s DELETE/UPDATE/DROP/TRUNCATE/MERGE/ALTER/GRANT/REVOKE, CREATE osim `CREATE TABLE #temp`, ili INSERT osim u `#temp`, je `preskočeno-zaštita`.\n\n");
            sb.Append("Usporedba s `docs/granule/ssMeni-svi-serveri.md` (inventar, CI 43233316047). Inventar je broj NetGen redova, ne broj datoteka.\n\n");
            bool stop = false;
            for (int i = 0; i < Baze.Length && !stop; i++)
            {
                string server = Baze[i][0];
                string baza = Baze[i][1];
                int inventar;
                int.TryParse(Baze[i][2], out inventar);
                sb.Append("## ").Append(baza).Append(" (").Append(server).Append(")\n\n");
                try
                {
                    string dio;
                    stop = Baza(root, server, baza, inventar, out dio);
                    sb.Append(dio);
                }
                catch (Exception ex)
                {
                    sb.Append("Veza nije uspjela. Ništa nije izvršeno. ").Append(Celija(ex.GetType().Name + ": " + ex.Message)).Append("\n\n");
                }
                Upisi(izlaz, sb.ToString());
                Console.WriteLine("zavrseno " + server + " " + baza + (stop ? " STOP" : ""));
            }
            if (!stop)
                sb.Append("Nijedna procedura koja piše nije pokrenuta.\n");
            Upisi(izlaz, sb.ToString());
            Console.WriteLine(sb.ToString());
            return stop ? 3 : 0;
        }

        private static bool Baza(string root, string server, string baza, int inventar, out string tekst)
        {
            var sb = new StringBuilder();
            var kes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var indeks = Indeks(Path.Combine(root, "definitions", "netgen"));
            List<string> parametri = Izbornik(server, baza);
            int retci = 0;
            int prazno = 0;
            int pad = 0;
            int zastita = 0;
            int pise = 0;
            int nema = 0;
            var stavke = new StringBuilder();
            var pisci = new StringBuilder();
            bool stop = false;
            for (int i = 0; i < parametri.Count && !stop; i++)
            {
                Red r = Granula(root, indeks, server, baza, parametri[i], kes);
                if (r.Ishod == "s-retcima")
                    retci++;
                else if (r.Ishod == "prazno")
                    prazno++;
                else if (r.Ishod == "pad")
                    pad++;
                else if (r.Ishod == "preskočeno-zaštita")
                    zastita++;
                else if (r.Ishod == "procedura-piše")
                    pise++;
                else if (r.Ishod == "nema-xml")
                    nema++;
                stavke.Append("| ").Append(Celija(r.Ime)).Append(" | ").Append(r.Ishod).Append(" | ").Append(r.Redova < 0 ? "" : r.Redova.ToString());
                stavke.Append(" | ").Append(Celija(r.Poruka)).Append(" |\n");
                if (r.Ishod == "procedura-piše")
                {
                    pisci.Append("- ").Append(Celija(r.Ime)).Append(": ").Append(Celija(r.Poruka)).Append("\n");
                }
                if (r.PoslanUpis)
                {
                    stop = true;
                    sb.Append("STOP. Poslan je upis.\n\n");
                    sb.Append("Baza: ").Append(server).Append(" / ").Append(baza).Append("\n\n");
                    sb.Append("Granula: ").Append(Celija(r.Ime)).Append("\n\n");
                    sb.Append("Parametri: ").Append(Celija(r.Poruka)).Append("\n\n");
                }
                Console.WriteLine(baza + " " + (i + 1) + "/" + parametri.Count + " " + r.Ime + " " + r.Ishod);
            }
            sb.Append("Inventar ").Append(inventar).Append(", izbornik ").Append(parametri.Count);
            sb.Append(", s retcima ").Append(retci).Append(", prazno ").Append(prazno);
            sb.Append(", pad ").Append(pad).Append(", preskočeno-zaštita ").Append(zastita);
            sb.Append(", procedura-piše ").Append(pise).Append(", nema-xml ").Append(nema).Append(".\n\n");
            if (pisci.Length > 0)
            {
                sb.Append("### procedura-piše\n\n").Append(pisci).Append("\n");
            }
            sb.Append("| XML | Ishod | Redova | Poruka |\n|---|---|---:|---|\n");
            sb.Append(stavke).Append("\n");
            tekst = sb.ToString();
            return stop;
        }

        private static List<string> Izbornik(string server, string baza)
        {
            var lista = new List<string>();
            var vidjeni = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var conn = Nova(server, baza))
            using (var cmd = new SqlCommand("select distinct Parametar from dbo.ssMeni with (nolock) where Parametar like '%netgendll%' order by Parametar", conn))
            {
                cmd.CommandTimeout = Timeout;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string p = rd.IsDBNull(0) ? "" : Convert.ToString(rd.GetValue(0));
                        p = (p ?? "").Trim();
                        if (p.Length == 0 || !vidjeni.Add(p))
                            continue;
                        string n = p.ToLowerInvariant();
                        if (n.IndexOf("netgendll.loader", StringComparison.Ordinal) < 0 && n.IndexOf("netgendll.frm", StringComparison.Ordinal) < 0)
                            continue;
                        lista.Add(p);
                    }
                }
            }
            return lista;
        }

        private static Red Granula(string root, Dictionary<string, List<string>> indeks, string server, string baza, string parametar, Dictionary<string, string> kes)
        {
            var r = new Red();
            r.Redova = -1;
            string put = PutXml(parametar);
            r.Ime = put.Length == 0 ? parametar : Path.GetFileName(put);
            string fajl = Nadji(indeks, put, baza);
            if (fajl == null)
            {
                r.Ishod = "nema-xml";
                r.Poruka = put;
                return r;
            }
            r.Ime = Path.GetFileName(fajl);
            var ucitano = NetGenXmlReader.Ucitaj(fajl);
            if (!ucitano.Uspjeh || ucitano.Projekt == null)
            {
                r.Ishod = "pad";
                r.Poruka = ucitano.Greska ?? "XML";
                return r;
            }
            var kontekst = new SqlKontekst();
            kontekst.OperaterId = "7";
            kontekst.Node = "null";
            kontekst.Id = "";
            kontekst.XmlName = r.Ime;
            string master;
            List<string> upiti = Otvaranje.Upiti(ucitano.Projekt, kontekst, r.Ime, out master);
            string detail = Otvaranje.Detail(ucitano.Projekt, kontekst);
            if (detail.Length > 0)
                upiti.Add(detail);
            for (int i = 0; i < upiti.Count; i++)
            {
                if (upiti[i].IndexOf("fiskal", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    r.Ishod = "preskočeno-zaštita";
                    r.Poruka = "fiskalizacija";
                    return r;
                }
                SqlZastita z = ZastitaSql.Tekst(upiti[i]);
                if (z.Pise)
                {
                    r.Ishod = "preskočeno-zaštita";
                    r.Poruka = z.Razlog + " | " + Skrati(upiti[i]);
                    return r;
                }
            }
            using (var conn = Nova(server, baza))
            {
                conn.Open();
                for (int i = 0; i < upiti.Count; i++)
                {
                    string razlog = Procedure(conn, upiti[i], kes);
                    if (razlog != null)
                    {
                        r.Ishod = "procedura-piše";
                        r.Poruka = razlog + " | " + Skrati(upiti[i]);
                        return r;
                    }
                }
                int masterRedova = 0;
                DataTable masterTablica = null;
                for (int i = 0; i < upiti.Count; i++)
                {
                    if (detail.Length > 0 && upiti[i] == detail)
                        continue;
                    string greska;
                    bool poslano;
                    DataTable tablica = Izvrsi(conn, upiti[i], kes, out greska, out poslano);
                    if (poslano)
                    {
                        r.PoslanUpis = true;
                        r.Ishod = "pad";
                        r.Poruka = Skrati(upiti[i]);
                        return r;
                    }
                    if (greska != null)
                    {
                        if (greska.StartsWith("zaustavljeno", StringComparison.Ordinal))
                        {
                            r.Ishod = "preskočeno-zaštita";
                            r.Poruka = Skrati(greska);
                            return r;
                        }
                        r.Ishod = "pad";
                        r.Poruka = Skrati(greska);
                        return r;
                    }
                    if (master.Length > 0 && upiti[i] == master)
                        masterTablica = tablica;
                }
                if (masterTablica != null)
                    masterRedova = masterTablica.Rows.Count;
                if (masterTablica != null && masterTablica.Rows.Count > 0 && detail.Length > 0)
                {
                    var det = new DetailGrid();
                    det.Kontekst.OperaterId = kontekst.OperaterId;
                    det.Kontekst.Node = kontekst.Node;
                    det.Kontekst.XmlName = kontekst.XmlName;
                    det.Pripremi(ucitano.Projekt);
                    string detSql = det.Slozi(masterTablica.Rows[0]);
                    if (!string.IsNullOrWhiteSpace(detSql) && SqlTekst.Odluka(detSql) == "salji")
                    {
                        string razlog = Procedure(conn, detSql, kes);
                        if (razlog != null)
                        {
                            r.Ishod = "procedura-piše";
                            r.Poruka = razlog + " | " + Skrati(detSql);
                            r.Redova = masterRedova;
                            return r;
                        }
                        string greska;
                        bool poslano;
                        Izvrsi(conn, detSql, kes, out greska, out poslano);
                        if (poslano)
                        {
                            r.PoslanUpis = true;
                            r.Ishod = "pad";
                            r.Poruka = "detail: " + Skrati(detSql);
                            r.Redova = masterRedova;
                            return r;
                        }
                        if (greska != null)
                        {
                            r.Ishod = greska.StartsWith("zaustavljeno", StringComparison.Ordinal) ? "preskočeno-zaštita" : "pad";
                            r.Poruka = "detail: " + Skrati(greska);
                            r.Redova = masterRedova;
                            return r;
                        }
                    }
                }
                r.Redova = master.Length > 0 ? masterRedova : 0;
                r.Ishod = r.Redova > 0 ? "s-retcima" : "prazno";
            }
            return r;
        }

        private static string Procedure(SqlConnection conn, string sql, Dictionary<string, string> kes)
        {
            SqlZastita z = ZastitaSql.Tekst(sql);
            if (z.Pise)
                return z.Razlog;
            var posjet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < z.Procedure.Count; i++)
            {
                string razlog = Tijelo(conn, z.Procedure[i], kes, posjet, 0);
                if (razlog != null)
                    return z.Procedure[i] + ": " + razlog;
            }
            return null;
        }

        private static string Tijelo(SqlConnection conn, string ime, Dictionary<string, string> kes, HashSet<string> posjet, int dubina)
        {
            if (string.IsNullOrEmpty(ime))
                return null;
            string kesirano;
            if (kes.TryGetValue(ime, out kesirano))
                return kesirano.Length == 0 ? null : kesirano;
            if (dubina > 8)
                return "preduboko";
            if (!posjet.Add(ime))
                return null;
            bool nasao = false;
            bool necitljivo = false;
            var definicije = new List<string>();
            using (var cmd = new SqlCommand(
                "select m.definition from sys.objects o left join sys.sql_modules m on m.object_id = o.object_id where o.name = @ime and o.type in ('P','PC','FN','IF','TF','V')",
                conn))
            {
                cmd.CommandTimeout = Timeout;
                cmd.Parameters.Add("@ime", SqlDbType.NVarChar, 256).Value = ime;
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        nasao = true;
                        if (rd.IsDBNull(0))
                            necitljivo = true;
                        else
                            definicije.Add(rd.GetString(0));
                    }
                }
            }
            if (!nasao)
            {
                kes[ime] = "";
                return null;
            }
            if (necitljivo)
            {
                kes[ime] = "tijelo nije čitljivo";
                return kes[ime];
            }
            for (int i = 0; i < definicije.Count; i++)
            {
                SqlZastita z = ZastitaSql.Tijelo(definicije[i]);
                if (z.Pise)
                {
                    kes[ime] = z.Razlog;
                    return z.Razlog;
                }
                for (int p = 0; p < z.Procedure.Count; p++)
                {
                    string dublje = Tijelo(conn, z.Procedure[p], kes, posjet, dubina + 1);
                    if (dublje != null)
                    {
                        string razlog = z.Procedure[p] + ": " + dublje;
                        kes[ime] = razlog;
                        return razlog;
                    }
                }
            }
            kes[ime] = "";
            return null;
        }

        private static DataTable Izvrsi(SqlConnection conn, string sql, Dictionary<string, string> kes, out string greska, out bool poslano)
        {
            greska = null;
            poslano = false;
            SqlZastita z = ZastitaSql.Tekst(sql);
            if (z.Pise)
            {
                greska = "zaustavljeno prije slanja: " + z.Razlog;
                return null;
            }
            string procedura = Procedure(conn, sql, kes);
            if (procedura != null)
            {
                greska = "zaustavljeno prije slanja: " + procedura;
                return null;
            }
            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = Timeout;
                    using (var rd = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(rd);
                        poslano = ZastitaSql.Tekst(sql).Pise;
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                greska = ex.Message;
                return null;
            }
        }

        private static SqlConnection Nova(string server, string baza)
        {
            string cs = "Server=" + server + ";Database=" + baza + ";Integrated Security=True;Pooling=false;Connect Timeout=15;Application Name=CarpoNetGen-smoke";
            return new SqlConnection(cs);
        }

        private static string PutXml(string parametar)
        {
            int a = parametar.IndexOf(':');
            if (a < 0)
                return "";
            int b = parametar.IndexOf(':', a + 1);
            if (b < 0 || b + 1 >= parametar.Length)
                return "";
            return parametar.Substring(b + 1).Trim();
        }

        private static Dictionary<string, List<string>> Indeks(string netgen)
        {
            var map = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists(netgen))
                return map;
            string[] fajlovi = Directory.GetFiles(netgen, "*.xml", SearchOption.AllDirectories);
            for (int i = 0; i < fajlovi.Length; i++)
            {
                string ime = Path.GetFileName(fajlovi[i]);
                List<string> lista;
                if (!map.TryGetValue(ime, out lista))
                {
                    lista = new List<string>();
                    map[ime] = lista;
                }
                lista.Add(fajlovi[i]);
            }
            return map;
        }

        private static string Nadji(Dictionary<string, List<string>> indeks, string put, string baza)
        {
            if (string.IsNullOrEmpty(put))
                return null;
            string ime = Path.GetFileName(put.Replace('/', '\\'));
            List<string> lista;
            if (!indeks.TryGetValue(ime, out lista) || lista.Count == 0)
                return null;
            string norm = put.Replace('/', '\\').TrimStart('\\');
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i].EndsWith(norm, StringComparison.OrdinalIgnoreCase))
                    return lista[i];
            }
            if (string.Equals(baza, "Biovega", StringComparison.OrdinalIgnoreCase))
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    if (lista[i].IndexOf("BIOVEGA", StringComparison.OrdinalIgnoreCase) >= 0)
                        return lista[i];
                }
            }
            if (norm.IndexOf('\\') >= 0)
                return null;
            return lista[0];
        }

        private static void Upisi(string izlaz, string tekst)
        {
            if (string.IsNullOrWhiteSpace(izlaz))
                return;
            string dir = Path.GetDirectoryName(izlaz);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(izlaz, tekst, new UTF8Encoding(false));
        }

        private static string Skrati(string s)
        {
            if (s == null)
                return "";
            s = s.Replace("\r", " ").Replace("\n", " ").Trim();
            if (s.Length > 220)
                return s.Substring(0, 220);
            return s;
        }

        private static string Celija(string s)
        {
            return (s ?? "").Replace("|", "/").Replace("\r", " ").Replace("\n", " ");
        }

        private sealed class Red
        {
            public string Ime = "";
            public string Ishod = "";
            public string Poruka = "";
            public int Redova;
            public bool PoslanUpis;
        }
    }
}

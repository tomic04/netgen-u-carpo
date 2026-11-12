using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Samo citanje ssMeni na tudjim serverima. Dozvoljen je iskljucivo SELECT.
    /// Nema EXEC, INSERT, UPDATE, DELETE, DDL, privremenih tablica ni pokretanja granula.
    /// </summary>
    public static class ServeriMeni
    {
        public static string Tekst;

        private const int TimeoutSekundi = 8;
        private const int MaxBaza = 300;
        private const int MaxRedova = 800;

        private static readonly string[] NatpisStupci = { "Opis", "Naziv", "Name", "Caption", "Tekst" };

        private static readonly Cilj[] Jupiter2019 =
        {
            new Cilj("biovega", "Biovega", true),
            new Cilj("centartehnike", "Centar tehnike", false),
            new Cilj("panissimo", "Panissimo", false),
            new Cilj("setcor", "Setcor", false),
            new Cilj("tusal", "Tusal", false)
        };

        private static readonly Cilj[] Jupiter2022 =
        {
            new Cilj("enna", "Enna", false),
            new Cilj("fidus", "Fidus", false),
            new Cilj("ireks", "Ireks", false)
        };

        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            var katalog = new List<KatalogRed>();
            var pristup = new List<PristupRed>();
            var stavke = new List<MeniRed>();

            l.Add(ProvjeriTekstUpita());

            CitajServer("jupiter2019", Jupiter2019, katalog, pristup, stavke, l);
            CitajServer("jupiter2022", Jupiter2022, katalog, pristup, stavke, l);
            CitajSpin(pristup, stavke, l);

            var definicije = IndeksXml(Path.Combine(root, "definitions", "netgen"), root);
            var checkliste = IndeksMd(Path.Combine(root, "docs", "granule"));
            List<XmlRed> xml = Agregiraj(stavke, definicije, checkliste);

            string dir = Path.Combine(root, "artifacts", "granule");
            try
            {
                Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, "ssmeni-serveri.tsv"), Tsv(katalog, pristup, stavke, xml), new UTF8Encoding(false));
                File.WriteAllText(Path.Combine(dir, "ssmeni-svi-serveri.md"), Markdown(katalog, pristup, xml), new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                l.Add(Qa.O("ssMeni-serveri", "SSMENI-DAT", "zapis artefakta", "artifacts/granule", Skrati(ex.GetType().Name + ": " + ex.Message)));
            }

            Tekst = Sazetak(pristup, xml);
            return l;
        }

        private static Stavka ProvjeriTekstUpita()
        {
            string[] upiti =
            {
                "select top (300) name, state_desc from sys.databases",
                "select top (5) s.name from sys.tables t inner join sys.schemas s on t.schema_id = s.schema_id where t.name = N'ssMeni'",
                "select top (40) c.name from sys.columns c where c.object_id = object_id(N'dbo.ssMeni')",
                "select top (800) [Opis], [Parametar] from [dbo].[ssMeni] with (nolock) where [Parametar] like '%netgendll%'"
            };
            for (int i = 0; i < upiti.Length; i++)
            {
                if (!Citanje.JeCitanje(upiti[i]) || upiti[i].IndexOf(';') >= 0)
                    return Qa.P("ssMeni-serveri", "SSMENI-00", "tekst upita je cisto citanje", "Citanje.JeCitanje", upiti[i]);
            }
            return Qa.Z("ssMeni-serveri", "SSMENI-00", "predlosci su SELECT, WITH (NOLOCK), bez EXEC i bez privremene tablice", "Citanje.JeCitanje");
        }

        private static void CitajServer(string server, Cilj[] ciljevi, List<KatalogRed> katalog, List<PristupRed> pristup, List<MeniRed> stavke, List<Stavka> l)
        {
            string greska;
            string spoj;
            string nacin;
            List<string[]> imena = Otvori(Spoj(server, "master", true), Spoj(server, "master", false),
                "select top (" + MaxBaza + ") name, state_desc from sys.databases", 2, MaxBaza, out greska, out spoj, out nacin);
            if (imena == null)
            {
                l.Add(Qa.O("ssMeni-serveri", "SSMENI-" + server, "sys.databases na " + server, "Integrated Security", greska));
                for (int i = 0; i < ciljevi.Length; i++)
                {
                    pristup.Add(new PristupRed
                    {
                        Server = server,
                        Baza = ciljevi[i].Naslov,
                        Kljuc = ciljevi[i].Kljuc,
                        Stanje = "nema-pristupa",
                        Napomena = greska
                    });
                }
                return;
            }

            l.Add(Qa.Z("ssMeni-serveri", "SSMENI-" + server, server + " sys.databases redova " + imena.Count + " (" + nacin + ")", "sys.databases"));
            var vidjene = new List<string>();
            for (int i = 0; i < imena.Count; i++)
            {
                string ime = imena[i].Length > 0 ? (imena[i][0] ?? "") : "";
                string stanje = imena[i].Length > 1 ? (imena[i][1] ?? "") : "";
                if (ime.Length == 0)
                    continue;
                vidjene.Add(ime);
                string norm = Norm(ime);
                Cilj cilj = Najbolji(norm, ciljevi);
                katalog.Add(new KatalogRed
                {
                    Server = server,
                    Baza = ime,
                    Stanje = stanje,
                    Kljuc = cilj == null ? "" : cilj.Kljuc,
                    Upitano = cilj != null && !Sustavna(norm) && Online(stanje)
                });
            }

            for (int c = 0; c < ciljevi.Length; c++)
            {
                var pogodci = new List<KatalogRed>();
                for (int i = 0; i < katalog.Count; i++)
                {
                    if (katalog[i].Server == server && katalog[i].Kljuc == ciljevi[c].Kljuc)
                        pogodci.Add(katalog[i]);
                }
                if (pogodci.Count == 0)
                {
                    string popis = Spoji(vidjene, 12);
                    l.Add(Qa.O("ssMeni-serveri", "SSMENI-" + server + "-" + ciljevi[c].Kljuc, "nema baze za " + ciljevi[c].Naslov, server, popis));
                    pristup.Add(new PristupRed
                    {
                        Server = server,
                        Baza = "(nema podudaranja)",
                        Kljuc = ciljevi[c].Kljuc,
                        Stanje = "nema-baze",
                        Napomena = popis
                    });
                    continue;
                }
                for (int p = 0; p < pogodci.Count; p++)
                {
                    KatalogRed k = pogodci[p];
                    if (Sustavna(Norm(k.Baza)))
                    {
                        l.Add(Qa.O("ssMeni-serveri", IdBaze(server, k.Baza), k.Baza + " je sustavna baza", server, k.Stanje));
                        continue;
                    }
                    if (!Online(k.Stanje))
                    {
                        l.Add(Qa.O("ssMeni-serveri", IdBaze(server, k.Baza), k.Baza + " nije ONLINE", server, k.Stanje));
                        pristup.Add(new PristupRed
                        {
                            Server = server,
                            Baza = k.Baza,
                            Kljuc = k.Kljuc,
                            Stanje = "nije-online",
                            Napomena = k.Stanje
                        });
                        continue;
                    }
                    ProcitajBazu(server, server, k.Baza, k.Kljuc, ciljevi[c].Biovega, pristup, stavke, l);
                }
            }

            if (imena.Count >= MaxBaza)
                l.Add(Qa.O("ssMeni-serveri", "SSMENI-" + server + "-kap", "popis baza skracen na " + MaxBaza, server, "top"));
        }

        private static void CitajSpin(List<PristupRed> pristup, List<MeniRed> stavke, List<Stavka> l)
        {
            SqlConnectionStringBuilder b;
            try
            {
                b = new SqlConnectionStringBuilder(Citanje.Spoj);
            }
            catch (Exception ex)
            {
                l.Add(Qa.O("ssMeni-serveri", "SSMENI-spin", "konekcija spin", "Citanje.Spoj", Skrati(ex.Message)));
                return;
            }
            string baza = string.IsNullOrWhiteSpace(b.InitialCatalog) ? "spin" : b.InitialCatalog;
            string prikaz = PrikazLocal(b.DataSource);
            ProcitajBazu(b.DataSource, prikaz, baza, "spin", false, pristup, stavke, l, true);
        }

        private static void ProcitajBazu(string dataSource, string prikaz, string baza, string kljuc, bool biovega, List<PristupRed> pristup, List<MeniRed> stavke, List<Stavka> l)
        {
            ProcitajBazu(dataSource, prikaz, baza, kljuc, biovega, pristup, stavke, l, false);
        }

        private static void ProcitajBazu(string dataSource, string prikaz, string baza, string kljuc, bool biovega, List<PristupRed> pristup, List<MeniRed> stavke, List<Stavka> l, bool spinAuth)
        {
            var red = new PristupRed { Server = prikaz, Baza = baza, Kljuc = kljuc, Stanje = "greska" };
            pristup.Add(red);
            if (!ImeBaze(baza) || string.IsNullOrWhiteSpace(dataSource))
            {
                red.Napomena = "ime baze nije prihvaceno za katalog veze";
                l.Add(Qa.O("ssMeni-serveri", IdBaze(prikaz, baza), baza + " preskocena", prikaz, red.Napomena));
                return;
            }

            string spoj = null;
            string greska;
            string nacin;
            string sqlShema = "select top (5) s.name from sys.tables t inner join sys.schemas s on t.schema_id = s.schema_id where t.name = N'ssMeni'";
            List<string[]> shema = Izvrsi(dataSource, baza, sqlShema, spinAuth, ref spoj, out greska, out nacin);
            if (shema == null)
            {
                red.Napomena = greska;
                l.Add(Qa.O("ssMeni-serveri", IdBaze(prikaz, baza), prikaz + " " + baza + " ssMeni", "Integrated Security", greska));
                return;
            }
            red.Nacin = nacin;
            if (shema.Count == 0)
            {
                red.Stanje = "nema-ssMeni";
                red.Napomena = "nema tablice ssMeni";
                l.Add(Qa.O("ssMeni-serveri", IdBaze(prikaz, baza), baza + " nema ssMeni", prikaz + "." + baza, "sys.tables"));
                return;
            }

            string schema = "dbo";
            for (int i = 0; i < shema.Count; i++)
            {
                string s = shema[i].Length > 0 ? shema[i][0] : "";
                if (string.Equals(s, "dbo", StringComparison.OrdinalIgnoreCase))
                {
                    schema = "dbo";
                    break;
                }
                if (Ident(s))
                    schema = s;
            }
            if (!Ident(schema))
            {
                red.Napomena = "shema nije obican identifikator";
                l.Add(Qa.O("ssMeni-serveri", IdBaze(prikaz, baza), "shema ssMeni", prikaz, schema));
                return;
            }

            string sqlStupci = "select top (40) c.name from sys.columns c where c.object_id = object_id(N'" + schema + ".ssMeni')";
            List<string[]> stupci = Izvrsi(dataSource, baza, sqlStupci, spinAuth, ref spoj, out greska, out nacin);
            if (stupci == null)
            {
                red.Napomena = greska;
                l.Add(Qa.O("ssMeni-serveri", IdBaze(prikaz, baza), "stupci ssMeni", schema + ".ssMeni", greska));
                return;
            }

            string natpis = null;
            bool imaParametar = false;
            var imenaStupaca = new List<string>();
            for (int i = 0; i < stupci.Count; i++)
            {
                string ime = stupci[i].Length > 0 ? (stupci[i][0] ?? "") : "";
                if (ime.Length == 0)
                    continue;
                imenaStupaca.Add(ime);
                if (string.Equals(ime, "Parametar", StringComparison.OrdinalIgnoreCase))
                    imaParametar = true;
            }
            for (int n = 0; n < NatpisStupci.Length && natpis == null; n++)
            {
                for (int i = 0; i < imenaStupaca.Count; i++)
                {
                    if (string.Equals(imenaStupaca[i], NatpisStupci[n], StringComparison.OrdinalIgnoreCase))
                    {
                        natpis = NatpisStupci[n];
                        break;
                    }
                }
            }
            if (!imaParametar)
            {
                red.Stanje = "nema-parametar";
                red.Napomena = Spoji(imenaStupaca, 15);
                l.Add(Qa.O("ssMeni-serveri", IdBaze(prikaz, baza), baza + " ssMeni nema Parametar", schema + ".ssMeni", red.Napomena));
                return;
            }

            string sql;
            bool dva;
            if (natpis == null)
            {
                dva = false;
                sql = "select top (" + MaxRedova + ") [Parametar] from [" + schema + "].[ssMeni] with (nolock) where [Parametar] like '%netgendll%'";
            }
            else
            {
                dva = true;
                sql = "select top (" + MaxRedova + ") [" + natpis + "], [Parametar] from [" + schema + "].[ssMeni] with (nolock) where [Parametar] like '%netgendll%'";
            }
            List<string[]> redovi = Izvrsi(dataSource, baza, sql, spinAuth, ref spoj, out greska, out nacin);
            if (redovi == null)
            {
                red.Napomena = greska;
                l.Add(Qa.O("ssMeni-serveri", IdBaze(prikaz, baza), "citanje " + schema + ".ssMeni", "WITH (NOLOCK)", greska));
                return;
            }

            int netgen = 0;
            for (int i = 0; i < redovi.Count; i++)
            {
                string opis = dva ? (redovi[i].Length > 0 ? redovi[i][0] : "") : "";
                string parametar = dva ? (redovi[i].Length > 1 ? redovi[i][1] : "") : (redovi[i].Length > 0 ? redovi[i][0] : "");
                if (parametar == null)
                    parametar = "";
                if (parametar.ToLowerInvariant().IndexOf("netgendll", StringComparison.Ordinal) < 0)
                    continue;
                netgen++;
                stavke.Add(new MeniRed
                {
                    Server = prikaz,
                    Baza = baza,
                    Kljuc = kljuc,
                    Biovega = biovega || Norm(baza).Contains("biovega"),
                    Opis = opis ?? "",
                    Parametar = parametar,
                    Fajl = ImeFajla(parametar),
                    PutanjaMeni = PutanjaMeni(parametar)
                });
            }
            red.Stanje = redovi.Count >= MaxRedova ? "skraceno" : "ok";
            red.Redova = netgen;
            red.Napomena = natpis == null ? "nema stupca naziva" : natpis;
            string kap = red.Stanje == "skraceno" ? ", skraceno na " + MaxRedova : "";
            l.Add(Qa.Z("ssMeni-serveri", IdBaze(prikaz, baza), prikaz + "." + baza + " netgendll redova " + netgen + kap, schema + ".ssMeni WITH (NOLOCK)"));
        }

        private static List<string[]> Izvrsi(string dataSource, string baza, string sql, bool spinAuth, ref string spoj, out string greska, out string nacin)
        {
            if (spoj != null)
            {
                nacin = "isti";
                return Redovi(spoj, sql, 4, MaxRedova, out greska);
            }
            string prvi = spinAuth ? SpojSpin(true) : Spoj(dataSource, baza, true);
            string drugi = spinAuth ? SpojSpin(false) : Spoj(dataSource, baza, false);
            return Otvori(prvi, drugi, sql, 4, MaxRedova, out greska, out spoj, out nacin);
        }

        private static List<string[]> Otvori(string spojReadOnly, string spojObicni, string sql, int maxPolja, int maxRedova, out string greska, out string radniSpoj, out string nacin)
        {
            nacin = "readonly";
            SqlConnection conn = OtvoriVezu(spojReadOnly, out greska);
            if (conn == null)
            {
                string prva = greska;
                nacin = "bez-readonly";
                conn = OtvoriVezu(spojObicni, out greska);
                if (conn == null)
                {
                    greska = Skrati((prva ?? "") + " | ponovno: " + (greska ?? ""));
                    radniSpoj = null;
                    return null;
                }
                radniSpoj = spojObicni;
            }
            else
            {
                radniSpoj = spojReadOnly;
            }
            try
            {
                return Citaj(conn, sql, maxPolja, maxRedova, out greska);
            }
            finally
            {
                conn.Dispose();
            }
        }

        /// <summary>
        /// Drugi pokusaj je ista veza bez ApplicationIntent=ReadOnly.
        /// ReadWrite na tom pokusaju samo bira primarni repliku; naredba i dalje mora proci JeCitanje.
        /// </summary>
        private static SqlConnection OtvoriVezu(string spoj, out string greska)
        {
            greska = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(spoj);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                if (conn != null)
                    conn.Dispose();
                greska = Skrati(ex.GetType().Name + ": " + ex.Message);
                return null;
            }
        }

        private static List<string[]> Redovi(string spoj, string sql, int maxPolja, int maxRedova, out string greska)
        {
            greska = null;
            SqlConnection conn = OtvoriVezu(spoj, out greska);
            if (conn == null)
                return null;
            try
            {
                return Citaj(conn, sql, maxPolja, maxRedova, out greska);
            }
            finally
            {
                conn.Dispose();
            }
        }

        private static List<string[]> Citaj(SqlConnection conn, string sql, int maxPolja, int maxRedova, out string greska)
        {
            greska = null;
            if (!Citanje.JeCitanje(sql) || sql.IndexOf(';') >= 0 || sql.IndexOf("--", StringComparison.Ordinal) >= 0)
            {
                greska = "odbio sam naredbu koja nije cisto citanje";
                return null;
            }
            var lista = new List<string[]>();
            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = TimeoutSekundi;
                    cmd.CommandType = CommandType.Text;
                    using (var rd = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        int n = rd.FieldCount;
                        if (n > maxPolja)
                            n = maxPolja;
                        while (rd.Read())
                        {
                            var red = new string[n];
                            for (int i = 0; i < n; i++)
                                red[i] = rd.IsDBNull(i) ? "" : Convert.ToString(rd.GetValue(i));
                            lista.Add(red);
                            if (maxRedova > 0 && lista.Count >= maxRedova)
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                greska = Skrati(ex.GetType().Name + ": " + ex.Message);
                return null;
            }
            return lista;
        }

        private static string Spoj(string server, string baza, bool readOnly)
        {
            if (server != "jupiter2019" && server != "jupiter2022")
                throw new InvalidOperationException("server nije na popisu");
            var b = new SqlConnectionStringBuilder();
            b.DataSource = server;
            b.InitialCatalog = baza;
            b.IntegratedSecurity = true;
            b.ConnectTimeout = TimeoutSekundi;
            b.Pooling = false;
            b.ApplicationName = "CarpoQA-ssMeni-read";
            // ReadWrite je samo drugi pokusaj veze (primarni replika). Naredba ostaje SELECT.
            b.ApplicationIntent = readOnly ? ApplicationIntent.ReadOnly : ApplicationIntent.ReadWrite;
            return b.ConnectionString;
        }

        private static string SpojSpin(bool readOnly)
        {
            var b = new SqlConnectionStringBuilder(Citanje.Spoj);
            b.ConnectTimeout = TimeoutSekundi;
            b.Pooling = false;
            b.ApplicationName = "CarpoQA-ssMeni-read";
            b.ApplicationIntent = readOnly ? ApplicationIntent.ReadOnly : ApplicationIntent.ReadWrite;
            return b.ConnectionString;
        }

        private static List<XmlRed> Agregiraj(List<MeniRed> stavke, Dictionary<string, List<string>> definicije, Dictionary<string, string> checkliste)
        {
            var mapa = new Dictionary<string, XmlRed>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < stavke.Count; i++)
            {
                MeniRed s = stavke[i];
                string kljuc = string.IsNullOrWhiteSpace(s.Fajl) ? "(bez imena)" : s.Fajl;
                XmlRed x;
                if (!mapa.TryGetValue(kljuc, out x))
                {
                    x = new XmlRed();
                    x.Fajl = kljuc;
                    mapa[kljuc] = x;
                    List<string> putanje;
                    if (definicije.TryGetValue(kljuc, out putanje))
                        x.Definicije.AddRange(putanje);
                    string chk;
                    string stem = Path.GetFileNameWithoutExtension(kljuc);
                    if (!string.IsNullOrEmpty(stem) && checkliste.TryGetValue(stem, out chk))
                        x.Checklist = chk;
                }
                string bazaOznaka = s.Baza + " (" + s.Server + ")";
                if (!Sadrzi(x.Baze, bazaOznaka))
                    x.Baze.Add(bazaOznaka);
                if (s.Biovega)
                    x.Biovega = true;
                if (!string.IsNullOrWhiteSpace(s.PutanjaMeni) && !Sadrzi(x.Putanje, s.PutanjaMeni))
                    x.Putanje.Add(s.PutanjaMeni);
                if (!string.IsNullOrWhiteSpace(s.Opis) && !Sadrzi(x.Natpisi, s.Opis))
                    x.Natpisi.Add(s.Opis);
                string low = (s.Parametar ?? "").ToLowerInvariant();
                if (low.Contains("netgendll.loader"))
                    x.Loader = true;
                else if (low.Contains("netgendll.frm"))
                    x.Frm = true;
                else
                    x.Drugo = true;
            }
            var lista = new List<XmlRed>(mapa.Values);
            lista.Sort(UsporediXml);
            return lista;
        }

        private static int UsporediXml(XmlRed a, XmlRed b)
        {
            int c = b.Baze.Count - a.Baze.Count;
            if (c != 0)
                return c;
            c = (b.Biovega ? 1 : 0) - (a.Biovega ? 1 : 0);
            if (c != 0)
                return c;
            return string.Compare(a.Fajl, b.Fajl, StringComparison.OrdinalIgnoreCase);
        }

        private static string Markdown(List<KatalogRed> katalog, List<PristupRed> pristup, List<XmlRed> xml)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# ssMeni NetGen na jupiter2019, jupiter2022 i spin");
            sb.AppendLine();
            sb.AppendLine("Samo citanje. `SELECT` iz `ssMeni` s `WITH (NOLOCK)`, `CommandTimeout` " + TimeoutSekundi + " s, `Connect Timeout` " + TimeoutSekundi + " s, `Pooling=false`, `Application Name=CarpoQA-ssMeni-read`, Integrated Security. Prvi pokusaj veze je `ApplicationIntent=ReadOnly`. Ako veza padne, jedan ponovni pokusaj bez readable secondary, i dalje samo `SELECT`. Nema `EXEC`, `INSERT`, `UPDATE`, `DELETE`, DDL, privremenih tablica, pokretanja granula ni master SQL-a.");
            sb.AppendLine();
            sb.AppendLine("Upit po bazi (shema i stupac naziva se uzimaju iz `sys.tables` / `sys.columns`; naziv je prvi od `Opis`, `Naziv`, `Name`, `Caption`, `Tekst`):");
            sb.AppendLine();
            sb.AppendLine("```sql");
            sb.AppendLine("select top (" + MaxRedova + ") [Opis], [Parametar]");
            sb.AppendLine("from [dbo].[ssMeni] with (nolock)");
            sb.AppendLine("where [Parametar] like '%netgendll%'");
            sb.AppendLine("```");
            sb.AppendLine();
            sb.AppendLine("## Pristup");
            sb.AppendLine();
            sb.AppendLine("| Server | Baza | Kljuc | Stanje | Redova | Nacin | Napomena |");
            sb.AppendLine("|---|---|---|---|---:|---|---|");
            for (int i = 0; i < pristup.Count; i++)
            {
                PristupRed p = pristup[i];
                sb.Append("| ");
                sb.Append(Celija(p.Server));
                sb.Append(" | ");
                sb.Append(Celija(p.Baza));
                sb.Append(" | ");
                sb.Append(Celija(p.Kljuc));
                sb.Append(" | ");
                sb.Append(Celija(p.Stanje));
                sb.Append(" | ");
                sb.Append(p.Redova.ToString());
                sb.Append(" | ");
                sb.Append(Celija(p.Nacin));
                sb.Append(" | ");
                sb.Append(Celija(p.Napomena));
                sb.AppendLine(" |");
            }
            sb.AppendLine();
            sb.AppendLine("## Baze na serveru");
            sb.AppendLine();
            sb.AppendLine("Imena iz `sys.databases`. Upit u `ssMeni` ide samo na bazu cije ime, bez razmaka i crtica, odgovara trazenom kljucu i koja je `ONLINE`. Ostala imena su samo popis.");
            sb.AppendLine();
            sb.AppendLine("| Server | Baza | Stanje | Kljuc | ssMeni upitan |");
            sb.AppendLine("|---|---|---|---|:---:|");
            for (int i = 0; i < katalog.Count; i++)
            {
                KatalogRed k = katalog[i];
                sb.Append("| ");
                sb.Append(Celija(k.Server));
                sb.Append(" | ");
                sb.Append(Celija(k.Baza));
                sb.Append(" | ");
                sb.Append(Celija(k.Stanje));
                sb.Append(" | ");
                sb.Append(Celija(k.Kljuc));
                sb.Append(" | ");
                sb.Append(k.Upitano ? "da" : "");
                sb.AppendLine(" |");
            }
            sb.AppendLine();
            sb.AppendLine("## XML u izborniku");
            sb.AppendLine();
            sb.AppendLine("Jedan red je jedno ime datoteke. Broj baza broji razlicite baze, ne retke izbornika. Biovega je oznacena posebno. Putanja je relativna na korijen repozitorija, samo `definitions/netgen`.");
            sb.AppendLine();
            sb.AppendLine("| XML | Baza | Biovega | Ulaz | Putanja u meniju | Naziv stavke | definitions/netgen | Checklist |");
            sb.AppendLine("|---|---:|:---:|---|---|---|---|---|");
            for (int i = 0; i < xml.Count; i++)
                RedXml(sb, xml[i]);
            sb.AppendLine();
            sb.AppendLine("## Nedostaje XML");
            sb.AppendLine();
            sb.AppendLine("Datoteke kojih nema u `definitions/netgen`, poredane po broju baza. **Biovega** znaci da je datoteka u izborniku te baze.");
            sb.AppendLine();
            sb.AppendLine("| XML | Baza | Biovega | Baze | Naziv stavke | Putanja u meniju |");
            sb.AppendLine("|---|---:|:---:|---|---|---|");
            int nema = 0;
            for (int i = 0; i < xml.Count; i++)
            {
                if (xml[i].Definicije.Count > 0)
                    continue;
                nema++;
                XmlRed x = xml[i];
                sb.Append("| `");
                sb.Append(Celija(x.Fajl));
                sb.Append("` | ");
                sb.Append(x.Baze.Count.ToString());
                sb.Append(" | ");
                sb.Append(x.Biovega ? "**Biovega**" : "");
                sb.Append(" | ");
                sb.Append(Celija(Spoji(x.Baze, 12)));
                sb.Append(" | ");
                sb.Append(Celija(Skrati(Spoji(x.Natpisi, 6), 180)));
                sb.Append(" | ");
                sb.Append(Celija(Spoji(x.Putanje, 6)));
                sb.AppendLine(" |");
            }
            if (nema == 0)
                sb.AppendLine("| — | 0 | | nema procitanih redova ili su svi XML-ovi u repou | | |");
            sb.AppendLine();
            return sb.ToString();
        }

        private static void RedXml(StringBuilder sb, XmlRed x)
        {
            sb.Append("| `");
            sb.Append(Celija(x.Fajl));
            sb.Append("` | ");
            sb.Append(x.Baze.Count.ToString());
            sb.Append(" | ");
            sb.Append(x.Biovega ? "**Biovega**" : "");
            sb.Append(" | ");
            sb.Append(Ulaz(x));
            sb.Append(" | ");
            sb.Append(Celija(Spoji(x.Putanje, 6)));
            sb.Append(" | ");
            sb.Append(Celija(Skrati(Spoji(x.Natpisi, 4), 120)));
            sb.Append(" | ");
            sb.Append(x.Definicije.Count == 0 ? "NEDOSTAJE" : Celija(Spoji(x.Definicije, 4)));
            sb.Append(" | ");
            sb.Append(string.IsNullOrEmpty(x.Checklist) ? "nema" : Celija(x.Checklist));
            sb.AppendLine(" |");
        }

        private static string Tsv(List<KatalogRed> katalog, List<PristupRed> pristup, List<MeniRed> stavke, List<XmlRed> xml)
        {
            var sb = new StringBuilder();
            sb.AppendLine("vrsta\tserver\tbaza\tkljuc\tstanje\topis\tparametar\tfajl\tdefinicija\tchecklist\tbiovega\tbroj_baza\tbaze");
            for (int i = 0; i < katalog.Count; i++)
            {
                KatalogRed k = katalog[i];
                Linija(sb, "KATALOG", k.Server, k.Baza, k.Kljuc, k.Stanje, k.Upitano ? "upitano" : "", "", "", "", "", "", "", "");
            }
            for (int i = 0; i < pristup.Count; i++)
            {
                PristupRed p = pristup[i];
                Linija(sb, "PRISTUP", p.Server, p.Baza, p.Kljuc, p.Stanje, p.Napomena, p.Redova.ToString(), p.Nacin, "", "", "", "", "");
            }
            for (int i = 0; i < stavke.Count; i++)
            {
                MeniRed s = stavke[i];
                Linija(sb, "STAVKA", s.Server, s.Baza, s.Kljuc, "", s.Opis, s.Parametar, s.Fajl, "", "", s.Biovega ? "da" : "", "", "");
            }
            for (int i = 0; i < xml.Count; i++)
            {
                XmlRed x = xml[i];
                Linija(sb, "XML", "", "", "", "", Spoji(x.Natpisi, 8), Spoji(x.Putanje, 8), x.Fajl,
                    x.Definicije.Count == 0 ? "NEDOSTAJE" : Spoji(x.Definicije, 6),
                    x.Checklist, x.Biovega ? "da" : "", x.Baze.Count.ToString(), Spoji(x.Baze, 20));
            }
            return sb.ToString();
        }

        private static void Linija(StringBuilder sb, params string[] polja)
        {
            for (int i = 0; i < polja.Length; i++)
            {
                if (i > 0)
                    sb.Append('\t');
                sb.Append(Polje(polja[i]));
            }
            sb.AppendLine();
        }

        private static string Sazetak(List<PristupRed> pristup, List<XmlRed> xml)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SSMENI n-xml=" + xml.Count);
            for (int i = 0; i < pristup.Count; i++)
            {
                PristupRed p = pristup[i];
                sb.Append("SSMENI-PRISTUP\t");
                sb.Append(Polje(p.Server));
                sb.Append('\t');
                sb.Append(Polje(p.Baza));
                sb.Append('\t');
                sb.Append(Polje(p.Stanje));
                sb.Append('\t');
                sb.Append(p.Redova.ToString());
                sb.Append('\t');
                sb.Append(Polje(p.Napomena));
                sb.AppendLine();
            }
            int nema = 0;
            for (int i = 0; i < xml.Count; i++)
            {
                XmlRed x = xml[i];
                if (x.Definicije.Count == 0)
                    nema++;
                sb.Append("SSMENI-XML\t");
                sb.Append(Polje(x.Fajl));
                sb.Append('\t');
                sb.Append(x.Baze.Count.ToString());
                sb.Append('\t');
                sb.Append(x.Biovega ? "biovega" : "-");
                sb.Append('\t');
                sb.Append(x.Definicije.Count == 0 ? "NEDOSTAJE" : Polje(Spoji(x.Definicije, 4)));
                sb.Append('\t');
                sb.Append(string.IsNullOrEmpty(x.Checklist) ? "-" : Polje(x.Checklist));
                sb.Append('\t');
                sb.Append(Polje(Spoji(x.Baze, 20)));
                sb.AppendLine();
            }
            sb.AppendLine("SSMENI nema-xml=" + nema);
            return sb.ToString();
        }

        private static Dictionary<string, List<string>> IndeksXml(string korijen, string root)
        {
            var d = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists(korijen))
                return d;
            string[] svi = Directory.GetFiles(korijen, "*.xml", SearchOption.AllDirectories);
            for (int i = 0; i < svi.Length; i++)
            {
                string ime = Path.GetFileName(svi[i]);
                string rel = svi[i];
                if (rel.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                    rel = rel.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                rel = rel.Replace('\\', '/');
                List<string> lista;
                if (!d.TryGetValue(ime, out lista))
                {
                    lista = new List<string>();
                    d[ime] = lista;
                }
                if (!Sadrzi(lista, rel))
                    lista.Add(rel);
            }
            return d;
        }

        private static Dictionary<string, string> IndeksMd(string korijen)
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists(korijen))
                return d;
            string[] svi = Directory.GetFiles(korijen, "*.md", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < svi.Length; i++)
            {
                string ime = Path.GetFileNameWithoutExtension(svi[i]);
                if (!d.ContainsKey(ime))
                    d[ime] = "docs/granule/" + Path.GetFileName(svi[i]);
            }
            return d;
        }

        private static Cilj Najbolji(string norm, Cilj[] ciljevi)
        {
            if (Sustavna(norm))
                return null;
            Cilj best = null;
            for (int i = 0; i < ciljevi.Length; i++)
            {
                if (!Odgovara(norm, ciljevi[i].Kljuc))
                    continue;
                if (best == null || ciljevi[i].Kljuc.Length > best.Kljuc.Length)
                    best = ciljevi[i];
            }
            return best;
        }

        private static bool Odgovara(string normBaza, string kljuc)
        {
            if (normBaza.Length == 0 || kljuc.Length == 0)
                return false;
            if (normBaza == kljuc)
                return true;
            if (kljuc.Length >= 6 && normBaza.Contains(kljuc))
                return true;
            if (normBaza.StartsWith(kljuc, StringComparison.Ordinal) && normBaza.Length <= kljuc.Length + 6)
                return true;
            return false;
        }

        private static bool Sustavna(string norm)
        {
            return norm == "master" || norm == "model" || norm == "msdb" || norm == "tempdb"
                || norm == "distribution" || norm == "ssisdb" || norm.StartsWith("reportserver", StringComparison.Ordinal);
        }

        private static bool Online(string stanje)
        {
            return stanje.Trim().Equals("ONLINE", StringComparison.OrdinalIgnoreCase);
        }

        private static string Norm(string s)
        {
            var sb = new StringBuilder();
            string t = (s ?? "").ToLowerInvariant();
            for (int i = 0; i < t.Length; i++)
            {
                char c = t[i];
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static bool Ident(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length > 128)
                return false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;
            }
            return true;
        }

        private static bool ImeBaze(string s)
        {
            if (string.IsNullOrWhiteSpace(s) || s.Length > 128)
                return false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (char.IsLetterOrDigit(c) || c == '_' || c == ' ' || c == '-' || c == '.')
                    continue;
                return false;
            }
            return true;
        }

        private static string ImeFajla(string parametar)
        {
            string rest = PutanjaMeni(parametar);
            int s = Math.Max(rest.LastIndexOf('\\'), rest.LastIndexOf('/'));
            if (s >= 0)
                rest = rest.Substring(s + 1);
            return rest.Trim();
        }

        private static string PutanjaMeni(string parametar)
        {
            if (string.IsNullOrEmpty(parametar))
                return "";
            int c = parametar.LastIndexOf(':');
            string rest = c >= 0 ? parametar.Substring(c + 1) : parametar;
            return rest.Trim();
        }

        private static string PrikazLocal(string dataSource)
        {
            string n = Norm(dataSource ?? "");
            if (n.Length == 0 || n == "localhost" || n == "local" || n.StartsWith("localdb", StringComparison.Ordinal) || n == ".")
                return "sql-ai";
            return dataSource;
        }

        private static string Ulaz(XmlRed x)
        {
            if (x.Loader && x.Frm)
                return "loader+frm";
            if (x.Loader)
                return "loader";
            if (x.Frm)
                return "frm";
            if (x.Drugo)
                return "drugo";
            return "";
        }

        private static string IdBaze(string server, string baza)
        {
            string s = Norm(server + "-" + baza);
            if (s.Length > 40)
                s = s.Substring(0, 40);
            return "SSMENI-" + s;
        }

        private static bool Sadrzi(List<string> lista, string vrijednost)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                if (string.Equals(lista[i], vrijednost, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static string Spoji(List<string> lista, int max)
        {
            var sb = new StringBuilder();
            int n = lista.Count < max ? lista.Count : max;
            for (int i = 0; i < n; i++)
            {
                if (i > 0)
                    sb.Append("; ");
                sb.Append(lista[i]);
            }
            if (lista.Count > max)
                sb.Append("; …+" + (lista.Count - max));
            return sb.ToString();
        }

        private static string Celija(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            return s.Replace("|", "/").Replace("\r", " ").Replace("\n", " ").Trim();
        }

        private static string Polje(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            return s.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Trim();
        }

        private static string Skrati(string s)
        {
            return Skrati(s, 240);
        }

        private static string Skrati(string s, int max)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            s = s.Replace("\r", " ").Replace("\n", " ").Trim();
            if (s.Length <= max)
                return s;
            return s.Substring(0, max);
        }

        private sealed class Cilj
        {
            public readonly string Kljuc;
            public readonly string Naslov;
            public readonly bool Biovega;

            public Cilj(string kljuc, string naslov, bool biovega)
            {
                Kljuc = kljuc;
                Naslov = naslov;
                Biovega = biovega;
            }
        }

        private sealed class KatalogRed
        {
            public string Server;
            public string Baza;
            public string Stanje;
            public string Kljuc;
            public bool Upitano;
        }

        private sealed class PristupRed
        {
            public string Server;
            public string Baza;
            public string Kljuc;
            public string Stanje;
            public string Nacin;
            public string Napomena;
            public int Redova;
        }

        private sealed class MeniRed
        {
            public string Server;
            public string Baza;
            public string Kljuc;
            public bool Biovega;
            public string Opis;
            public string Parametar;
            public string Fajl;
            public string PutanjaMeni;
        }

        private sealed class XmlRed
        {
            public string Fajl;
            public bool Biovega;
            public bool Loader;
            public bool Frm;
            public bool Drugo;
            public string Checklist;
            public readonly List<string> Baze = new List<string>();
            public readonly List<string> Putanje = new List<string>();
            public readonly List<string> Natpisi = new List<string>();
            public readonly List<string> Definicije = new List<string>();
        }
    }
}

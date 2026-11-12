using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Read-only smoke: svaki asm:NetGenDLL.loader / frm iz ssMeni. Ne piše u spin.
    /// </summary>
    internal static class SmokeMeni
    {
        public static int Pokreni(string root, string izlaz)
        {
            string greska;
            List<string> parametri = Lista(out greska);
            if (parametri == null)
            {
                Console.Error.WriteLine("ssMeni: " + greska);
                return 1;
            }
            var indeks = Indeks(Path.Combine(root, "definitions", "netgen"));
            var redovi = new List<Red>();
            var vidjeni = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < parametri.Count; i++)
            {
                string p = (parametri[i] ?? "").Trim();
                if (p.Length == 0 || !vidjeni.Add(p))
                    continue;
                redovi.Add(Otvori(root, indeks, p));
            }
            string tekst = Izvjestaj(redovi);
            Console.WriteLine(tekst);
            if (!string.IsNullOrWhiteSpace(izlaz))
            {
                string dir = Path.GetDirectoryName(izlaz);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(izlaz, tekst, new UTF8Encoding(false));
            }
            return 0;
        }

        private static List<string> Lista(out string greska)
        {
            var lista = new List<string>();
            greska = null;
            try
            {
                using (var conn = new SqlConnection(Citanje.Spoj))
                using (var cmd = new SqlCommand(Upit(), conn))
                {
                    cmd.CommandTimeout = 30;
                    conn.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                            lista.Add(rd.IsDBNull(0) ? "" : Convert.ToString(rd.GetValue(0)));
                    }
                }
            }
            catch (Exception ex)
            {
                greska = ex.GetType().Name + ": " + ex.Message;
                return null;
            }
            return lista;
        }

        private static string Upit()
        {
            return "select distinct Parametar from ssMeni where Parametar like 'asm:NetGenDLL.loader%' or Parametar like 'asm:NetGenDLL.frm%' order by Parametar";
        }

        private static Red Otvori(string root, Dictionary<string, List<string>> indeks, string parametar)
        {
            var r = new Red();
            r.Parametar = parametar;
            string put = PutXml(parametar);
            r.TrazeniPut = put;
            if (put.Length == 0)
            {
                r.Uzrok = "nema-puta";
                r.Greska = "Parametar nema putanju iza druge dvotocke";
                return r;
            }
            string fajl = Nadji(indeks, put);
            r.Xml = fajl;
            if (fajl == null)
            {
                r.Uzrok = "nema-xml";
                r.Greska = "nema datoteke za " + put;
                return r;
            }
            var ucitano = NetGenXmlReader.Ucitaj(fajl);
            if (!ucitano.Uspjeh || ucitano.Projekt == null)
            {
                r.Uzrok = "xml";
                r.Greska = ucitano.Greska ?? "XML";
                return r;
            }
            var g = new MasterGrid();
            g.Kontekst.OperaterId = "7";
            g.Ucitaj(ucitano.Projekt, null);
            g.Stanje.TimeoutSekundi = 12;
            if (g.Stanje.Odluka == "salji" && !g.Stanje.BlokiranoPisanje)
                g.IzvrsiCitanje(Citanje.Spoj);
            r.Odluka = g.Stanje.Odluka ?? "";
            if (!string.IsNullOrEmpty(g.Stanje.SqlGreska))
            {
                r.Uzrok = Uzrok(g.Stanje.SqlGreska);
                r.Greska = Skrati(g.Stanje.Poruka ?? g.Stanje.SqlGreska);
                r.Sql = Skrati(g.Stanje.SqlZaSlanje);
                return r;
            }
            if (g.Stanje.BlokiranoPisanje)
            {
                r.Uzrok = "upis-blokiran";
                r.Greska = Skrati(g.Stanje.IzvorSql);
                return r;
            }
            if (g.Stanje.OtvaraFormu)
            {
                r.Uzrok = "forma";
                r.Greska = "ONLOAD je forma " + (g.Stanje.ImeForme ?? "");
                return r;
            }
            if (g.Stanje.Odluka != "salji")
            {
                r.Uzrok = "nema-sql";
                r.Greska = g.Stanje.Odluka ?? "prazno";
                r.Otvoreno = true;
                return r;
            }
            r.Otvoreno = true;
            r.Redova = g.Stanje.Tablica == null ? 0 : g.Stanje.Tablica.Rows.Count;
            r.Uzrok = "otvoreno";
            if (!string.IsNullOrWhiteSpace(ucitano.Projekt.Tree))
            {
                var st = new Carpo.NetGen.Grid.Stablo();
                st.Kontekst.OperaterId = "7";
                st.ImeXml = Path.GetFileName(fajl);
                st.Ucitaj(ucitano.Projekt, delegate (string sql) { return Citaj(sql); });
                r.Korijena = st.Stanje.Korijen.Count;
            }
            return r;
        }

        private static DataTable Citaj(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql) || SqlTekst.JeZabranaPisanja(sql))
                return null;
            try
            {
                using (var conn = new SqlConnection(Citanje.Spoj))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = 12;
                    conn.Open();
                    var dt = new DataTable();
                    using (var rd = cmd.ExecuteReader())
                        dt.Load(rd);
                    return dt;
                }
            }
            catch (Exception)
            {
                return null;
            }
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

        private static string Nadji(Dictionary<string, List<string>> indeks, string put)
        {
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
            return lista[0];
        }

        private static string Uzrok(string greska)
        {
            string g = (greska ?? "").ToLowerInvariant();
            if (g.IndexOf("could not find stored procedure", StringComparison.Ordinal) >= 0)
                return "nema-procedure";
            if (g.IndexOf("invalid column name", StringComparison.Ordinal) >= 0)
                return "krivi-stupac";
            if (g.IndexOf("invalid object name", StringComparison.Ordinal) >= 0)
                return "nema-objekta";
            if (g.IndexOf("timeout", StringComparison.Ordinal) >= 0)
                return "timeout";
            if (g.IndexOf("incorrect syntax", StringComparison.Ordinal) >= 0)
                return "sintaksa";
            if (g.IndexOf("permission", StringComparison.Ordinal) >= 0 || g.IndexOf("denied", StringComparison.Ordinal) >= 0)
                return "dozvola";
            if (g.IndexOf("conversion failed", StringComparison.Ordinal) >= 0 || g.IndexOf("converting", StringComparison.Ordinal) >= 0)
                return "pretvorba";
            if (g.IndexOf("snix", StringComparison.Ordinal) >= 0 || g.IndexOf("network", StringComparison.Ordinal) >= 0)
                return "veza";
            return "ostalo";
        }

        private static string Skrati(string s)
        {
            if (s == null)
                return "";
            s = s.Replace("\r", " ").Replace("\n", " ").Trim();
            if (s.Length > 180)
                return s.Substring(0, 180);
            return s;
        }

        private static string Izvjestaj(List<Red> redovi)
        {
            var po = new Dictionary<string, int>(StringComparer.Ordinal);
            int otvoreno = 0;
            for (int i = 0; i < redovi.Count; i++)
            {
                if (redovi[i].Otvoreno)
                    otvoreno++;
                int n;
                po.TryGetValue(redovi[i].Uzrok, out n);
                po[redovi[i].Uzrok] = n + 1;
            }
            var kljucevi = new List<string>(po.Keys);
            kljucevi.Sort(delegate (string a, string b) { return po[b].CompareTo(po[a]); });
            var sb = new StringBuilder();
            sb.Append("# Smoke spin: ssMeni NetGenDLL\n\n");
            sb.Append("Samo citanje. Upit je `asm:NetGenDLL.loader` i `asm:NetGenDLL.frm`. Operater u kuki je 7. Timeout naredbe je 12 s.\n\n");
            sb.Append("Ukupno ").Append(redovi.Count).Append(", otvoreno ").Append(otvoreno).Append(".\n\n");
            sb.Append("## Uzroci\n\n");
            sb.Append("| Uzrok | Broj |\n|---|---|\n");
            for (int i = 0; i < kljucevi.Count; i++)
                sb.Append("| ").Append(kljucevi[i]).Append(" | ").Append(po[kljucevi[i]]).Append(" |\n");
            sb.Append("\n## Stavke\n\n");
            sb.Append("| XML | Redova | Korijena | Uzrok | Poruka |\n|---|---:|---:|---|---|\n");
            for (int i = 0; i < redovi.Count; i++)
            {
                Red r = redovi[i];
                string ime = r.Xml == null ? r.TrazeniPut : Path.GetFileName(r.Xml);
                sb.Append("| ").Append(Celija(ime)).Append(" | ");
                sb.Append(r.Otvoreno ? r.Redova.ToString() : "").Append(" | ");
                sb.Append(r.Korijena < 0 ? "" : r.Korijena.ToString()).Append(" | ");
                sb.Append(r.Uzrok).Append(" | ").Append(Celija(r.Greska)).Append(" |\n");
            }
            return sb.ToString();
        }

        private static string Celija(string s)
        {
            return (s ?? "").Replace("|", "/");
        }

        private sealed class Red
        {
            public string Parametar = "";
            public string TrazeniPut = "";
            public string Xml;
            public string Odluka = "";
            public string Uzrok = "";
            public string Greska = "";
            public string Sql = "";
            public bool Otvoreno;
            public int Redova;
            public int Korijena = -1;
        }
    }
}

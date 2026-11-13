using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Print;

namespace Carpo.NetGen.Ulaz
{
    /// <summary>
    /// PokaziUpit, dozvola gumba, custom format, raspored i DMS.
    /// Form1.vb 1825–2180, 2714–3026, 7451–7646, 6192–6431, 8484–8665.
    /// </summary>
    public static class Form1Srce
    {
        public sealed class Upit
        {
            public bool PrazanIzlaz;
            public bool PunimMaster = true;
            public string ZadnjiSql = "";
            public string Id = "0";
            public bool Dx;
            public string Veza = "";
            public bool UcitajRaspored;
            public bool Kvacica = true;
            public bool Total = true;
            public bool Detalj;
            public bool DetaljJeForma;
            public string Poruka = "";
            public bool Timer;
            public int Splitter = -1;
            public bool ZadnjiRed;
            public bool Filter;
            public bool DozvoliUredivanje = true;
            public string SelfRoditelj = "";
            public string SelfDijete = "";
            public readonly List<int> Grupe = new List<int>();
            public readonly List<string> DodatniSql = new List<string>();
        }

        /// <summary>Form1.vb 1825–2180. Prazan SQL izlazi prije zadnjeg SQL-a. DX ne pali timer.</summary>
        public static Upit Pokazi(string sql, string trenutniId, string idNovi, bool dx, string relacija, bool resetRasporeda, string kvacica, string total, string detalj, int visina, bool zadnjiRed, string grupe, string filter, string stupciUredivanja, string sql2, string sql3, string sql4)
        {
            var u = new Upit();
            u.Dx = dx;
            if ((sql ?? "").Trim().Length == 0)
            {
                u.PrazanIzlaz = true;
                return u;
            }
            u.ZadnjiSql = sql;
            string id = trenutniId ?? "";
            if (id != (idNovi ?? "") && (idNovi ?? "").Length > 0 && idNovi != "0")
                id = idNovi;
            u.Id = id;
            string rel = (relacija ?? "").Trim().ToLowerInvariant();
            u.Veza = rel;
            if (rel == "self")
            {
                u.SelfRoditelj = "id";
                u.SelfDijete = "parentid";
            }
            else if (rel == "multi")
            {
                if ((sql2 ?? "").Trim().Length > 0)
                    u.DodatniSql.Add(sql2);
                if ((sql3 ?? "").Trim().Length > 0)
                    u.DodatniSql.Add(sql3);
                if ((sql4 ?? "").Trim().Length > 0)
                    u.DodatniSql.Add(sql4);
            }
            string det = detalj ?? "";
            if (det.Trim().Length > 0)
            {
                if (det.Trim().Substring(0, 1) == ":")
                {
                    u.DetaljJeForma = true;
                    u.Poruka = "sorkac!";
                }
                else
                    u.Detalj = true;
            }
            else
                u.Splitter = visina - 50;
            u.Kvacica = (kvacica ?? "") != "FALSE";
            if (dx)
            {
                u.PunimMaster = false;
                return u;
            }
            u.UcitajRaspored = !resetRasporeda;
            u.Total = (total ?? "") != "FALSE";
            u.ZadnjiRed = zadnjiRed;
            u.Timer = true;
            u.PunimMaster = false;
            if ((grupe ?? "").Trim().Length > 0)
            {
                string[] ar = grupe.Split('~');
                for (int i = 0; i < ar.Length; i++)
                {
                    if (ar[i].Trim().Length == 0)
                        continue;
                    int n;
                    if (int.TryParse(ar[i].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n))
                        u.Grupe.Add(n + 2);
                }
            }
            if ((filter ?? "").Trim().ToLowerInvariant() == "true")
            {
                u.Filter = true;
                u.DozvoliUredivanje = (stupciUredivanja ?? "").Length > 0;
            }
            return u;
        }

        /// <summary>Form1.vb 2270–2276. Dok se puni master ili ručno kvači, detail ne ide.</summary>
        public static string Detalj(bool punimMaster, bool rucno, string sql, int visina)
        {
            if (punimMaster || rucno)
                return "";
            if ((sql ?? "").Trim().Length == 0)
                return "splitter=" + (visina - 50).ToString(CultureInfo.InvariantCulture);
            return "sql";
        }

        public sealed class GumbStanje
        {
            public bool Ukljucen = true;
            public bool Vidljiv = true;
            public bool Kvaka;
        }

        /// <summary>Form1.vb 2742–3018. True na smije se ne gasi. Gumb 3 i 8 imaju drukčiju vidljivost.</summary>
        public static void Dozvola(int gumb, GumbStanje g, bool? smije, bool? vidi)
        {
            if (g == null)
                return;
            if (smije == false && !g.Kvaka)
                g.Ukljucen = false;
            if (smije == true)
            {
                g.Ukljucen = true;
                g.Kvaka = true;
            }
            if (vidi == false)
            {
                if (gumb == 3 || gumb == 8 || !g.Kvaka)
                    g.Vidljiv = false;
            }
            if (vidi == true)
            {
                if (gumb == 8)
                {
                    if (!g.Kvaka)
                        g.Vidljiv = false;
                }
                else if (gumb == 3)
                {
                    if (!g.Kvaka)
                        g.Vidljiv = false;
                    g.Vidljiv = true;
                }
                else
                    g.Vidljiv = true;
            }
        }

        public sealed class Format
        {
            public string Kljuc = "";
            public int Sirina;
            public string Natpis = "";
            public string FormatTekst = "";
            public string Vidljiva = "";
            public string Boja = "";
            public string Pozadina = "";
            public int MaxLinija;
            public bool Prelomi;
            public string Agregat = "";
            public bool NegativnoCrveno;
            public bool NulaNijeCrveno;
            public string Pozicija = "";
            public bool Auto;
            public string Zaglavlje = "";
            public bool SakrijSve;
        }

        /// <summary>Form1.vb 7459–7535. WIDTH ide kroz 14. Točka-zarez u natpisu je novi red. Agregat je AGREGATEFUNCTION.</summary>
        public static Format Kolona(int index, string ime, int sirina, string natpis, string format, string vidljiva, string boja, string pozadina, int maxLinija, string agregat, bool negativno, bool nijeNula, string pozicija, bool auto, bool clear, string zaglavlje)
        {
            var f = new Format();
            f.SakrijSve = clear;
            if (index == 0 && (ime ?? "").Length == 0)
                return f;
            f.Kljuc = index == 0 ? ime : index.ToString(CultureInfo.InvariantCulture);
            if (sirina > 0)
                f.Sirina = (int)Math.Round(sirina / 14.0, MidpointRounding.ToEven);
            f.Natpis = (natpis ?? "").Replace(";", "\r\n");
            f.FormatTekst = format ?? "";
            f.Vidljiva = vidljiva ?? "";
            f.Boja = (boja ?? "").Trim().ToLowerInvariant();
            f.Pozadina = (pozadina ?? "").Trim().ToLowerInvariant();
            if (maxLinija > 0)
            {
                f.MaxLinija = maxLinija;
                f.Prelomi = true;
            }
            f.Agregat = agregat ?? "";
            f.NegativnoCrveno = negativno;
            f.NulaNijeCrveno = nijeNula;
            f.Pozicija = pozicija ?? "";
            f.Auto = auto;
            f.Zaglavlje = (zaglavlje ?? "").Trim();
            return f;
        }

        /// <summary>Form1.vb 6246–6372. DX piše xml. Janus piše gxl. Brisanje dira samo tu vrstu.</summary>
        public static string Datoteka(string mapa, string ime, string jezik, bool dx, bool detalj)
        {
            string prefiks = detalj ? "GridDet" : "GridMaster";
            string ext = !detalj && dx ? ".xml" : ".gxl";
            string korijen = mapa ?? "";
            if (!korijen.EndsWith("\\", StringComparison.Ordinal))
                korijen += "\\";
            return korijen + "\\" + prefiks + (ime ?? "") + "-" + (jezik ?? "") + ext;
        }

        public static void Spremi(string put, string xml)
        {
            string dir = Path.GetDirectoryName(put);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(put, xml ?? "");
        }

        public static string Ucitaj(string put, bool reset)
        {
            if (reset || string.IsNullOrEmpty(put) || !File.Exists(put))
                return "";
            return File.ReadAllText(put);
        }

        public static bool Obrisi(string put)
        {
            if (string.IsNullOrEmpty(put) || !File.Exists(put))
                return false;
            File.Delete(put);
            return true;
        }

        public const string DmsSklop = "DmsManagerV1";
        public const string DmsDokumenti = "DmsManager.Db.Documents";
        public const string DmsStart = "DmsManagerV1.Start";

        /// <summary>Form1.vb 8488–8490. Nema sklopa: log i izlaz. DLL se ne commitaj.</summary>
        public static string UcitajDms()
        {
            try
            {
                Assembly.Load(DmsSklop);
                return "";
            }
            catch (Exception ex)
            {
                return "Greška kod Assembly.Load DMSScan : " + ex.Message;
            }
        }

        public static string SqlDmsBrisi(string entitet, string dok)
        {
            return "spNetGenDMSAzur @action='delete',@EntityName='" + (entitet ?? "").Trim() + "',@selectedids ='" + (dok ?? "").Trim() + "'";
        }

        /// <summary>Form1.vb 7773. ISEMPTY, pa SrediSQL.</summary>
        public static string Rafalna(string tekst, SqlKontekst kontekst)
        {
            return SqlTekst.Sredi(IspisMotora.Zaglavlje(tekst), kontekst);
        }

        public sealed class Pogled
        {
            public string Kontrola = "";
            public bool Ikona;
            public bool Navigator;
            public bool ProsiriKartice;
            public string NoviRed = "";
        }

        /// <summary>Form1.vb 6161–6184. TableView je GridView, CardView je CardView, SingleCard je LayoutView.</summary>
        public static Pogled PogledIz(string janus)
        {
            var p = new Pogled();
            string u = (janus ?? "").Trim();
            if (string.Equals(u, "CardView", StringComparison.OrdinalIgnoreCase))
            {
                p.Kontrola = "CardView";
                p.Ikona = false;
                p.ProsiriKartice = true;
                p.Navigator = false;
                return p;
            }
            if (string.Equals(u, "SingleCard", StringComparison.OrdinalIgnoreCase))
            {
                p.Kontrola = "LayoutView";
                p.Ikona = false;
                p.ProsiriKartice = false;
                p.Navigator = true;
                p.NoviRed = "Bottom";
                return p;
            }
            p.Kontrola = "GridView";
            p.Ikona = true;
            p.Navigator = false;
            p.NoviRed = "Top";
            return p;
        }

        public static bool DxPreskaceJanus(bool dx)
        {
            return dx;
        }

        public static bool DesniKlik(string gumb)
        {
            return string.Equals(gumb, "Right", StringComparison.OrdinalIgnoreCase);
        }

        public static string EnterForma(string tipka, string onEnter)
        {
            if (string.Equals(tipka, "Enter", StringComparison.OrdinalIgnoreCase) && (onEnter ?? "").Trim().Length > 0)
                return onEnter.Trim();
            return "";
        }

        public static bool KontrolnaTipka(int kod)
        {
            return kod == 17;
        }

        public static bool KvacicaNakonZapisa(bool prijevod)
        {
            return !prijevod;
        }

        public static bool ZatvaranjeOtkazano(bool neZatvaraj)
        {
            return neZatvaraj;
        }

        public static bool StupacNakonKlika(bool vidljiv)
        {
            return !vidljiv;
        }

        public static string AplikacijaIzTaga(string tag)
        {
            if ((tag ?? "").Trim().Length == 0)
                return "";
            return tag;
        }

        public static bool TimerKlik(bool upaljen)
        {
            return !upaljen;
        }

        public static string BojaPrijeOdabira()
        {
            return "White";
        }

        public static string BojaBezFokusa()
        {
            return "LightGreen";
        }

        public static int StabloSirina(bool vidljivo, int spremljeno)
        {
            return vidljivo ? 1 : spremljeno;
        }

        public static bool ResizePaliTimer(bool maksimiziran)
        {
            return maksimiziran;
        }

        public static int[] Roditelj(int sirina, int visina)
        {
            return new[] { 0, 0, sirina, visina };
        }

        public static string IzvozIme(bool master)
        {
            return master ? "exportmaster.xls" : "export.xls";
        }

        public static string RedoslijedUcitavanja()
        {
            return "ulaz,spoj,operater,drvo,jezik,xml,ngparams";
        }

        public static string SpojiParametre(string master, string detalj)
        {
            if ((master ?? "").Length == 0)
                return "";
            return master + (detalj ?? "");
        }

        public static int TreciNivo(bool roditeljImaParent)
        {
            return roditeljImaParent ? 1 : 0;
        }

        public static bool SearchJeStupac(bool noEdit)
        {
            return noEdit;
        }

        public static bool GrupeSeSklope()
        {
            return true;
        }

        /// <summary>Form1.vb 7421. Prazan centar podnožja dobije broj stranice.</summary>
        public static string Podnozje(string centar, int stranica, int ukupno)
        {
            if ((centar ?? "").Trim().Length > 0)
                return centar;
            return "Stranica " + stranica.ToString(CultureInfo.InvariantCulture) + " / " + ukupno.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Form1.vb 7899–7924. Ikona ako ima ključ. Boje samo ako oba stupca pročitaju.</summary>
        public static string[] Red(string ikona, bool ikonaGreska, string font, string pozadina, bool bojaGreska)
        {
            string slika = !ikonaGreska && (ikona ?? "").Trim().Length > 0 ? ikona.Trim() : "";
            if (bojaGreska)
                return new[] { slika, "", "" };
            return new[] { slika, font ?? "", pozadina ?? "" };
        }

        public static string TekstPovlacenja(string tekst)
        {
            return tekst ?? "";
        }

        public static string NoviProzor()
        {
            return "store-all,dock-fill,row-headers-off";
        }

        public static string DmsMetoda(string akcija)
        {
            if (akcija == "checkout")
                return "DocumentCheckOut";
            if (akcija == "checkin")
                return "DocumentCheckIn";
            if (akcija == "spremi")
                return "SaveDocuments";
            if (akcija == "pogledaj")
                return "OpenDocument";
            return "";
        }

        public static string DmsKlasa(string akcija)
        {
            if (akcija == "spremi" || akcija == "pogledaj")
                return DmsStart;
            return DmsDokumenti;
        }
    }
}

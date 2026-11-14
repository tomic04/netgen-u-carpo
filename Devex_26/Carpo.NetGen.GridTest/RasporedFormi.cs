using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;
using Carpo.NetGen.Ui;
using Carpo.NetGen.Xml;
using DevExpress.XtraEditors;

namespace Carpo.NetGen.GridTest
{
    /// <summary>
    /// RAS-01: složi svaku formu svake granule (bez baze, bez prikaza) i traži greške izgleda koje funkcionalni
    /// testovi ne vide: kontrolu iz XML-a koja nije nastala, kontrolu koju prekriva neprozirna susjedna kontrola
    /// (kao okvir preko polja u RobaWebKlase, LAY-02), kontrolu bez veličine i kontrolu izvan prozora.
    /// Forma koja baci iznimku pri slaganju je IZNIMKA. Izlaz je TSV i sažetak na konzoli.
    /// </summary>
    internal static class RasporedFormi
    {
        private static readonly MethodInfo GetState = typeof(Control).GetMethod("GetState", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(int) }, null);

        /// <summary>Control.Visible je false dok forma nije prikazana; STATE_VISIBLE (0x2) je ono što je kod postavio.</summary>
        private static bool Vidljiva(Control c)
        {
            if (GetState == null)
                return c.Visible;
            try { return (bool)GetState.Invoke(c, new object[] { 2 }); }
            catch { return c.Visible; }
        }

        private static bool VidljivaDoForme(Control c, Control forma)
        {
            for (Control x = c; x != null && x != forma; x = x.Parent)
            {
                if (!Vidljiva(x))
                    return false;
            }
            return true;
        }

        private static bool Neprozirna(Control c)
        {
            return !(c is LabelControl) && !(c is Label);
        }

        public static int Provjeri(string korijen, string izvjestaj)
        {
            NgPoruka.Tiho = true;
            var sat = Stopwatch.StartNew();
            var tsv = new StringBuilder();
            tsv.AppendLine("granula\tforma\tvrsta\tkontrola\tdetalj");
            int granula = 0, formi = 0, iznimka = 0, prekriveno = 0, formiPrekriveno = 0, preklapanje = 0, nema = 0, bezVelicine = 0, izvan = 0, ucitanje = 0;
            var nemaPoGrani = new SortedDictionary<string, int>();
            var primjeri = new List<string>();
            var primjeriOstalo = new Dictionary<string, List<string>>();
            var putevi = Directory.GetFiles(korijen, "*.xml", SearchOption.AllDirectories);
            Array.Sort(putevi, StringComparer.OrdinalIgnoreCase);
            foreach (string put in putevi)
            {
                string rel = put.Substring(korijen.Length).TrimStart('\\', '/');
                NgLoadResult r;
                try { r = NetGenXmlReader.Ucitaj(put); }
                catch (Exception ex)
                {
                    ucitanje++;
                    tsv.AppendLine(rel + "\t\tUCITANJE\t\t" + Jedan(ex.Message));
                    continue;
                }
                if (r == null || !r.Uspjeh || r.Projekt == null)
                    continue;
                granula++;
                foreach (NgForm forma in r.Projekt.Forme)
                {
                    if (forma == null || forma.Kontrole == null || forma.Kontrole.Count == 0)
                        continue;
                    formi++;
                    var k = new SqlKontekst();
                    k.XmlName = Path.GetFileName(put);
                    k.XmlPut = put;
                    Dijalog d = null;
                    try
                    {
                        d = new Dijalog();
                        d.Slozi(forma, k, delegate (string sql) { return new DataTable(); });
                        d.PerformLayout();
                        bool imaPrekriveno = false;
                        var vidjeno = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        foreach (NgControl nk in forma.Kontrole)
                        {
                            if (string.IsNullOrEmpty(nk.Name) || !vidjeno.Add(nk.Name))
                                continue;
                            Control[] nadjeno = d.Controls.Find(nk.Name, true);
                            if (nadjeno.Length == 0)
                            {
                                nema++;
                                string g = nk.Grana.ToString();
                                int n;
                                nemaPoGrani.TryGetValue(g, out n);
                                nemaPoGrani[g] = n + 1;
                                tsv.AppendLine(rel + "\t" + forma.Ime + "\tNEMA\t" + nk.Name + "\t" + nk.TypeRaw);
                                Primjer(primjeriOstalo, "NEMA", rel + "\t" + forma.Ime + "\t" + nk.Name + "\t" + nk.TypeRaw);
                                continue;
                            }
                            Control c = nadjeno[0];
                            if (!VidljivaDoForme(c, d))
                                continue;
                            // Natpis bez teksta ima širinu 0 dok ga ONLOAD/SETTEXT ne napuni; to nije greška.
                            if (c.Dock == DockStyle.None && !(c is LabelControl) && (c.Width <= 0 || c.Height <= 0))
                            {
                                bezVelicine++;
                                tsv.AppendLine(rel + "\t" + forma.Ime + "\tBEZ-VELICINE\t" + c.Name + "\t" + c.Width + "x" + c.Height);
                                Primjer(primjeriOstalo, "BEZ-VELICINE", rel + "\t" + forma.Ime + "\t" + c.Name + " (" + nk.TypeRaw + ")\t" + c.Width + "x" + c.Height);
                                continue;
                            }
                            // Okvir i panel su pozadina; njihovo središte smiju pokriti kontrole (VB ih šalje straga).
                            bool okvir = c is PanelControl;
                            Control pokriva = okvir ? null : Prekriva(c, d);
                            if (pokriva != null && pokriva is PanelControl)
                            {
                                // Isto što i greška u RobaWebKlase (LAY-02): okvir ili panel ispred polja. VB to nikad ne radi.
                                prekriveno++;
                                imaPrekriveno = true;
                                string red = rel + "\t" + forma.Ime + "\tPREKRIVENO-OKVIROM\t" + c.Name + "\t" + pokriva.Name + " (" + pokriva.GetType().Name + ")";
                                tsv.AppendLine(red);
                                if (primjeri.Count < 40)
                                    primjeri.Add(red);
                            }
                            else if (pokriva != null)
                            {
                                // Dvije kontrole na istom mjestu: kasnija je ispred (kao NetGen), npr. polja koja se izmjenjuju.
                                preklapanje++;
                                string red = rel + "\t" + forma.Ime + "\tPREKLAPANJE\t" + c.Name + "\t" + pokriva.Name + " (" + pokriva.GetType().Name + ")";
                                tsv.AppendLine(red);
                                Primjer(primjeriOstalo, "PREKLAPANJE", red);
                            }
                            Control p = c.Parent;
                            if (p != null && c.Dock == DockStyle.None && (c.Left >= p.ClientSize.Width || c.Top >= p.ClientSize.Height || c.Right <= 0 || c.Bottom <= 0))
                            {
                                izvan++;
                                tsv.AppendLine(rel + "\t" + forma.Ime + "\tIZVAN\t" + c.Name + "\t" + c.Bounds + " u " + p.Name + " " + p.ClientSize);
                                Primjer(primjeriOstalo, "IZVAN", rel + "\t" + forma.Ime + "\t" + c.Name + " (" + nk.TypeRaw + ")\t" + c.Bounds + " u " + (p == d ? "formi" : p.Name) + " " + p.ClientSize);
                            }
                        }
                        if (imaPrekriveno)
                            formiPrekriveno++;
                    }
                    catch (Exception ex)
                    {
                        iznimka++;
                        string red = rel + "\t" + forma.Ime + "\tIZNIMKA\t\t" + ex.GetType().Name + ": " + Jedan(ex.Message);
                        tsv.AppendLine(red);
                        if (primjeri.Count < 40)
                            primjeri.Add(red);
                    }
                    finally
                    {
                        if (d != null)
                        {
                            try { d.Dispose(); } catch { }
                        }
                    }
                }
            }
            sat.Stop();
            var glava = new StringBuilder();
            glava.AppendLine("# Raspored formi (RAS-01)");
            glava.AppendLine("granula=" + granula + " formi=" + formi + " sekundi=" + (int)sat.Elapsed.TotalSeconds);
            glava.AppendLine("iznimka=" + iznimka + " ucitanje=" + ucitanje);
            glava.AppendLine("prekriveno-okvirom=" + prekriveno + " u formi=" + formiPrekriveno + " (mora biti 0)");
            glava.AppendLine("preklapanje=" + preklapanje + " (kao NetGen: kasnija kontrola ispred)");
            glava.AppendLine("nema=" + nema + " bez-velicine=" + bezVelicine + " izvan=" + izvan);
            foreach (var par in nemaPoGrani)
                glava.AppendLine("nema\t" + par.Key + "\t" + par.Value);
            glava.AppendLine("---- primjeri (prekriveno okvirom i iznimka) ----");
            foreach (string p in primjeri)
                glava.AppendLine(p);
            foreach (var par in primjeriOstalo)
            {
                glava.AppendLine("---- primjeri " + par.Key + " ----");
                foreach (string p in par.Value)
                    glava.AppendLine(p);
            }
            Console.Write(glava.ToString());
            if (!string.IsNullOrEmpty(izvjestaj))
            {
                string mapa = Path.GetDirectoryName(izvjestaj);
                if (!string.IsNullOrEmpty(mapa))
                    Directory.CreateDirectory(mapa);
                File.WriteAllText(izvjestaj, glava.ToString() + tsv.ToString(), new UTF8Encoding(false));
            }
            // Okvir preko polja i forma koja se ne složi ruše build; ostalo je izvještaj.
            return prekriveno > 0 || iznimka > 0 ? 1 : 0;
        }

        private static void Primjer(Dictionary<string, List<string>> primjeri, string vrsta, string red)
        {
            List<string> l;
            if (!primjeri.TryGetValue(vrsta, out l))
            {
                l = new List<string>();
                primjeri[vrsta] = l;
            }
            if (l.Count < 25)
                l.Add(red);
        }

        /// <summary>Susjed ispred (manji indeks u roditelju), vidljiv i neproziran, preko središta kontrole.</summary>
        private static Control Prekriva(Control c, Control forma)
        {
            Control p = c.Parent;
            if (p == null)
                return null;
            int ja = p.Controls.GetChildIndex(c);
            var sredina = new Point(c.Left + c.Width / 2, c.Top + c.Height / 2);
            for (int i = 0; i < ja; i++)
            {
                Control s = p.Controls[i];
                if (s == null || s == c || !Vidljiva(s) || !Neprozirna(s))
                    continue;
                if (s.Width <= 0 || s.Height <= 0)
                    continue;
                if (s.Bounds.Contains(sredina))
                    return s;
            }
            return null;
        }

        private static string Jedan(string s)
        {
            string t = (s ?? "").Replace("\t", " ").Replace("\r", " ").Replace("\n", " ");
            return t.Length > 200 ? t.Substring(0, 200) : t;
        }
    }
}

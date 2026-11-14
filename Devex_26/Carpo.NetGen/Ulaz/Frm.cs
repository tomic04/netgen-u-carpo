using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Model;

namespace Carpo.NetGen
{
    /// <summary>
    /// Drugi NetGen ulaz. U spin.dbo.ssMeni ih je 9: asm:NetGenDLL.frm:&lt;xml&gt;.
    /// Ista polja kao netgen/frm.vb. Zivi je Run8 (New Form2). Stari Run u VB-u
    /// ne radi New, ali potpis postoji pa ga host i dalje moze pozvati.
    /// Nema strNGParams ni ShellResize. Ima ulazniID i ulaznaForma (Form2.vb 489, 503).
    /// Run8 (ULZ-02) otvara dijalog kao Form2.Run: forma ulaznaForma ili MAINFORM, ID = ulazniID,
    /// ProcitajXML, greška ONLOAD-a ne otvara formu, pa SetParent na clientHandle i Show (PrvaForma 993-1028;
    /// Koler "Jup10" je bez ruba i prati veličinu roditelja). Zatvaranje javi Jupiteru poruku 0x7D0
    /// (FormClosing 4112-4113), kao Carpo Base.OnFormClosed. VB ne puni hWnd ni WCaption; ovdje su
    /// hWnd i naslov dijaloga.
    /// Ime klase je frm, malim slovom, kao u VB-u i u Parametar retku.
    /// </summary>
    public class frm
    {
        public string moduleId;
        public int mainHandle;
        public string WCaption;
        public int hWnd;
        public string Ikona;
        public string Koler = "";
        public string ulazniID = "";
        public string ulaznaForma = "";
        public string TestXmlPath;
        public string ZadnjaGreska;

        /// <summary>Provjera puta bez prozora (test). Jupiter ovo ne postavlja.</summary>
        public bool BezProzora;

        /// <summary>Dijalog koji je Run8 otvorio.</summary>
        public System.Windows.Forms.Form Forma { get; private set; }

        public NgProject Projekt { get; private set; }

        public object Run(string server, string database, string userId, string password,
            int operater, int clientHandle, short top, short left, string remoteDllPath)
        {
            // Form2.vb: stari Run ne napravi New Form2, pa ne otvara ništa.
            return Izvrsi(server, database, remoteDllPath, operater, false, 0);
        }

        public object Run8(string server, string database, string userId, string password,
            int operater, int clientHandle, short top, short left, string remoteDllPath)
        {
            return Izvrsi(server, database, remoteDllPath, operater, !BezProzora, clientHandle);
        }

        private object Izvrsi(string server, string database, string remoteDllPath, int operater, bool otvori, int clientHandle)
        {
            var o = NgUlaz.Pokreni(TestXmlPath, moduleId, remoteDllPath);
            if (o.Ucitano && o.Greska == null)
                WCaption = o.WCaption;
            ZadnjaGreska = o.Greska;
            Projekt = o.Projekt;
            if (otvori && Projekt != null && o.Greska == null)
                OtvoriDijalog(o.Putanja, server, database, operater, clientHandle);
            return null;
        }

        private void OtvoriDijalog(string putanja, string server, string database, int operater, int clientHandle)
        {
            // Form2.vb Run 64-65: strImeForme = "MAINFORM", ulaznaForma ima prednost.
            string ime = string.IsNullOrWhiteSpace(ulaznaForma) ? "MAINFORM" : ulaznaForma.Trim();
            // Glavni XML, pa INCLUDE (Form2.ProcitajXML 840-934, XML-02).
            NgForm forma = Carpo.NetGen.Xml.IncludeForme.Nadji(Projekt, ime);
            if (forma == null)
            {
                ZadnjaGreska = "nema forme " + ime;
                return;
            }
            try
            {
                string spoj = SpinVeza.Spoj(server, database);
                var k = new SqlKontekst();
                k.OperaterId = operater.ToString(CultureInfo.InvariantCulture);
                // 55: If ulazniID <> "" Then ID = ulazniID (inače 0).
                k.Id = string.IsNullOrWhiteSpace(ulazniID) ? "0" : ulazniID.Trim();
                k.XmlName = System.IO.Path.GetFileName(putanja ?? "");
                k.Version = Carpo.NetGen.Ui.NgVerzija.Tekst();
                k.Node = "null";
                k.ProjektNaslov = Projekt.Naslov ?? "";
                k.HelpKod = (Projekt.Atributi == null ? "" : (Projekt.Atributi.Vrijednost("HELPKOD") ?? "")).Trim().ToUpperInvariant();
                k.Clipboard = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var d = new Carpo.NetGen.Ui.Dijalog();
                d.Konekcija = spoj;
                d.Operater = k.OperaterId;
                d.Id = k.Id;
                var citac = new Carpo.NetGen.Ulaz.FrmCitac(spoj);
                d.Prikazi(forma, k, citac.Citaj, false, null);
                // Form2.Run 70: greška ONLOAD-a, forma se ne otvara.
                if (!string.IsNullOrEmpty(d.GreskaOnLoad))
                {
                    ZadnjaGreska = d.GreskaOnLoad;
                    d.Dispose();
                    return;
                }
                Ugradi(d, clientHandle);
                d.Show();
                Forma = d;
                hWnd = (int)d.Handle;
                if (!string.IsNullOrEmpty(d.Text))
                    WCaption = d.Text;
            }
            catch (Exception ex)
            {
                ZadnjaGreska = ex.Message;
                Carpo.NetGen.Log.NgDnevnik.Greska("frm.Run8: " + ex.Message);
            }
        }

        /// <summary>
        /// Form2 PrvaForma (993-1028): SetParent na Jupiterov panel. Koler2 "Jup10": bez ruba, a na Resize
        /// roditelja i LocationChanged njegove forme dijalog je na (0,0) i veličine roditelja.
        /// Form2_FormClosing 4112-4113: SendMessage(main_handle, &amp;H7D0, Handle) (Carpo Base šalje 2000).
        /// </summary>
        private void Ugradi(Form d, int clientHandle)
        {
            bool jup10 = clientHandle != 0 && string.Equals((Koler ?? "").Trim(), "Jup10", StringComparison.Ordinal);
            // Rub prije SetParent: promjena stila nakon njega može ponovo stvoriti prozor bez roditelja.
            if (jup10)
                d.FormBorderStyle = FormBorderStyle.None;
            if (clientHandle != 0)
                SetParent(d.Handle, new IntPtr(clientHandle));
            if (jup10)
            {
                Control roditelj = Control.FromHandle(new IntPtr(clientHandle));
                if (roditelj != null)
                {
                    Form forma = roditelj.FindForm();
                    EventHandler prati = delegate
                    {
                        if (d.IsDisposed)
                            return;
                        d.WindowState = FormWindowState.Normal;
                        d.Location = System.Drawing.Point.Empty;
                        d.Size = roditelj.Size;
                    };
                    roditelj.Resize += prati;
                    if (forma != null)
                        forma.LocationChanged += prati;
                    d.FormClosed += delegate
                    {
                        roditelj.Resize -= prati;
                        if (forma != null)
                            forma.LocationChanged -= prati;
                    };
                }
            }
            int glavni = mainHandle;
            if (glavni != 0)
            {
                d.FormClosed += delegate
                {
                    try
                    {
                        SendMessage(new IntPtr(glavni), 0x7D0, d.Handle, IntPtr.Zero);
                    }
                    catch (Exception ex)
                    {
                        Carpo.NetGen.Log.NgDnevnik.Greska("frm 0x7D0: " + ex.Message);
                    }
                };
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}

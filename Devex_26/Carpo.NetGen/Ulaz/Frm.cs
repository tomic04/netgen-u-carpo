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
            Carpo.NetGen.Grid.SpinVeza.Prijava(userId, password);
            return Izvrsi(server, database, remoteDllPath, operater, false, 0);
        }

        public object Run8(string server, string database, string userId, string password,
            int operater, int clientHandle, short top, short left, string remoteDllPath)
        {
            // Form2 441-444 i 282-285: s korisnikom SQL prijava (ULZ-07).
            Carpo.NetGen.Grid.SpinVeza.Prijava(userId, password);
            return Izvrsi(server, database, remoteDllPath, operater, !BezProzora, clientHandle);
        }

        private object Izvrsi(string server, string database, string remoteDllPath, int operater, bool otvori, int clientHandle)
        {
            // M5 (XML-01): Jupiter traži XML u DMS-u i bazi JupiterXML prije diska; test ide s diska.
            if (string.IsNullOrWhiteSpace(TestXmlPath))
                Carpo.NetGen.Xml.XmlIzvor.Ukljuci(server);
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
            Action otpusti = null;
            try
            {
                string spoj = SpinVeza.SaAplikacijom(SpinVeza.Spoj(server, database), putanja);
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
                k.XmlPut = putanja ?? "";
                k.ProjektVerzija = (Projekt.Atributi == null ? "" : (Projekt.Atributi.Vrijednost("VERSION") ?? "")).Trim();
                var d = new Carpo.NetGen.Ui.Dijalog();
                d.Konekcija = spoj;
                d.Operater = k.OperaterId;
                d.Id = k.Id;
                var citac = new Carpo.NetGen.Ulaz.FrmCitac(spoj);
                // SES-01: veza živi dok je dijalog otvoren, kao Conn u Form2.Run.
                NgVeza.Prijavi(spoj);
                bool otpustena = false;
                otpusti = delegate
                {
                    if (otpustena)
                        return;
                    otpustena = true;
                    NgVeza.Otpusti(spoj);
                };
                d.FormClosed += delegate { otpusti(); };
                citac.Rok = Carpo.NetGen.Komande.VrijemeUpita.Iz(forma.Atributi, citac.Rok);
                // ULZ-05: bez podloge dijalog sam otvara djecu (Form2.NovaGenerickaForma2).
                d.OtvoriDrugu = delegate (string imeDjeteta) { OtvoriDijete(d, imeDjeteta, k, spoj, citac, 1); };
                d.Prikazi(forma, k, citac.Citaj, false, null);
                // Form2.Run 70: greška ONLOAD-a, forma se ne otvara.
                if (!string.IsNullOrEmpty(d.GreskaOnLoad))
                {
                    ZadnjaGreska = d.GreskaOnLoad;
                    otpusti();
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
                if (otpusti != null && (Forma == null || Forma.IsDisposed))
                    otpusti();
            }
        }

        /// <summary>
        /// ULZ-05. Form2.NovaGenerickaForma2 4259-4479 za ulaz frm, gdje nema podloge: dijete je modalni dijalog
        /// nad roditeljem, s istim spojem, operaterom, ID-om i CB-om (parametri roditelja su u CB-u, kao
        /// strPrevParametri 4392). ONSUBMIT djeteta šalje sam dijalog na OK; nakon OK s NEXTFORM otvara se
        /// sljedeća (4440-4466). Roditelj poslije osvježi kontrole s REFRESHAFTERCHILD. Ime može imati
        /// (parametre); forma se traži bez njih, kao na podlozi.
        /// </summary>
        private void OtvoriDijete(Carpo.NetGen.Ui.Dijalog roditelj, string naredba, SqlKontekst k, string spoj,
            Carpo.NetGen.Ulaz.FrmCitac citac, int dubina)
        {
            if (dubina > 8 || roditelj == null || roditelj.IsDisposed)
                return;
            string ime = (naredba ?? "").Trim();
            if (ime.StartsWith(":", StringComparison.Ordinal))
                ime = ime.Substring(1).Trim();
            int z = ime.IndexOf('(');
            if (z > 0)
                ime = ime.Substring(0, z).Trim();
            if (ime.Length == 0)
                return;
            NgForm forma = Carpo.NetGen.Xml.IncludeForme.Nadji(Projekt, ime);
            if (forma == null)
            {
                Carpo.NetGen.Log.NgDnevnik.Akcija("nema forme " + ime);
                return;
            }
            SqlKontekst kd = SqlTekst.ZaFormu(k);
            kd.Id = roditelj.Id;
            citac.Rok = Carpo.NetGen.Komande.VrijemeUpita.Iz(forma.Atributi, citac.Rok);
            string sljedeca = "";
            using (var dij = new Carpo.NetGen.Ui.Dijalog())
            {
                dij.Konekcija = spoj;
                dij.Operater = k.OperaterId;
                dij.Id = roditelj.Id;
                dij.ZadnjiSql = roditelj.ZadnjiSql;
                dij.OtvoriDrugu = delegate (string imeDjeteta) { OtvoriDijete(dij, imeDjeteta, kd, spoj, citac, dubina + 1); };
                dij.Prikazi(forma, kd, citac.Citaj, true, roditelj);
                if (dij.DialogResult == DialogResult.OK)
                    sljedeca = Carpo.NetGen.Komande.DijalogTok.NextForm(forma);
            }
            if (sljedeca.Length > 0)
                OtvoriDijete(roditelj, sljedeca, k, spoj, citac, dubina + 1);
        }

        /// <summary>
        /// Form2 PrvaForma (993-1028): SetParent na Jupiterov panel. Koler2 "Jup10": bez ruba, a na Resize
        /// roditelja i LocationChanged njegove forme dijalog je na (0,0) i veličine roditelja.
        /// Form2_FormClosing 4112-4113: SendMessage(main_handle, &amp;H7D0, Handle) (Carpo Base šalje 2000).
        /// </summary>
        private void Ugradi(Form d, int clientHandle)
        {
            bool jup10 = Carpo.NetGen.Ulaz.JupiterProzor.JeJup10(Koler, clientHandle);
            if (jup10)
                Carpo.NetGen.Ulaz.JupiterProzor.BezRuba(d);
            if (clientHandle != 0)
                SetParent(d.Handle, new IntPtr(clientHandle));
            if (jup10)
                Carpo.NetGen.Ulaz.JupiterProzor.PratiRoditelja(d, clientHandle, false);
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

using System;
using System.Diagnostics;
using System.Globalization;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// DajPokaziHelp (Form1 7781-7799, Form2 9483-9497): JupiterHelp.exe -s server -d baza -h HELPKOD
    /// -o Str(operater) -p imeXml. Podloga (F1, "?") i dijalog (F1, gumb bez ONCLICK ili "help") (HLP-01, HLP-02).
    /// </summary>
    public static class JupiterPomoc
    {
        public static string Argumenti(string server, string baza, string kod, string operater, string imeXml)
        {
            int broj;
            // VB Str() pozitivnom broju doda razmak sprijeda.
            string op = operater ?? "";
            string str = int.TryParse(op, NumberStyles.Integer, CultureInfo.InvariantCulture, out broj) && broj >= 0
                ? " " + broj.ToString(CultureInfo.InvariantCulture)
                : op;
            return "-s " + (server ?? "") + " -d " + (baza ?? "") + " -h " + (kod ?? "") + " -o " + str + " -p " + (imeXml ?? "");
        }

        public static void Pokreni(string server, string baza, string kod, string operater, string imeXml)
        {
            string argumenti = Argumenti(server, baza, kod, operater, imeXml);
            NgDnevnik.Akcija("JupiterHelp.exe " + argumenti);
            // Klik-pregled ne pokreće vanjske programe.
            if (NgPoruka.Tiho)
                return;
            try
            {
                var p = new Process();
                p.StartInfo.FileName = "JupiterHelp.exe";
                p.StartInfo.Arguments = argumenti;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                p.Start();
            }
            catch (Exception ex)
            {
                // On Error Resume Next: bez JupiterHelp.exe ne javlja ništa korisniku.
                NgDnevnik.Greska("JupiterHelp.exe: " + ex.Message);
            }
        }
    }
}

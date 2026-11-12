using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Carpo.Lite
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            string server = Properties.Settings.Default.Server;
            string baza = Properties.Settings.Default.Baza;
            string Uid = Properties.Settings.Default.Uid;
            string Pwd = Properties.Settings.Default.Pwd;
            string xml = Properties.Settings.Default.xmlPath;
            int OperateriId = Properties.Settings.Default.OperateriId;

            Gen.Start start = new Gen.Start();
            start.TestXmlPath = xml;
            start.procID = Globalka.getProces();
            start.ModuleId = Path.GetFileName(xml);
            start.Run(server, baza, Uid, Pwd, OperateriId,1837690, 0, 0, "");

            Application.Run();
        }
    }

    public static class Globalka
    {
        public static int proces = getProces();

        public static int getProces()
        {
            Int32 currentProcess = Process.GetCurrentProcess().Id;
            return currentProcess;
        }
    }
}
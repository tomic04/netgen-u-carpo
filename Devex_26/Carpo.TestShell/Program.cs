using System;
using System.Windows.Forms;

namespace Carpo.TestShell
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args != null && args.Length >= 3 && args[0] == "--ui-smoke")
            {
                var main = new Main("localhost", "spin", args[1], 7, "", "", true);
                string snimka = args[2];
                bool klik = args.Length >= 4 && args[3] == "--klik";
                main.Shown += delegate
                {
                    main.BeginInvoke(new Action(delegate
                    {
                        int kod = main.PokreniUiSmoke(snimka, klik);
                        Environment.Exit(kod);
                    }));
                };
                Application.Run(main);
                return;
            }
            Application.Run(new Main());

            //Carpo.XmlGen.Generate.Start("d:\\xmlGenTest.xml");
        }
    }
}
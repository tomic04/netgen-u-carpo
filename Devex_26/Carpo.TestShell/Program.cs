using System;
using System.IO;
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
            if (args != null && args.Length >= 3 && args[0] == "--ui-ext")
            {
                string snimka = args[2];
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                var main = new Main("localhost", "spin", args[1], 7, "", "", true);
                main.Shown += delegate
                {
                    main.BeginInvoke(new Action(delegate
                    {
                        int kod = main.PokreniEkstenzije(args[1], snimka);
                        Environment.Exit(kod);
                    }));
                };
                Application.Run(main);
                return;
            }
            if (args != null && args.Length >= 3 && args[0] == "--ui-boja")
            {
                string snimka = args[2];
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += delegate (object s, System.Threading.ThreadExceptionEventArgs e)
                {
                    try { File.AppendAllText(snimka + ".iznimka.txt", e.Exception.ToString() + "\r\n"); } catch { }
                };
                var main = new Main("localhost", "spin", args[1], 7, "", "", true);
                main.Shown += delegate
                {
                    main.BeginInvoke(new Action(delegate
                    {
                        int kod = main.PokreniBoju(args[1], snimka);
                        Environment.Exit(kod);
                    }));
                };
                Application.Run(main);
                return;
            }
            if (args != null && args.Length >= 3 && args[0] == "--sweep")
            {
                string izvjestaj = args[2];
                int sekundi = 6;
                int shard = 0;
                int dijelova = 6;
                if (args.Length >= 4)
                    int.TryParse(args[3], out sekundi);
                if (args.Length >= 6)
                {
                    int.TryParse(args[4], out shard);
                    int.TryParse(args[5], out dijelova);
                }
                // SWP-02: sweep nad drugom bazom (npr. backup klijenta na jupiter2019); bez toga localhost/spin.
                string sweepServer = args.Length >= 8 && args[6].Trim().Length > 0 ? args[6].Trim() : "localhost";
                string sweepBaza = args.Length >= 8 && args[7].Trim().Length > 0 ? args[7].Trim() : "spin";
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += delegate (object s, System.Threading.ThreadExceptionEventArgs e)
                {
                    global::Carpo.TestShell.Main.ZadnjaNit = e.Exception == null ? "ThreadException" : global::Carpo.TestShell.Main.OpisIznimke(e.Exception);
                    global::Carpo.TestShell.Main.ZapisiIznimku(e.Exception);
                };
                var main = new Main(sweepServer, sweepBaza, args[1], 7, "", "", true);
                main.Shown += delegate
                {
                    main.BeginInvoke(new Action(delegate
                    {
                        int kod = main.PokreniSweep(args[1], izvjestaj, sekundi, shard, dijelova);
                        Environment.Exit(kod);
                    }));
                };
                Application.Run(main);
                return;
            }
            if (args != null && args.Length >= 3 && args[0] == "--ui-smoke")
            {
                string snimka = args[2];
                bool klik = args.Length >= 4 && args[3] == "--klik";
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += delegate (object s, System.Threading.ThreadExceptionEventArgs e)
                {
                    try { File.AppendAllText(snimka + ".iznimka.txt", e.Exception.ToString() + "\r\n"); } catch { }
                };
                var main = new Main("localhost", "spin", args[1], 7, "", "", true);
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
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
            if (args != null && args.Length >= 3 && args[0] == "--ui-smoke")
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
                        int kod = main.PokreniUiSmoke(snimka);
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
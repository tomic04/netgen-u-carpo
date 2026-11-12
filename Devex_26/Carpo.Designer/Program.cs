using System;
using System.Windows.Forms;

namespace Carpo.Designer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            //DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Visual Studio 2013 Blue");
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Devexpress Style");
            //var splashScreenImage = new Bitmap(CurrentAssembly.GetManifestResourceStream("DockingDemo.Resources.splashScreen.png"));
            //DevExpress.XtraSplashScreen.SplashScreenManager.ShowImage(splashScreenImage, true, false);

            SetConnectionString();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        private static void SetConnectionString()
        {
            string server = "spin-sql";
            string database = "Spin";
            string userId = "";
            string password = "";

            Carpo.Data.Connection.ConnectionString = Carpo.Data.Constructor.SetConnectionString(server, database, userId, password);
        }
    }
}
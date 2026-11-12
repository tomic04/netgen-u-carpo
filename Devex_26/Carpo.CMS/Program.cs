using System;
using System.Windows.Forms;

namespace Carpo.CMS
{
    // TEST - probni test sa VM
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
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
using System;
using System.IO;
using System.Windows.Forms;

namespace Carpo.Common
{
    public static class LogWriter
    {
        public static bool IsTestShell { get; set; } = false;
        public static string LogDir { get; set; }
        public static string AppName { get; set; }
        public static StreamWriter log;

        public static void Init()
        {
            if (!string.IsNullOrEmpty(LogDir) && !Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }

            string logFile = LogDir + Convert.ToString(AppName + "_Log_" + System.DateTime.Now.ToString("yyyyMMdd") + ".txt");

            log = new StreamWriter(logFile, System.IO.File.Exists(logFile));
        }

        public static void LogLine(string logMessage, bool raiseMessage = false)
        {
            log.WriteLine(String.Format(Convert.ToString("{0} | ") + logMessage, DateTime.Now.ToString("dd.MM.yyyy. @ HH:mm")));

            if (raiseMessage && IsTestShell)
                MessageBox.Show(logMessage);
        }

        public static void InsertBrakeLine()
        {
            log.WriteLine("--------------------");
        }

        public static void LogEnd()
        {
            log.Close();
        }
    }
}
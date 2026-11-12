using Carpo.Core.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using Jupiter.Utility;
// DORADA: Dodano TXT mirror logiranje za CarpoBot/Cursor integraciju

namespace Carpo.Core
{
    public class Logger : ILogger
    {
        public List<Log.Models.LoggerDef> LogList { get; set; } = new List<Log.Models.LoggerDef>();

        public LogLevel LogLevel { get; set; }
        public int LogLevelUser { get; set; }

        public string Version { get; set; } //root --> version

        //public PictureBox ImgInfo { get; set; }
        public DevExpress.XtraBars.BarEditItem Imginfo { get; set; } // Repositor ImgInfo { get; set; }

        public Dictionary<string, Image> Infoimages { get; set; }

        public bool ErrorExists { get; set; }
        public bool WarningExists { get; set; }
        public DataTable DatalistInfo { get; set; }

        private readonly object _mirrorLock = new object();
        private string _mirrorFilePath;
        private readonly DateTime _mirrorSessionTime = DateTime.Now;
        private bool _mirrorPathInitialized = false;

        /// <summary>
        /// Lokacija direktorija u koji se zapisuje plain-text log za potrebe Cursor-a.
        /// Default: folder LogoviTestShella pored exe-a (Carpo.TestShell.exe).
        /// Override: <ExeDir>\LogoviTestShella.config (prva linija = putanja) â€“ ako postoji i nije prazan.
        /// </summary>
        public string MirrorLogDirectory { get; set; }

        private const string MirrorConfigFileName = "LogoviTestShella.config";
        private const string DefaultLogFolderName = "LogoviTestShella";

        /// <summary>
        /// Naziv granule (Carpo XML bez ekstenzije). TXT mirror ime + JupiterLogs.GranuleName.
        /// </summary>
        string _granuleName;
        public string GranuleName
        {
            get { return _granuleName; }
            set
            {
                _granuleName = value;
                try
                {
                    if (!string.IsNullOrWhiteSpace(value))
                        AppLogger.SetGranule(value);
                }
                catch { }
            }
        }

        /// <summary>
        /// Ime servera; koristi se u imenu log datoteke.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Ime baze; koristi se u imenu log datoteke.
        /// </summary>
        public string DatabaseName { get; set; }

        private static readonly object _jupiterInitLock = new object();
        private static bool _jupiterInitialized;
        private static ComponentLogger _jupiterLogger;

        public Logger(LogLevel logLevel)
        {
            LogLevel = logLevel;
            ResolveMirrorPath();
            if (!string.IsNullOrWhiteSpace(MirrorLogDirectory))
            {
                EnsureMirrorDirectory();
                WriteLog(LogLevel.InfoCore, "MirrorPath", MirrorLogDirectory);
            }
        }

        public void Info(string opis, string text, params object[] args)
        {
            if (LogLevel < LogLevel.Info)
                return;
            WriteLog(LogLevel.Info, opis, text, args);
        }

        public void InfoCore(string opis, string text, params object[] args)
        {
            if (LogLevel < LogLevel.Info)
                return;
            WriteLog(LogLevel.InfoCore, opis, text, args);
        }
        public void InfoTest(string opis, string text, params object[] args)
        {

            WriteLog(LogLevel.Test, opis, text, args);
        }
        public void InfoData(string opis, string text, params object[] args)
        {
            if (LogLevel < LogLevel.Info)
                return;
            WriteLog(LogLevel.InfoData, opis, text, args);
        }

        public void Warn(string opis, string text, params object[] args)
        {
            if (LogLevel < LogLevel.Warn)
                return;

            if (LogLevelUser == 2)
            {
                Common.Helpers.JMessages.Warning(opis + Environment.NewLine + text);
            }
            else if (LogLevelUser == 1)
            {
                Common.Helpers.JMessages.Warning("Molim Vas informirajte Spin d.o.o." + Environment.NewLine + "preko HelpDesk-a ili na tel. 031/20 30 20");
            }

            WriteLog(LogLevel.Warn, opis, text, args);
            try
            {
                InsertLog("Warn", opis, text, args[0] as Exception, Core.PubVars.OperateriId, args);
            }
            catch
            {
                InsertLog("Warn", opis, text, null, Core.PubVars.OperateriId, args);
            }
            WarningExists = true;
            SetIcon();
        }

        public void Error(string opis, string text, params object[] args)
        {
            if (LogLevel < LogLevel.Error)
                return;
            WriteLog(LogLevel.Error, opis, text, args);
            try
            {
                InsertLog("Error", opis, text, args[0] as Exception, Core.PubVars.OperateriId, args);
            }
            catch// sluÄŤajevi kada u parametar args nije niĹˇta proslijeÄ‘eno - insertat Ä‡e sve osim ErrorTeksta
            {
                InsertLog("Error", opis, text, null, Core.PubVars.OperateriId, args);
            }
            if (LogLevelUser == 4)
            {
                Common.Helpers.JMessages.Error(null, text, "GreĹˇka (" + opis + ")", true);
            }
            else if (LogLevelUser == 3)
            {
                if (args != null && args.Length > 0)
                {
                    var err = args[0] as Exception;
                    if (err != null)
                    {
                        Common.Helpers.JMessages.Error(err, "Molim Vas informirajte Spin d.o.o. preko HelpDesk-a ili na tel. 031/20 30 20");
                        //Common.Helpers.JMessages.Error(err, "Molim Vas informirajte Spin d.o.o." + Environment.NewLine + "preko HelpDesk-a ili na tel. 031/20 30 20");
                    }
                    else
                    {
                        Common.Helpers.JMessages.Error(err, "Molim Vas informirajte Spin d.o.o." + Environment.NewLine + "preko HelpDesk-a ili na tel. 031/20 30 20", "GreĹˇka br. -1");
                    }
                }
            }
            ErrorExists = true;
            SetIcon();
        }

        public void Debug(string opis, string text, params object[] args)
        {
            if (LogLevel < LogLevel.Debug)
                return;
            WriteLog(LogLevel.Debug, opis, text, args);
        }

        /// <summary>
        /// PERF into JupiterLogs (file always; SQL if ElapsedMs >= AppLogger.SqlMinPerfMs).
        /// Does not go to spCarpoLogInsert / Ctrl+F11 list.
        /// </summary>
        public void Performance(string source, string operation, Stopwatch sw, string actionCode = "Perf")
        {
            try
            {
                EnsureJupiterLogger();
                if (_jupiterLogger == null)
                    return;
                if (sw != null && sw.IsRunning)
                    sw.Stop();
                long ms = sw == null ? 0 : sw.ElapsedMilliseconds;
                _jupiterLogger.Performance(
                    string.IsNullOrWhiteSpace(source) ? "Carpo" : source,
                    operation ?? string.Empty,
                    ms,
                    actionCode);
            }
            catch { }
        }

        private void WriteLog(LogLevel logLevel, string opis, string text, params object[] args)
        {
            try
            {
                Log.Models.LoggerDef logger = new Log.Models.LoggerDef() { Time = DateTime.Now, LogLevel = logLevel, Opis = opis, Text = text, Args = args };
                LogList.Add(logger);
                // Pozovi MirrorToFile samo ako je MirrorLogDirectory postavljen (config postoji i ima putanju)
                if (!string.IsNullOrWhiteSpace(MirrorLogDirectory))
                {
                    MirrorToFile(logLevel, opis, text, args);
                }
                // Dual-write Ctrl+F11 LogList unosa u JupiterLogger fajl
                MirrorToJupiter(logLevel, opis, text, args);
            }
            catch
            { }
        }

        private static void EnsureJupiterLogger()
        {
            if (_jupiterInitialized)
                return;

            lock (_jupiterInitLock)
            {
                if (_jupiterInitialized)
                    return;

                try
                {
                    AppLogger.Init("Carpo");
                    _jupiterLogger = AppLogger.For("Carpo");
                    try
                    {
                        var server = Carpo.Data.Connection.Server;
                        if (!string.IsNullOrWhiteSpace(server))
                            AppLogger.ConfigureSql(server);
                    }
                    catch { }
                }
                catch
                {
                    _jupiterLogger = null;
                }

                _jupiterInitialized = true;
            }
        }

        /// <summary>
        /// Standalone hosts (TestShell) bind JupiterLogs after they know the SQL server.
        /// In-process Jupiter already called ConfigureSql / SetContext.
        /// </summary>
        public static void BindJupiterLogs(string server, int? operatorId = null, string operatorName = null, string clientDatabase = null, string companyName = null, string granuleName = null)
        {
            try
            {
                lock (_jupiterInitLock)
                {
                    if (!_jupiterInitialized)
                    {
                        AppLogger.Init("Carpo");
                        _jupiterLogger = AppLogger.For("Carpo");
                        _jupiterInitialized = true;
                    }
                }
                if (!string.IsNullOrWhiteSpace(server))
                    AppLogger.ConfigureSql(server);
                AppLogger.SetContext(operatorId, operatorName, clientDatabase, companyName, null);
                if (!string.IsNullOrWhiteSpace(granuleName))
                    AppLogger.SetGranule(granuleName);
            }
            catch { }
        }

        /// <summary>
        /// Dual-write Ctrl+F11 LogList entry into JupiterLogger file.
        /// Failures are swallowed so Carpo flow is never broken.
        /// </summary>
        private void MirrorToJupiter(LogLevel logLevel, string opis, string text, object[] args)
        {
            try
            {
                EnsureJupiterLogger();
                if (_jupiterLogger == null)
                    return;

                string source = string.IsNullOrWhiteSpace(opis) ? "Carpo" : opis;
                string message = text ?? string.Empty;
                Exception ex = null;

                if (args != null && args.Length > 0)
                {
                    ex = args.OfType<Exception>().FirstOrDefault();
                    var extras = args.Where(a => a != null && !(a is Exception)).Select(a => a.ToString());
                    string extraText = string.Join(" | ", extras);
                    if (!string.IsNullOrWhiteSpace(extraText))
                        message = string.IsNullOrWhiteSpace(message) ? extraText : message + " | " + extraText;
                }

                // Base.cs logs the full connection string (Set ConnStr / Set ConnStrRes); on SQL-auth sites
                // that would put the password into JupiterLogs. Ctrl+F11 LogList keeps the original text.
                message = RedactSecrets(message);

                switch (logLevel)
                {
                    case LogLevel.Debug:
                        _jupiterLogger.Debug(source, message);
                        break;
                    case LogLevel.Warn:
                        _jupiterLogger.Warn(source, message);
                        break;
                    case LogLevel.Error:
                        // RAISERROR from a stored procedure without a sys message id (Number 50000) is a business
                        // validation the user tripped, not a fault. Only JupiterLogs is downgraded; Ctrl+F11,
                        // spCarpoLogInsert and the user-facing message are untouched. Real SQL errors
                        // (8144 too many arguments, 8134 divide by zero, timeouts, deadlocks) stay Error.
                        var sqlEx = ex as System.Data.SqlClient.SqlException;
                        if (sqlEx != null && sqlEx.Number == 50000)
                            _jupiterLogger.Warn(source, message + " | SqlException 50000 (validation)");
                        else
                            _jupiterLogger.Error(source, message, ex);
                        break;
                    default:
                        _jupiterLogger.Info(source, message);
                        break;
                }
            }
            catch
            { }
        }

        private static readonly System.Text.RegularExpressions.Regex _secretPattern =
            new System.Text.RegularExpressions.Regex(@"(?<key>\b(password|pwd)\s*=\s*)[^;]*",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Masks Password= / PWD= values before the row leaves the process. Cheap pre-check so the
        /// regex runs only on the rare rows that actually contain a credential-looking key.
        /// </summary>
        private static string RedactSecrets(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;
            if (message.IndexOf("pwd", StringComparison.OrdinalIgnoreCase) < 0
                && message.IndexOf("password", StringComparison.OrdinalIgnoreCase) < 0)
                return message;
            try
            {
                return _secretPattern.Replace(message, "${key}***");
            }
            catch
            {
                return message;
            }
        }

        /// <summary>
        /// Tenant-baza log (spCarpoLogInsert). PARKIRANO: ne uklanjati — support/dev
        /// godinama čitaju ovaj zapis. JupiterLogs je dodatni kanal, ne zamjena.
        /// Gašenje samo nakon eksplicitnog dogovora.
        /// </summary>
        private void InsertLog(string logLevel, string opis, string text, Exception err, int operateriid, params object[] args)
        {
            try
            {
                if (err == null)
                    Carpo.Data.Common.CarpoLogInsert(logLevel, opis, text, "", operateriid, args);
                else
                    Carpo.Data.Common.CarpoLogInsert(logLevel, opis, text, err.Message, operateriid, args);
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Warn, "method: InsertLog", ex.Message, null);
            }
        }

        public void SetIcon()
        {
            try
            {
                if (ErrorExists == true && WarningExists == true)
                    Imginfo.EditValue = Infoimages["errorwarning"];
                else if (ErrorExists == true)
                    Imginfo.EditValue = Infoimages["error"];
                else if (WarningExists == true)
                    Imginfo.EditValue = Infoimages["warning"];
                else
                    Imginfo.EditValue = Infoimages["valid"];
            }
            catch { }
        }

        private void MirrorToFile(LogLevel logLevel, string opis, string text, object[] args)
        {
            if (string.IsNullOrWhiteSpace(MirrorLogDirectory))
                return;

            try
            {
                EnsureMirrorDirectory();

                if (string.IsNullOrWhiteSpace(_mirrorFilePath))
                {
                    string granule = GranuleName;
                    if (string.IsNullOrWhiteSpace(granule))
                        granule = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName) ?? "UnknownGranule";

                    string server = SanitizeForFileName(ServerName ?? "");
                    string database = SanitizeForFileName(DatabaseName ?? "");

                    // Log_ImeGranule_Server_Baza_yyyyMMdd_HHmmss.txt
                    string fileName = $"Log_{granule}_{server}_{database}_{_mirrorSessionTime:yyyyMMdd_HHmmss}.txt";
                    _mirrorFilePath = Path.Combine(MirrorLogDirectory, fileName);
                }

                string argsText = "";
                if (args != null && args.Length > 0)
                {
                    var renderedArgs = args
                        .Where(a => a != null)
                        .Select(a => a.ToString());
                    if (renderedArgs.Any())
                        argsText = " | args: " + string.Join(" ; ", renderedArgs);
                }

                string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {logLevel} | {opis} | {text}{argsText}";

                lock (_mirrorLock)
                {
                    File.AppendAllText(_mirrorFilePath, line + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                // Ne smije sruĹˇiti osnovni logging. PokuĹˇaj fallback u temp.
                try
                {
                    var fallbackDir = Path.Combine(Path.GetTempPath(), "CarpoTestShellLog");
                    Directory.CreateDirectory(fallbackDir);
                    var fallbackFile = Path.Combine(fallbackDir, "MirrorFallback.txt");
                    File.AppendAllText(fallbackFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | FALLBACK | {opis} | {text} | err: {ex.Message} | path: {MirrorLogDirectory}{Environment.NewLine}");
                }
                catch
                {
                    // zadnja linija obrane: Debug output
                    System.Diagnostics.Debug.WriteLine("MirrorToFile failure for Logger.");
                }
            }
        }

        private void EnsureMirrorDirectory()
        {
            if (_mirrorPathInitialized)
                return;
            try
            {
                if (string.IsNullOrWhiteSpace(MirrorLogDirectory))
                    return;
                Directory.CreateDirectory(MirrorLogDirectory);
                _mirrorPathInitialized = true;
            }
            catch
            {
                try
                {
                    var fallbackDir = Path.Combine(Path.GetTempPath(), "CarpoTestShellLog");
                    Directory.CreateDirectory(fallbackDir);
                    MirrorLogDirectory = fallbackDir;
                    _mirrorPathInitialized = true;
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("EnsureMirrorDirectory failed.");
                }
            }
        }

        private void ResolveMirrorPath()
        {
            try
            {
                // Log u TXT samo kada je host Carpo.TestShell â€“ ne kada Jupiter ili drugi program pokreÄ‡e granulu
                var entryName = Assembly.GetEntryAssembly()?.GetName().Name ?? "";
                if (entryName.IndexOf("TestShell", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    MirrorLogDirectory = null;
                    return;
                }

                var candidateDirs = new List<string>();
                string appBase = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.IsNullOrWhiteSpace(appBase)) candidateDirs.Add(appBase);

                var entryLocation = System.Reflection.Assembly.GetEntryAssembly()?.Location;
                if (!string.IsNullOrWhiteSpace(entryLocation))
                {
                    var entryDir = Path.GetDirectoryName(entryLocation);
                    if (!string.IsNullOrWhiteSpace(entryDir)) candidateDirs.Add(entryDir);
                }

                var loggerAsmLoc = typeof(Logger).Assembly.Location;
                if (!string.IsNullOrWhiteSpace(loggerAsmLoc))
                {
                    var loggerDir = Path.GetDirectoryName(loggerAsmLoc);
                    if (!string.IsNullOrWhiteSpace(loggerDir)) candidateDirs.Add(loggerDir);
                }

                // remove duplicates, keep order
                var dirs = candidateDirs.Where(d => !string.IsNullOrWhiteSpace(d))
                                        .Select(d => d.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                        .ToList();

                string chosenBaseDir = dirs.FirstOrDefault() ?? appBase ?? Path.GetTempPath();
                string configPath = null;

                // probaj naÄ‡i postojeÄ‡i config u nekom od kandidata
                foreach (var dir in dirs)
                {
                    var candidateConfig = Path.Combine(dir, MirrorConfigFileName);
                    if (File.Exists(candidateConfig))
                    {
                        configPath = candidateConfig;
                        break;
                    }
                }

                if (configPath == null)
                {
                    configPath = Path.Combine(chosenBaseDir, MirrorConfigFileName);
                }

                // ako config postoji -> proÄŤitaj ga
                if (File.Exists(configPath))
                {
                    var lines = File.ReadAllLines(configPath)
                                    .Select(l => CleanPath(l))
                                    .Where(l => !string.IsNullOrWhiteSpace(l))
                                    .ToList();
                    if (lines.Count > 0)
                    {
                        // Config postoji i ima putanju -> koristi je (korisniÄŤka override)
                        MirrorLogDirectory = lines[0];
                        return;
                    }
                }
                else
                {
                    // Config ne postoji -> kreiraj ga prazan (opciono, za buduÄ‡u override)
                    try
                    {
                        File.WriteAllText(configPath, "");
                    }
                    catch
                    {
                        // Ako ne moĹľe kreirati config, ignoriraj
                    }
                }

                // Default: folder LogoviTestShella pored exe-a (Carpo.TestShell.exe)
                MirrorLogDirectory = Path.Combine(chosenBaseDir, DefaultLogFolderName);
            }
            catch
            {
                // Ako doÄ‘e do greĹˇke, ostavi null - ne pokuĹˇavaj kreirati log
                MirrorLogDirectory = null;
            }
        }

        private string CleanPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;
            return path.Trim().Trim('"');
        }

        private static string SanitizeForFileName(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return "";
            foreach (char c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');
            return s.Trim();
        }
    }
}

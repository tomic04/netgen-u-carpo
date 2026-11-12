using Carpo.Common.Helpers;
using Carpo.Core;
using Carpo.Data;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using SpinDMSUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
// DORADA: Dodano postavljanje GranuleName za TXT logiranje

namespace Carpo.UI.Connector.Abstract
{
    public abstract class Starter
    {
        public enum FormTypeEnum { None, Manager, Standard }

        public Starter()
        { }

        public string TestXmlPath { get; set; }
        public bool isModal { get; set; }
        public int procID { get; set; }

        public string WCaption { get; set; }
        public int HWnd { get; set; }
        public int MainHandle { get; set; }
        public string ModuleId { get; set; }
        public string Ikona { get; set; }
        public Form glavnaForma { get; set; }
        public string SkinName { get; set; }
        public string PaletteName { get; set; }
        public string CompanyName { get; set; }
        [ThreadStatic]
        static string imeFirme;


        public static string ImeFirme
        {
            get { return imeFirme; }
            set { imeFirme = value; }
        }
        public string MenuID { get; set; }
        public JRoot DefinitionRoot;
        public JForm InitForm;

        public string xmlPath { get; set; } = "";
        public FormTypeEnum FormType { get; set; }
        public Logger Logger { get; set; }
        public string ConnectionString { get; set; }
        public string ConnStrResources { get; set; }
        public string AssemblyName { get; set; }

        public Dictionary<string, object> VariablesForm { get; set; }
        public Dictionary<string, object> VariablesGlobal { get; set; }

        private string assemblyVersion;
        private string assemblyVersionToolTip;
        private string assemblyFolder;
        private string assemblyLocation;

        private DataTable dt;

        private string server, database, username, password, path;
        private int operateriid, parent;
        private short pTop, pLeft;

        public Forms.Jupiter.Base frmManager;
        public Forms.Jupiter.Base frmStandard;

        private FileFinder finder = new FileFinder("Carpo");

        public virtual void Run(string server, string database, string username, string password, int operateriid, int parent, short pTop, short pLeft, string path)
        {

            Logger = new Logger(Core.Log.LogLevel.Debug);
            // DORADA: postavi naziv granule (XML) za TXT log + JupiterLogs.GranuleName
            TrySetGranuleName(Logger);
            Carpo.Core.Log.DependencyContainer.Add(Logger);
            var startTime = DateTime.Now;
            Logger.InfoTest("Init", "Prva metoda, ulazna točka", startTime);
            Logger.InfoTest("START", $"Granula start | ModuleId={ModuleId}; TestXml={TestXmlPath}; server={server}; db={database}", startTime);
            this.server = server;
            this.database = database;
            this.username = username;
            this.password = password;
            this.operateriid = operateriid;
            this.parent = parent;
            this.pTop = pTop;
            this.pLeft = pLeft;
            this.path = path;

            PubVars.OperateriId = operateriid;


            Assembly assembly = this.GetType().Assembly;
            AssemblyName = assembly.GetName().Name;

            assemblyVersion = assembly.GetName().Version.Major.ToString() + "." + assembly.GetName().Version.Minor.ToString() + "." + assembly.GetName().Version.Build.ToString() + "." + assembly.GetName().Version.Revision.ToString();
            assemblyVersionToolTip = assembly.GetName().Version.ToString();
            assemblyFolder = Path.GetDirectoryName(assembly.Location);
            assemblyLocation = assembly.Location;

            string appName = AssemblyName + "." + ModuleId;
            ConnectionString = Carpo.Data.Constructor.SetConnectionString(server, database, username, password, appName);
            ConnStrResources = Carpo.Data.Constructor.SetConnectionString(server, "JupiterResources", username, password, appName);
            //VB6 connection string
            Connection.ConnectionStringADO = string.Format(
                    "Driver={0};Server={1};Database={2};Trusted_Connection=yes;Persist Security Info=true",
                    "{SQL Server}", server, database);

            Connection.ConnectionString = ConnectionString;
            Connection.ConnStrResources = ConnStrResources;
            Connection.DataBase = database;
            Connection.Server = server;
            Connection.Uid = username;
            Connection.Pwd = password;
            GlobalName.Name = ModuleId;
            Common.Translator.Init(IniFile.Ini["Server"], false);


            if (DeserializeXml() == false)
                return;

            if (SetInitForm() == false)
                return;
            //if (RunInitForm() == false)
            //    return;


            RegisterForm();
            Logger.InfoTest("Run", "Ukupno vrijeme", startTime);
            Logger.InfoTest("END", $"Granula završila init | ModuleId={ModuleId}; TestXml={TestXmlPath}; server={server}; db={database}", DateTime.Now);
        }

        private void TrySetGranuleName(Logger logger)
        {
            try
            {
                string granuleName = null;
                if (!string.IsNullOrWhiteSpace(ModuleId))
                    granuleName = Path.GetFileNameWithoutExtension(ModuleId);
                else if (!string.IsNullOrWhiteSpace(TestXmlPath))
                    granuleName = Path.GetFileNameWithoutExtension(TestXmlPath);

                var prop = logger.GetType().GetProperty("GranuleName");
                if (prop != null && prop.CanWrite)
                    prop.SetValue(logger, granuleName);
            }
            catch (Exception ex)
            {
                // ako property ne postoji u starijoj verziji, preskoči bez rušenja
                try { logger?.Warn("TrySetGranuleName", ex.Message); } catch { }
            }
        }

        private bool DeserializeXml()
        {
            var startTime = DateTime.Now;
            if (!string.IsNullOrEmpty(TestXmlPath))
                xmlPath = TestXmlPath;
            else if (!string.IsNullOrEmpty(ModuleId))
            {
                if (ModuleId.Contains("\\"))
                    xmlPath = ModuleId;
                else
                    xmlPath = Path.Combine(IniFile.Ini["RemoteDllPath"], "Carpo", ModuleId);
            }

            //if (!File.Exists(xmlPath))
            //{
            //    Common.Helpers.JMessages.Error(null, "putanja: '" + xmlPath + "'", "Neispravna putanja");
            //    return false;
            //}

            try
            {
                if (!finder.CheckFileExist(xmlPath))
                {
                    string message = "Na odabranoj putanji: '" + Path.GetDirectoryName(xmlPath) + "' ne pronalazi se datoteka " + Path.GetFileName(xmlPath) + ".";
                    
                    // Create exception with details so "Prikaži više" button is visible
                    Exception detailsException = null;
                    if (!string.IsNullOrEmpty(finder.exception))
                        detailsException = new Exception(finder.exception);
                    
                    Common.Helpers.JMessages.Error(detailsException, message, "Neuspješni dohvat granule");
                    return false;
                }



                using (var parts = new Serializer(Logger, xmlPath))
                {
                    DefinitionRoot = parts.DeserializeXml(ModuleId);
                    if (DefinitionRoot != null)
                        return true;
                    else
                        return false;
                }

            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(null, "putanja: '" + xmlPath + "'" + Environment.NewLine + ex.Message, "Neispravna deserializacija");
                return false;
            }
        }

        private bool SetInitForm()
        {
            var startTime = DateTime.Now;
            try
            {
                object masterForm = DefinitionRoot.Form.Where(x => x.Type == "master").FirstOrDefault();

                if (masterForm != null)
                {
                    InitForm = DefinitionRoot.Form.Where(x => x.Type == "master").FirstOrDefault();
                }
                else
                {
                    InitForm = DefinitionRoot.Form.FirstOrDefault();
                }

                if (InitForm != null)
                {
                    FormType = FormTypeEnum.None;
                    if (InitForm.ManagerLayout != null)
                        FormType = FormTypeEnum.Manager;
                    else if (InitForm.Layout != null)
                        FormType = FormTypeEnum.Standard;

                    if (FormType == FormTypeEnum.None)
                    {
                        Common.Helpers.JMessages.Error(null, "Elementi 'managerlayout' ili 'layout' nisu pronađeni unutar xml-a!");
                        return false;
                    }

                    return true;
                }
                else
                {
                    Common.Helpers.JMessages.Error(null, "putanja: '" + xmlPath + "'" + Environment.NewLine + "Nedostaje obavezan atribut type='master' unutar 'form' elementa");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(null, "method: SetInitForm - " + xmlPath + "'" + Environment.NewLine + ex.Message);
                return false;
            }
            finally
            {
                Logger.InfoTest("SetInitForm", "Postavlja inicijalnu formu", startTime);
            }
        }

        public virtual void RegisterForm(Forms.Jupiter.Base form)
        {
            var startTime = DateTime.Now;
            glavnaForma = form;
            form.InitLogger(null);

            if (VariablesGlobal != null)
                AddAllVars(form.VariablesGlobal, VariablesGlobal);
            if (VariablesForm != null)
                AddAllVars(form.VariablesForm, VariablesForm);

            form.AssamblyName = AssemblyName;
            form.AssamblyVersion = assemblyVersion;
            form.AssamblyVersionToolTip = assemblyVersionToolTip;
            form.AssamblyFolder = assemblyFolder;
            form.ConnectionString = ConnectionString;
            form.ConnStrResources = ConnStrResources;

            form.ModuleId = ModuleId;
            form.isModal = isModal;
            form.TestXmlPath = TestXmlPath;
            form.XmlPath = xmlPath;
            form.procID = procID;

            form.Logger.Info("Info App", "name: " + form.AssamblyName +
                "; version: " + form.AssamblyVersion +
                "; path: " + assemblyLocation +
                "; module: " + ModuleId +
                "; test xml path: " + TestXmlPath);

            form.Logger.Info("Info RunParameters", "server: " + server +
                "; database: " + database +
                "; username: " + username +
                "; password: " + (string.IsNullOrEmpty(password) ? "(empty)" : "***") +
                "; operateriid: " + operateriid.ToString());

            //set connection

            // ovdje disejblamo SetPermissions za Manager jer smo njemu ručno morali stavljati tj. ovdje ima malo utjecaja jer se permissionsi
            // okinu poslije incijalizacije kontrola i menija a nama treba prije
            if (!(form is Forms.Master.Manager))
                try
                {
                    Logic.Permissions.SetPermissions(form as Forms.Base.JupiterShared);
                }
                catch (Exception ex)
                {
                    Logger.Error("Starter > SetPermissions", ex.Message, ex);
                }

            try
            {
                VarsLayout varsLayout = new VarsLayout(form as Forms.Base.JupiterShared);
                varsLayout.SetLayout();
            }
            catch (Exception ex)
            {
                Logger.Error("Starter > SetLayout", ex.Message, ex);
            }

            //try
            //{
            //    string sqlText = "";
            //    Carpo.Core.PubVars.DmsExists = Carpo.Data.Get.Vars.GetDmsExists(form.ConnectionString, out sqlText);
            //    form.Logger.InfoData("data reader; GetDmsExists", sqlText, DateTime.Now);
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error("Starter > SetLayout > GetDmsExists", ex.Message, ex);
            //}

            if (form is Forms.Base.JupiterShared)
            {
                (form as Forms.Base.JupiterShared).DefinitionRoot = DefinitionRoot;
                (form as Forms.Base.JupiterShared).DefinitionForm = InitForm;
            }
            SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder();
            stringBuilder.DataSource = server;
            stringBuilder.InitialCatalog = database;
            stringBuilder.IntegratedSecurity = true;

            if (MenuID != null)
            {

                form.InitConnectionParameters(stringBuilder.ConnectionString, operateriid.ToString(), MenuID, SkinName, PaletteName);
                InitForm.Caption = CompanyName + " - " + InitForm.Caption;
                ImeFirme = CompanyName;
                PubVars.MenuID = MenuID;
            }
            Logger.InfoTest("RegisterForm p2", "učitavanje varijabili, kontrola, layout", startTime);
            form.Start(server, database, username, password, operateriid, parent, pTop, pLeft, path);

            if (form.IsDisposed) return;

            HWnd = (int)form.Handle;

            form.MainHandle = MainHandle;
            //form.Ikona = Ikona;
            form.ParentHandle = parent;

            //show form

            form.Start();

            //set caption after
            WCaption = InitForm.Caption;
        }

        private void AddAllVars(Dictionary<string, object> formVarDict, Dictionary<string, object> varDict)
        {
            foreach (var v in varDict)
            {
                formVarDict.Add(v.Key, v.Value);
            }
        }

        public abstract void RegisterForm();
    }
}
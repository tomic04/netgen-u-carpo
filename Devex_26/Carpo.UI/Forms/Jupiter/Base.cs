using Carpo.Core;
using Carpo.Structure.Models.Core;
using Carpo.UI.Connector.Native;
using Carpo.UI.Logic;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Carpo.UI.Forms.Jupiter
{
    public partial class Base : RibbonBase.RibbonBase
    {
        public bool IsMaster { get; set; } = false;
        public List<Structure.Models.JControl> ControlList { get; set; } = new List<Structure.Models.JControl>();
        public List<Structure.Models.Permission> PermissionList { get; set; } = new List<Structure.Models.Permission>();

        public string AssamblyName { get; set; }
        public string AssamblyVersion { get; set; }
        public string AssamblyVersionToolTip { get; set; }
        public string AssamblyFolder { get; set; }
        public Logger Logger { get; set; }

        public bool isModal { get; set; } = false;

        //public DataTable Layout { get; set; }

        public string TestXmlPath { get; set; } // TestShell xml putanja
        public string XmlPath { get; set; } // Xml putanja

        public Dictionary<string, object> VariablesForm { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, object> VariablesGlobal { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public static List<JCustomDef> CustomControlsElement { get; set; } = new List<JCustomDef>();

        #region JupiterParameters

        public int MainHandle { get; set; } //JupiterApp
        public int ParentHandle { get; set; } //JupiterApp panel
        public string ModuleId { get; set; } // asm:Carpo.Granula.Start:ModuleId

        public string WCaption { get; set; }
        public int HWnd { get; set; }

        public string Ikona { set { SetFormIcon(value); } } //ssMeni Ikona8

        //public int OperateriId { get; set; }
        public int procID { get; set; }

        public Jupiter.Base FormMaster { get; set; }

        internal string ConnectionString { get; set; }
        internal string ConnStrResources { get; set; }

        #endregion JupiterParameters

        #region JupiterStart

        public virtual void Start(string server, string database, string username, string password, int operateriid, int parent, short pTop, short pLeft, string path)
        {
            var startTime = DateTime.Now;
            BonusSkins.Register();
            SkinManager.EnableFormSkins();

            Permissions.PermissionsAreSet = false;

            InitLogger(Logger);
            if (this is Forms.Base.JupiterShared)
            {
                Forms.Base.JupiterShared form = this as Forms.Base.JupiterShared;
                Logger.Imginfo = form.bbiLog;
                form.bbiLog.EditValue = Properties.Resources.circle_gray_16;
                form.repPicEditLog.ContextMenuStrip = new ContextMenuStrip(); //onemogućuje desni klik
            }

            Core.PubVars.OperateriId = operateriid;
            string appName = AssamblyName + "." + ModuleId;
            Logger.InfoCore("Set AppName", appName);

            //ConnectionString = Carpo.Data.Constructor.SetConnectionString(server, database, username, password, appName);
            //ConnStrResources = Carpo.Data.Constructor.SetConnectionString(server, "JupiterResources", username, password, appName);

            ////VB6 connection string
            //Connection.ConnectionStringADO = string.Format(
            //        "Driver={0};Server={1};Database={2};Trusted_Connection=yes;Persist Security Info=true",
            //        "{SQL Server}", server, database);

            //Connection.ConnectionString = ConnectionString;
            //Connection.ConnStrResources = ConnStrResources;
            //Connection.DataBase = database;
            //Connection.Server = server;
            //Connection.Uid = username;
            //Connection.Pwd = password;

            Logger.InfoCore("Set ConnStr", ConnectionString);
            Logger.InfoCore("Set ConnStrRes", ConnStrResources);
            Logger.InfoTest("FormStart", "stvaranje forme", startTime);
        }

        public void Start()
        {
            BonusSkins.Register();
            SkinManager.EnableFormSkins();
            //prije prikaza pozicioniranje unutar parent kontrole
            SafeNativeMethods.SetParent(this.Handle, new IntPtr(this.ParentHandle));
            Logger.InfoCore("Set Parent (thisHande, parentHandle)", (this.Handle.ToString() + ", " + this.ParentHandle.ToString()));
            //this.StartPosition = FormStartPosition.CenterParent;
            //this.Location = new Point(0);

            //this.MaximumSize = new Size(1000,500);
            //this.WindowState = FormWindowState.Maximized;

            if (isModal == true)
                ShowDialog();
            else
            {
                Show();
                //this.Location = new Point(10, 10);
            }
        }

        #endregion JupiterStart

        public void InitLogger(Logger loger)
        {
            if (Logger == null)
            {
                if (loger != null)
                    Logger = loger;
                else
                    Logger = new Logger(Core.Log.LogLevel.Debug);

                Dictionary<string, Image> infoimages = new Dictionary<string, Image>();
                infoimages.Add("error", Properties.Resources.error_16);
                infoimages.Add("warning", Properties.Resources.warning_16);
                infoimages.Add("errorwarning", Properties.Resources.error_warning_16);
                infoimages.Add("valid", Properties.Resources.valid_16);

                Logger.Infoimages = infoimages;
            }
        }

        public Base()
        {
            InitializeComponent();
            //privremeno
            this.Icon = Properties.Resources.spinDev;
            Screen scrn = Screen.FromControl(this);
            //this.Location = new System.Drawing.Point(scrn.Bounds.Left, scrn.Bounds.Top);            
        }

        private void SetFormIcon(string iconStr)
        {
            try
            {
                Icon icon = Common.Helpers.Images.GetFormIcon(iconStr, ConnStrResources);
                this.Icon = icon;
                Logger.InfoCore("Set Icon", iconStr);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex.StackTrace);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (this.Owner == null)
            {
                try
                {
                    if (this.Visible == true)
                    {
                        SafeNativeMethods.SendMessage(new IntPtr(this.MainHandle), 2000, this.Handle, IntPtr.Zero);
                        base.OnFormClosed(e);
                    }
                }
                catch (Exception es) { }

                try
                {
                    if (procID > 0)
                    {
                        Process p = Process.GetProcessById(procID);
                        p.Kill();
                    }
                }
                catch { }
            }

            //Ne koristi!!!!! Samo kao dokaz oslobađanja memorije!
            //GC.Collect();
        }
    }
}
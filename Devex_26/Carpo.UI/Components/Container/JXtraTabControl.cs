using Carpo.Core;
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Forms.Base;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Carpo.UI.Components.Container
{
    public partial class JXtraTabControl : XtraTabControl, IJControl
    {

        public string TabNameHelper { get; set; }

        public Base.ComponentTabs ComponentExt { get; set; }

        public JTabs JTabsInit { get; set; }


        public bool RunActions { get; set; } = true;


        protected JTabs definition;


        public JXtraTabControl()
        {
            InitializeComponent();
        }

        public JXtraTabControl(JTabs definition)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(definition.Name))
                this.Name = definition.Name;

            this.definition = definition;

            Init();
        }


        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //form = (Forms.Base.JupiterShared)FindForm();
            //connectionString = form.ConnectionString;

            if (definition == null)
                if (this.JTabsInit != null)
                {
                    this.definition = this.JTabsInit;
                    ComponentExt = new Base.ComponentTabs(this, this.JTabsInit);
                    ComponentExt.Init();

                }
                else
                {
                    ComponentExt = new Base.ComponentTabs();
                }

            else
            {
                ComponentExt = new Base.ComponentTabs(this, definition);
                ComponentExt.Init();
            }
        }

        private void Init()
        {
            this.Dock = DockStyle.Fill;

            if (!string.IsNullOrEmpty(definition.TabsLocation))
                SetTabsLocation(definition.TabsLocation);
            if (!string.IsNullOrEmpty(definition.TabsOrientation))
                SetTabsOrientation(definition.TabsOrientation);
            
            this.AppearancePage.HeaderActive.Font = new System.Drawing.Font(this.AppearancePage.HeaderActive.Font, System.Drawing.FontStyle.Bold);
        }

        private void SetTabsLocation(string value)
        {
            switch (value)
            {
                case "left":
                    this.HeaderLocation = TabHeaderLocation.Left;
                    break;

                case "right":
                    this.HeaderLocation = TabHeaderLocation.Right;
                    break;

                case "bottom":
                    this.HeaderLocation = TabHeaderLocation.Bottom;
                    break;

                default:
                    this.HeaderLocation = TabHeaderLocation.Top;
                    break;
            }
        }

        private void SetTabsOrientation(string value)
        {
            switch (value)
            {
                case "vertical":
                    this.HeaderOrientation = TabOrientation.Vertical;
                    break;

                case "horizontal":
                    this.HeaderOrientation = TabOrientation.Horizontal;
                    break;

                default:
                    this.HeaderOrientation = TabOrientation.Default;
                    break;
            }
        }

        #region TabPagesBugWorkaround

        public void TabPagesBugWorkAround()
        {
            //****Microsoft bug sa tab kontrolom
            //****problem: http://stackoverflow.com/questions/943473/does-data-binding-work-on-invisible-control
            //****workaround: https://www.devexpress.com/Support/Center/Question/Details/T188373

            if (this == null || this.TabPages.Count == 0)
                return;

            foreach (XtraTabPage tabPage in this.TabPages)
            {
                TabPageWorkAround(tabPage);
            }

            //****ili prikazati drugi tabpage pa se vratiti na prvi
            //****
        }

        public void TabPagesBugWorkAround(XtraTabPage tabPage)
        {
            //****Microsoft bug sa tab kontrolom
            //****problem: http://stackoverflow.com/questions/943473/does-data-binding-work-on-invisible-control
            //****workaround: https://www.devexpress.com/Support/Center/Question/Details/T188373

            if (this == null || tabPage == null)
                return;

            TabPageWorkAround(tabPage);

            //****ili prikazati drugi tabpage pa se vratiti na prvi
            //****
        }

        private void TabPageWorkAround(XtraTabPage tabPage)
        {
            MethodInfo method = typeof(Control).GetMethod("CreateControl", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(tabPage, new object[] { true });
        }

        #endregion TabPagesBugWorkaround

        public XtraTabPage CreateTabPage(JTab definition)
        {            
            XtraTabPage tabPage = new XtraTabPage();
            if (!string.IsNullOrEmpty(definition.Name))
                tabPage.Name = Carpo.Common.Translator.TranslateString(definition.Name);
            if (!string.IsNullOrEmpty(definition.Caption))
                tabPage.Text = Carpo.Common.Translator.TranslateString(definition.Caption);

            if (!string.IsNullOrEmpty(definition.Enabled))
            {
                if (definition.Enabled.ToString() == "false")
                {
                    tabPage.PageEnabled = false;
                };
            }

            tabPage.Padding = new Padding(4);

            this.TabPages.Add(tabPage);

            return tabPage;
        }

        public object GetValue(string extension)
        {
            if (extension.ToLower() == "selectedtab")
                return this.SelectedTabPage.Name;
            return null;
        }

        public void SetEvents(object ev)
        {
            if (ev is List<JEvent>)
                ComponentExt.SetEvents(ev as List<JEvent>);
            else if (ev is JEvent)
                ComponentExt.SetEvents(ev as JEvent);
        }

        public void SetNotifications(JEventAction action, object value)
        {

            ComponentExt.SetNotify(action, value);
        }

        public void SetEnabled(bool enabled)
        {
            if (!string.IsNullOrEmpty(TabNameHelper))
            {
                int tabIndex;
                bool isNumerical = int.TryParse(TabNameHelper, out tabIndex);
                if (isNumerical)
                {
                    TabPages[tabIndex].Enabled = enabled;
                }
            }
        }

        public void SetVisibility(JEventAction action)
        {
            if (!string.IsNullOrEmpty(TabNameHelper))
            {
                int tabIndex;
                bool isNumerical = int.TryParse(TabNameHelper, out tabIndex);
                if (isNumerical)
                {
                    object valueObj = Carpo.UI.Logic.Parameters.GetValue(ComponentExt.form, action);
                    bool value;
                    if (Boolean.TryParse(valueObj.ToString(), out value))
                    {
                        TabPages[tabIndex].PageVisible = value;
                    }
                }
            }
        }

        public void RefreshValue()
        {
            throw new NotImplementedException();
        }

        public void SetAttribute(JEventAction action)
        {
            throw new NotImplementedException();
        }

        public object GetKeyCode()
        {
            throw new NotImplementedException();
        }
    }
}
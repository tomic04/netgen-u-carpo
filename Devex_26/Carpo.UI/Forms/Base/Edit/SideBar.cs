using Carpo.Core;
using Carpo.Structure.Models.Core;
using Carpo.Structure.Models.Core.Base;
using Carpo.UI.Components.Container;
using Carpo.UI.Logic;
using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpo.UI.Forms.Edit
{
    public partial class SideBar : Base.JupiterInfo
    {

        JTabs tabsDefinition;

        public SideBar()
        {
            InitializeComponent();
        }

        public SideBar(JRoot definitionRoot, JForm definition, Logger logger, string connectionString, string moduleId) : base(definitionRoot, definition, logger, connectionString, moduleId)
        {
            InitializeComponent();
        }

        public SideBar(Forms.Base.JupiterShared form, JForm definition) : base(form, definition)
        { 
            InitializeComponent();

            tabsDefinition = definition.InfoControls?.Tabs;
            tabs.JTabsInit = tabsDefinition;

            this.ControlList.Add(new Structure.Models.JControl() { ControlName = definition.Name, Control = tabs, ControlType = tabs.GetType(), IsNameDefinied = true, SaveValueSession = false, SaveValueApp = false });
            SetNotification(tabsDefinition, tabs);

        }

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode)
                base.OnLoad(e);
        }

        private void SideBar_Shown(object sender, EventArgs e)
        {
            tabs.SelectedTabPageIndex = 0;

            if (TabPagesEnabled.Count > 0 || TabPagesVisible.Count > 0)
            {
                for (int i = 0; i < tabs.TabPages.Count; i++)
                {
                    if (TabPagesVisible.Contains(i))
                    {
                        tabs.TabPages[i].PageVisible = false;
                    }
                    else if (TabPagesEnabled.Contains(i))
                    {
                        tabs.TabPages[i].PageEnabled = false;
                    }
                }
            }

            FormShown = true;
            pnlSpace.Visible = true;
            pnlSpace.Width=tabs.PageClientBounds.X;

            tabs.RunActions = true;
        }

        private void tabs_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (FormShown == false)
                return;

            FormsVar.SetVariable(this, "SelectedTabIndex", tabs.SelectedTabPageIndex);
        }

        private void SetNotification(JModelEditorDataBase definition, System.Windows.Forms.Control control)
        {
            if (definition == null)
                return;

            if (definition.Event != null && definition.Event.Count > 0)
                this.EventList.Add(new Structure.Models.ControlEvents() { Control = control, Events = definition.Event });
        }

    }
}
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentTabs : ComponentBase
    {
        public System.Type ValueFieldColumnType { get; set; }
        private readonly XtraTabControl jTabsControl;
        public string[] DataColumns { get; set; } = new string[] { };

        public ComponentTabs() : base()
        {
        }

        public ComponentTabs(Control control, object def) : base(control, def)
        {
            jTabsControl = control as XtraTabControl;
        }

        internal virtual void Init()
        {
        }




        public void Populate(bool force)
        {
            
        }

    }
}
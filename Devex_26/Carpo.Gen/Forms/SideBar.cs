using Carpo.Core;
using Carpo.Structure.Models.Core;
using System;

namespace Carpo.Gen.Forms
{
    public partial class SideBar : UI.Forms.Edit.SideBar
    {
        public SideBar()
        {
            InitializeComponent();
        }

        public SideBar(JRoot definitionRoot, JForm jForm, Logger logger, string connectionString, string moduleId) : base(definitionRoot, jForm, logger, connectionString, moduleId)
        {
            //DevExpress.Skins.SkinManager.EnableFormSkins();
            //DevExpress.Skins.SkinManager.EnableMdiFormSkins();
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            //DeserializeXml();
            base.OnLoad(e);
        }
    }
}
using Carpo.Core;
using Carpo.Structure.Models.Core;
using System;

namespace Carpo.Gen.Forms
{
    public partial class Manager : UI.Forms.Master.Manager
    {//asd

        public Manager(JRoot definitionRoot, JForm definition, Logger logger, string connectionString, string moduleId) : base(definitionRoot, definition, logger, connectionString, moduleId)
        {
            ////Enable title bar skinning
            //DevExpress.Skins.SkinManager.EnableFormSkins();

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            //DeserializeXml();
            base.OnLoad(e);
        }
    }
}
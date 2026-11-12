using Carpo.Core;
using Carpo.Structure.Models.Core;
using System;

namespace Carpo.UI.Forms.Edit
{
    public partial class Standard : Base.JupiterEditor
    {
        public Standard()
        {
            InitializeComponent();
        }

        //direktan poziv
        public Standard(JRoot definitionRoot, JForm definition, Logger logger, string connectionString, string moduleId) : base(definitionRoot, definition, logger, connectionString, moduleId)
        { InitializeComponent(); }

        public Standard(Forms.Base.JupiterShared form, JForm definition) : base(form, definition)
        { InitializeComponent(); }

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode)
                base.OnLoad(e);
        }
    }
}
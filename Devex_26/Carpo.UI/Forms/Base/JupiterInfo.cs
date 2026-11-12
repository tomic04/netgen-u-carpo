using Carpo.Core;
using Carpo.Structure.Models.Core;
using DevExpress.XtraTab;
using System.Collections.Generic;

namespace Carpo.UI.Forms.Base
{
    public partial class JupiterInfo : JupiterEditor
    {
        public bool FormShown { get; set; } = false;

        public List<int> TabPagesEnabled = new List<int>();
        public List<int> TabPagesVisible = new List<int>();


        public JupiterInfo()
        {
            InitializeComponent();
        }

        public JupiterInfo(JRoot definitionRoot, JForm definition, Logger logger, string connectionString, string moduleId) : base(definitionRoot, definition, logger, connectionString, moduleId)
        { InitializeComponent(); }

        public JupiterInfo(Forms.Base.JupiterShared form, JForm definition) : base(form, definition)
        { InitializeComponent(); }

    }
}
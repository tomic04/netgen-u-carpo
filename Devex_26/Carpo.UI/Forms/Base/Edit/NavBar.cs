using Carpo.Core;
using Carpo.Structure.Models.Core;

namespace Carpo.UI.Forms.Edit
{
    public partial class NavBar : Forms.Edit.Splitter
    {
        public NavBar()
        {
            InitializeComponent();
        }

        //direktan poziv
        public NavBar(JRoot definitionRoot, JForm definition, Logger logger, string connectionString, string moduleId) : base(definitionRoot, definition, logger, connectionString, moduleId)
        { InitializeComponent(); }

        public NavBar(Forms.Base.JupiterShared form, JForm definition) : base(form, definition)
        { InitializeComponent(); }
    }
}
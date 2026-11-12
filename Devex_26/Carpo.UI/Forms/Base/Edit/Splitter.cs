using Carpo.Core;
using Carpo.Structure.Models.Core;

namespace Carpo.UI.Forms.Edit
{
    public partial class Splitter : Forms.Base.JupiterEditor
    {
        public Splitter()
        {
            InitializeComponent();
        }

        //direktan poziv
        public Splitter(JRoot definitionRoot, JForm definition, Logger logger, string connectionString, string moduleId) : base(definitionRoot, definition, logger, connectionString, moduleId)
        { InitializeComponent(); }

        public Splitter(Forms.Base.JupiterShared form, JForm definition) : base(form, definition)
        { InitializeComponent(); }
    }
}
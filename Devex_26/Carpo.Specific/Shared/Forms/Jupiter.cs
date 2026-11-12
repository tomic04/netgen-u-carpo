using Carpo.Core;
using System.Windows.Forms;

namespace Carpo.Specific.Shared.Forms
{
    public partial class Jupiter : DevExpress.XtraEditors.XtraForm
    {
        public Logger Logger { get; set; }

        public Jupiter()
        {
            InitializeComponent();
        }

        private void Jupiter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                if (Logger != null)
                {
                    using (Core.Log.Log frm = new Core.Log.Log(Logger))
                    {
                        frm.ShowDialog(this);
                    }
                }
            }
        }
    }
}
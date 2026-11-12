using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace Carpo.Common.Helpers
{
    public class Style
    {
        public static void SetStyleController(Control parent, StyleController styleController)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is BaseControl)
                {
                    (control as BaseControl).StyleController = styleController;
                }
                SetStyleController(control, styleController);
            }
        }
    }
}
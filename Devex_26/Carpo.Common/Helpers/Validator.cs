using DevExpress.XtraEditors.DXErrorProvider;
using System.Windows.Forms;

namespace Carpo.Common.Helpers
{
    public static class Validator
    {
        public class MyConditionValidationRule : ConditionValidationRule
        {
            public override bool CanValidate(Control control)
            {
                return base.CanValidate(control) && control.Enabled;
            }
        }
    }
}
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.Structure.Models.Core.Base;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentCheckEdit : ComponentBase
    {
        private CheckEdit checkEdit;

        private new JModelEditorCheck definition;

        public ComponentCheckEdit() : base()
        {
        }

        public ComponentCheckEdit(Control control, object def) : base(control, def)
        {
            definition = def as JModelEditorCheck;

            if (!(control is CheckEdit))
                return;

            checkEdit = (control as CheckEdit);

            Init();
            SetValue(checkEdit as IJEditor);
        }

        private void Init()
        {
            checkEdit.AutoSizeInLayoutControl = true;
            checkEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;

            if (!string.IsNullOrEmpty(definition.Caption))
                checkEdit.Text = definition.Caption;
            else
                checkEdit.Text = "";

            if (definition is JRadioEdit)
            {
                checkEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
                int groupIndex = 0;
                if (definition.GroupIndex != null)
                    groupIndex = (int)definition.GroupIndex;

                checkEdit.Properties.RadioGroupIndex = groupIndex;
            }

            if (definition.Checked == true)
                checkEdit.Checked = true;

            checkEdit.EnterMoveNextControl = true;
        }
    }
}
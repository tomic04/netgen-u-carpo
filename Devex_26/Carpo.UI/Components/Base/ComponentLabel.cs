using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentLabel : ComponentBase
    {
        private LabelControl labelControl;
        private new JLabelEdit definition;

        public ComponentLabel() : base()
        {
        }

        public ComponentLabel(Control control, object def) : base(control, def)
        {
            definition = def as JLabelEdit;
            labelControl = (LabelControl)control;
            Init();
            SetValue(labelControl as IJEditor);
        }

        private void Init()
        {
            labelControl.AllowHtmlString = true;

            string labelText = "";
            if (!string.IsNullOrEmpty(definition.Value))
            {
                using (Parser parser = new Parser(form, null))
                {
                    string valueStr = definition.Value;
                    object value = parser.GetLogicPartValue(valueStr);

                    if (value != null)
                        labelText = value.ToString().Replace("\\n", "\r\n");
                    else
                        labelText = definition.Value.Replace("\\n", "\r\n");
                }
                labelText = definition.Value;
            }

            SetTextPosition();
            SetMultiLine();
            //  SetText();
        }

        private void SetTextPosition()
        {
            if (string.IsNullOrEmpty(definition.TextPosition))
                return;

            if (form is Forms.Base.JupiterEditor)
            {
                foreach (LayoutControl layoutControl in (form as Forms.Base.JupiterEditor).LayoutControlList)
                {
                    LayoutControlItem item = layoutControl.GetItemByControl(control);
                    if (item != null)
                    {
                        if (definition.TextPosition == "right")
                            item.ControlAlignment = System.Drawing.ContentAlignment.MiddleRight;
                        else if (definition.TextPosition == "center")
                            item.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;

                        break;
                    }
                }
            }
        }

        private void SetMultiLine()
        {
            if (definition.MultiLine == true)
            {
                labelControl.AutoSizeMode = LabelAutoSizeMode.Vertical;

                if (definition.TextPosition == "right")
                    labelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
        }

        //public void SetText()
        //{
        //    if (!string.IsNullOrEmpty(definition.Caption))
        //    {
        //        labelControl.Text =definition.Caption ;
        //    }
        //string prefixText = "", sufixText = "";

        //if (!string.IsNullOrEmpty(definition.Prefix))
        //    prefixText = definition.Prefix;
        //if (!string.IsNullOrEmpty(definition.Sufix))
        //    sufixText = definition.Sufix;

        //if (!string.IsNullOrEmpty(definition.Format))
        //    value = Common.Helpers.Formats.FormatLabel(definition.Format, value);

        //labelControl.Text = prefixText + value + sufixText;
    }
}
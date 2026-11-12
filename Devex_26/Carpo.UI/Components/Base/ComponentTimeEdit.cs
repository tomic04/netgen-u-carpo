using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Components.Editor;
using DevExpress.XtraEditors.Mask;
using System;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentTimeEdit : ComponentBase
    {
        private JTimeEditControl timeEdit;
        private new JTimeEdit definition;

        public ComponentTimeEdit() : base()
        {
        }

        public ComponentTimeEdit(Control control, object def) : base(control, def)
        {
            definition = def as JTimeEdit;
            timeEdit = control as JTimeEditControl;
            Init();
            SetValue(timeEdit as IJEditor);
        }

        public void SetTimeValue()
        {
            if (!string.IsNullOrEmpty(definition.Value))
            {
                if (definition.Value.ToLower() == "now")
                {
                    DateTime time = (DateTime)Common.Helpers.Conversion.Parse("datetime", "nowtime");
                    timeEdit.EditValue = time;
                    return;
                }
                timeEdit.EditValue = definition.Value;
            }
        }

        public void SetEnabled(bool enabled)
        {
            timeEdit.Enabled = enabled;
        }

        private void Init()
        {
            SetTimeValue();

            timeEdit.Properties.Mask.MaskType = MaskType.DateTime;
            timeEdit.Properties.Mask.EditMask = "HH:mm";
            timeEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            timeEdit.EditValue = String.Format("{0:00}:{1:00}:{2:00}", timeEdit.Time.Hour, timeEdit.Time.Minute, timeEdit.Time.Second);

            timeEdit.EnterMoveNextControl = true;
            timeEdit.Properties.AllowDropDownWhenReadOnly = DevExpress.Utils.DefaultBoolean.False;
            timeEdit.Properties.AllowFocused = false;

            SetFormat();

            //SetEvents(definition.Event);
        }

        public void SetFormat()
        {
            try
            {
                if (!string.IsNullOrEmpty(definition.TabIndex)) // neće vrijediti ako na layoutu nije isključen 'autotaborder'
                {
                    int n;
                    bool isNumeric = int.TryParse(definition.TabIndex, out n);
                    if (isNumeric)
                    {
                        if (timeEdit != null)
                            timeEdit.TabIndex = n;
                        else
                            timeEdit.TabIndex = n;
                    }
                }

                if (!string.IsNullOrEmpty(definition.Format))
                {
                    if ((definition.Format.ToLower() == "hh:mm:ss") || definition.Format.ToLower() == "hh:mm")
                    {
                        timeEdit.Properties.Mask.EditMask = definition.Format;
                        timeEdit.Properties.Mask.MaskType = MaskType.DateTime;
                        timeEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
                    }
                    else
                        form.Logger.Warn("TimeEdit - SetFormat()", "Neispravan format atributa format - '" + definition.Name + "'");
                }
            }
            catch
            {
                form.Logger.Warn("TimeEdit - SetFormat()", "Neispravan format vrijednosti kontrole '" + definition.Name + "'.");
            }
        }
    }
}
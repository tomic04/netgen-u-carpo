using Carpo.Structure.Models.Core;
using System;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentButtonText : ComponentBase
    {
        private JButtonEditControl jButtonTextControl;

        public ComponentButtonText() : base()
        {
        }

        public ComponentButtonText(Control control, object def) : base(control, def)
        {
            definition = def as JButtonTextEdit;
            jButtonTextControl = (JButtonEditControl)control;

            SetEvents();
            Init();
            SetValue();
        }

        private void SetEvents()
        {
            jButtonTextControl.cmd.Click += (s, e) =>
            {
                jButtonTextControl.cmd.Visible = false;
                jButtonTextControl.txt.Visible = true;

                jButtonTextControl.txt.Focus();
                jButtonTextControl.txt.SelectAll();
            };
            jButtonTextControl.txt.Leave += (s, e) =>
            {
                jButtonTextControl.cmd.Visible = true;
                jButtonTextControl.txt.Visible = false;
                jButtonTextControl.cmd.Text = jButtonTextControl.txt.Text.ToString();
            };
        }

        private void Init()
        {
            if (!string.IsNullOrEmpty(definition.Name))
                jButtonTextControl.Name = definition.Name;
            else
                jButtonTextControl.Name = Guid.NewGuid().ToString();

            jButtonTextControl.TabStop = false;

            jButtonTextControl.txt.EnterMoveNextControl = true;

            Common.Helpers.Formats.FormatTextEdit(definition.Format, jButtonTextControl.txt.Properties);

            if (definition.FontSizeDelta != null)
            {
                jButtonTextControl.txt.Properties.Appearance.FontSizeDelta = (int)definition.FontSizeDelta;
                jButtonTextControl.cmd.Appearance.FontSizeDelta = (int)definition.FontSizeDelta;
            }

            if (definition.FontBold == true)
            {
                jButtonTextControl.txt.Properties.Appearance.Font = new System.Drawing.Font(jButtonTextControl.txt.Properties.Appearance.Font, System.Drawing.FontStyle.Bold);
                jButtonTextControl.cmd.Appearance.Font = new System.Drawing.Font(jButtonTextControl.cmd.Appearance.Font, System.Drawing.FontStyle.Bold);
            }
            jButtonTextControl.cmd.Height = jButtonTextControl.txt.Height;

            jButtonTextControl.cmd.Visible = true;
            jButtonTextControl.txt.Visible = false;
        }

        public void SetValue()
        {
            if (!string.IsNullOrEmpty(definition.DataValue))
            {
                Logic.Data.Core data = new Logic.Data.Core();
                data.ConnectionString = form.ConnectionString;
                data.CurrentForm = form;

                string tableName = Logic.Parser.ParseParameterGetName(definition.DataValue.Replace("#", "~"), "~");

                if (form.DataList.ContainsKey(tableName))
                {
                    object value = data.GetDataValue(definition.DataValue);
                    SetEditValue(value);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(definition.Value))
            {
                SetEditValue(definition.Value);
            }
        }

        internal void SetEditValue(object value)
        {
            jButtonTextControl.txt.EditValue = value;
            jButtonTextControl.cmd.Text = jButtonTextControl.txt.Text;
        }
    }
}
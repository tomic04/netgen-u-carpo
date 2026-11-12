using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.XtraEditors;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentHypeLinkEdit : ComponentBase
    {
        private HyperLinkEdit hyperlinkEdit;

        private new JTextEdit definition;

        public ComponentHypeLinkEdit() : base()
        {
        }

        public ComponentHypeLinkEdit(Control control, object def) : base(control, def)
        {
            definition = def as JHyperlinkEdit;

            if (!(control is HyperLinkEdit))
                return;

            hyperlinkEdit = (control as HyperLinkEdit);

            SetEvents();
            Init();

            SetValue(hyperlinkEdit as IJEditor);
        }

        private void Init()
        {
            if (definition.FontSizeDelta != null)
                hyperlinkEdit.Properties.Appearance.FontSizeDelta = (int)definition.FontSizeDelta;

            hyperlinkEdit.Properties.Appearance.ForeColor = Common.Helpers.Colors.HtmlToColor(definition.FontColor, hyperlinkEdit.Properties.Appearance.ForeColor);

            SetFormat();

            if (!string.IsNullOrEmpty(definition.ErrorText))
                hyperlinkEdit.ErrorText = definition.ErrorText;

            if (definition.NullText != null)
            {
                hyperlinkEdit.Properties.NullValuePrompt = definition.NullText;
                hyperlinkEdit.Properties.NullValuePromptShowForEmptyValue = true;
                hyperlinkEdit.Properties.ShowNullValuePromptWhenFocused = true;
            }
            if (definition.AllowNull != false)
                hyperlinkEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;

            if (!string.IsNullOrEmpty(definition.MaxLength))
            {
                using (Parser parser = new Parser(form, null))
                {
                    object value = parser.GetLogicPartValue(definition.MaxLength);
                    int n;
                    bool isNumeric = int.TryParse(value.ToString(), out n);
                    if (isNumeric)
                        hyperlinkEdit.Properties.MaxLength = n;
                }
            }

            SetTextPosition();
        }

        private void SetEvents()
        {
            hyperlinkEdit.OpenLink += (s, e) =>
            {
                const string mailPrefix = "mailto:";
                if (definition.Type == "email" && !e.EditValue.ToString().ToLower().StartsWith(mailPrefix))
                {
                    e.EditValue = mailPrefix + e.EditValue.ToString();
                }
            };
        }

        private void SetFormat()
        {
            if (!string.IsNullOrEmpty(definition.FormatType) && !string.IsNullOrEmpty(definition.Format))
            {
                hyperlinkEdit.Properties.Mask.MaskType = Common.Helpers.Strings.GetMaskTypeByString(definition.FormatType);
                hyperlinkEdit.Properties.Mask.ShowPlaceHolders = false;
                if (!string.IsNullOrEmpty(definition.AutoComplete))
                    hyperlinkEdit.Properties.Mask.AutoComplete = Common.Helpers.Strings.GetMaskAutoCompleteType(definition.AutoComplete);

                hyperlinkEdit.CausesValidation = false;

                var control = form.ControlList.Where(x => x.ControlName == hyperlinkEdit.Name).FirstOrDefault();
                if (control != null)
                    control.DoValidate = true;

                hyperlinkEdit.Properties.Mask.UseMaskAsDisplayFormat = true;

                try
                {
                    if (!string.IsNullOrEmpty(definition.Format))
                        hyperlinkEdit.Properties.Mask.EditMask = definition.Format;
                }
                catch { }
            }
            else if (!string.IsNullOrEmpty(definition.Format)) //pretpostavka da je numerička vrijednost (npr. n2)
            {
                Common.Helpers.Formats.FormatTextEdit(definition.Format, hyperlinkEdit.Properties);
            }
        }

        private void SetTextPosition()
        {
            if (!string.IsNullOrEmpty(definition.TextPosition))
            {
                if (definition.TextPosition == "left")
                    hyperlinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                else if (definition.TextPosition == "right")
                    hyperlinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                else if (definition.TextPosition == "center")
                    hyperlinkEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
        }
    }
}
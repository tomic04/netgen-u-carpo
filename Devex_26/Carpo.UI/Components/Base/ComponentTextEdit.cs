using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.XtraEditors;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentTextEdit : ComponentBase
    {
        private TextEdit textEdit;

        private new JTextEdit definition;

        public ComponentTextEdit() : base()
        {
        }

        public ComponentTextEdit(Control control, object def) : base(control, def)
        {
            definition = def as JTextEdit;

            if (!(control is TextEdit))
                return;

            textEdit = (control as TextEdit);

            SetEvents();
            Init();

            SetValue(textEdit as IJEditor);
        }

        private void Init()
        {
            //if (!(control is ButtonEdit) && !(control is MemoEdit))
            //    textEdit.EnterMoveNextControl = true;
            //else if (control is MemoEdit && textEdit.EnterMoveNextControl == true)
            //    textEdit.EnterMoveNextControl = true;

            if (definition.FontSizeDelta != null)
                textEdit.Properties.Appearance.FontSizeDelta = (int)definition.FontSizeDelta;

            //if (definition.FontBold == true)
            //    textEdit.Properties.Appearance.Font = new System.Drawing.Font(textEdit.Properties.Appearance.Font, System.Drawing.FontStyle.Bold);

            //if (!string.IsNullOrEmpty(definition.FontColor))
            //    textEdit.Properties.Appearance.ForeColor = Common.Helpers.Colors.HtmlToColor(definition.FontColor, textEdit.Properties.Appearance.ForeColor);

            SetFormat();

            if (!string.IsNullOrEmpty(definition.ErrorText))
                textEdit.ErrorText = definition.ErrorText;

            if (definition.NullText != null)
            {
                textEdit.Properties.NullValuePrompt = definition.NullText;
                textEdit.Properties.NullValuePromptShowForEmptyValue = true;

                //ako je true; izgleda da je dx bug
                //jer kad se nalazi unutar layout kontrole onda fokusom preko tab tipke
                //ostavlja prompt vrijednost
                if (textEdit is MemoEdit)
                    textEdit.Properties.ShowNullValuePromptWhenFocused = false;
            }
            if (definition.AllowNull != false)
                textEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;

            if (!string.IsNullOrEmpty(definition.MaxLength))
            {
                using (Parser parser = new Parser(form, null))
                {
                    object value = parser.GetLogicPartValue(definition.MaxLength);
                    int n;
                    bool isNumeric = int.TryParse(value.ToString(), out n);
                    if (isNumeric)
                        textEdit.Properties.MaxLength = n;
                }
            }

            SetTextPosition();
        }

        private void SetEvents()
        {
            textEdit.Enter += (s, e) =>
            {
                // T.A. 03.01.2019. - forsiram selektiranje samo date dio kontrole -> vidi HD 293
                if (definition.FormatType != null && definition.FormatType.ToLower().Contains("datetime"))
                {
                    // Same as ComponentDateEdit: no SendKeys (needs message pump), place caret at start directly.
                    try
                    {
                        textEdit.BeginInvoke(new MethodInvoker(() =>
                        {
                            try
                            {
                                textEdit.SelectionStart = 0;
                                textEdit.SelectionLength = 0;
                            }
                            catch { }
                        }));
                    }
                    catch { }
                    return;
                }

                if (!(textEdit is MemoEdit))
                {
                    textEdit.BeginInvoke(new MethodInvoker(() =>
                    {
                        textEdit.SelectionStart = 0;
                        textEdit.SelectionLength = textEdit.Text.Length;
                    }));
                }
                else
                {
                    textEdit.BeginInvoke(new MethodInvoker(() =>
                    {
                        textEdit.SelectionStart = textEdit.Text.Length;
                        textEdit.SelectionLength = 0;
                    }));
                }
            };

            if (!string.IsNullOrEmpty(definition.Format) && definition.Format.StartsWith("n"))
                createNumericTextSelectionEvent();
        }

        private void createNumericTextSelectionEvent()
        {
            // kada je numerički input format n..., i selektiran je cijeli text (s tabom ulazim u kontrolu ili ručno),
            // želim selektirati cjelobrojnu vrijednost s tipkom left i decimalnu vrijednost s tipkom right radi brže promjene
            var ind = false;

            textEdit.KeyDown += (s, e) =>
            {
                if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) && textEdit.SelectionLength == textEdit.Text.Length)
                    ind = true;
            };

            textEdit.KeyUp += (s, e) =>
            {
                if (!ind)
                    return;

                // radim ovo u keyup eventu jer u keydownu još uvijek odrađuje svoj default event - pomicanje kursora.
                // Ne funkcionira osim na ovaj način.
                if (e.KeyCode == Keys.Left)
                {
                    textEdit.SelectionStart = 0;
                    textEdit.SelectionLength = textEdit.Text.IndexOf(",");
                    ind = false;
                }
                else if (e.KeyCode == Keys.Right)
                {
                    textEdit.SelectionStart = textEdit.Text.IndexOf(",") + 1;
                    textEdit.SelectionLength = textEdit.Text.Length - textEdit.Text.IndexOf(",");
                    ind = false;
                }
            };
        }

        public void SetFormat()
        {
            if (!string.IsNullOrEmpty(definition.FormatType) && !string.IsNullOrEmpty(definition.Format))
            {
                textEdit.Properties.Mask.MaskType = Common.Helpers.Strings.GetMaskTypeByString(definition.FormatType);
                textEdit.Properties.Mask.ShowPlaceHolders = false;
                if (!string.IsNullOrEmpty(definition.AutoComplete))
                    textEdit.Properties.Mask.AutoComplete = Common.Helpers.Strings.GetMaskAutoCompleteType(definition.AutoComplete);

                textEdit.CausesValidation = false;

                //if (definition.AllowNull == true) textEdit.Properties.MaskSettings.Set("allowBlankInput", true);

                var control = form.ControlList.Where(x => x.ControlName == textEdit.Name).FirstOrDefault();
                if (control != null)
                    control.DoValidate = true;

                textEdit.Properties.Mask.UseMaskAsDisplayFormat = true;

                try
                {
                    if (!string.IsNullOrEmpty(definition.Format))
                        textEdit.Properties.Mask.EditMask = definition.Format;
                }
                catch { }
            }
            else if (!string.IsNullOrEmpty(definition.Format)) //pretpostavka da je numerička vrijednost (npr. n2)
            {
                // DevEx bug - ako je format "n0" kontrola ne može formatirati decimalne vrijednosti sa zarezom
                if (definition.Format == "n0" && textEdit.EditValue != null)
                    textEdit.EditValue = textEdit.EditValue.ToString().Replace(",", ".");

                Common.Helpers.Formats.FormatTextEdit(definition.Format, textEdit.Properties);
            }
        }

        private void SetTextPosition()
        {
            if (!string.IsNullOrEmpty(definition.TextPosition))
            {
                if (definition.TextPosition == "left")
                    textEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                else if (definition.TextPosition == "right")
                    textEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                else if (definition.TextPosition == "center")
                    textEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
        }
    }
}
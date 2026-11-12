using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentDateEdit : ComponentBase
    {
        private bool? allowNull;
        private bool? allownullafterclear;

        DateTime? dateBeforeNull = null;


        private JDateEditControl dateEditControl;
        private new JDateEdit definition;

        public ComponentDateEdit() : base()
        {
        }

        public ComponentDateEdit(Control control, object def) : base(control, def)
        {
            definition = def as JDateEdit;
            dateEditControl = control as JDateEditControl;
            Init();
            SetValue(dateEditControl as IJEditor);
        }

        private void Init()
        {
            DateTime startTime = DateTime.Now;
            allowNull = definition.AllowNull;
            allownullafterclear = definition.AllowNulAfterClear;

            using (Parser parser = new Parser(form, null))
            {
                if (!string.IsNullOrEmpty(definition.MinValue))
                {
                    object valueMin = parser.GetLogicPartValue(definition.MinValue);
                    if (valueMin != null)
                        dateEditControl.Properties.MinValue = (DateTime)Common.Helpers.Conversion.Parse("date", valueMin);
                }
                if (!string.IsNullOrEmpty(definition.MaxValue))
                {
                    object valueMax = parser.GetLogicPartValue(definition.MaxValue);
                    if (valueMax != null)
                        dateEditControl.Properties.MaxValue = (DateTime)Common.Helpers.Conversion.Parse("date", valueMax);
                }
                if (!(string.IsNullOrEmpty(definition.ShowWeekNumbers)) && definition.ShowWeekNumbers.ToString().ToLower() == "true")
                    dateEditControl.Properties.ShowWeekNumbers = true;

                if (!(string.IsNullOrEmpty(definition.WeekNumberRule)))
                {
                    switch (definition.WeekNumberRule.ToString().ToLower())
                    {
                        case "firstday":
                            dateEditControl.Properties.WeekNumberRule = DevExpress.XtraEditors.Controls.WeekNumberRule.FirstDay;
                            break;

                        case "firstfourdayweek":
                            dateEditControl.Properties.WeekNumberRule = DevExpress.XtraEditors.Controls.WeekNumberRule.FirstFourDayWeek;
                            break;

                        case "firstfullweek":
                            dateEditControl.Properties.WeekNumberRule = DevExpress.XtraEditors.Controls.WeekNumberRule.FirstFullWeek;
                            break;
                    }
                }

                dateEditControl.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            }

            dateEditControl.EnterMoveNextControl = true;
            dateEditControl.Properties.AllowDropDownWhenReadOnly = DevExpress.Utils.DefaultBoolean.False;
            dateEditControl.Properties.AllowFocused = false;
            dateEditControl.Properties.ShowClear = false;
            dateEditControl.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;

            if (allownullafterclear == true) 
            {
                dateEditControl.Properties.ShowClear = true;
            }

            if (allowNull == true)
            {
                SetButtons();
                SetCheckStyle();
            }
            SetSelfEvents();
            //SetEvents(definition.Event);
            form.Logger.InfoData("Init DateEdit ", definition.Name, startTime);
        }

        private void SetSelfEvents()
        {
            // T.A. 03.01.2019. - forsiram selektiranje samo date dio kontrole -> vidi HD 293
            // SendKeys.Send needs an active message pump and throws InvalidOperationException when focus is
            // assigned outside of it (kills the granule); positioning the caret directly has the same effect.
            dateEditControl.Enter += (s, e) =>
            {
                try
                {
                    dateEditControl.BeginInvoke(new MethodInvoker(() =>
                    {
                        try
                        {
                            dateEditControl.SelectionStart = 0;
                            dateEditControl.SelectionLength = 0;
                        }
                        catch { }
                    }));
                }
                catch { }
            };

            if (allowNull == true)
            {
                dateEditControl.ButtonClick += (s, e) =>
                {
                    ButtonClick(s, e);
                };
                dateEditControl.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Space)
                    {
                        SetCheck(null);
                        if (dateEditControl.Tag != null && (bool)dateEditControl.Tag == true)
                            dateEditControl.Select();
                        dateEditControl.Focus();
                    }
                };

                if (!string.IsNullOrEmpty(definition.Checked))
                {
                    string parsedCheckedValue;

                    using (Parser parser = new Parser(form, null))
                        parsedCheckedValue = parser.GetLogicPartValue(definition.Checked).ToString();

                    if (parsedCheckedValue.ToLower() == "false")
                        SetCheck(true);
                }
            }
        }

        public override void SetEvents(List<JEvent> events)
        {
            base.SetEvents(events);
        }

        private void SetCheckStyle()
        {
            string SkinElementName;
            DevExpress.Skins.Skin currentSkin;

            currentSkin = DevExpress.Skins.CommonSkins.GetSkin(dateEditControl.LookAndFeel);
            SkinElementName = DevExpress.Skins.CommonColors.DisabledControl;
            string ssSkinElementName = DevExpress.Skins.CommonColors.DisabledText;
            Color disabledColor = currentSkin.Colors[SkinElementName];
            Color disabledForeColor = currentSkin.Colors[ssSkinElementName];

            dateEditControl.Properties.AppearanceReadOnly.BackColor = disabledColor;
            dateEditControl.Properties.AppearanceReadOnly.ForeColor = disabledForeColor;

            SetCheck(false); //init set
        }

        private void SetButtons()
        {
            DevExpress.XtraEditors.Controls.EditorButton button = new DevExpress.XtraEditors.Controls.EditorButton();
            button.IsDefaultButton = false;
            button.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph;
            button.IsLeft = true;

            dateEditControl.Properties.Buttons.Insert(0, button);
            dateEditControl.Properties.ActionButtonIndex = 1;
            //dateEditControl.Properties.Buttons[0].Enabled = false;
        }

        private void ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
            {
                SetCheck(null);
            }
        }

        private void SetCheck(bool? tagForce)
        {
            if (tagForce == true || (dateEditControl.Tag != null && (bool)(dateEditControl.Tag)))
            {
                dateEditControl.Properties.Buttons[0].Image = Properties.Resources.unchecked_16;
                dateEditControl.Tag = false;
                dateEditControl.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                dateEditControl.ReadOnly = true;
                dateBeforeNull = (DateTime?)dateEditControl.EditValue;
                dateEditControl.EditValue = null;
            }
            else
            {
                dateEditControl.Properties.Buttons[0].Image = Properties.Resources.checked_16;
                dateEditControl.Tag = true;
                dateEditControl.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                dateEditControl.ReadOnly = false;
                dateEditControl.EditValue = dateBeforeNull;
            }
        }

        public void SetEnabled(bool enabled)
        {
            if (allowNull == true && (dateEditControl.Tag.ToString() != enabled.ToString()))
                SetCheck(!enabled);
            else
                dateEditControl.Enabled = enabled;
        }
    }
}
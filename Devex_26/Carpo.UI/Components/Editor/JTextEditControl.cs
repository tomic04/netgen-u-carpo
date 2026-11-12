using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using DevExpress.LookAndFeel;
using DevExpress.Office.Utils;
using DevExpress.Skins;
using DevExpress.XtraRichEdit.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    public partial class JTextEditControl : DevExpress.XtraEditors.TextEdit, IJTextEditControl
    {
        public Base.ComponentTextEdit ComponentExt { get; set; }
        //public Models.JControlProperty JControlProperty { get; set; } = new Models.JControlProperty();

        private JTextEdit definition;

        public JTextEditControl()
        {
        }

        public JTextEditControl(JTextEdit definition)
        {
            this.definition = definition;
            this.DoubleClick += JTextEditControl_DoubleClick;

            // Registracija TextChanged događaja
            //this.TextChanged += JTextEditControl_TextChanged;
            InitializeComponent();
            ChangeToEmail();
        }

        private void Default_StyleChanged(object sender, EventArgs e)
        {
            ColoriseEmail();
        }

        private void ColoriseEmail()
        {
            var skin = CommonSkins.GetSkin(UserLookAndFeel.Default);

            if (Skin.DefaultSkinPaletteName == null) { this.ForeColor = Color.Blue; return; }
            try {
                var textColor = skin.SvgPalettes[Skin.DefaultSkinPaletteName].Colors.Where(x => x.Name == "Blue").First().Value;
                this.ForeColor = textColor;
            }
            catch { this.ForeColor = Color.Blue; };
            
            
        }

        private void ChangeToEmail()
        {
            if (definition.IsEmail) //&& IsValidEmail(this.Text))
            {
                // Ako je validna e-mail adresa, postavite boju teksta na plavu
                ColoriseEmail();
                this.Font = new Font(this.Font, FontStyle.Underline);
                // definition.IsEmail = true;
            }
            else
            {
                // Ako nije e-mail, vratite na podrazumevanu boju teksta
                this.ForeColor = Color.Empty;
                this.Font = new Font(this.Font, FontStyle.Regular);
                // definition.IsEmail = false;
            }
        }

        private void JTextEditControl_DoubleClick(object sender, EventArgs e)
        {
            if (definition.IsEmail && !string.IsNullOrEmpty(this.Text))
            {
                try
                {
                    // Pokrenite mailto: komandu sa e-mail adresom
                    Process.Start(new ProcessStartInfo($"mailto:{this.Text}")
                    {
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    // Obrada grešaka (npr. prikaz poruke o grešci)
                    MessageBox.Show($"Could not open email client: {ex.Message}");
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            // Prosta provera za validnu e-mail adresu
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (definition == null)
                ComponentExt = new Base.ComponentTextEdit();
            else
                ComponentExt = new Base.ComponentTextEdit(this, definition);
        }

        public void SetEvents(object ev)
        {
            if (ev is List<JEvent>)
                ComponentExt.SetEvents(ev as List<JEvent>);
            else if (ev is JEvent)
                ComponentExt.SetEvents(ev as JEvent);
        }

        public void SetNotifications(JEventAction action, object value)
        {
            ComponentExt.SetNotify(action, value);
        }

        public object GetValue(string helperStr)
        {
            if (this.EditValue is string)
            {
                if (string.IsNullOrEmpty(this.EditValue.ToString()) &&
                    definition.AllowNull == true)
                {
                    return null;
                }
            }

            return this.EditValue;
        }


        public void SetValue(object value, string text)
        {
            if (value != null)
            {
                // T.A. 03.01.2019. - prebačeno u contains jer atribut ima value datetime i datetimeadvancingcaret
                if (definition.FormatType != null && definition.FormatType.ToLower().Contains("datetime"))
                {
                    if (!(value is DateTime))
                        value = Common.Helpers.Conversion.Parse("date", value);
                }
                else if (value is string)
                    value = value.ToString().Trim();
                //else
                //    value = value.ToString().Trim();
            }

            this.EditValue = value;
        }

        public void SetItemCaption(object caption)
        {
            ComponentExt.SetItemCaption(caption);
        }

        public void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }

        public void SetVisibility(JEventAction action)
        {
            ComponentExt.SetVisibility(action);
        }

        public void RefreshValue()
        {
            ComponentExt.RefreshValue();
        }

        public void SetAttribute(JEventAction action)
        {
            ComponentExt.SetAttribute(action);
        }

        public object GetKeyCode()
        {
            return ComponentExt.GetKeycode();
        }
    }
}
using Carpo.Structure.Models.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Carpo.UI.Components
{
    public partial class JSeparatorControl : DevExpress.XtraEditors.SeparatorControl
    {
        public JSeparatorControl()
        {
        }

        private JSeparator definition;

        public JSeparatorControl(JSeparator definition)
        {
            this.TabStop = false;
            this.definition = definition;
            InitializeComponent();

            Init();

            this.Enter += (s, e) =>
            {
                SendKeys.Send("{TAB}");
            };
        }

        private void Init()
        {
            this.TabStop = false;

            this.Padding = new System.Windows.Forms.Padding(0);

            if (definition.Dark == true)
            {
                this.LookAndFeel.SkinName = "DevExpress Dark Style";
                this.LookAndFeel.UseDefaultLookAndFeel = false;
            }

            if (definition.Orientation == "vertical")
                this.LineOrientation = System.Windows.Forms.Orientation.Vertical;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (definition.BackColor == "gray")
                this.BackColor = (this.Parent as Components.CustomLayoutControl).Root.AppearanceGroup.BackColor;
            else
                this.BackColor = Color.White;
        }
    }
}
using DevExpress.XtraWaitForm;
using System;

namespace Carpo.UI.Forms
{
    public partial class WaitForm1 : WaitForm
    {
        public WaitForm1()
        {
            InitializeComponent();
            this.progressPanel1.AutoHeight = true;
            TranslateControls();
        }

        #region Overrides

        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.progressPanel1.Caption = Common.Translator.TranslateString(caption);
        }

        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.progressPanel1.Description = Common.Translator.TranslateString(description);
        }

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion Overrides

        public enum WaitFormCommand
        {
        }

        private void TranslateControls()
        {
            progressPanel1.Caption = Common.Translator.TranslateString("Molim sačekajte");
            progressPanel1.Description = Common.Translator.TranslateString("Obrada ...");
        }
    }
}
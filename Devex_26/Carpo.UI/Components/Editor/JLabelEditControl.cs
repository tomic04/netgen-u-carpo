using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    [Browsable(true)]
    public partial class JLabelEditControl : DevExpress.XtraEditors.LabelControl, IJLabelEditControl
    {
        public Base.ComponentLabel ComponentExt { get; set; }

        private JLabelEdit definition;

        public JLabelEditControl()
        {
        }

        public JLabelEditControl(JLabelEdit definition)
        {
            InitializeComponent();
            this.definition = definition;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //form = (Forms.Base.JupiterShared)FindForm();
            //connectionString = form.ConnectionString;

            if (definition == null)
                ComponentExt = new Base.ComponentLabel();
            else
                ComponentExt = new Base.ComponentLabel(this, definition);
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
            return this.Text;
        }

        public void SetValue(object value, string text)
        {
            string valueStr = "...";
            if (value != null)
                valueStr = value.ToString().Replace("\\n", "\r\n");

            //ComponentExt.SetText(valueStr);

            string prefixText = "", sufixText = "";

            Forms.Base.JupiterShared form = (Forms.Base.JupiterShared)this.FindForm();
            if (!string.IsNullOrEmpty(definition.Prefix))
            {
                Logic.Evaluate evaluate = new Logic.Evaluate(form, definition.Prefix);
                prefixText = evaluate.ToString();
            }
            if (!string.IsNullOrEmpty(definition.Sufix))
            {
                Logic.Evaluate evaluate = new Logic.Evaluate(form, definition.Sufix);
                sufixText = evaluate.ToString();
            }

            if (!string.IsNullOrEmpty(definition.Format))
                valueStr = Common.Helpers.Formats.FormatLabel(definition.Format, valueStr);

            //if (this.AutoSizeMode == DevExpress.XtraEditors.LabelAutoSizeMode.Default)
            // this.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
            //if (this.Appearance.TextOptions.HAlignment == DevExpress.Utils.HorzAlignment.Default)
            //    this.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            this.Text = definition.Caption + prefixText + valueStr + sufixText;
            //if (!string.IsNullOrEmpty(definition.Caption))
            //{
            //    this.Text = definition.Caption;
            //}
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
            throw new NotImplementedException();
        }
    }
}
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    public partial class JMemoEditControl : DevExpress.XtraEditors.MemoEdit, IJMemoEditControl
    {
        public Base.ComponentTextEdit ComponentExt { get; set; }
        //public Models.JControlProperty JControlProperty { get; set; } = new Models.JControlProperty();

        private JMemoEdit definition;

        public JMemoEditControl()
        {
        }

        public JMemoEditControl(JMemoEdit definition)
        {
            this.definition = definition;
            this.MinimumSize = new System.Drawing.Size(this.DefaultMinimumSize.Width, 50);
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

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
            return this.EditValue;
        }

        public void SetValue(object value, string text)
        {
            if (value != null && value is string)
                value = value.ToString().Trim();

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
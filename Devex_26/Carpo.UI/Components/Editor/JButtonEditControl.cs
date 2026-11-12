using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Carpo.UI.Components
{
    public partial class JButtonEditControl : UserControl, IJButtonEditControl
    {
        public Base.ComponentButtonText ComponentExt { get; set; }
        private JButtonTextEdit definition;

        public JButtonEditControl(JButtonTextEdit definition)
        {
            this.definition = definition;
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (definition == null)
                ComponentExt = new Base.ComponentButtonText();
            else
                ComponentExt = new Base.ComponentButtonText(this, definition);
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
            return txt.EditValue;
        }

        public void SetValue(object value, string text)
        {
            ComponentExt.SetEditValue(value);
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
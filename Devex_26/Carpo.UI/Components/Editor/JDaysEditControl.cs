using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    [Browsable(true)]
    public partial class JDaysEditControl : DevExpress.XtraEditors.XtraUserControl, IJDaysEditControl
    {
        public Base.ComponentDaysEdit ComponentExt { get; set; }

        private JDaysEdit definition;

        public JDaysEditControl()
        {
        }

        public JDaysEditControl(JDaysEdit definition)
        {
            this.definition = definition;
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (definition == null)
                ComponentExt = new Base.ComponentDaysEdit();
            else
                ComponentExt = new Base.ComponentDaysEdit(this, definition);
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
            return txtDan.EditValue;
        }

        public void SetValue(object value, string text)
        {
            if (value == null)
                this.txtDan.EditValue = 0;
            else
                this.txtDan.EditValue = value;
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
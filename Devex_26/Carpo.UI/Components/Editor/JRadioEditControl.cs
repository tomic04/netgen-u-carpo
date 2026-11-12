using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.Structure.Models.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    public partial class JRadioEditControl : DevExpress.XtraEditors.CheckEdit, IJRadioEditControl
    {
        public Base.ComponentCheckEdit ComponentExt { get; set; }

        private JModelEditorCheck definition;

        public JRadioEditControl()
        {
        }

        public JRadioEditControl(JModelEditorCheck definition)
        {
            InitializeComponent();
            this.definition = definition;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (definition == null)
                ComponentExt = new Base.ComponentCheckEdit();
            else
                ComponentExt = new Base.ComponentCheckEdit(this, definition);
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
            object parse = Common.Helpers.Conversion.Parse(typeof(bool), value);
            if (parse == null)
                parse = false;
            this.EditValue = (bool)parse;
        }

        public void SetItemCaption(object caption)
        {
            ComponentExt.SetItemCaption(caption);
        }

        public void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }

        public void SetControlCaption(object caption)
        {
            ComponentExt.SetControlCaption(caption);
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
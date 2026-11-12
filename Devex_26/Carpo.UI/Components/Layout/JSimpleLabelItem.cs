using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;

namespace Carpo.UI.Components.Layout
{
    public partial class JSimpleLabelItem : SimpleLabelItem, IJControl, IJEditor
    {
        public Base.ComponentSimpleLabel ComponentExt { get; set; }

        private JLabelEdit definition;

        public JSimpleLabelItem(LayoutControl layoutControl, JLabelEdit definition)
        {
            InitializeComponent();
            this.definition = definition;

            ComponentExt = new Base.ComponentSimpleLabel(layoutControl, this, definition);
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
            if (value is string)
                this.Text = value.ToString();
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
            throw new NotImplementedException();
        }

        public void SetAttribute(JEventAction action)
        {
            throw new NotImplementedException();
        }

        public object GetKeyCode()
        {
            throw new NotImplementedException();
        }
    }
}
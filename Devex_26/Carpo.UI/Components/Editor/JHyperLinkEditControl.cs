using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    public partial class JHyperLinkEditControl : DevExpress.XtraEditors.HyperLinkEdit, IJHyperLinkEditControl
    {
        public Base.ComponentHypeLinkEdit ComponentExt { get; set; }
        //public Models.JControlProperty JControlProperty { get; set; } = new Models.JControlProperty();

        private JHyperlinkEdit definition;

        public JHyperLinkEditControl()
        {
        }

        public JHyperLinkEditControl(JHyperlinkEdit definition)
        {
            this.definition = definition;
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (definition == null)
                ComponentExt = new Base.ComponentHypeLinkEdit();
            else
                ComponentExt = new Base.ComponentHypeLinkEdit(this, definition);
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
            if (value != null)
            {
                if (definition.FormatType == "datetime")
                {
                    if (!(value is DateTime))
                        value = Common.Helpers.Conversion.Parse("date", value);
                }
                else
                    value = value.ToString().Trim();
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
            throw new NotImplementedException();
        }
    }
}
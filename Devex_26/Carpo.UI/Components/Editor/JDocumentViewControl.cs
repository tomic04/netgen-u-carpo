using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Components.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.UI.Components.Editor
{
    internal class JDocumentViewControl : PanelControl, IJDocumentViewControl
    {
        public ComponentDocumentViewer ComponentExt { get; set; }
        private JDocumentViewer definition;

        public JDocumentViewControl(JDocumentViewer definition)
        {
            this.definition = definition;
            this.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            ComponentExt = new ComponentDocumentViewer(this, definition);
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

        public void SetValue(object value, string text) { }

        public void SetItemCaption(object caption)
        {
            ComponentExt.SetItemCaption(caption);
        }

        public void SetEnabled(bool enabled)
        {
            // ComponentExt.SetEnabled(enabled);
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

        public object GetValue(string helperStr)
        {
            return null;
        }
    }
}

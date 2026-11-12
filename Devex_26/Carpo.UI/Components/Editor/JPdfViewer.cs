using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Components.Base;
using System;
using System.Collections.Generic;

namespace Carpo.UI.Components.Editor
{
    public partial class JPdfViewerControl : DevExpress.XtraPdfViewer.PdfViewer, IJPdfViewerControl
    {
        //int editValueCounter = 0;
        //bool intervalExists;
        //int interval;

        public Base.ComponentPdfViewer ComponentExt { get; set; }

        private JPdfViewer definition;

        public JPdfViewerControl()
        {
        }

        public JPdfViewerControl(JPdfViewer definition)
        {
            this.definition = definition;
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //form = (Forms.Base.JupiterShared)FindForm();
            //connectionString = form.ConnectionString;

            if (definition == null)
                ComponentExt = new ComponentPdfViewer();
            else
                ComponentExt = new Base.ComponentPdfViewer(this, definition);
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

        public void SetValue(object value, string text)
        {
        }

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
            throw new NotImplementedException();
        }

        //public object GetValue(string helperStr)
        //{
        //    return this.EditValue;
        //}
    }
}
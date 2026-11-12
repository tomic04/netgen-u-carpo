using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;

namespace Carpo.UI.Components.Editor
{
    public partial class JPictureEditControl : DevExpress.XtraEditors.PictureEdit, IJPictureEditControl
    {
        //int editValueCounter = 0;
        //bool intervalExists;
        //int interval;

        public Base.ComponentPictureEdit ComponentExt { get; set; }

        private JPictureEdit definition;

        public JPictureEditControl()
        {
        }

        public JPictureEditControl(JPictureEdit definition)
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
                ComponentExt = new Base.ComponentPictureEdit();
            else
                ComponentExt = new Base.ComponentPictureEdit(this, definition);
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
            ComponentExt.SetEnabled(enabled);
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
            //if (!string.IsNullOrEmpty(definition.Format) && definition.Format.ToLower() == "hh:mm")
            //    return  String.Format("{0:00}:{1:00}", this.Time.Hour, this.Time.Minute);
            //else
            return this.EditValue;
        }
    }
}
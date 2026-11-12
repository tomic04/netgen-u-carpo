using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Carpo.UI.Components
{
    public partial class JDateEditControl : DevExpress.XtraEditors.DateEdit, IJDateEditControl
    {
        public Base.ComponentDateEdit ComponentExt { get; set; }

        private JDateEdit definition;

        public JDateEditControl()
        {
        }

        public JDateEditControl(JDateEdit definition)
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
                ComponentExt = new Base.ComponentDateEdit();
            else
                ComponentExt = new Base.ComponentDateEdit(this, definition);
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
            try
            {
                if (!string.IsNullOrEmpty(definition.controlref))
                {
                    Forms.Base.JupiterShared form = (Forms.Base.JupiterShared)this.FindForm();
                    System.Windows.Forms.Control[] timeEditControlArray = Logic.Controls.FindControl(form, definition.controlref.Replace("#", ""));
                    if (timeEditControlArray.Count() > 0)
                    {
                        TimeEdit timeEditControl = timeEditControlArray[0] as TimeEdit;
                        return this.DateTime.Date + TimeSpan.Parse(timeEditControl.EditValue.ToString());
                    }
                }
            }
            catch
            { }

            if (!string.IsNullOrEmpty(helperStr))
            {
                if (this.EditValue != null && this.EditValue is DateTime)
                {
                    if (helperStr.ToLower() == "year")
                        return Convert.ToDateTime(this.EditValue).Year;
                    else if (helperStr.ToLower() == "month")
                        return Convert.ToDateTime(this.EditValue).Month;
                    else if (helperStr.ToLower() == "day")
                        return Convert.ToDateTime(this.EditValue).Day;
                    else
                        return Convert.ToDateTime(this.EditValue).ToString(helperStr);
                }
            }

            if (this.Tag != null && (bool)this.Tag == false)
                return null;
            else
                return this.EditValue;
        }

        public void SetValue(object value, string text)
        {
            if (value is DateTime)
                this.EditValue = value;
            else
                this.EditValue = Common.Helpers.Conversion.Parse("date", value);
            //if ("now".Equals(value))
            //{
            //    this.EditValue = DateTime.Today;
            //}
            //else
            //{
            //
            //}
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
    }
}
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;

namespace Carpo.UI.Components.Editor
{
    public partial class JTimeEditControl : DevExpress.XtraEditors.TimeEdit, IJTimeEditControl
    {
        private int editValueCounter = 0;
        private bool intervalExists;
        private int interval;

        public Base.ComponentTimeEdit ComponentExt { get; set; }

        private JTimeEdit definition;

        public JTimeEditControl()
        {
        }

        public JTimeEditControl(JTimeEdit definition)
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
                ComponentExt = new Base.ComponentTimeEdit();
            else
                ComponentExt = new Base.ComponentTimeEdit(this, definition);
        }

        protected override void OnSpin(DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            try
            {
                intervalExists = false;
                interval = 1;
                int selectionStart = this.SelectionStart;
                int selectionLength = this.SelectionLength;
                DateTime editValue = Convert.ToDateTime(this.Time);
                DateTime newValue = new DateTime();

                setInterval(interval, intervalExists);

                // kada je setiran interval želim da mi se samo minute inkrementiraju
                if (intervalExists)
                {
                    if (e.IsSpinUp)
                        newValue = editValue.AddMinutes(interval);
                    else
                        newValue = editValue.AddMinutes(-interval);
                    // nakon ovoga će postaviti novi editvalue, otići u taj event i postaviti na odgovarajuću intervalnu vrijednost
                }
                // kada nije setiran interval želim da radi po defaultu
                else
                {
                    // povećavam vrijednost na strelicu
                    if (e.IsSpinUp)
                    {
                        if (selectionStart < 2)
                            newValue = editValue.AddHours(1);
                        else if (selectionStart > 2 && selectionStart < 5)
                            newValue = editValue.AddMinutes(1);
                        else
                            newValue = editValue.AddSeconds(1);
                    }
                    // smanjujem vrijednost na strelicu
                    else
                    {
                        if (selectionStart < 2)
                            newValue = editValue.AddHours(-1);
                        else if (selectionStart > 2 && selectionStart < 5)
                            newValue = editValue.AddMinutes(-1);
                        else
                            newValue = editValue.AddSeconds(-1);
                    }
                }
                this.EditValue = newValue;
                e.Handled = true;
            }
            catch
            { }
        }

        protected override void OnEditValueChanged()
        {
            try
            {
                if (editValueCounter == 0)
                {
                    intervalExists = false;
                    interval = 1;

                    int selectionStart = this.SelectionStart;
                    int selectionLength = this.SelectionLength;
                    DateTime editValue = Convert.ToDateTime(this.Time);
                    DateTime newValue = new DateTime();

                    setInterval(interval, intervalExists);

                    // kada je setiran interval želim da mi se samo minute inkrementiraju
                    int minutes = interval - editValue.Minute % interval;
                    if (intervalExists && (minutes % interval != 0))
                    {
                        if (minutes > interval / 2)
                            newValue = editValue.AddMinutes(minutes - interval);
                        else
                            newValue = editValue.AddMinutes(minutes);

                        editValueCounter = 1;
                        this.EditValue = newValue;
                    }

                    editValueCounter = 1;
                    // formatiram string vrijednosti kako bih u loggeru prikazao samo Time dio kontrole, postavljam counter u 1 kako
                    // editValue event ne bi ušao u petlju
                    this.EditValue = String.Format("{0:00}:{1:00}:{2:00}", this.Time.Hour, this.Time.Minute, this.Time.Second);
                    this.Refresh();
                    this.SelectionStart = selectionStart;
                    this.SelectionLength = selectionLength;
                }
                else
                    editValueCounter = 0;
            }
            catch
            { }
        }

        private void setInterval(int interval, bool intervalExists)
        {
            try
            {
                if (!string.IsNullOrEmpty(definition.interval))
                {
                    int n;
                    bool isNumeric = int.TryParse(definition.interval, out n);

                    if (isNumeric && (n == 10 || n == 15 || n == 20 || n == 30))
                    {
                        this.interval = n;
                        this.intervalExists = true;
                    }
                }
            }
            catch
            { }
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
            if (value != null && value.ToString().ToLower() == "now")
            {
                value = ((DateTime)Common.Helpers.Conversion.Parse("datetime", "nowtime")).TimeOfDay;
            }
            this.EditValue = Common.Helpers.Conversion.Parse("timespan", value);
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
            if (!string.IsNullOrEmpty(definition.Format) && definition.Format.ToLower() == "hh:mm")
                return String.Format("{0:00}:{1:00}", this.Time.Hour, this.Time.Minute);
            else
                return this.EditValue;
        }
    }
}
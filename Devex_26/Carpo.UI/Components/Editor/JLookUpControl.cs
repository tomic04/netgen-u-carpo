using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    public partial class JLookUpControl : DevExpress.XtraEditors.GridLookUpEdit, IJLookUpControl
    {
        public Base.ComponentLookUpEdit ComponentExt { get; set; }

        public DataRow DataRowRef { get; set; }

        //private Forms.Base.JupiterShared form;
        //private string connectionString;
        private JComboEdit definition;

        //private List<JNotification> notifValueChanged;

        public JLookUpControl(JComboEdit definition)
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
                ComponentExt = new Base.ComponentLookUpEdit();
            else
            {
                ComponentExt = new Base.ComponentLookUpEdit(this, definition);
                ComponentExt.Init();
            }
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

        public void Populate(JEventAction action, bool force, bool async, FocusType keepFocus)
        {
            ComponentExt.Populate(force);

            if (force && definition.AllowNull == false)
                ComponentExt.SetValue(this as IJEditor);
        }

        public void OpenPopup()
        {
            ComponentExt.OpenPopup();
        }

        public object GetValue(string helperStr)
        {
            //ako obrišemo vrijednost (sa x buttonom) a postoji item s praznim nazivom onda vraća vrijednost te stavke
            if (string.IsNullOrEmpty(this.Text.Trim()))
                return null;

            return this.EditValue;
        }

        public object GetValue(string field, List<string> helperStr)
        {
            if (string.IsNullOrEmpty(this.Text.Trim()))
                return null;

            return ComponentExt.GetValue(field);
        }

        public void SetValue(object value, string text)
        {
            if (value != null)
                Populate(null, false, false, FocusType.Default);

            this.EditValue = value;

            ComponentExt.SetClearButton();
        }

        public void SetItemCaption(object caption)
        {
            ComponentExt.SetItemCaption(caption);
        }

        public void ClearDataSource(JEventAction action)
        {
            this.Properties.DataSource = null;
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

        public void SetNotifyForEachRow(JEventAction action, bool onlySelected)
        {
            throw new NotImplementedException();
        }

        public object GetValueSpecial(string name, List<string> helperStr)
        {
            throw new NotImplementedException();
        }

        public object GetSelectedIds(List<string> helperStr)
        {
            throw new NotImplementedException();
        }

        public DataTable GetDataSource()
        {
            throw new NotImplementedException();
        }

        public void DataSourceFilter(JEventAction action)
        {
            throw new NotImplementedException();
        }

        public void SetAttribute(JEventAction action)
        {
            ComponentExt.SetAttribute(action);
        }

        public object GetKeyCode()
        {
            return ComponentExt.GetKeycode();
        }

        public System.Type ValueFieldColumnType()
        {
            return ComponentExt.ValueFieldColumnType;
        }
    }
}
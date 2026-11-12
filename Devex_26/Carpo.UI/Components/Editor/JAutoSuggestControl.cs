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
    public partial class JAutoSuggestControl : DevExpress.XtraEditors.LookUpEdit, IJAutoSuggestControl
    {
        public Base.ComponentAutoSuggest ComponentExt { get; set; }

        public DataRow DataRowRef { get; set; }

        public JAutoSuggest JAutoSuggestInit { get; set; }

        //private Forms.Base.JupiterShared form;
        //private string connectionString;
        private JAutoSuggest definition;

        //private List<JNotification> notifValueChanged;

        public JAutoSuggestControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public JAutoSuggestControl(JAutoSuggest definition)
        {
            InitializeComponent();
            this.definition = definition;
            this.DoubleBuffered = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //form = (Forms.Base.JupiterShared)FindForm();
            //connectionString = form.ConnectionString;

            if (definition == null)
                if (this.JAutoSuggestInit != null)
                {
                    this.definition = this.JAutoSuggestInit;
                    ComponentExt = new Base.ComponentAutoSuggest(this, this.JAutoSuggestInit);
                    ComponentExt.Init();

                } else
                {
                    ComponentExt = new Base.ComponentAutoSuggest();
                }
                
            else
            {
                ComponentExt = new Base.ComponentAutoSuggest(this, definition);
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


        public object GetValue(string helperStr)
        {
            //ako obrišemo vrijednost (sa x buttonom) a postoji item s praznim nazivom onda vraća vrijednost te stavke
            if (string.IsNullOrEmpty(this.Text.Trim()))
                return null;

            if (this.EditValue is DataRowView)
            {
                return (this.EditValue as DataRowView).Row[0];
            }

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
                Populate(null, true, false, FocusType.Default);

            if (value is DataRowView)
            {
                this.EditValue = value;
            }
            
        }

        public void SetItemCaption(object caption)
        {
            throw new NotImplementedException();
        }

        public void ClearDataSource(JEventAction action)
        {
            throw new NotImplementedException();
        }

        public void SetEnabled(bool enabled)
        {
            throw new NotImplementedException();
        }

        public void SetVisibility(JEventAction action)
        {
            throw new NotImplementedException();
        }

        public void RefreshValue()
        {
            throw new NotImplementedException();
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

        public void OpenPopup()
        {
            throw new NotImplementedException();
        }
    }
}
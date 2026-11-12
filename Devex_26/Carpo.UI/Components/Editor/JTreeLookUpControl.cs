using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.Data;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Components
{
    public partial class JTreeLookUpControl : DevExpress.XtraEditors.TreeListLookUpEdit, IJTreeLookUpControl, IJDataRow
    {
        public Base.ComponentTreeLookUpEdit ComponentExt { get; set; }

        //private Forms.Base.JupiterShared form;
        //private string connectionString;
        private JComboTreeEdit definition;

        //private List<JNotification> notifValueChanged;

        public JTreeLookUpControl(JComboTreeEdit definition)
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
                ComponentExt = new Base.ComponentTreeLookUpEdit();
            else
            {
                ComponentExt = new Base.ComponentTreeLookUpEdit(this, definition);
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
        }

        public void OpenPopup()
        {
            ComponentExt.OpenPopup();
        }

        public object GetValue(string helperStr)
        {
            return this.EditValue;
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
            return ComponentExt.GetKeycode();
        }

        public object GetValue(string field, List<string> helperStr)
        {
            return ComponentExt.GetValue(field, helperStr);
        }

        public object GetValueSpecial(string name, List<string> helperStr)
        {
            return ComponentExt.GetValueSpecial(name, helperStr);
        }

        public DataTable GetDataSource()
        {
            throw new NotImplementedException();
        }

        public DataRow DataRowRef { get; set; }

        public object GetSelectedIds(List<string> helperStr)
        {
            throw new NotImplementedException();
        }

        public void SetNotifyForEachRow(JEventAction action, bool onlySelected)
        {
            throw new NotImplementedException();
        }
    }
}
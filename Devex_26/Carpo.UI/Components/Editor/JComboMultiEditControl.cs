using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    public partial class JComboMultiEditControl : DevExpress.XtraEditors.CheckedComboBoxEdit, IJComboMultiEditControl
    {
        public Base.ComponentCheckedComboBoxEdit ComponentExt { get; set; }
        private JComboMultiEdit definition;

        public JComboMultiEditControl(JComboMultiEdit definition)
        {
            InitializeComponent();
            this.definition = definition;
        }

        //public JComboMultiEditControl(IContainer container)
        //{
        //    container.Add(this);

        //    InitializeComponent();
        //}

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //form = (Forms.Base.JupiterShared)FindForm();
            //connectionString = form.ConnectionString;

            if (definition == null)
                ComponentExt = new Base.ComponentCheckedComboBoxEdit();
            else
            {
                ComponentExt = new Base.ComponentCheckedComboBoxEdit(this, definition);
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

        public object GetValue(string helperStr)
        {
            return this.EditValue;
        }

        public void SetItemCaption(object caption)
        {
            ComponentExt.SetItemCaption(caption);
        }

        public void Populate(JEventAction action, bool force, bool async, FocusType keepFocus)
        {
            ComponentExt.Populate(force);
        }

        public void SetValue(object value, string text)
        {
            if (value != null)
                Populate(null, false, false, FocusType.Default);

            this.SetEditValue(value);

            ComponentExt.SetClearButton();
        }

        public void OpenPopup()
        {
            ComponentExt.OpenPopup();
        }

        public void ClearDataSource(JEventAction action)
        {
            this.Properties.DataSource = null;
        }

        public void DataSourceFilter(JEventAction action)
        {
            throw new NotImplementedException();
        }

        public object GetKeyCode()
        {
            return ComponentExt.GetKeycode();
        }
    }
}
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    public partial class JTokenEditControl : DevExpress.XtraEditors.TokenEdit, IJTokenEditControl
    {
        public Base.ComponentTokenEdit ComponentExt { get; set; }
        private JTokenEdit definition;

        public JTokenEditControl()
        {
            InitializeComponent();
        }

        public JTokenEditControl(JTokenEdit definition)
        {
            InitializeComponent();
            this.definition = definition;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (definition == null)
                ComponentExt = new Base.ComponentTokenEdit();
            else
            {
                ComponentExt = new Base.ComponentTokenEdit(this, definition);
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
            this.EditValue = value;
        }

        public void ClearDataSource(JEventAction action)
        {
            this.Properties.DataSource = null;
            this.Properties.Tokens.Clear();
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

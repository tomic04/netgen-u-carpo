using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Carpo.UI.Components
{
    [ToolboxItem(true)]
    [Browsable(true)]
    public partial class JButtonControl : DevExpress.XtraEditors.DropDownButton, IJButtonControl
    {
        public Base.ComponentButton ComponentExt { get; set; }

        public DataRow DataRowRef { get; set; }

        public JButton JButtonInit { get; set; }

        protected JButton definition;

        public JButtonControl()
        {
        }

        public JButtonControl(JButton definition)
        {
            this.definition = definition;
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);


            if (definition == null)
                if (this.JButtonInit != null)
                {
                    this.definition = this.JButtonInit;
                    ComponentExt = new Base.ComponentButton(this, JButtonInit);

                }
                else
                {
                    ComponentExt = new Base.ComponentButton();
                }

            else
            {
                ComponentExt = new Base.ComponentButton(this, definition);
            }

        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (ComponentExt == null)
            {
                base.OnControlAdded(e);

                if (definition == null)
                    ComponentExt = new Base.ComponentButton();
                else
                    ComponentExt = new Base.ComponentButton(this, definition);
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (ComponentExt == null)
            {
                if (definition == null)
                    ComponentExt = new Base.ComponentButton();
                else
                    ComponentExt = new Base.ComponentButton(this, definition);
            }

            //potrebno za panel buttons
            if (ComponentExt.DefaultVisibility == false && this.Visible == true)
                ComponentExt.SetVisibility(false);
        }

        public void SetEvents(object ev)
        {
            CheckHandle();
            if (ev is List<JEvent>)
                ComponentExt.SetEvents(ev as List<JEvent>);
            else if (ev is JEvent)
                ComponentExt.SetEvents(ev as JEvent);
        }

        public void SetNotifications(JEventAction action, object value)
        {
            CheckHandle();
            ComponentExt.SetNotify(action, value);
        }

        public void SetEnabled(bool enabled)
        {
            CheckHandle();
            if (ComponentExt.PermissionEnabled != false)
                this.Enabled = enabled;
        }

        private void CheckHandle()
        {
            if (!this.IsHandleCreated)
                CreateHandle();
        }

        public void SetNotifyForEachRow(JEventAction action, bool onlySelected)
        {
            throw new NotImplementedException();
        }

        public object GetValue(string field, List<string> helperStr)
        {
            return ComponentExt.GetValue(field);
        }

        public object GetSelectedIds(List<string> helperStr)
        {
            throw new NotImplementedException();
        }

        public DataTable GetDataSource()
        {
            throw new NotImplementedException();
        }

        public object GetValueSpecial(string name, List<string> helperStr)
        {
            throw new NotImplementedException();
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

        public string GetSearchText()
        {
            return ComponentExt.SearchText;
        }

        public void SetSearchText(string value)
        {
            ComponentExt.SearchText = value;
        }

        public void SetControlCaption(object caption)
        {
            ComponentExt.SetControlCaption(caption);
        }

        public object GetKeyCode()
        {
            return ComponentExt.GetKeycode();
        }
    }
}
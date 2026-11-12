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
    public partial class JSearchControl : DevExpress.XtraEditors.ButtonEdit, IJSearchControl
    {
        public Base.ComponentSearh ComponentExt { get; set; }
        public JSearchEdit JSearchEditInit { get; set; }


        private JSearchEdit definition;

        public DataRow DataRowRef { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;
        public JSearchControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public JSearchControl(JSearchEdit definition)
        {
            InitializeComponent();
            this.definition = definition;
            this.DoubleBuffered = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (definition == null)
                if (this.JSearchEditInit != null)
                {
                    this.definition = this.JSearchEditInit;
                    ComponentExt = new Base.ComponentSearh(this, this.JSearchEditInit);
                    ComponentExt.Init();
                }
                else
                {
                    ComponentExt = new Base.ComponentSearh();
                }

            else
            {
                ComponentExt = new Base.ComponentSearh(this, definition);
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

        public object GetValue(string helperStr)
        {
            return ComponentExt.Id;
        }

        public string GetValueText() //za SaveValue="session", "app"
        {
            return this.Text;
        }

        public void SetValue(object value, string text)
        {
            ComponentExt.DisplayText = text;
            if (string.IsNullOrEmpty(text))
                this.EditValue = null;
            else
                this.EditValue = ComponentExt.DisplayText;

            ComponentExt.Id = value;
        }

        public void SetItemCaption(object caption)
        {
            ComponentExt.SetItemCaption(caption);
        }

        public void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
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

        public object GetKeyCode()
        {
            return ComponentExt.GetKeycode();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Tab && Base.ComponentSearh.MoveNextControl == false)
                return true;
            else
                return base.IsInputKey(keyData);
        }
    }
}
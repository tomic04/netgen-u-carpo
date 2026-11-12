using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace Carpo.UI.Components
{
    public class JCustomUC : IJCustomUC
    {
        public JCustomUCDef definition;
        public bool InitOnMaster { get; set; }
        public Control Control { get; set; }
        public string ControlName { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        public JCustomUC()
        {
        }

        public JCustomUC(JCustomUCDef definition)
        {
            this.definition = definition;
            this.ControlName = definition.Name;
            Init();
        }

        private void Init()
        {
            ReferenceObject();
        }

        private void ReferenceObject()
        {
            try
            {
                Assembly assembly = Assembly.Load(definition.Dll);
                Type type = assembly.GetType(definition.Class);

                ConstructorInfo info = type.GetConstructor(new Type[] { });
                object instance = info.Invoke(new object[] { });
                this.Control = (Control)instance;

                if (!string.IsNullOrEmpty(definition.Name))
                    this.Control.Name = definition.Name;

                PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (PropertyInfo prp in props)
                {
                    if (prp.Name == "InitOnMaster")
                    {
                        if (!string.IsNullOrEmpty(definition.InitOnMaster) && definition.InitOnMaster == "true")
                            prp.SetValue(this.Control, true, null);
                    }
                    else
                    {
                        object value = prp.GetValue(instance, new object[] { });
                        Properties.Add(prp.Name, value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SetNotifications(JEventAction action, object value)
        {
            Type type = this.Control.GetType();

            if (action.Parameter == null)
                return;

            foreach (JEventActionParameter parameter in action.Parameter)
            {
                PropertyInfo pinfo = type.GetProperty(parameter.Name);

                if (pinfo == null)
                    return;

                var typeInfo = pinfo.PropertyType;

                if (parameter.Value != "this")
                {
                    Forms.Base.JupiterShared form = Control.FindForm() as Forms.Base.JupiterShared;

                    using (Parser parser = new Parser(form, value))
                    {
                        value = parser.GetLogicPartValue(parameter.Value, null);
                    }
                }
                //value = Logic.Parameters.GetValue(Control.FindForm() as Forms.Base.JupiterShared, value, parameter.Value);

                var parse = Common.Helpers.Conversion.Parse(pinfo.PropertyType, value);

                if (parse != null)
                    pinfo.SetValue(this.Control, Convert.ChangeType(parse, pinfo.PropertyType), null);
                else
                {
                    if (Properties.ContainsKey(parameter.Name))
                    {
                        value = Properties[parameter.Name];
                        pinfo.SetValue(this.Control, value, null);
                    }
                }
            }
        }

        public object GetValue(string name)
        {
            var names = name.Split('.');

            object value = this.Control.GetType().GetProperty(names[0]).GetValue(this.Control, null);

            if (names.Length > 1) //radi se o tablici;
            {
                return (value as DataTable).Rows[0][names[1]];
            }

            return value;
        }

        public void SetEvents(object events)
        {
            throw new NotImplementedException();
        }

        public void SetEnabled(bool enabled)
        {
            Control.Enabled = enabled;
        }

        public void SetVisibility(JEventAction action)
        {
            bool value;
            if (Boolean.TryParse(action.Value, out value))
                Control.Visible = value;
        }

        public void RefreshValue()
        {
            //za sada nema mogućnosti refresha custom kontrole - popraviti naknadno!!!
        }

        public void SetAttribute(JEventAction action)
        {
            throw new NotImplementedException();
        }

        public object GetKeyCode()
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;
            else
            {
                disposedValue = true;

                if (this.Control != null)
                    this.Control.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
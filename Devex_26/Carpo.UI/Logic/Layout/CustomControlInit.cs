using Carpo.Structure.Models.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Carpo.UI.Logic.Layout
{
    public class CustomControlInit : IDisposable
    {
        public void StartInit(JCustomDef customControl, DateTime date, Carpo.Core.Logger log)
        {
            Task tsk = new Task(() =>
            {
                JLayoutItem item = new JLayoutItem();
                var cusContr = new Carpo.Structure.Models.Core.JCustomUCDef();
                item.CustomControl = cusContr;
                item.CustomControl.Name = customControl.Name;
                item.CustomControl.Dll = customControl.Dll;
                item.CustomControl.Class = customControl.Class;
                item.CustomControl.InitOnMaster = "true";

                //using (var specific = new UI.Components.JCustomUC(item.CustomControl))
                var specific = new UI.Components.JCustomUC(item.CustomControl);
                //{
                specific.InitOnMaster = true;
                Carpo.UI.Components.Custom.GlobalCustomControls.CustomControls.Add(specific);
                //}

                log.InfoData("Inicijalizacija globalne custom kontrole", "Kontrola '" + customControl.Name +
                    "' uspješno incijalizirana ", date);
            });
            tsk.Start();
        }

        public void DisposeCustomControls()
        {
            try
            {
                foreach (var c in Carpo.UI.Components.Custom.GlobalCustomControls.CustomControls)
                {
                    if (c.InitOnMaster == true)
                    {
                        var type = c.Control.GetType();
                        PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        foreach (PropertyInfo prp in props)
                        {
                            if (prp.Name == "InitOnMaster")
                            {
                                prp.SetValue(c.Control, false, null);
                                break;
                            }
                        }
                    }

                    c.Dispose();
                    c.Control = null;
                }

                Carpo.UI.Components.Custom.GlobalCustomControls.CustomControls.Clear();
            }
            catch (Exception ex)
            {
                Carpo.Common.Helpers.JMessages.Error(ex, ex.Message.ToString(), "CustomControlInit :: DisposeCustomControls()");
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}
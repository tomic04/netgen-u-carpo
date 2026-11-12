using Carpo.Structure.Models.Core;
using System;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    public class ComDll
    {
        public static void Run(Forms.Base.JupiterShared form, JEventAction action)
        {
            if (action.Parameter.Count > 0)
                TotalGen(form, action);
            else
                SpinClient(form, action);
        }

        private static void SpinClient(Forms.Base.JupiterShared form, JEventAction action)
        {
            string program = "";
            if (!string.IsNullOrEmpty(action.Value))
                program = action.Value;

            try
            {
                string[] progPars = program.Split(':');
                string programId = progPars[0];
                string moduleId = "";
                if (progPars.Length > 1)
                    moduleId = progPars[1];

                string prms = programId + "#" +
                    moduleId + "#" +
                    "0" + "#" +
                    Core.PubVars.OperateriId.ToString() + "#" +
                    Carpo.Data.Connection.ConnectionStringADO.ToString() + "#" +
                    form.ParentHandle.ToString() + "#" +
                    Carpo.Data.Connection.Server + "#" +
                    Carpo.Data.Connection.DataBase + "#" +
                    Carpo.Core.IniFile.Ini["RemoteDllPath"] + "#" +
                    Carpo.Data.Connection.Uid + "#" +
                    Carpo.Data.Connection.Pwd + "#" +
                    "0" + "#" +
                    "0" + "#" +
                    "0";

                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("SpinClient.exe", prms);

                System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: ComDll.SpinClient - " + program, ex.Message, ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private static void TotalGen(Forms.Base.JupiterShared form, JEventAction action)
        {
            string comObjectStr = action.Value;

            try
            {
                int id = 0;
                string par1 = "";
                string par2 = "";

                Parser parser = new Parser(form, null);
                object value;
                string valueStr;
                foreach (var par in action.Parameter)
                {
                    switch (par.Name.ToLower())
                    {
                        case "id":
                            value = parser.GetLogicPartValue(par.Value);
                            valueStr = "";
                            if (value != null)
                            {
                                valueStr = value.ToString();
                                id = (int)Common.Helpers.ValueHelpers.CoalesceInt(valueStr, 0);
                            }
                            break;

                        case "par1":
                            value = parser.GetLogicPartValue(par.Value);
                            if (value != null)
                            {
                                par1 = value.ToString();
                            }
                            break;

                        case "par2":
                            value = parser.GetLogicPartValue(par.Value);
                            if (value != null)
                            {
                                par2 = value.ToString();
                            }
                            break;
                    }
                }

                System.Type objType = System.Type.GetTypeFromProgID(comObjectStr);
                dynamic comObject = System.Activator.CreateInstance(objType);
                string connStrADO = Carpo.Data.Connection.ConnectionStringADO;

                try
                {
                    form.Logger.Info("ComDll", id.ToString()
                    + " ," + Core.PubVars.OperateriId.ToString()
                    + " ," + connStrADO
                    + @" ,""" + par1 + @""""
                    + @" ,""" + par2 + @"""");
                }
                catch { }

                comObject.TotalGen(id, Core.PubVars.OperateriId, connStrADO, par1, par2);
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: ComDll.TotalGen - " + comObjectStr, ex.Message, ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;

namespace Carpo.UI.Logic
{
    public class Generate : IDisposable
    {
        private JEventAction action;
        private Forms.Base.JupiterShared form;

        public Generate(JEventAction Action, Forms.Base.JupiterShared Form)
        {
            action = Action;
            form = Form;
        }

        public bool GenerateString()
        {
            using (Carpo.UI.Logic.Parser parser = new Carpo.UI.Logic.Parser(form, null))
            {
                string outStr = "", outParName = "";

                if (string.IsNullOrEmpty(action.Parameter[0].Name))
                {
                    form.Logger.Warn("method: CheckGenerateString", "Prvi parametar ne sadrži 'name' atribut");
                    return false;
                }
                else
                    outParName = action.Parameter[0].Name;

                for (int i = 1; i < action.Parameter.Count; i++)
                {
                    object value = parser.GetLogicPartValue(action.Parameter[i].Value);
                    if (value != null)
                        outStr += value.ToString();
                    else
                    {
                        form.Logger.Warn("method: CheckGenerateString", "Neuspješno parsiranje parametra '" + action.Parameter[i].Value + "'");
                        return false;
                    }
                }

                FormsVar.SetVariable(form, outParName, outStr);
            }

            return true;
        }

        public bool GenerateDatastring()
        {
            using (Carpo.UI.Logic.Parser parser = new Carpo.UI.Logic.Parser(form, null))
            {
                object findDataDef = parser.GetLogicPartValue(action.Value);

                string actionValue = action.Value.Replace("~", "");

                if (findDataDef == null)
                {
                    form.Logger.Warn("method: CheckGenerateDatastring", "Dataset imena '" + actionValue + "' ne postoji!");
                    return false;
                }

                Logic.Data.Core data = new Logic.Data.Core();

                string value = data.GenerateDatasring(form, findDataDef as JData);

                FormsVar.SetScalarParameter(form as Forms.Base.JupiterShared, action, value);
            }

            return true;
        }

        public bool GenerateNumber()
        {
            string outParName = "", arithmeticType = "";

            if (string.IsNullOrEmpty(action.Parameter[0].Name))
            {
                form.Logger.Warn("method: CheckGenerateNumber", "Prvi parametar ne sadrži 'name' atribut");
                return false;
            }
            else if (string.IsNullOrEmpty(action.Parameter[0].ArithmeticType))
            {
                form.Logger.Warn("method: CheckGenerateNumber", "Prvi parametar ne sadrži 'arithmetictype' atribut");
                return false;
            }
            else
            {
                arithmeticType = action.Parameter[0].ArithmeticType.ToLower();
                outParName = action.Parameter[0].Name;
            }

            switch (arithmeticType)
            {
                case "add":
                    Add(outParName, action.Parameter);
                    break;

                case "sub":
                case "mul":
                case "div":
                    SubMulDiv(arithmeticType, outParName, action.Parameter);
                    break;

                case "mod":
                    Mod(outParName, action.Parameter);
                    break;
            }

            return true;
        }

        private void Add(string outParName, List<JEventActionParameter> parList)
        {
            try
            {
                using (Carpo.UI.Logic.Parser parser = new Carpo.UI.Logic.Parser(form, null))
                {
                    decimal retValue = 0;

                    for (int i = 1; i < parList.Count; i++)
                    {
                        object value = parser.GetLogicPartValue(parList[i].Value);
                        retValue += Convert.ToDecimal(value);
                    }

                    FormsVar.SetVariable(form, outParName, retValue);
                }
            }
            catch
            {
                throw;
            }
        }

        private void SubMulDiv(string arithmeticType, string outParName, List<JEventActionParameter> parList)
        {
            try
            {
                using (Carpo.UI.Logic.Parser parser = new Carpo.UI.Logic.Parser(form, null))
                {
                    object valueFirst = parser.GetLogicPartValue(parList[1].Value);
                    decimal retValue = Convert.ToDecimal(valueFirst);

                    for (int i = 2; i < parList.Count; i++)
                    {
                        object value = parser.GetLogicPartValue(parList[i].Value);

                        switch (arithmeticType)
                        {
                            case "sub":
                                retValue -= Convert.ToDecimal(value);
                                break;

                            case "mul":
                                retValue *= Convert.ToDecimal(value);
                                break;

                            case "div":
                                retValue /= Convert.ToDecimal(value);
                                break;
                        }
                    }

                    FormsVar.SetVariable(form, outParName, retValue);
                }
            }
            catch
            {
                throw;
            }
        }

        private void Mod(string outParName, List<JEventActionParameter> parList)
        {
            try
            {
                using (Carpo.UI.Logic.Parser parser = new Carpo.UI.Logic.Parser(form, null))
                {
                    object valueFirst = parser.GetLogicPartValue(parList[1].Value);
                    int retValue = Convert.ToInt32(valueFirst);

                    for (int i = 2; i < parList.Count; i++)
                    {
                        object value = parser.GetLogicPartValue(parList[i].Value);
                        retValue %= Convert.ToInt32(value);
                    }

                    FormsVar.SetVariable(form, outParName, retValue);
                }
            }
            catch
            {
                throw;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Notifications() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
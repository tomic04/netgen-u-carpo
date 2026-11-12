using System;
using System.Collections.Generic;
using System.Reflection;

namespace Carpo.Common.Helpers
{
    public class AssemblyLoad : IDisposable
    {
        private Assembly assembly;
        private Type type;

        public object Load(string dllStr, string classStr, string methodStr, List<object> paramList)
        {
            var model = SetModel(dllStr, classStr, methodStr, paramList);

            if (assembly == null)
                assembly = Assembly.Load(model.Dll);

            if (type == null)
                type = assembly.GetType(model.Class);

            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod(model.Method);

                if (methodInfo != null)
                {
                    try
                    {
                        object result = null;
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        object classInstance = Activator.CreateInstance(type, null);
                        result = methodInfo.Invoke(classInstance, paramList.ToArray());

                        if (result != null)
                            return result;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return null;
        }

        private Carpo.Common.Models.Assembly SetModel(string dllStr, string classStr, string methodStr, List<object> paramList)
        {
            return new Carpo.Common.Models.Assembly()
            {
                Dll = dllStr,
                Class = classStr,
                Method = methodStr,
                Params = paramList
            };
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

                if (this != null)
                    this.Dispose();
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
using System;
using System.Reflection;

namespace Carpo.XmlGen.XmlParse.Repx
{
    public class Initialize
    {
        public bool Init(string template, string parameters, string tempPathFileName)
        {
            try
            {
                Assembly asm = Assembly.Load("rptDevRepx");
                Type t = asm.GetType("rptDevRepx.clsReport");
                var methodInfoStatic = t.GetMethod("ExportToFile");
                if (methodInfoStatic == null)
                {
                    throw new Exception("No such static method exists.");
                }
                else
                {
                    object[] constructorParameters = new object[7];
                    constructorParameters[0] = Global.ConnectionString;
                    constructorParameters[1] = 0;
                    constructorParameters[2] = Global.RepxFolder;
                    constructorParameters[3] = template;
                    constructorParameters[4] = parameters;
                    constructorParameters[5] = tempPathFileName;
                    constructorParameters[6] = "pdf";

                    var o = Activator.CreateInstance(t);

                    var result = methodInfoStatic.Invoke(o, constructorParameters);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Carpo.Common.LogWriter.LogLine("Carpo.XmlGen.XmlParse.Repx.Init() :: " + ex.Message, true);
                return false;
            }
        }
    }
}
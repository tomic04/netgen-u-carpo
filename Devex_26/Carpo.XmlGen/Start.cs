using Carpo.Structure.Models.Core.XmlGen;
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Carpo.XmlGen
{
    public class Start : IDisposable
    {
        //public Models.SetupData setupData;
        public static string errorMethodStr = "";

        public static string ErrorMethodStr
        {
            get
            {
                return errorMethodStr;
            }
            set
            {
                errorMethodStr = "Carpo.XmlGen." + value.ToString() + " ::";
            }
        }

        public string ErrorMsg { get; set; }
        public bool IsTestShell { get; set; } = false;
        public XDocument GeneratedXDocument;
        public Dictionary<string, DataTable> listDataTable = new Dictionary<string, DataTable>();
        public SplashScreenManager SplashScreenManager;

        public object GetXmlObject(string connectionStr, string operateriId, string variablesList, string objectType,
            string repxFolder = "", XDocument xmlScheme = null, string xmlSchemePath = "", SplashScreenManager splashScreenManager = null)
        {
            Global.ConnectionString = connectionStr;
            Global.ObjectType = objectType.ToLower();
            Global.OperateriId = operateriId;
            Global.RepxFolder = IsTestShell ? Carpo.Core.IniFile.Ini["RemoteDllPath"] + @"\SharedDLLs\repx" : repxFolder;
            Global.VariablesList = SetVariablesList(variablesList);
            Global.XmlSchema = xmlScheme != null ? xmlScheme : XDocument.Load(xmlSchemePath);

            if (splashScreenManager != null)
                SplashScreenManager = splashScreenManager;

            return Generate();
        }

        public bool CreateXmlOnPath(string connectionStr, string operateriId, string variablesList, string repxFolder,
            string createPath, XDocument xmlScheme = null, string xmlSchemePath = "")
        {
            Global.ConnectionString = connectionStr;
            Global.CreatePath = createPath;
            Global.OperateriId = operateriId;
            Global.RepxFolder = IsTestShell ? Carpo.Core.IniFile.Ini["RemoteDllPath"] + @"\SharedDLLs\repx" : repxFolder;
            Global.VariablesList = SetVariablesList(variablesList);
            Global.XmlSchema = xmlScheme != null ? xmlScheme : XDocument.Load(xmlSchemePath);

            return Convert.ToBoolean(Generate());
        }

        public Dictionary<string, object> SetVariablesList(string listStr)
        {
            ErrorMethodStr = "Start.SetVariablesList()";

            try
            {
                var variablesList = new Dictionary<string, object>();

                var listArr = listStr.Split(',');

                foreach (var par in listArr)
                {
                    int splitIndex = par.IndexOf('=');
                    string paramName = par.Substring(0, splitIndex);
                    string parValue = par.Substring(splitIndex + 1);

                    variablesList.Add(paramName, parValue);
                }

                return variablesList;
            }
            catch (Exception ex)
            {
                ErrorMsg = ErrorMethodStr + ex.Message;
                return new Dictionary<string, object>();
            }
        }

        public object Generate()
        {
            ErrorMethodStr = "Start.Generate()";

            // inicijalizacija LOG WRITERA
            Carpo.Common.LogWriter.LogDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log\\";
            Carpo.Common.LogWriter.IsTestShell = this.IsTestShell;
            Carpo.Common.LogWriter.AppName = "Carpo.XmlGen";
            Carpo.Common.LogWriter.Init();
            Carpo.Common.LogWriter.LogLine("START - Započinjem proces generiranja XML-a");

            try
            {
                GeneratedXDocument = SetXml(Global.XmlSchema);
                Carpo.Common.LogWriter.LogLine("Uspješno generiran XML !");

                if (string.IsNullOrEmpty(Global.CreatePath))
                {
                    switch (Global.ObjectType)
                    {
                        case "xdocument":
                            return GeneratedXDocument;

                        case "xmldocument":
                            return GetXmlDocument(GeneratedXDocument);

                        case "string":
                            return GeneratedXDocument.ToString();

                        case "byte":
                            return GetByte(GeneratedXDocument);

                        default:
                            return GeneratedXDocument.ToString();
                    }
                }
                else
                {
                    return TryCreateXmlOnPath(GeneratedXDocument);
                }
            }
            catch (Exception ex)
            {
                ErrorMsg = ErrorMethodStr + ex.Message;
                Carpo.Common.LogWriter.LogLine(ErrorMsg, true);

                return null;
            }
            finally
            {
                if (SplashScreenManager != null && SplashScreenManager.IsSplashFormVisible == true)
                    SplashScreenManager.CloseWaitForm();

                Carpo.Common.LogWriter.LogLine("END: Proces završen!");
                Carpo.Common.LogWriter.InsertBrakeLine();
                Carpo.Common.LogWriter.LogEnd();
            }
        }

        public XmlDocument GetXmlDocument(XDocument xDocument)
        {
            ErrorMethodStr = "Start.GetXmlDocument()";

            try
            {
                var xmlDocument = new XmlDocument();
                using (var xmlReader = GeneratedXDocument.CreateReader())
                {
                    xmlDocument.Load(xmlReader);
                }
                return xmlDocument;
            }
            catch (Exception ex)
            {
                ErrorMsg = ErrorMethodStr + ex.Message;
                Carpo.Common.LogWriter.LogLine(ErrorMsg, true);

                return null;
            }
        }

        public byte[] GetByte(XDocument xDocument)
        {
            ErrorMethodStr = "Start.GetByte()";

            try
            {
                var valueObj = Regex.Replace(xDocument.ToString(), @"[^\u0000-\u007F]", string.Empty); // Removes non ascii characters

                return Encoding.UTF8.GetBytes(valueObj);
            }
            catch (Exception ex)
            {
                ErrorMsg = ErrorMethodStr + ex.Message;
                Carpo.Common.LogWriter.LogLine(ErrorMsg, true);

                return null;
            }
        }

        public bool TryCreateXmlOnPath(XDocument xDocument)
        {
            ErrorMethodStr = "Start.TryCreateXmlOnPath()";

            try
            {
                string testFolder = Global.CreatePath.Substring(0, Global.CreatePath.LastIndexOf(@"\"));

                if (!Directory.Exists(testFolder))
                {
                    Carpo.Common.LogWriter.LogLine(ErrorMethodStr + ":: nepostojeći folder " + testFolder);
                    return false;
                }

                xDocument.Save(Global.CreatePath); // automatski dodaje <?xml version="1.0" encoding="utf-8"?>
                Carpo.Common.LogWriter.LogLine("Xml uspješno kreiran na putanji " + Global.CreatePath);

                return true;
            }
            catch (Exception ex)
            {
                ErrorMsg = ErrorMethodStr + ex.Message;
                Carpo.Common.LogWriter.LogLine(ErrorMsg, true);

                return false;
            }
        }

        public XDocument SetXml(XDocument xDoc)
        {
            ErrorMethodStr = "Start.SetXml()";

            var model = new JXmlGenRoot();

            try
            {
                SetWaitFormDescription("Parsiraranje varijabli");

                xDoc = ParseVariables(Global.VariablesList, xDoc); // parsiram varijable poslane kroz parametar u pozivu generatora
                xDoc = ParseGlobalVariables(xDoc); // parsiram globalne varijable u xml šemi

                using (var dsrXml = new Deserialize())
                {
                    SetWaitFormDescription("Deserijalizacija XML-a");
                    model = dsrXml.DeserializeXml(xDoc);
                }

                if (model.DataList != null)
                {
                    SetWaitFormDescription("Dohvat podataka iz baze");
                    Data.GetDataSet.FillDataList(listDataTable, model.DataList);
                }

                if (model.Xml != null)
                {
                    SetWaitFormDescription("Parsiranje XML-a");
                    using (var parse = new XmlGen.XmlParse.Parse(model.Xml, listDataTable, SplashScreenManager))
                    {
                        parse.ParseForeach(); // prije konačnog parsiranja moram isparsirati prvo foreach da zaljepim sve elemente
                        return parse.ParseXml();
                    }
                }
                else
                    return null;
            }
            catch
            {
                throw;
            }
        }

        private XDocument ParseVariables(Dictionary<string, object> paramList, XDocument xDoc)
        {
            ErrorMethodStr = "Start.ParseVariables()";

            try
            {
                string xstring = xDoc.ToString();

                if (paramList.Count == 0)
                    return xDoc;

                foreach (var param in paramList)
                {
                    string variableName = param.Key.Replace("@", "$");
                    xstring = xstring.Replace(variableName, param.Value.ToString());
                }

                return XDocument.Parse(xstring);
            }
            catch (Exception ex)
            {
                ErrorMsg = ErrorMethodStr + ex.Message;
                Carpo.Common.LogWriter.LogLine(ErrorMsg, true);

                return null;
            }
        }

        private XDocument ParseGlobalVariables(XDocument xDoc)
        {
            ErrorMethodStr = "Start.ParseGlobalVariables()";

            try
            {
                string xstring = xDoc.ToString();

                var regex = new Regex("@@operateriid", RegexOptions.IgnoreCase);
                xstring = regex.Replace(xstring, Global.OperateriId.ToString());

                return XDocument.Parse(xstring);
            }
            catch (Exception ex)
            {
                ErrorMsg = ErrorMethodStr + ex.Message;
                Carpo.Common.LogWriter.LogLine(ErrorMsg, true);

                return null;
            }
        }

        private void SetWaitFormDescription(string text)
        {
            if (SplashScreenManager != null)
                SplashScreenManager.SetWaitFormDescription(text);
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
using System;
using System.Collections.Generic;

namespace Carpo.Core.Print
{
    public class DoPrint
    {
        private string report;
        private int copies = 0;
        private int id = 0;
        private int operaterId = 0;
        private int entityId = 0;
        private string entityName = "";
        private string fileName = "";
        private string parameters = "";
        private string printer = "";
        private bool sendToDms = false;
        private byte[] repxAsByte;

        public DoPrint(string report, string parameters, string printer, int copies, int operaterId, bool sendToDms)
        {
            this.report = report;
            this.parameters = sendToDms ? ParseDmsParameters(parameters) : parameters;
            this.copies = copies;
            this.printer = printer;
            this.operaterId = operaterId;
            this.sendToDms = sendToDms;
        }

        public void Run()
        {
            object resultObj;

            if (!this.sendToDms)
            {
                using (var repxClass = new Carpo.Common.Helpers.AssemblyLoad())
                {
                    repxClass.Load("rptDevRepx", "rptDevRepx.clsReport", "DoPrint2", CreateParams("Repx_DoPrint2"));
                }
            }
            else
            {
                using (var repxClass = new Carpo.Common.Helpers.AssemblyLoad())
                {
                    resultObj = repxClass.Load("rptDevRepx", "rptDevRepx.clsReport", "GetBytes", CreateParams("Repx_GetBytes"));
                }

                if (resultObj != null)
                {
                    using (var repxClass = new Carpo.Common.Helpers.AssemblyLoad())
                    {
                        repxAsByte = resultObj as byte[];
                        var str = repxClass.Load("DmsManagerV1", "DmsManagerV1.Start", "SaveDocumentByte", CreateParams("DmsManagerV1_SaveDocumentByte"));
                    }
                }
            }
        }

        private List<object> CreateParams(string switchParam)
        {
            List<object> pars = new List<object>();

            switch (switchParam)
            {
                case "Repx_DoPrint2":
                    //Public Function DoPrint2(ByVal pConnectionString As String, ByVal pID As Integer, ByVal pRepxFolder As String, ByVal pRepxNaziv As String, Optional ByVal pCopies As Integer = 0, Optional ByVal pPrinter As String = "", Optional ByVal pParametri As String = "") As Boolean
                    pars.Add(Data.Connection.ConnectionString);
                    pars.Add(id);
                    pars.Add(System.IO.Path.Combine(Core.IniFile.Ini["RemoteDllPath"], "SharedDLLs", "repx"));
                    pars.Add(report);
                    pars.Add(copies);
                    pars.Add(printer);
                    pars.Add(parameters);
                    break;

                case "Repx_GetBytes":
                    //Public Function GetBytes(connectionString As String, id As Integer, repxFolder As String, repxNaziv As String, parametri As String, ext As String) As Byte()
                    pars.Add(Data.Connection.ConnectionString);
                    pars.Add(id);
                    pars.Add(System.IO.Path.Combine(Core.IniFile.Ini["RemoteDllPath"], "SharedDLLs", "repx"));
                    pars.Add(report);
                    pars.Add(parameters);
                    pars.Add("pdf");
                    break;

                case "DmsManagerV1_SaveDocumentByte":
                    //public string SaveDocumentByte(string server, string database, string username, string password, int operaterId, byte[] file, string fileName, string entityName, int entityId)
                    pars.Add(Data.Connection.Server);
                    pars.Add(Data.Connection.DataBase);
                    pars.Add(Data.Connection.Uid);
                    pars.Add(Data.Connection.Pwd);
                    pars.Add(operaterId);
                    pars.Add(repxAsByte);
                    pars.Add(fileName);
                    pars.Add(entityName);
                    pars.Add(entityId);
                    break;
            }

            return pars;
        }

        private string ParseDmsParameters(string parameters)
        {
            List<string> listStr = new List<string>();

            string[] parametersArr = parameters.Replace("'", "").Split(',');
            if (parametersArr.Length > 0)
            {
                for (int i = 0; i < parametersArr.Length; i++)
                {
                    if (parametersArr[i].ToLower().Contains("@dmsid"))
                        entityId = Convert.ToInt32(parametersArr[i].Split('=')[1]);
                    else if (parametersArr[i].ToLower().Contains("@dmsentityname"))
                        entityName = parametersArr[i].Split('=')[1];
                    else if (parametersArr[i].ToLower().Contains("@dmsfilename"))
                        fileName = parametersArr[i].Split('=')[1];
                    else
                        listStr.Add(parametersArr[i]);
                }

                if (entityId != 0 && !string.IsNullOrEmpty(entityName) && string.IsNullOrEmpty(fileName))
                    fileName = String.Format("{0}_{1}.pdf", entityName, entityId);

                return String.Join(", ", listStr);
            }
            else
                return "";
        }
    }
}
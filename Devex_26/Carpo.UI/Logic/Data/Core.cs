using Carpo.Common.Helpers;
using Carpo.Core;
using Carpo.Core.Log;
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Logic.Data
{
    public class Core
    {
        /****start - varijable potrebne prilikom serijalizacije xml-a****/
        public JRoot DefinitionRoot { get; set; }
        public Logger Log { get; set; }
        /****end****/

        public enum DataType { NonQuery, Reader, Scalar };

        public string ParameterControlValue { get; set; }
        public string ConnectionString { get; set; }
        public Forms.Base.JupiterShared CurrentForm { get; set; }
        public static SqlConnection SqlConnection { get; set; }

        public Core()
        { }

        public JData FindJData(List<JData> dataList)
        {
            foreach (JData data in dataList)
            {
                Statemants statemants = new Statemants(Conditions.GetIfString(data as IJCondition), CurrentForm, null);
                if (statemants.CheckIf() == true)
                    return data;
            }
            return null;
        }

        public DataSet GetDataSet(JData data, string infoOpis, out bool stopActions, Structure.Models.ConnWithTran connTranModel = null)
        {
            stopActions = false;
            return SetData(data, DataType.Reader, infoOpis, out stopActions, connTranModel) as DataSet;
        }

        public void ExecuteData(JData data, string infoOpis, out bool stopActions, Structure.Models.ConnWithTran connTranModel = null)
        {
            stopActions = false;
            SetData(data, DataType.NonQuery, infoOpis, out stopActions, connTranModel);
        }

        public object ExecuteScalar(JData data, string infoOpis, out bool stopActions, Structure.Models.ConnWithTran connTranModel)
        {
            stopActions = false;
            return SetData(data, DataType.Scalar, infoOpis, out stopActions, connTranModel);
        }

        public string GenerateDatasring(Form form, JData data)
        {
            CurrentForm = form as Carpo.UI.Forms.Base.JupiterShared;
            Carpo.Data.Common.DataType commandDataType = Carpo.Data.Common.DataType.StoredProcedure;
            if (!string.IsNullOrEmpty(data.Type) && data.Type == "text")
                commandDataType = Carpo.Data.Common.DataType.Text;

            string command = data.Command;
            Evaluate evaluate = new Evaluate(form, command);
            command = evaluate.ToString();
            Dictionary<string, object> parameters = GetParameters(data.DataParameterList);

            using (var sqlCommand = new SqlCommand(command))
            {
                string hlpStr = "";
                if (commandDataType == Carpo.Data.Common.DataType.StoredProcedure)
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    hlpStr += "exec dbo.";
                }

                sqlCommand.CommandTimeout = Convert.ToInt32(data.Timeout);

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, Object> a in parameters)
                    {
                        sqlCommand.Parameters.Add(Carpo.Data.Common.ToConvertSqlParams(sqlCommand, a.Key, a.Value));
                    }
                }

                return hlpStr + Carpo.Data.Log.CommandAsSql(sqlCommand);
            }
        }

        public object SetData(JData data, DataType dataType, string infoOpis, out bool stopActions, Structure.Models.ConnWithTran connTranModel)
        {
            stopActions = false;
            int timeout = 30;
            var outPar = new List<Carpo.Data.Models.OutputParametersModel.OutParModel>();

            if (data == null)
            {
                LogData(LogType.Warning, "data = null", "data reader; " + infoOpis);
                return null;
            }

            if (!string.IsNullOrEmpty(data.Timeout))
            {
                try
                {
                    timeout = Convert.ToInt32(ParseParameter(data.Timeout));
                }
                catch
                {
                    LogData(LogType.Warning, "timeout=''" + ParseParameter(data.Timeout) + "''", "neispravno definiran atribut 'timeout'");
                }
            }

            string sqlText = "";

            try
            {
                if (data.Type == null || data.Type.ToLower() != "integrated")
                {
                    Logic.Statemants statemants = new Logic.Statemants(Conditions.GetIfString(data as IJCondition), CurrentForm, null);
                    if (statemants.CheckIf() == true)
                    {
                        Carpo.Data.Common.DataType commandDataType = Carpo.Data.Common.DataType.StoredProcedure;
                        if (!string.IsNullOrEmpty(data.Type) && data.Type == "text")
                            commandDataType = Carpo.Data.Common.DataType.Text;

                        string command = data.Command;

                        Evaluate evaluate = new Evaluate(CurrentForm, command);
                        command = evaluate.ToString();

                        Dictionary<string, object> parameters = GetParameters(data.DataParameterList);

                        DateTime startTime = DateTime.Now;
                        // A8: avoid CommandAsSql expansion when InfoData will not be written.
                        bool prevExpandSql = Carpo.Data.Common.ExpandSqlParametersForLog;
                        if (CurrentForm?.Logger != null)
                            Carpo.Data.Common.ExpandSqlParametersForLog = CurrentForm.Logger.LogLevel >= LogLevel.Info;
                        try
                        {
                        if (dataType == DataType.Reader)
                        {
                            DataSet ds;
                            if (connTranModel != null)
                                ds = Carpo.Data.Common.ExecuteQueryInTran(ConnectionString, command, commandDataType, parameters, out sqlText, timeout, connTranModel.Connection, connTranModel.Transaction);
                            else
                            {
                                ds = Carpo.Data.Common.ExecuteQuery(ConnectionString, command, commandDataType, parameters, out sqlText, timeout, out outPar);
                            }
                            LogData(LogType.InfoData, "data reader; " + infoOpis, sqlText, startTime);

                            return ds;
                        }
                        else if (dataType == DataType.Scalar)
                        {
                            object value;
                            if (connTranModel != null)
                                value = Carpo.Data.Common.ExecuteAzurInTran(ConnectionString, command, commandDataType, parameters, true, out sqlText, timeout, connTranModel.Connection, connTranModel.Transaction);
                            else
                                value = Carpo.Data.Common.ExecuteScalar(ConnectionString, command, commandDataType, parameters, out sqlText, timeout);
                            LogData(LogType.InfoData, "data scalar; " + infoOpis, sqlText, startTime);
                            return value;
                        }
                        else if (dataType == DataType.NonQuery)
                        {
                            if (connTranModel != null)
                                Carpo.Data.Common.ExecuteAzurInTran(ConnectionString, command, commandDataType, parameters, false, out sqlText, timeout, connTranModel.Connection, connTranModel.Transaction);
                            else
                                Carpo.Data.Common.ExecuteNonQuery(ConnectionString, command, commandDataType, parameters, out sqlText, timeout);
                            LogData(LogType.InfoData, "data nonquery; " + infoOpis, sqlText, startTime);
                            return null;
                        }
                        }
                        finally
                        {
                            Carpo.Data.Common.ExpandSqlParametersForLog = prevExpandSql;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                LogData(LogType.InfoData, GetDataTypeForLog(dataType) + infoOpis, sqlText, null);
                string message = ex.Message.Trim();
                string controlStr = "";

                JMessages.DbTypes messageType = JMessages.DbTypes.None;
                if (message.EndsWith("+close", StringComparison.OrdinalIgnoreCase))
                    messageType = JMessages.DbTypes.Close;
                else if (message.EndsWith("+ok", StringComparison.OrdinalIgnoreCase))
                    messageType = JMessages.DbTypes.Ok;
                else if (message.IndexOf("+focus", StringComparison.OrdinalIgnoreCase) >= 0)
                    messageType = JMessages.DbTypes.Focus;

                int number = ex.Number;

                if (messageType != JMessages.DbTypes.None || number >= 50000)
                {
                    if (messageType != JMessages.DbTypes.Ok)
                        stopActions = true;

                    if (messageType == JMessages.DbTypes.Focus)
                    {
                        int index = message.IndexOf("+focus", StringComparison.OrdinalIgnoreCase);
                        if (index >= 0)
                        {
                            string leftSide = message.Substring(0, index);
                            controlStr = message.Substring(index + 1);

                            message = leftSide;

                            string[] output = controlStr.Split('(', ')');
                            if (output.Length > 0)
                                controlStr = output[1];

                            if (controlStr.StartsWith("#"))
                                controlStr = controlStr.Replace("#", "");

                            Control[] c = Logic.Controls.FindControl(CurrentForm, controlStr);
                            if (c.Length > 0)
                                c[0].Focus();
                        }
                    }
                    else
                    {
                        if (messageType != JMessages.DbTypes.None)
                        {
                            int msgTypeLength = messageType.ToString().Length + 1; //+1 -> zbog + prefixa
                            message = message.Remove(message.Length - msgTypeLength, msgTypeLength);
                        }
                    }
                    string parsedMessage = message.Replace("\\n", "\r\n");
                    Common.Helpers.JMessages.Info(parsedMessage);
                    LogData(LogType.InfoData, "DB message", ex.Message, ex);
                }
                else
                {
                    stopActions = true;
                    LogData(LogType.Error, "method: SetData", ex.Message, ex);
                }

                if (connTranModel != null)
                    throw;
            }
            catch (Exception ex)
            {
                stopActions = true;
                LogData(LogType.Error, "method: SetData", ex.Message, ex);
            }
            finally
            {
                CheckOutputParameters(outPar, data.DataParameterList);
            }
            return null;
        }

        private string GetDataTypeForLog(DataType dataType)
        {
            if (dataType == DataType.NonQuery)
                return "data nonquery";
            else if (dataType == DataType.Reader)
                return "data reader";
            else if (dataType == DataType.Scalar)
                return "data scalar";
            else
                return "data";
        }

        private void LogData(LogType logType, string opis, string text, params object[] args)
        {
            Logger logger = null;
            if (Log != null)
                logger = Log;
            else if (CurrentForm.Logger != null)
                logger = CurrentForm.Logger;

            if (logger != null)
            {
                if (logType == LogType.InfoData)
                {
                    logger.InfoData(opis, text, args);
                }
                else if (logType == LogType.Warning)
                {
                    logger.Warn(opis, text, args);
                }
                else if (logType == LogType.Error)
                {
                    logger.Error(opis, text, args);
                }
            }
        }

        public void LoadingFormData(JData data, string infoOpis, Structure.Models.ConnWithTran connTranModel, Dictionary<string, int> dataTablesList = null)
        {
            bool stopActions = false;
            DataSet ds = GetDataSet(data, infoOpis, out stopActions, connTranModel);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (CurrentForm.DataList.ContainsKey(data.Name))
                {
                    CurrentForm.DataList[data.Name] = ds.Tables[0];
                }
                else
                {
                    if (dataTablesList != null && dataTablesList.Count > 0)
                    {
                        foreach (var table in dataTablesList)
                        {
                            if (CurrentForm.DataList.ContainsKey(table.Key))
                                CurrentForm.DataList[table.Key] = ds.Tables[table.Value];
                            else
                                CurrentForm.DataList.Add(table.Key, ds.Tables[table.Value]);
                        }
                    }
                    else
                        CurrentForm.DataList.Add(data.Name, ds.Tables[0]);
                }
            }
        }

        public object GetDataValue(string dataValue)
        {
            dataValue = dataValue.Replace("#", "~");
            return ParseParameter(dataValue);
        }

        public Dictionary<string, object> GetParameters(List<JDataParameter> dataParameterList)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            if (dataParameterList != null && dataParameterList.Count > 0)
            {
                bool checkIfResult = true;
                foreach (JDataParameter par in dataParameterList)
                {
                    if (Conditions.CheckCondition(ref checkIfResult, par, CurrentForm, null) == false)
                        continue;

                    ParseParameter(parameters, par);
                }
            }
            return parameters;
        }

        private void ParseParameter(Dictionary<string, object> parameters, JDataParameter par)
        {
            object value = null;

            if (par.Type == "tablepar")
                ParseTableParameters(parameters, par);
            else
            {
                if (par.Value != null)
                {
                    value = ParseParameter(par.Value);
                    if (!string.IsNullOrEmpty(par.ValueTransform) && value != null)
                    {
                        value = ApplyValueTransform(par.ValueTransform, value);
                    }
                    value = Common.Helpers.Conversion.Parse(par.Type, value);
                }

                if (value == null)
                    value = DBNull.Value;

                parameters.Add(par.Name, value);
            }
        }

        private void ParseTableParameters(Dictionary<string, object> parameters, JDataParameter par)
        {
            object value = null;

            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            foreach (var tablepar in par.TableParList)
            {
                DataRow dr = dt.NewRow();

                value = ParseParameter(tablepar.Value);
                value = Common.Helpers.Conversion.Parse(tablepar.Type, value);

                if (value == null)
                    value = DBNull.Value;

                dr["Name"] = tablepar.Name;
                dr["Value"] = value;
                dt.Rows.Add(dr);
            }

            parameters.Add(par.Name, dt);
        }

        private object ParseParameter(string parameter)
        {
            using (Parser parser = new Parser(CurrentForm, null))
            {
                return parser.GetLogicPartValue(parameter);
            }
        }

        private static object ApplyValueTransform(string transformName, object value)
        {
            string s = value?.ToString() ?? "";
            if (string.IsNullOrEmpty(s)) return value;

            switch (transformName?.ToLowerInvariant())
            {
                case "daterangenowarehouse":
                    if (s.Contains("=") && !s.Contains("+"))
                        return s.Replace("=", "#");
                    break;
                case "commatotilde":
                    return s.Replace(", ", "~");
            }
            return value;
        }

        public JData GetDataByName(string dataName)
        {
            bool isGlobal = false;
            string[] dataNames = dataName.Split('.');
            // treba uci a nece ako postoje 3 string splita... 1)global 2)dt.. 3)@parametar
            // - ulazi u krivi uvjet dolje, krivi scope dataliste trazi, vrati null jer ga nema tamo
            if (dataNames.Length >= 2 && dataNames[0].ToLower() == "global")
            {
                isGlobal = true;
                dataName = dataNames[1];
            }
            dataName = dataName.Replace("~", "");

            JData jData = null;
            JDataList jDataList;

            if (this.DefinitionRoot != null)
                jDataList = this.DefinitionRoot.DataList;
            else if (isGlobal == true)
                jDataList = CurrentForm.DefinitionRoot.DataList;
            else
                jDataList = CurrentForm.DefinitionForm.DataList;

            if (jDataList != null)
            {
                var jDataDef = jDataList.Data.Where(x => x.Name.ToLower() == dataName.ToLower());

                bool checkIfResult = true;
                if (jDataDef.Count() > 0)
                {
                    foreach (JData data in jDataDef)
                    {
                        if (Conditions.CheckCondition(ref checkIfResult, data, CurrentForm, null) == true)
                        {
                            jData = data;
                            break;
                        }
                    }
                }

                return jData;
            }

            return null;
        }

        public object GetDataTableValue(string formDataTableName, string fieldName, bool getAll = false, bool isXmlGen = false,
            Dictionary<string, DataTable> dtForwarded = null)
        {
            DataTable dt;

            int rowIndex = CheckDataTableRowIndex(ref formDataTableName);

            if (!isXmlGen)
            {
                Dictionary<string, DataTable> dataList = CurrentForm.DataList;
                if (!dataList.ContainsKey(formDataTableName))
                    return null;

                dt = CurrentForm.DataList[formDataTableName];
            }
            else
            {
                dt = dtForwarded.Where(x => x.Key == formDataTableName).FirstOrDefault().Value;

                if (fieldName == "rowhandle")
                    return rowIndex;
                else if (fieldName == "rowindex")
                    return rowIndex + 1;
                else if (fieldName == "rowcount")
                    return dt.Rows.Count;
            }

            if (dt != null)
            {
                if (getAll)
                    return dt;
                else if (dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains(fieldName))
                    {
                        object value = dt.Rows[rowIndex][fieldName];
                        if (value is System.DBNull)
                            return null;

                        return dt.Rows[rowIndex][fieldName];
                    }
                }
            }

            return null;
        }

        public int CheckDataTableRowIndex(ref string dtName)
        {
            try
            {
                if (dtName.Contains("[") && dtName.EndsWith("]"))
                {
                    int hlpIndex = dtName.LastIndexOf('[');
                    string hlpStr = dtName.Substring(0, hlpIndex);

                    string hlpIndex1 = dtName.Substring(hlpIndex);
                    string number = hlpIndex1.Substring(0, hlpIndex1.Length - 1).Substring(1);

                    int tempLevel;
                    bool isNumeric = int.TryParse(number, out tempLevel);
                    if (isNumeric != false)
                    {
                        dtName = hlpStr;
                        return tempLevel;
                    }
                }
            }
            catch
            {
                return 0;
            }

            return 0;
        }

        public object GetByAttribute(string formDataTableName, string attribute)
        {
            Dictionary<string, DataTable> dataList = CurrentForm.DataList;
            if (!dataList.ContainsKey(formDataTableName))
                return null;

            DataTable dt = CurrentForm.DataList[formDataTableName];
            if (attribute == "rowcount")
                return dt.Rows.Count;

            return null;
        }

        public static string FindDataColumn(string name, string[] dataColumns)
        {
            string result = Array.Find(dataColumns, s => s.Equals(name, StringComparison.OrdinalIgnoreCase));
            return result;
        }

        private void CheckOutputParameters(List<Carpo.Data.Models.OutputParametersModel.OutParModel> outputParList, List<JDataParameter> parList)
        {
            if (outputParList.Count > 0)
            {
                foreach (var outPar in outputParList)
                {
                    string variableName = outPar.Name.Replace("@", "");

                    var par = parList.Where(x => x.Name.Split('.')[0] == outPar.Name).FirstOrDefault();
                    if (par != null && !string.IsNullOrEmpty(par.VariableName))
                        variableName = par.VariableName;

                    Carpo.UI.Logic.FormsVar.SetVariable(CurrentForm, variableName, outPar.Value);
                }
            }
        }
    }
}
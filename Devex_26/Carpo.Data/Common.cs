using Carpo.Common;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Carpo.Data
{
    public class Common
    {
        public enum DataType { StoredProcedure, Text }

        /// <summary>When false, skip CommandAsSql param expansion (CommandText only). Default true.</summary>
        public static bool ExpandSqlParametersForLog { get; set; } = true;

        private static void EnsureArithAbort(SqlConnection connection, DataType dataType, ref string commandText)
        {
            if (connection == null) return;
            // Text batches: fold SET into the same round-trip (skips extra ExecuteNonQuery).
            if (dataType == DataType.Text)
            {
                if (!string.IsNullOrEmpty(commandText) &&
                    commandText.IndexOf("ARITHABORT", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    commandText = "SET ARITHABORT ON; " + commandText;
                }
                return;
            }
            using (var commandArithAbort = new SqlCommand("SET ARITHABORT ON", connection))
            {
                commandArithAbort.ExecuteNonQuery();
            }
        }

        private static string FormatSqlForLog(SqlCommand command)
        {
            if (command == null) return string.Empty;
            if (!ExpandSqlParametersForLog) return command.CommandText ?? string.Empty;
            return Log.CommandAsSql(command);
        }

        private static List<Models.OutputParametersModel.OutParModel> OutParams;

        public static DataSet ExecuteQuery(string connectionString, string commandText, DataType dataType, Dictionary<string, Object> par)
        {
            string sqlText = "";
            try
            {
                return ExecuteQuery(connectionString, commandText, dataType, par, out sqlText, 30, out OutParams);
            }
            catch { throw; }
        }

        public static DataSet ExecuteQuery(string connectionString, string commandText, DataType dataType, Dictionary<string, Object> par, out string sqlText)
        {
            try
            {
                return ExecuteQuery(connectionString, commandText, dataType, par, out sqlText, 30, out OutParams);
            }
            catch { throw; }
        }

        public static DataSet ExecuteQuery(string connectionString, string commandText, DataType dataType, Dictionary<string, Object> par, out string sqlText, int commandTimeout, out List<Models.OutputParametersModel.OutParModel> outParModel)
        {
            var command = new SqlCommand();
            DataSet ds = new DataSet();
            outParModel = null;
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SetInfoMsgHelper(par, conn);

                using (var connection = conn)
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    string commandTextLocal = commandText;
                    EnsureArithAbort(connection, dataType, ref commandTextLocal);

                    using (command = new SqlCommand(commandTextLocal, connection))
                    {
                        if (dataType == DataType.StoredProcedure)
                            command.CommandType = CommandType.StoredProcedure;

                        command.CommandTimeout = commandTimeout;

                        if (par != null)
                        {
                            foreach (KeyValuePair<string, Object> a in par)
                            {
                                if (!a.Key.Contains(".output"))
                                    command.Parameters.Add(ToConvertSqlParams(command, a.Key, a.Value));
                            }

                            outParModel = CreateNewDictionary(par);
                            par = ParseOutputParameters(par);

                            foreach (var outP in outParModel)
                            {
                                foreach (var p in par)
                                {
                                    if (p.Key == outP.Name)
                                    {
                                        if (outP.Type == SqlDbType.VarChar) // varcharu moramo dati i duljinu (-1 označava varchar(max))
                                            command.Parameters.Add(outP.Name, SqlDbType.VarChar, -1).Direction = ParameterDirection.InputOutput;
                                        else
                                            command.Parameters.Add(outP.Name, outP.Type).Direction = ParameterDirection.InputOutput;
                                        break;
                                    }
                                }
                            }
                        }

                        sqlText = FormatSqlForLog(command);

                        if (par != null && CheckForDataTableParam(par))
                            sqlText = CheckSqlTextForDataTableParm(sqlText, par);

                        if (outParModel != null && outParModel.Count > 0)
                            sqlText = CheckSqlTextForOutputParm(sqlText, outParModel);

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            
                            //if (CheckIfProcedureExist(connection, command))
                            //{
                                adapter.Fill(ds);
                            //}
                            //else
                            //{
                            //    throw new Exception("Could not find stored procedure '" + command.CommandText + "'.");
                            //}
                        }
                    }

                    connection.Close();
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (outParModel != null && outParModel.Count > 0)
                {
                    foreach (var param in outParModel)
                    {
                        var objValue = command.Parameters[param.Name].Value;
                        param.Value = objValue;
                    }
                }

                command = null;
            }
        }

        public static object ExecuteNonQuery(string connectionString, string commandtext, DataType dataType, Dictionary<string, Object> par, out string sqlText, int commnadTimeout)
        {
            try
            {
                return ExecuteAzur(connectionString, commandtext, dataType, par, false, out sqlText, commnadTimeout);
            }
            catch { throw; }
        }

        public static object ExecuteScalar(string connectionString, string commandtext, DataType dataType, Dictionary<string, Object> par, out string sqlText, int commnadTimeout)
        {
            try
            {
                return ExecuteAzur(connectionString, commandtext, dataType, par, true, out sqlText, commnadTimeout);
            }
            catch { throw; }
        }

        private static object ExecuteAzur(string connectionString, string commandtext, DataType dataType, Dictionary<string, Object> par, bool returnId, out string sqlText, int commnadTimeout)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SetInfoMsgHelper(par, conn);

            object value = null;
            using (var connection = conn)
            {
                try
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    string commandTextLocal = commandtext;
                    EnsureArithAbort(connection, dataType, ref commandTextLocal);

                    using (var command = new SqlCommand(commandTextLocal, connection))
                    {
                        if (dataType == DataType.StoredProcedure)
                            command.CommandType = CommandType.StoredProcedure;

                        command.CommandTimeout = commnadTimeout;

                        foreach (KeyValuePair<string, Object> a in par)
                        {
                            command.Parameters.Add(ToConvertSqlParams(command, a.Key, a.Value));
                        }

                        sqlText = FormatSqlForLog(command);

                        if (par != null && CheckForDataTableParam(par))
                            sqlText = CheckSqlTextForDataTableParm(sqlText, par);

                        if (returnId == true)
                            value = command.ExecuteScalar();
                        else
                            command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    throw;
                }

                connection.Close();
            }

            return value;
        }

        public static DataSet ExecuteQueryInTran(string connectionString, string commandText, DataType dataType, Dictionary<string, Object> par, out string sqlText, int commnadTimeout, SqlConnection connImported, SqlTransaction tranImported)
        {
            DataSet ds = new DataSet();

            SqlConnection connection = connImported;
            SetInfoMsgHelper(par, connection);

            SqlCommand command = new SqlCommand(commandText, connection, tranImported);

            try
            {
                if (dataType == DataType.StoredProcedure)
                    command.CommandType = CommandType.StoredProcedure;

                command.CommandTimeout = commnadTimeout;

                if (par != null)
                {
                    foreach (KeyValuePair<string, Object> a in par)
                    {
                        command.Parameters.Add(ToConvertSqlParams(command, a.Key, a.Value));
                    }
                }

                sqlText = FormatSqlForLog(command);

                if (par != null && CheckForDataTableParam(par))
                    sqlText = CheckSqlTextForDataTableParm(sqlText, par);

                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(ds);
                }

                return ds;
            }
            catch { throw; }
        }

        public static object ExecuteAzurInTran(string connectionString, string commandtext, DataType dataType, Dictionary<string, Object> par, bool returnId, out string sqlText, int commnadTimeout, SqlConnection connImported, SqlTransaction tranImported)
        {
            SqlConnection connection = connImported;
            SetInfoMsgHelper(par, connection);

            SqlCommand command = new SqlCommand(commandtext, connection, tranImported);
            object value = null;

            try
            {
                if (dataType == DataType.StoredProcedure)
                    command.CommandType = CommandType.StoredProcedure;

                command.CommandTimeout = commnadTimeout;

                foreach (KeyValuePair<string, Object> a in par)
                {
                    command.Parameters.Add(ToConvertSqlParams(command, a.Key, a.Value));
                }

                sqlText = FormatSqlForLog(command);

                if (par != null && CheckForDataTableParam(par))
                    sqlText = CheckSqlTextForDataTableParm(sqlText, par);

                if (returnId == true)
                    value = command.ExecuteScalar();
                else
                    command.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }

            return value;
        }

        public static SqlParameter ToConvertSqlParams(SqlCommand command, string name, object value)
        {
            var p = command.CreateParameter();
            p.ParameterName = name;
      
            p.Value = value;
            return p;
        }

        public static void CarpoLogInsert(string logLevel, string opis, string text, string errorString, int operaterid, params object[] args)
        {
            string spName = "spCarpoLogInsert";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@LogTip", logLevel },
                    { "@Opis", opis },
                    { "@ErrorOpis", text },
                    { "@ErrorTekst", errorString },
                    { "@operateriid", operaterid }
                };

            string sqlText = "";
            Carpo.Data.Common.ExecuteNonQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 20);
        }

        public static bool CheckForDataTableParam(Dictionary<string, Object> par)
        {
            foreach (var p in par)
            {
                var tableObj = p.Value as System.Data.DataTable;
                if (tableObj != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static string CheckSqlTextForDataTableParm(string sqlText, Dictionary<string, Object> par)
        {
            string helperStr = "";
            foreach (var p in par)
                sqlText = GetDataTableValueString(helperStr, sqlText, p);

            return sqlText;
        }

        public static string GetDataTableValueString(string helperStr, string sqlText, KeyValuePair<string, Object> param)
        {
            var tableObj = param.Value as System.Data.DataTable;
            if (tableObj != null)
            {
                for (int i = 0; i < tableObj.Rows.Count; i++)
                {
                    var dr = tableObj.Rows[i];
                    helperStr += (dr[0].ToString());
                    if (i < tableObj.Rows.Count - 1)
                        helperStr += ",";
                }
                if (string.IsNullOrEmpty(sqlText))  //ako pozivam pri punjenju loggera onda je sqlText = "", i želim samo string array
                    return helperStr;

                int helperIndex = sqlText.IndexOf(param.Key) + param.Key.Length;
                sqlText = sqlText.Substring(0, helperIndex + 1) + "'" + helperStr + "'" + sqlText.Substring(helperIndex + 1);
            }

            return sqlText;
        }

        public static string CheckSqlTextForOutputParm(string sqlText, List<Models.OutputParametersModel.OutParModel> par)
        {
            foreach (var p in par)
            {
                sqlText = sqlText.Replace(p.Name + "=NULL", p.Name + " OUT");
            }

            return sqlText;
        }

        public static List<Models.OutputParametersModel.OutParModel> CreateNewDictionary(Dictionary<string, Object> par)
        {
            var outPar = new List<Models.OutputParametersModel.OutParModel>();

            foreach (var p in par)
            {
                if (p.Key.Contains(".output"))
                {
                    var strList = p.Key.Split('.');
                    if (strList.Length != 3)
                        continue;

                    var model = new Models.OutputParametersModel.OutParModel()
                    {
                        Name = strList[0],
                        TypeSet = strList[2]
                    };
                    outPar.Add(model);
                }
            }

            return outPar;
        }

        public static Dictionary<string, Object> ParseOutputParameters(Dictionary<string, Object> par)
        {
            Dictionary<string, Object> newParDict = new Dictionary<string, Object>();

            foreach (var p in par)
            {
                if (p.Key.Contains(".output"))
                {
                    int tempIndex = p.Key.IndexOf(".output");
                    newParDict.Add(p.Key.Substring(0, tempIndex), p.Value);
                    continue;
                }

                newParDict.Add(p.Key, p.Value);
            }

            return newParDict;
        }

        public static void OnInfoMessageGenerated(object sender, SqlInfoMessageEventArgs args)
        {
            if (!args.Message.StartsWith("Warning:"))
                XtraMessageBox.Show(Translator.TranslateString(args.Message), Translator.TranslateString("Info"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void SetInfoMsgHelper(Dictionary<string, Object> par, SqlConnection conn)
        {
            if (par != null)
                foreach (var p in par)
                {
                    if (p.Key == "RaiseInfoMsg" && p.Value.ToString() == "true")
                    {
                        conn.InfoMessage += OnInfoMessageGenerated;
                        par.Remove(p.Key.ToString());
                        break;
                    }
                }
        }

        //private static bool CheckIfProcedureExist(SqlConnection connection, SqlCommand command)
        //{
        //    bool exist = false;
        //    using (SqlConnection conn = new SqlConnection(Carpo.Data.Constructor.SetConnectionString(connection.DataSource, "JupiterSystem", "", "")))
        //    {
        //        using (var comm = new SqlCommand("spCheckIfProcedureExist", conn))
        //        {
        //            try
        //            {
        //                conn.Open();
        //                comm.CommandType = CommandType.StoredProcedure;
        //                comm.Parameters.AddWithValue("database", connection.Database);
        //                comm.Parameters.AddWithValue("procedure", command.CommandText);
        //                exist = (bool)comm.ExecuteScalar();
        //            }
        //            catch { throw; }
        //        }
        //    }

        //    return exist;
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Carpo.Data.Set
{
    public class Vars
    {
        public static void SetSource(string connectionString,
            int operateriId, string source, string form, string name, string type, string value, string version, out string sqlText)
        {
            try
            {
                SetSource(connectionString, operateriId, source, form, name, type, value, null, version, out sqlText);
            }
            catch
            {
                throw;
            }
        }

        public static void SetSourceXml(string connectionString,
            int operateriId, string source, string form, string name, string type, string valueXml, string version, out string sqlText)
        {
            try
            {
                SetSource(connectionString, operateriId, source, form, name, type, null, valueXml, version, out sqlText);
            }
            catch
            {
                throw;
            }
        }

        public static void SetAppControlValues(string connectionString, int operateriId, string source, string form, string componentname,
            string value, string valuetype, string valuetext, out string sqlText)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                using (var connection = conn)
                {
                    using (var command = new SqlCommand("spCarpoAppControlValuesAzur", connection))
                    {
                        conn.Open();
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Action", "save");
                        command.Parameters.AddWithValue("@OperateriId", operateriId);
                        command.Parameters.AddWithValue("@Source", source);
                        command.Parameters.AddWithValue("@Form", form);
                        command.Parameters.AddWithValue("@ComponentName", componentname);
                        if (value == "null")
                            command.Parameters.AddWithValue("@Value", DBNull.Value);
                        else
                            command.Parameters.AddWithValue("@Value", value);
                        command.Parameters.AddWithValue("@ValueType", valuetype);
                        command.Parameters.AddWithValue("@ValueText", valuetext);

                        sqlText = Log.CommandAsSql(command);
                        command.ExecuteNonQuery();

                        connection.Close();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static DataSet GetAppControlValues(string connectionString, int operateriId, string source, string form, out string sqlText)
        {
            try
            {
                string spName = "spCarpoAppControlValuesQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "get" },
                    { "@operateriid", operateriId },
                    { "@source", source },
                    { "@form", form }
                };

                DataSet ds = Carpo.Data.Common.ExecuteQuery(connectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public static void SetSource(string connectionString,
            int operateriId, string source, string form, string name, string type,
            string value, string valueXml, string version,
            out string sqlText, bool isSearchForm = false, string previousLayoutName = "")
        {
            SqlConnection conn = new SqlConnection(connectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoVarsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    if (isSearchForm && value != null)
                        command.Parameters.AddWithValue("Action", "saveSearchFormSize");
                    else if (isSearchForm && value == null)
                        command.Parameters.AddWithValue("Action", "saveSearchFormXML");
                    else
                        command.Parameters.AddWithValue("Action", "save");

                    if (!string.IsNullOrEmpty(previousLayoutName))
                        name = name + "#" + previousLayoutName;

                    command.Parameters.AddWithValue("OperateriId", operateriId);
                    command.Parameters.AddWithValue("Source", source);
                    command.Parameters.AddWithValue("Form", form);
                    command.Parameters.AddWithValue("ComponentName", name);
                    command.Parameters.AddWithValue("ComponentType", type);
                    command.Parameters.AddWithValue("Value", value);
                    command.Parameters.AddWithValue("Version", version);

                    command.Parameters.Add("ValueXml", SqlDbType.Xml);
                    command.Parameters["ValueXml"].Value = valueXml;

                    command.ExecuteNonQuery();

                    connection.Close();
                    sqlText = "";
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Data;

namespace Carpo.Data.Get
{
    public class GridTemplates
    {
        public static DataTable GetTemplatesQuery(string connStr, int operateriId, string source,
            string form, string componentName)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "masterQuery"},
                    { "@OperateriId", operateriId},
                    { "@source", source},
                    { "@form", form},
                    { "@componentname", componentName}
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        public static List<string> GetTemplateColumns(string connStr, int carpogridglaid)
        {
            List<string> columnList = new List<string>();
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getTemplateColumns"},
                    { "@carpogridglaid", carpogridglaid},
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            columnList.Add(ds.Tables[0].Rows[i]["ColumnName"].ToString());
                        }
                    }
                }

                return columnList;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetVisibleColumns(string connStr, int operateriId, string source, string form, string componentName)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getVisibleColumns"},
                    { "@OperateriId", operateriId},
                    { "@source", source},
                    { "@form", form},
                    { "@componentname", componentName}
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);

                return ds.Tables[0];
            }
            catch
            {
                throw;
            }
        }

        public static DataTable PrintDialogComboFill(string connStr, int operateriId, string source, string form, string componentName)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "printDialogComboTemplates"},
                    { "@OperateriId", operateriId},
                    { "@source", source},
                    { "@form", form},
                    { "@componentname", componentName}
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);

                return ds.Tables[0];
            }
            catch
            {
                throw;
            }
        }

        public static DataSet DataSetForPrinting(string action, string connStr, int operateriId, string source, string form, string componentName)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", action},
                    { "@OperateriId", operateriId},
                    { "@source", source},
                    { "@form", form},
                    { "@componentname", componentName}
                };

                string sqlText = "";
                return Carpo.Data.Common.ExecuteQuery(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
            }
            catch
            {
                throw;
            }
        }
    }
}
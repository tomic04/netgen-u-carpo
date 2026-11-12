using System;
using System.Collections.Generic;

namespace Carpo.Data.Set
{
    public class GridTemplates
    {
        public static int InsertTemplate
            (string connStr, string naziv, int operateriId, string source, string form, string componentName,
            bool zaIspis, bool isJavan, out string sqlText)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "insert" },
                    { "@Naziv", naziv },
                    { "@OperateriId", operateriId },
                    { "@Source", source },
                    { "@Form", form },
                    { "@ComponentName", componentName },
                    { "@ZaIspis", zaIspis },
                    { "@IsJavan", isJavan }
                };

                return Convert.ToInt32(Carpo.Data.Common.ExecuteScalar(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30));
            }
            catch
            {
                throw;
            }
        }

        public static int EditTemplate(string connStr, string naziv, int operateriId, bool zaIspis, bool isJavan, int id, out string sqlText)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "update" },
                    { "@Naziv", naziv },
                    { "@OperateriId", operateriId },
                    { "@ZaIspis", zaIspis },
                    { "@IsJavan", isJavan },
                    { "@id", id }
                };

                return Convert.ToInt32(Carpo.Data.Common.ExecuteScalar(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30));
            }
            catch
            {
                throw;
            }
        }

        public static int DeleteTemplate(string connStr, int operateriId, int carpoGridGlaId, out string sqlText)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "delete" },
                    { "@CarpoGridGlaId", carpoGridGlaId },
                    { "@OperateriId", operateriId }
                };

                return Convert.ToInt32(Carpo.Data.Common.ExecuteScalar(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30));
            }
            catch
            {
                throw;
            }
        }

        public static void SetTemplateForUse(string connStr, int operateriId, string source, string form,
            string componentName, int carpoGridGlaId, out string sqlText)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "postaviPredlozak" },
                    { "@OperateriId", operateriId },
                    { "@Source", source },
                    { "@Form", form },
                    { "@ComponentName", componentName },
                    { "@CarpoGridGlaId", carpoGridGlaId }
                };

                Common.ExecuteNonQuery(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }

        public static void StopUsingTemplate(string connStr, int operateriId, string source, string form,
            string componentName, int? carpogridglaid, out string sqlText)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "prestaniKoristitiTemplate" },
                    { "@OperateriId", operateriId },
                    { "@Source", source },
                    { "@Form", form },
                    { "@ComponentName", componentName },
                    { "@CarpoGridGlaId", carpogridglaid }
                };

                Common.ExecuteNonQuery(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }

        public static void SetColumnsForTemplate(string connStr, string selectedIds, int carpoGridGlaId, out string sqlText)
        {
            try
            {
                string spName = "spCarpoGridColumns";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@Action", "insertTemplateColumns" },
                    { "@CarpoGridGlaId", carpoGridGlaId },
                    { "@selectedids", selectedIds }
                };

                Common.ExecuteNonQuery(connStr, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 30);
            }
            catch
            {
                throw;
            }
        }
    }
}
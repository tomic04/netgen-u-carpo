using System;
using System.Data;
using System.Data.SqlClient;

namespace Carpo.Data
{
    public class Log
    {
        public static string ParameterValueForSQL(SqlParameter sp)
        {
            try
            {
                string retval = "";

                switch (sp.SqlDbType)
                {
                    case SqlDbType.Char:
                    case SqlDbType.NChar:
                    case SqlDbType.NText:
                    case SqlDbType.NVarChar:
                    case SqlDbType.Text:
                    case SqlDbType.Time:
                    case SqlDbType.VarChar:
                    case SqlDbType.Xml:
                        if (sp.Value == null || sp.Value == DBNull.Value)
                            retval = "NULL";
                        else
                            retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                        break;

                    case SqlDbType.Date:
                    case SqlDbType.DateTime:
                    case SqlDbType.DateTime2:
                    case SqlDbType.DateTimeOffset:
                        if (sp.Value == null || sp.Value == DBNull.Value)
                            retval = "NULL";
                        else
                        {
                            //retval = "'" + string.Format("{0:yyyyMMdd HH:mm:ss}", sp.Value).Replace("'", "''") + "'";
                            retval = "'" + string.Format("{0:yyyyMMdd HH:mm:ss}", sp.Value).Replace("'", "''") + "'";
                        }

                        break;

                    case SqlDbType.Int:
                    case SqlDbType.BigInt:
                    case SqlDbType.SmallInt:
                    case SqlDbType.TinyInt:
                        if (sp.Value == null || sp.Value == DBNull.Value)
                            retval = "NULL";
                        else
                            retval = sp.Value.ToString();
                        break;

                    case SqlDbType.Bit:
                        if (sp.Value == null || sp.Value == DBNull.Value)
                            retval = "NULL";
                        else
                            retval = ((bool)sp.Value == false) ? "0" : "1";
                        break;

                    default:
                        if (sp.Value == null || sp.Value == DBNull.Value)
                            retval = "NULL";
                        else
                            retval = sp.Value.ToString().Replace("'", "''");
                        break;
                }

                return retval;
            }
            catch
            {
                return "???";
            }
        }

        public static string CommandAsSql(SqlCommand sc)
        {
            string sql = sc.CommandText;

            sql = sql.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            sql = System.Text.RegularExpressions.Regex.Replace(sql, @"\s+", " ");

            int i = 0;
            foreach (SqlParameter sp in sc.Parameters)
            {
                string spName = sp.ParameterName;
                string spValue = ParameterValueForSQL(sp);
                if (sc.CommandType == CommandType.Text)
                    sql = sql.Replace(spName, spValue);
                else
                    sql += (i > 0 ? ", " : " ") + spName + "=" + spValue;

                i++;
            }
            return sql;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Carpo.Data
{
    public class Parameters
    {
        public static SqlParameter[] GetStoredProcedureParameters(string connectionString, string storedProcedure, Dictionary<string, object> populatedParameters)
        {
            SqlParameter[] spPars = SqlHelperParameterCache.GetSpParameterSet(connectionString, storedProcedure);

            for (int i = 0, j = spPars.Length; i < j; i++)
            {
                var value = spPars[i].Value as IDbDataParameter;
                if (populatedParameters != null && populatedParameters.ContainsKey(spPars[i].ParameterName))
                {
                    spPars[i].Value = populatedParameters[spPars[i].ParameterName];
                }
                else if (value == null)
                {
                    spPars[i].Value = DBNull.Value;
                }
            }
            return spPars;
        }
    }
}
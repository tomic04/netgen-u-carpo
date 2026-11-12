using System;

namespace Carpo.Common.Helpers
{
    public class Db
    {
        public static object SetDbValue(object value)
        {
            if (value == null || value.ToString() == "")
                return DBNull.Value;
            else
                return value;
        }
    }
}
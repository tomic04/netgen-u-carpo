using System;

namespace Carpo.Common.Helpers
{
    public static class ValueHelpers
    {
        public static string CoalesceStr(this object value, string newValue = "")
        {
            if (value == null)
                return newValue;

            return value == (DBNull.Value) ? newValue : value.ToString().Trim();
        }

        public static int? CoalesceInt(this object value, int? newValue = null)
        {
            if (value == null || value == DBNull.Value)
                return newValue;

            int num;
            if (int.TryParse(value.ToString(), out num))
                return num;

            return newValue;
        }
    }
}
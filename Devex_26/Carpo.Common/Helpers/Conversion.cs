using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace Carpo.Common.Helpers
{
    public static class Conversion
    {
        /// <summary>
        /// Gets the specified property value and converts it, if necessary.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="o">The object to convert.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T Convert<T>(this object o, T defaultValue = default(T))
        {
            if (o == null) return defaultValue;
            o = o.Convert(typeof(T), defaultValue);
            return o is T ? (T)o : defaultValue;
        }

        /// <summary>
        /// Gets the specified property value and converts it, if necessary.
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <param name="type">The type to convert to.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static object Convert(this object o, Type type, object defaultValue)
        {
            if (o == null || o is DBNull) return defaultValue;
            if (type.IsInstanceOfType(o)) return o;

            try
            {
                object result;
                var s = o as string;

                if (s != null)
                {
                    if (s.Length == 0) return defaultValue;
                    result = Parse(type, s);
                    if (result != null) return result;
                }

                var valueType = o.GetType();
                var tc = TypeDescriptor.GetConverter(valueType);

                if (tc.CanConvertTo(type))
                {
                    result = tc.ConvertTo(o, type);
                    return result;
                }

                tc = TypeDescriptor.GetConverter(type);

                if (tc.CanConvertFrom(valueType))
                {
                    result = tc.ConvertFrom(o);
                    return result;
                }

                result = Parse(type, o) ?? defaultValue;
                return result;
            }
            catch
            {
                return Parse(type, o.ToString()) ?? defaultValue;
            }
        }

        /// <summary>
        /// Parses the specified object into the specified type.
        /// </summary>
        /// <param name="type">The type to parse into.</param>
        /// <param name="o">The object to parse.</param>
        /// <returns>An object of the specified type, if successfull; otherwise, null.</returns>
        public static object Parse(Type type, object o)
        {
            return Parse(type.Name, o);
        }

        public static object Parse(string typeStr, object o)
        {
            if (string.IsNullOrEmpty(typeStr))// || typeStr.ToLower().StartsWith("str"))
                return o;

            //typeStr = typeStr.Replace(".output", "");

            if (o == null) return null;
            var text = o.ToString();
            if (string.IsNullOrEmpty(text) && typeStr.ToLower() != "string") return null;

            object result = null;

            switch (typeStr.ToLower())
            {
                case "uint16":
                    {
                        UInt16 v2;
                        if (UInt16.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out v2)) result = v2;
                        break;
                    }
                case "uint32":
                    {
                        UInt32 v2;
                        if (UInt32.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out v2)) result = v2;
                        break;
                    }
                case "uint64":
                    {
                        UInt64 v2;
                        if (UInt64.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out v2)) result = v2;
                        break;
                    }
                case "int16":
                    {
                        Int16 v2;
                        if (Int16.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out v2)) result = v2;
                        break;
                    }
                case "int32":
                case "int":
                    {
                        Int32 v2;
                        if (Int32.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out v2)) result = v2;
                        break;
                    }
                case "int64":
                    {
                        Int64 v2;
                        if (Int64.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out v2)) result = v2;
                        break;
                    }
                case "byte":
                    {
                        Byte v2;
                        if (Byte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out v2)) result = v2;
                        break;
                    }
                case "boolean":
                case "bool":
                    {
                        result = BooleanValues.Contains(text);
                        break;
                    }
                case "decimal":
                case "dec":
                    {
                        decimal v2;
                        //if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out v2)) result = v2;
                        if (decimal.TryParse(text, NumberStyles.Any, new NumberFormatInfo() { NumberDecimalSeparator = "," }, out v2)) result = v2;
                        break;
                    }
                case "datetime":
                case "date":
                    {
                        DateTime v2;

                        if (o is DateTime)
                            result = o;
                        else if (text.ToLower() == "now")
                            result = DateTime.Now.Date;
                        else if (text.ToLower().Replace(" ", "") == "nowtime")
                            result = DateTime.Now;
                        else if (DateTime.TryParse(text, CultureInfo.CurrentCulture,
                                                DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces,
                                                out v2)) result = v2;
                        else if (DateTime.TryParseExact(text, "yyyyMMdd", CultureInfo.InvariantCulture,
                                                DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces,
                                                out v2)) result = v2;
                        break;
                    }
                case "datetimeoffset":
                    {
                        DateTimeOffset v2;
                        if (DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture,
                                                    DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces,
                                                    out v2)) result = v2;
                        break;
                    }
                case "timespan":
                    {
                        TimeSpan v2;
                        if (TimeSpan.TryParse(text, CultureInfo.InvariantCulture, out v2)) result = v2;
                        break;
                    }
                case "datatable":
                    {
                        var dt = o as DataTable;
                        if (dt != null)
                            result = dt;
                        break;
                    }
                case "list`1":
                    {
                        var list = new List<string>();
                        if (!string.IsNullOrEmpty(o.ToString()))
                            list.Add(o.ToString());
                        result = list;

                        break;
                    }
                case "varbinary": // u objekt vrijednosti 'o' dolazi putanja sa koje učitavamo byte[]
                    {
                        if (o is Bitmap)
                        {
                            var t = o as Bitmap;
                            var mem = new MemoryStream();
                            t.Save(mem, ImageFormat.Png);
                            result = mem.ToArray();
                            break;
                        }
                        else
                        {
                            var fileObj = File.ReadAllBytes(o.ToString());
                            if (fileObj != null)
                                result = fileObj;
                            break;
                        }
              
                    }
                case "string":
                    result = o.ToString();
                    break;
            }

            return result;
        }

        private static readonly HashSet<string> BooleanValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "true", "1", "on", "yes", "ja" };

        public static bool TryCast<T>(this object obj, out T result)
        {
            if (obj is T)
            {
                result = (T)obj;
                return true;
            }

            result = default(T);
            return false;
        }

        public static T TryParse<T>(string inValue)
        {
            TypeConverter converter =
                TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromString(null,
                CultureInfo.InvariantCulture, inValue);
        }

        public static bool IsString(object value)
        {
            return value is string;
        }
    }
}
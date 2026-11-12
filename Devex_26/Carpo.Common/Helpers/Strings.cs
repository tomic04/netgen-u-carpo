using System;
using System.Globalization;
using System.Linq;

namespace Carpo.Common.Helpers
{
    public class Strings
    {
        public static string AddSpacesToSentence(string sentence)
        {
            return AddSpacesToSentence(sentence, "");
        }

        public static string AddSpacesToSentence(string sentence, string cultureInfoName)
        {
            CultureInfo cultureInfo = null;
            if (cultureInfoName != "")
                cultureInfo = new CultureInfo(cultureInfoName);

            return AddSpacesToSentence(sentence, cultureInfo);
        }

        public static string AddSpacesToSentence(string sentence, CultureInfo cultureInfo)
        {
            string s = string.Concat(sentence.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
            if (cultureInfo == null)
                return ToTitleCase(s);
            return ToTitleCase(s, cultureInfo);
        }

        /// <summary>
        /// Use the current thread's culture info for conversion
        /// </summary>
        public static string ToTitleCase(string str)
        {
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Overload which uses the culture info with the specified name
        /// </summary>
        public static string ToTitleCase(string str, string cultureInfoName)
        {
            var cultureInfo = new CultureInfo(cultureInfoName);
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Overload which uses the specified culture info
        /// </summary>
        public static string ToTitleCase(string str, CultureInfo cultureInfo)
        {
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }

        public static DevExpress.XtraEditors.Mask.MaskType GetMaskTypeByString(string type)
        {
            switch (type.ToLower())
            {
                case "datetime": return DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
                case "datetimeadvancingcaret": return DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
                case "numeric": return DevExpress.XtraEditors.Mask.MaskType.Numeric;
                case "regex": return DevExpress.XtraEditors.Mask.MaskType.RegEx;
                case "regular": return DevExpress.XtraEditors.Mask.MaskType.Regular;
                case "simple": return DevExpress.XtraEditors.Mask.MaskType.Simple;
                default: return DevExpress.XtraEditors.Mask.MaskType.None;
            }
        }

        public static DevExpress.XtraEditors.Mask.AutoCompleteType GetMaskAutoCompleteType(string type)
        {
            switch (type.ToLower())
            {
                case "none": return DevExpress.XtraEditors.Mask.AutoCompleteType.None;
                case "optimistic": return DevExpress.XtraEditors.Mask.AutoCompleteType.Optimistic;
                case "strong": return DevExpress.XtraEditors.Mask.AutoCompleteType.Strong;
                default: return DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            }
        }
    }
}
using DevExpress.XtraEditors.Repository;
using System;

namespace Carpo.Common.Helpers
{
    public class Formats
    {
        public static void FormatTextEdit(string format, RepositoryItemTextEdit repositoryItemTextEdit)
        {
            if (string.IsNullOrEmpty(format))
                return;
            if (format.ToLower().StartsWith("f") ||
                format.ToLower().StartsWith("n") ||
                format.ToLower().StartsWith("d"))
            {
                repositoryItemTextEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                repositoryItemTextEdit.Mask.UseMaskAsDisplayFormat = true;
                repositoryItemTextEdit.Mask.EditMask = format;
                repositoryItemTextEdit.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
        }

        public static string FormatLabel(string format, object value)
        {
            object valueParsed = Common.Helpers.Conversion.Parse("decimal", value);
            if (valueParsed != null)
            {
                return string.Format("{0:" + format + "}", Convert.ToDecimal(valueParsed));
            }
            if (value != null)
                return value.ToString();

            return "";
        }
    }
}
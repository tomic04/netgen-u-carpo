using Carpo.Structure.Models.Core;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Carpo.Common.Helpers
{
    public class Grid
    {
        //public enum GridType { Master, Detail, Simple };
        //public static void FormatGrid(GridView gv)
        //{
        //    FormatGrid(gv, false);
        //}

        //public static void FormatGrid(GridView gv, bool hideIdFields)
        //{
        //    foreach (GridColumn col in gv.Columns)
        //    {
        //        switch (Type.GetTypeCode(col.ColumnType))
        //        {
        //            case TypeCode.Int32:
        //            case TypeCode.Int64:
        //                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        //                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        //                break;
        //            case TypeCode.Decimal:
        //            case TypeCode.Double:
        //            case TypeCode.Single:
        //                //default format
        //                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;

        //                string formatString = "n2";

        //                if (col.Tag is JField)
        //                {
        //                    var colTag = (col.Tag as JField);
        //                    if (!string.IsNullOrEmpty(colTag.FormatString))
        //                        formatString = colTag.FormatString;
        //                }
        //                col.DisplayFormat.FormatString = formatString;

        //                //set header caption to right
        //                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

        //                if (col.Tag == null || (col.Tag is JField && ((JField)col.Tag).ShowSum != false))
        //                {
        //                    CreateGroupSummary(gv, col.FieldName);

        //                    col.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
        //                    col.SummaryItem.DisplayFormat = "{0:" + formatString + "}";
        //                }

        //                break;
        //            case TypeCode.DateTime:
        //                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        //                col.DisplayFormat.FormatString = "dd.MM.yyyy";
        //                break;

        //        }
        //        //ako nije definiran unutar xml-a, onda sakri ako završava sa id
        //        if (!(col.Tag is JField) && col.FieldName.ToLower().EndsWith("id"))
        //            col.Visible = false;

        //        if (col.Tag is JField && (col.Tag as JField).Editable == true)
        //        {
        //            RepositoryItemTextEdit repTxt = new RepositoryItemTextEdit();
        //            gv.GridControl.RepositoryItems.Add(repTxt);
        //            repTxt.Mask.EditMask = col.DisplayFormat.FormatString;
        //            if (col.DisplayFormat.FormatType == DevExpress.Utils.FormatType.Numeric)
        //                repTxt.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
        //            repTxt.Mask.UseMaskAsDisplayFormat = true;
        //            col.ColumnEdit = repTxt;
        //            col.OptionsColumn.AllowEdit = true;
        //        }
        //        else
        //            col.OptionsColumn.AllowEdit = false;

        //    }

        //}

        //private static void CreateGroupSummary(GridView gv, string fieldName)
        //{
        //    GridGroupSummaryItem sumItem = new GridGroupSummaryItem();
        //    sumItem.FieldName = fieldName;
        //    sumItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
        //    sumItem.DisplayFormat = "{0:n2}";
        //    sumItem.ShowInGroupColumnFooter = gv.Columns[fieldName];
        //    gv.GroupSummary.Add(sumItem);
        //}

        public static List<JField> GetFieldList(List<JField> fieldList)
        {
            List<JField> fields = new List<JField>();
            foreach (var item in fieldList)
            {
            }
            return fields;
        }

        public static string NetGenField(string fieldName, bool? smartSpaces)
        {
            fieldName = fieldName.Trim();

            if (fieldName.Length <= 4)
                return fieldName;

            if (smartSpaces == true)
                fieldName = Common.Helpers.Strings.AddSpacesToSentence(fieldName);

            string strWidth = fieldName.Substring(fieldName.Length - 4);
            int outInt;
            bool isNum = int.TryParse(strWidth, out outInt);
            if (isNum == false)
                return fieldName;

            return fieldName.Substring(0, fieldName.Length - 4);
        }

        public static DevExpress.Data.SummaryItemType GetSummaryType(string sumType)
        {
            if (string.IsNullOrEmpty(sumType))
                return DevExpress.Data.SummaryItemType.Sum;

            sumType = sumType.ToLower();

            if (sumType == "average")
                return DevExpress.Data.SummaryItemType.Average;
            else if (sumType == "count")
                return DevExpress.Data.SummaryItemType.Count;
            else if (sumType == "max")
                return DevExpress.Data.SummaryItemType.Max;
            else if (sumType == "min")
                return DevExpress.Data.SummaryItemType.Min;
            else if (sumType == "wavg")
                return DevExpress.Data.SummaryItemType.Custom;
            else
                return DevExpress.Data.SummaryItemType.Sum;
        }

        public static string DataSourceFilterHelpers(string nameStr, string valueStr, TypeCode? type)
        {
            if (nameStr == "filterValue")
            {
                switch (type)
                {
                    case TypeCode.DateTime:
                        var expressionSplit = valueStr.Split(new string[] { "::" }, StringSplitOptions.None);
                        return DateTime.ParseExact(expressionSplit[0], expressionSplit[1], CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return valueStr.Replace(',', '.');

                    default:
                        return valueStr;
                }
            }
            else if (nameStr == "typeStr")
            {
                switch (valueStr)
                {
                    case "==":
                    case "equal":
                        return " = ";

                    case "!=":
                    case "notequal":
                        return " <> ";

                    case "startswith":
                    case "endswith":
                    case "contains":
                        return " LIKE ";

                    case "greaterthan":
                        return " > ";

                    case "greaterthanorequal":
                        return " >= ";

                    case "lessthan":
                        return " < ";

                    case "lessthanorequal":
                        return " <= ";

                    default:
                        return valueStr;
                }
            }
            else if (nameStr == "literalStr")
            {
                switch (type)
                {
                    case TypeCode.String:
                        return "'";

                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return "";

                    case TypeCode.DateTime:
                        return "#";
                }
            }
            else if (nameStr == "likeHelper1")
            {
                switch (valueStr)
                {
                    case "startswith":
                        return "";

                    case "endswith":
                    case "contains":
                        return "*";
                }
            }
            else if (nameStr == "likeHelper2")
            {
                switch (valueStr)
                {
                    case "endswith":
                        return "";

                    case "startswith":
                    case "contains":
                        return "*";
                }
            }
            return "";
        }

        public static void FormatColumn(GridColumn column)
        {
            string formatString = string.Empty;

            switch (Type.GetTypeCode(column.ColumnType))
            {
                case TypeCode.Int32:
                case TypeCode.Int64:
                    column.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    formatString = "f0";
                    break;

                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    //default format
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    formatString = "n2";
                    column.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    break;
            }
            if (!string.IsNullOrEmpty(formatString))
            {
                column.DisplayFormat.FormatString = formatString;
            }
        }
    }
}
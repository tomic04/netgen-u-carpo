using Carpo.Structure.Models.Core;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Carpo.Common.GridFormatRules
{
    public class GridFormatRules
    {
        public GridFormatRule SetFormatConditionRuleDataBar(GridView gridView1, string fieldName, List<GridFormatRuleParameter> paramList = null, string valueType = "", string showInField = "", bool showNegativeBar = true, bool showBarOnly = false)
        {
            try
            {
                if (gridView1.FormatRules.Where(x => x.Name.ToLower() == ("DataBar_" + fieldName).ToLower()).FirstOrDefault() != null)
                    gridView1.FormatRules.Remove(gridView1.FormatRules.Where(x => x.Name.ToLower() == ("DataBar_" + fieldName).ToLower()).FirstOrDefault());

                GridFormatRule gridFormatRule1 = new GridFormatRule();
                FormatConditionRuleDataBar formatConditionRuleDataBar1 = new FormatConditionRuleDataBar();

                gridFormatRule1.Column = gridView1.Columns.Where(col => col.FieldName.ToLower() == fieldName.ToLower()).FirstOrDefault();
                gridFormatRule1.Name = "DataBar_" + showInField;
                gridFormatRule1.Name = string.IsNullOrEmpty(showInField) ? gridFormatRule1.Name + fieldName : gridFormatRule1.Name + "_" + fieldName;
                formatConditionRuleDataBar1.AxisColor = System.Drawing.Color.Black;
                formatConditionRuleDataBar1.DrawAxisAtMiddle = true;
                formatConditionRuleDataBar1.PredefinedName = "Green Gradient";

                if (!string.IsNullOrEmpty(valueType) && valueType.ToLower() == "number")
                {
                    formatConditionRuleDataBar1.MaximumType = FormatConditionValueType.Number;
                    formatConditionRuleDataBar1.MinimumType = FormatConditionValueType.Number;
                }

                if (!string.IsNullOrEmpty(showInField))
                    gridFormatRule1.ColumnApplyTo = gridView1.Columns.Where(col => col.FieldName.ToLower() == showInField.ToLower()).FirstOrDefault();

                if (showNegativeBar == false)
                    formatConditionRuleDataBar1.AllowNegativeAxis = false;

                if (showBarOnly)
                    formatConditionRuleDataBar1.ShowBarOnly = true;

                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "low").FirstOrDefault() != null)
                {
                    var paramLow = paramList.Where(x => x.Name.ToLower() == "low").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (paramLow < 0)
                    {
                        paramLow = paramLow * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionRuleDataBar1.Minimum = new decimal(new int[] { paramLow, 0, 0, positiveNegativeValue });
                }

                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "high").FirstOrDefault() != null)
                {
                    var paramHigh = paramList.Where(x => x.Name.ToLower() == "high").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (paramHigh < 0)
                    {
                        paramHigh = paramHigh * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionRuleDataBar1.Maximum = new decimal(new int[] { paramHigh, 0, 0, positiveNegativeValue });
                }

                gridFormatRule1.Rule = formatConditionRuleDataBar1;
                return gridFormatRule1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public GridFormatRule SetFormatConditionRuleArrows(GridView gridView1, string fieldName, List<GridFormatRuleParameter> paramList = null, string valueType = "", string showInField = "")
        {
            try
            {
                GridFormatRule gridFormatRule1 = new GridFormatRule();
                FormatConditionRuleIconSet formatConditionRuleIconSet1 = new FormatConditionRuleIconSet();
                FormatConditionIconSet formatConditionIconSet1 = new FormatConditionIconSet();
                FormatConditionIconSetIcon formatConditionIconSetIcon1 = new FormatConditionIconSetIcon();
                FormatConditionIconSetIcon formatConditionIconSetIcon2 = new FormatConditionIconSetIcon();
                FormatConditionIconSetIcon formatConditionIconSetIcon3 = new FormatConditionIconSetIcon();
                FormatConditionIconSetIcon formatConditionIconSetIcon4 = new FormatConditionIconSetIcon();
                FormatConditionIconSetIcon formatConditionIconSetIcon5 = new FormatConditionIconSetIcon();

                formatConditionIconSet1.CategoryName = "Directional";

                if (gridView1.FormatRules.Where(x => x.Name.ToLower() == ("Arrows_" + fieldName).ToLower()).FirstOrDefault() != null)
                    gridView1.FormatRules.Remove(gridView1.FormatRules.Where(x => x.Name.ToLower() == ("Arrows_" + fieldName).ToLower()).FirstOrDefault());

                gridFormatRule1.Name = "Arrows_" + showInField;
                gridFormatRule1.Name = string.IsNullOrEmpty(showInField) ? gridFormatRule1.Name + fieldName : gridFormatRule1.Name + "_" + fieldName;
                gridFormatRule1.Column = gridView1.Columns.Where(col => col.FieldName.ToLower() == fieldName.ToLower()).FirstOrDefault();

                if (!string.IsNullOrEmpty(showInField))
                    gridFormatRule1.ColumnApplyTo = gridView1.Columns.Where(col => col.FieldName.ToLower() == showInField.ToLower()).FirstOrDefault();

                formatConditionIconSet1.Name = "Arrows5Colored";
                if (!string.IsNullOrEmpty(valueType) && valueType.ToLower() == "percent")
                    formatConditionIconSet1.ValueType = FormatConditionValueType.Percent;
                else
                    formatConditionIconSet1.ValueType = FormatConditionValueType.Number;

                formatConditionIconSetIcon1.PredefinedName = "Arrows5_1.png";
                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "high").FirstOrDefault() != null)
                {
                    var paramHigh = paramList.Where(x => x.Name.ToLower() == "high").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (paramHigh < 0)
                    {
                        paramHigh = paramHigh * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionIconSetIcon1.Value = new decimal(new int[] { paramHigh, 0, 0, positiveNegativeValue });
                }
                else
                    formatConditionIconSetIcon1.Value = new decimal(new int[] { 80, 0, 0, 0 });
                formatConditionIconSetIcon1.ValueComparison = FormatConditionComparisonType.GreaterOrEqual;

                formatConditionIconSetIcon2.PredefinedName = "Arrows5_2.png";
                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "midhigh").FirstOrDefault() != null)
                {
                    var parammidhigh = paramList.Where(x => x.Name.ToLower() == "midhigh").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (parammidhigh < 0)
                    {
                        parammidhigh = parammidhigh * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionIconSetIcon2.Value = new decimal(new int[] { parammidhigh, 0, 0, positiveNegativeValue });
                }
                else
                    formatConditionIconSetIcon2.Value = new decimal(new int[] { 60, 0, 0, 0 });
                formatConditionIconSetIcon2.ValueComparison = FormatConditionComparisonType.GreaterOrEqual;

                formatConditionIconSetIcon3.PredefinedName = "Arrows5_3.png";
                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "mid").FirstOrDefault() != null)
                {
                    var parammid = paramList.Where(x => x.Name.ToLower() == "mid").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (parammid < 0)
                    {
                        parammid = parammid * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionIconSetIcon3.Value = new decimal(new int[] { parammid, 0, 0, positiveNegativeValue });
                }
                else
                    formatConditionIconSetIcon3.Value = new decimal(new int[] { 40, 0, 0, 0 });
                formatConditionIconSetIcon3.ValueComparison = FormatConditionComparisonType.GreaterOrEqual;

                formatConditionIconSetIcon4.PredefinedName = "Arrows5_4.png";
                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "midlow").FirstOrDefault() != null)
                {
                    var parammidlow = paramList.Where(x => x.Name.ToLower() == "midlow").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (parammidlow < 0)
                    {
                        parammidlow = parammidlow * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionIconSetIcon4.Value = new decimal(new int[] { parammidlow, 0, 0, positiveNegativeValue });
                }
                else
                    formatConditionIconSetIcon4.Value = new decimal(new int[] { 20, 0, 0, 0 });
                formatConditionIconSetIcon4.ValueComparison = FormatConditionComparisonType.GreaterOrEqual;

                formatConditionIconSetIcon5.PredefinedName = "Arrows5_5.png";
                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "low").FirstOrDefault() != null)
                {
                    var paramlow = paramList.Where(x => x.Name.ToLower() == "low").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (paramlow < 0)
                    {
                        paramlow = paramlow * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionIconSetIcon5.Value = new decimal(new int[] { paramlow, 0, 0, positiveNegativeValue });
                }
                else
                    formatConditionIconSetIcon5.Value = new decimal(new int[] { 0, 0, 0, 0 });
                formatConditionIconSetIcon5.ValueComparison = FormatConditionComparisonType.GreaterOrEqual;

                formatConditionIconSet1.Icons.Add(formatConditionIconSetIcon1);
                formatConditionIconSet1.Icons.Add(formatConditionIconSetIcon2);
                formatConditionIconSet1.Icons.Add(formatConditionIconSetIcon3);
                formatConditionIconSet1.Icons.Add(formatConditionIconSetIcon4);
                formatConditionIconSet1.Icons.Add(formatConditionIconSetIcon5);

                formatConditionRuleIconSet1.IconSet = formatConditionIconSet1;
                gridFormatRule1.Rule = formatConditionRuleIconSet1;
                return gridFormatRule1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public GridFormatRule SetFormatConditionRuleColorScale(GridView gridView1, string fieldName, List<GridFormatRuleParameter> paramList = null, string valueType = "", string showInField = "")
        {
            try
            {
                if (gridView1.FormatRules.Where(x => x.Name.ToLower() == ("ColorScale_" + fieldName).ToLower()).FirstOrDefault() != null)
                    gridView1.FormatRules.Remove(gridView1.FormatRules.Where(x => x.Name.ToLower() == ("ColorScale_" + fieldName).ToLower()).FirstOrDefault());

                GridFormatRule gridFormatRule1 = new GridFormatRule();
                FormatConditionRule3ColorScale formatConditionRule3ColorScale1 = new FormatConditionRule3ColorScale();
                gridFormatRule1.Name = "ColorScale_" + showInField;
                gridFormatRule1.Name = string.IsNullOrEmpty(showInField) ? gridFormatRule1.Name + fieldName : gridFormatRule1.Name + "_" + fieldName;
                gridFormatRule1.Column = gridView1.Columns.Where(col => col.FieldName.ToLower() == fieldName.ToLower()).FirstOrDefault();
                if (!string.IsNullOrEmpty(showInField))
                    gridFormatRule1.ColumnApplyTo = gridView1.Columns.Where(col => col.FieldName.ToLower() == showInField.ToLower()).FirstOrDefault();

                if (!string.IsNullOrEmpty(valueType) && valueType.ToLower() == "number")
                {
                    formatConditionRule3ColorScale1.MinimumType = FormatConditionValueType.Number;
                    formatConditionRule3ColorScale1.MiddleType = FormatConditionValueType.Number;
                    formatConditionRule3ColorScale1.MaximumType = FormatConditionValueType.Number;
                }

                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "high").FirstOrDefault() != null)
                {
                    var paramhigh = paramList.Where(x => x.Name.ToLower() == "high").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (paramhigh < 0)
                    {
                        paramhigh = paramhigh * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionRule3ColorScale1.Maximum = new decimal(new int[] { paramhigh, 0, 0, positiveNegativeValue });
                }
                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "mid").FirstOrDefault() != null)
                {
                    var parammid = paramList.Where(x => x.Name.ToLower() == "mid").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (parammid < 0)
                    {
                        parammid = parammid * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionRule3ColorScale1.Middle = new decimal(new int[] { parammid, 0, 0, positiveNegativeValue });
                }
                if (paramList != null && paramList.Where(x => x.Name.ToLower() == "low").FirstOrDefault() != null)
                {
                    var paramlow = paramList.Where(x => x.Name.ToLower() == "low").FirstOrDefault().Value;
                    var positiveNegativeValue = 0;
                    if (paramlow < 0)
                    {
                        paramlow = paramlow * (-1);
                        positiveNegativeValue = -2147483648;    // negative
                    }
                    formatConditionRule3ColorScale1.Minimum = new decimal(new int[] { paramlow, 0, 0, positiveNegativeValue });
                }

                gridFormatRule1.Rule = formatConditionRule3ColorScale1;
                gridView1.FormatRules.Add(gridFormatRule1);
                return gridFormatRule1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
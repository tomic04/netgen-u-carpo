using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Carpo.Common.Helpers
{
    public class GridTemplates
    {
        public static void SetGridColumnsFromTemplate(DataTable dt, GridView gv)
        {
            List<int> expandedRows = new List<int>();
            List<string> visibleColumnsList = new List<string>();

            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows.Count == 1 && dt.Rows[0]["ColumnName"] == DBNull.Value)
                {
                    SetTemplateColumns(visibleColumnsList, gv, expandedRows);
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        visibleColumnsList.Add(dt.Rows[i]["ColumnName"].ToString());
                    }

                    SetTemplateColumns(visibleColumnsList, gv, expandedRows);
                }
            }
        }

        private static void SetTemplateColumns(List<string> visibleColumns, GridView gv, List<int> expandedRows)
        {
            Dictionary<string, int> groupedColumns = new Dictionary<string, int>();
            expandedRows = new List<int>();
            int index = 0;

            foreach (var c in gv.Columns)
            {
                var col = c as GridColumn;
                if (col.GroupIndex >= 0 && visibleColumns.Contains(col.FieldName))
                    groupedColumns.Add(col.FieldName, col.GroupIndex);
            }

            int rHandle = -1;
            while (gv.IsValidRowHandle(rHandle))
            {
                // zapamti expandane group rowse za postavljanje u isti layout nakon promijenjenog predloška
                if (gv.GetRowExpanded(rHandle))
                    expandedRows.Add(rHandle);

                rHandle--;
            }

            foreach (var c in gv.Columns)
            {
                var col = c as GridColumn;
                col.GroupIndex = -1;
                col.Visible = false;
                col.OptionsColumn.ShowInCustomizationForm = true;
            }

            foreach (var visiblekolona in visibleColumns)
            {
                var obj = gv.Columns.ColumnByFieldName(visiblekolona);
                if (obj != null)
                {
                    gv.Columns[visiblekolona].VisibleIndex = index;
                    index++;
                }
            }

            SetTemplateGrouping(groupedColumns, gv, expandedRows);
        }

        private static void SetTemplateGrouping(Dictionary<string, int> GroupedColumns, GridView gv, List<int> expandedRows)
        {
            var sortedDict = from entry in GroupedColumns orderby entry.Value ascending select entry;

            foreach (var value in sortedDict)
            {
                gv.Columns[value.Key].GroupIndex = value.Value;
            }

            foreach (var groupRowHandle in expandedRows)
                gv.ExpandGroupRow(groupRowHandle);
        }
    }
}
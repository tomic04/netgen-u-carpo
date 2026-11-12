using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpo.UI.Helpers
{
    class SelectionControl
    {
        List<int> selectedRows = new List<int>();
        public SelectionControl()
        {

        }
        public void RestoreSelection(GridView view)
        {
            for (int i = 0; i < selectedRows.Count; i++)
                view.SelectRow(view.GetRowHandle(selectedRows[i]));
        }
        public void MouseDown(GridView view, MouseEventArgs e)
        {
            GridHitInfo hi = view.CalcHitInfo(e.Location);
            if (hi.Column == null)
                return;
            if (hi.Column.FieldName == "DX$CheckboxSelectorColumn")
            {
                if (!hi.InRow)
                {
                    bool allSelected = view.DataController.Selection.Count == view.DataRowCount;
                    if (!allSelected)
                    {
                        for (int i = 0; i < view.RowCount; i++)
                        {
                            int sourceHandle = view.GetDataSourceRowIndex(i);
                            if (!selectedRows.Contains(sourceHandle))
                                selectedRows.Add(sourceHandle);
                        }
                    }
                    else
                    {
                        selectedRows.Clear();
                        //for (int i = 0; i < view.RowCount; i++)
                        //{
                        //    int sourceHandle = view.GetDataSourceRowIndex(i);
                        //    if (selectedRows.Contains(sourceHandle))
                        //        selectedRows.Remove(sourceHandle);
                        //}
                    }

                }
                else
                {
                    int sourceHandle = view.GetDataSourceRowIndex(hi.RowHandle);
                    if (!selectedRows.Contains(sourceHandle))
                        selectedRows.Add(sourceHandle);
                    else
                        selectedRows.Remove(sourceHandle);
                }
            }
        }
    }
}

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.Common.Helpers
{
    public class GridRefresh
    {
        [Serializable]
        public struct RowInfo
        {
            public object Id;
            public int level;
        };

        private GridView view;
        private string keyFieldName;
        private ArrayList saveExpList;
        private ArrayList saveSelList;
        private ArrayList saveMasterRowsList;
        private int visibleRowIndex = 0;
        private int focusedRowHadle = -1;

        private FocusType focusType = FocusType.Default;

        public GridRefresh(GridView view, string keyFieldName)
        {
            this.view = view;
            this.keyFieldName = keyFieldName;
        }

        public ArrayList SaveExpList
        {
            get
            {
                if (saveExpList == null)
                    saveExpList = new ArrayList();
                return saveExpList;
            }
        }

        public ArrayList SaveSelList
        {
            get
            {
                if (saveSelList == null)
                    saveSelList = new ArrayList();
                return saveSelList;
            }
        }

        public ArrayList SaveMasterRowsList
        {
            get
            {
                if (saveMasterRowsList == null)
                    saveMasterRowsList = new ArrayList();
                return saveMasterRowsList;
            }
        }

        protected int FindParentRowHandle(RowInfo rowInfo, int rowHandle)
        {
            int result = view.GetParentRowHandle(rowHandle);
            while (view.GetRowLevel(result) != rowInfo.level)
                result = view.GetParentRowHandle(result);
            return result;
        }

        protected void ExpandRowByRowInfo(RowInfo rowInfo)
        {
            //int dataRowHandle = view.LocateByValue(0, view.Columns[keyFieldName], rowInfo.Id);
            int dataRowHandle = view.LocateByValue(keyFieldName, rowInfo.Id);
            if (dataRowHandle != GridControl.InvalidRowHandle)
            {
                int parentRowHandle = FindParentRowHandle(rowInfo, dataRowHandle);
                view.SetRowExpanded(parentRowHandle, true, false);
            }
        }

        protected int GetRowHandleToSelect(RowInfo rowInfo, bool isFocused)
        {
            //int dataRowHandle = view.LocateByValue(0, view.Columns[keyFieldName], rowInfo.Id);
            int dataRowHandle = view.LocateByValue(keyFieldName, rowInfo.Id);
            if (dataRowHandle != GridControl.InvalidRowHandle)
            {
                if (view.GetRowLevel(dataRowHandle) != rowInfo.level)
                    return FindParentRowHandle(rowInfo, dataRowHandle);
            }
            else if (focusType == FocusType.KeepFocus && focusedRowHadle >= 0)
            {
                if (isFocused)
                    return focusedRowHadle;
                else
                    return GetFocusedRowHandle();
            }
            return dataRowHandle;
        }

        protected int GetFocusedRowHandle()
        {
            try
            {
                GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                int lastRowHandle = viewInfo.RowsInfo[viewInfo.RowsInfo.Count - 1].RowHandle;
                if (focusedRowHadle > lastRowHandle)
                    return lastRowHandle;
            }
            catch
            { }

            return focusedRowHadle;
        }

        protected void SelectRowByRowInfo(RowInfo rowInfo, bool isFocused)
        {
            if (isFocused)
                view.FocusedRowHandle = GetRowHandleToSelect(rowInfo, isFocused);
            else
                view.SelectRow(GetRowHandleToSelect(rowInfo, isFocused));
        }

        /// <summary>
        /// Key value for focus/selection restore. Prefer GetRowCellValue — GetDataRow is null under VirtualServerMode.
        /// </summary>
        private object GetRowKeyValue(int dataRowHandle)
        {
            if (dataRowHandle < 0 || string.IsNullOrEmpty(keyFieldName) || view == null)
                return null;

            try
            {
                var dataRow = view.GetDataRow(dataRowHandle);
                if (dataRow != null && dataRow.Table != null && dataRow.Table.Columns.Contains(keyFieldName))
                    return dataRow[keyFieldName];
            }
            catch
            {
                // fall through to GetRowCellValue
            }

            try
            {
                if (view.Columns[keyFieldName] != null)
                    return view.GetRowCellValue(dataRowHandle, keyFieldName);
            }
            catch
            {
                return null;
            }

            return null;
        }

        public void SaveSelectionViewInfo(ArrayList list)
        {
            list.Clear();
            //GridColumn column = view.Columns[keyFieldName];
            RowInfo rowInfo;
            int[] selectionArray = view.GetSelectedRows();
            if (selectionArray != null)  // otherwise we have a single focused but not selected row
                for (int i = 0; i < selectionArray.Length; i++)
                {
                    int dataRowHandle = selectionArray[i];
                    rowInfo.level = view.GetRowLevel(dataRowHandle);
                    if (dataRowHandle < 0) // group row
                        dataRowHandle = view.GetDataRowHandleByGroupRowHandle(dataRowHandle);
                    if (view.FocusedRowHandle >= 0)
                    {
                        object id = GetRowKeyValue(dataRowHandle);
                        if (id == null)
                            continue;
                        rowInfo.Id = id;
                        list.Add(rowInfo);
                    }
                }

            if (view.FocusedRowHandle >= 0)
            {
                object id = GetRowKeyValue(view.FocusedRowHandle);
                if (id != null)
                {
                    rowInfo.Id = id;
                    rowInfo.level = view.GetRowLevel(view.FocusedRowHandle);
                    list.Add(rowInfo);
                }
            }
        }

        public void SaveExpansionViewInfo(ArrayList list)
        {
            if (view.GroupedColumns.Count == 0) return;
            list.Clear();
            //GridColumn column = view.Columns[keyFieldName];
            for (int i = -1; i > int.MinValue; i--)
            {
                if (!view.IsValidRowHandle(i)) break;
                if (view.GetRowExpanded(i))
                {
                    RowInfo rowInfo;
                    int dataRowHandle = view.GetDataRowHandleByGroupRowHandle(i);
                    object id = GetRowKeyValue(dataRowHandle);
                    if (id == null)
                        continue;
                    rowInfo.Id = id;
                    rowInfo.level = view.GetRowLevel(i);
                    list.Add(rowInfo);
                }
            }
        }

        public void SaveExpandedMasterRows(ArrayList list)
        {
            if (view.GridControl.Views.Count == 1) return;
            list.Clear();
            GridColumn column = view.Columns[keyFieldName];
            for (int i = 0; i < view.DataRowCount; i++)
                if (view.GetMasterRowExpanded(i))
                    list.Add(view.GetRowCellValue(i, column));
        }

        public void SaveVisibleIndex()
        {
            visibleRowIndex = view.GetVisibleIndex(view.FocusedRowHandle) - view.TopRowIndex;
        }

        public void SaveFocusedRowHandle()
        {
            focusedRowHadle = view.FocusedRowHandle;
        }

        public void LoadVisibleIndex()
        {
            view.MakeRowVisible(view.FocusedRowHandle, true);
            view.TopRowIndex = view.GetVisibleIndex(view.FocusedRowHandle) - visibleRowIndex;
        }

        public void LoadSelectionViewInfo(ArrayList list)
        {
            view.BeginSelection();
            try
            {
                view.ClearSelection();
                for (int i = 0; i < list.Count; i++)
                    SelectRowByRowInfo((RowInfo)list[i], i == list.Count - 1);
            }
            finally
            {
                view.EndSelection();
            }
        }

        public void LoadExpansionViewInfo(ArrayList list)
        {
            if (view.GroupedColumns.Count == 0) return;
            view.BeginUpdate();
            try
            {
                view.CollapseAllGroups();
                foreach (RowInfo info in list)
                    ExpandRowByRowInfo(info);
            }
            finally
            {
                view.EndUpdate();
            }
        }

        public void LoadExpandedMasterRows(ArrayList list)
        {
            view.BeginUpdate();
            try
            {
                view.CollapseAllDetails();
                GridColumn column = view.Columns[keyFieldName];
                for (int i = 0; i < list.Count; i++)
                {
                    int rowHandle = view.LocateByValue(0, column, list[i]);
                    view.SetMasterRowExpanded(rowHandle, true);
                }
            }
            finally
            {
                view.EndUpdate();
            }
        }

        public void SaveViewInfo()
        {
            SaveExpandedMasterRows(SaveMasterRowsList);
            SaveExpansionViewInfo(SaveExpList);
            SaveSelectionViewInfo(SaveSelList);
            SaveVisibleIndex();
            SaveFocusedRowHandle();
        }

        public void LoadViewInfo(FocusType focusType)
        {
            this.focusType = focusType;
            LoadExpandedMasterRows(SaveMasterRowsList);
            LoadExpansionViewInfo(SaveExpList);
            LoadSelectionViewInfo(SaveSelList);
            LoadVisibleIndex();
        }
    }
}
// Developer Express Code Central Example:
// Multiple selection using checkbox (web style)
//
// The following example shows how to implement multiple selection in the web style
// (via check boxes). The XtraGrid does not have this functionality built-in. The
// GridCheckMarksSelection class allows you to implement it. End-users can
// select/unselect rows, group rows or select/unselect all rows by clicking on the
// column header. Changing check box value does not start row editing. This example
// is based on the http://www.devexpress.com/scid=A371 article.
//
// See
// Also:
// http://www.devexpress.com/scid=E990
//
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E1271

using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DevExpress.XtraGrid.Selection
{
    public class GridCheckMarksSelection
    {
        static protected GridView _view;
        static protected ArrayList selection;
        public static int viewSelection = 0;
        private static GridColumn column;
        private static RepositoryItemCheckEdit edit;
        private const int CheckboxIndent = 4;

        public GridCheckMarksSelection()
        {
            selection = new ArrayList();
        }

        public GridCheckMarksSelection(GridView view)
            : this()
        {
            View = view;
        }

        public static GridView View
        {
            get { return _view; }
            set
            {
                if (_view != value)
                {
                    Detach();
                    Attach(value);
                }
            }
        }

        public static GridColumn CheckMarkColumn { get { return column; } }

        public static int SelectedCount { get { return selection.Count; } }
        //public object GetSelectedRow(int index)
        //{
        //    return selection[index];
        //}

        public static string GetSelectedIds(string fieldName)
        {
            int i = 0;
            string selectedIds = "";
            foreach (DataRowView drv in selection)
            {
                i++;
                if (i > 1)
                    selectedIds += ",";
                selectedIds += (drv.Row)[fieldName].ToString();
            }

            return selectedIds;
        }

        public static int GetSelectedIndex(object row)
        {
            return selection.IndexOf(row);
        }

        public static void ClearSelection()
        {
            selection.Clear();
            Invalidate();
        }

        public static void SelectAll(bool selected)
        {
            viewSelection = 0;
            if (!selected)
            {
                for (int i = 0; i < _view.DataRowCount; i++)
                {
                    if (selection.Contains(_view.GetRow(i)))
                        selection.Remove(_view.GetRow(i));
                }
            }
            else
            {
                for (int i = 0; i < _view.DataRowCount; i++)
                {
                    if (!selection.Contains(_view.GetRow(i)))
                        selection.Add(_view.GetRow(i));
                    viewSelection++;
                }
            }

            Invalidate();
        }

        public static void SelectGroup(int rowHandle, bool select)
        {
            if (IsGroupRowSelected(rowHandle) && select) return;
            for (int i = 0; i < _view.GetChildRowCount(rowHandle); i++)
            {
                int childRowHandle = _view.GetChildRowHandle(rowHandle, i);
                if (_view.IsGroupRow(childRowHandle))
                    SelectGroup(childRowHandle, select);
                else
                    SelectRow(childRowHandle, select, false);
            }
            Invalidate();
        }

        public static void SelectRow(int rowHandle, bool select)
        {
            SelectRow(rowHandle, select, true);
        }

        public static void InvertRowSelection(int rowHandle)
        {
            if (View.IsDataRow(rowHandle))
            {
                SelectRow(rowHandle, !IsRowSelected(rowHandle));
            }
            if (View.IsGroupRow(rowHandle))
            {
                SelectGroup(rowHandle, !IsGroupRowSelected(rowHandle));
            }
        }

        public static bool IsGroupRowSelected(int rowHandle)
        {
            for (int i = 0; i < _view.GetChildRowCount(rowHandle); i++)
            {
                int row = _view.GetChildRowHandle(rowHandle, i);
                if (_view.IsGroupRow(row))
                {
                    if (!IsGroupRowSelected(row)) return false;
                }
                else
                    if (!IsRowSelected(row)) return false;
            }
            return true;
        }

        public static bool IsRowSelected(int rowHandle)
        {
            if (_view.IsGroupRow(rowHandle))
                return IsGroupRowSelected(rowHandle);

            object row = _view.GetRow(rowHandle);
            return GetSelectedIndex(row) != -1;
        }

        protected static void Attach(GridView view)
        {
            if (view == null) return;
            selection.Clear();
            _view = view;
            view.BeginUpdate();
            try
            {
                edit = view.GridControl.RepositoryItems.Add("CheckEdit") as RepositoryItemCheckEdit;

                column = view.Columns.Add();
                column.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                column.Visible = true;
                column.VisibleIndex = 0;
                column.FieldName = "CheckMarkSelection";
                column.Caption = "Mark";
                column.OptionsColumn.ShowCaption = false;
                column.OptionsColumn.AllowEdit = false;
                column.OptionsColumn.AllowSize = false;
                column.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
                column.Width = GetCheckBoxWidth();
                column.ColumnEdit = edit;

                view.Click += new EventHandler(View_Click);
                view.CustomDrawColumnHeader += new ColumnHeaderCustomDrawEventHandler(View_CustomDrawColumnHeader);
                view.CustomDrawGroupRow += new RowObjectCustomDrawEventHandler(View_CustomDrawGroupRow);
                view.CustomUnboundColumnData += new CustomColumnDataEventHandler(view_CustomUnboundColumnData);
                //view.KeyDown += new KeyEventHandler(view_KeyDown);
                view.ColumnFilterChanged += new EventHandler(view_ColumnFilterChanged);
                //view.RowStyle += new RowStyleEventHandler(view_RowStyle);
            }
            finally
            {
                view.EndUpdate();
            }
        }

        public static void Detach()
        {
            if (_view == null) return;
            if (column != null)
                column.Dispose();
            if (edit != null)
            {
                if (_view.GridControl != null)
                    _view.GridControl.RepositoryItems.Remove(edit);
                edit.Dispose();
            }

            _view.Click -= new EventHandler(View_Click);
            _view.CustomDrawColumnHeader -= new ColumnHeaderCustomDrawEventHandler(View_CustomDrawColumnHeader);
            _view.CustomDrawGroupRow -= new RowObjectCustomDrawEventHandler(View_CustomDrawGroupRow);
            _view.CustomUnboundColumnData -= new CustomColumnDataEventHandler(view_CustomUnboundColumnData);
            //_view.KeyDown -= new KeyEventHandler(view_KeyDown);
            _view.ColumnFilterChanged -= new EventHandler(view_ColumnFilterChanged);
            //_view.RowStyle -= new RowStyleEventHandler(view_RowStyle);

            _view = null;
        }

        protected static int GetCheckBoxWidth()
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            int width = 0;
            GraphicsInfo.Default.AddGraphics(null);
            try
            {
                width = info.CalcBestFit(GraphicsInfo.Default.Graphics).Width;
            }
            finally
            {
                GraphicsInfo.Default.ReleaseGraphics();
            }
            return width + CheckboxIndent * 2;
        }

        protected static void DrawCheckBox(Graphics g, Rectangle r, bool Checked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;
            info = edit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            painter = edit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = Checked;
            info.Bounds = r;
            info.CalcViewInfo(g);
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, new DevExpress.Utils.Drawing.GraphicsCache(g), r);
            painter.Draw(args);
            args.Cache.Dispose();
        }

        private static void Invalidate()
        {
            _view.CloseEditor();
            _view.BeginUpdate();
            _view.EndUpdate();
        }

        public static void SelectRow(int rowHandle, bool select, bool invalidate)
        {
            if (IsRowSelected(rowHandle) == select) return;
            object row = _view.GetRow(rowHandle);
            if (select)
            {
                selection.Add(row);
                viewSelection++;
                _view.SelectRow(rowHandle);
            }
            else
            {
                selection.Remove(row);
                viewSelection--;
                _view.UnselectRow(rowHandle);
            }

            if (invalidate)
            {
                Invalidate();
            }
        }

        public static void view_ColumnFilterChanged(object sender, EventArgs e)
        {
            // rješava problem selektiranja SELECTALL pri filtiranju

            viewSelection = 0;
            for (int i = 0; i < _view.RowCount; i++)
            {
                if (Convert.ToBoolean(_view.GetRowCellValue(i, "CheckMarkSelection")) == true)
                    viewSelection++;
            }
        }

        public static void view_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.Column == CheckMarkColumn)
            {
                if (e.IsGetData)
                    e.Value = IsRowSelected(View.GetRowHandle(e.ListSourceRowIndex));
                else
                    SelectRow(View.GetRowHandle(e.ListSourceRowIndex), (bool)e.Value);
            }
        }

        // TRENUTNO DISEJBLANO ... NA ZAHTJEV ĆEMO OVO RADIT !
        public static void view_KeyDown(object sender, KeyEventArgs e)
        {
            if (View.FocusedColumn != column || e.KeyCode != Keys.Space) return;
            InvertRowSelection(View.FocusedRowHandle);
        }

        public static void View_Click(object sender, EventArgs e)
        {
            GridHitInfo info;
            Point pt = _view.GridControl.PointToClient(Control.MousePosition);
            info = _view.CalcHitInfo(pt);
            if (info.Column == column)
            {
                if (info.InColumn)
                {
                    if (viewSelection == _view.DataRowCount)
                        SelectAll(false);
                    else
                        SelectAll(true);
                }
                if (info.InRowCell)
                {
                    InvertRowSelection(info.RowHandle);
                }
            }
            if (info.InRow && _view.IsGroupRow(info.RowHandle) && info.HitTest != GridHitTest.RowGroupButton)
            {
                InvertRowSelection(info.RowHandle);
            }
        }

        public static void View_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == column)
            {
                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);
                DrawCheckBox(e.Graphics, e.Bounds, viewSelection == _view.DataRowCount); // BUG - ako filtiramo, a u glavnom view-u ima već nešto selektirano
                e.Handled = true;
            }
        }

        public static void View_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info;
            info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            info.GroupText = "         " + info.GroupText.TrimStart();
            e.Info.Paint.FillRectangle(e.Graphics, e.Appearance.GetBackBrush(e.Cache), e.Bounds);
            e.Painter.DrawObject(e.Info);

            Rectangle r = info.ButtonBounds;
            r.Offset(r.Width + CheckboxIndent * 2 - 1, 0);
            DrawCheckBox(e.Graphics, r, IsGroupRowSelected(e.RowHandle));
            e.Handled = true;
        }

        public void view_RowStyle(object sender, RowStyleEventArgs e)
        {
            // DISEJBLANO, KOD ZADRŽAN ZBOG NEKIH DRUGIH SLUČAJA U BUDUĆNOSTI
            if (IsRowSelected(e.RowHandle))
            {
                e.Appearance.BackColor = SystemColors.Highlight;
                e.Appearance.ForeColor = SystemColors.HighlightText;
            }
        }
    }
}
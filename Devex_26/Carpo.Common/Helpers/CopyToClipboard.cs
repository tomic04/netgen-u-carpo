using DevExpress.Export.Xl;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Carpo.Common.Helpers
{
    #region declaration

    public class CopyToClipboard
    {
        private GridView view = null;
        private string sheetName = "";
        private XlCellRange dataRange = null;
        private List<int> childHandleList = new List<int>();
        private List<int> parentHandleList = new List<int>();
        private List<int> selectedRows = new List<int>();
        private List<int> parentHandlesCopied = new List<int>();
        private List<int> parentsNotSelectedRowHandles = new List<int>();
        private List<GridColumn> colListReordered = new List<GridColumn>();

        // služi da znam kada su kopirani svi redovi - nakon toga kopiram totalRow
        private int rowNumber = 0;

        public void CopySelectionToClipboard(GridView view, List<int> childRowHandles, List<int> parentRowHandles, List<int> parentsNotSelectedRowHandles)
        {
            this.view = view;
            this.childHandleList = childRowHandles;
            this.parentHandleList = parentRowHandles;
            this.parentsNotSelectedRowHandles = parentsNotSelectedRowHandles;

            MemoryStream biff8DataStream = CreateBIFF8DataStream();
            MemoryStream linkDataStream = CreateLinkDataStream();
            DataObject dataObject = new DataObject();
            dataObject.SetData("Biff8", biff8DataStream);
            dataObject.SetData("Link", linkDataStream);
            Clipboard.Clear();
            Clipboard.SetDataObject(dataObject, true);
        }

        private MemoryStream CreateBIFF8DataStream()
        {
            IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xls);
            MemoryStream dataStream = new MemoryStream();
            using (IXlDocument document = exporter.CreateDocument(dataStream))
            {
                using (IXlSheet sheet = document.CreateSheet())
                {
                    ExportColumns(sheet);

                    ExportRows(sheet);

                    this.sheetName = sheet.Name;
                    this.dataRange = sheet.DataRange;
                }
            }
            dataStream.Position = 0;
            return dataStream;
        }

        #endregion declaration

        #region GridExport

        private void ExportColumns(IXlSheet sheet)
        {
            foreach (GridColumn column in this.view.Columns)
            {
                ExportColumn(sheet, column);
            }
        }

        private void ExportColumn(IXlSheet sheet, GridColumn gridColumn)
        {
            // Skip hidden column
            if (!gridColumn.Visible)
                return;

            using (IXlColumn column = sheet.CreateColumn())
            {
                // Setup number format
                if (gridColumn.DisplayFormat.FormatType == FormatType.DateTime)
                    column.ApplyFormatting(XlCellFormatting.FromNetFormat(gridColumn.DisplayFormat.FormatString, true));
                else if (gridColumn.DisplayFormat.FormatType != FormatType.None)
                    column.ApplyFormatting(XlCellFormatting.FromNetFormat(gridColumn.DisplayFormat.FormatString, false));
            }
        }

        private void ExportChildrenRecursive(IXlSheet sheet, int parentRowHandle)
        {
            if (!parentsNotSelectedRowHandles.Contains(parentRowHandle))
            {
                ExportRow(sheet, parentRowHandle);
                rowNumber++;
            }

            var childCount = this.view.GetChildRowCount(parentRowHandle);

            if (childCount > 0 && parentRowHandle < 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    if (childHandleList.Contains(this.view.GetChildRowHandle(parentRowHandle, i)) || parentHandleList.Contains(this.view.GetChildRowHandle(parentRowHandle, i)))
                        ExportChildrenRecursive(sheet, this.view.GetChildRowHandle(parentRowHandle, i));
                }
            }
        }

        private void ExportRowsRecursive(IXlSheet sheet, int parentRowHandle)
        {
            // provjera da li je najviša toèka parenta, izaði ako je parent handle majni od najmanjeg u listi
            if (parentHandleList.Min() > (this.view.GetParentRowHandle(parentRowHandle)))
                ExportChildrenRecursive(sheet, parentRowHandle);
        }

        private void ExportRows(IXlSheet sheet)
        {
            ExportColumnHeaderRow(sheet);

            if (parentHandleList.Count == 0)
            {
                foreach (var childRowHandle in childHandleList)
                {
                    ExportRow(sheet, childRowHandle);
                    rowNumber++;
                }
            }
            else
            {
                foreach (var parentRowHandle in parentHandleList)
                    ExportRowsRecursive(sheet, parentRowHandle);
            }
            // sumarni (total) red (šaljem 0 jer ne mogu null)

            if (view.OptionsView.ShowFooter && (childHandleList.Count + parentHandleList.Count) == this.view.RowCount)
                ExportRow(sheet, 0);
        }

        private void ExportRow(IXlSheet sheet, int gridRowHandle)
        {
            //if (gridRowHandle < 0)
            //    sheet.BeginGroup(false);

            using (IXlRow row = sheet.CreateRow())
            {
                ExportCells(sheet, row, gridRowHandle);
            }
            //if (gridRowHandle < 0)
            //    sheet.EndGroup();
        }

        private bool CheckIfAtLeaseOneChildSelected(bool atLeaseOneChildSelected, int gridRowHandle)
        {
            for (int i = 0; i < this.view.GetChildRowCount(gridRowHandle); i++)
            {
                if (this.view.GetRowExpanded(gridRowHandle) &&
                    (childHandleList.Contains(this.view.GetChildRowHandle(gridRowHandle, i))
                    || parentHandleList.Contains(this.view.GetChildRowHandle(gridRowHandle, i))))
                { atLeaseOneChildSelected = true; }
            }
            return atLeaseOneChildSelected;
        }

        private bool CheckIfAllChildRowsSelected(bool allChildRowsSelected, int gridRowHandle)
        {
            for (int i = 0; i < this.view.GetChildRowCount(gridRowHandle); i++)
            {
                if (this.view.GetRowExpanded(gridRowHandle)
                    && !childHandleList.Contains(this.view.GetChildRowHandle(gridRowHandle, i))
                    && !parentHandleList.Contains(this.view.GetChildRowHandle(gridRowHandle, i))
                    && allChildRowsSelected)
                {
                    allChildRowsSelected = false;
                    break;
                }
            }
            return allChildRowsSelected;
        }

        private void ExportSumRow(IXlSheet sheet, IXlRow row, int gridRowHandle, bool totalRow = false)
        {
            var headerExported = false;
            var allChildRowsSelected = true;
            var atLeaseOneChildSelected = false;

            atLeaseOneChildSelected = CheckIfAtLeaseOneChildSelected(atLeaseOneChildSelected, gridRowHandle);
            allChildRowsSelected = CheckIfAllChildRowsSelected(allChildRowsSelected, gridRowHandle);

            foreach (GridColumn col in colListReordered)
            {
                using (IXlCell cell = row.CreateCell())
                {
                    // group header
                    if (!totalRow && view.GetGroupRowDisplayText(gridRowHandle) != null && !headerExported
                        && view.IsGroupRow(gridRowHandle))
                    {
                        var pFrom = view.GetGroupRowDisplayText(gridRowHandle).LastIndexOf("(") + 1;
                        var pTo = view.GetGroupRowDisplayText(gridRowHandle).LastIndexOf(")");
                        var recordCount = view.GetGroupRowDisplayText(gridRowHandle).Substring(pFrom, pTo - pFrom);

                        cell.Value = view.GetGroupRowValue(gridRowHandle).ToString().Trim() + " (" + recordCount + ")";
                        headerExported = true;
                        cell.ApplyFormatting(XlFill.SolidFill(Color.FromArgb(242, 242, 242)));
                    }
                    // group æelije
                    else
                    {
                        if (col.SummaryItem.SummaryValue != null)
                        {
                            // red sveukupno
                            if (totalRow)
                                cell.Value = XlVariantValue.FromObject(col.SummaryItem.SummaryValue);
                            // group summary æelije - nakon kopiranja group header texta i SAMO AKO su svi childovi odabrani
                            // ako je grupa expandana i nije odabran NITI JEDAN child - kopiraj grupu i sumu bez childova
                            else if (headerExported && (atLeaseOneChildSelected ? allChildRowsSelected : true))
                                cell.Value = XlVariantValue.FromObject(view.GetRowSummaryItem(gridRowHandle, col).Value);
                        }
                        else
                            cell.Value = XlVariantValue.Empty;

                        cell.Formatting = XlCellFormatting.FromNetFormat(col.SummaryItem.DisplayFormat, false);
                        cell.ApplyFormatting(XlFill.SolidFill(Color.FromArgb(242, 242, 242)));
                    }

                    if (totalRow)
                    {
                        cell.Formatting = XlCellFormatting.Calculation;
                        cell.Formatting = XlCellFormatting.FromNetFormat(col.SummaryItem.DisplayFormat, false);
                        cell.ApplyFormatting(XlFill.SolidFill(Color.LightGoldenrodYellow));
                        cell.ApplyFormatting(XlBorder.OutlineBorders(Color.Black, XlBorderLineStyle.Thin));
                    }
                    cell.ApplyFormatting(XlBorder.OutlineBorders(Color.Black, XlBorderLineStyle.Thin));
                    // Apply the font style to the cell content.
                    XlFont font = new XlFont() { Bold = true };
                    cell.ApplyFormatting(font);
                }
            }
        }

        private void ExportCells(IXlSheet sheet, IXlRow row, int gridRowHandle)
        {
            // group sum row
            if (gridRowHandle < 0 && rowNumber < (childHandleList.Count + parentHandleList.Count))
                ExportSumRow(sheet, row, gridRowHandle, false);
            // total row (šaljem rowhandle 0 jer ne mogu null)
            else if ((childHandleList.Count + parentHandleList.Count) == rowNumber)
                ExportSumRow(sheet, row, gridRowHandle, true);
            // ostalo
            else
            {
                foreach (GridColumn column in colListReordered)
                    ExportCell(row, gridRowHandle, column);
            }
        }

        private void ExportColumnHeaderRow(IXlSheet sheet)
        {
            using (IXlRow row = sheet.CreateRow())
            {
                ExportColumnHeaders(row);
            }
        }

        private void ExportColumnHeaders(IXlRow row)
        {
            // pravim novu listu koloni, prvo idu redom grupirane kolone pa onda ostale
            colListReordered.Clear();
            foreach (GridColumn group in this.view.GroupedColumns)
                colListReordered.Add(group);

            // ako su samo grupe želim prikazato samo sumarne kolone
            if (childHandleList.Count == 0)
            {
                foreach (GridColumn summaryCol in this.view.VisibleColumns
                    .Where(x => !colListReordered.Contains(x) && !string.IsNullOrEmpty(x.SummaryText)))
                {
                    colListReordered.Add(summaryCol);
                }
            }
            else
            {
                foreach (GridColumn visibleCol in this.view.VisibleColumns.Where(x => !colListReordered.Contains(x)))
                    colListReordered.Add(visibleCol);
            }

            foreach (GridColumn colHeader in colListReordered)
            {
                var cell = ExportCell(row, -1, colHeader);  // pokupi sve stilove i formate
                cell.Value = XlVariantValue.FromObject(colHeader.Caption);
                cell.ApplyFormatting(XlBorder.OutlineBorders(Color.Black, XlBorderLineStyle.Thin));
                cell.ApplyFormatting(XlFill.SolidFill(Color.LightBlue));
            }
        }

        private IXlCell ExportCell(IXlRow row, int gridRowHandle, GridColumn gridColumn)
        {
            using (IXlCell cell = row.CreateCell())
            {
                // Get cell appearance
                AppearanceObject appearance = GetCellAppearance(gridRowHandle, gridColumn);
                Font appearanceFont = appearance.Font;
                XlFont font = XlFont.CustomFont(appearanceFont.Name);

                if (gridRowHandle < 0)

                    cell.Value = XlVariantValue.FromObject(this.view.GetGroupRowDisplayText(gridRowHandle));
                else if (gridRowHandle >= 0 && colListReordered.IndexOf(gridColumn) >= this.view.GroupedColumns.Count - 1)
                {
                    //var test = XlVariantValue.FromObject(this.view.GetRowCellDisplayText(gridRowHandle, gridColumn));
                    //System.Console.WriteLine($"T: {test.TextValue} N: {test.NumericValue}");
                    var celldata = view.GetRowCellValue(gridRowHandle, gridColumn.FieldName);

                    cell.Value = XlVariantValue.FromObject(celldata);
                }
                    

                #region FormatCell

                // Apply alignment
                XlCellAlignment alignment = new XlCellAlignment()
                {
                    WrapText = appearance.TextOptions.WordWrap.HasFlag(WordWrap.Wrap),
                    VerticalAlignment = ConvertAlignment(appearance.TextOptions.VAlignment),
                    HorizontalAlignment = ConvertAlignment(appearance.TextOptions.HAlignment)
                };
                cell.ApplyFormatting(alignment);

                // Apply borders
                //Color borderColor = appearance.GetBorderColor();
                //if (!DXColor.IsTransparentOrEmpty(borderColor))
                //    cell.ApplyFormatting(XlBorder.OutlineBorders(borderColor));

                // Apply fill
                cell.ApplyFormatting(XlBorder.OutlineBorders(Color.Black, XlBorderLineStyle.Thin));

                // Apply background color - TODO - kopirati background color kod recimo conditional
                // formattinga i sl. Problem je raspoznavanje što bojati jer ne možemo samo kopirati boju
                //if (gridRowHandle != -1)
                //{
                //   cell.ApplyFormatting(XlFill.SolidFill(Color.FromArgb(255, 255, 255, 255))); // stavi bijelu pozadinu ako je obièni red
                //    //else  // kopiraj pozadinu ako ima neki drugi background color
                //    //    cell.ApplyFormatting(XlFill.SolidFill(appearance.BackColor));
                //}

                // Apply font
                if (gridRowHandle != -1)
                {
                    font.Size = appearanceFont.SizeInPoints;
                    font.Bold = appearanceFont.Bold;
                    font.Italic = appearanceFont.Italic;
                    font.StrikeThrough = appearanceFont.Strikeout;
                    font.Underline = appearanceFont.Underline ? XlUnderlineType.Single : XlUnderlineType.None;
                    if (appearance.Options.UseForeColor)
                        font.Color = appearance.ForeColor;

                    cell.ApplyFormatting(font);
                }
                return cell;

                #endregion FormatCell
            }
        }

        #endregion GridExport

        #region Formatting

        private AppearanceObject GetCellAppearance(int gridRowHandle, GridColumn gridColumn)
        {
            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
            GridCellInfo cellInfo = viewInfo.GetGridCellInfo(gridRowHandle, gridColumn);
            if (cellInfo == null)
            {
                cellInfo = new GridCellInfo(new GridColumnInfoArgs(gridColumn), new GridDataRowInfo(viewInfo, gridRowHandle, view.GetRowLevel(gridRowHandle)), Rectangle.Empty);
            }
            MethodInfo me = viewInfo.GetType().GetMethod("UpdateCellAppearance", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (me != null)
                me.Invoke(viewInfo, new object[] { cellInfo, true });
            viewInfo.UpdateCellAppearance(cellInfo);
            return cellInfo.Appearance;
        }

        private XlVerticalAlignment ConvertAlignment(VertAlignment verticalAlignment)
        {
            switch (verticalAlignment)
            {
                case VertAlignment.Top:
                    return XlVerticalAlignment.Top;

                case VertAlignment.Center:
                    return XlVerticalAlignment.Center;

                case VertAlignment.Bottom:
                    return XlVerticalAlignment.Bottom;
            }
            return XlVerticalAlignment.Bottom;
        }

        private XlHorizontalAlignment ConvertAlignment(HorzAlignment horizontalAlignment)
        {
            switch (horizontalAlignment)
            {
                case HorzAlignment.Center:
                    return XlHorizontalAlignment.Center;

                case HorzAlignment.Near:
                    return XlHorizontalAlignment.Left;

                case HorzAlignment.Far:
                    return XlHorizontalAlignment.Right;

                default:
                    return XlHorizontalAlignment.General;
            }
        }

        #endregion Formatting

        #region CreateLinkDataStream

        private MemoryStream CreateLinkDataStream()
        {
            string link = string.Format("Excel\0[Book1]{0}\0{1}:{2}\0\0", sheetName, GetR1C1(this.dataRange.TopLeft), GetR1C1(this.dataRange.BottomRight));
            byte[] linkData = DXEncoding.Default.GetBytes(link);
            return new MemoryStream(linkData);
        }

        private string GetR1C1(XlCellPosition cellPosition)
        {
            return string.Format("R{0}C{1}", cellPosition.Row + 1, cellPosition.Column + 1);
        }

        #endregion CreateLinkDataStream
    }
}
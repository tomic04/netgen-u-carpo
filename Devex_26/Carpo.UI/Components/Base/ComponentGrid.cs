using Carpo.Structure.Models.Core;
using Carpo.UI.Forms.Master;
using Carpo.UI.Helpers;
using Carpo.UI.Interfaces;
using Carpo.UI.Logic;
using Carpo.UI.Logic.Grid;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Selection;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.UI.Components.Base
{
    public class ComponentGrid : ComponentBase
    {
        public bool blockEvents = false;

        private enum EDitDirection { Horizontal, Vertical };

        private bool expandingCollapsingWebStyle = false;

        private EDitDirection editDirection;

        private JGridControl gridControl;
        private GridView gridView1;
        private new JGrid definition;
        private List<JField> definitionFields; //koristi se umjesto definition.Fields; Zato što treba dinamički, uvjetovano napuniti..

        private RowCellStyleEventArgs rowCellStyleEventArgs = null;
        private RowStyleEventArgs rowStyleEventArgs = null;
        private List<int> childRowHandlesList = new List<int>();
        private List<int> parentRowHandlesList = new List<int>();
        private List<int> parentsNotSelectedRowHandlesList = new List<int>();

        public Stream DefaultLayoutStream { get; set; }

        private GridHitInfo dragHitInfo = null; //DragRows potrebe

        //poziva se ili ako je definiran style ili ako postoji formatid polje (netGen);
        //u slučaju formatid event se kreira se nakon kreiranja svih polja i to samo ako već nije aktiviran
        private bool eventRowStyleActivated = false;

        private bool isFiltering = false;
        private bool setGroupRowText = false;
        private bool groupSummaryCreated = false;

        //stvarni popis kolona - iz DataSet-a
        public string[] DataColumns { get; set; } = new string[] { };

        private Dictionary<string, string> CustomSortList = new Dictionary<string, string>();
        private bool isCustomSortInitialized = false;

        private List<Structure.Models.NameType> editHyperlinkColumns = new List<Structure.Models.NameType>();
        private List<Structure.Models.NameType> editCheckColumns = new List<Structure.Models.NameType>();
        private List<Structure.Models.NameType> editDateColumns = new List<Structure.Models.NameType>();

        private List<GridCell> selectedCells = new List<GridCell>();

        //private bool isPropertyCreated = false;
        //****indikatori public
        public bool IsCollapsed { get; set; } = true;

        public bool IsInited { get; set; }

        /// <summary>B6: true when this grid is bound via VirtualServerModeSource (opt-in).</summary>
        public bool IsVirtualServerModeActive => definition?.VirtualServerMode == true && vsmBinder != null;

        private VirtualServerModeBinder vsmBinder;
        public bool IsBestFitted { get; set; }
        private bool InitCustomColumnDisplayTextEvent { get; set; } = false;
        private List<string> clearColumnList = new List<string>();

        // group inline helper Vars
        public bool gridInited { get; set; } = false;
        public string FieldName { get; set; } // helper - trenutno za fokus na field unutar grida

        private int groupedColsCounter = 0;

        public bool? AutoPopulate { get; set; }

        //ako ijedno polje ima if, svaki put prilikom refresha
        //ide se u reinijalizaciju fieldova
        //varijabla poprima vrijednost true
        public bool ForceReinitFields { get; set; } = false;

        //public object ChangedValue;
        //public string ChangedField;

        private Common.Helpers.GridRefresh refreshHelper;

        private bool isRestored = false;

        private string formatIdField;
        private string rowColorIdField;
        private string rowForeColorIdField;

        private string previewField;

        private int topRowIndex;
        private int focusedRowHandle;
        public string keyField;
        private string searchField;

        public object KeyFocusedValue { get; set; }

        public JGridControl MasterControl { get; set; }
        public JGridControl DetailControl { get; set; }

        public int RowsCount { get; set; }
        public Control RowsCountTextControl { get; set; }

        public Dictionary<string, object> DataParameters { get; set; } = new Dictionary<string, object>();
        private Dictionary<string, int?> sortList = new Dictionary<string, int?>();

        //public DevExpress.XtraEditors.LabelControl lblCount;

        public string lblCountCaption = Common.Translator.TranslateString("gornji grid") + ": <b>##</b>";
        public DevExpress.XtraBars.BarStaticItem lblCount;

        public string tabPageCaption;
        public DevExpress.XtraTab.XtraTabPage tabPage; //ako se koriste tabovi, varijabla koja označava za koji tabpage je grid vezan

        public bool initInd = true;
        private List<int> selectedRows = new List<int>();
        public Carpo.Common.Helpers.ColumnOrderModel ColumnOrderModel = new Carpo.Common.Helpers.ColumnOrderModel();
        public Carpo.Common.Helpers.ColumnPropertiesModel ColumnPropertiesModel = new Carpo.Common.Helpers.ColumnPropertiesModel();
        //public Carpo.Common.Helpers.ColumnOrderModel ResetColumnOrderModel = new Carpo.Common.Helpers.ColumnOrderModel();
        SelectionControl selectionControl;
        Timer timer;


        private GridControl PrintClonedGridControl { get; set; } //IL - ako se printaju samo selektirani redovi, klonira se grid radi prikazivanja ispravnih "suma" vrijednosti


        public ComponentGrid() : base()
        {
        }

        public ComponentGrid(Control control, object def) : base(control, def)
        {
            definition = def as JGrid;

            //ako field ima if uvjet, potrebno je forsirati reinti fieldova
            if (definition.Fields != null &&
                definition.Fields.FieldList != null &&
                definition.Fields.FieldList.Where(x => !string.IsNullOrEmpty(x.If)).Count() > 0)
            {
                ForceReinitFields = true;
            }

            definitionFields = PopulateFieldList();

            gridControl = (control as JGridControl);
            gridView1 = gridControl.MainView as GridView;
            gridControl.UseDisabledStatePainter = false;

            gridView1.OptionsMenu.EnableFooterMenu = true;

            if (gridView1 != null)
            {
                Init();
                if (definition.SelectionWebStyle == true)
                    new GridCheckMarksSelection(gridView1);
            }
            CreateMenu();
            if (definition.KeepSelectionAfterFilter == true)
            {
                selectionControl = new SelectionControl();
                //gridView1.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
            }
            timer = new Timer();
            timer.Tick += Timer_Tick;
            if (definition.AutoRefresh > 0)
            {
                timer.Interval = definition.AutoRefresh;
                timer.Start();
            }
            gridControl.Disposed += GridControl_Disposed;
        }

        private void GridControl_Disposed(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer.Dispose();
                timer = null;
            }
            if (gridControl != null)
                gridControl.Disposed -= GridControl_Disposed;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (gridControl == null || gridControl.IsDisposed || form == null || form.IsDisposed)
                return;
            Populate(null, false, null, FocusType.Default);
        }

        #region ExcelCopy

        private void ExcelCopy(KeyEventArgs e)
        {
            try
            {
                selectedRows.Clear();

                for (int i = gridView1.SelectedRowsCount - 1; i >= 0; i--)
                {
                    selectedRows.Add(gridView1.GetSelectedRows()[i]);
                }

                parentsNotSelectedRowHandlesList.Clear();
                childRowHandlesList.Clear();
                parentRowHandlesList.Clear();

                // kopiram sve iz int[] u List<int> i punim listu parenta (po distinct pravilu)
                foreach (int i in selectedRows)
                {
                    if (i >= 0 && !childRowHandlesList.Contains(i))
                        childRowHandlesList.Add(i);
                    else if (i < 0 && !parentRowHandlesList.Contains(i))
                        parentRowHandlesList.Add(i);

                    gridView1.SelectRow(i);
                    // idem selektirati neselektirane parente(grupe) privremeno dok odradim iterativno kopiranje,
                    // kasnije ih unselectam
                    int parent = gridView1.GetParentRowHandle(i);
                    SelectParentsRecursive(parent);
                }
                childRowHandlesList.Sort();
                parentRowHandlesList.Sort();
                parentRowHandlesList.Reverse();

                Carpo.Common.Helpers.CopyToClipboard CopyToClipboardHelper = new Common.Helpers.CopyToClipboard();
                CopyToClipboardHelper.CopySelectionToClipboard(gridControl.MainView as GridView, childRowHandlesList, parentRowHandlesList, parentsNotSelectedRowHandlesList);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                form.Logger.Warn("ExcelCopy()", ex.Message);
            }
        }

        private void SelectParentsRecursive(int parentRowHandle)
        {
            try
            {
                if (!parentRowHandlesList.Contains(parentRowHandle) && gridView1.IsGroupRow(parentRowHandle))
                    parentRowHandlesList.Add(parentRowHandle);

                if (gridView1.IsGroupRow(parentRowHandle) && !selectedRows.Contains(parentRowHandle)
                    && !parentsNotSelectedRowHandlesList.Contains(parentRowHandle))
                    parentsNotSelectedRowHandlesList.Add(parentRowHandle);

                if (gridView1.GetParentRowHandle(parentRowHandle) > parentRowHandle && (!parentRowHandlesList.Contains(gridView1.GetParentRowHandle(parentRowHandle))))
                {
                    SelectParentsRecursive(gridView1.GetParentRowHandle(parentRowHandle));
                }
            }
            catch (Exception ex)
            {
                form.Logger.Warn("SelectParentsRecursive()", ex.Message);
            }
        }

        #endregion ExcelCopy

        public override void SetEvents(List<JEvent> events)
        {
            if (definition.ForceRestoreLayout == true && definition.SaveLayout == true)
            {
                gridView1.ColumnPositionChanged += (s, e) =>
                {
                    if (IsInited == true && Action != "populating")
                    {
                        SaveLayoutToVariable();
                    }
                };
                gridView1.ColumnWidthChanged += (s, e) =>
                {
                    if (IsInited == true && Action != "populating")
                    {
                        SaveLayoutToVariable();
                    }
                };
            }


            DateTime startTime = DateTime.Now;
            if ((definition.Styles != null && definition.Styles.StyleList != null))
            {
                if (string.IsNullOrEmpty(definition.Styles.Type) || definition.Styles.Type.ToLower() == "cell")
                    gridView1.RowCellStyle += (s, e) =>
                    {
                        RowCellStyle(s, e);
                    };
                else if (definition.Styles.Type.ToLower() == "row")
                    SetEventRowStyle();
            }

            //za editor (npr. checkbox) i Drag&Drop
            gridView1.MouseDown += (s, e) =>
            {
                MouseDown(s, e);
                if (definition.KeepSelectionAfterFilter == true && definition.ShowCheckBoxes == true)
                {
                    selectionControl.MouseDown(gridView1, e);
                }
            };

            if (definition.AllowDrag == true)
            {
                gridView1.MouseMove += (s, e) =>
                {
                    MouseMove(s, e);
                };
            }

            gridView1.CustomDrawCell += (s, e) =>
            {
                try
                {
                    //sakrivanje chcekboxa iz autofilter reda
                    if (e.RowHandle == GridControl.AutoFilterRowHandle && editCheckColumns.FirstOrDefault(x => x.Name == e.Column.FieldName) != null)
                        e.Handled = true;

                    if (e.Column.FieldName == "CarpoRowNumberColumn" && e.RowHandle >= 0)
                    {
                        e.DisplayText = (e.RowHandle + 1).ToString();

                        SetStylesHelper(true, "GridRowCellStyle", e.Appearance);
                    }
                }
                catch { }
            };

            gridView1.KeyDown += (s, e) =>
            {
                {
                    // Overajdano šaltanje po gridu. Prethodno CTRL+, a sada samo HOME i END
                    if (e.KeyCode == Keys.End)
                    {
                        gridView1.FocusedRowHandle = gridView1.RowCount - 1;
                        e.Handled = true;
                    }
                    else if (e.KeyCode == Keys.Home)
                    {
                        gridView1.FocusedRowHandle = 0;
                        e.Handled = true;
                    }
                    ////////////////////////////////////////////////////////////////////////

                    if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.C)
                    {
                        if (gridView1.SelectedRowsCount > 0)
                        {
                            using (Carpo.Core.Forms.GridExport frm = new Core.Forms.GridExport())
                            {
                                frm.cmdFullGrid.Text = Common.Translator.TranslateString("Standardni izvoz");
                                frm.cmdSelectedRows.Text = Common.Translator.TranslateString("Izvoz u Excel");
                                frm.Text = Common.Translator.TranslateString("Izvoz podataka");

                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    if (frm.ExportTip == Core.Forms.GridExport.Export.SelectedRows)
                                    {
                                        ExcelCopy(e);
                                    }
                                    else
                                    {
                                        #region stariCopy

                                        int[] selectedRows = GetSelectedRows();

                                        List<int> selectedRowsList = new List<int>();
                                        List<int> parentRowHandles = new List<int>();

                                        // kopiram sve iz int[] u List<int> i punim listu parenta (po distinct pravilu)
                                        foreach (int i in selectedRows)
                                        {
                                            selectedRowsList.Add(i);

                                            int parent = gridView1.GetParentRowHandle(i);
                                            if (!parentRowHandles.Contains(parent))
                                                parentRowHandles.Add(parent);
                                        }
                                        // dodatno selektiram sve parente da bi mogli kopirati u Excel
                                        foreach (int i in parentRowHandles)
                                        {
                                            gridView1.SelectRow(i);
                                        }

                                        #endregion stariCopy
                                    }
                                }
                            }
                        }
                    }

                    if (gridView1.FocusedRowHandle >= 0)
                    {
                        if (e.KeyCode == Keys.Return && (gridView1.ActiveEditor != null || definition.EnterMoveNextEditor == true))
                        {
                            //if (gridView1.ActiveEditor.GetType() != typeof(TextEdit))
                            //    return;
                            //(s as ColumnView).CloseEditor();
                            //(s as ColumnView).MoveNext();
                            (s as GridView).CloseEditor();
                            GetNextEditableCell();
                            //(s as GridView).MoveNext();
                            e.Handled = false;
                        }
                    }
                }
            };

            gridView1.PopupMenuShowing += (s, e) =>
            {
                PopupMenuShowing(s, e);
            };

            gridView1.ShowingEditor += (s, e) =>
            {
                //GridColumn focusedField = gridView.FocusedColumn;
                //if (focusedField.VisibleIndex == 2 && gridView1.FocusedRowHandle < 3)
                //{
                //    e.Cancel = true;
                //}
                ShowingEditor(s, e);
            };

            gridView1.ShownEditor += (s, e) =>
            {
                ShownEditor(s, e);
            };

            gridView1.ColumnFilterChanged += (s, e) =>
            {
                if (definition.KeepSelectionAfterFilter == true && definition.ShowCheckBoxes == true)
                {
                    selectionControl.RestoreSelection(gridView1);
                }
                SetFiltering();
                SetLabelCount();
            };


            if (definition.SelectionWebStyle == true && events != null)  // moralo biti napravljeno zbog WebStyle grida (ručno mu okidamo selectionchanged)
            {
                Console.WriteLine("a");
                var selectionChanged = events.Where(x => x.Type.StartsWith("selectionchanged")).FirstOrDefault();
                if (selectionChanged != null)
                {
                    // slijedeća 2 eventa bila su nužna za kreiranje jer nam se u suprotnom selectionchanged okidao kada bismo samo expandali i collapsali grupe
                    gridView1.GroupRowExpanding += (s, e) =>
                    {
                        expandingCollapsingWebStyle = true;
                    };
                    gridView1.GroupRowCollapsing += (s, e) =>
                    {
                        expandingCollapsingWebStyle = true;
                    };

                    gridView1.Click += (s, e) =>
                    {
                        try
                        {
                            var mouseHit = e as HandledMouseEventArgs;
                            if (mouseHit.Button == MouseButtons.Left)
                            {
                                GridHitInfo info;
                                Point pt = gridView1.GridControl.PointToClient(Control.MousePosition);
                                info = gridView1.CalcHitInfo(pt);

                                if (((info.Column != null && info.Column.FieldName == "CheckMarkSelection") && (info.InDataRow == true || info.InColumnPanel == true))
                                    || (gridView1.IsGroupRow(info.RowHandle) && expandingCollapsingWebStyle == false))
                                    OnSelectionChangedExtracted(); // ručno okidanje selectionchangeda
                            }
                        }
                        catch (Exception ex)
                        {
                            form.Logger.Error("gridView1.Click", ex.Message);
                        }
                        expandingCollapsingWebStyle = false;
                    };
                }
            }

            gridView1.StartGrouping += (s, e) =>
            {
                // ovo hendlam samo ako je inline grouping
                if (gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True)
                {
                    try
                    {
                        if (gridView1.GroupCount == 0)  // ako ungroup radim
                            LoadSavedColumnsModel();
                        else                            // ako grupiram
                        {
                            if (groupedColsCounter == 0)    // hoću save i odfixirati SAMO AKO tek počinjem grupirati (1. grupa)
                            {
                                SaveColumnsModelBeforeGroup();
                                HandleFixedColumns();       // "odFixiraj" fixirane kolone zbog grupiranja
                            }
                            groupedColsCounter = gridView1.GroupCount;
                        }
                    }
                    catch (Exception ex)
                    {
                        form.Logger.Warn("gridView1.StartGrouping()", ex.Message);
                    }
                }
            };

            gridView1.EndGrouping += (s, e) =>
            {
                // ovo hendlam samo ako je inline grouping
                if (gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True)
                {
                    if (gridView1.GroupCount > 0)   // ako grupiram
                        BestFitInlineColumns();
                    else                            // ako mičem group - vrati kolone kakve su bile prije grupiranja
                    {
                        groupedColsCounter = 0;
                        foreach (GridColumn gridCol in this.gridView1.Columns)
                            ProcessColumnWidth(gridCol, false);
                    }
                }
            };

            gridView1.CustomSummaryCalculate += GridView1_CustomSummaryCalculate;

            base.SetEvents(events);
            //form.Logger.InfoData("Postavljanje event-ova grida", definition.Name, startTime);
        }
        WeightedAverageCalculator calculator = null;
        private void GridView1_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            switch (e.SummaryProcess)
            {
                case CustomSummaryProcess.Start:
                    calculator = new WeightedAverageCalculator();
                    break;
                case CustomSummaryProcess.Calculate:
                    DataRow data = gridView1.GetDataRow(e.RowHandle);

                    var columnDefiniton = definitionFields.Find(x => x.Name == ((GridSummaryItem)e.Item).FieldName);

                    var fieldValue = data.Field<object>(columnDefiniton.WeightedColumnName) != null ? data.Field<decimal>(columnDefiniton.WeightedColumnName) : 0;
                    var row = e.FieldValue != null ? (decimal)e.FieldValue : 0;

                    calculator.Add(fieldValue, row);
                    break;
                case CustomSummaryProcess.Finalize:
                    e.TotalValue = calculator.Calculate();
                    calculator = null;
                    break;
            }
        }

        private void SetEventRowStyle()
        {
            gridView1.RowStyle += (s, e) =>
            {
                RowStyle(s, e);
                eventRowStyleActivated = true;
                //if ((e.State & DevExpress.XtraGrid.Views.Base.GridRowCellState.Focused) == DevExpress.XtraGrid.Views.Base.GridRowCellState.Focused)
                //{
                //e.Appearance.Assign(view.GetViewInfo().PaintAppearance.GetAppearance("FocusedRow"));
                //}
            };
        }

        #region Style

        private void RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                SetStyle(e.Appearance, e, e.RowHandle);
            }
        }

        private void RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                SetStyle(e.Appearance, e, e.RowHandle);
            }
        }

        private void SetStyle(AppearanceObject appearanceObject, EventArgs e, int rowHandle)
        {
            try
            {
                string type = "GridRowCellStyle";
                if (e is RowCellStyleEventArgs)
                    rowCellStyleEventArgs = (e as RowCellStyleEventArgs);
                if (e is RowStyleEventArgs)
                {
                    rowStyleEventArgs = (e as RowStyleEventArgs);
                    type = "GridRowStyle";
                }

                bool checkIfResult = true;

                //***begin netGen styles
                if (!string.IsNullOrEmpty(formatIdField))
                    SetRowStyle(appearanceObject, rowHandle, formatIdField, "format");

                if (!string.IsNullOrEmpty(rowColorIdField))
                {
                    SetRowStyle(appearanceObject, rowHandle, rowColorIdField, "backgroundcolor");

                    // dodao Ognjen
                    object colorObj = GetRowFieldValue(rowHandle, "rowcolorid");
                    string color = colorObj == null || colorObj == DBNull.Value ? "" : colorObj.ToString();
                    if (Carpo.UI.Logic.Style.GetColor(color) != null)
                        rowStyleEventArgs.HighPriority = true;
                    //
                }

                if (!string.IsNullOrEmpty(rowForeColorIdField))
                    SetRowStyle(appearanceObject, rowHandle, rowForeColorIdField, "fontcolor");
                //***end netGen styles

                SetStylesHelper(checkIfResult, type, appearanceObject);
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: SetStyle (grid: '" + gridControl.Name + "')", ex.Message, ex);
            }
        }

        private void SetStylesHelper(bool checkIfResult, string type, AppearanceObject appearanceObject)
        {
            if (definition.Styles != null && definition.Styles.StyleList.Count > 0)
            {
                foreach (JStyle style in definition.Styles.StyleList)
                {
                    if (Conditions.CheckCondition(ref checkIfResult, style, form, type, false) == false)
                        continue;

                    Logic.Style.SetStyle(appearanceObject, style.Name, style.Value);
                }
            }
        }

        private void SetRowStyle(AppearanceObject appearanceObject, int rowHandle, string fieldName, string styleType)
        {
            var value = GetRowFieldValue(rowHandle, fieldName);
            if (value != null && value != DBNull.Value)
                Logic.Style.ParseNetGenStyles(appearanceObject, value.ToString(), styleType);
        }

        #endregion Style

        #region Init&Properties

        private void Init()
        {
            DateTime startTime = DateTime.Now;
            //add string filter
            if (!string.IsNullOrEmpty(definition.Filter))
            {
                gridView1.ActiveFilterString = definition.Filter;
                gridView1.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.ShowAlways;
            }

            SetProperties();
            SetEvents(definition.Event);
            if (definition.Data != null && definition.AutoPopulate == true)
                Populate(null, false, null, FocusType.Default);
            form.Logger.InfoData("Init grida ", definition.Name, startTime);
        }

        private void SetProperties()
        {

            DateTime startTime = DateTime.Now;
            gridControl.TabStop = false;
            gridControl.MinimumSize = new Size(100, 100);

            //gridView1.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDown;

            this.AutoPopulate = definition.AutoPopulate;
            //dodano zbog buga kada se povećavao font samo kod bold i italic
            if (definition.FontSizeDelta != null)
            {

                gridView1.Appearance.Row.FontSizeDelta = (int)definition.FontSizeDelta;
                gridView1.Appearance.Row.Font = new Font(gridView1.Appearance.Row.Font.FontFamily, gridView1.Appearance.Row.Font.Size + (int)definition.FontSizeDelta);
                //Console.WriteLine(gridView1.Appearance.Row.FontSizeDelta);
            }

            if (definition.ShowGroupPanel == false)
                gridView1.OptionsView.ShowGroupPanel = false;

            if (definition.ShowGroupedColumns == false)
                gridView1.OptionsView.ShowGroupedColumns = false;
            else
                gridView1.OptionsView.ShowGroupedColumns = true;

            if (definition.ShowFooter == true)
                gridView1.OptionsView.ShowFooter = true;

            if (definition.ColumnAutoWidth == true)
                gridView1.OptionsView.ColumnAutoWidth = true;
            else
                gridView1.OptionsView.ColumnAutoWidth = false;

            if (definition.ShowColumnHeaders == false)
                gridView1.OptionsView.ShowColumnHeaders = false;

            if (definition.TabStop == true)
                gridControl.TabStop = true;

            SetGridCaption();

            gridView1.OptionsView.AllowHtmlDrawHeaders = true;
            if (definition.WordWrap == true)
            {
                gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
                gridView1.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            }

            //start I.L. 17.03.2024. - radi omogućavanje printa headera grida u više redova
            gridView1.OptionsPrint.UsePrintStyles = true;
            gridView1.AppearancePrint.HeaderPanel.TextOptions.Trimming = Trimming.Word;
            gridView1.AppearancePrint.HeaderPanel.TextOptions.WordWrap = WordWrap.Wrap;
            //end

            //provjeriti točnu funkcionalnost... zadržava stil fokusiranog reda
            gridView1.OptionsSelection.EnableAppearanceHideSelection = false;

            //za pojačavanje selektiranog reda - promijeniti boju
            //gridView1.Appearance.FocusedRow.BackColor = Color.FromArgb(255,255,192);
            //gridView1.Appearance.FocusedRow.BackColor = Color.FromArgb(210, 210, 255);
            //gridView1.Appearance.SelectedRow.BackColor = Color.FromArgb(220, 220, 255);

            //gridView1.OptionsBehavior.Editable = false;

            //gridView1.OptionsView.ColumnAutoWidth = false;
            if (definition.EnableAppearanceOddRow != false)
                gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView1.OptionsPrint.AutoWidth = false;

            if (!(definition.ShowAutoFilterRow == false))
                gridView1.OptionsView.ShowAutoFilterRow = true;

            // Opt-in DevExpress Find Panel (default off — existing granules unchanged).
            if (definition.ShowFindPanel == true)
                SetAttributeFindPanel(true, definition.FindPanelMode);
            else if (definition.ShowFindPanel == false)
                SetAttributeFindPanel(false, null);

            // Wave12: empty-state message when grid has no rows.
            ApplyEmptyMessageHandler();

            // Opt-in Filter Editor (default: leave DX defaults; true enables, false disables).
            if (definition.AllowFilterEditor == true)
                gridView1.OptionsFilter.AllowFilterEditor = true;
            else if (definition.AllowFilterEditor == false)
                gridView1.OptionsFilter.AllowFilterEditor = false;

            // Opt-in Column Chooser polish (default off — existing granules unchanged).
            if (definition.AllowColumnChooser == true)
                ApplyColumnChooserOptions(true);

            gridView1.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Default;
            //

            gridView1.OptionsFilter.AllowAutoFilterConditionChange = DefaultBoolean.False;
            gridView1.OptionsBehavior.AllowIncrementalSearch = true;
            // gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.True;
            gridView1.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
            gridView1.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.True;
            gridView1.OptionsClipboard.CopyCollapsedData = DevExpress.Utils.DefaultBoolean.True;
            gridView1.OptionsMenu.ShowConditionalFormattingItem = true;
            // Uključuje Conditional Formatting pravila (FormatRules) u SaveLayout/RestoreLayout,
            // tako da se mogu snimiti u CarpoVars zajedno sa ostalim podešavanjima grida.
            gridView1.OptionsLayout.StoreFormatRules = true;
            gridView1.OptionsMenu.ShowGroupSummaryEditorItem = true;
            gridView1.OptionsPrint.ExpandAllGroups = false;
            gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            if (definition.ShowCheckBoxes == true)
                gridView1.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DefaultBoolean.True;

            //tomic kopiranje
            if (definition.AllowCopyCell == true)
            {
                gridView1.OptionsBehavior.Editable = false;
                gridView1.OptionsSelection.MultiSelect = true;
                gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
                gridView1.OptionsClipboard.CopyColumnHeaders = DefaultBoolean.False;
            }
            //gridView1.OptionsSelection.MultiSelect = true;

            //// Draw a dotted focus rectangle around the entire row.
            //gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;

            if (definition.SelectionWebStyle != true)
            {
                if (definition.MultiSelect == true || definition.ShowCheckBoxes == true)
                    gridView1.OptionsSelection.MultiSelect = true;

                if (definition.ShowCheckBoxes == true)
                {
                    gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 24;
                    gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                    gridView1.OptionsSelection.UseIndicatorForSelection = false;
                }
            }
            else
                gridView1.OptionsSelection.UseIndicatorForSelection = false;

            //selektira tekst unutar editabilne ćelije
            gridView1.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;

            // After AllowCopyCell (which forces Editable=false) so EditForm can re-enable editing.
            ApplyEditFormOptions();

            gridView1.Appearance.FooterPanel.Font = new Font(gridView1.Appearance.FooterPanel.Font, FontStyle.Bold);

            searchField = definition.SearchField;
            //form.Logger.InfoData("Postavljanje propertija grida ", definition.Name, startTime);
        }

        #endregion Init&Properties

        #region DragRows

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
            {
                GridView view = sender as GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                {
                    if (hi.InRowCell && hi.RowHandle >= 0)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(RepositoryItemCheckEdit) &&
                            hi.Column.FieldName != GridView.CheckBoxSelectorColumnName &&
                            hi.Column.ColumnEdit != null)
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            SetFocusSelected();
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit edit = (view.ActiveEditor as CheckEdit);
                            if (edit != null)
                            {
                                edit.Toggle();
                                (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            }
                        }
                    }
                }
            }

            dragHitInfo = gridView1.CalcHitInfo(new Point(e.X, e.Y));
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (dragHitInfo == null || !dragHitInfo.InDataRow) return;
            if (e.Button != MouseButtons.Left) return;
            Rectangle dragRect = new Rectangle(new Point(
                dragHitInfo.HitPoint.X - SystemInformation.DragSize.Width / 2,
                dragHitInfo.HitPoint.Y - SystemInformation.DragSize.Height / 2), SystemInformation.DragSize);
            if (!dragRect.Contains(new Point(e.X, e.Y)))
            {
                int[] data = GetSelectedRows();
                gridControl.DoDragDrop(data, DragDropEffects.Copy);
            }
        }

        #endregion DragRows

        public void Populate(JEventAction eventAction, bool? isEmpty, bool? async, FocusType focusType)
        {

            DateTime startTime = DateTime.Now;
            Action = "populating";
            try
            {
                SetFocusedValue(eventAction);

                //if (focusType == FocusType.KeepFocus)
                //{
                if (refreshHelper != null)
                    refreshHelper.SaveViewInfo();
                //}

                //if (focusType != FocusType.KeepFocus)
                //{
                //    if (gridControl.DataSource != null)
                //    {
                gridControl.DataSource = null;
                RowsCount = 0;
                isFiltering = false;
                SetLabelCount();

                //    }
                //}

                bool populateGrid = true;
                if (MasterControl != null)
                {
                    GridView gridViewMaster = MasterControl.ComponentExt.gridView1;

                    if (!(gridViewMaster.FocusedRowHandle >= 0) && (MasterControl as JGridControl).Definition.PreventFocusGroup != false)
                        populateGrid = false;
                }

                if (populateGrid == true)
                {
                    //if (refreshHelper != null)
                    //    refreshHelper.SaveViewInfo();

                    // B6: opt-in VirtualServerMode — isolated from default DataTable path.
                    if (definition.VirtualServerMode == true)
                    {
                        BindVirtualServerMode(focusType);
                    }
                    // Opt-in background fetch: async="true" on grid or populateasync action.
                    // Default path stays synchronous so existing granules keep focus/event ordering.
                    else if ((definition.Async == true || async == true) && async != false)
                    {
                        DisposeVirtualServerModeBinder();
                        GetDataAsync(focusType);
                    }
                    else
                    {
                        DisposeVirtualServerModeBinder();
                        _ = GetDataAsync();
                        if (definition.ResetFocus == true)//treba drugačije
                        {
                            SetState(FocusType.KeepFocus);
                        }
                        else
                        {
                            SetState(focusType);
                        }


                        //ako je fokusiran 1.red ili ako je prazan grid okini FocusedRowChanged
                        //u suprotnom je već okinut preko SetState-a
                        // + Ognjen edit 20.03.2017. dodao provjeru da li je 'ignorefocus' na snazi
                        //jer se focus okidao 2x u kombinaciji control populate i control focus

                        if ((gridView.FocusedRowHandle <= 0 && Action2 != "ignorefocus") || gridView1.RowCount == 1)
                            FocusedRowChanged(null);
                    }

                    SetFocusOnSearchRow();

                    //nakon populate i ako je uključen multiselect postavi select na fokusirani red
                    //u suprotnom selektiran je uvije prvi red
                    SetFocusSelected();
                }
                CreateGroupSummary();
                SetColumnFilterPopupMode();
                SetGridCaption();
                if (initInd)
                {
                    SetGroupSummaryStyle();
                    initInd = false;
                }

                SetConditionalFormatting();
            }
            catch (Exception ex)
            {
                form.Logger.Error(ex.Message, ex.StackTrace.ToString());
            }
            finally
            {
                Action2 = "";
            }
            //lblCnt.Text = cnt.ToString();
            if (definition.ExpandAllOnLoad == true)
                gridView1.ExpandAllGroups();

            form.Logger.InfoData("Populate grida ", definition.Name, startTime);

            TranslateGridColumns(gridView);




        }
        public static void TranslateGridColumns(GridView dataGridView)
        {
            foreach (GridColumn column in dataGridView.Columns)
            {

                column.Caption = Carpo.Common.Translator.TranslateString(column.Caption);
            }
        }
        private void SetColumnFilterPopupMode()
        {
            // prebačeno sa evenata ovdje jer nije bilo moguće zaletit se kroz sve kolone prije nego ih dodamo (AddGridColumns())
            if (definition.AllowFilter == true)// && setAllowFilter)
            {
                gridView1.OptionsCustomization.AllowFilter = true;

                foreach (GridColumn gc in gridView1.Columns)
                {
                    gc.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
                }
            }
            else
                gridView1.OptionsCustomization.AllowFilter = false;
        }

        private void SetFocusedValue(JEventAction action)
        {
            if (IsSearchDocMode() == true)
                return;

            if (action != null && action.Actions.Count > 0 && !string.IsNullOrEmpty(action.Value))
            {
                var subAction = action.Actions.Where(x => !string.IsNullOrEmpty(x.Value) && x.Value.ToLower() == action.Value.ToLower());
                if (subAction.Count() > 0)
                {
                    //ignoriraj fokus; naknadno ga postavljam!!
                    Action2 = "ignorefocus";
                    return;
                }
            }

            //***čuvanje focusa
            KeyFocusedValue = null;
            if (gridView1.FocusedRowHandle >= 0)
            {
                focusedRowHandle = gridView1.FocusedRowHandle;
                if (focusedRowHandle >= 0 && keyField != null)
                    KeyFocusedValue = GetRowFieldValue(focusedRowHandle, keyField);
                topRowIndex = gridView1.TopRowIndex;
            }
        }

        private void SetFocusOnSearchRow()
        {
            //DateTime startTime = DateTime.Now;
            if (form.Action == "searchdoc" && !string.IsNullOrEmpty(searchField))
            {
                string field = Logic.Data.Core.FindDataColumn(keyField, DataColumns);
                LocateByValue(keyField, form.SearchDataRow[searchField]);
            }
            //form.Logger.InfoData("SetFocusOnSearchRow", definition.Name, startTime);
        }

        //focus i grupe
        private void SetState(FocusType focusType)
        {
            if (IsSearchDocMode() == true)
                return;

            if (Action2 != "ignorefocus")
            {
                if (focusType != FocusType.FocusMoveFirst)
                {
                    if (refreshHelper != null)
                        refreshHelper.LoadViewInfo(focusType);

                    if (focusType == FocusType.KeepFocus && gridView1.FocusedRowHandle != focusedRowHandle)
                    {
                        gridView1.TopRowIndex = topRowIndex;
                        if (KeyFocusedValue != null)
                        {
                            try
                            {
                                LocateByValue(KeyFocusedValue);
                            }
                            catch
                            {
                                gridView1.FocusedRowHandle = focusedRowHandle;
                            }
                        }
                    }
                }
                else
                {
                    if (IsCollapsed == false)
                    {
                        gridView1.ExpandAllGroups();
                    }
                }
            }
            //gridView1.FocusedRowHandle = gridView1.LocateByValue(Carpo.UI.Logic.Data.Core.FindDataColumn("MjestaId", DataColumns), 11);
        }

        private void LocateByValue(object value)
        {
            LocateByValue(keyField, value);
        }

        private bool IsSearchDocMode()
        {
            if (form.Action == "searchdoc" && !string.IsNullOrEmpty(searchField))
                return true;

            return false;
        }

        private void LocateByValue(string field, object value)
        {
            gridView1.FocusedRowHandle = gridView1.LocateByValue(field, value);
            SetFocusSelected();
        }

        //nakon populate i ako je uključen multiselect postavi select na fokusirani red
        //u suprotnom selektiran je uvije prvi red
        private void SetFocusSelected()
        {
            //DateTime startTime = DateTime.Now;
            if (definition.MultiSelect == true && definition.ShowCheckBoxes != true && definition.SelectionWebStyle != true)
            {
                gridView1.ClearSelection();
                if (gridView1.FocusedRowHandle >= 0)
                    gridView1.SelectRow(gridView1.FocusedRowHandle);
            }
            //form.Logger.InfoData("SetFocusSelected", definition.Name, startTime);
        }

        private async void GetDataAsync(FocusType focusType)
        {
            gridView1.BeginUpdate();
            bool useSplashCaption = !string.IsNullOrWhiteSpace(definition.AsyncCaption);
            bool splashShown = false;
            if (useSplashCaption)
                splashShown = TryShowAsyncSplash(definition.AsyncCaption);
            else
                gridView1.ShowLoadingPanel();

            try
            {
                bool stopActions = false;
                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);
                var sw = Stopwatch.StartNew();
                var t = Task.Factory.StartNew<DataSet>(() => dataDefinition.GetDataSet("punjenje grida '" + definition.Name + "'", out stopActions));
                DataSet ds = await t;//  t.Result; //vedrantomic
                int rows = ds != null && ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count : 0;
                form.Logger?.Performance("ComponentGrid",
                    $"GridFill name={definition.Name} rows={rows}", sw, "GridFill");
                SetData(ds);
                SetState(focusType);
                if ((gridView1.FocusedRowHandle <= 0 && Action2 != "ignorefocus") || gridView1.RowCount == 1)
                    FocusedRowChanged(null);
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: ComponentGrid - GetDataAsync" + gridControl.Name, ex.Message, ex);
            }
            finally
            {
                if (splashShown)
                    TryCloseAsyncSplash();
                else
                    gridView1.HideLoadingPanel();
                gridView1.EndUpdate();
                Action = "";
            }
        }

        private bool TryShowAsyncSplash(string caption)
        {
            try
            {
                form.splashScreenManager1.ShowWaitForm();
                form.splashScreenManager1.SetWaitFormCaption("Carpo");
                form.splashScreenManager1.SetWaitFormDescription(caption);
                return true;
            }
            catch
            {
                gridView1.ShowLoadingPanel();
                return false;
            }
        }

        private void TryCloseAsyncSplash()
        {
            try { form.splashScreenManager1.CloseWaitForm(); }
            catch { }
        }

        public void SetGroupSummaryStyle(string groupSummaryStyle = "")
        {
            if (string.IsNullOrEmpty(groupSummaryStyle))
                groupSummaryStyle = !string.IsNullOrEmpty(this.definition.GroupSummaryStyle) ? this.definition.GroupSummaryStyle : "";
            // gledaj prvo proslijeđeni string, ako ga nema traži atribut grida i postavi varijablu u njegovu vrijednost
            switch (groupSummaryStyle)
            {
                case "inline":
                    GridGroupSumsInline();
                    break;

                case "under":
                    GridGroupSumsUnder();
                    break;

                case "ifexpanded":
                    GridGroupSumsIfExpanded();
                    break;

                default:
                    GridGroupSumsUnder();
                    break;
            }
        }

        private void RestoreLayout()
        {
            DateTime startTime = DateTime.Now;
            //ako forisaram punjenje grida, ne snima se layout
            if (isRestored == false && definition.SaveLayout == true && ForceReinitFields == false)
            {
                List<GridGroupSummaryItem> beforeLoadList = new List<GridGroupSummaryItem>();///group summary bug fix vj
                foreach (GridGroupSummaryItem item in gridView1.GroupSummary)
                {
                    beforeLoadList.Add(item);
                }

                if (!string.IsNullOrEmpty(definition.LayoutName))
                {
                    // settiram na početni da kasnije ne bude null
                    using (Parser parser = new Parser(form, null))
                        definition.PreviousLayoutName = parser.GetLogicPartValue(definition.LayoutName).ToString();
                }

                Logic.VarsLayout l = new VarsLayout(form, definition.LayoutVersion);
                l.SetLayout(control.Name, definition.LayoutName, definition.PreviousLayoutName);
                SetGridCaption();
                isRestored = true;

                gridView1.GroupSummary.Clear();
                foreach (GridGroupSummaryItem item in beforeLoadList)
                {
                    gridView1.GroupSummary.Add(item);
                }
            }

            //fix za Manager detail kontrolu
            else if (definition.ForceRestoreLayout == true && definition.SaveLayout == true)
            {
                Logic.VarsLayout l = new VarsLayout(form, definition.LayoutVersion);
                l.SetLayout(control.Name, definition.LayoutName, definition.PreviousLayoutName);

                isRestored = true;
            }
            //form.Logger.InfoData("RestoreLayout ", definition.Name, startTime);

        }
        //public void ForceRestoreLayout()
        //{
        //    isRestored = false;
        //    RestoreLayout();
        //}

        private void SaveLayoutToVariable()
        {
            //fix za Manager detail kontrolu; Ako postoji MasterControla - radi se o detalj gridu
            if (definition.ForceRestoreLayout == true && definition.SaveLayout == true)
            {

                //ako je layout već restoran, pohrani trenutni izgled grida u varijablu jer kod refresha ide po izgled iz varijable, tj. ako su mijenjane postavke restorat će se na taj izgled
                if (isRestored == true)
                {
                    var currentLayout = form.CarpoVarsLayout.Where(x => x.Name == control.Name).FirstOrDefault();
                    if (currentLayout != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            gridView1.SaveLayoutToStream(ms);

                            ms.Seek(0, System.IO.SeekOrigin.Begin);
                            StreamReader reader = new StreamReader(ms);
                            string text = reader.ReadToEnd().Replace("utf-8", "utf-16");
                            currentLayout.ValueXml = text;
                        }
                    }

                }
            }
        }

        private async Task GetDataAsync()
        {

            Cursor.Current = Cursors.WaitCursor;
            gridView1.BeginUpdate();
            try
            {
                bool stopActions = false;
                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);

                // Sync populate path: keep SQL on caller thread so SetState/focus ordering stays correct.
                var sw = Stopwatch.StartNew();
                DataSet ds = await Task.FromResult<DataSet>(dataDefinition.GetDataSet("punjenje grida '" + definition.Name + "'", out stopActions));
                int rows = ds != null && ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count : 0;
                form.Logger?.Performance("ComponentGrid",
                    $"GridFill name={definition.Name} rows={rows}", sw, "GridFill");
                SetData(ds);
            }
            catch { throw; }
            finally
            {
                gridView1.EndUpdate();
                Cursor.Current = Cursors.Default;
                Action = "";
            }

        }

        private void DisposeVirtualServerModeBinder()
        {
            if (vsmBinder == null)
                return;
            try { vsmBinder.Dispose(); }
            catch { /* ignore */ }
            vsmBinder = null;
        }

        /// <summary>
        /// B6: bind VirtualServerModeSource via paging SP. Schema from first page; default SetData path unused.
        /// </summary>
        private void BindVirtualServerMode(FocusType focusType)
        {
            Cursor.Current = Cursors.WaitCursor;
            gridView1.BeginUpdate();
            try
            {
                DisposeVirtualServerModeBinder();

                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);
                JData jData = dataDefinition.GetJData();
                if (jData == null || string.IsNullOrWhiteSpace(jData.Command))
                {
                    form.Logger.Error("B6 VirtualServerMode", "virtualservermode=\"true\" requires data command (paging SP).");
                    Common.Helpers.JMessages.Info(
                        "virtualservermode=\"true\" zahtijeva data command (paging SP, npr. sp_DxAudit26_BigGrid_Page).\n" +
                        "Default DataTable path nije korišten — SP mora biti deployan.");
                    return;
                }

                if (!string.IsNullOrEmpty(jData.Type) &&
                    string.Equals(jData.Type, "text", StringComparison.OrdinalIgnoreCase))
                {
                    form.Logger.Error("B6 VirtualServerMode", "virtualservermode does not support type=\"text\"; use stored procedure paging contract.");
                    Common.Helpers.JMessages.Info(
                        "virtualservermode ne podržava type=\"text\".\n" +
                        "Koristi stored procedure s @Skip/@Take/@Sort/@Filter/@TotalCount.");
                    return;
                }

                string procedureName = jData.Command.Trim();
                vsmBinder = new VirtualServerModeBinder(form, gridControl, gridView1, form.ConnectionString, procedureName, pageSize: 50);

                DataTable firstPage;
                int total;
                try
                {
                    firstPage = vsmBinder.FetchPage(0, 50, "Id ASC", null, out total);
                }
                catch (Exception ex)
                {
                    DisposeVirtualServerModeBinder();
                    form.Logger.Error("B6 VirtualServerMode", "Paging SP failed: " + ex.Message, ex);
                    Common.Helpers.JMessages.Info(
                        "B6 VirtualServerMode: paging SP nije uspio.\n" +
                        procedureName + "\n" + ex.Message + "\n" +
                        "Pokreni Demo\\DxAudit26\\Sql\\B6_VirtualServerMode_Test.sql na bazi.");
                    return;
                }

                if (firstPage == null)
                {
                    DisposeVirtualServerModeBinder();
                    return;
                }

                DataSet schemaDs = new DataSet();
                schemaDs.Tables.Add(firstPage.Copy());
                SetDataColumnsProp(schemaDs);

                if (ForceReinitFields == true || this.IsInited == false)
                {
                    if (ForceReinitFields == true)
                    {
                        definitionFields = PopulateFieldList();
                        this.IsBestFitted = false;
                    }
                    InitGrid(schemaDs);
                    this.IsInited = true;
                }

                vsmBinder.Bind(total);
                RowsCount = total;
                SetLabelCount();

                form.Logger.Info("B6 VirtualServerMode",
                    "Bound '" + definition.Name + "' → " + procedureName + "; TotalCount=" + total);

                if (definition.ResetFocus == true)
                    SetState(FocusType.KeepFocus);
                else
                    SetState(focusType);

                if ((gridView.FocusedRowHandle <= 0 && Action2 != "ignorefocus") || gridView1.RowCount == 1)
                    FocusedRowChanged(null);
            }
            catch (Exception ex)
            {
                DisposeVirtualServerModeBinder();
                form.Logger.Error("B6 VirtualServerMode Bind", ex.Message, ex);
                throw;
            }
            finally
            {
                gridView1.EndUpdate();
                Cursor.Current = Cursors.Default;
                Action = "";
            }
        }

        private void SetData(DataSet ds)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                if (ds == null || ds.Tables.Count == 0)
                    return;

                SetDataColumnsProp(ds); // prebačeno iz InitGrid zato jer bi ga trebali pri svakom populate-u setat.

                //definitionFields = new JFields() { FieldList = new List<JField>() };
                //definitionFields.FieldList = new List<JField> {
                //    new JField { Name = "Naziv", FontBold = true, Hidden = true },
                //    new JField { Name = "HptPoziv", Hidden = true }
                //};
                //InitGrid(ds);

                //ako postoji if uvijet na field polju, forsiraj reinicijalizaciju grida (fieldova)
                if (ForceReinitFields == true)
                {
                    definitionFields = PopulateFieldList();
                    this.IsBestFitted = false;
                    InitGrid(ds);
                    this.IsInited = true;
                }

                if (this.IsInited == false)
                {
                    InitGrid(ds);
                    this.IsInited = true;
                }

                if (MasterControl != null && MasterControl.DataSource != null && (MasterControl.DataSource as DataTable).Rows.Count == 0)
                {
                    RowsCount = 0;
                }
                else
                {
                    gridControl.DataSource = ds.Tables[0];
                    //this.IsInited = true;
                    RowsCount = ds.Tables[0].Rows.Count;

                    //if (RowsCount == 0)
                    //FocusedRowChanged(null);

                    BestFitColumnsInit();
                }

                if (DefaultLayoutStream == null)
                {
                    DefaultLayoutStream = new MemoryStream();
                    gridView1.SaveLayoutToStream(DefaultLayoutStream);
                }

                Carpo.Common.Helpers.GridTemplates.SetGridColumnsFromTemplate(GetTemplateVisibleColumns(GetTemplatesModel()), gridView1);

                SetRowNumberColumnCaption();

                RestoreLayout();

                // T.A. proširio uvjet - ne želim best fit na initu grida ako je savean-loadan layout
                if (gridInited)  // ako je inicijaliziran grid
                {
                    if (definition.BestFitOnRefresh == true)
                        BestFitColumns();
                }
                else  // ako se grid inicijalizira
                {
                    if (definition.BestFitOnRefresh == true && !isRestored)
                        BestFitColumns();
                }
                this.gridInited = true;

                SetLabelCount();
            }
            catch (Exception ex)
            {
                form.Logger.Error(ex.Message, "grid " + control.Name, ex);
                //Common.Helpers.JMessages.Error(ex.Message);
            }
            form.Logger.InfoData("SetData", definition.Name, startTime);
        }

        private void SetRowNumberColumnCaption()
        {
            var col = gridView1.Columns["CarpoRowNumberColumn"];

            if (col != null)
            {
                if (string.IsNullOrEmpty(col.Caption.Trim()))
                {
                    if (RowsCount > 99)
                    {
                        for (int i = 3; i <= (RowsCount.ToString().Length); i++)
                        {
                            col.Caption = col.Caption + "   ";
                        }
                    }
                }
            }
        }

        public void SetLabelCount()
        {
            if (lblCount != null)
            {
                if (isFiltering == true)
                    lblCount.Caption = lblCountCaption.Replace("##", RowsCount.ToString()) + " <b>(" + gridView1.RowCount + ")</b>";
                else
                    lblCount.Caption = lblCountCaption.Replace("##", RowsCount.ToString());
            }
            if (tabPage != null)
            {
                tabPage.Text = (tabPageCaption + " (").Trim() + RowsCount.ToString() + ")";
            }
        }

        private void SetFiltering()
        {
            if (gridView1.ActiveFilter.IsEmpty != true && gridView1.ActiveFilterEnabled)
                isFiltering = true;
            else
                isFiltering = false;
        }

        private void SetDataColumnsProp(DataSet ds)
        {
            //case-insensitive - custom rješenje
            //1. vrijerdnosti u array
            string[] arrDtColumns = ds.Tables[0].Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            this.DataColumns = arrDtColumns;
        }

        private void InitGrid(DataSet ds)
        {
            try
            {
                if (gridView1.Name == "")   // dodano za tab gridove (osim prvog taba, imaju naziv "") - font je bio 8.25
                    gridView1.Appearance.Row.Font = new Font("tahoma", 9F);

                gridView1.OptionsView.AllowHtmlDrawHeaders = true; // T.A. - omogućeno za html formatiranje captiona kolone

                if (definitionFields == null)
                {
                    definitionFields = new List<JField>();
                    List<JField> fieldList = PopulateFieldListFromDb(false);

                    definitionFields = fieldList;
                }

                SetPreviewField();

                formatIdField = Logic.Data.Core.FindDataColumn("formatid", DataColumns);
                rowForeColorIdField = Logic.Data.Core.FindDataColumn("rowforecolorid", DataColumns);
                rowColorIdField = Logic.Data.Core.FindDataColumn("rowcolorid", DataColumns);

                if (eventRowStyleActivated == false &&
                    (!string.IsNullOrEmpty(formatIdField) ||
                    !string.IsNullOrEmpty(rowForeColorIdField) ||
                    !string.IsNullOrEmpty(rowColorIdField)))
                    SetEventRowStyle();

                //punim željene kolone i zamjenjujem upisana imena sa stvarnim imenima kolona
                //također, ako kolona ne postoji, izostavljam
                //samo prvim ulazom
                AddGridColumns();

                if (!string.IsNullOrEmpty(definition.Key))
                {
                    keyField = Logic.Data.Core.FindDataColumn(definition.Key, DataColumns);
                    if (keyField == null)
                        form.Logger.Warn("Neispravan naziv kolone", "Key kolona '" + definition.Key + "' grida '" + gridControl.Name + "' nije pronađena");
                }

                if (keyField != null)
                    refreshHelper = new Common.Helpers.GridRefresh(gridView1, keyField);
                else if (definition.AllowDrag == true)
                {
                    form.Logger.Warn("grid: '" + gridControl.Name + "'; AllowDrag funkcionalnost", "Za ispravno funkcioniranje 'drag' funckionalnosti potrebno je definirati key kolonu preko atributa 'key'");
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("InitGrid", ex.Message, ex);
            }
        }

        #region Columns

        private void SetPreviewField()
        {
            previewField = definition.PreviewField;
            if (string.IsNullOrEmpty(previewField))
                return;

            previewField = Logic.Data.Core.FindDataColumn(previewField, DataColumns);
            if (string.IsNullOrEmpty(previewField))
                return;

            gridView1.PreviewFieldName = previewField;
            gridView1.OptionsView.AutoCalcPreviewLineCount = true;
            gridView1.OptionsView.ShowPreview = true;
            //gridView1.OptionsView.ShowPreviewRowLines = DefaultBoolean.True;
        }

        //sve kolone iz baze koje ne završavaju sa id
        private List<JField> PopulateFieldListFromDb(bool fieldsDefined)
        {
            List<JField> fieldList = new List<JField>();

            // u slučaju da nije definiran FIELDS listu punim sa svim poljima osim "Id"
            // u suprotnom listu punim sa svim poljima (a kasnije ću njima dodatno upravljati)
            foreach (string column in this.DataColumns)
            {
                if (fieldsDefined == false)
                {
                    if (!column.ToLower().EndsWith("id"))
                        fieldList.Add(new JField() { Name = column });
                }
                else
                    fieldList.Add(new JField() { Name = column });
            }
            return fieldList;
        }

        private List<JField> PopulateFieldList()
        {
            bool checkIfResult = true;

            List<JField> fields = new List<JField>();
            if (definition.Fields != null)
            {
                foreach (var item in definition.Fields.FieldList)
                {
                    if (Conditions.CheckCondition(ref checkIfResult, item, form, null, false) == false)
                        continue;
                    fields.Add(item);
                }
            }
            return fields;
        }

        public void BestFitColumnsInit()
        {
            if (this.IsBestFitted == false && RowsCount > 0)
            {
                FormatGridColumns();
                if (definition.BestFitOnRefresh == true)
                {
                    BestFitColumns();
                }

                this.IsBestFitted = true;
                SetFocusSelected();
            }
        }

        private void BestFitColumns()
        {
            //ako je definiran atribut "savelayout", onda prvo provjeri postji li snimljeni layout u layout varijablama
            //savelayout nadjačava betfit
            if (definition.SaveLayout == true)
            {
                Logic.VarsLayout l = new VarsLayout(form, definition.LayoutVersion);
                string layout = l.GetLayout(control.Name, definition.LayoutName, definition.PreviousLayoutName);
                if (!string.IsNullOrEmpty(layout))
                {
                    return;
                }
            }

            bool isDefault = true;
            if (!string.IsNullOrEmpty(definition.BestFitMaxRowCount))
            {
                int index;
                bool isNumeric = int.TryParse(definition.BestFitMaxRowCount, out index);

                if (isNumeric == true)
                {
                    gridView1.BestFitMaxRowCount = index;
                    isDefault = false;
                }
                else
                    form.Logger.Warn("Neispravno definiran atribut 'bestfitmaxrowcount'", "Atribut 'bestfitmaxrowcount' može sadržavati samo cjelobrojne vrijednosti!");
            }

            if (isDefault)
            {
                gridView1.BestFitMaxRowCount = 50; //perfomanse
            }

            if (gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True && gridView1.GroupCount > 0) // ako nije grupirano best fit obicni
            {
                BestFitInlineColumns();
            }
            else
            {
                gridView1.BestFitColumns();
            }
            gridInited = false;

            if (definition.SaveLayout == true) //snimi layout nakon bestfita; svaki sljedeći reload koristiti će layout
            {
                SaveLayoutToVariable();
            }

        }

        private void AddGridColumns()
        {
            if (definitionFields == null)
                return;

            List<JField> fieldList = new List<JField>();

            bool columnsPopulated = false;
            bool? isInherited = null;

            if (definition.Fields != null)
                isInherited = definition.Fields.Inherited;

            //ako je inherited ili ako nije definiran fields element onda punim sve kolone
            if (definitionFields == null || definitionFields.Count == 0 ||
                definition.Fields.Inherited == true)
            {
                if (definitionFields.Count == 0)
                    fieldList = PopulateFieldListFromDb(false);
                else
                    fieldList = PopulateFieldListFromDb(true);

                List<JField> fieldListCopy = fieldList.ToList();

                foreach (JField jField in fieldListCopy)
                {
                    if (definitionFields.Exists(x => x.Name.ToLower() == jField.Name.ToLower()))
                    {
                        var i = fieldList.FindIndex(x => x.Name.ToLower() == jField.Name.ToLower());
                        fieldList[i] = definitionFields.Where(x => x.Name.ToLower() == jField.Name.ToLower()).FirstOrDefault();
                    }
                }

                JField test = definitionFields.FirstOrDefault(x => x.Name.ToLower() == "carporownumbercolumn");
                if (test != null)
                    fieldList.Add(test);

                fieldListCopy = null;
                columnsPopulated = true;
            }

            if (columnsPopulated == false)
                fieldList = definitionFields;

            //fieldList.RemoveAll(x => x.Visible == false);

            if (fieldList == null)
                return;

            CustomSortList.Clear();
            if (!string.IsNullOrEmpty(definition.CustomSort))
            {
                if (!isCustomSortInitialized)
                    InitializeCustomSortEvent();

                CustomSortList.Add("GridControl", definition.CustomSort);
            }

            //za svaki slučaj uklanjam kolone
            gridView1.Columns.Clear();
            foreach (JField col in fieldList)
            {
                var result = Logic.Data.Core.FindDataColumn(col.Name, DataColumns);
                if (result != null || col.Name == "CarpoRowNumberColumn")
                {
                    string fieldName = col.Name == "CarpoRowNumberColumn" ? "CarpoRowNumberColumn" : result.ToString();
                    bool setRnCaption = fieldName == "CarpoRowNumberColumn" && string.IsNullOrEmpty(col.Caption) ? true : false;

                    if (!string.IsNullOrEmpty(previewField) && previewField.ToLower() == fieldName.ToLower())
                        continue;

                    GridColumn column = new GridColumn();

                    column.FieldName = fieldName;

                    if (string.IsNullOrEmpty(col.Caption) && definition.NetGenFields == true)
                        column.Caption = Common.Helpers.Grid.NetGenField(col.Name, definition.FieldSmartSpaces);
                    else if (!string.IsNullOrEmpty(col.Caption))
                    {
                        using (Parser parser = new Parser(form, null))
                        {
                            string valueStr = col.Caption;
                            object value = parser.GetLogicPartValue(valueStr);

                            if (value != null)
                                column.Caption = value.ToString().Replace("\\n", "\r\n");
                            else
                                column.Caption = col.Caption.Replace("\\n", "\r\n");
                        }
                    }
                    else if (definition.FieldSmartSpaces == false)
                        column.Caption = fieldName;
                    else
                        column.Caption = Common.Helpers.Strings.AddSpacesToSentence(fieldName);

                    if (!string.IsNullOrEmpty(col.AutoFilterCondition) && col.AutoFilterCondition == "contains")
                        column.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Contains;
                    else
                        column.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Like;

                    if (col.AllowSort == false)
                        column.OptionsColumn.AllowSort = DefaultBoolean.False;
                    else
                        column.OptionsColumn.AllowSort = DefaultBoolean.True;

                    if (col.AllowAutoFilter == false)
                        column.OptionsFilter.AllowAutoFilter = false;
                    else
                        column.OptionsFilter.AllowAutoFilter = true;

                    if (col.MinWidth != null)
                        column.MinWidth = (int)col.MinWidth;

                    if (col.MaxWidth != null)
                        column.MaxWidth = (int)col.MaxWidth;

                    if (col.ClearValueOnPrint == true)
                    {
                        clearColumnList.Add(column.FieldName);
                        InitCustomColumnDisplayTextEvent = true;
                    }

                    if (!string.IsNullOrEmpty(col.RowAutoHeight) && col.RowAutoHeight.ToLower() == "true")
                    {
                        gridView1.OptionsView.RowAutoHeight = true;

                        column.ColumnEdit = new RepositoryItemMemoEdit();
                        (column.ColumnEdit as RepositoryItemMemoEdit).WordWrap = true;
                        (column.ColumnEdit as RepositoryItemMemoEdit).LinesCount = 0;
                    }

                    CheckColumnCustomSort(col, column, fieldName);

                    gridView1.Columns.Add(column);
                    column.Visible = true;

                    if (fieldName == "CarpoRowNumberColumn")
                    {
                        var gc = gridView1.Columns["CarpoRowNumberColumn"];
                        gc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gc.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
                        if (setRnCaption)
                            gc.Caption = " ";
                        gc.VisibleIndex = 0;
                        gc.OptionsFilter.AllowAutoFilter = false;
                    }

                    if (!string.IsNullOrEmpty(col.Index))
                    {
                        int index;
                        bool isNumeric = int.TryParse(col.Index, out index);

                        if (isNumeric == true)
                            AddToSortList(column.FieldName, index);
                        else
                        {
                            form.Logger.Warn("Neispravno definiran index kolone", "Index kolone '" + column.FieldName + "' grida '" + gridControl.Name + "' nije ispravno definiran!");
                            AddToSortList(column.FieldName, null);
                        }
                    }
                    else
                    {
                        AddToSortList(column.FieldName, null);
                    }

                    if (col.FontBold == true)
                        column.AppearanceCell.Font = new Font(column.AppearanceCell.Font, FontStyle.Bold);
                    if (!string.IsNullOrEmpty(col.FontColor))
                    {
                        Color? colColor = Logic.Style.GetColor(col.FontColor);
                        if (colColor != null)
                            column.AppearanceCell.ForeColor = colColor.Value;
                    }
                    if (!string.IsNullOrEmpty(col.BackColor))
                    {
                        Color? backColor = Logic.Style.GetColor(col.BackColor);
                        if (backColor != null)
                            column.AppearanceCell.BackColor = backColor.Value;
                    }
                    if (!string.IsNullOrEmpty(col.Fixed))
                    {
                        if (col.Fixed == "right")
                            column.Fixed = FixedStyle.Right;
                        else
                            column.Fixed = FixedStyle.Left;
                        // defaultno disableam grupiranje fixiranih . Isto tako je logično da se ne mogu grupirati kao ni mijenjati visible index
                        // na zahtjev je dodan Field atribut allowgroup koji će omogućiti grupiranje fixiranih koloni
                        // ali neće ih znati opet fixirati kada se razgrupira osim ako se ne reseta ili re-inita grid
                        if (!string.IsNullOrEmpty(col.AllowGroup) && col.AllowGroup.ToLower() == "true")
                            column.OptionsColumn.AllowGroup = DefaultBoolean.True;
                        else
                            column.OptionsColumn.AllowGroup = DefaultBoolean.False;
                    }

                    if (col.GroupIndex != null && col.GroupIndex >= 0)
                        column.GroupIndex = Convert.ToInt32(col.GroupIndex);

                    if (col.FormatString != null)
                    {
                        if (col.FormatString.Length <= 2 && col.FormatString.Substring(1, 1).ToLower() == "n")
                        {
                            column.DisplayFormat.FormatString = col.FormatString;
                        }
                    }
                    if (!string.IsNullOrEmpty(col.Tooltip))
                    {
                        column.ToolTip = col.Tooltip;
                    }

                    if (!string.IsNullOrEmpty(col.Align))
                    {
                        if (col.Align.ToLower() == "left")
                            column.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
                        else if (col.Align.ToLower() == "right")
                            column.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
                        else if (col.Align.ToLower() == "center")
                            column.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                    }

                    if (col.FixWidth != null)
                    {
                        int fixWidth;
                        if (Carpo.Common.Helpers.Conversion.TryCast(col.FixWidth, out fixWidth) == false)
                        {
                            form.Logger.Warn("Neispravna vrijednost atributa grid kolone", "Kolona: '" + col.Name + "'; Očekivan tip atributa 'FixWidth' je integer");
                        }
                        else
                        {
                            column.Width = (int)col.FixWidth;
                            column.OptionsColumn.FixedWidth = true;
                        }
                    }
                    column.Tag = col;
                }
                else
                {
                    form.Logger.Warn("Neispravan naziv kolone", "Kolona '" + col.Name + "' grid kontrole '" + gridControl.Name + "' nije pronađena");
                }
            }
            SetLayout(fieldList, isInherited);
        }

        private void AddToSortList(string fieldName, int? index)
        {
            if (!sortList.ContainsKey(fieldName))
                sortList.Add(fieldName, index);
        }

        private void SetLayout(List<JField> fieldLista, bool? isInherited)
        {
            #region sortZakomentiranoDoDorade

            //Dictionary<string, int?> fieldsWithIndex = new Dictionary<string, int?>();
            //Dictionary<string, int?> fieldsWithoutIndex = new Dictionary<string, int?>();

            //// listu polja [fieldLista] 'razbijam' na dva djela (jedna lista sadrži polja sa indexom, a druga sadrži polja bez indexa)
            //foreach (KeyValuePair<string, int?> par in sortList)
            //{
            //    if (par.Value != null)
            //        fieldsWithIndex.Add(par.Key, par.Value);
            //    else
            //        fieldsWithoutIndex.Add(par.Key, null);
            //}

            //// u sortiranje liste krećem tek ako imamo barem jedno polje koje sadrži definiran index
            //if (!fieldsWithIndex.IsEmpty())
            //{
            //    for (int i = 0; i < gridView1.Columns.Count; i++)
            //    {
            //        bool dodao = false;
            //        if (!fieldsWithIndex.IsEmpty())
            //        {
            //            foreach (KeyValuePair<string, int?> par in fieldsWithIndex) // prednost imaju polja sa definiranim indexom te prvo njih uspoređujem
            //            {
            //                if (par.Value == i) // ako je index polja jednak indexu kolone, postavljam mu visible index
            //                {
            //                    gridView1.Columns.ColumnByFieldName(par.Key).VisibleIndex = i;
            //                    fieldsWithIndex.Remove(par.Key);
            //                    dodao = true;
            //                    break;
            //                }
            //            }
            //        }

            //        if (dodao == true)
            //            continue;

            //        if (!fieldsWithoutIndex.IsEmpty()) // ako ne postoji polje sa indexom kolone koju trenutno pretražujemo, onda u nju spremamo prvi zapis bez definiranog indexa
            //        {
            //            gridView1.Columns.ColumnByFieldName(fieldsWithoutIndex.First().Key).VisibleIndex = i;
            //            fieldsWithoutIndex.Remove(fieldsWithoutIndex.FirstOrDefault().Key);
            //            continue;
            //        }
            //        else // za slučajeve gdje su definirani indexi veći od broja kolona
            //        {
            //            var val = fieldsWithIndex.OrderBy(x => x.Value).FirstOrDefault();
            //            gridView1.Columns.ColumnByFieldName(val.Key).VisibleIndex = i;
            //            fieldsWithIndex.Remove(val.Key);
            //            continue;
            //        }
            //    }
            //}

            #endregion sortZakomentiranoDoDorade
            DateTime startTime = DateTime.Now;

            foreach (JField col in fieldLista) // tek kada smo sortirali polja krećemo u provjeru 'visible' i 'hidden' atributa
            {
                var result = Logic.Data.Core.FindDataColumn(col.Name, DataColumns);
                if (result != null || col.Name == "CarpoRowNumberColumn")
                {
                    string colName = col.Name == "CarpoRowNumberColumn" ? "CarpoRowNumberColumn" : result.ToString();
                    if (isInherited == true)
                    {
                        if (col.Name.ToLower().EndsWith("id") && col.Visible != true) // Sve kolone koje završavaju na "Id" se sakrivaju, ako im nije definirano da su vidljive
                        {
                            gridView1.Columns.ColumnByFieldName(colName).Visible = false;
                            if (col.Hidden != true)
                                gridView1.Columns.ColumnByFieldName(colName).OptionsColumn.ShowInCustomizationForm = false;
                        }
                    }

                    if (col.Visible == false || col.Hidden == true) // ako kolona sadrži atribut visible ili hidden nju sakrivamo, a u slučaju visible-a i mičemo u potpunosti
                    {
                        gridView1.Columns.ColumnByFieldName(colName).Visible = false;
                        if (col.Visible == false)
                            gridView1.Columns.ColumnByFieldName(colName).OptionsColumn.ShowInCustomizationForm = false;
                    }
                }
                else
                {
                    form.Logger.Warn("Neispravan naziv kolone", "Kolona '" + col.Name + "' grid kontrole '" + gridControl.Name + "' nije pronađena");
                }

            }
            // form.Logger.InfoData("Set layout", definition.Name, startTime);
        }

        private void FormatGridColumnsEditable() //format kolona bez JFields
        {
            foreach (GridColumn column in gridView1.Columns)
            {
                FormatColumnEditable(column);
            }
        }

        private void FormatColumnEditable(GridColumn column)
        {
            var jField = column.Tag as JField;

            string formatString = "";

            bool sumField = false;

            if (jField != null)
            {
                var editableField = jField.Editable;

                // != true && !=false jer se radi o uvjetu, želim ići u parsiranje
                if (!string.IsNullOrEmpty(editableField) && editableField.ToLower() != "true" && editableField.ToLower() != "false")
                {
                    Logic.Statemants statemants = new Logic.Statemants(jField.Editable, this.form, null);
                    editableField = statemants.CheckIf().ToString();
                }

                if (!string.IsNullOrEmpty(editableField) && editableField.ToLower() == "true")
                {
                    //column.AppearanceCell.BackColor = Color.LightYellow;

                    if (!string.IsNullOrEmpty(jField.EditType))
                        CreateEditableColumn(column, jField, jField.EditType);
                    else
                        CreateEditableColumn(column, jField);
                }
                else
                    column.OptionsColumn.AllowEdit = false;

                // nisu namijenjene biti editabilne već da samo imaju mogućnost klika
                if (!string.IsNullOrEmpty(jField.EditType) && (jField.EditType.ToLower() == "url" || jField.EditType.ToLower() == "file"))
                {
                    CreateEditableColumn(column, jField, jField.EditType);
                    column.OptionsColumn.AllowEdit = true;
                }
            }
            else
                column.OptionsColumn.AllowEdit = false;
        }

        #endregion Columns

        #region FormatColumns

        private void FormatGridColumns() //format kolona bez JFields
        {
            foreach (GridColumn column in gridView1.Columns)
            {
                FormatColumn(column);
            }
        }
        //private Color ColoriseColumn()
        //{
        //    var skin = CommonSkins.GetSkin(UserLookAndFeel.Default);
        //    if (Skin.DefaultSkinPaletteName == null) return Color.LightYellow;
        //    return skin.SvgPalettes[Skin.DefaultSkinPaletteName].Colors.Where(x => x.Name == "Yellow").First().Value;

        //}
        private void FormatColumn(GridColumn column)
        {
            var jField = column.Tag as JField;

            string formatString = "";

            bool sumField = false;

            switch (Type.GetTypeCode(column.ColumnType))
            {
                case TypeCode.Int32:
                case TypeCode.Int64:
                    SetCaptionAlign(column);
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    formatString = "f0";
                    sumField = true;
                    break;

                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    //default format
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    formatString = "n2";
                    SetCaptionAlign(column);
                    column.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    sumField = true;
                    break;

                case TypeCode.DateTime:
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    formatString = "dd.MM.yyyy";
                    break;
            }

            if (jField != null)
            {
                if (!string.IsNullOrEmpty(jField.FormatString))
                    formatString = jField.FormatString;
            }

            if (sumField == true)
                CreateSum(column, jField, formatString);

            //ako nije definiran unutar xml-a, onda sakri ako završava sa id
            //if (jField != null && column.FieldName.ToLower().EndsWith("id"))
            //    column.Visible = false;

            if (!string.IsNullOrEmpty(formatString))
            {
                var formatStr = ValueParserToString(formatString);
                if (!string.IsNullOrEmpty(formatStr))
                    column.DisplayFormat.FormatString = formatStr;
            }

            if (jField != null)
            {
                var editableField = jField.Editable;

                // != true && !=false jer se radi o uvjetu, želim ići u parsiranje
                if (!string.IsNullOrEmpty(editableField) && editableField.ToLower() != "true" && editableField.ToLower() != "false")
                {
                    Logic.Statemants statemants = new Logic.Statemants(jField.Editable, this.form, null);
                    editableField = statemants.CheckIf().ToString();
                }

                if (!string.IsNullOrEmpty(editableField) && editableField.ToLower() == "true")
                {
                    column.AppearanceCell.BackColor = Color.Empty;

                    if (!string.IsNullOrEmpty(jField.EditType))
                        CreateEditableColumn(column, jField, jField.EditType);
                    else
                        CreateEditableColumn(column, jField);
                }
                else
                    column.OptionsColumn.AllowEdit = false;

                // nisu namijenjene biti editabilne već da samo imaju mogućnost klika
                if (!string.IsNullOrEmpty(jField.EditType) && (jField.EditType.ToLower() == "url" || jField.EditType.ToLower() == "file"))
                {
                    CreateEditableColumn(column, jField, jField.EditType);
                    column.OptionsColumn.AllowEdit = true;
                }
            }
            else
                column.OptionsColumn.AllowEdit = false;
        }

        private void SetCaptionAlign(GridColumn column)
        {
            try
            {
                // ako je postavljen captionalign atribut na field, forsiraj ga
                if (definition.Fields != null && definition.Fields.FieldList != null)
                {
                    JField field = definition.Fields.FieldList.Where(x => x.Name.ToLower() == column.FieldName.ToLower()).FirstOrDefault();
                    if (field != null)
                    {
                        if (!string.IsNullOrEmpty(field.CaptionAlign))
                        {
                            var col = definition.Fields.FieldList.Where(x => x.Name == column.FieldName).FirstOrDefault();
                            if (col.CaptionAlign.ToLower() == "left")
                            { column.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Near; return; }
                            else if (col.CaptionAlign.ToLower() == "right")
                            { column.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Far; return; }
                            else if (col.CaptionAlign.ToLower() == "center")
                            { column.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center; return; }
                        }
                    }
                }

                column.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
            catch (Exception ex)
            {
                this.form.Logger.Warn("ComponentGrid - SetCaptionAlign(), column: " + column.FieldName, ex.Message);
            }
        }

        private void SetGridCaption()
        {
            if (!string.IsNullOrEmpty(definition.Caption))
            {
                string gridCaption = Common.Translator.TranslateString(ValueParserToString(definition.Caption));
                gridView1.ViewCaption = gridCaption;
                gridView1.OptionsView.ShowViewCaption = true;
            }
            else
                gridView1.OptionsView.ShowViewCaption = false;
        }

        private void CreateSum(GridColumn column, JField jField, string formatString)
        {
            if (jField == null)
                return;

            if (definition.ShowGroupSum != false && jField.ShowGroupSum != false)
            {
                CreateGroupSummary(column, jField, formatString);
            }

            if (definition.ShowTotalSum != false && jField.ShowTotalSum != false)
            {
                column.SummaryItem.SummaryType = Common.Helpers.Grid.GetSummaryType(jField.TotalSumType);
                if (!string.IsNullOrEmpty(formatString))
                    column.SummaryItem.DisplayFormat = "{0:" + formatString + "}";
            }
        }

        private void CreateGroupSummary(GridColumn column, JField jField, string formatString)
        {
            GridGroupSummaryItem sumItem = new GridGroupSummaryItem();
            sumItem.FieldName = column.FieldName;
            sumItem.SummaryType = Common.Helpers.Grid.GetSummaryType(jField.GroupSumType);
            sumItem.DisplayFormat = "{0:" + formatString + "}";
            sumItem.ShowInGroupColumnFooter = column;
            gridView1.GroupSummary.Add(sumItem);
        }

        private void CreateEditableColumn(GridColumn column, JField jField)
        {
            gridView1.OptionsView.RowAutoHeight = true;
            column.Image = Properties.Resources.edit_16x16;




            if (column.ColumnType == typeof(System.String) && jField.CellStayOnEnter == true)
            {
                RepositoryItemMemoEdit repTxt = new RepositoryItemMemoEdit();
                gridControl.RepositoryItems.Add(repTxt);
                if (!string.IsNullOrEmpty(column.DisplayFormat.FormatString))
                {

                    repTxt.Mask.EditMask = column.DisplayFormat.FormatString;
                    repTxt.Mask.UseMaskAsDisplayFormat = true;
                }
                column.ColumnEdit = repTxt;
                column.OptionsColumn.AllowEdit = true;
            }
            else
            {
                RepositoryItemTextEdit repTxt = new RepositoryItemTextEdit();
                gridControl.RepositoryItems.Add(repTxt);
                if (!string.IsNullOrEmpty(column.DisplayFormat.FormatString))
                {

                    repTxt.Mask.EditMask = column.DisplayFormat.FormatString;
                    repTxt.Mask.UseMaskAsDisplayFormat = true;
                }
                column.ColumnEdit = repTxt;
                column.OptionsColumn.AllowEdit = true;
            }





            if (column.ColumnType == typeof(int) || column.ColumnType == typeof(double) || column.ColumnType == typeof(decimal) || column.ColumnType == typeof(float))
            {
                RepositoryItemTextEdit repTxt = new RepositoryItemTextEdit();
                gridControl.RepositoryItems.Add(repTxt);
                if (column.DisplayFormat.FormatType == DevExpress.Utils.FormatType.Numeric)
                {
                    repTxt.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                }
                column.ColumnEdit = repTxt;
                column.OptionsColumn.AllowEdit = true;


            }






            //RepositoryItemTextEdit
            //RepositoryItemMemoEdit repTxt = new RepositoryItemMemoEdit();

            //gridControl.RepositoryItems.Add(repTxt);
            //if (!string.IsNullOrEmpty(column.DisplayFormat.FormatString))
            //{
            //    repTxt.Mask.EditMask = column.DisplayFormat.FormatString;
            //    repTxt.Mask.UseMaskAsDisplayFormat = true;
            //}
            //if (column.DisplayFormat.FormatType == DevExpress.Utils.FormatType.Numeric)
            //    repTxt.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            //column.ColumnEdit = repTxt;
            //column.OptionsColumn.AllowEdit = true;

            ////repTxt.KeyDown += (s, e) =>
            ////{
            ////    if (e.KeyCode == Keys.Enter)
            ////        gridView1.PostEditor();
            ////    gridView1.ShowEditor();
            ////};
        }

        private void CreateEditableColumn(GridColumn column, JField jField, string editType)
        {
            editType = editType.ToLower();
            Structure.Models.NameType nameType = new Structure.Models.NameType();

            RepositoryItem ri = null;
            if (editType == "email" || editType == "url" || editType == "file")
            {
                ri = new RepositoryItemHyperLinkEdit();
                editHyperlinkColumns.Add(new Structure.Models.NameType() { Name = column.FieldName, Type = editType });
            }
            else if (editType == "bit" || editType.StartsWith("bool"))
            {
                ri = new RepositoryItemCheckEdit();
                editCheckColumns.Add(new Structure.Models.NameType() { Name = column.FieldName, Type = editType });
                column.OptionsFilter.AllowAutoFilter = false;
                ri.EditValueChanged += (s, e) =>
                {
                    //okidanje odma nakon promjene
                    gridView1.PostEditor();
                };
            }
            else if (editType == "date")
            {
                ri = new RepositoryItemDateEdit();
                editDateColumns.Add(new Structure.Models.NameType() { Name = column.FieldName, Type = editType });
                column.OptionsFilter.AllowAutoFilter = false;

                /****širina kolone se proširuje zbog dropdown buttona koji se pojavljuje kada je ćelija fokusirana*/
                column.BestFit();
                column.OptionsColumn.FixedWidth = true;
                if (column.Width < 82)
                    column.Width = 82;
                /****/

                ri.Leave += (s, e) =>
                {
                    //okidanje nakon napuštanja ćelije - specifično za Date type
                    gridView1.PostEditor();
                };
            }

            else if (editType == "combo")
            {
                var combo = new RepositoryItemComboBox();
                combo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                var c = new Logic.Data.Core()
                {
                    ConnectionString = form.ConnectionString,
                    ParameterControlValue = "",
                    CurrentForm = form
                };

                var t = c.GetDataByName(jField.DataName);

                //bool stop = false;
                DataSet dataSet = c.GetDataSet(t, "kombo", out bool stop);

                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    combo.Items.Add(item[jField.ValueField]);
                }

                ri = combo;
            }

            if (ri != null)
            {
                column.ColumnEdit = ri;
                //column.OptionsColumn.AllowEdit = false;
            }
        }

        #endregion FormatColumns

        #region Opcije

        private void SaveColumnsModelBeforeGroup()
        {
            try
            {
                ColumnOrderModel = new Common.Helpers.ColumnOrderModel();   // novi order model

                //  postavi property model za svaku kolonu i dodaj ih sve u order model
                foreach (GridColumn col in gridView.Columns)
                {
                    this.ColumnPropertiesModel = new Common.Helpers.ColumnPropertiesModel();
                    this.ColumnPropertiesModel.Column = col;
                    this.ColumnPropertiesModel.ColumnFixedSide = col.Fixed;
                    this.ColumnPropertiesModel.ColumnVisibleIndex = col.VisibleIndex;
                    this.ColumnPropertiesModel.ColumnMinWidth = col.MinWidth;
                    this.ColumnPropertiesModel.ColumnMaxWidth = col.MaxWidth;

                    ColumnOrderModel.columnProperties.Add(this.ColumnPropertiesModel);
                }
            }
            catch (Exception ex)
            {
                form.Logger.Warn("SaveColumnsModelBeforeGroup()", ex.Message);
            }
        }

        private void LoadSavedColumnsModel()
        {
            try
            {
                // sortiraj kolone kao što su bile prije grupiranja
                ColumnOrderModel.columnProperties.Sort((emp1, emp2) => emp1.ColumnVisibleIndex.CompareTo(emp2.ColumnVisibleIndex));

                foreach (Common.Helpers.ColumnPropertiesModel col in ColumnOrderModel.columnProperties)
                {
                    //var gridCol = gridView1.Columns[col.Column.FieldName];
                    var gridCol = gridView1.Columns.Where(x => x.FieldName == col.Column.FieldName).FirstOrDefault();
                    gridCol.VisibleIndex = col.ColumnVisibleIndex;
                    gridCol.Fixed = col.ColumnFixedSide;
                    gridCol.MinWidth = col.ColumnMinWidth;
                    gridCol.MaxWidth = col.ColumnMaxWidth;
                }
            }
            catch (Exception ex)
            {
                form.Logger.Warn("LoadSavedColumnsModel()", ex.Message);
            }
        }

        public void HandleFixedColumns()
        {
            try
            {
                if (this.gridView1.GroupCount > 0)
                {
                    foreach (GridColumn fixedCol in gridView1.Columns.Where(x => x.Fixed != FixedStyle.None))
                    { fixedCol.Fixed = FixedStyle.None; }
                }
            }
            catch (Exception ex)
            {
                form.Logger.Warn("HandleFixedColumns()", ex.Message);
            }
        }

        private void ProcessColumnWidth(GridColumn col, bool grouping = true)
        {
            try
            {
                // koristim Tag za pamćenje širine kolone
                if (grouping)   // kada grupiram želim zapamtiti širine koloni
                {
                    (col.Tag as JField).ColWidthBeforeGrouping = col.Width; // pamtim širinu kolone u Tagu

                    if (col.GroupIndex >= 0)    // ako je to kolona po kojoj grupiramo - dodajem 100 widtha da prilagodim inlineu
                    {
                        col.MinWidth = 20;
                        col.MaxWidth = 0;
                        col.BestFit();
                        col.Width += 100;
                    }
                    else
                        col.BestFit();
                }
                else            // ungroup - postavljam širine prije grupiranja nazad
                {
                    col.Width = Convert.ToInt32((col.Tag as JField).ColWidthBeforeGrouping);
                    (col.Tag as JField).ColWidthBeforeGrouping = null;
                }
            }
            catch (Exception ex)
            {
                form.Logger.Warn("ProcessColumnWidth()", ex.Message);
            }
        }

        private void SetInlineBestFit(GridColumn groupedCol)
        {
            try
            {
                // col.Tag je napunjen preko definicije XML-a ponekad, bilo je potrebno proširiti uvjetovanje prvim dijelom
                if ((groupedCol.Tag is JField))
                {
                    if ((groupedCol.Tag as JField).ColWidthBeforeGrouping == null)
                        ProcessColumnWidth(groupedCol);
                    else
                        return;
                }
                else if (Convert.ToBoolean(groupedCol.Tag) != true)
                    ProcessColumnWidth(groupedCol);
            }
            catch (Exception ex)
            {
                form.Logger.Warn("SetInlineBestFit()", ex.Message);
            }
        }

        private void BestFitInlineColumns()
        {
            try
            {
                foreach (GridColumn groupedCol in this.gridView1.GroupedColumns)
                    SetInlineBestFit(groupedCol);
                foreach (GridColumn gridCol in this.gridView1.Columns.Where(x => x.GroupIndex < 0))
                    ProcessColumnWidth(gridCol);
            }
            catch (Exception ex)
            {
                form.Logger.Warn("BestFitInlineColumns()", ex.Message);
            }
        }

        public void SetProperty(JGridControl.Property property, bool value)
        {
            gridView1.BeginUpdate();
            try
            {
                switch (property)
                {
                    case JGridControl.Property.ShowFooter:
                        gridView1.OptionsView.ShowFooter = value;
                        break;

                    case JGridControl.Property.SumsUnder:
                        {
                            gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.False;
                            this.gridView1.OptionsView.GroupFooterShowMode = value == true ? GroupFooterShowMode.VisibleAlways : GroupFooterShowMode.VisibleIfExpanded;
                            break;
                        }
                    case JGridControl.Property.SumsIfExpanded:
                        {
                            gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.False;
                            this.gridView1.OptionsView.GroupFooterShowMode = value == true ? GroupFooterShowMode.VisibleIfExpanded : this.gridView1.OptionsView.GroupFooterShowMode;
                            break;
                        }
                    case JGridControl.Property.SumsInline:
                        if (value == true && gridInited)
                        {
                            // isključi VisibleIfExpanded
                            // spremi redoslijed koloni i njihov fixed value, makni im fixed jer ne radi s AlignGroupSummaryInGroupRow
                            this.gridView1.OptionsView.GroupFooterShowMode = GroupFooterShowMode.Hidden;

                            //// ako je inline - save layout u model da znam dobiti taj layout kad razgrupiram (init je grupirano)
                            gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow = value == true ? DefaultBoolean.True : DefaultBoolean.False;

                            if (value)
                            {
                                SaveColumnsModelBeforeGroup();
                                if (gridView1.GroupCount > 0)
                                {
                                    HandleFixedColumns();
                                    BestFitInlineColumns();
                                    groupedColsCounter = gridView1.GroupCount;
                                }
                            }
                        }
                        else
                        {
                            // nakon isključivanja inlinea postavi kolone i fixiraj ih prema modelu napravljenom ranije
                            gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow = value == true ? DefaultBoolean.True : DefaultBoolean.False;
                            this.gridView1.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleIfExpanded;
                        }
                        break;
                }
            }
            finally
            {
                gridView1.EndUpdate();
            }
        }

        #endregion Opcije

        #region Expand/Collapse

        private void ExpandCollapse()
        {
            if (IsCollapsed == true)
                gridView1.ExpandAllGroups();
            else
                gridView1.CollapseAllGroups();

            IsCollapsed = !IsCollapsed;
        }

        #endregion Expand/Collapse

        #region NotificationSpecific

        public void SetNotifyForEachRow(JEventAction action, bool selectedOnly)
        {
            if (action.Actions != null && action.Actions.Count > 0)
            {
                // B6: full-table foreachrow is unsupported under VirtualServerMode (no local DataTable).
                if (definition?.VirtualServerMode == true)
                {
                    form.Logger.Warn("B6 VirtualServerMode",
                        "foreachrow unsupported under virtualservermode (loaded/selected rows only). Prefer #grid.selectedids batch.");
                    Common.Helpers.JMessages.Info(
                        "B6 VirtualServerMode: foreachrow nije podržan na cijelom setu.\n" +
                        "Koristi #grid.selectedids (samo učitani/označeni redovi) ili paging SP batch.");
                    return;
                }

                // A9 guidance: nested datarun inside foreachrow = classic N+1. Prefer #grid.selectedids → one SP.
                if (action.Actions.Any(a => a.Type != null && a.Type.IndexOf("datarun", StringComparison.OrdinalIgnoreCase) >= 0))
                    form.Logger.Warn("A9 foreachrow", "foreachrow+datarun (N+1 risk). Prefer #grid.selectedids or tablepar batch.");

                try
                {
                    if (selectedOnly == true)
                    {
                        int[] selectedRows;
                        if (definition.SelectionWebStyle == true)
                        {
                            List<int> selectedRowsList = new List<int>();

                            for (int i = 0; i < gridView1.DataRowCount; i++)
                            {
                                if (GridCheckMarksSelection.IsRowSelected(i))
                                    selectedRowsList.Add(i);
                            }
                            selectedRows = selectedRowsList.ToArray();
                        }
                        else
                            selectedRows = GetSelectedRows();

                        foreach (int rowHandle in selectedRows)
                        {
                            gridControl.DataRowRef = gridView1.GetDataRow(rowHandle);
                            RowForEachRowAction(action);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < gridView1.DataRowCount; i++)
                        {
                            gridControl.DataRowRef = gridView1.GetDataRow(i);
                            RowForEachRowAction(action);
                        }
                    }
                }
                finally
                {
                    gridControl.DataRowRef = null; //privremena referenca
                }
            }
        }

        public void Print(JEventAction action, bool selected)
        {
            if (action.Actions != null && action.Actions.Count > 0)
            {
                try
                {
                    if (selected == true)
                    {
                        List<Structure.Models.Print> printList = new List<Structure.Models.Print>();
                        //List<string> printParametersList = new List<string>();
                        int[] selectedRows = GetSelectedRows();
                        foreach (int rowHandle in selectedRows)
                        {
                            gridControl.DataRowRef = gridView1.GetDataRow(rowHandle);
                            Structure.Models.Print printDefinition = RowPrintAction(action);
                            printList.Add(printDefinition);

                            if (action.SinglePrint == "true")
                                break;
                        }

                        Logic.PrintDialog printDialog = new Logic.PrintDialog(form, printList);
                    }
                }
                finally
                {
                    gridControl.DataRowRef = null; //privremena referenca
                }
            }
        }

        #endregion NotificationSpecific

        public void PrintControl(bool selected)
        {
            try
            {
                // B6 Iter 4: VSM print hydrates full filtered set (or selected rows) into a temp clone.
                if (IsVirtualServerModeActive)
                {
                    if (!TryPrepareVsmHydratePrintClone(selected))
                        return;
                    selected = false; // clone already contains the print set
                }

                var currentLayoutStream = new MemoryStream();
                gridView1.SaveLayoutToStream(currentLayoutStream); // ovo sejvamo da možemo vratit na njega !

                gridView1.OptionsPrint.PrintSelectedRowsOnly = selected;

                // ne može inline if zbog bool?
                if (definition.ShowFooter == true)
                    gridView1.OptionsPrint.PrintFooter = true;
                else
                    gridView1.OptionsPrint.PrintFooter = false;

                //start I.L. 17.03.2024. - radi omogućavanje printa headera grida u više redova
                GridViewInfo vi = gridView1.GetViewInfo() as GridViewInfo;
                var headerHeight = vi.ColumnRowHeight;
                gridView1.ColumnPanelRowHeight = headerHeight;
                //start 

                var dtCombo = DataSetForPrinting("printDialogComboTemplates", GetTemplatesModel()).Tables[0];
                var dsForPrinting = DataSetForPrinting("getDataSetForPrinting", GetTemplatesModel());

                bool isLandscape = false;
                var linkParameters = SetPrintHeaderParameters(ref isLandscape);

                if (InitCustomColumnDisplayTextEvent)
                    gridView1.CustomColumnDisplayText += gridView1_CustomColumnDisplayText;

                using (var clsPrint = new Spin.Printing.Start())
                {
                    clsPrint.PrintLink(form.ConnectionString, Core.PubVars.OperateriId, GetPrintableControl(), linkParameters, dtCombo, dsForPrinting, currentLayoutStream, isLandscape); ;
                    PrintClonedGridControl?.Dispose();
                    PrintClonedGridControl = null;
                }

                if (InitCustomColumnDisplayTextEvent)
                    gridView1.CustomColumnDisplayText -= gridView1_CustomColumnDisplayText;

                currentLayoutStream.Seek(0, SeekOrigin.Begin);
                gridControl.ComponentExt.gridView1.RestoreLayoutFromStream(currentLayoutStream);
            }
            catch (Exception ex)
            {
                form.Logger.Error("method: PrintDialog", ex.Message, ex);
            }
        }

        private GridControl GetPrintableControl()
        {
            if (PrintClonedGridControl != null)
                return PrintClonedGridControl;

            if (gridView1.OptionsPrint.PrintSelectedRowsOnly)
            {
                return GetGridWithSelection(gridControl);
            }
            return gridControl;
        }

        /// <summary>
        /// B6: hydrate VSM rows into PrintClonedGridControl for print/export (full filtered set or selection).
        /// </summary>
        private bool TryPrepareVsmHydratePrintClone(bool selectedOnly)
        {
            PrintClonedGridControl?.Dispose();
            PrintClonedGridControl = null;

            if (vsmBinder == null)
            {
                Common.Helpers.JMessages.Info("B6 VirtualServerMode: binder nije aktivan.");
                return false;
            }

            if (selectedOnly)
            {
                DataTable selectedTable = BuildDataTableFromGridSelection(gridView1);
                if (selectedTable.Rows.Count == 0)
                {
                    Common.Helpers.JMessages.Info("Nema označenih redova za print/export.");
                    return false;
                }

                PrintClonedGridControl = CreatePrintCloneWithTable(selectedTable);
                form.Logger.Info("B6 VirtualServerMode",
                    "Print/export hydrate selected rows=" + selectedTable.Rows.Count);
                return true;
            }

            int n = vsmBinder.TotalCount;
            string msg = n > 0
                ? "Za print/export cijelog seta učitat će se " + n + " redova (trenutni filter/sort).\nNastaviti?"
                : "Za print/export cijelog seta učitat će se svi redovi po trenutnom filteru/sortu.\nNastaviti?";

            if (XtraMessageBox.Show(msg, "B6 VirtualServerMode", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return false;

            bool waitShown = false;
            try
            {
                try
                {
                    form.splashScreenManager1.ShowWaitForm();
                    form.splashScreenManager1.SetWaitFormCaption("B6 VirtualServerMode");
                    form.splashScreenManager1.SetWaitFormDescription(
                        n > 0 ? ("Učitavam " + n + " redova…") : "Učitavam redove…");
                    waitShown = true;
                }
                catch { /* splash optional */ }

                bool truncated;
                DataTable table = vsmBinder.FetchAllFiltered(out truncated);
                if (table == null || table.Rows.Count == 0)
                {
                    Common.Helpers.JMessages.Info("Nema redova za print/export.");
                    return false;
                }

                if (truncated)
                {
                    Common.Helpers.JMessages.Info(
                        "Učitano je maksimalno " + table.Rows.Count + " redova (cap 100000). Print/export nije potpun.");
                }

                PrintClonedGridControl = CreatePrintCloneWithTable(table);
                form.Logger.Info("B6 VirtualServerMode",
                    "Print/export hydrate-all rows=" + table.Rows.Count + ", truncated=" + truncated);
                return true;
            }
            catch (Exception ex)
            {
                form.Logger.Error("B6 VirtualServerMode Print hydrate", ex.Message, ex);
                Common.Helpers.JMessages.Info("B6: učitavanje za print/export nije uspjelo.\n" + ex.Message);
                return false;
            }
            finally
            {
                if (waitShown)
                {
                    try { form.splashScreenManager1.CloseWaitForm(); }
                    catch { }
                }
            }
        }

        private GridControl CreatePrintCloneWithTable(DataTable table)
        {
            GridControl clone = CloneGrid(gridControl);
            clone.DataSource = table;
            clone.Parent = gridControl.Parent;
            clone.Visible = false;

            GridView view = clone.MainView as GridView;
            if (view != null)
            {
                view.ExpandAllGroups();
                view.OptionsPrint.PrintSelectedRowsOnly = false;
            }

            return clone;
        }

        private static DataTable BuildDataTableFromGridSelection(GridView view)
        {
            var dt = new DataTable();
            if (view == null)
                return dt;

            foreach (GridColumn col in view.Columns)
            {
                if (string.IsNullOrEmpty(col.FieldName) || col.FieldName.StartsWith("DX$", StringComparison.Ordinal))
                    continue;
                if (!dt.Columns.Contains(col.FieldName))
                    dt.Columns.Add(col.FieldName, typeof(object));
            }

            foreach (int rh in view.GetSelectedRows())
            {
                if (rh < 0)
                    continue;

                DataRow row = dt.NewRow();
                foreach (DataColumn dc in dt.Columns)
                {
                    object val = view.GetRowCellValue(rh, dc.ColumnName);
                    row[dc.ColumnName] = val ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }

            dt.AcceptChanges();
            return dt;
        }

        #region KloniranjeGridaRadiPrintanjaSelektiranihRedova
        private DataTable GetTableOfSelectedRows(GridView view)
        {
            DataTable resultTable = new DataTable();
            DataTable sourceTable = null;
            if (view.DataSource is DataView)
                sourceTable = ((DataView)view.DataSource).Table;
            else if (view.DataSource is BindingSource)
            {
                object dv = ((BindingSource)view.DataSource).List;
                sourceTable = ((DataView)dv).Table;
            }
            if (sourceTable != null)
            {
                resultTable = sourceTable.Clone();
                foreach (int rowHandle in view.GetSelectedRows())
                {
                    DataRow row = view.GetDataRow(rowHandle);
                    resultTable.Rows.Add(row.ItemArray);
                }
                resultTable.AcceptChanges();
            }
            return resultTable;
        }

        private GridControl CloneGrid(GridControl sourceGrid)
        {
            GridControl resultGrid = new GridControl();
            resultGrid.MainView = new GridView(resultGrid);
            resultGrid.MainView.Assign(sourceGrid.MainView, false);

            resultGrid.Visible = true;
            return resultGrid;
        }

        private GridControl GetGridWithSelection(GridControl grid)
        {
            PrintClonedGridControl = CloneGrid(grid);
            DataTable clonedTable = GetTableOfSelectedRows(grid.MainView as GridView);
            PrintClonedGridControl.DataSource = clonedTable;

            PrintClonedGridControl.Parent = gridControl.Parent;
            PrintClonedGridControl.Visible = false;

            (PrintClonedGridControl.MainView as GridView).ExpandAllGroups();
            (PrintClonedGridControl.MainView as GridView).OptionsPrint.PrintSelectedRowsOnly = false;

            return PrintClonedGridControl;
        }

        #endregion

        private Dictionary<string, string> SetPrintHeaderParameters(ref bool isLandscape)
        {
            var linkParameters = new Dictionary<string, string>();

            string caption = "", captionAlign = "", header = "", headerAlign = "",
                    subHeader = "", subHeaderAlign = "", headerScale = "", imagePath = "";

            if (definition.Print != null)
            {
                caption = SetPrintHeaderValue(Common.Translator.TranslateString(definition.Print.Caption));
                captionAlign = SetPrintHeaderAlignment(definition.Print.CaptionAlign);
                header = SetPrintHeaderValue(definition.Print.Header);
                headerAlign = SetPrintHeaderAlignment(definition.Print.HeaderAlign);
                subHeader = SetPrintHeaderValue(definition.Print.SubHeader);
                subHeaderAlign = SetPrintHeaderAlignment(definition.Print.SubHeaderAlign);
                headerScale = SetPrintHeaderValue(definition.Print.HeaderScale);
                imagePath = SetPrintHeaderValue(definition.Print.ImagePath);
            }
            else
            {
                caption = SetPrintHeaderValue(Common.Translator.TranslateString(definition.PrintCaption));
                captionAlign = SetPrintHeaderValue(definition.PrintCaptionAlign);
            }

            if (!string.IsNullOrEmpty(definition.PrintOrientation))
            {
                // radim ovo sa selectedIndex jer ako setam value preko interfacea group control postaje disablean. Želim zadržati
                // to ponašanje ali i omogućiti setanje default vrijednosti putem atributa

                isLandscape = definition.PrintOrientation.ToLower() == "portrait" ? false : true;
            }

            linkParameters.Add("caption", caption);
            linkParameters.Add("captionAlign", captionAlign);
            linkParameters.Add("header", header);
            linkParameters.Add("headerAlign", headerAlign);
            linkParameters.Add("subHeader", subHeader);
            linkParameters.Add("subHeaderAlign", subHeaderAlign);
            linkParameters.Add("headerScale", headerScale);
            linkParameters.Add("imagePath", imagePath);

            return linkParameters;
        }

        private void RowForEachRowAction(JEventAction action)
        {
            Form formActive = FormsVar.GetActiveForm(form);
            using (Logic.Events ev = new Logic.Events(control, formActive as Forms.Base.JupiterShared))
            {
                ev.RunActions(action.Actions);
            }
        }

        private Structure.Models.Print RowPrintAction(JEventAction action)
        {
            Form formActive = FormsVar.GetActiveForm(form);
            using (Logic.Events ev = new Logic.Events(control, formActive as Forms.Base.JupiterShared))
            {
                ev.RunActions(action.Actions);
                return ev.PrintDefinition;
            }
        }

        #region Menu

        private void PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            //e.Allow = false;

            if (gridControl.MenuItems == null)
                return;

            bool popItUp = false;

            //e.HitInfo.InGroupPanel

            if (e.HitInfo.InRow)
            {
                gridView1.FocusedRowHandle = e.HitInfo.RowHandle;
                if (gridView1.FocusedRowHandle >= 0)
                    popItUp = true;
            }
            //else if (e.HitInfo.HitTest == GridHitTest.EmptyRow)
            //    popItUp = true;

            PopulateMenuItems("", "grid", popItUp);
            if (e.HitInfo.HitTest != GridHitTest.RowFooter) gridControl.ControlMenu.Show(gridView1.GridControl, e.Point);
        }

        #endregion Menu

        /// <summary>
        /// Row field for styles/parser. DataTable uses GetDataRow; VirtualServerMode (Expando) uses GetRowCellValue.
        /// </summary>
        private object GetRowFieldValue(int rowHandle, string field)
        {
            if (rowHandle < 0 || string.IsNullOrEmpty(field) || gridView1 == null)
                return null;

            DataRow dataRow = gridView1.GetDataRow(rowHandle);
            if (dataRow != null)
            {
                if (dataRow.Table != null && dataRow.Table.Columns.Contains(field))
                    return dataRow[field];
                return null;
            }

            // B6 VirtualServerMode / non-DataTable rows
            try
            {
                return gridView1.GetRowCellValue(rowHandle, field);
            }
            catch
            {
                return null;
            }
        }

        public object GetValue(string field, List<string> helperStr)
        {
            string helperStr1 = "";
            if (helperStr != null && helperStr.Count > 0)
                helperStr1 = helperStr[0];

            field = Carpo.UI.Logic.Data.Core.FindDataColumn(field, DataColumns);

            if (string.IsNullOrEmpty(field))
                return null;

            if (helperStr1 == "sum")
            {
                if (gridView1.Columns[field].Summary.Count > 0)
                    return gridView1.Columns[field].Summary[0].SummaryValue;
                else
                    return 0;
            }
            else if (gridControl.DataSource != null && helperStr1 == "GridRowCellStyle" && rowCellStyleEventArgs != null)
                return GetRowFieldValue(rowCellStyleEventArgs.RowHandle, field);
            else if (gridControl.DataSource != null && helperStr1 == "GridRowStyle" && rowStyleEventArgs != null)
                return GetRowFieldValue(rowStyleEventArgs.RowHandle, field);
            else if (gridControl.DataSource != null && gridControl.DataRowRef != null)
                return gridControl.DataRowRef[field];
            else if (gridControl.DataSource != null && gridView1.FocusedRowHandle >= 0)
                return GetRowFieldValue(gridView1.FocusedRowHandle, field);

            return null;
        }

        public object GetValueSpecial(string name, List<string> helperStr)
        {
            string helperStr1 = "";
            if (helperStr != null && helperStr.Count > 0)
                helperStr1 = helperStr[0];

            name = name.Trim().ToLower();
            if (name == "rowcount")
                return gridView1.DataRowCount.ToString();
            else if (name == "selectedrowscount")
            {
                if (definition.SelectionWebStyle == true)
                    return GridCheckMarksSelection.SelectedCount.ToString();
                else
                    return GetSelectedRows().Length.ToString();
            }
            else if (name.ToLower().StartsWith("selectedids"))
                return GetSelectedIds(helperStr);
            else if (name == "focusedvalue")
                return gridView1.FocusedValue;
            else if (name.StartsWith("cellvalue") && helperStr1 == "GridRowCellStyle" && rowCellStyleEventArgs != null)
            {
                int rowHandle = rowCellStyleEventArgs.RowHandle;
                int indexCol = gridView1.Columns[rowCellStyleEventArgs.Column.FieldName].VisibleIndex;

                if (name.ToLower().Contains("previous") || name.ToLower().Contains("-1") || name.ToLower().Contains("up"))
                    rowHandle -= 1;
                else if (name.ToLower().Contains("next") || name.ToLower().Contains("+1") || name.ToLower().Contains("down"))
                    rowHandle += 1;

                if (name.ToLower().Contains("left"))
                    indexCol -= 1;
                else if (name.ToLower().Contains("right"))
                    indexCol += 1;

                if (rowHandle < 0 || rowHandle >= gridView1.RowCount
                            || indexCol < 0 || indexCol >= gridView1.Columns.Count)
                    return null;
                else
                {
                    var fieldName = gridView1.Columns.FirstOrDefault(x => x.VisibleIndex == indexCol).FieldName;
                    return gridView1.GetRowCellValue(rowHandle, fieldName);
                }
            }
            else if (name == "focusedfield")
                return gridView1.FocusedColumn.FieldName;
            else if (name == "rownumber")
            {
                if (rowCellStyleEventArgs != null)
                    //tomic
                    //   return rowCellStyleEventArgs.RowHandle + 1;
                    return gridView1.FocusedRowHandle + 1;
                else
                    return gridView1.FocusedRowHandle + 1;
            }
            else if (name == "rowindex" || name == "rowhandle")
            {
                if (rowCellStyleEventArgs != null)
                    return rowCellStyleEventArgs.RowHandle;
                else
                    return gridView1.FocusedRowHandle;
            }
            else if (name == "fieldname" && helperStr1 == "GridRowCellStyle" && rowCellStyleEventArgs != null)
                return rowCellStyleEventArgs.Column.FieldName;
            else if (name == "visible")
                return gridControl.Visible;

            return null;
        }

        public object GetSelectedIds(List<string> helperStr)
        {
            string key = definition.Key;
            if (string.IsNullOrEmpty(key))
                return null;

            if (helperStr != null && helperStr[0].ToLower() != "table")
                key = helperStr[0].ToLower();

            var keyField = Logic.Data.Core.FindDataColumn(key, DataColumns);
            if (keyField == null)
                return null;

            if (definition.SelectionWebStyle == true)
                return GridCheckMarksSelection.GetSelectedIds(keyField);

            int[] selectedRows = GetSelectedRows();

            /* selectedids u tablicu */
            if (helperStr != null && helperStr[helperStr.Count - 1].ToLower() == "table")
            {
                Type type = typeof(int); // ovdje po potrebi dodati mogućnost string tipa
                DataTable dt = new DataTable();
                dt.Columns.Add("Id", type);

                foreach (var rowHandle in selectedRows)
                {
                    try
                    {
                        var value = gridView1.GetDataRow(rowHandle)[keyField];
                        if (value != null)
                        {
                            dt.Rows.Add(value);
                        }
                    }
                    catch
                    {
                        form.Logger.Warn("Vrijednosti koje se šalju u obliku tablice nisu odgovarajućeg tipa", "GetSelectedIds");
                    }
                }
                return dt;
            }

            List<string> listId = new List<string>();
            foreach (int rowHandle in selectedRows)
            {
                var value = gridView1.GetDataRow(rowHandle)[keyField];
                if (value != null)
                {
                    listId.Add(value.ToString());
                }
            }

            if (listId.Count > 0)
            {
                if (definition.SelectedTildaIds == "true")
                {
                    var test = string.Join("~", listId.ToArray());
                    return string.Join("~", listId.ToArray());
                }
                // test = string.Join(",", listId.ToArray());
                else
                {
                    return string.Join(",", listId.ToArray());
                }

            }

            // 

            return null;
        }

        #region Editor

        private void ShowingEditor(object sender, CancelEventArgs e)
        {
            string fieldName = gridView1.FocusedColumn.FieldName;

            if (editHyperlinkColumns.FirstOrDefault(x => x.Name == fieldName) != null)
            {
                EditHyperlink(fieldName);
            }
            //else if (editCheckColumns.FirstOrDefault(x => x.Name == fieldName) != null)
            //{
            //    //EditHyperlink(fieldName);
            //}
            else
            {
                using (Logic.Events events = new Logic.Events(form))
                {
                    object value = events.RunActions(eventShowingEditor, "editor");
                    if (value is bool? && value as bool? == false)
                        e.Cancel = true;
                }
            }
        }

        // dodao Ognjen
        private void ShownEditor(object sender, EventArgs e)
        {
            if (definition.ValueChangeOnScroll == false)
            {
                TextEdit edit = (sender as BaseView).ActiveEditor as TextEdit;
                if (edit != null)
                    edit.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(edit_Spin);
            }

            // (klikabilna url/mail/file ćelija) - forsiram kraj editiranja ćelije kako bi opet mogli kliknuti u nju bez mijenjanja selecta/focusa na drugu
            if ((sender as BaseView).ActiveEditor.GetType() == typeof(HyperLinkEdit))
                gridView1.CloseEditor();


            var isT = gridView1.ActiveEditor is MemoEdit;
            if (!(gridView1.ActiveEditor is MemoEdit)) return;

            (gridView1.ActiveEditor as MemoEdit).TextChanged += ComponentGrid_TextChanged;
        }

        private void ComponentGrid_TextChanged(object sender, EventArgs e)
        {
            if (!(sender is MemoEdit)) return;
            var memoEdit = (MemoEdit)sender;

            var viewInfo = (MemoEditViewInfo)memoEdit.GetViewInfo();
            var cache = new GraphicsCache(memoEdit.CreateGraphics());
            int height = ((IHeightAdaptable)viewInfo).CalcHeight(cache, viewInfo.MaskBoxRect.Width);
            var args = new ObjectInfoArgs();
            args.Bounds = new Rectangle(0, 0, viewInfo.ClientRect.Width, height);
            Rectangle rect = viewInfo.BorderPainter.CalcBoundsByClientRectangle(args);
            cache.Dispose();
            if (memoEdit.Height == rect.Height) return;

            var s = memoEdit.SelectionStart;
            gridView1.PostEditor();
            gridView1.ShowEditor();
            memoEdit.SelectionLength = 0;
            memoEdit.SelectionStart = s;

            memoEdit.Height = rect.Height;
        }

        private void edit_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            e.Handled = true;
        }

        public void GetNextEditableCell()
        {
            int rowHandle = gridView1.FocusedRowHandle;
            int columnIndex = gridView1.FocusedColumn.VisibleIndex;

            editDirection = EDitDirection.Horizontal;
            if (!string.IsNullOrEmpty(definition.EditDirection) && definition.EditDirection.ToLower() == "vertical")
                editDirection = EDitDirection.Vertical;

            GetNextEditableCell(rowHandle, columnIndex);
            SetFocusSelected();
        }

        public void GetNextEditableCell(int rowHandle, int columnIndex)
        {
            int nextRowHandle, nextColumnIndex;

            //pronađi sljedeće column i row indekse
            if (editDirection == EDitDirection.Vertical)
                GetNextEditableCellVertical(rowHandle, columnIndex, out nextRowHandle, out nextColumnIndex);
            else
                GetNextEditableCellHorizontal(rowHandle, columnIndex, out nextRowHandle, out nextColumnIndex);

            GridColumn nextColumn = gridView1.VisibleColumns[nextColumnIndex];

            bool condition = nextColumn.OptionsColumn.AllowEdit == true &&
                editHyperlinkColumns.FirstOrDefault(x => x.Name == nextColumn.FieldName) == null;

            if (condition == false)
            {
                GetNextEditableCell(nextRowHandle, nextColumnIndex);
                return;
            }

            gridView1.FocusedRowHandle = nextRowHandle;
            gridView1.FocusedColumn = gridView1.VisibleColumns[nextColumnIndex];

            //ako ne mogu prikazati editor, traži sljedeći;
            bool canShowEditor = gridView1.CanShowEditor;
            if (canShowEditor == false)
            {
                GetNextEditableCell(nextRowHandle, nextColumnIndex);
            }
        }

        private void GetNextEditableCellHorizontal(int rowHandle, int columnIndex, out int nextRowHandle, out int nextColumnIndex)
        {
            nextColumnIndex = columnIndex + 1;
            nextRowHandle = rowHandle;

            if (nextColumnIndex > gridView1.VisibleColumns.Count - 1)
            {
                nextColumnIndex = 0;
                nextRowHandle = GetNextRowHandle(rowHandle);
            }
        }

        private void GetNextEditableCellVertical(int rowHandle, int columnIndex, out int nextRowHandle, out int nextColumnIndex)
        {
            nextRowHandle = rowHandle + 1;
            nextColumnIndex = columnIndex;

            if (nextRowHandle > GetVisibleRowCount() - 1)
            {
                nextRowHandle = 0;
                nextColumnIndex = GetNextColumnIndex(columnIndex);
            }
        }

        private int GetVisibleRowCount()
        {
            int returnInt = 0;
            if (gridView1.GroupCount == 0)
            {
                returnInt = gridView1.RowCount;
            }
            else
            {
                for (int rowHandle = -1; gridView1.IsValidRowHandle(rowHandle); rowHandle--)
                {
                    if (gridView1.GetChildRowHandle(rowHandle, 0) > -1)
                    {
                        returnInt += gridView1.GetChildRowCount(rowHandle);
                    }
                }
            }
            return returnInt;
        }

        private int GetNextColumnIndex(int columnIndex)
        {
            if (columnIndex < gridView1.VisibleColumns.Count - 1)
                return columnIndex + 1;
            else
            {
                return 0;
            }
        }

        private int GetNextRowHandle(int rowHandle)
        {
            if (rowHandle < gridView1.RowCount - 1)
                return rowHandle + 1;
            else
            {
                return 0;
            }
        }

        private int GetPreviousRowHandle(int rowHandle)
        {
            if (rowHandle != 0)
                return rowHandle - 1;
            else
            {
                return 0;
            }
        }

        private void EditHyperlink(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                fieldName = gridView1.FocusedColumn.FieldName;

            string type = editHyperlinkColumns.FirstOrDefault(x => x.Name == fieldName).Type;
            if (!string.IsNullOrEmpty(type))
            {
                var value = gridView1.GetDataRow(gridView1.FocusedRowHandle)[fieldName];
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    try
                    {
                        if (type.ToLower() == "email")
                            Process.Start("mailto:" + value.ToString());
                        // url(browser) i file(file explorer)
                        else if (type.ToLower() == "file")
                        {
                            if (File.Exists(value.ToString()) || Directory.Exists(value.ToString()))
                                Process.Start(value.ToString());
                            else
                                form.Logger.Warn("Greška otvaranja datoteke/URL-a", value.ToString(), null);
                        }
                        else if (type.ToLower() == "url")
                            Process.Start(value.ToString());
                    }
                    catch (Exception ex)
                    {
                        if (type.ToLower() == "email")
                            form.Logger.Error("Greška slanja e-maila.", ex.Message, ex);
                        // url(browser) i file(file explorer)
                        else
                        {
                            if (type.ToLower() == "file")
                                form.Logger.Warn("Greška otvaranja datoteke.", ex.Message, ex);
                            else
                                form.Logger.Warn("Greška otvaranja URL-a.", ex.Message, ex);
                        }
                    }
                }
            }
        }

        #endregion Editor

        public void SetFocus(JEventAction action)
        {
            try
            {
                if (action.Parameter.Count > 0)
                {
                    using (Parser parser = new Parser(FormsVar.GetActiveForm(form), null))
                    {
                        string outValueStr = "";
                        object value = Carpo.UI.Logic.Parameters.GetValue(form, null, action, out outValueStr);
                        if (value != null)
                            LocateByValue(value);
                    }
                }
                else
                {
                    gridView1.Focus();
                }

                if (!string.IsNullOrEmpty(FieldName))
                {
                    gridView1.FocusedRowHandle = focusedRowHandle;
                    gridView1.FocusedColumn = gridView1.Columns[FieldName];

                    gridView1.ShowEditor();

                    FieldName = string.Empty;
                }



            }
            catch (Exception ex)
            {
                form.Logger.Error("SetFocus", ex.Message, ex);
            }
        }

        public void SetFocusNextRow(JEventAction action)
        {
            try
            {
                if (gridView1.RowCount > 1 && gridView1.FocusedRowHandle != gridView1.RowCount - 1)
                {
                    int rowHandle = gridView1.FocusedRowHandle + 1;
                    gridView1.FocusedRowHandle = rowHandle;
                    SetFocusSelected();
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("SetFocusNextRow", ex.Message, ex);
            }
        }

        public void SetFocusLastRow(JEventAction action)
        {
            try
            {
                if (gridView1.RowCount > 1 && gridView1.FocusedRowHandle != gridView1.RowCount - 1)
                {
                    int rowHandle = gridView1.RowCount - 1;
                    gridView1.FocusedRowHandle = rowHandle;
                    SetFocusSelected();
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("SetFocusLastRow", ex.Message, ex);
            }
        }

        public void SetSelectedRows(JEventAction action)
        {
            try
            {
                if (action.Parameter.Count > 0)
                {
                    using (Parser parser = new Parser(FormsVar.GetActiveForm(form), null))
                    {
                        string outValueStr = "";
                        object value = Carpo.UI.Logic.Parameters.GetValue(form, null, action, out outValueStr);

                        if (value != null)
                        {
                            try
                            {
                                if (value.ToString().ToLower() == "all")
                                    gridView1.SelectAll();
                                else if (value.ToString().ToLower() == "none")
                                    gridView1.ClearSelection();
                                else
                                {
                                    string[] arrSelectedIds = value.ToString().Split(',');
                                    bool intArray = IntArrayCheck(arrSelectedIds);
                                    SelectRows(intArray, arrSelectedIds);
                                }
                            }
                            catch
                            {
                                form.Logger.Warn("Parametar za selektiranje nije ispravno definiran", "SetSelectedRows");
                                gridView1.Focus();
                            }
                        }
                        else
                            CheckClearSelectionWebStyle();
                    }
                }
                else
                {
                    form.Logger.Warn("Parametar za selektiranje nije definiran", "SetSelectedRows");
                    gridView1.Focus();
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("SetSelectedRows", ex.Message, ex);
            }
        }

        public bool IntArrayCheck(string[] arr)
        {
            foreach (string str in arr)
            {
                int index;
                bool isNumeric = int.TryParse(str, out index);

                if (isNumeric == false)
                    return false;
            }
            return true;
        }

        public void SetAttributeShowCheckBoxes(bool value)
        {
            gridView1.OptionsSelection.MultiSelect = value;
            if (value == true)
            {
                gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            }
            else
            {
                gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                gridView1.ClearSelection();
            }
        }

        public void SetExport(JEventAction action)
        {
            try
            {
                if (action.Parameter.Count > 0)
                {
                    // provjeravam da li parametar ima i 'name' i 'value'
                    if (action.Parameter[0].Value != null && action.Parameter[0].Name != null)
                    {
                        string parName = ValueParserToString(action.Parameter[0].Name);
                        string parValue = ValueParserToString(action.Parameter[0].Value);

                        if (parName != "doctype")
                            form.Logger.Warn("Neispravan 'name' parametra", "Akcija 'control export' mora sadržavati 'doctype' parametar");
                        else
                        {
                            List<string> typeList = new List<string> { "xlsx", "xls", "pdf", "html", "rtf", "csv" };





                            if (!(typeList.Contains(parValue)))
                                form.Logger.Warn("Neispravan 'value' parametra", "Neispravan tip dokumenta");
                            else
                                GridExport(parValue);
                        }

                        return;
                    }
                    else
                        form.Logger.Warn("Neispravan format parametra", "Parametar akcije 'control export' mora sadržavati atribute 'name' i 'value' ");
                }
                else
                    form.Logger.Warn("Nepostojeći parametar za exportanje", "Akcija 'control export' mora sadržavati 'doctype' parametar");
            }
            catch (Exception ex)
            {
                form.Logger.Error("SetExport", ex.Message, ex);
            }
        }

        private void SelectRows(bool isArrayInt, string[] arr)
        {
            bool reset = true;
            int i = 0;
            blockEvents = true; // potreban kako bismo blokirali višestruko okidanje selectionchangeda i focusedrowchangeda

            gridView1.BeginUpdate();
            CheckClearSelectionWebStyle();

            foreach (string str in arr)
            {
                i++;
                int loc = -1;
                int arrCount = arr.Length;
                if (i == arrCount)
                    blockEvents = false;

                if (isArrayInt)
                    loc = gridView1.LocateByValue(keyField, Convert.ToInt32(str));
                else
                    loc = gridView1.LocateByValue(keyField, str);

                if (loc != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                    reset = false;

                gridView1.FocusedRowHandle = loc;

                if (loc >= 0)
                {
                    if (definition.SelectionWebStyle == true)
                        GridCheckMarksSelection.SelectRow(gridView1.FocusedRowHandle, true); // lupamo TRUE u check kolonu

                    gridView1.SelectRow(loc);
                }
            }

            if (reset)
                CheckClearSelectionWebStyle();

            if (definition.SelectionWebStyle == true)
                OnSelectionChangedExtracted(); // ovo je da uđe u selection changed ako postoji u XML-u

            gridView1.EndUpdate();
        }

        public void SetFilter(JEventAction action)
        {
            try
            {
                // provjeravam da li akcija ima barem jedan parametar
                if (action.Parameter.Count > 0)
                {
                    // provjeravam da li parametar ima i 'name' i 'value'
                    if (action.Parameter[0].Value != null && action.Parameter[0].Name != null)
                    {
                        string colCaption = ValueParserToString(action.Parameter[0].Name);
                        string filterValue = ValueParserToString(action.Parameter[0].Value);

                        if (colCaption == "" && filterValue == "")
                        {
                            gridView1.ClearColumnsFilter();
                            return;
                        }
                        //provjeravam da li u gridu postoji kolona
                        object filterColumn = Logic.Data.Core.FindDataColumn(colCaption, DataColumns);

                        if (filterColumn == null)
                            form.Logger.Warn("Neispravan naziv kolone za filtriranje", "Kolona '" + colCaption + "' grida '" + gridControl.Name + "' nije pronađena");
                        else
                            gridView1.SetRowCellValue(DevExpress.XtraGrid.GridControl.AutoFilterRowHandle, gridView1.Columns[filterColumn.ToString()], filterValue);
                    }
                    else
                        form.Logger.Warn("Neispravan format parametra", "Parametar akcije 'control filter' mora sadržavati atribute 'name' i 'value' ");
                }
                else
                {
                    form.Logger.Warn("Nepostojeći parametar za filtriranje", "Akcija 'control filter' mora sadržavati parametar sa atributima 'name' i 'value' ");
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("SetFilter", ex.Message, ex);
            }
        }

        public void DataSourceFilter(JEventAction action)
        {
            string filterStr = "";
            var dt = gridControl.DataSource as DataTable;

            if (dt == null || dt.Rows.Count == 0)
            {
                form.Logger.Warn("Prazan dataset", "Datasource kontrole '" + gridControl.Name + "' ne sadrži niti jedan zapis.");
                return;
            }

            try
            {
                // provjeravam da li akcija ima barem jedan parametar
                if (action.Parameter.Count > 0)
                {
                    for (int i = 0; i < action.Parameter.Count; i++)
                    {
                        // provjeravam da li parametar ima 'name', 'value' i 'type'
                        if (action.Parameter[i].Value != null && action.Parameter[i].Name != null && action.Parameter[i].Type != null)
                        {
                            string colCaption = ValueParserToString(action.Parameter[i].Name);
                            string filterValue = ValueParserToString(action.Parameter[i].Value);

                            //provjeravam da li u gridu postoji kolona
                            object filterColumn = Logic.Data.Core.FindDataColumn(colCaption, DataColumns);
                            if (filterColumn == null)
                                form.Logger.Warn("Neispravan naziv kolone za filtriranje", "Kolona '" + colCaption + "' grida '" + gridControl.Name + "' nije pronađena");
                            else
                            {
                                filterValue = Carpo.Common.Helpers.Grid.DataSourceFilterHelpers("filterValue", filterValue, Type.GetTypeCode(gridView1.Columns[colCaption].ColumnType));
                                string typeStr = Carpo.Common.Helpers.Grid.DataSourceFilterHelpers("typeStr", action.Parameter[i].Type, null);
                                string literalStr = Carpo.Common.Helpers.Grid.DataSourceFilterHelpers("literalStr", colCaption, Type.GetTypeCode(gridView1.Columns[colCaption].ColumnType));
                                string likeHelper1 = Carpo.Common.Helpers.Grid.DataSourceFilterHelpers("likeHelper1", action.Parameter[i].Type, null);
                                string likeHelper2 = Carpo.Common.Helpers.Grid.DataSourceFilterHelpers("likeHelper2", action.Parameter[i].Type, null);
                                string conStr = "";

                                if (i > 0)
                                {
                                    if (!string.IsNullOrEmpty(action.Parameter[i].Condition))
                                    {
                                        if (action.Parameter[i].Condition.ToLower() == "and" || action.Parameter[i].Condition.ToLower() == "or")
                                            conStr = " " + action.Parameter[i].Condition + " ";
                                        else
                                        {
                                            form.Logger.Warn("datasource filter", "Višestruki (dodatni) parametri zahtjevaju pravilno definiran atribut 'condition'");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        form.Logger.Warn("datasource filter", "Nedefiniran atribut 'condition'");
                                        continue;
                                    }
                                }
                                filterStr += conStr + colCaption + typeStr + literalStr + likeHelper1 + filterValue + likeHelper2 + literalStr;
                            }
                        }
                        else
                            form.Logger.Warn("Neispravan format parametra", "Neispravan format parametra (parametar mora sadržavati atribute 'name', 'value' i 'type'");
                    }
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("DataSourceFilter", ex.Message, ex);
                filterStr = "";
            }
            finally
            {
                dt.DefaultView.RowFilter = filterStr;
            }

            return;
        }

        private string ValueParserToString(string valueStr)
        {
            using (Parser parser = new Parser(form, null))
            {
                object value = parser.GetLogicPartValue(valueStr);
                if (value != null)
                    return value.ToString();
                else
                    return valueStr;
            }
        }

        public void RefreshRow(JEventAction action)
        {
            try
            {
                DataTable updatedRowTable = getRefreshedData(action);
                DataRow updatedRow = updatedRowTable.Rows[0];
                var rowHandleToRefresh = gridView1.LocateByValue(keyField, updatedRow[keyField]);

                for (int i = 0; i < updatedRow.ItemArray.Length; i++)
                {
                    gridView1.SetRowCellValue(rowHandleToRefresh, updatedRowTable.Columns[i].ColumnName, updatedRow[i]);
                }
            }
            catch (Exception ex)
            {
                form.Logger.Warn("RefreshRow()", ex.Message);
            }
        }

        private DataTable getRefreshedData(JEventAction action)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                bool stopActions = false;
                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, action.Parameter[0].Value);
                return dataDefinition.GetDataSet("dohvaćanje data rowa grida '" + definition.Name + "'", out stopActions).Tables[0];
            }
            catch { throw; }
            finally
            {
                Cursor.Current = Cursors.Default;
                Action = "";
            }
        }

        private string SetPrintHeaderValue(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return ValueParserToString(str);
            else return "";
        }

        private string SetPrintHeaderAlignment(string str)
        {
            if (!string.IsNullOrEmpty(str) && (str == "center" || str == "right"))
                return str;
            else
                return "left";
        }

        public void SearchDoc()
        {
            try
            {
                SetFocusOnSearchRow();
            }
            catch (Exception ex)
            {
                form.Logger.Error("grid: SearchDoc", ex.Message, ex);
            }
        }

        #region GridTools

        public void GridExpand()
        {
            if (gridControl.ComponentExt.IsCollapsed == true)
            {
                gridControl.ComponentExt.gridView1.ExpandAllGroups();
            }
            else
            {
                gridControl.ComponentExt.gridView1.CollapseAllGroups();
            }
            gridControl.ComponentExt.IsCollapsed = !gridControl.ComponentExt.IsCollapsed;
        }

        public void GridReset()
        {
            if (Common.Helpers.JConfirm.Confirm("Želite li vratiti grid na inicijalni prikaz?", "Grid - Inicijalni prikaz", null, null) == DialogResult.Yes)
            {
                try
                {
                    if (form is Manager) ((ISettingsForm)form).ResetSettings();
                    StopUsingTemplate(GetTemplatesModel());

                    // dodao T.A., zbog hardkodiranja fixiranih koloni u inlineu nije vraćao pravilan raspored koloni iz DefaultLayoutStreama
                    if (gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True)
                        gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.False;

                    MemoryStream ms = gridControl.ComponentExt.DefaultLayoutStream as MemoryStream;
                    ms.Seek(0, SeekOrigin.Begin);
                    gridControl.ComponentExt.gridView1.RestoreLayoutFromStream(ms);
                    if (gridControl.ComponentExt.gridView1.RowCount > 0)
                    {
                        gridControl.ComponentExt.IsBestFitted = false;
                        gridControl.ComponentExt.BestFitColumnsInit();
                    }
                    groupSummaryCreated = false;
                    CreateGroupSummary();
                    SetGroupSummaryStyle();
                    //HandleBBIcheckOnLayoutChange(true);
                }
                catch (Exception ex)
                {
                    form.Logger.Error("Neuspješan restore grida", ex.Message, ex);
                }
            }
        }

        public void GridOptionSum()
        {
            bool reverse = !Convert.ToBoolean(gridControl.ComponentExt.gridView1.OptionsView.ShowFooter);
            gridControl.ComponentExt.SetProperty(Components.JGridControl.Property.ShowFooter, reverse);
        }

        public void GridGroupSumsInline()
        {
            // na layoutu je bio slučaj kada je gridControl.ComponentExt = null...
            if (gridControl.ComponentExt != null)
            {
                bool reverse = gridControl.ComponentExt.gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True ? false : true;
                gridControl.ComponentExt.SetProperty(Components.JGridControl.Property.SumsInline, reverse);
                if (this.form is Forms.Master.Manager && !(string.IsNullOrEmpty(this.definition.Type)) && this.definition.Type.ToLower() == "master")
                { (this.form as Forms.Master.Manager).HandleBbiCheck((this.form as Forms.Master.Manager).bbiOptionGroupSumInline, reverse); }
            }
        }

        public void GridGroupSumsUnder()
        {
            if (gridControl.ComponentExt != null)
            {
                bool reverse = gridControl.ComponentExt.gridView1.OptionsView.GroupFooterShowMode == GroupFooterShowMode.VisibleAlways ? false : true;
                gridControl.ComponentExt.SetProperty(Components.JGridControl.Property.SumsUnder, reverse);
                if (this.form is Forms.Master.Manager && !(string.IsNullOrEmpty(this.definition.Type)) && this.definition.Type.ToLower() == "master")
                { (this.form as Forms.Master.Manager).HandleBbiCheck((this.form as Forms.Master.Manager).bbiOptionGroupSumUnder, reverse); }
            }
        }

        public void GridGroupSumsIfExpanded()
        {
            if (gridControl.ComponentExt != null)
            {
                bool reverse = gridControl.ComponentExt.gridView1.OptionsView.GroupFooterShowMode == GroupFooterShowMode.VisibleIfExpanded ? false : true;
                gridControl.ComponentExt.SetProperty(Components.JGridControl.Property.SumsIfExpanded, reverse);
                if (this.form is Forms.Master.Manager && !(string.IsNullOrEmpty(this.definition.Type)) && this.definition.Type.ToLower() == "master")
                { (this.form as Forms.Master.Manager).HandleBbiCheck(null, false); }
            }
        }

        public void GridExport(string tag)
        {
            using (Carpo.Core.Forms.GridExport frm = new Core.Forms.GridExport())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    bool selectedOnly = frm.ExportTip == Core.Forms.GridExport.Export.SelectedRows;
                    if (selectedOnly)
                        gridView1.OptionsPrint.PrintSelectedRowsOnly = true;

                    GridControl exportControl = gridControl;
                    try
                    {
                        if (IsVirtualServerModeActive)
                        {
                            if (!TryPrepareVsmHydratePrintClone(selectedOnly))
                                return;
                            exportControl = PrintClonedGridControl ?? gridControl;
                        }

                        string filePath = "";
                        using (SaveFileDialog dialog = new SaveFileDialog())
                        {
                            dialog.Filter = "(*." + tag + ")|*." + tag + "";
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                filePath = dialog.FileName;
                                switch (tag)
                                {
                                    case "xlsx":
                                        exportControl.ExportToXlsx(filePath);
                                        break;

                                    case "xls":
                                        exportControl.ExportToXls(filePath);
                                        break;

                                    case "pdf":
                                        exportControl.ExportToPdf(filePath);
                                        break;

                                    case "html":
                                        exportControl.ExportToHtml(filePath);
                                        break;

                                    case "rtf":
                                        exportControl.ExportToRtf(filePath);
                                        break;

                                    case "csv":
                                        exportControl.ExportToCsv(filePath);
                                        break;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            if (XtraMessageBox.Show(Common.Translator.TranslateString("Datoteka je kreirana. Želite li ju otvoriti?"), "Grid Export", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Process.Start(filePath);
                            }
                        }
                    }
                    finally
                    {
                        gridView1.OptionsPrint.PrintSelectedRowsOnly = false;
                        if (IsVirtualServerModeActive)
                        {
                            PrintClonedGridControl?.Dispose();
                            PrintClonedGridControl = null;
                        }
                    }
                }
            }
        }

        private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (clearColumnList.Contains(e.Column.FieldName))
                e.DisplayText = "";
        }

        public void ClearColumns()
        {
            gridView1.Columns.Clear();
            ForceReinitFields = true;
        }

        public void ClearTabCaption()
        {
            if (tabPage != null)
                tabPage.Text = tabPageCaption;
        }

        #endregion GridTools

        #region CreatingGroupSummary

        private void CreateGroupSummary()
        {
            if (groupSummaryCreated || !(DataColumns.Length > 0))
                return;

            if (definition.GroupSummary != null && definition.GroupSummary.SummaryList.Count > 0)
            {
                bool addCount = true;
                int sumCount = definition.GroupSummary.SummaryList.Count;
                foreach (JSummary sum in definition.GroupSummary.SummaryList)
                {
                    if (sum.Type.ToLower() == "count") // ako je type="count" definiran onda ne idem u ručno setanje
                    {
                        addCount = false;
                        break;
                    }
                }

                if (addCount) // ručno (defaultno) setanje count-a
                {
                    sumCount++;
                    AddCountSummaryType("", sumCount);
                }

                foreach (JSummary sum in definition.GroupSummary.SummaryList)
                {
                    var summaryType = GetGroupSummaryType(sum.Type.ToLower());

                    if (summaryType == DevExpress.Data.SummaryItemType.None)
                    {
                        form.Logger.Warn("Neispravan tip za grupno sumiranje", "Tip '" + sum.Type.ToLower() + "' ne postoji!");
                        continue;
                    }
                    else if (summaryType == DevExpress.Data.SummaryItemType.Count)
                    {
                        if (sum.Visible != false) // ako mu nisam setao visible="false" idem u defaultno kreiranje
                            AddCountSummaryType(sum.Format, sumCount);
                        else
                            sumCount--;
                    }
                    else
                    {
                        string fieldName = Logic.Data.Core.FindDataColumn(sum.FieldName, DataColumns);
                        if (fieldName != null)
                        {
                            string colCaption = Common.Helpers.Strings.AddSpacesToSentence(fieldName);
                            string formatDef = colCaption + " (" + sum.Type.ToLower() + "): {0:n2}";
                            if (!string.IsNullOrEmpty(sum.Format))
                                formatDef = sum.Format;

                            gridView1.GroupSummary.Add(new GridGroupSummaryItem(summaryType, fieldName, null, formatDef));
                            setGroupRowText = true;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(sum.FieldName))
                                form.Logger.Warn("Neispravan naziv kolone", "Ne postoji atribut 'fieldname' ili mu je vrijednost prazna!");
                            else
                                form.Logger.Warn("Neispravan naziv kolone", "Kolona '" + sum.FieldName + "' grid kontrole '" + gridControl.Name + "' nije pronađena");

                            continue;
                        }
                    }
                }

                if (setGroupRowText) // formatiram string (samo ako nije defaultni count u pitanju
                    SetGroupRowText(sumCount);
            }
            else
            {


                //gridView1.GroupSummary.Clear();
                gridView1.GroupSummary.Add(DevExpress.Data.SummaryItemType.Count, string.Empty, null, "({0})");
                // }
                //if (gridView1.GroupSummary.Count > 1)
                //{
                //    for (int i = 2; i < gridView1.GroupSummary.Count; i++)
                //    {
                //        gridView1.GroupSummary.RemoveAt(i);
                //    }

                //}
            }

            groupSummaryCreated = true;
        }

        private void AddCountSummaryType(string format, int sumCount)
        {
            setGroupRowText = true;

            string formatDef = "({0})";

            if (!string.IsNullOrEmpty(format))
                formatDef = format;
            else if (sumCount > 1)
                formatDef = "Ukupno: {0}";
            else
                setGroupRowText = false;

            gridView1.GroupSummary.Add(DevExpress.Data.SummaryItemType.Count, string.Empty, null, formatDef);
        }

        private DevExpress.Data.SummaryItemType GetGroupSummaryType(string type)
        {
            var summaryType = DevExpress.Data.SummaryItemType.None;
            switch (type)
            {
                case "sum":
                    return DevExpress.Data.SummaryItemType.Sum;

                case "min":
                    return DevExpress.Data.SummaryItemType.Min;

                case "max":
                    return DevExpress.Data.SummaryItemType.Max;

                case "count":
                    return DevExpress.Data.SummaryItemType.Count;

                case "avg":
                    return DevExpress.Data.SummaryItemType.Average;
            }
            return summaryType;
        }

        private void SetGroupRowText(int sumCount)
        {
            int gsHandle = gridView1.GroupSummary.Count - sumCount; // prvom dodajem |
            gridView1.GroupSummary[gsHandle].DisplayFormat = "| " + gridView1.GroupSummary[gsHandle].DisplayFormat;

            if (sumCount > 1) // ako postoji više itema, na kraj svakog (osim zadnjeg) dodajem ; da bi mogao uočiti separator i setati ga na ;
            {
                foreach (GridGroupSummaryItem g in gridView1.GroupSummary)
                {
                    if (g.ShowInGroupColumnFooter == null && g.SummaryType != DevExpress.Data.SummaryItemType.None)
                    {
                        if (g.Index != gridView1.GroupSummary.Count - 1)
                            gridView1.GroupSummary[g.Index].DisplayFormat = gridView1.GroupSummary[g.Index].DisplayFormat + ";";
                    }
                }

                gridView1.CustomDrawGroupRow += (s, e) =>
                {
                    CustomDrawGroupRow(s, e);
                };
            }
        }

        private void CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            info.GroupText = info.GroupText.Replace(";,", ";");
        }

        #endregion CreatingGroupSummary

        #region GridTemplates

        public void MenuItemPredlosciClick()
        {
            try
            {
                var columns = new Dictionary<string, string>();

                foreach (var c in gridView1.Columns)
                {
                    columns.Add((c as GridColumn).FieldName, (c as GridColumn).ToString());
                }

                using (var frmColumnSettings = new Carpo.Core.Forms.ColumnSettings(GetTemplatesModel(), gridView1
                    , gridControl.ComponentExt.DefaultLayoutStream as MemoryStream, columns))
                {
                    frmColumnSettings.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("MenuItemPredlosciClick: ", ex.Message, ex);
            }
        }

        private DataTable GetTemplateVisibleColumns(GridTemplatesModel templateModel)
        {
            var dt = new DataTable();

            try
            {
                dt = Carpo.Data.Get.GridTemplates.GetVisibleColumns(templateModel.ConnStr, templateModel.OperateriId
                    , templateModel.ModuleId, templateModel.FormName, templateModel.ComponentName);
            }
            catch (Exception ex)
            {
                form.Logger.Error("GetTemplateVisibleColumns: ", ex.Message, ex);
                dt = null;
            }

            return dt;
        }

        private void StopUsingTemplate(GridTemplatesModel templatesModel)
        {
            try
            {
                string sqlText = "";
                Carpo.Data.Set.GridTemplates.StopUsingTemplate(templatesModel.ConnStr, templatesModel.OperateriId
                    , templatesModel.ModuleId, templatesModel.FormName, templatesModel.ComponentName, null, out sqlText);
            }
            catch (Exception ex)
            {
                form.Logger.Error("StopUsingTemplate: ", ex.Message, ex);
            }
        }

        private DataSet DataSetForPrinting(string action, GridTemplatesModel templatesModel)
        {
            try
            {
                return Carpo.Data.Get.GridTemplates.DataSetForPrinting(action, templatesModel.ConnStr, templatesModel.OperateriId
                    , templatesModel.ModuleId, templatesModel.FormName, templatesModel.ComponentName);
            }
            catch (Exception ex)
            {
                form.Logger.Error("DataSetForPrinting: ", ex.Message, ex);
                return null;
            }
        }

        private GridTemplatesModel GetTemplatesModel()
        {
            return new Carpo.Structure.Models.Core.GridTemplatesModel()
            {
                ConnStr = connectionString,
                OperateriId = Core.PubVars.OperateriId,
                ModuleId = form.ModuleId,
                FormName = form.FormName,
                ComponentName = control.Name
            };
        }

        #endregion GridTemplates

        private void CheckClearSelectionWebStyle()
        {
            gridView1.ClearSelection();
            if (definition.SelectionWebStyle == true)
                GridCheckMarksSelection.ClearSelection();
        }

        private int[] GetSelectedRows()
        {
            var selectedRows = gridView1.GetSelectedRows();

            if (gridView1.GroupedColumns.Count > 0)
            {
                var listSelectedRows = new List<int>(selectedRows);
                listSelectedRows.RemoveAll(x => x < 0);
                selectedRows = listSelectedRows.ToArray();
            }

            return selectedRows;
        }

        #region CustomSort

        private void CheckColumnCustomSort(JField col, GridColumn column, string fieldName)
        {
            if (col.CustomSort != null)
            {
                column.SortMode = ColumnSortMode.Custom;
                CustomSortList.Add(fieldName, col.CustomSort);

                if (!isCustomSortInitialized)
                    InitializeCustomSortEvent();
            }
            else
            {
                if (!string.IsNullOrEmpty(definition.CustomSort))
                {
                    column.SortMode = ColumnSortMode.Custom;
                    CustomSortList.Add(fieldName, definition.CustomSort);
                }
            }
        }

        private void InitializeCustomSortEvent()
        {
            gridView1.CustomColumnSort += (s, e) =>
            {
                CustomColumnSort(s, e);
            };

            isCustomSortInitialized = true;
        }

        private void CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null)
                return;

            string kriterij = "";

            if (CustomSortList.ContainsKey(e.Column.FieldName))
                kriterij = CustomSortList.Where(x => x.Key == e.Column.FieldName).FirstOrDefault().Value;
            else if (!string.IsNullOrEmpty(definition.CustomSort))
                kriterij = definition.CustomSort;

            if (kriterij != "")
            {
                try
                {
                    string columnTypeStr = e.Column.ColumnType.Name.ToLower();
                    object val1 = view.GetListSourceRowCellValue(e.ListSourceRowIndex1, e.Column.FieldName);
                    object val2 = view.GetListSourceRowCellValue(e.ListSourceRowIndex2, e.Column.FieldName);
                    e.Handled = true;

                    if (kriterij == "nullaszero")
                    {
                        if (columnTypeStr == "decimal" || columnTypeStr == "int32")
                        {
                            using (var cls = new Carpo.UI.Logic.Grid.CustomSort())
                            {
                                e.Result = cls.CustomNumericalColumnSort(val1, val2, e);
                            }
                        }
                        else
                        {
                            e.Column.SortMode = ColumnSortMode.Default;
                            e.Result = System.Collections.Comparer.Default.Compare(val1, val2);
                        }
                    }
                    else if (kriterij == "stringasint" && columnTypeStr == "string")
                    {
                        using (var cls = new Carpo.UI.Logic.Grid.CustomSort())
                        {
                            e.Result = cls.CustomStringColumnSort(val1, val2, e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.Logger.Error("CustomColumnSort", ex.Message, ex);
                }
            }
        }

        #endregion CustomSort

        #region Conditional formatting

        public void SetConditionalFormatting()
        {
            if (definition.Styles == null || definition.Styles.GridFormatRuleList.Count == 0)
                return;

            // Prevent FormatRules from accumulating across Populate / refresh.
            gridView1.FormatRules.Clear();

            Carpo.Common.GridFormatRules.GridFormatRules gridFormatRules = new Common.GridFormatRules.GridFormatRules();

            foreach (JGridFormatRuleList conditionalFormat in definition.Styles.GridFormatRuleList)
            {
                GridFormatRule gridFormatRule = new GridFormatRule();

                switch (conditionalFormat.Name.ToLower())
                {
                    case "arrows":
                        gridFormatRule = gridFormatRules.SetFormatConditionRuleArrows(gridView1, conditionalFormat.Field, conditionalFormat.GridFormatRuleParameterList,
                            conditionalFormat.ValueType, conditionalFormat.ShowInField);
                        break;

                    case "databar":
                        gridFormatRule = gridFormatRules.SetFormatConditionRuleDataBar(gridView1, conditionalFormat.Field, conditionalFormat.GridFormatRuleParameterList,
                            conditionalFormat.ValueType, conditionalFormat.ShowInField, conditionalFormat.ShowNegativeBar, conditionalFormat.ShowBarOnly);
                        break;

                    case "colorscale":
                        gridFormatRule = gridFormatRules.SetFormatConditionRuleColorScale(gridView1, conditionalFormat.Field, conditionalFormat.GridFormatRuleParameterList,
                            conditionalFormat.ValueType, conditionalFormat.ShowInField);
                        break;
                }

                gridView1.FormatRules.Add(gridFormatRule);
            }
        }

        public void SetAttributeFindPanel(bool value)
        {
            SetAttributeFindPanel(value, value ? "always" : null);
        }

        /// <summary>Wave12: mode always|allow; enables highlight + clear when on.</summary>
        public void SetAttributeFindPanel(bool enabled, string mode)
        {
            definition.ShowFindPanel = enabled;
            bool always = enabled && !string.Equals(mode, "allow", StringComparison.OrdinalIgnoreCase);
            definition.FindPanelMode = enabled ? (always ? "always" : "allow") : null;

            gridView1.OptionsFind.AllowFindPanel = enabled;
            gridView1.OptionsFind.AlwaysVisible = always;
            if (enabled)
            {
                gridView1.OptionsFind.HighlightFindResults = true;
                gridView1.OptionsFind.ShowClearButton = true;
                if (always)
                    gridView1.ShowFindPanel();
                else
                    gridView1.HideFindPanel();
            }
            else
            {
                gridView1.HideFindPanel();
            }
        }

        private void ApplyEmptyMessageHandler()
        {
            gridView1.CustomDrawEmptyForeground -= gridView1_CustomDrawEmptyForeground;
            if (!string.IsNullOrWhiteSpace(definition.EmptyMessage))
                gridView1.CustomDrawEmptyForeground += gridView1_CustomDrawEmptyForeground;
        }

        private void gridView1_CustomDrawEmptyForeground(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e)
        {
            if (gridView1.RowCount > 0 || string.IsNullOrWhiteSpace(definition?.EmptyMessage))
                return;

            e.DefaultDraw();
            using (var brush = new SolidBrush(Color.Gray))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                e.Graphics.DrawString(definition.EmptyMessage, e.Appearance.Font, brush, e.Bounds, format);
            }
            e.Handled = true;
        }


        public void ApplyEditFormOptions()
        {
            if (definition.AllowEditForm != true)
                return;

            gridView1.OptionsBehavior.Editable = true;
            string mode = (definition.EditFormMode ?? "editform").Trim().ToLowerInvariant();
            switch (mode)
            {
                case "inplace":
                    gridView1.OptionsBehavior.EditingMode = GridEditingMode.Inplace;
                    break;
                case "editforminplace":
                    gridView1.OptionsBehavior.EditingMode = GridEditingMode.EditFormInplace;
                    break;
                case "editforminplacehidecurrentrow":
                    gridView1.OptionsBehavior.EditingMode = GridEditingMode.EditFormInplaceHideCurrentRow;
                    break;
                default:
                    gridView1.OptionsBehavior.EditingMode = GridEditingMode.EditForm;
                    break;
            }

            // Keep Carpo double-click actions free; use F2 for EditForm.
            gridView1.OptionsEditForm.ShowOnDoubleClick = DefaultBoolean.False;
            gridView1.OptionsEditForm.ShowOnF2Key = DefaultBoolean.True;
            gridView1.OptionsEditForm.ShowUpdateCancelPanel = DefaultBoolean.True;
        }

        public void SetAttributeEditForm(bool enabled, string mode = null)
        {
            definition.AllowEditForm = enabled;
            if (!string.IsNullOrEmpty(mode))
                definition.EditFormMode = mode;
            else if (enabled && string.IsNullOrEmpty(definition.EditFormMode))
                definition.EditFormMode = "editform";
            if (!enabled)
            {
                gridView1.OptionsBehavior.EditingMode = GridEditingMode.Default;
                return;
            }
            ApplyEditFormOptions();
        }
        public void SetAttributeFilterEditor(bool value)
        {
            gridView1.OptionsFilter.AllowFilterEditor = value;
        }

        public void ApplyColumnChooserOptions(bool enabled)
        {
            definition.AllowColumnChooser = enabled;
            gridView1.OptionsCustomization.AllowColumnMoving = enabled;
            gridView1.OptionsCustomization.AllowQuickHideColumns = enabled;
            gridView1.OptionsCustomization.CustomizationFormSearchBoxVisible = enabled;
            if (!enabled)
                gridView1.HideCustomization();
        }

        /// <summary>
        /// Runtime: columnchooser=true applies polish + opens chooser; false hides it.
        /// </summary>
        public void SetAttributeColumnChooser(bool show)
        {
            ApplyColumnChooserOptions(true);
            if (show)
                gridView1.ShowCustomization();
            else
                gridView1.HideCustomization();
        }

        #endregion Conditional formatting
    }
}
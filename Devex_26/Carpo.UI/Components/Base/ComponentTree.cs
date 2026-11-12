using Carpo.Common.Helpers;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.ViewInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentTree : ComponentBase
    {
        private JTreeListControl treeControl;
        public new JTree definition { get; set; }

        //public string treeAction { get; set; } = "";
        //stvarni popis kolona - iz DataSet-a
        public string[] DataColumns { get; set; } = new string[] { };

        public string ParameterControlValue { get; set; } = "";

        private TreeListNode nodeCellStyle = null;
        private TreeListNode locatedNode = null;

        public BarEditItem MasterBarEditControl { get; set; }
        public Control MasterControl { get; set; }
        public JGridControl DetailControl { get; set; }

        public bool? AutoPopulate { get; set; }
        public bool AllowSelfDrag { get; set; } = false;
        public bool AllowDrop { get; set; } = false;
        public bool AllowDrag { get; set; } = false;
        public bool AllowDragMultiple { get; set; } = true;

        public string valueField { get; set; } = "Id";
        private string keyField = "Id";
        public string parentField { get; set; } = "ParentId";

        public string SearchField = null; //field iz search store

        public JEvent DisableNodeEvent { get; set; }

        public TreeListNode DropNode { get; set; } //DragDrop potrebe > Drop
        public TreeListNode SelfDropNode { get; set; } //DragDrop potrebe > Drop
        public TreeListNode BeforeFocusNode { get; set; } // za blokiranje klika na red

        public bool SelfDragging { get; set; }  //ako je uključen edit drveta i omogućen reordering
        public bool BeforeFocusing { get; set; }  //ako je uključeno blokiranje klika na red
        public DXDragEventArgs SelfDraggingArg { get; set; } = null;  //ako je uključen edit drveta i omogućen reordering
        private DataTable selfDragDataTable;

        private string formatIdField;
        private string rowColorIdField;
        private string rowForeColorIdField;

        private TreeState treeState;

        /// <summary>B8 lazy: parent keys whose children were already fetched (or had none).</summary>
        private readonly HashSet<object> lazyLoadedKeys = new HashSet<object>();
        private bool lazyLoadingChildren;
        private string hasChildrenField;

        public ComponentTree() : base()
        {
        }

        public ComponentTree(Control control, object def) : base(control, def)
        {
            definition = def as JTree;
            treeControl = (control as JTreeListControl);

            Init();

            CreateMenu();
        }

        public void Init()
        {
            DateTime startTime = DateTime.Now;
            treeState = new TreeState(treeControl);

            treeControl.OptionsView.AllowHtmlDrawHeaders = true; // T.A. - omogućeno za html formatiranje captiona kolone
            treeControl.OptionsBehavior.Editable = false;
            treeControl.OptionsMenu.ShowExpandCollapseItems = false;
            treeControl.OptionsBehavior.EnableFiltering = true;
            treeControl.OptionsFilter.AllowColumnMRUFilterList = false;
            treeControl.OptionsFilter.AllowFilterEditor = false;
            treeControl.OptionsFilter.AllowAutoFilterConditionChange = DefaultBoolean.False;
            if (definition?.FilterMode == "default")
                treeControl.OptionsFilter.FilterMode = FilterMode.Default;
            else if (definition?.FilterMode == "smart")
                treeControl.OptionsFilter.FilterMode = FilterMode.Smart;
            else
                treeControl.OptionsFilter.FilterMode = FilterMode.Extended;

            treeControl.OptionsSelection.EnableAppearanceFocusedCell = false;
            //****
            //Control.Modifier + click; u suprotnom ctrl i shift služe za multiselect
            //defaultno ponašanje; kod edit drveta je vrijendost true; preko allowmultiselect možemo podesiti na false
            treeControl.OptionsSelection.MultiSelect = false;
            //****

            treeControl.OptionsView.EnableAppearanceOddRow = true;
            treeControl.OptionsView.ShowAutoFilterRow = true;
            treeControl.TabStop = false;
            this.AutoPopulate = definition?.AutoPopulate;
            if (definition == null)
                return;

            SetEvents(definition.Event);

            if (definition.ShowColumns == true)
                treeControl.OptionsView.ShowColumns = true;
            else
                treeControl.OptionsView.ShowColumns = false;

            SearchField = definition.SearchField;

            if (definition.AllowDrag == true)
            {
                AllowDrag = true;
                if (definition.AllowDragMultiple == false)
                    AllowDragMultiple = false;
            }

            if (definition.AllowSelfDrag == true)
                AllowSelfDrag = true;

            if (definition.AllowDrop == true)
            {
                AllowDrop = true;
                treeControl.AllowDrop = true;
            }

            if (definition.ShowAutoFilterRow == false)
                treeControl.OptionsView.ShowAutoFilterRow = false;

            // Opt-in DevExpress Find Panel (default off).
            if (definition.ShowFindPanel == true)
                SetAttributeFindPanel(true, definition.FindPanelMode);
            else if (definition.ShowFindPanel == false)
                SetAttributeFindPanel(false, null);

            if (definition.TabStop == true)
                treeControl.TabStop = true;

            treeControl.Columns.Clear();

            if (definition.Fields == null || definition.Fields.FieldList.Count == 0)
            {
                form.Logger.Warn("tree: '" + treeControl.Name + "'; 'fields' element", "Nije definirana niti jedna kolona");
                return;
            }

            foreach (JField field in definition.Fields.FieldList)
            {
                TreeListColumn col = new TreeListColumn()
                {
                    FieldName = field.Name,
                    Caption = !string.IsNullOrEmpty(field.Caption) ? field.Caption : field.Name,
                    Name = Guid.NewGuid().ToString(),
                    Visible = true
                };

                col.OptionsFilter.AllowFilter = false;

                if (!string.IsNullOrEmpty(field.AutoFilterCondition) && field.AutoFilterCondition == "contains")
                    col.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Contains;
                else
                    col.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Like;

                if (field.AllowSort == false)
                    col.OptionsColumn.AllowSort = false;
                else
                    col.OptionsColumn.AllowSort = true;

                if (field.AllowAutoFilter == false)
                    col.OptionsFilter.AllowAutoFilter = false;
                else
                    col.OptionsFilter.AllowAutoFilter = true;

                if (field.FormatString != null)
                {
                    col.Format.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.Format.FormatString = field.FormatString;
                }

                if (!string.IsNullOrEmpty(field.Tooltip))
                {
                    col.ToolTip = field.Tooltip;
                }

                treeControl.Columns.Add(col);
            }
            //form.Logger.InfoData("Init tree", definition.Name, startTime);
        }

        public override void SetEvents(List<JEvent> events)
        {
            treeControl.MouseDown += (s, e) =>
            {
                MouseDown(s, e);
            };
            treeControl.DoubleClick += (s, e) =>
            {
                DoubleClick(s, e);
            };

            treeControl.FilterNode += (s, e) =>
            {
                event_FilterNode(s, e);
            };

            treeControl.GetSelectImage += (s, e) =>
            {
                GetSelectImage(s, e);
            };

            treeControl.NodeCellStyle += (s, e) =>
            {
                NodeCellStyle(s, e);
            };

            //if (definition.AllowDrop == true)
            //{
            //treeControl.DragEnter += (s, e) =>
            //{
            //    DragEnter(s, e);
            //};

            treeControl.DragOver += (s, e) =>
            {
                DragOver(s, e);
            };

            //treeControl.CalcNodeDragImageIndex += (s, e) =>
            //{
            //    CalcNodeDragImageIndex(s, e);
            //};

            treeControl.DragDrop += (s, e) =>
            {
                DragDrop(s, e);
            };

            treeControl.BeforeDragNode += (s, e) =>
            {
                BeforeDragNode(s, e);
            };

            treeControl.DragLeave += (s, e) =>
                {
                    DropNode = null;
                    treeControl.Refresh();
                };
            //}

            treeControl.PopupMenuShowing += (s, e) =>
            {
                {
                    PopupMenuShowing(s, e);
                }
            };

            treeControl.ControlMenu.Closed += (s, e) =>
            {
                DropNode = null;
                treeControl.Refresh();
            };

            // B8: lazy children on expand (opt-in lazy="true")
            if (definition?.Lazy == true)
            {
                treeControl.BeforeExpand += Tree_BeforeExpand;
            }

            base.SetEvents(events);
        }

        private void DoubleClick(object s, EventArgs e)
        {
            
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var key = Control.ModifierKeys;
            DevExpress.XtraTreeList.TreeListHitInfo hi = treeControl.CalcHitInfo(e.Location);
            if (hi.Node == treeControl.FocusedNode)
            {
                FocusedNodeChanged(false);
                //PopulateDetailControl(true);
            }
        }

        #region DragDrop > Drop

        private TreeListNode GetDragNode(IDataObject data)
        {
            return (TreeListNode)data.GetData(typeof(TreeListNode));
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            if (Action == "")
                e.Effect = DragDropEffects.Move;
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            //if (Action == "")
            //{
            try
            {
                TreeListHitInfo hi = treeControl.CalcHitInfo(treeControl.PointToClient(new Point(e.X, e.Y)));

                if (hi.Node != null)
                {
                    //if (hi.Node.Level != 1)
                    //    Debugger.Break();

                    e.Effect = DragDropEffects.Move;

                    SelfDraggingArg = null;
                    SelfDraggingArg = ((TreeList)sender).GetDXDragEventArgs(e);

                    if (SelfDragging == false)
                        DropNode = hi.Node;
                    else
                    {
                        SelfDropNode = SelfDraggingArg.TargetNode;
                    }

                    bool? permission = null;

                    using (Logic.Events events = new Logic.Events(form))
                    {
                        JEvent jEventDragOver;
                        if (SelfDragging == true)
                            jEventDragOver = eventDragOverSelf;
                        else
                            jEventDragOver = eventDragOver;
                        object value = events.RunActions(jEventDragOver, "drop");
                        if (value is bool?)
                            permission = value as bool?;
                    }

                    //if (permission == false || (SelfDragging == true && hi.Node.Level == SelfDraggingArg.Node.Level))
                    if (permission == false)
                        e.Effect = DragDropEffects.None;
                    else if (SelfDragging == false) //samo za SelfDragging
                        e.Effect = DragDropEffects.Copy;

                    //treeControl.Refresh();
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: ComponentTree - DragOver", ex.Message, ex);
            }
            finally
            {
                if (SelfDragging == true)
                    SelfDropNode = null;
            }

            //}
        }

        private void DragDrop(object sender, DragEventArgs e)
        {
            if (Action == "" && SelfDragging == false)
            {
                //dropNode = null;
                TreeListHitInfo hi = treeControl.CalcHitInfo(treeControl.PointToClient(new Point(e.X, e.Y)));
                if (hi == null) return;

                if (hi.Node != null)
                {
                    Rectangle onDropNode = ((CellInfo)treeControl.ViewInfo.RowsInfo[hi.Node].Cells[0]).Bounds;
                    Point menuLocation = new Point(onDropNode.Left + 3, onDropNode.Bottom + 1);
                    menuLocation = treeControl.PointToScreen(menuLocation);
                    PopulateMenuItems("drop", "tree", true);
                    treeControl.ControlMenu.Show(menuLocation);
                }
                treeControl.Refresh();
            }
            if (SelfDragging == true)
            {
                SetSelfDragButtons(true);

                if (form is Forms.Master.Manager)
                {
                    foreach (var button in (form as Forms.Master.Manager).CrudButtons)
                        button.Enabled = false;
                }
            }
        }

        private void BeforeDragNode(object sender, BeforeDragNodeEventArgs e)
        {
            bool? canDrag = true;
            if (SelfDragging == true && eventBeforeDragNode != null)
            {
                using (Logic.Events events = new Logic.Events(form))
                {
                    object value = events.RunActions(eventBeforeDragNode, "drag");
                    if (value is bool?)
                        canDrag = value as bool?;
                }
            }
            if (canDrag != false)
                e.CanDrag = true;
            else
                e.CanDrag = false;
        }

        private void CalcNodeDragImageIndex(object sender, CalcNodeDragImageIndexEventArgs e)
        {
            if (e.DragArgs.Effect == DragDropEffects.None)
                e.ImageIndex = -1;
            //e.ImageIndex = 0;
            //e.ImageIndex -> 0 = -zelena ikona(in); 1 = plava ikona(before); 2 = plava ikona(after);
        }

        #endregion DragDrop > Drop

        #region Menu

        private void PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (treeControl.MenuItems == null)
                return;

            TreeListHitInfo hitInfo = treeControl.CalcHitInfo(e.Point);
            if (hitInfo.Node != null)
            {
                PopulateMenuItems("", "tree", true);
                treeControl.FocusedNode = hitInfo.Node;
                treeControl.ControlMenu.Show(treeControl, e.Point);
            }
        }

        #endregion Menu

        #region ExtendedFilter(keep child nodes visible)

        private void event_FilterNode(object sender, DevExpress.XtraTreeList.FilterNodeEventArgs e)
        {
            try
            {
                if (definition.FilterMode == "smart")
                {
                    if (treeControl.FocusedColumn == null ||
                    treeControl.FocusedColumn.FilterInfo.AutoFilterRowValue == null)
                        return;

                    string filterValue = treeControl.FocusedColumn.FilterInfo.AutoFilterRowValue.ToString();

                    TreeList tree = sender as TreeList;
                    if (string.IsNullOrEmpty(filterValue)) return;
                    e.Node.Visible = IsNodeVisible(e.Node, filterValue);
                    e.Handled = true;
                }
            }
            catch { }
        }

        private bool IsNodeVisible(TreeListNode node, string filterValue)
        {
            object val = node[0];
            if (val != null && val.ToString().ToLower().Contains(filterValue.ToLower()))
                return true;

            return IsAnyOfChildNodeVisible(node, filterValue);
        }

        private bool IsAnyOfChildNodeVisible(TreeListNode node, string filterValue)
        {
            if (node.ParentNode != null)
                if (node.ParentNode[0].ToString().ToLower().Contains(filterValue.ToLower()))
                    return true;

            if (!node.HasChildren)
                return false;

            foreach (TreeListNode n in node.Nodes)
            {
                object val = n[0];
                if (val != null && val.ToString().ToLower().Contains(filterValue.ToLower()))
                    return true;
            }

            return false;
        }

        #endregion ExtendedFilter(keep child nodes visible)

        private object GetMasterControlValue()
        {
            if (MasterBarEditControl is BarEditItem)
            {
                return ((BarEditItem)MasterBarEditControl).EditValue;
            }
            else if (MasterControl is JLookUpControl)
            {
                return ((JLookUpControl)MasterControl).EditValue;
            }
            return null;
        }

        public void PopulateTreeList(bool saveState, bool additionalFocus, bool useAsync = false)
        {
            // B8: opt-in background fetch (async="true" / populateasync). Integrated calendar tree stays sync.
            if (useAsync)
            {
                PopulateTreeListAsync(saveState, additionalFocus);
                return;
            }

            PopulateTreeListCore(saveState, additionalFocus, null);
        }

        private void PrepareLazyRootFetch()
        {
            if (definition?.Lazy != true)
                return;

            lazyLoadedKeys.Clear();
            FormsVar.SetVariable(form, "TreeParentId", DBNull.Value);
        }

        private async void PopulateTreeListAsync(bool saveState, bool additionalFocus)
        {
            bool error = false;
            bool splashShown = false;
            treeControl.BeginUpdate();
            try
            {
                if (treeControl.DataSource != null && saveState == true)
                    treeState.SaveState();

                Action = "populating";
                treeControl.ClearColumnsFilter();
                treeControl.DataSource = null;
                if (!string.IsNullOrWhiteSpace(definition.AsyncCaption))
                    splashShown = TryShowAsyncSplash(definition.AsyncCaption);
                else
                    treeControl.ShowLoadingPanel();

                PrepareLazyRootFetch();

                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);
                JData jData = dataDefinition.GetJData();
                if (jData != null && !string.IsNullOrEmpty(jData.Type) && jData.Type == "integrated")
                {
                    // Fall back to sync path for integrated months tree.
                    if (splashShown) TryCloseAsyncSplash();
                    else treeControl.HideLoadingPanel();
                    splashShown = false;
                    treeControl.EndUpdate();
                    PopulateTreeListCore(saveState, additionalFocus, null);
                    return;
                }

                bool stopActions = false;
                var sw = Stopwatch.StartNew();
                var t = Task.Factory.StartNew(() => dataDefinition.GetDataSet("punjenje drveta '" + definition.Name + "'", out stopActions));
                DataSet dataSet = await t;
                int rows = dataSet != null && dataSet.Tables.Count > 0 ? dataSet.Tables[0].Rows.Count : 0;
                form.Logger?.Performance("ComponentTree",
                    $"TreeFill name={definition.Name} rows={rows}", sw, "GridFill");
                ApplyTreeDataSet(dataSet, saveState, isIntegrated: false);
                Action = "";
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: Punjenje drveta (async)", ex.Message, ex);
                error = true;
            }
            finally
            {
                Action = "";
                if (splashShown)
                    TryCloseAsyncSplash();
                else
                {
                    try { treeControl.HideLoadingPanel(); } catch { }
                }
                treeControl.EndUpdate();
                Cursor.Current = Cursors.Default;
            }
            if (error == false)
                FocusedNodeChanged(additionalFocus);
        }

        private void PopulateTreeListCore(bool saveState, bool additionalFocus, DataSet prefetched)
        {
            bool error = false;

            treeControl.BeginUpdate();

            try
            {
                if (treeControl.DataSource != null && saveState == true)
                    treeState.SaveState();

                Action = "populating";
                Cursor.Current = Cursors.WaitCursor;

                treeControl.ClearColumnsFilter(); // dodao Ognjen
                treeControl.DataSource = null;

                PrepareLazyRootFetch();

                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);
                JData jData = dataDefinition.GetJData();
                bool isIntegrated = false;

                if (jData != null && !string.IsNullOrEmpty(jData.Type) && jData.Type == "integrated")
                    isIntegrated = true;

                if (isIntegrated == true)
                {
                    int yearFrom = DateTime.Now.Year - 4;
                    List<Structure.Models.Dates.TreeMonths> list = Common.Helpers.Dates.GetMonthsBetweenDates(new DateTime(yearFrom, 1, 1), new DateTime(DateTime.Now.Year, 12, 1), new string[] { Common.Helpers.Dates.Predef.jucer.ToString(), Common.Helpers.Dates.Predef.danas.ToString(), Common.Helpers.Dates.Predef.sutra.ToString(), Common.Helpers.Dates.Predef.ovajTjedan.ToString() });
                    if (list != null)
                    {
                        Structure.Models.Dates.TreeMonths helperObj = new Structure.Models.Dates.TreeMonths();
                        PropertyInfo[] properties = helperObj.GetType()
                            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic |
                                              BindingFlags.Instance).ToArray();
                        this.DataColumns = properties.Select(x => x.Name).ToArray();
                    }
                    treeControl.DataSource = list;
                    TreeListNode n = treeControl.FindNodeByFieldValue("Id", DateTime.Now.Year.ToString());
                    if (n != null)
                        n.Expanded = true;

                    TreeListNode focusedNode = treeControl.FindNodeByFieldValue("Name", "Danas");
                    treeControl.FocusedNode = focusedNode;
                }
                else
                {
                    DataSet dataSet = prefetched;
                    if (dataSet == null)
                    {
                        bool stopActions = false;
                        var sw = Stopwatch.StartNew();
                        dataSet = dataDefinition.GetDataSet("punjenje drveta '" + definition.Name + "'", out stopActions);
                        int rows = dataSet != null && dataSet.Tables.Count > 0 ? dataSet.Tables[0].Rows.Count : 0;
                        form.Logger?.Performance("ComponentTree",
                            $"TreeFill name={definition.Name} rows={rows}", sw, "GridFill");
                    }
                    ApplyTreeDataSet(dataSet, saveState, isIntegrated: false);
                }

                if (isIntegrated == true && treeControl.DataSource != null && saveState == true)
                    treeState.LoadState();

                Action = "";
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                form.Logger.Error("event: Punjenje drveta", ex.Message, ex);
                error = true;
                //JMessages.Error(ex.Message);
            }
            finally
            {
                Action = "";
                treeControl.EndUpdate();
                Cursor.Current = Cursors.Default;
            }
            if (error == false)
                FocusedNodeChanged(additionalFocus);
        }

        private void ApplyTreeDataSet(DataSet dataSet, bool saveState, bool isIntegrated)
        {
            if (isIntegrated)
                return;

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                string[] arrDtColumns = dataSet.Tables[0].Columns.Cast<DataColumn>()
                    .Select(x => x.ColumnName)
                    .ToArray();
                this.DataColumns = arrDtColumns;

                SetTree();
                treeControl.DataSource = dataSet.Tables[0];
                if (treeControl.Nodes.Count > 0)
                    treeControl.FocusedNode = treeControl.Nodes[0];
            }
            else
            {
                treeControl.DataSource = null;
            }

            ApplyTreeExpandAndLayout(saveState);
        }

        private void ApplyTreeExpandAndLayout(bool saveState)
        {
            // Lazy trees: never ExpandAll (children not loaded yet). Glyphs from haschildren column.
            if (definition?.Lazy == true)
            {
                treeControl.CollapseAll();
                ApplyLazyHasChildrenMarkers(treeControl.Nodes);
            }
            else if (definition.ExpandAllOnLoad == true || definition.ExpandedLevel != null)
                treeControl.ExpandAll();
            else
                treeControl.CollapseAll();

            if (definition?.Lazy != true && definition.ExpandedLevel != null)
            {
                CollapseToLevel collapser = new CollapseToLevel((int)definition.ExpandedLevel);
                treeControl.NodesIterator.DoOperation(collapser);
            }
            else if (definition?.Lazy != true && definition.CollapseAllOnLoad != true && treeControl.Nodes.Count > 0)
            {
                if (ParameterControlValue.ToLower().ToString() == "vremenskapodjela" || definition.ExpandLast == true)
                    treeControl.Nodes[treeControl.Nodes.Count - 1].ExpandAll();
                else if (definition.ExpandFirst == true)
                {
                    treeControl.FocusedNode = treeControl.Nodes[0];
                    treeControl.Nodes[0].ExpandAll();
                }
            }

            // ako je managerlayout u pitanju i ako je samo drvo na formi onda forsiramo bestfit dok god netko ne specificira baš columnautowidth="true"
            if (form is Forms.Master.Manager && (form as Forms.Master.Manager).gcGla.Visible == false)
            {
                if (definition.ColumnAutoWidth != true)
                {
                    SetBestFitColumns(treeControl);
                }
            }
            // layout forme ili managerlayout forme gdje nije u pitanju samo drvo - aktivacija pomoću atributa columnautowidth="false"
            else if (definition.ColumnAutoWidth == false)
            {
                SetBestFitColumns(treeControl);
            }

            if (treeControl.DataSource != null && saveState == true)
                treeState.LoadState();
        }

        private void SetTree()
        {
            if (!string.IsNullOrEmpty(definition.Key))
                this.keyField = Logic.Data.Core.FindDataColumn(definition.Key, DataColumns);

            if (string.IsNullOrEmpty(this.keyField))
                form.Logger.Warn("tree atribut 'KeyField' nije definiran", "Kontrola: " + treeControl.Name);

            if (!string.IsNullOrEmpty(definition.ValueField))
                this.valueField = Logic.Data.Core.FindDataColumn(definition.ValueField, DataColumns);
            else
                this.valueField = keyField;
            if (string.IsNullOrEmpty(Logic.Data.Core.FindDataColumn(this.valueField, DataColumns)))
                form.Logger.Warn("Ne postoji key kolona '" + this.valueField + "'", "Kontrola: " + treeControl.Name + ". Dodajte u query ili definirajte kroz atribut 'valuefield'");

            if (!string.IsNullOrEmpty(definition.ParentField))
                this.parentField = Logic.Data.Core.FindDataColumn(definition.ParentField, DataColumns);

            if (string.IsNullOrEmpty(Logic.Data.Core.FindDataColumn(this.parentField, DataColumns)))
                form.Logger.Warn("Ne postoji parent kolona '" + this.parentField + "'", "Kontrola: " + treeControl.Name + ". Dodajte u query ili definirajte kroz atribut 'parentfield'");

            treeControl.KeyFieldName = keyField;
            treeControl.ParentFieldName = parentField;

            formatIdField = Logic.Data.Core.FindDataColumn("formatid", DataColumns);
            rowForeColorIdField = Logic.Data.Core.FindDataColumn("rowforecolorid", DataColumns);
            rowColorIdField = Logic.Data.Core.FindDataColumn("rowcolorid", DataColumns);
            hasChildrenField = Logic.Data.Core.FindDataColumn("haschildren", DataColumns);
        }

        private static bool IsTruthyHasChildren(object value)
        {
            if (value == null || value == DBNull.Value)
                return false;
            if (value is bool b)
                return b;
            if (value is byte by)
                return by != 0;
            if (value is short s)
                return s != 0;
            if (value is int i)
                return i != 0;
            if (value is long l)
                return l != 0;
            if (bool.TryParse(value.ToString(), out bool parsedBool))
                return parsedBool;
            if (int.TryParse(value.ToString(), out int parsedInt))
                return parsedInt != 0;
            return false;
        }

        private void ApplyLazyHasChildrenMarkers(TreeListNodes nodes)
        {
            if (definition?.Lazy != true || nodes == null || string.IsNullOrEmpty(hasChildrenField))
                return;

            foreach (TreeListNode node in nodes)
            {
                if (node.Nodes.Count == 0 && IsTruthyHasChildren(node.GetValue(hasChildrenField)))
                    node.HasChildren = true;

                if (node.Nodes.Count > 0)
                    ApplyLazyHasChildrenMarkers(node.Nodes);
            }
        }

        private void Tree_BeforeExpand(object sender, BeforeExpandEventArgs e)
        {
            if (definition?.Lazy != true || e?.Node == null || lazyLoadingChildren)
                return;

            object key = e.Node.GetValue(keyField);
            if (key == null || key == DBNull.Value)
                return;

            if (lazyLoadedKeys.Contains(key))
                return;

            if (e.Node.Nodes.Count > 0)
            {
                lazyLoadedKeys.Add(key);
                return;
            }

            try
            {
                lazyLoadingChildren = true;
                FormsVar.SetVariable(form, "TreeParentId", key);

                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);
                bool stopActions = false;
                DataSet dataSet = dataDefinition.GetDataSet("lazy child '" + definition.Name + "'", out stopActions);

                DataTable bound = treeControl.DataSource as DataTable;
                if (bound == null || dataSet == null || dataSet.Tables.Count == 0)
                {
                    lazyLoadedKeys.Add(key);
                    e.Node.HasChildren = false;
                    return;
                }

                DataTable childTable = dataSet.Tables[0];
                int imported = 0;
                treeControl.BeginUpdate();
                try
                {
                    foreach (DataRow row in childTable.Rows)
                    {
                        object childKey = row[keyField];
                        if (childKey != null && childKey != DBNull.Value && treeControl.FindNodeByKeyID(childKey) != null)
                            continue;

                        bound.ImportRow(row);
                        imported++;
                    }

                    lazyLoadedKeys.Add(key);

                    if (imported == 0)
                        e.Node.HasChildren = false;
                    else
                        ApplyLazyHasChildrenMarkers(e.Node.Nodes);
                }
                finally
                {
                    treeControl.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: Lazy tree expand", ex.Message, ex);
                e.CanExpand = false;
            }
            finally
            {
                lazyLoadingChildren = false;
                FormsVar.SetVariable(form, "TreeParentId", DBNull.Value);
            }
        }

        private void GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)           // kad se naprave ikonice nove dodati u kod za 'jučer' i 'sutra' te obrisat komentar
        {
            if (e.Node.GetValue("Id") == null)
                return;

            if (e.Node.GetValue("Id").ToString() == Common.Helpers.Dates.Predef.danas.ToString())
                e.NodeImageIndex = 2;
            else if (e.Node.GetValue("Id").ToString() == Common.Helpers.Dates.Predef.ovajTjedan.ToString())
                e.NodeImageIndex = 3;
            else if (e.Node.GetValue("Id").ToString() == Common.Helpers.Dates.Predef.jucer.ToString())
                e.NodeImageIndex = 4;
            else if (e.Node.GetValue("Id").ToString() == Common.Helpers.Dates.Predef.sutra.ToString())
                e.NodeImageIndex = 5;
            else if (e.Node.HasChildren)
                e.NodeImageIndex = 1;
            else
                e.NodeImageIndex = 0;
        }

        public int GetNodeLevel(List<string> helperStr)
        {
            string helperStr1 = "";
            if (helperStr != null && helperStr.Count > 0)
                helperStr1 = helperStr[0];

            if (helperStr1 == "NodeCellStyle" && nodeCellStyle != null)
                return nodeCellStyle.Level;
            else if (SelfDragging == true && SelfDropNode != null)
            {
                int level = SelfDropNode.Level;
                if (SelfDraggingArg != null)
                {
                    if (SelfDraggingArg.DragInsertPosition == DragInsertPosition.AsChild)
                        return level + 1;
                    else
                        return level;
                }
                return -1;
            }
            else if (SelfDragging == false && DropNode != null)
                return DropNode.Level;
            else if (BeforeFocusing == true && BeforeFocusNode != null)
                return BeforeFocusNode.Level;
            else if (treeControl.FocusedNode != null)
                return treeControl.FocusedNode.Level;
            else
                return -1;
        }

        public object GetValue(string field, List<string> helperStr)
        {
            try
            {
                string helperStr1 = "";
                if (helperStr != null && helperStr.Count > 0)
                    helperStr1 = helperStr[0];

                TreeListNode node = null;

                if (helperStr1 == "NodeCellStyle" && nodeCellStyle != null)
                    node = nodeCellStyle;
                else if (SelfDragging == true && SelfDropNode != null)
                    node = SelfDropNode;
                else if (SelfDragging == false && DropNode != null)
                    node = DropNode;
                else if (BeforeFocusing == true && BeforeFocusNode != null)
                    node = BeforeFocusNode;
                else if (treeControl.FocusedNode != null && treeControl.FocusedNode != treeControl.Nodes.AutoFilterNode)
                    node = treeControl.FocusedNode;

                if (!string.IsNullOrEmpty(field))
                {
                    field = Logic.Data.Core.FindDataColumn(field, treeControl.ComponentExt.DataColumns);

                    if (treeControl.DataRowRef != null)
                        return treeControl.DataRowRef[field];
                    else if (node != null)
                        return node[field];
                }
                else if (node != null)
                {
                    return node.GetValue(valueField);
                    //return treeControl.FocusedNode.Id;
                }
            }
            catch { }

            // T.A. zakomentirao 06.06.2019. - ukoliko se radi o IF/ELSE provjeri prilikom dohvaćanja vrijednosti
            // tada bespotrebno nullira DropNode i zbog toga vraća focusedNode - linija 724 umjesto dropNoda
            //finally
            //{
            //DropNode = null;
            //}

            return null;
        }

        public void SetFocus(JEventAction action)
        {
            try
            {
                if (action.Parameter.Count > 0)
                {
                    ParseAndLocate(action, "");
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("SetFocus", ex.Message, ex);
            }
        }

        //private void LocateByValue(object value, string expand)
        //{
        //    LocateByValue(valueField, value, expand);
        //}

        private void LocateByValue(string field, object value, string helperStr)
        {
            TreeListNode node = null;
            if (!string.IsNullOrEmpty(field))
                node = treeControl.FindNodeByFieldValue(field, value);

            if (node != null)
                treeControl.FocusedNode = node;
            else if (helperStr == "searchDoc")
            {
                string[] valueArray = (value.ToString()).Split('_');
                string newValue = valueArray[valueArray.Length - 1];

                node = treeControl.FindNodeByFieldValue(field, newValue);
                if (node != null)
                    treeControl.FocusedNode = node;
            }

            if (helperStr == "focus&expand")
                locatedNode = node;
        }

        #region SelfReordering

        //public void SetSelfReorderderButton(JButtonControl button)
        //{
        //    button.Enabled = false;
        //    button.Click += (s, e) =>
        //    {
        //        SetSelfDragButtons(false);
        //    };
        //}

        //public void SetSelfReorderderCancelButton(JButtonControl button)
        //{
        //    button.Enabled = false;
        //    button.Click += (s, e) =>
        //    {
        //        SetSelfDragButtons(false);
        //    };
        //}

        private void SetSelfDragButtons(bool enableDragButtons)
        {
            if (form is Forms.Master.Manager)
            {
                foreach (var b in (form as Forms.Master.Manager).CrudButtons)
                    b.Enabled = !enableDragButtons;
                foreach (var b in (form as Forms.Master.Manager).SelfDragButtons)
                    b.Enabled = enableDragButtons;
            }
            foreach (TreeListColumn col in treeControl.Columns)
                col.OptionsColumn.AllowSort = !enableDragButtons;
        }

        #region GetDataSource - SelfDragDataTable

        public DataTable GetDataSource()
        {
            selfDragDataTable = new DataTable();
            selfDragDataTable.Columns.Add(new DataColumn("Id", typeof(Int32)));
            selfDragDataTable.Columns.Add(new DataColumn("ParentId", typeof(Int32)));
            selfDragDataTable.Columns.Add(new DataColumn("Sort", typeof(Int32)));
            for (int i = 0; i <= treeControl.Nodes.Count - 1; i++)
            {
                CreateDataRow(treeControl.Nodes[i], i + 1);
                if (treeControl.Nodes[i].HasChildren)
                    NodetIterate(treeControl.Nodes[i]);
            }
            return selfDragDataTable;
        }

        private void NodetIterate(TreeListNode node)
        {
            for (int i = 0; i <= node.Nodes.Count - 1; i++)
            {
                CreateDataRow(node.Nodes[i], i);
                if (node.Nodes[i].HasChildren)
                    NodetIterate(node.Nodes[i]);
            }
        }

        private void CreateDataRow(TreeListNode node, int sort)
        {
            DataRow drNew = selfDragDataTable.NewRow();
            drNew["Id"] = node[valueField];
            drNew["ParentId"] = node[parentField];
            drNew["Sort"] = sort;
            selfDragDataTable.Rows.Add(drNew);
        }

        #endregion GetDataSource - SelfDragDataTable

        #endregion SelfReordering

        public void SetMode(string mode)
        {
            if (string.IsNullOrEmpty(mode))
                return;

            mode = mode.ToLower();
            if (mode.StartsWith("drag"))
                SetSelfDragButtons(true);
            else
                SetSelfDragButtons(false);
        }

        public void PrintControl(bool selected)
        {
            using (var cls = new Common.Helpers.Printing())
            {
                cls.PrintControl(treeControl);
            }
        }

        #region Style

        private void NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            SetCellStyle(sender, e);

            if (DropNode != null)
            {
                if (e.Node == DropNode)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    e.Appearance.ForeColor = Color.FromArgb(0, 100, 0);
                }
            }
        }

        private void SetCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                nodeCellStyle = e.Node;

                if (treeControl.FocusedNode != null && definition.FocusedBold != false)
                    if (e.Node.Id == treeControl.FocusedNode.Id)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        e.Appearance.FontSizeDelta = 1;
                        e.Appearance.ForeColor = Color.DarkBlue;
                    }

                bool checkIfResult = true;

                if (BeforeFocusing == true && SetDisabledRowCellStyle(e.Appearance))
                    return;

                //***begin netGen styles
                if (!string.IsNullOrEmpty(formatIdField))
                    SetCellStyle(e.Appearance, e.Node, formatIdField, "format");

                if (!string.IsNullOrEmpty(rowColorIdField))
                    SetCellStyle(e.Appearance, e.Node, rowColorIdField, "backgroundcolor");

                if (!string.IsNullOrEmpty(rowForeColorIdField))
                    SetCellStyle(e.Appearance, e.Node, rowForeColorIdField, "fontcolor");
                //***end netGen styles

                if (definition.Styles != null && definition.Styles.StyleList.Count > 0)
                {
                    foreach (JStyle style in definition.Styles.StyleList)
                    {
                        if (Conditions.CheckCondition(ref checkIfResult, style, form, "NodeCellStyle", false) == false)
                            continue;

                        Logic.Style.SetStyle(e.Appearance, style.Name, style.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: SetStyle (tree: '" + treeControl.Name + "')", ex.Message, ex);
            }
        }

        private bool SetDisabledRowCellStyle(AppearanceObject appearanceObject)
        {
            bool checkIfResult = true;

            if (Conditions.CheckCondition(ref checkIfResult, DisableNodeEvent.Actions[0], form, "NodeCellStyle", false) == false)
                return false;

            Logic.Style.SetStyle(appearanceObject, "fontcolor", "darkgray");
            return true;
        }

        private void SetCellStyle(AppearanceObject appearanceObject, TreeListNode node, string fieldName, string styleType)
        {
            var value = node.GetValue(fieldName);
            if (value != null)
                Logic.Style.ParseNetGenStyles(appearanceObject, value.ToString(), styleType);
        }

        #endregion Style

        public object GetValueSpecial(string name, List<string> helperStr)
        {
            string helperStr1 = "";
            if (helperStr != null && helperStr.Count > 0)
                helperStr1 = helperStr[0];

            name = name.Trim().ToLower();

            if (name == "level")
                return treeControl.GetNodeLevel(helperStr).ToString();
            else if (name == "searchfield")
                return SearchField;
            else if (name == "datasource")
            {
                return treeControl.GetDataSource();
            }

            return null;
        }

        public void SearchDoc(DataRow dr)
        {
            try
            {
                TreeListNode focusedNode = treeControl.FocusedNode;

                form.Action = "searchdoc";
                form.SearchDataRow = dr;
                LocateByValue(valueField, dr[SearchField], "searchDoc");

                //ako je fokus drveta isti, ne ulazi se u repopulate pa treba ručno pokrenuti searchdoc proceduru
                if (focusedNode == treeControl.FocusedNode)
                    DetailControl.SearchDoc();
            }
            catch (Exception ex)
            {
                form.Logger.Error("tree: SearchDoc", ex.Message, ex);
            }
            finally
            {
                form.SearchDataRow = null;
                form.Action = "";
            }
        }

        public void SetExpand(JEventAction action)
        {
            if (action == null)
                return;

            int level = -1;
            bool keyExist = false, levelExist = false;

            if (action.Parameter.Count > 0)
            {
                foreach (JEventActionParameter par in action.Parameter)
                {
                    if (par.Name.ToLower() == "key")
                        keyExist = ParseAndLocate(action, "focus&expand");
                    else if (par.Name.ToLower() == "level")
                    {
                        int tempLevel;
                        bool isNumeric = int.TryParse(par.Value, out tempLevel);
                        if (isNumeric == false)
                            form.Logger.Warn("SetExpand", "Parametar 'level' mora biti cjelobrojna vrijednost!");
                        else
                        {
                            level = tempLevel;
                            levelExist = true;
                        }
                    }
                }

                if (keyExist == true)
                {
                    if (locatedNode.HasChildren)
                    {
                        if (levelExist == false)
                            locatedNode.ExpandAll();
                        else
                        {
                            if (level > 0)
                            {
                                locatedNode.Expanded = true;
                                CollapseNode(locatedNode, locatedNode.Level + level);
                            }
                        }
                    }
                }
                else
                    form.Logger.Warn("SetExpand", "Parametar 'key' nije (dobro) definiran!");
            }
            else
                form.Logger.Warn("SetExpand", "Nije definiran niti jedan parametar!");
        }

        private void CollapseNode(TreeListNode parent, int level)
        {
            IEnumerator en = parent.Nodes.GetEnumerator();
            TreeListNode child;
            while (en.MoveNext())
            {
                child = (TreeListNode)en.Current;

                if (child.Level < level)
                    child.Expanded = true;
                if (child.HasChildren)
                    CollapseNode(child, level);
            }
        }

        private bool ParseAndLocate(JEventAction action, string expand)
        {
            using (Parser parser = new Parser(form, null))
            {
                string outValueStr = "";
                object value = Carpo.UI.Logic.Parameters.GetValue(form, null, action, out outValueStr);
                if (value != null)
                    LocateByValue(valueField, value, expand);

                if (locatedNode != null)
                    return true;
            }
            return false;
        }

        private void SetBestFitColumns(TreeList treelist)
        {
            treelist.ForceInitialize();
            treelist.OptionsView.AutoWidth = false;
            treelist.BestFitColumns();
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

            treeControl.OptionsFind.AllowFindPanel = enabled;
            treeControl.OptionsFind.AlwaysVisible = always;
            if (enabled)
            {
                treeControl.OptionsFind.HighlightFindResults = true;
                treeControl.OptionsFind.ShowClearButton = true;
                if (always)
                    treeControl.ShowFindPanel();
                else
                    treeControl.HideFindPanel();
            }
            else
            {
                treeControl.HideFindPanel();
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
                treeControl.ShowLoadingPanel();
                return false;
            }
        }

        private void TryCloseAsyncSplash()
        {
            try { form.splashScreenManager1.CloseWaitForm(); }
            catch { }
        }
    }

    internal class CollapseToLevel : TreeListOperation
    {
        private int lvl = -1;

        public CollapseToLevel(int level)
        {
            lvl = level;
        }

        public override void Execute(DevExpress.XtraTreeList.Nodes.TreeListNode node)
        {
            node.Expanded = node.Level != lvl;
        }
    }
}
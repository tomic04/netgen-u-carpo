using Carpo.Core;
using Carpo.Data.Get;
using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.Structure.Models.Core.Base;
using Carpo.UI.Components;
using Carpo.UI.Components.Container;
using Carpo.UI.Helpers;
using Carpo.UI.Logic;
using Carpo.UI.Properties;
using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.TextEditController.Win32;
using DevExpress.XtraRichEdit.Model.History;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Carpo.UI.Forms
{
    public partial class Info : Base.JupiterInfo
    {
        JInfoControls infoControlsDefinition;
        JAutoSuggest autoSuggestDefinition;
        JDataNavigator dataNavigatorDefinition;
        JInfo infoDefinition;
        JTabs tabsDefinition;
        JButton newButtonDefinition;
        SwapAnimator animator;
        enum FormType
        {
            InfoMain,
            InfoEdit
        }

        FormType formType;


        DataTable dataNavigatorDataTable;


        int currentEditValueChangedIdValue = -1;
        int historyCurrentIdValue = -1;

        List<Common.Models.InfoHistory> lstHistory = new List<Common.Models.InfoHistory>();


        public enum SearchTypeEnums
        {
            Default,
            Popup
        }
        public SearchTypeEnums SearchType;



        enum FormAction
        {
            Nothing,
            Search_EditValueChanged,
            PositionChenged,
            HistoryChanged
        }

        FormAction currentAction;

        FormAction currentActionLevel2;

        public Info()
        {
            InitializeComponent();
        }

        public Info(JRoot definitionRoot, JForm definition, Logger logger, string connectionString, string moduleId) : base(definitionRoot, definition, logger, connectionString, moduleId)
        {
            InitializeComponent();

            animator = new SwapAnimator(500, 20);
            animator.Init += Animator_Init;
            animator.Completed += Animator_Completed;
            this.DoubleBuffered = true;

            infoControlsDefinition = definitionRoot.Form[0].InfoControls;
            autoSuggestDefinition = definitionRoot.Form[0].InfoControls.AutoSuggest;
            dataNavigatorDefinition = definitionRoot.Form[0].InfoControls.DataNavigator;
            infoDefinition = definitionRoot.Form[0].InfoControls.Info;
            tabsDefinition = definitionRoot.Form[0].InfoControls.Tabs;

            tabs.JTabsInit = tabsDefinition;

            newButtonDefinition = definitionRoot.Form[0].InfoControls.NewButton;
            cmdNew.JButtonInit = newButtonDefinition;

            SetControlAppValues();
            SetSearchType();


            if (SearchType == SearchTypeEnums.Popup)
            {
                src.Name = "srcHidden";
                cmdSwitchSearch.ImageOptions.SvgImage = svgSearch["Detaljna-black-50"];
                srcPopup.Name = autoSuggestDefinition.Name;
                srcPopup.BringToFront();

            }
            else
            {
                srcPopup.Name = "srcHidden";
                cmdSwitchSearch.ImageOptions.SvgImage = svgSearch["Brza-black-50"];
                src.Name = autoSuggestDefinition.Name;
                src.BringToFront();
            }


            typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(src.Parent, true, null);

            typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(srcPopup.Parent, true, null);

            InfoHistoryGet();
            //var historylastid = lstHistory.Last().Id;
            //VariablesGlobal.Add("historylastid", historylastid);


            this.ControlList.Add(new Structure.Models.JControl() { ControlName = definition.Name, Control = cmdNew, ControlType = cmdNew.GetType(), IsNameDefinied = true, SaveValueSession = false, SaveValueApp = false });
            SetNotification(newButtonDefinition, cmdNew);

            this.ControlList.Add(new Structure.Models.JControl() { ControlName = definition.Name, Control = tabs, ControlType = tabs.GetType(), IsNameDefinied = true, SaveValueSession = false, SaveValueApp = false });
            SetNotification(tabsDefinition, tabs);


            //JAutoSuggestControl autoSuggest = new JAutoSuggestControl(autoSuggestDefinition);
            //autoSuggest.


            srcPopup.JSearchEditInit = autoSuggestDefinition;

            SetNotification(autoSuggestDefinition, srcPopup);


            src.JAutoSuggestInit = autoSuggestDefinition;

            SetNotification(autoSuggestDefinition, src);


            //if (searchType == SearchType.Popup)
            //{
            //    srcPopup.Visible = true;
            //    src.Visible= false;
            //    srcPopup.JSearchEditInit = autoSuggestDefinition;
            //    srcPopup.Name = autoSuggestDefinition.Name;
            //    SetNotification(autoSuggestDefinition, srcPopup);
            //} else
            //{
            //    src.Visible = true;
            //    srcPopup.Visible = false;
            //    src.JAutoSuggestInit = autoSuggestDefinition;
            //    src.Name = autoSuggestDefinition.Name;
            //    SetNotification(autoSuggestDefinition, src);
            //}

            FormsVar.SetVariable(this, "SelectedTabIndex", 0);
            //pnlButtons.Visible = false;
        }

        private void SetSearchType()
        {
            var searchType = CarpoVarsApp.FirstOrDefault(x => x.FormName == this.FormName && x.Name == "srcInfo");
            if (searchType != null && !string.IsNullOrEmpty(searchType.ValueText))
            {
                if (searchType.ValueText.ToString().ToLower() == "popup")
                {
                    SearchType = SearchTypeEnums.Popup;
                    return;
                }
                else if (searchType.ValueText.ToString().ToLower() == "default")
                {
                    SearchType = SearchTypeEnums.Default;
                    return;
                }
            }

            if (infoControlsDefinition.SearchType == "popup")
            {
                SearchType = SearchTypeEnums.Popup;
            }
            else
            {
                SearchType = SearchTypeEnums.Default;
            }

        }

        private void SetNotification(JModelEditorDataBase definition, System.Windows.Forms.Control control)
        {
            if (definition == null)
                return;

            if (definition.Event != null && definition.Event.Count > 0)
                this.EventList.Add(new Structure.Models.ControlEvents() { Control = control, Events = definition.Event });
        }

        public void PopulateNav(bool force)
        {
            if (force == false && dataNavigatorDataTable != null)
                return;

            Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(this, dataNavigatorDefinition);
            bool stopActions = false;
            DataSet ds = dataDefinition.GetDataSet("punjenje search-nav kontrole '" + infoDefinition.Name + "'", out stopActions);

            dataNavigatorDataTable = ds.Tables[0];
            dataNavigator1.DataSource = dataNavigatorDataTable;
        }

        static Func<string, string, DisplayTextHighlightRange[]> HighlightTags(string pattern)
        {
            var indexOf = IgnoreCaseComparisonFunctions.GetIndexOf(CultureInfo.CurrentCulture.CompareInfo, CompareOptions.IgnoreCase);
            var parts = pattern.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return (displayText, fieldName) =>
            {
                var tags = displayText.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                var ranges = new List<DisplayTextHighlightRange>();
                for (int i = 0; i < tags.Length; i++)
                {
                    int tagStart = displayText.IndexOf(tags[i]);
                    for (int j = 0; j < parts.Length; j++)
                    {
                        int index = indexOf(tags[i], parts[j]);
                        if (index != -1)
                            ranges.Add(new DisplayTextHighlightRange(tagStart + index, parts[j].Length));
                    }
                }
                return ranges.ToArray();
            };
        }

        private void jAutoSuggestControl1_AutoSuggest(object sender, LookUpEditAutoSuggestEventArgs e)
        {
            // Set delay (if needed)
            if (currentAction == FormAction.Search_EditValueChanged)
            {
                e.SetMinimumAnimationDuration(TimeSpan.FromMilliseconds(0));
            }
            else
            {
                e.SetMinimumAnimationDuration(TimeSpan.FromMilliseconds(1000));
            }
            // Set Custom Highlight Strategy
            e.SetHighlightRanges(HighlightTags(e.Text));
            // Create a Task that return suggestions
            e.QuerySuggestions = QueryAsync(e.Text, e.CancellationToken);


            //SetData(ds);
        }


        public Task<ICollection> QueryAsync(string text, CancellationToken cancellation)
        {
            var contains = IgnoreCaseComparisonFunctions.GetContains(CultureInfo.CurrentCulture.CompareInfo, CompareOptions.IgnoreCase);
            return Task.Run(new Func<ICollection>(() =>
            {
                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(this, autoSuggestDefinition);
                bool stopActions = false;
                DataSet ds = dataDefinition.GetDataSet("punjenje search kontrole '" + autoSuggestDefinition.Name + "'", out stopActions);


                return ds.Tables[0].AsDataView() as ICollection;



                //var entries = CsvDataHelper.EnsureAllEntries();
                //var parts = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //if (parts.Length == 0)
                //    return entries as ICollection;
                //else
                //    return entries.Where(e =>
                //    {
                //        // stop task if needed
                //        cancellation.ThrowIfCancellationRequested();
                //        return parts.All(x => e.Tags.Any(tag => contains(tag, x)));
                //    }).ToList();
            }));
        }

        private void src_EditValueChanged(object sender, EventArgs e)
        {
            if (currentAction == FormAction.Search_EditValueChanged)
                return;

            if (src.EditValue == null)
            {
                lblInfoText.Text = "...";
                return;
            }

            if (!(src.EditValue is DataRowView))
            {
                return;
            }

            if (currentActionLevel2 == FormAction.PositionChenged)
            {
                timerHistory.Stop();
                timerHistory.Start();
            }
            else
            {
                timerHistory.Stop();
            }

            currentAction = FormAction.Search_EditValueChanged;
            try
            {
                int id;
                if (src.EditValue is Int32)
                {
                    id = (int)src.EditValue;
                }
                else
                {
                    id = Convert.ToInt32((src.EditValue as DataRowView).Row[0]);
                }

                srcPopup.EditValue = src.Text;
                srcPopup.ComponentExt.Id = id;


                if (currentActionLevel2 != FormAction.PositionChenged)
                {
                    historyCurrentIdValue = id;
                }

                if (currentActionLevel2 != FormAction.HistoryChanged && currentActionLevel2 != FormAction.PositionChenged)
                {
                    SetInfoHistory(id);
                }


                if (sender == null && dataNavigator1.Position > 0 && currentEditValueChangedIdValue == id)
                {
                    return;
                }

                currentEditValueChangedIdValue = id;

                //src.Text = (src.EditValue as DataRowView).Row[1].ToString();
                //lblInfoText.Text = (src.EditValue as DataRowView).Row[1].ToString();


                var rows = dataNavigatorDataTable.AsEnumerable()
                   .Where(r => r.Field<int>("Id") == id);

                DataRow[] foundRows = dataNavigatorDataTable.Select(String.Format("Id={0}", id));

                int position = dataNavigatorDataTable.Rows.IndexOf(foundRows[0]);
                if (sender == null)
                {
                    currentAction = FormAction.Nothing;
                }

                lblInfoText.Text = foundRows[0][1].ToString();

                dataNavigator1.Position = position;
            }
            catch
            {
            }
            finally
            {
                currentAction = FormAction.Nothing;
            }
        }



        private void SetInfoHistory(int id)
        {
            //TODO: možda uključiti da ne blokira ako slučajno bude grešaka kod spreanja u history; 
            //try
            //{
            InfoHistoryAzur(id);
            InfoHistoryGet();
            InfoHistorySetControls(id);
            //}
            //catch
            //{}
        }

        private void InfoHistoryAzur(int id)
        {
            Data.Set.InfoHistory.Azur(infoControlsDefinition.Entity, id, Carpo.Core.PubVars.OperateriId);
        }

        private void InfoHistoryGet()
        {
            DataTable dt = Data.Get.InfoHistory.Get(infoControlsDefinition.Entity, Carpo.Core.PubVars.OperateriId);
            //konvertiraj datatable u listu radi lakše manipulacije
            lstHistory = (from DataRow row in dt.Rows
                          select new Common.Models.InfoHistory
                          {
                              Id = (int)row["Id"],
                              Naziv = row["Naziv"].ToString(),
                              VrijemeUnosa = (DateTime)row["VrijemeUnosa"]

                          }).ToList();


            popupMenu1.ClearLinks();
            foreach (var item in lstHistory)
            {
                BarButtonItem menuItem = new BarButtonItem();
                menuItem.Caption = item.Naziv;
                menuItem.Id = item.Id;
                menuItem.ItemClick += HistoryMenu_ItemClick;
                menuItem.ImageOptions.SvgImage = Resources.paymentrefund;

                popupMenu1.AddItem(menuItem);
            }
        }

        private void InfoHistorySetControls(int id)
        {
            if (id < 0)
            {
                id = currentEditValueChangedIdValue;
            }

            //nađi trenutno index
            int currentIndex = lstHistory.FindIndex(x => x.Id == id);
            if (currentIndex == 0)
            {
                cmdHistoryLeft.Enabled = false;
            }
            else
            {
                cmdHistoryLeft.Enabled = true;
            }

            //currentEditValueChangedIdValue je manji od 0 kod inicijalnog učitavanja -- uvijek je right disejblan
            if (id < 0 || currentIndex == lstHistory.Count - 1)
            {
                cmdHistoryRight.Enabled = false;
            }
            else
            {
                cmdHistoryRight.Enabled = true;
            }


            //if (lstHistory.Count > 1)
            //{
            //    cmdHistoryLeft.Enabled = true;
            //}
        }


        private void cmdHistoryLeft_Click(object sender, EventArgs e)
        {
            HistoryButtonClick(-1);
        }

        private void cmdHistoryRight_Click(object sender, EventArgs e)
        {
            HistoryButtonClick(1);
        }

        private void HistoryButtonClick(int valueChange)
        {
            try
            {
                currentActionLevel2 = FormAction.HistoryChanged;

                //nađi index od trenutnog zapisa
                int currentIndex = lstHistory.FindIndex(x => x.Id == historyCurrentIdValue);
                var entity = lstHistory[currentIndex + valueChange];


                //podesi googlesearch
                string displayMember = "Naziv";
                if (src.Visible == true)
                {
                    displayMember = src.Properties.DisplayMember;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add(displayMember);
                DataRow dr = dt.NewRow();
                dr["Id"] = entity.Id;
                dr[displayMember] = entity.Naziv;
                dt.Rows.Add(dr);

                //src.Properties.DisplayMember = dt.Columns[1].ColumnName;

                src.Properties.DataSource = dt;


                DataRowView drv = dt.DefaultView[0];
                src.EditValue = drv;


                //podesi popupsearch
                srcPopup.EditValue = entity.Naziv;
                srcPopup.ComponentExt.Id = entity.Id;


                if (src.Visible == true)
                {

                }
                else
                {

                }


                InfoHistorySetControls(-1);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                currentActionLevel2 = FormAction.Nothing;
            }
        }


        private void tabs_Click(object sender, EventArgs e)
        {

        }

        private void Info_Load(object sender, EventArgs e)
        {
            if (dataNavigatorDataTable != null)
                return;

            PopulateNav(false);
            //cmdNew.Appearance.Font = new System.Drawing.Font(cmdNew.Font.FontFamily, 9F, System.Drawing.FontStyle.Bold);

            //HistorMenuItemClick(11871);

            dataNavigator1.TextStringFormat = "{1}";
            dataNavigator1.Enabled = false;
        }

        private void dataNavigator1_PositionChanged(object sender, EventArgs e)
        {
            DataNavigatorPositionChanged();
        }

        private void DataNavigatorPositionChanged()
        {
            //return;
            if (currentAction == FormAction.Search_EditValueChanged)
            {
                return;
            }
            currentActionLevel2 = FormAction.PositionChenged;
            try
            {
                DataRow dr = dataNavigatorDataTable.Rows[dataNavigator1.Position];
                if (dr != null)
                {

                    DataTable dt = dataNavigatorDataTable.Clone();
                    dt.ImportRow(dr);


                    //podesi googlesearch
                    src.Properties.DataSource = dt;
                    DataRowView drv = dt.DefaultView[0];
                    src.EditValue = drv;


                    //podesi popupsearch
                    srcPopup.EditValue = dt.Rows[0][1];
                    srcPopup.ComponentExt.Id = dt.Rows[0][0];


                    //if (src.Visible)
                    //{

                    //}
                    //else
                    //{

                    //}
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                currentActionLevel2 = FormAction.Nothing;
            }
        }


        private void src_MouseUp(object sender, MouseEventArgs e)
        {
            ((BaseEdit)sender).SelectAll();
        }

        private void Info_Shown(object sender, EventArgs e)
        {


            tabs.SelectedTabPageIndex = 0;

            if (TabPagesEnabled.Count > 0 || TabPagesVisible.Count > 0)
            {
                for (int i = 0; i < tabs.TabPages.Count; i++)
                {
                    if (TabPagesVisible.Contains(i))
                    {
                        tabs.TabPages[i].PageVisible = false;
                    }
                    else if (TabPagesEnabled.Contains(i))
                    {
                        tabs.TabPages[i].PageEnabled = false;
                    }
                }
            }




            FormShown = true;
            pnlSpace.Visible = true;
            pnlSpace.Width = tabs.PageClientBounds.X;

            tabs.RunActions = true;

            if (currentEditValueChangedIdValue == -1 && lstHistory.Count > 0)
            {
                HistorMenuItemClick(lstHistory.Last().Id);
            }


        }

        private void src_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (e.OldValue == null)
            {
                dataNavigator1.TextStringFormat = "{0} / {1}";
                dataNavigator1.Enabled = true;
            }
        }

        private void tabs_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (FormShown == false)
                return;
            FormsVar.SetVariable(this, "SelectedTabIndex", tabs.SelectedTabPageIndex);
        }

        private void cmdHistoryLeft_MouseUp(object sender, MouseEventArgs e)
        {
            HistoryMenuShow(sender, e);
        }

        private void cmdHistoryRight_MouseUp(object sender, MouseEventArgs e)
        {
            HistoryMenuShow(sender, e);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            HistoryMenuShow(sender, e);
        }

        private void HistoryMenuShow(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                popupMenu1.ShowPopup((sender as System.Windows.Forms.Control).PointToScreen(e.Location));
            }
        }

        private void HistoryMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            HistorMenuItemClick(e.Item.Id);
        }

        private void HistorMenuItemClick(int id)
        {
            try
            {
                currentActionLevel2 = FormAction.HistoryChanged;

                //nađi index od trenutnog zapisa
                int index = lstHistory.FindIndex(x => x.Id == id);
                var entity = lstHistory[index];


                //podesi googlesarch
                string displayMember = "Naziv";
                if (src.Visible == true)
                {
                    displayMember = src.Properties.DisplayMember;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add(displayMember);
                DataRow dr = dt.NewRow();
                dr["Id"] = entity.Id;
                dr[displayMember] = entity.Naziv;
                dt.Rows.Add(dr);

                src.Properties.DataSource = dt;

                DataRowView drv = dt.DefaultView[0];
                src.EditValue = drv;


                if (srcPopup.ComponentExt.Id.ToString() != entity.Id.ToString())
                {
                    //podesi popup search
                    srcPopup.EditValue = entity.Naziv;
                    srcPopup.ComponentExt.Id = entity.Id;
                }


                //if (src.Visible)
                //{

                //}
                //else
                //{

                //}

                InfoHistorySetControls(-1);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                currentActionLevel2 = FormAction.Nothing;
            }
        }


        private void timerHistory_Tick(object sender, EventArgs e)
        {
            SetInfoHistory(currentEditValueChangedIdValue);
            historyCurrentIdValue = currentEditValueChangedIdValue;
            timerHistory.Stop();
        }


        public void Repopulate()
        {
            PopulateNav(true);



            if (src.Visible == true)
            {
                PopulateNav(false);
                //src_EditValueChanged(null, null);
            }

            //SrcPopupEditValueChanged(true);
            //DataNavigatorPositionChanged();
        }


        public void SrcPopupEditValueChanged(bool force)
        {
            if (currentAction == FormAction.Search_EditValueChanged)
                return;

            var id = srcPopup.ComponentExt.Id;

            if (id == null)
            {
                lblInfoText.Text = "...";
                return;
            }

            dataNavigator1.TextStringFormat = "{0} / {1}";
            dataNavigator1.Enabled = true;


            if (currentActionLevel2 == FormAction.PositionChenged)
            {
                timerHistory.Stop();
                timerHistory.Start();
            }
            else
            {
                timerHistory.Stop();
            }

            currentAction = FormAction.Search_EditValueChanged;
            try
            {

                if (currentActionLevel2 != FormAction.PositionChenged)
                {
                    historyCurrentIdValue = (int)id;
                }

                if (currentActionLevel2 != FormAction.HistoryChanged && currentActionLevel2 != FormAction.PositionChenged)
                {
                    SetInfoHistory((int)id);
                }


                //*****podesi googlesarch
                string displayMember = "Naziv";
                if (src.Visible == true)
                {
                    displayMember = src.Properties.DisplayMember;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add(displayMember);
                DataRow dr = dt.NewRow();
                dr["Id"] = id;
                dr[displayMember] = srcPopup.Text;
                dt.Rows.Add(dr);

                src.Properties.DataSource = dt;

                DataRowView drv = dt.DefaultView[0];
                src.EditValue = drv;
                //*****


                if (force == false && dataNavigator1.Position > 0 && currentEditValueChangedIdValue == (int)id)
                {
                    lblInfoText.Text = srcPopup.Text;
                    return;
                }

                currentEditValueChangedIdValue = (int)id;

                lblInfoText.Text = srcPopup.Text;

                var rows = dataNavigatorDataTable.AsEnumerable()
                   .Where(r => r.Field<int>("Id") == (int)id);

                DataRow[] foundRows = dataNavigatorDataTable.Select(String.Format("Id={0}", id));

                int position = dataNavigatorDataTable.Rows.IndexOf(foundRows[0]);
                dataNavigator1.Position = position;
            }
            catch
            {
            }
            finally
            {
                currentAction = FormAction.Nothing;
            }
        }

        private void cmdNew_Click(object sender, EventArgs e)
        {

        }
        System.Windows.Forms.Control parent;
        private void cmdSwitchSearch_Click(object sender, EventArgs e)
        {
            animator.Swap(src, srcPopup);
        }
        private void Animator_Completed()
        {
            this.SuspendLayout();

            var p1 = src.PointToScreen(src.Location);
            var p2 = srcPopup.PointToScreen(srcPopup.Location);

            int zA = src.Parent.Controls.GetChildIndex(src);
            int zB = srcPopup.Parent.Controls.GetChildIndex(srcPopup);

            src.Parent = parent;
            srcPopup.Parent = parent;

            src.Location = src.PointToClient(p1);
            srcPopup.Location = srcPopup.PointToClient(p2);

            src.Dock = DockStyle.Fill;
            srcPopup.Dock = DockStyle.Fill;

            if (zA > zB)
            {
                src.BringToFront();
                srcPopup.BringToFront();
            }
            else
            {
                srcPopup.BringToFront();
                src.BringToFront();
            }

            if (SearchType == SearchTypeEnums.Popup)
            {
                cmdSwitchSearch.ImageOptions.SvgImage = svgSearch["Brza-black-50"];
                SearchType = SearchTypeEnums.Default;
                //src.BringToFront();
            }
            else
            {
                cmdSwitchSearch.ImageOptions.SvgImage = svgSearch["Detaljna-black-50"];
                SearchType = SearchTypeEnums.Popup;
                //srcPopup.BringToFront();
            }

            this.ResumeLayout(false);
            string sqlText;
            Carpo.Data.Set.Vars.SetAppControlValues(this.ConnectionString, Core.PubVars.OperateriId, this.ModuleId, this.FormName, "srcInfo",
                            null, "string", SearchType.ToString(), out sqlText);
        }

        private void Animator_Init()
        {
            this.SuspendLayout();

            var sizeA = src.Size;
            var sizeB = srcPopup.Size;

            src.Dock = DockStyle.None;
            srcPopup.Dock = DockStyle.None;


            src.Size = sizeA;
            srcPopup.Size = sizeB;

            var p1 = src.PointToScreen(src.Location);
            var p2 = srcPopup.PointToScreen(srcPopup.Location);

            parent = src.Parent;


            int zA = src.Parent.Controls.GetChildIndex(src);
            int zB = srcPopup.Parent.Controls.GetChildIndex(srcPopup);

            src.Parent = this;
            srcPopup.Parent = this;

            src.Location = src.PointToClient(p1);
            srcPopup.Location = srcPopup.PointToClient(p2);


            if (zA > zB)
            {

                src.BringToFront();
                srcPopup.BringToFront();
            }
            else
            {
                srcPopup.BringToFront();
                src.BringToFront();
            }
            this.ResumeLayout(false);
        }
    }
}


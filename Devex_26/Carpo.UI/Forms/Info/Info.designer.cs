using Carpo.UI.Components;

namespace Carpo.UI.Forms
{
    partial class Info
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Carpo.UI.Components.Base.ComponentSearh componentSearh1 = new Carpo.UI.Components.Base.ComponentSearh();
            Carpo.UI.Components.Base.ComponentAutoSuggest componentAutoSuggest1 = new Carpo.UI.Components.Base.ComponentAutoSuggest();
            Carpo.UI.Components.Base.ComponentButton componentButton1 = new Carpo.UI.Components.Base.ComponentButton();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Info));
            Carpo.UI.Components.Base.ComponentButton componentButton2 = new Carpo.UI.Components.Base.ComponentButton();
            Carpo.UI.Components.Base.ComponentTabs componentTabs1 = new Carpo.UI.Components.Base.ComponentTabs();
            this.pnlSearch = new DevExpress.XtraEditors.PanelControl();
            this.panel5 = new System.Windows.Forms.Panel();
            this.srcPopup = new Carpo.UI.Components.JSearchControl();
            this.src = new Carpo.UI.Components.JAutoSuggestControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmdSwitchSearch = new Carpo.UI.Components.JButtonControl();
            this.panel6 = new System.Windows.Forms.Panel();
            this.dataNavigator1 = new DevExpress.XtraEditors.DataNavigator();
            this.panel7 = new System.Windows.Forms.Panel();
            this.cmdNew = new Carpo.UI.Components.JButtonControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdHistoryLeft = new DevExpress.XtraEditors.SimpleButton();
            this.cmdHistoryRight = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
            this.tabs = new Carpo.UI.Components.Container.JXtraTabControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pnlInfoText = new DevExpress.XtraEditors.PanelControl();
            this.lblInfoText = new DevExpress.XtraEditors.LabelControl();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            this.menuHistory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.popupMenu1 = new DevExpress.XtraBars.PopupMenu(this.components);
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.svgSearch = new DevExpress.Utils.SvgImageCollection(this.components);
            this.timerHistory = new System.Windows.Forms.Timer(this.components);
            this.svgSearch = new DevExpress.Utils.SvgImageCollection(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSearch)).BeginInit();
            this.pnlSearch.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.srcPopup.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.src.Properties)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
            this.panelControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlInfoText)).BeginInit();
            this.pnlInfoText.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.menuHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlSpace
            // 
            this.pnlSpace.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(35, 37, 35, 37);
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1});
            this.ribbonControl1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ribbonControl1.MaxItemId = 5;
            this.ribbonControl1.OptionsMenuMinWidth = 243;
            this.ribbonControl1.Size = new System.Drawing.Size(1107, 50);
            this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
            // 
            // pnlSearch
            // 
            this.pnlSearch.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlSearch.Controls.Add(this.panel5);
            this.pnlSearch.Controls.Add(this.panel2);
            this.pnlSearch.Controls.Add(this.panel6);
            this.pnlSearch.Controls.Add(this.panel7);
            this.pnlSearch.Controls.Add(this.panel1);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 50);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(1107, 31);
            this.pnlSearch.TabIndex = 8;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.srcPopup);
            this.panel5.Controls.Add(this.src);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(57, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(3);
            this.panel5.Size = new System.Drawing.Size(756, 31);
            this.panel5.TabIndex = 4;
            // 
            // srcPopup
            // 
            componentSearh1.Action = "";
            componentSearh1.Action2 = "";
            componentSearh1.ActionEvent = "";
            componentSearh1.DataColumns = new string[0];
            componentSearh1.DefaultVisibility = null;
            componentSearh1.DisplayText = null;
            componentSearh1.form = null;
            componentSearh1.Id = null;
            componentSearh1.PermissionEnabled = null;
            componentSearh1.PermissionVisible = null;
            componentSearh1.PreventTabChanging = false;
            componentSearh1.SelectedDataRow = null;
            this.srcPopup.ComponentExt = componentSearh1;
            this.srcPopup.DataRowRef = null;
            this.srcPopup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.srcPopup.JSearchEditInit = null;
            this.srcPopup.Location = new System.Drawing.Point(3, 3);
            this.srcPopup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.srcPopup.MenuManager = this.ribbonControl1;
            this.srcPopup.Name = "srcPopup";
            this.srcPopup.Properties.AutoHeight = false;
            this.srcPopup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.srcPopup.Size = new System.Drawing.Size(750, 25);
            this.srcPopup.TabIndex = 1;
            // 
            // src
            // 
            componentAutoSuggest1.Action = "";
            componentAutoSuggest1.Action2 = "";
            componentAutoSuggest1.ActionEvent = "";
            componentAutoSuggest1.DataColumns = new string[0];
            componentAutoSuggest1.DefaultVisibility = null;
            componentAutoSuggest1.form = null;
            componentAutoSuggest1.PermissionEnabled = null;
            componentAutoSuggest1.PermissionVisible = null;
            componentAutoSuggest1.PreventTabChanging = false;
            componentAutoSuggest1.ValueFieldColumnType = null;
            this.src.ComponentExt = componentAutoSuggest1;
            this.src.DataRowRef = null;
            this.src.Dock = System.Windows.Forms.DockStyle.Fill;
            this.src.JAutoSuggestInit = null;
            this.src.Location = new System.Drawing.Point(3, 3);
            this.src.MenuManager = this.ribbonControl1;
            this.src.Name = "src";
            this.src.Properties.AutoHeight = false;
            this.src.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.src.Properties.DisplayMember = "Naziv";
            this.src.Properties.NullText = "";
            this.src.Properties.NullValuePrompt = "Tražilica... po nazivu, šifri, matičnom broju, žiroračunu";
            this.src.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoSuggest;
            this.src.Properties.ShowHeader = false;
            this.src.Size = new System.Drawing.Size(750, 25);
            this.src.TabIndex = 0;
            this.src.AutoSuggest += new DevExpress.XtraEditors.Controls.LookUpEditAutoSuggestEventHandler(this.jAutoSuggestControl1_AutoSuggest);
            this.src.EditValueChanged += new System.EventHandler(this.src_EditValueChanged);
            this.src.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.src_EditValueChanging);
            this.src.MouseUp += new System.Windows.Forms.MouseEventHandler(this.src_MouseUp);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cmdSwitchSearch);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(813, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.panel2.Size = new System.Drawing.Size(31, 31);
            this.panel2.TabIndex = 7;
            // 
            // cmdSwitchSearch
            // 
            this.cmdSwitchSearch.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
            this.cmdSwitchSearch.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.cmdSwitchSearch.Appearance.Options.UseFont = true;
            this.cmdSwitchSearch.Appearance.Options.UseTextOptions = true;
            this.cmdSwitchSearch.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Word;
            this.cmdSwitchSearch.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            componentButton1.Action = "";
            componentButton1.Action2 = "";
            componentButton1.ActionEvent = "";
            componentButton1.DefaultVisibility = null;
            componentButton1.form = null;
            componentButton1.PermissionEnabled = null;
            componentButton1.PermissionVisible = null;
            componentButton1.PreventTabChanging = false;
            componentButton1.SearchText = null;
            this.cmdSwitchSearch.ComponentExt = componentButton1;
            this.cmdSwitchSearch.DataRowRef = null;
            this.cmdSwitchSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdSwitchSearch.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
            this.cmdSwitchSearch.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.cmdSwitchSearch.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.cmdSwitchSearch.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("cmdSwitchSearch.ImageOptions.SvgImage")));
            this.cmdSwitchSearch.ImageOptions.SvgImageSize = new System.Drawing.Size(20, 20);
            this.cmdSwitchSearch.JButtonInit = null;
            this.cmdSwitchSearch.Location = new System.Drawing.Point(0, 3);
            this.cmdSwitchSearch.Name = "cmdSwitchSearch";
            this.cmdSwitchSearch.Size = new System.Drawing.Size(28, 25);
            this.cmdSwitchSearch.TabIndex = 2;
            this.cmdSwitchSearch.ToolTip = "Promijeni vrstu tražilice";
            this.cmdSwitchSearch.Click += new System.EventHandler(this.cmdSwitchSearch_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.dataNavigator1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel6.Location = new System.Drawing.Point(844, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.panel6.Size = new System.Drawing.Size(190, 31);
            this.panel6.TabIndex = 5;
            // 
            // dataNavigator1
            // 
            this.dataNavigator1.Buttons.Append.Visible = false;
            this.dataNavigator1.Buttons.CancelEdit.Visible = false;
            this.dataNavigator1.Buttons.EndEdit.Visible = false;
            this.dataNavigator1.Buttons.NextPage.Visible = false;
            this.dataNavigator1.Buttons.PrevPage.Visible = false;
            this.dataNavigator1.Buttons.Remove.Visible = false;
            this.dataNavigator1.CustomButtons.AddRange(new DevExpress.XtraEditors.NavigatorCustomButton[] {
            new DevExpress.XtraEditors.NavigatorCustomButton(0, -1),
            new DevExpress.XtraEditors.NavigatorCustomButton(7, -1)});
            this.dataNavigator1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataNavigator1.Location = new System.Drawing.Point(0, 3);
            this.dataNavigator1.Name = "dataNavigator1";
            this.dataNavigator1.Size = new System.Drawing.Size(187, 25);
            this.dataNavigator1.TabIndex = 0;
            this.dataNavigator1.Text = "dataNavigator1";
            this.dataNavigator1.TextLocation = DevExpress.XtraEditors.NavigatorButtonsTextLocation.Center;
            this.dataNavigator1.TextStringFormat = "{0} / {1}";
            this.dataNavigator1.PositionChanged += new System.EventHandler(this.dataNavigator1_PositionChanged);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.cmdNew);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel7.Location = new System.Drawing.Point(1034, 0);
            this.panel7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel7.Name = "panel7";
            this.panel7.Padding = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.panel7.Size = new System.Drawing.Size(73, 31);
            this.panel7.TabIndex = 6;
            // 
            // cmdNew
            // 
            this.cmdNew.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
            this.cmdNew.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.cmdNew.Appearance.Options.UseFont = true;
            this.cmdNew.Appearance.Options.UseTextOptions = true;
            this.cmdNew.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Word;
            this.cmdNew.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            componentButton2.Action = "";
            componentButton2.Action2 = "";
            componentButton2.ActionEvent = "";
            componentButton2.DefaultVisibility = null;
            componentButton2.form = null;
            componentButton2.PermissionEnabled = null;
            componentButton2.PermissionVisible = null;
            componentButton2.PreventTabChanging = false;
            componentButton2.SearchText = null;
            this.cmdNew.ComponentExt = componentButton2;
            this.cmdNew.DataRowRef = null;
            this.cmdNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdNew.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
            this.cmdNew.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.cmdNew.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.cmdNew.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("cmdNew.ImageOptions.SvgImage")));
            this.cmdNew.ImageOptions.SvgImageSize = new System.Drawing.Size(20, 20);
            this.cmdNew.JButtonInit = null;
            this.cmdNew.Location = new System.Drawing.Point(0, 3);
            this.cmdNew.Name = "cmdNew";
            this.cmdNew.Size = new System.Drawing.Size(70, 25);
            this.cmdNew.TabIndex = 2;
            this.cmdNew.ToolTip = "Dodaj";
            this.cmdNew.Click += new System.EventHandler(this.cmdNew_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdHistoryLeft);
            this.panel1.Controls.Add(this.cmdHistoryRight);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(57, 31);
            this.panel1.TabIndex = 3;
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // cmdHistoryLeft
            // 
            this.cmdHistoryLeft.Enabled = false;
            this.cmdHistoryLeft.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.cmdHistoryLeft.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("cmdHistoryLeft.ImageOptions.SvgImage")));
            this.cmdHistoryLeft.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.cmdHistoryLeft.Location = new System.Drawing.Point(0, 2);
            this.cmdHistoryLeft.Margin = new System.Windows.Forms.Padding(0);
            this.cmdHistoryLeft.Name = "cmdHistoryLeft";
            this.cmdHistoryLeft.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.cmdHistoryLeft.Size = new System.Drawing.Size(22, 26);
            this.cmdHistoryLeft.TabIndex = 1;
            this.cmdHistoryLeft.Click += new System.EventHandler(this.cmdHistoryLeft_Click);
            this.cmdHistoryLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cmdHistoryLeft_MouseUp);
            // 
            // cmdHistoryRight
            // 
            this.cmdHistoryRight.Enabled = false;
            this.cmdHistoryRight.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
            this.cmdHistoryRight.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("cmdHistoryRight.ImageOptions.SvgImage")));
            this.cmdHistoryRight.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            this.cmdHistoryRight.Location = new System.Drawing.Point(27, 2);
            this.cmdHistoryRight.Margin = new System.Windows.Forms.Padding(0);
            this.cmdHistoryRight.Name = "cmdHistoryRight";
            this.cmdHistoryRight.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.cmdHistoryRight.Size = new System.Drawing.Size(22, 26);
            this.cmdHistoryRight.TabIndex = 2;
            this.cmdHistoryRight.Click += new System.EventHandler(this.cmdHistoryRight_Click);
            this.cmdHistoryRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cmdHistoryRight_MouseUp);
            // 
            // panelControl4
            // 
            this.panelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl4.Controls.Add(this.tabs);
            this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl4.Location = new System.Drawing.Point(0, 107);
            this.panelControl4.Name = "panelControl4";
            this.panelControl4.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.panelControl4.Size = new System.Drawing.Size(1107, 649);
            this.panelControl4.TabIndex = 10;
            // 
            // tabs
            // 
            this.tabs.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.tabs.AppearancePage.Header.ForeColor = System.Drawing.Color.DimGray;
            this.tabs.AppearancePage.Header.Options.UseForeColor = true;
            this.tabs.AppearancePage.HeaderActive.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tabs.AppearancePage.HeaderActive.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.tabs.AppearancePage.HeaderActive.Options.UseFont = true;
            this.tabs.AppearancePage.HeaderActive.Options.UseForeColor = true;
            componentTabs1.Action = "";
            componentTabs1.Action2 = "";
            componentTabs1.ActionEvent = "";
            componentTabs1.DataColumns = new string[0];
            componentTabs1.DefaultVisibility = null;
            componentTabs1.form = null;
            componentTabs1.PermissionEnabled = null;
            componentTabs1.PermissionVisible = null;
            componentTabs1.PreventTabChanging = false;
            componentTabs1.ValueFieldColumnType = null;
            this.tabs.ComponentExt = componentTabs1;
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Left;
            this.tabs.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Horizontal;
            this.tabs.JTabsInit = null;
            this.tabs.Location = new System.Drawing.Point(8, 8);
            this.tabs.MultiLine = DevExpress.Utils.DefaultBoolean.False;
            this.tabs.Name = "tabs";
            this.tabs.Padding = new System.Windows.Forms.Padding(4);
            this.tabs.RunActions = false;
            this.tabs.Size = new System.Drawing.Size(1091, 637);
            this.tabs.TabIndex = 5;
            this.tabs.TabNameHelper = null;
            this.tabs.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabs_SelectedPageChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 106);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1107, 1);
            this.panel3.TabIndex = 13;
            // 
            // pnlInfoText
            // 
            this.pnlInfoText.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlInfoText.Controls.Add(this.lblInfoText);
            this.pnlInfoText.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlInfoText.Location = new System.Drawing.Point(0, 81);
            this.pnlInfoText.Name = "pnlInfoText";
            this.pnlInfoText.Size = new System.Drawing.Size(1107, 25);
            this.pnlInfoText.TabIndex = 16;
            // 
            // lblInfoText
            // 
            this.lblInfoText.Appearance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblInfoText.Appearance.Options.UseFont = true;
            this.lblInfoText.Appearance.Options.UseTextOptions = true;
            this.lblInfoText.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblInfoText.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblInfoText.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblInfoText.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblInfoText.Location = new System.Drawing.Point(0, 0);
            this.lblInfoText.Name = "lblInfoText";
            this.lblInfoText.Padding = new System.Windows.Forms.Padding(3);
            this.lblInfoText.Size = new System.Drawing.Size(1107, 24);
            this.lblInfoText.TabIndex = 0;
            this.lblInfoText.Text = "...";
            // 
            // menuHistory
            // 
            this.menuHistory.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuHistory.Name = "menuHistory";
            this.menuHistory.Size = new System.Drawing.Size(181, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // popupMenu1
            // 
            this.popupMenu1.ItemLinks.Add(this.barButtonItem1);
            this.popupMenu1.Name = "popupMenu1";
            this.popupMenu1.Ribbon = this.ribbonControl1;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "test";
            this.barButtonItem1.Id = 4;
            this.barButtonItem1.Name = "barButtonItem1";
            //
            // svgImageCollection
            //
            this.svgSearch.Add("Brza-black-50", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImageCollection.Brza-black-50"))));
            this.svgSearch.Add("Detaljna-black-50", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImageCollection.Detaljna-black-50"))));
            // 
            // timerHistory
            // 
            this.timerHistory.Interval = 15000;
            this.timerHistory.Tick += new System.EventHandler(this.timerHistory_Tick);
            // 
            // svgSearch
            // 
            this.svgSearch.Add("Brza-black-50", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgSearch.Brza-black-50"))));
            this.svgSearch.Add("Detaljna-black-50", ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgSearch.Detaljna-black-50"))));
            // 
            // Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 824);
            this.Controls.Add(this.panelControl4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.pnlInfoText);
            this.Controls.Add(this.pnlSearch);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("Info.IconOptions.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3);
            this.Name = "Info";
            this.Ribbon = null;
            this.Text = "Info";
            this.Load += new System.EventHandler(this.Info_Load);
            this.Shown += new System.EventHandler(this.Info_Shown);
            this.Controls.SetChildIndex(this.ribbonControl1, 0);
            this.Controls.SetChildIndex(this.pnlSearch, 0);
            this.Controls.SetChildIndex(this.pnlInfoText, 0);
            this.Controls.SetChildIndex(this.panel3, 0);
            this.Controls.SetChildIndex(this.panelControl4, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSearch)).EndInit();
            this.pnlSearch.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.srcPopup.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.src.Properties)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
            this.panelControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlInfoText)).EndInit();
            this.pnlInfoText.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.menuHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgSearch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.PanelControl pnlSearch;
        private DevExpress.XtraEditors.SimpleButton cmdHistoryRight;
        private DevExpress.XtraEditors.SimpleButton cmdHistoryLeft;
        private DevExpress.XtraEditors.DataNavigator dataNavigator1;
        private DevExpress.XtraEditors.PanelControl panelControl4;
        private System.Windows.Forms.Panel panel3;
        private DevExpress.XtraEditors.PanelControl pnlInfoText;
        private DevExpress.XtraEditors.LabelControl lblInfoText;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        internal Components.Container.JXtraTabControl tabs;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel6;
        private Components.JAutoSuggestControl src;
        private System.Windows.Forms.Panel panel7;
        private JButtonControl cmdNew;
        private System.Windows.Forms.ContextMenuStrip menuHistory;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.PopupMenu popupMenu1;
        private System.Windows.Forms.Timer timerHistory;
        private JSearchControl srcPopup;
        private System.Windows.Forms.Panel panel2;
        private JButtonControl cmdSwitchSearch;
        private DevExpress.Utils.SvgImageCollection svgSearch;
    }
}
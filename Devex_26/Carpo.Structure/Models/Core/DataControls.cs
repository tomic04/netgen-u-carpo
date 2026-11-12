using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JTree : Base.JModelDataControlsBase
    {
        [XmlAttribute("parentfield")]
        public string ParentField { get; set; }

        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("keyfield")]
        public string KeyField
        {
            set { Key = value; }
        }

        [XmlAttribute("filtermode")]
#pragma warning disable CS0108 // 'JTree.FilterMode' hides inherited member 'JModelEditorDataBase.FilterMode'. Use the new keyword if hiding was intended.
        public string FilterMode { get; set; }

#pragma warning restore CS0108 // 'JTree.FilterMode' hides inherited member 'JModelEditorDataBase.FilterMode'. Use the new keyword if hiding was intended.

        //[XmlAttribute("valuefield")]
        //public string ValueField { get; set; }

        [XmlIgnore]
        public bool? ExpandAllOnLoad { get; set; }

        [XmlIgnore]
        public bool? CollapseAllOnLoad { get; set; }

        [XmlIgnore]
        public bool? ExpandFirst { get; set; }

        [XmlIgnore]
        public bool? ExpandLast { get; set; }

        // Dodao Ognjen
        [XmlIgnore]
        public bool? ShowAutoFilterRow { get; set; }

        [XmlIgnore]
        public bool? ShowFindPanel { get; set; }

        /// <summary>Wave12: always (default for findpanel=true) | allow (Ctrl+F, not pinned).</summary>
        [XmlIgnore]
        public string FindPanelMode { get; set; }

        /// <summary>Opt-in background SQL fetch for full tree populate (B8). Default sync.</summary>
        [XmlIgnore]
        public bool? Async { get; set; }

        /// <summary>Wave12: optional loading caption when async="true".</summary>
        [XmlIgnore]
        public string AsyncCaption { get; set; }

        /// <summary>Opt-in lazy children on expand (B8). Uses $TreeParentId; keep async for full-tree bg fetch.</summary>
        [XmlIgnore]
        public bool? Lazy { get; set; }

        //

        [XmlIgnore]
        public bool? ShowColumns { get; set; }

        [XmlIgnore]
        public bool? FocusedBold { get; set; }

        [XmlIgnore]
        public int? ExpandedLevel { get; set; }

        [XmlElement("styles")]
        public JStyles Styles { get; set; }

        [XmlAttribute("expandallonload")]
        public string ExpandAllOnLoadStr
        {
            get { return (ExpandAllOnLoad.HasValue) ? ExpandAllOnLoad.ToString() : null; }
            set { ExpandAllOnLoad = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("collapseallonload")]
        public string CollapseAllOnLoadStr
        {
            get { return (CollapseAllOnLoad.HasValue) ? CollapseAllOnLoad.ToString() : null; }
            set { CollapseAllOnLoad = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("expandfirst")]
        public string ExpandFirstStr
        {
            get { return (ExpandFirst.HasValue) ? ExpandFirst.ToString() : null; }
            set { ExpandFirst = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("expandlast")]
        public string ExpandLastStr
        {
            get { return (ExpandLast.HasValue) ? ExpandLast.ToString() : null; }
            set { ExpandLast = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showautofilterrow")]
        public string ShowAutoFilterRowStr
        {
            get { return (ShowAutoFilterRow.HasValue) ? ShowAutoFilterRow.ToString() : null; }
            set { ShowAutoFilterRow = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("findpanel")]
        public string ShowFindPanelStr
        {
            get
            {
                if (ShowFindPanel == false) return "false";
                if (ShowFindPanel != true) return null;
                return string.Equals(FindPanelMode, "allow", System.StringComparison.OrdinalIgnoreCase) ? "allow" : "true";
            }
            set { DxUxAttr.ParseFindPanel(value, out var show, out var mode); ShowFindPanel = show; FindPanelMode = mode; }
        }

        [XmlAttribute("async")]
        public string AsyncStr
        {
            get { return (Async.HasValue) ? Async.ToString() : null; }
            set { Async = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("asynccaption")]
        public string AsyncCaptionStr
        {
            get { return AsyncCaption; }
            set { AsyncCaption = value; }
        }

        [XmlAttribute("lazy")]
        public string LazyStr
        {
            get { return (Lazy.HasValue) ? Lazy.ToString() : null; }
            set { Lazy = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showcolumns")]
        public string ShowColumnsStr
        {
            get { return (ShowColumns.HasValue) ? ShowColumns.ToString() : null; }
            set { ShowColumns = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("expandedlevel")]
        public string ExpandedLevelStr
        {
            get { return (ExpandedLevel.HasValue) ? ExpandedLevel.ToString() : null; }
            set { ExpandedLevel = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("focusedbold")]
        public string FocusedBoldStr
        {
            get { return (FocusedBold.HasValue) ? FocusedBold.ToString() : null; }
            set { FocusedBold = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }

    public class JGrid : Base.JModelDataControlsBase
    {
        [XmlAttribute("autorefresh")]
        public int AutoRefresh { get; set; } = 0;
        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlIgnore]
        public bool? Editable { get; set; }

        [XmlIgnore]
        public bool? MultiSelect { get; set; }

        [XmlIgnore]
        public bool? ForceRestoreLayout { get; set; }

        [XmlIgnore]
        public bool? ShowCheckBoxes { get; set; }

        public bool? SelectionWebStyle { get; set; }

        [XmlIgnore]
        public bool? EnableAppearanceOddRow { get; set; }

        [XmlIgnore]
        public bool? ShowGroupPanel { get; set; }

        // Dodao Ognjen
        [XmlIgnore]
        public bool? ShowAutoFilterRow { get; set; }

        [XmlIgnore]
        public bool? ShowFindPanel { get; set; }

        /// <summary>Wave12: always (default for findpanel=true) | allow (Ctrl+F, not pinned).</summary>
        [XmlIgnore]
        public string FindPanelMode { get; set; }

        /// <summary>Wave12: message when grid has zero rows (CustomDrawEmptyForeground).</summary>
        [XmlIgnore]
        public string EmptyMessage { get; set; }

        [XmlIgnore]
        public bool? AllowFilterEditor { get; set; }

        [XmlIgnore]
        public bool? AllowEditForm { get; set; }

        /// <summary>editform | editforminplace | inplace (used when AllowEditForm is set / editform attr).</summary>
        [XmlIgnore]
        public string EditFormMode { get; set; }

        /// <summary>Opt-in DX Column Chooser polish (search box + quick hide). Default off.</summary>
        [XmlIgnore]
        public bool? AllowColumnChooser { get; set; }

        //

        [XmlIgnore]
        public bool? ShowFooter { get; set; }

        [XmlIgnore]
        public bool? SaveLayout { get; set; }

        [XmlAttribute("cleargridfilter")]
        public string ClearGridFilter { get; set; }

        [XmlIgnore]
        public bool? ShowGroupSum { get; set; } //prikazivanje sume grupe -- default null (true) ako na field nije postavljen atribut showsum="false"

        [XmlIgnore]
        public bool? ShowTotalSum { get; set; } //prikazivanje sume grupe -- default null (true) ako na field nije postavljen atribut showsum="false"

        [XmlIgnore]
        public bool? WordWrap { get; set; }

        [XmlIgnore]
        public bool? BestFitOnRefresh { get; set; }

        [XmlIgnore]
        public bool? AllowCopyCell { get; set; }
        [XmlIgnore]
        public bool? KeepSelectionAfterFilter { get; set; }
        [XmlIgnore]
        public bool? ResetFocus { get; set; }

        [XmlIgnore]
        public bool? Async { get; set; }

        /// <summary>Wave12: optional loading caption when async="true".</summary>
        [XmlIgnore]
        public string AsyncCaption { get; set; }

        /// <summary>B6 opt-in VirtualServerMode (paging SP). Default null/false — DataTable path unchanged.</summary>
        [XmlIgnore]
        public bool? VirtualServerMode { get; set; }

        [XmlIgnore]
        public bool? NetGenFields { get; set; }

        [XmlIgnore]
        public bool? NetGenSizes { get; set; }

        [XmlIgnore]
        public bool? AllowFilter { get; set; }

        [XmlIgnore]
        public bool? NetGenStyles { get; set; }

        [XmlIgnore]
        public bool? EnterMoveNextEditor { get; set; }

        [XmlIgnore]
        public bool? FieldSmartSpaces { get; set; }

        [XmlIgnore]
        public bool? ShowGroupedColumns { get; set; }

        [XmlIgnore]
        public bool? ExpandAllOnLoad { get; set; }

        [XmlIgnore]
        public bool? PreventFocusGroup { get; set; }

        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("bestfitmaxrowcount")]
        public string BestFitMaxRowCount { get; set; }

        [XmlAttribute("selectedtildaids")]
        public string SelectedTildaIds { get; set; }

        [XmlAttribute("printcaption")]
        public string PrintCaption { get; set; }

        [XmlAttribute("printcaptionalign")]
        public string PrintCaptionAlign { get; set; }

        [XmlAttribute("printorientation")]
        public string PrintOrientation { get; set; }

        [XmlAttribute("keyfield")]
        public string KeyField
        {
            get { return Key; }
            set { Key = value; }
        }

        [XmlAttribute("previewfield")]
        public string PreviewField { get; set; }

        [XmlElement("styles")]
        public JStyles Styles { get; set; }

        [XmlElement("groupsummary")]
        public JGroupSummary GroupSummary { get; set; }

        [XmlElement("print")]
        public JPrint Print { get; set; }

        [XmlAttribute("editable")]
        public string EditableStr
        {
            get { return (Editable.HasValue) ? Editable.ToString() : null; }
            set { Editable = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("enableappearanceoddrow")]
        public string EnableAppearanceOddRowStr
        {
            get { return (EnableAppearanceOddRow.HasValue) ? EnableAppearanceOddRow.ToString() : null; }
            set { EnableAppearanceOddRow = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("multiselect")]
        public string MultiSelectStr
        {
            get { return (MultiSelect.HasValue) ? MultiSelect.ToString() : null; }
            set { MultiSelect = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("forcerestorelayout")]
        public string ForceRestoreLayoutStr
        {
            get { return (ForceRestoreLayout.HasValue) ? ForceRestoreLayout.ToString() : null; }
            set { ForceRestoreLayout = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("preventfocusgroup")]
        public string PreventFocusGroupStr
        {
            get { return (PreventFocusGroup.HasValue) ? PreventFocusGroup.ToString() : null; }
            set { PreventFocusGroup = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("expandallonload")]
        public string ExpandAllOnLoadStr
        {
            get { return (ExpandAllOnLoad.HasValue) ? ExpandAllOnLoad.ToString() : null; }
            set { ExpandAllOnLoad = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showcheckboxes")]
        public string ShowCheckBoxesStr
        {
            get { return (ShowCheckBoxes.HasValue) ? ShowCheckBoxes.ToString() : null; }
            set { ShowCheckBoxes = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("selectionwebstyle")]
        public string SelectionWebStyleStr
        {
            get { return (SelectionWebStyle.HasValue) ? SelectionWebStyle.ToString() : null; }
            set { SelectionWebStyle = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("bestfitonrefresh")]
        public string BestFitOnRefreshStr
        {
            get { return (BestFitOnRefresh.HasValue) ? BestFitOnRefresh.ToString() : null; }
            set { BestFitOnRefresh = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("allowcopycell")]
        public string AllowCopyCellStr
        {
            get { return (AllowCopyCell.HasValue) ? AllowCopyCell.ToString() : null; }
            set { AllowCopyCell = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("resetfocus")]
        public string ResetFocusStr
        {
            get { return (ResetFocus.HasValue) ? ResetFocus.ToString() : null; }
            set { ResetFocus = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("keepselectionafterfilter")]
        public string KeepSelectionAfterFilterStr
        {
            get { return (KeepSelectionAfterFilter.HasValue) ? KeepSelectionAfterFilter.ToString() : null; }
            set { KeepSelectionAfterFilter = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showgrouppanel")]
        public string ShowGroupPanelStr
        {
            get { return (ShowGroupPanel.HasValue) ? ShowGroupPanel.ToString() : null; }
            set { ShowGroupPanel = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("wordwrap")]
        public string WordWrapStr
        {
            get { return (WordWrap.HasValue) ? WordWrap.ToString() : null; }
            set { WordWrap = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        // Dodao Ognjen
        [XmlAttribute("showautofilterrow")]
        public string ShowAutoFilterRowStr
        {
            get { return (ShowAutoFilterRow.HasValue) ? ShowAutoFilterRow.ToString() : null; }
            set { ShowAutoFilterRow = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("findpanel")]
        public string ShowFindPanelStr
        {
            get
            {
                if (ShowFindPanel == false) return "false";
                if (ShowFindPanel != true) return null;
                return string.Equals(FindPanelMode, "allow", System.StringComparison.OrdinalIgnoreCase) ? "allow" : "true";
            }
            set { DxUxAttr.ParseFindPanel(value, out var show, out var mode); ShowFindPanel = show; FindPanelMode = mode; }
        }

        [XmlAttribute("emptymessage")]
        public string EmptyMessageStr
        {
            get { return EmptyMessage; }
            set { EmptyMessage = value; }
        }

        [XmlAttribute("filtereditor")]
        public string AllowFilterEditorStr
        {
            get { return (AllowFilterEditor.HasValue) ? AllowFilterEditor.ToString() : null; }
            set { AllowFilterEditor = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("editform")]
        public string EditFormStr
        {
            get
            {
                if (!string.IsNullOrEmpty(EditFormMode)) return EditFormMode;
                return (AllowEditForm.HasValue) ? AllowEditForm.ToString() : null;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) { AllowEditForm = null; EditFormMode = null; return; }
                var v = value.Trim().ToLowerInvariant();
                if (v == "true" || v == "1" || v == "editform")
                { AllowEditForm = true; EditFormMode = "editform"; }
                else if (v == "editforminplace" || v == "inplacehide")
                { AllowEditForm = true; EditFormMode = v == "inplacehide" ? "editforminplacehidecurrentrow" : "editforminplace"; }
                else if (v == "editforminplacehidecurrentrow")
                { AllowEditForm = true; EditFormMode = "editforminplacehidecurrentrow"; }
                else if (v == "false" || v == "0")
                { AllowEditForm = false; EditFormMode = null; }
                else
                { AllowEditForm = true; EditFormMode = v; }
            }
        }

        [XmlAttribute("columnchooser")]
        public string AllowColumnChooserStr
        {
            get { return (AllowColumnChooser.HasValue) ? AllowColumnChooser.ToString() : null; }
            set { AllowColumnChooser = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        //

        [XmlAttribute("showfooter")]
        public string ShowFooterStr
        {
            get { return (ShowFooter.HasValue) ? ShowFooter.ToString() : null; }
            set { ShowFooter = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("allowfilter")]
        public string AllowFilterStr
        {
            get { return (AllowFilter.HasValue) ? AllowFilter.ToString() : null; }
            set { AllowFilter = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("savelayout")]
        public string SaveLayoutStr
        {
            get { return (SaveLayout.HasValue) ? SaveLayout.ToString() : null; }
            set { SaveLayout = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showgroupsum")]
        public string ShowGroupSumStr
        {
            get { return (ShowGroupSum.HasValue) ? ShowGroupSum.ToString() : null; }
            set { ShowGroupSum = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showtotalsum")]
        public string ShowTotalSumStr
        {
            get { return (ShowTotalSum.HasValue) ? ShowTotalSum.ToString() : null; }
            set { ShowTotalSum = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("async")]
        public string AsyncStr
        {
            get { return (Async.HasValue) ? Async.ToString() : null; }
            set { Async = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("asynccaption")]
        public string AsyncCaptionStr
        {
            get { return AsyncCaption; }
            set { AsyncCaption = value; }
        }

        [XmlAttribute("virtualservermode")]
        public string VirtualServerModeStr
        {
            get { return (VirtualServerMode.HasValue) ? VirtualServerMode.ToString() : null; }
            set { VirtualServerMode = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("netgenfields")]
        public string NetGenFieldsStr
        {
            get { return (NetGenFields.HasValue) ? NetGenFields.ToString() : null; }
            set { NetGenFields = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("netgensizes")]
        public string NetGenSizesStr
        {
            get { return (NetGenSizes.HasValue) ? NetGenSizes.ToString() : null; }
            set { NetGenSizes = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("netgenstyles")]
        public string NetGenStylesStr
        {
            get { return (NetGenStyles.HasValue) ? NetGenStyles.ToString() : null; }
            set { NetGenStyles = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("entermovenexteditor")]
        public string EnterMoveNextEditorStr
        {
            get { return (EnterMoveNextEditor.HasValue) ? EnterMoveNextEditor.ToString() : null; }
            set { EnterMoveNextEditor = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("fieldsmartspaces")]
        public string FieldSmartSpacesStr
        {
            get { return (FieldSmartSpaces.HasValue) ? FieldSmartSpaces.ToString() : null; }
            set { FieldSmartSpaces = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showgroupedcolumns")]
        public string ShowGroupedColumnsStr
        {
            get { return (ShowGroupedColumns.HasValue) ? ShowGroupedColumns.ToString() : null; }
            set { ShowGroupedColumns = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("editdirection")]
        public string EditDirection { get; set; }

        [XmlAttribute("layoutversion")]
        public string LayoutVersion { get; set; }

        [XmlIgnore]
        public string PreviousLayoutName { get; set; } = "";

        [XmlAttribute("layoutname")]
        public string LayoutName { get; set; } = "";

        [XmlAttribute("groupsummarystyle")]
        public string GroupSummaryStyle { get; set; }
    }

    /// <summary>Wave12 shared XML parsers for UX opt-in attrs.</summary>
    public static class DxUxAttr
    {
        public static void ParseFindPanel(string value, out bool? show, out string mode)
        {
            show = null;
            mode = null;
            if (string.IsNullOrEmpty(value))
                return;

            string v = value.Trim().ToLowerInvariant();
            if (v == "false" || v == "0")
            {
                show = false;
                return;
            }

            show = true;
            mode = (v == "allow") ? "allow" : "always";
        }
    }
}
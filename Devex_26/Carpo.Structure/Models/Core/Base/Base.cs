using Carpo.Structure.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core.Base
{
    public class JCondition : IJCondition
    {
        [XmlAttribute("if")]
        [NotifyParentPropertyAttribute(true)]
        public string If { get; set; }

        [XmlAttribute("elseif")]
        [NotifyParentPropertyAttribute(true)]
        public string ElseIf { get; set; }

        [XmlAttribute("else")]
        [NotifyParentPropertyAttribute(true)]
        public string Else { get; set; }
    }

    public class JModelEditorSimple : Interfaces.IJModelEditorBase
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlAttribute("text")]
        public string Text { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("textposition")]
        public string TextPosition { get; set; }

        [XmlAttribute("filter")]
        public string Filter { get; set; }

        [XmlAttribute("errortext")]
        public string ErrorText { get; set; }

        [XmlIgnore]
        public bool? Enabled { get; set; }

        [XmlAttribute("enabled")]
        public string EnabledStr
        {
            get { return (Enabled.HasValue) ? Enabled.ToString() : null; }
            set { Enabled = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public bool? ReadOnly { get; set; }

        [XmlAttribute("readonly")]
        public string ReadOnlyStr
        {
            get { return (ReadOnly.HasValue) ? ReadOnly.ToString() : null; }
            set { ReadOnly = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public int? Width { get; set; }

        [XmlAttribute("width")]
        public string WidthStr
        {
            get { return (Width.HasValue) ? Width.ToString() : null; }
            set { Width = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("visible")]
        public string Visible { get; set; }

        [XmlIgnore]
        public bool? CausesValidation { get; set; }

        [XmlAttribute("savevalue")]
        public string SaveValue { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("minvalue")]
        public string MinValue { get; set; }

        [XmlAttribute("maxvalue")]
        public string MaxValue { get; set; }

        [XmlAttribute("fontcolor")]
        public string FontColor { get; set; }
       
        [XmlAttribute("dock")]
        public string Dock { get; set; }

        [XmlAttribute("required")]
        public string Required { get; set; }

        [XmlAttribute("causesvalidation")]
        public string CausesValidationStr
        {
            get { return (CausesValidation.HasValue) ? CausesValidation.ToString() : null; }
            set { CausesValidation = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

      
        [XmlAttribute("showweeknumbers")]
        public string ShowWeekNumbers { get; set; }

        [XmlAttribute("weeknumberrule")]
        public string WeekNumberRule { get; set; }

        [XmlIgnore]
        public bool? AutoPopulate { get; set; }

        [XmlAttribute("autopopulate")]
        public string AutoPopulateStr
        {
            get { return (AutoPopulate.HasValue) ? AutoPopulate.ToString() : null; }
            set { AutoPopulate = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
        
        [XmlAttribute("tooltip")]
        public string ToolTip { get; set; }

        [XmlAttribute("format")]
        public string Format { get; set; }

        [XmlAttribute("formattype")]
        public string FormatType { get; set; }

        [XmlAttribute("autocomplete")]
        public string AutoComplete { get; set; }

        [XmlIgnore]
        public bool? AllowNull { get; set; }

        [XmlAttribute("allownull")]
        public string AllowNullStr
        {
            get { return (AllowNull.HasValue) ? AllowNull.ToString() : null; }
            set { AllowNull = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public bool? AllowNulAfterClear { get; set; }
        [XmlAttribute("allownullafterclear")]
        public string AllowNullAfterClearStr
        {
            get { return (AllowNulAfterClear.HasValue) ? AllowNulAfterClear.ToString() : null; }
            set { AllowNulAfterClear = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        


        [XmlAttribute("nulltext")]
        public string NullText { get; set; }

        [XmlAttribute("multiselecttext")]
        public string MultiSelectText { get; set; }

        [XmlIgnore]
        public bool? ShowMultiSelectText { get; set; }

        [XmlAttribute("showmultiselecttext")]
        public string ShowMultiSelectTextStr
        {
            get { return (ShowMultiSelectText.HasValue) ? ShowMultiSelectText.ToString() : null; }
            set { ShowMultiSelectText = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public int? FontSizeDelta { get; set; }

        [XmlAttribute("fontsizedelta")]
        public string FontSizeDeltaStr
        {
            get { return (FontSizeDelta.HasValue) ? FontSizeDelta.ToString() : null; }
            set { FontSizeDelta = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("tabindex")]
        public string TabIndex { get; set; }

        [XmlIgnore]
        public bool? FontBold { get; set; }

        [XmlAttribute("fontbold")]
        public string FontBoldStr
        {
            get { return (FontBold.HasValue) ? FontBold.ToString() : null; }
            set { FontBold = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public bool? EnterMoveNextControl { get; set; }

        [XmlAttribute("entermovenextcontrol")]
        public string EnterMoveNextControlStr
        {
            get { return (EnterMoveNextControl.HasValue) ? EnterMoveNextControl.ToString() : null; }
            set { EnterMoveNextControl = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public bool? IgnoreEnterOnNull { get; set; }

        [XmlAttribute("ignoreenteronnull")]
        public string IgnoreEnterOnNullStr
        {
            get { return (IgnoreEnterOnNull.HasValue) ? IgnoreEnterOnNull.ToString() : null; }
            set { IgnoreEnterOnNull = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        //[XmlElement("event")]
        //public JEvent Event { get; set; }

        [XmlElement("event")]
        public List<JEvent> Event { get; set; }

        [XmlIgnore]
        public bool? TabStop { get; set; }

        [XmlAttribute("tabstop")]
        public string TabStopStr
        {
            get { return (TabStop.HasValue) ? TabStop.ToString() : null; }
            set { TabStop = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public bool? MultiLine { get; set; }

        [XmlAttribute("multiline")]
        public string MultiLineStr
        {
            get { return (MultiLine.HasValue) ? MultiLine.ToString() : null; }
            set { MultiLine = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }

    //public class JModelStyle
    //{
    //    [XmlIgnore]
    //    public int? FontSizeDelta { get; set; }

    //    [XmlAttribute("fontsizedelta")]
    //    public string FontSizeDeltaStr
    //    {
    //        get { return (FontSizeDelta.HasValue) ? FontSizeDelta.ToString() : null; }
    //        set { FontSizeDelta = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
    //    }

    //    [XmlIgnore]
    //    public bool? FontBold { get; set; }

    //    [XmlAttribute("fontbold")]
    //    public string FontBoldStr
    //    {
    //        get { return (FontBold.HasValue) ? FontBold.ToString() : null; }
    //        set { FontBold = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
    //    }
    //}

    public class JModelEditorDataBase : JModelEditorSimple, Interfaces.IJModelDataBase
    {
        [XmlElement("data")]
        public List<JData> Data { get; set; }

        //[XmlElement("datalist")]
        //public List<JData> DataList { get; set; }

        [XmlElement("fields")]
        public JFields Fields { get; set; }

        [XmlAttribute("valuefield")]
        public string ValueField { get; set; }

        [XmlAttribute("displayfield")]
        public string DisplayField { get; set; }

        [XmlAttribute("parentfieldname")]
        public string ParentFieldName { get; set; }

        [XmlAttribute("function")]
        public string Function { get; set; } //ssPermission -> FunctionName

        [XmlAttribute("datavalue")]
        public string DataValue { get; set; }

        [XmlAttribute("dataname")]
        public string DataName { get; set; }

        [XmlAttribute("datatext")]
        public string DataText { get; set; }

        [XmlAttribute("datavalueaction")]
        public string DataValueName { get; set; }

        [XmlAttribute("if")]
        public string If { get; set; }

        [XmlElement("properties")]
        public List<JModelEditorDataBase> Properties { get; set; }

        [XmlElement("menu")]
        public List<JMenu> Menu { get; set; }

        [XmlAttribute("filtermode")]
        public string FilterMode { get; set; }

        [XmlAttribute("nodefiltermode")]
        public string NodeFilterMode { get; set; }
    }

    public class JModelEditorText : JModelEditorDataBase
    {
        [XmlAttribute("maxlength")]
        public string MaxLength { get; set; }


        [XmlAttribute("isemail")]
        public bool IsEmail { get; set; }

    }

    public class JModelEditorButton : JModelEditorDataBase
    {
        [XmlAttribute("image")]
        public string Image { get; set; }

        public string ConfirmType { get; set; }

        [XmlAttribute("imagelocation")]
        public string ImageLocation { get; set; }

        [XmlAttribute("color")]
        public string Color { get; set; }

        [XmlElement("menuitem")]
        public JMenuItem MenuItem { get; set; }
    }

    public class JModelEditorLabel : JTextEdit
    {
        [XmlAttribute("prefix")]
        public string Prefix { get; set; }

        [XmlAttribute("sufix")]
        public string Sufix { get; set; }
    }

    public class JModelEditorCheck : JModelEditorDataBase
    {
        [XmlIgnore]
        public int? GroupIndex { get; set; }

        [XmlIgnore]
        public bool? Checked { get; set; }

        [XmlAttribute("checked")]
        public string CheckedStr
        {
            get { return (Checked.HasValue) ? Checked.ToString() : null; }
            set { Checked = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("groupindex")]
        public string GroupIndexStr
        {
            get { return (GroupIndex.HasValue) ? GroupIndex.ToString() : null; }
            set { GroupIndex = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }
    }

    public class JModelEditorLookUp : JTextEdit
    {
        [XmlAttribute("fieldtext")]
        public string FieldText { get; set; }

        [XmlAttribute("fieldvalue")]
        public string FieldValue { get; set; }

        /// <summary>
        /// Opt-in GridLookUp SearchMode: none | autocomplete | autosuggest | autosearch (alias: autofilter).
        /// Default leaves DX control defaults unchanged.
        /// </summary>
        [XmlAttribute("dxsearchmode")]
        public string DxSearchMode { get; set; }

        [XmlIgnore]
        public bool? ImmediatePopup { get; set; }

        [XmlAttribute("immediatepopup")]
        public string ImmediatePopupStr
        {
            get { return (ImmediatePopup.HasValue) ? ImmediatePopup.ToString() : null; }
            set { ImmediatePopup = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }

    public class JModelEditorTreeLookUp : JModelEditorLookUp
    {
        [XmlIgnore]
        public bool? MultiSelect { get; set; }

        [XmlAttribute("multiselectstr")]
        public string MultiSelectStr
        {
            get { return (MultiSelect.HasValue) ? MultiSelect.ToString() : null; }
            set { MultiSelect = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }

    public class JModelEditorComboMultiEdit : JModelEditorLookUp
    {
    }

    public class JModelEditorSearch : JModelEditorLookUp
    {
    }
}
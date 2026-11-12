using Carpo.Structure.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    /*Lookup*/

    public class JComboEdit : Base.JModelEditorLookUp //lookup
    {
        [XmlElement("comboitem")]
        public List<JComboEditItem> ComboItemList { get; set; }
    }

    public class JComboEditItem : Interfaces.IJModelDataItem
    {
        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("text")]
        public string Text { get; set; }

        [XmlAttribute("searchfield")]
        public string SearchField { get; set; }
    }

    public class JAutoSuggest : JSearchEdit
    {
    }

    public class JDataNavigator : Base.JModelEditorDataBase
    {
    }

    public class JInfo : Base.JModelEditorDataBase
    {
    }

    public class JComboMultiEdit : Base.JModelEditorComboMultiEdit // ovo provjeriti !!!!!!!!!!!!!!!
    {
        [XmlElement("combomultiedit")]
        public List<JComboMultiEdit> ComboItemList { get; set; }
    }

    public class JComboTreeEdit : Base.JModelEditorTreeLookUp //lookup
    {
        [XmlElement("comboitem")]
        public List<JComboEditItem> ComboItemList { get; set; }
    }

    /// <summary>DX TokenEdit (B9). EditValue = separator-joined string.</summary>
    public class JTokenEdit : Base.JModelEditorLookUp
    {
        [XmlElement("comboitem")]
        public List<JComboEditItem> ComboItemList { get; set; }

        /// <summary>Default ','.</summary>
        [XmlAttribute("separator")]
        public string Separator { get; set; }

        [XmlIgnore]
        public bool? AllowCustom { get; set; }

        [XmlAttribute("allowcustom")]
        public string AllowCustomStr
        {
            get { return (AllowCustom.HasValue) ? AllowCustom.ToString() : null; }
            set { AllowCustom = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }

    /*TextEdit*/

    public class JTextEdit : Base.JModelEditorText  //textedit
    { }

    public class JMemoEdit : JTextEdit //memoedit
    { }
    public class JRichEdit : JTextEdit
    { }
    public class JHyperlinkEdit : JTextEdit  //textedit
    { }

    /*DateEdit*/

    public class JDateEdit : Base.JModelEditorDataBase //dateedit
    {
        [XmlAttribute("checked")]
        public string Checked { get; set; }

        [XmlAttribute("controlref")]
        public string controlref { get; set; }
    }

    public class JBrowseEdit : Base.JModelEditorDataBase
    {
        [XmlAttribute("texteditstyle")]
        public string TextEditStyle { get; set; }
    }

    /*DaysEdit*/

    public class JDaysEdit : Base.JModelEditorDataBase //daysedit
    {
        [XmlAttribute("basedate")]
        public string Basedate { get; set; }
    }

    /*RadioEdit*/

    public class JRadioEdit : Base.JModelEditorCheck //radioedit
    { }

    /*CheckEdit*/

    public class JCheckEdit : Base.JModelEditorCheck //checkedit
    { }

    /*LabelEdit*/

    public class JLabelEdit : Base.JModelEditorLabel //LabelEdit
    { }

    /*ButtonText - Jupiter style*/

    public class JButtonTextEdit : Base.JModelEditorText //buttontext
    { }

    public class JSeparator
    {
        [XmlAttribute("backcolor")]
        public string BackColor { get; set; }

        [XmlIgnore]
        public bool? Dark { get; set; }

        [XmlAttribute("dark")]
        public string DarkStr
        {
            get { return (Dark.HasValue) ? Dark.ToString() : null; }
            set { Dark = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("orientation")]
        public string Orientation { get; set; }
    }

    /*Button*/

    public class JButtons
    {
        [XmlElement("button")]
        public List<JButton> ButtonList { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }

    public class JButton : Base.JModelEditorButton
    {
        [XmlElement("dropdownitem")]
        public List<JButtonDropDownItem> DropDownItemList { get; set; }

        [XmlIgnore]
        public bool? BeginGroup { get; set; }

        [XmlAttribute("begingroup")]
        public string BeginGroupStr
        {
            get { return (BeginGroup.HasValue) ? BeginGroup.ToString() : null; }
            set { BeginGroup = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("oneitempopup")]
        public string OneItemPopup { get; set; }

        [XmlAttribute("treestructure")]
        public string TreeStructure { get; set; }
    }

    public class JButtonDropDownItem : Base.JModelEditorButton, IJCondition
    {
        //[XmlAttribute("name")]
        //public string Name { get; set; }

        [XmlElement("dropdownitem")]
        public List<JButtonDropDownItem> DropDownItemList { get; set; }

        [XmlAttribute("elseif")]
        [NotifyParentPropertyAttribute(true)]
        public string ElseIf { get; set; }

        [XmlAttribute("else")]
        [NotifyParentPropertyAttribute(true)]
        public string Else { get; set; }

        //[XmlAttribute("caption")]
        //public string Caption { get; set; }

        //[XmlAttribute("value")]
        //public string Value { get; set; }

        //[XmlElement("event")]
        //public JEvent Event { get; set; }
    }

    public class JSearchEdit : Base.JModelEditorSearch
    {
        [XmlIgnore]
        public int? MinSearchLength { get; set; }

        [XmlAttribute("minsearchlength")]
        public string MinSearchLengthtStr
        {
            get { return (MinSearchLength.HasValue) ? MinSearchLength.ToString() : null; }
            set { MinSearchLength = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlIgnore]
        public bool? NetGenFields { get; set; }

        [XmlIgnore]
        public bool? FieldSmartSpaces { get; set; }

        [XmlAttribute("netgenfields")]
        public string NetGenFieldsStr
        {
            get { return (NetGenFields.HasValue) ? NetGenFields.ToString() : null; }
            set { NetGenFields = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("fieldsmartspaces")]
        public string FieldSmartSpacesStr
        {
            get { return (FieldSmartSpaces.HasValue) ? FieldSmartSpaces.ToString() : null; }
            set { FieldSmartSpaces = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("savelayout")]
        public string SaveLayout { get; set; }

        [XmlAttribute("cleargridfilter")]
        public string ClearGridFilter { get; set; }

        // atribut - vrijednost "custom". Služi kada želimo mogućnost unosa search texta koji ne postoji u DS-u,
        // forsiramo search kontroli text i value u upisani string, te se sa strane XML-a kontrolirano unosimo novi slog u bazu (znamo da ne postoji)
        // primjer: ukoliko je pronađen rezultat postavit ćemo IDvalue u id rezultata, ako nije, postavljamo IDvalue u string koji je i upisan i kao takvog ga insertamo
        [XmlAttribute("searchmode")]
        public string searchmode { get; set; }
    }

    public class JTimeEdit : Base.JModelEditorDataBase //timeedit
    {
        [XmlAttribute("interval")]
        public string interval { get; set; }
    }

    public class JPictureEdit : Base.JModelEditorDataBase
    {
        [XmlAttribute("dbimg")]
        public string dbimg { get; set; }

        [XmlAttribute("localimg")]
        public string localimg { get; set; }

        [XmlAttribute("localfolder")]
        public string localfolder { get; set; }

        [XmlAttribute("sizemode")]
        public string sizemode { get; set; }

        [XmlIgnore]
        public bool? DisplayNoImage { get; set; }

        [XmlAttribute("displaynoimage")]
        public string DisplayNoImageStr
        {
            get { return (DisplayNoImage.HasValue) ? DisplayNoImage.ToString() : null; }
            set { DisplayNoImage = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("rightclick")]
        public bool RightClick { get; set; }
    }

    public class JChart : Base.JModelEditorDataBase
    {
        [XmlElement("styles")]
        public JStyles Styles { get; set; }

        [XmlAttribute("viewtype")]
        public string viewType { get; set; }

        [XmlAttribute("legendvisible")]
        public string LegendVisible { get; set; }

        //[XmlAttribute("legendcheckboxes")]
        //public string LegendCheckboxes { get; set; }

        [XmlAttribute("labelvisible")]
        public string LabelVisible { get; set; }

        [XmlAttribute("valueaspercent")]
        public string ValueAsPercent { get; set; }

        [XmlAttribute("showareamarkers")]
        public string ShowAreaMarkers { get; set; }

        [XmlAttribute("gridspacing")]
        public int GridSpacing { get; set; } = 1;

        [XmlAttribute("measureunit")]
        public string MeasureUnit { get; set; }

        [XmlAttribute("xformat")]
        public string Xformat { get; set; }

        [XmlAttribute("yformat")]
        public string Yformat { get; set; }
    }

    public class JDocumentViewer : Base.JModelEditorDataBase
    {
        [XmlAttribute("columnname")]
        public string ColumnName { get; set; }
        [XmlAttribute("columndata")]
        public string ColumnData { get; set; }
        [XmlAttribute("columnrepixid")]
        public string ColumnRepixID { get; set; }
        [XmlAttribute("columnrepixname")]
        public string ColumnRepixName { get; set; }
    }

    public class JPdfViewer : Base.JModelEditorDataBase
    {
        [XmlAttribute("dbpdf")]
        public string dbpdf { get; set; }

        [XmlAttribute("localpdf")]
        public string localpdf { get; set; }

        [XmlAttribute("localfolder")]
        public string localfolder { get; set; }
    }
}
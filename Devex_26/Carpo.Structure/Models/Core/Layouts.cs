using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core.Base;
using Carpo.Structure.Models.Core.XmlGen;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JLayout
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlIgnore]
        public int? ItemPadding { get; set; }

        [XmlIgnore]
        public int? MinSize { get; set; }

        [XmlIgnore]
        public bool? AutoTabOrder { get; set; }

        [XmlAttribute("focusdirection")]
        public string FocusDirection { get; set; }

        [XmlAttribute("textposition")]
        public string TextPosition { get; set; }

        [XmlElement("group")]
        public List<JLayoutGroup> Group { get; set; }

        [XmlAttribute("function")]
        public string Function { get; set; }

        //[XmlElement("datalist")]
        //public JDataList DataList { get; set; } = new JDataList();
        //[XmlElement("data")]
        //public JData Data { get; set; } = new JData();

        [XmlAttribute("itemspadding")]
        public string ItemPaddingStr
        {
            get { return (ItemPadding.HasValue) ? ItemPadding.ToString() : null; }
            set { ItemPadding = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("minsize")]
        public string MinSizeStr
        {
            get { return (MinSize.HasValue) ? MinSize.ToString() : null; }
            set { MinSize = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("autotaborder")]
        public string AutoTabOrderStr
        {
            get { return (AutoTabOrder.HasValue) ? AutoTabOrder.ToString() : null; }
            set { AutoTabOrder = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }

    public class JLayoutGroup : JLayoutShared, IJLayoutCell, IJCondition
    {
        [XmlElement("columnsdefinition")]
        public JColumnsDefinition ColumnsDefinition { get; set; }

        [XmlElement("rowsdefinition")]
        public JRowsDefinition RowsDefinition { get; set; }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlAttribute("backgroundcolor")]
        public string BackgroundColor { get; set; }

        [XmlAttribute("alignmode")]
        public string AlignMode { get; set; }

        [XmlAttribute("colsize")]
        public string ColSize { get; set; }

        [XmlAttribute("rowsize")]
        public string RowSize { get; set; }

        [XmlAttribute("rows")]
        public string Rows { get; set; }

        [XmlAttribute("cols")]
        public string Cols { get; set; }

        [XmlIgnore]
        public bool? TextVisible { get; set; }

        [XmlAttribute("textvisible")]
        public string TextVisibleStr
        {
            get { return (TextVisible.HasValue) ? TextVisible.ToString() : null; }
            set { TextVisible = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public bool? BorderVisible { get; set; }

        [XmlAttribute("bordervisible")]
        public string BorderVisibleStr
        {
            get { return (BorderVisible.HasValue) ? BorderVisible.ToString() : null; }
            set { BorderVisible = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public bool? GroupBorderVisible { get; set; }

        [XmlAttribute("groupbordervisible")]
        public string GroupBorderVisibleStr
        {
            get { return (GroupBorderVisible.HasValue) ? GroupBorderVisible.ToString() : null; }
            set { GroupBorderVisible = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        //[XmlIgnore]
        //public bool? Border { get; set; }

        //[XmlAttribute("border")]
        //public string BorderStr
        //{
        //    get { return (Border.HasValue) ? Border.ToString() : null; }
        //    set { GroupBorderVisible = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        //}

        [XmlElement("item")]
        public List<JLayoutItem> ItemList { get; set; }

        [XmlElement("group")]
        public List<JLayoutGroup> GroupList { get; set; }

        [XmlElement("tabs")]
        public List<JLayoutTabs> TabsList { get; set; }

        [XmlElement("itemdefault")]
        public JLayoutItem ItemDefault { get; set; }

        //[XmlAttribute("cols")]
        //public string ColsStr
        //{
        //    get { return (Cols.HasValue) ? Cols.ToString() : null; }
        //    set { Cols = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        //}

        //[XmlAttribute("rows")]
        //public string RowsStr
        //{
        //    get { return (Rows.HasValue) ? Rows.ToString() : null; }
        //    set { Rows = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        //}
    }

    public class JLayoutShared : JCondition
    {
        [XmlAttribute("name")] //left,right,top,bottom
        public string Name { get; set; }

        [XmlAttribute("row")]
        public string Row { get; set; }

        [XmlAttribute("col")]
        public string Col { get; set; }

        [XmlIgnore]
        public int? RowSpan { get; set; }

        [XmlIgnore]
        public int? ColSpan { get; set; }

        [XmlAttribute("rowspan")]
        public string RowSpanStr
        {
            get { return (RowSpan.HasValue) ? RowSpan.ToString() : null; }
            set { RowSpan = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("colspan")]
        public string ColSpanStr
        {
            get { return (ColSpan.HasValue) ? ColSpan.ToString() : null; }
            set { ColSpan = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("pos")]
        public string Pos
        {
            get { return null; }
            set { ParsePos(value); }
        }

        //public string Pos { get; set; }
        [XmlAttribute("width")]
        public string Width { get; set; }

        [XmlAttribute("height")]
        public string Height { get; set; }

        [XmlAttribute("padding")] //left,right,top,bottom
        public string Padding { get; set; }

        [XmlAttribute("spacing")]
        public string Spacing { get; set; }

        public void ParsePos(string pos)
        {
            if (string.IsNullOrEmpty(pos))
                return;
            string[] posArr = pos.Replace(" ", "").Split(',');
            if (posArr.Length != 2)
                return;

            int x, y;
            bool isNumeric = int.TryParse(posArr[0], out x);

            if (isNumeric == false)
                return;
            isNumeric = int.TryParse(posArr[1], out y);
            if (isNumeric == false)
                return;

            Col = x.ToString();
            Row = y.ToString();
        }
    }

    public class JLayoutTabs : JLayoutShared, IJLayoutCell
    {
        [XmlElement("group")]
        public List<JLayoutGroup> GroupList { get; set; }

        [XmlAttribute("selectedtabindex")]
        public string SelectedTabIndex { get; set; }

        [XmlIgnore]
        public bool? MultiLine { get; set; }

        [XmlAttribute("multiline")]
        public string MultiLineStr
        {
            get { return (MultiLine.HasValue) ? MultiLine.ToString() : null; }
            set { MultiLine = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlElement("event")]
        public List<JEvent> Events { get; set; }
    }

    public class JLayoutItem : JLayoutShared, IJLayoutCell
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlAttribute("textposition")]
        public string TextPosition { get; set; }

        [XmlAttribute("colsize")]
        public string ColSize { get; set; }

        [XmlAttribute("rowsize")]
        public string RowSize { get; set; }

        [XmlAttribute("textalignmode")] //auto
        public string TextAlignMode { get; set; }

        [XmlAttribute("nulltext")]
        public string NullText { get; set; }

        [XmlIgnore]
        public bool? TextVisible { get; set; }

        [XmlAttribute("textvisible")]
        public string TextVisibleStr
        {
            get { return (TextVisible.HasValue) ? TextVisible.ToString() : null; }
            set { TextVisible = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("visible")]
        public string Visible { get; set; }

        [XmlIgnore]
        public bool? Highlight { get; set; }

        [XmlAttribute("highlight")]
        public string HighlightStr
        {
            get { return (Highlight.HasValue) ? Highlight.ToString() : null; }
            set { Highlight = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        /******editors*****/

        [XmlElement("textedit")]
        public JTextEdit TextEdit { get; set; }

        [XmlElement("hyperlinkedit")]
        public JHyperlinkEdit HyperLinkEdit { get; set; }

        [XmlElement("memoedit")]
        public JMemoEdit MemoEdit { get; set; }
        [XmlElement("richedit")]
        public JRichEdit RichEdit { get; set; }

        [XmlElement("comboedit")]
        public JComboEdit ComboEdit { get; set; }

        [XmlElement("combomultiedit")]
        public JComboMultiEdit ComboMultiEdit { get; set; }

        [XmlElement("tokenedit")]
        public JTokenEdit TokenEdit { get; set; }

        [XmlElement("combotreeedit")]
        public JComboTreeEdit ComboTreeEdit { get; set; }

        [XmlElement("browseedit")]
        public JBrowseEdit BrowseEdit { get; set; }

        [XmlElement("dateedit")]
        public JDateEdit DateEdit { get; set; }

        [XmlElement("buttontextedit")]
        public JButtonTextEdit ButtonTextEdit { get; set; }

        [XmlElement("button")]
        public JButton Button { get; set; }

        [XmlElement("searchedit")]
        public JSearchEdit SearchEdit { get; set; }

        [XmlElement("labeledit")]
        public JLabelEdit LabelEdit { get; set; }

        [XmlElement("checkedit")]
        public JCheckEdit CheckEdit { get; set; }

        [XmlElement("radioedit")]
        public JRadioEdit RadioEdit { get; set; }

        [XmlElement("daysedit")]
        public JDaysEdit DaysEdit { get; set; }

        [XmlElement("timeedit")]
        public JTimeEdit TimeEdit { get; set; }

        [XmlElement("pictureedit")]
        public JPictureEdit PictureEdit { get; set; }

        [XmlElement("pdfviewer")]
        public JPdfViewer PdfViewer { get; set; }

        [XmlElement("documentviewer")]
        public JDocumentViewer DocumentViewer { get; set; }


        [XmlElement("grid")]
        public JGrid Grid { get; set; }

        [XmlElement("tree")]
        public JTree Tree { get; set; }

        [XmlElement("tabs")]
        public JTabs Tabs { get; set; }

        [XmlElement("separator")]
        public JSeparator Separator { get; set; }

        [XmlElement("customcontrol")]
        public JCustomUCDef CustomControl { get; set; }

        [XmlElement("chart")]
        public JChart Chart { get; set; }

        /******containers*****/

        [XmlElement("container")]
        public JContainer Container { get; set; }

        [XmlIgnore]
        public bool? FontBold { get; set; }

        [XmlAttribute("fontbold")]
        public string FontBoldStr
        {
            get { return (FontBold.HasValue) ? FontBold.ToString() : null; }
            set { FontBold = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public int? FontSizeDelta { get; set; }

        [XmlAttribute("fontsizedelta")]
        public string FontSizeDeltaStr
        {
            get { return (FontSizeDelta.HasValue) ? FontSizeDelta.ToString() : null; }
            set { FontSizeDelta = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }
    }

    public class JColumnsDefinition
    {
        [XmlAttribute("size")]
        public string Size { get; set; }

        [XmlElement("column")]
        public List<JColumn> ColumnList { get; set; }
    }

    public class JRowsDefinition
    {
        [XmlAttribute("size")]
        public string Size { get; set; }

        [XmlElement("row")]
        public List<JRow> RowList { get; set; }
    }

    public class JColumn
    {
        [XmlIgnore]
        public int? Position { get; set; }

        [XmlAttribute("size")]
        public string Size { get; set; }

        [XmlAttribute("position")]
        public string PositionStr
        {
            get { return (Position.HasValue) ? Position.ToString() : null; }
            set { Position = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }
    }

    public class JRow
    {
        [XmlIgnore]
        public int? Position { get; set; }

        [XmlAttribute("size")]
        public string Size { get; set; }

        [XmlAttribute("position")]
        public string PositionStr
        {
            get { return (Position.HasValue) ? Position.ToString() : null; }
            set { Position = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }
    }
}
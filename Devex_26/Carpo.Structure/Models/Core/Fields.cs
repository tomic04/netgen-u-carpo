using Carpo.Structure.Models.Core.Base;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    //ideja je da se defaultna definicija može pregaziti definicijom unutar xml-a

    public class JFields : JCondition
    {
        [XmlElement("field")]
        public List<JField> FieldList { get; set; }

        [XmlIgnore]
        public bool? Inherited { get; set; }

        [XmlAttribute("inherited")]
        public string InheritedStr
        {
            get { return (Inherited.HasValue) ? Inherited.ToString() : null; }
            set { Inherited = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }

    public class JField : JCondition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        private string formatString;
        [XmlAttribute("dataname")]
        public string DataName { get; set; }
        [XmlAttribute("valuefield")]
        public string ValueField { get; set; }
        [XmlAttribute("formatstring")]
        public string FormatString
        {
            get
            {
                if (string.IsNullOrEmpty(formatString))
                    return Format;
                else
                    return formatString;
            }
            set { formatString = value; }
        }

        [XmlAttribute("format")]
        public string Format { get; set; }

        [XmlAttribute("customsort")]
        public string CustomSort { get; set; }

        [XmlAttribute("autofiltercondition")]
        public string AutoFilterCondition { get; set; }

        [XmlAttribute("fixed")]
        public string Fixed { get; set; }

        [XmlAttribute("totalsumtype")]
        public string TotalSumType { get; set; }

        [XmlAttribute("weightedcolumnname")]
        public string WeightedColumnName { get; set; }
        [XmlAttribute("groupsumtype")]
        public string GroupSumType { get; set; }

        [XmlIgnore]
        public bool? ClearValueOnPrint { get; set; }

        [XmlIgnore]
        public bool? ShowGroupSum { get; set; }

        [XmlIgnore]
        public bool? ShowTotalSum { get; set; }

        [XmlIgnore]
        public int? FixWidth { get; set; }

        [XmlIgnore]
        public int? GroupIndex { get; set; }

        [XmlIgnore]
        public bool? Visible { get; set; }

        [XmlIgnore]
        public bool? AllowSort { get; set; }

        [XmlIgnore]
        public bool? AllowAutoFilter { get; set; }

        [XmlAttribute("index")]
        public string Index { get; set; }

        [XmlIgnore]
        public bool? Hidden { get; set; }

        [XmlAttribute("groupindex")]
        public string GroupIndexStr
        {
            get { return (GroupIndex.HasValue) ? GroupIndex.ToString() : null; }
            set { GroupIndex = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("fixwidth")]
        public string FixWidthStr
        {
            get { return (FixWidth.HasValue) ? FixWidth.ToString() : null; }
            set { FixWidth = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlIgnore]
        public int? MinWidth { get; set; }

        [XmlAttribute("minwidth")]
        public string MinWidthStr
        {
            get { return (MinWidth.HasValue) ? MinWidth.ToString() : null; }
            set { MinWidth = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlIgnore]
        public int? MaxWidth { get; set; }

        [XmlAttribute("maxwidth")]
        public string MaxWidthStr
        {
            get { return (MaxWidth.HasValue) ? MaxWidth.ToString() : null; }
            set { MaxWidth = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("editable")]
        public string Editable { get; set; }

        [XmlAttribute("edittype")]
        public string EditType { get; set; }

        [XmlIgnore]
        public bool? CellStayOnEnter { get; set; }

        [XmlAttribute("cellstayonenter")]
        public string CellStayOnEnterStr
        {
            get { return (CellStayOnEnter.HasValue) ? CellStayOnEnter.ToString() : null; }
            set { CellStayOnEnter = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }


        [XmlIgnore]
        public bool? FontBold { get; set; }

        [XmlAttribute("fontbold")]
        public string FontBoldStr
        {
            get { return (FontBold.HasValue) ? FontBold.ToString() : null; }
            set { FontBold = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        /// <summary>
        /// Fiksna boja teksta za celu kolonu (npr. red, blue, #ff0000).
        /// Ako je definisan i uslovni stil na nivou reda (styles/fontcolor), on ima prednost za taj red.
        /// </summary>
        [XmlAttribute("fontcolor")]
        public string FontColor { get; set; }

        /// <summary>
        /// Fiksna boja pozadine ćelija za celu kolonu (npr. lightyellow, #ffffcc).
        /// Ako je definisan uslovni stil na nivou reda (styles/backgroundcolor), on ima prednost za taj red.
        /// </summary>
        [XmlAttribute("backcolor")]
        public string BackColor { get; set; }

        [XmlAttribute("showgroupsum")]
        public string ShowGroupSumStr
        {
            get { return (ShowGroupSum.HasValue) ? ShowGroupSum.ToString() : null; }
            set { ShowGroupSum = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showtotalsum")]
        public string ShowTotalSUmStr
        {
            get { return (ShowTotalSum.HasValue) ? ShowTotalSum.ToString() : null; }
            set { ShowTotalSum = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("clearvalueonprint")]
        public string ClearValueOnPrintStr
        {
            get { return (ClearValueOnPrint.HasValue) ? ClearValueOnPrint.ToString() : null; }
            set { ClearValueOnPrint = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("visible")]
        public string VisibleStr
        {
            get { return (Visible.HasValue) ? Visible.ToString() : null; }
            set { Visible = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("hidden")]
        public string HiddenStr
        {
            get { return (Hidden.HasValue) ? Hidden.ToString() : null; }
            set { Hidden = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("allowsort")]
        public string AllowSortStr
        {
            get { return (AllowSort.HasValue) ? AllowSort.ToString() : null; }
            set { AllowSort = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("allowautofilter")]
        public string AllowAutoFilterStr
        {
            get { return (AllowAutoFilter.HasValue) ? AllowAutoFilter.ToString() : null; }
            set { AllowAutoFilter = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("allowgroup")]
        public string AllowGroup { get; set; }

        [XmlIgnore]
        public int? ColWidthBeforeGrouping { get; set; } = null;

        [XmlAttribute("tooltip")]
        public string Tooltip { get; set; }

        [XmlAttribute("captionalign")]
        public string CaptionAlign { get; set; }

        [XmlAttribute("align")]
        public string Align { get; set; }

        [XmlAttribute("rowautoheight")]
        public string RowAutoHeight { get; set; }
    }
}
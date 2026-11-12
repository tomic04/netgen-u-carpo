using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core.Base
{
    //drvo i grid
    public class JModelDataControlsBase : JModelEditorDataBase
    {
        [XmlIgnore]
        public bool? ColumnAutoWidth { get; set; }

        [XmlIgnore]
        public bool? ShowColumnHeaders { get; set; }

        [XmlAttribute("searchfield")]
        public string SearchField { get; set; }

        [XmlAttribute("customsort")]
        public string CustomSort { get; set; }

        [XmlIgnore]
        public bool? ValueChangeOnScroll { get; set; }

        [XmlIgnore]
        public bool? ShowMenuPrintItem { get; set; }

        [XmlIgnore]
        public bool? ShowMenuTools { get; set; }

        [XmlIgnore]
        public bool? AllowDrop { get; set; }

        [XmlIgnore]
        public bool? AllowDrag { get; set; }

        [XmlIgnore]
        public bool? AllowSelfDrag { get; set; }

        [XmlIgnore]
        public bool? AllowDragMultiple { get; set; }

        [XmlAttribute("showmenuprintitem")]
        public string ShowMenuPrintItemStr
        {
            get { return (ShowMenuPrintItem.HasValue) ? ShowMenuPrintItem.ToString() : null; }
            set { ShowMenuPrintItem = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showcolumnheaders")]
        public string ShowColumnHeadersStr
        {
            get { return (ShowColumnHeaders.HasValue) ? ShowColumnHeaders.ToString() : null; }
            set { ShowColumnHeaders = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("showmenutools")]
        public string ShowMenuGridToolsStr
        {
            get { return (ShowMenuTools.HasValue) ? ShowMenuTools.ToString() : null; }
            set { ShowMenuTools = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("allowdrop")]
        public string AllowDropStr
        {
            get { return (AllowDrop.HasValue) ? AllowDrop.ToString() : null; }
            set { AllowDrop = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("columnautowidth")]
        public string ColumnAutoWidthStr
        {
            get { return (ColumnAutoWidth.HasValue) ? ColumnAutoWidth.ToString() : null; }
            set { ColumnAutoWidth = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("allowdrag")]
        public string AllowDragStr
        {
            get { return (AllowDrag.HasValue) ? AllowDrag.ToString() : null; }
            set { AllowDrag = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        // dodao Ognjen
        [XmlAttribute("valuechangeonscroll")]
        public string ValueChangeOnScrollStr
        {
            get { return (ValueChangeOnScroll.HasValue) ? ValueChangeOnScroll.ToString() : null; }
            set { ValueChangeOnScroll = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        //

        [XmlAttribute("allowselfdrag")]
        public string AllowSelfDragStr
        {
            get { return (AllowSelfDrag.HasValue) ? AllowSelfDrag.ToString() : null; }
            set { AllowSelfDrag = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("allowdragmultiple")]
        public string AllowDragMultipleStr
        {
            get { return (AllowDragMultiple.HasValue) ? AllowDragMultiple.ToString() : null; }
            set { AllowDragMultiple = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }
}
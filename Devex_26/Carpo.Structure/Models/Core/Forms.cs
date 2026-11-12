using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    [XmlRoot("form")]
    public class JForm
    {
        [XmlAttribute("disposeOnClose")]
        public bool DisposeOnClose { get; set; } = false;
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("template")]
        public string Template { get; set; }

        [XmlIgnore]
        public int? Width { get; set; }

        [XmlIgnore]
        public int? Height { get; set; }

        [XmlIgnore]
        public int? SplitterPosition { get; set; }

        [XmlIgnore]
        public bool? FixedSize { get; set; }

        [XmlIgnore]
        public bool? SaveFormSize { get; set; }

        [XmlIgnore]
        public bool? Maximized { get; set; }

        [XmlIgnore]
        public bool? StatusBar { get; set; }

        [XmlIgnore]
        public bool? ShowVersion { get; set; }

        [XmlIgnore]
        public bool? SplitterFixed { get; set; }

        [XmlAttribute("operaterlocktable")]
        public string OperaterLockTable { get; set; }

        [XmlAttribute("operaterlockkey")]
        public string OperaterLockKey { get; set; }

        [XmlAttribute("operaterlockkeyvalue")]
        public string OperaterLockKeyValue { get; set; }

        [XmlAttribute("statusbar")]
        public string StatusBarStr
        {
            get { return (StatusBar.HasValue) ? StatusBar.ToString() : null; }
            set { StatusBar = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("fixedsize")]
        public string FixedSizeStr
        {
            get { return (FixedSize.HasValue) ? FixedSize.ToString() : null; }
            set { FixedSize = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("maximized")]
        public string MaximizedStr
        {
            get { return (Maximized.HasValue) ? Maximized.ToString() : null; }
            set { Maximized = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlAttribute("exportButtonVisible")]
        public bool exportButtonVisible { get; set; } = true;

        [XmlAttribute("showversion")]
        public string ShowVersionStr
        {
            get { return (ShowVersion.HasValue) ? ShowVersion.ToString() : null; }
            set { ShowVersion = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlIgnore]
        public int? CaptionFontSizeDelta { get; set; }

        [XmlAttribute("captionfontsizedelta")]
        public string CaptionFontSizeDeltaStr
        {
            get { return (CaptionFontSizeDelta.HasValue) ? CaptionFontSizeDelta.ToString() : null; }
            set { CaptionFontSizeDelta = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlIgnore]
        public bool? CaptionFontBold { get; set; }

        [XmlAttribute("captionfontbold")]
        public string CaptionFontBoldStr
        {
            get { return (CaptionFontBold.HasValue) ? CaptionFontBold.ToString() : null; }
            set { CaptionFontBold = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("captionforecolor")]
        public string CaptionForeColor { get; set; }

        [XmlElement("managerlayout")]
        public JManagerLayout ManagerLayout { get; set; }

        //[XmlElement("container")]
        //public JContainer Container { get; set; }

        [XmlElement("layout")]
        public List<JLayout> Layout { get; set; }

        [XmlElement("infocontrols")]
        public JInfoControls InfoControls { get; set; }

        [XmlAttribute("width")]
        public string WidthStr
        {
            get { return (Width.HasValue) ? Width.ToString() : null; }
            set { Width = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("height")]
        public string HeightStr
        {
            get { return (Height.HasValue) ? Height.ToString() : null; }
            set { Height = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("splitterposition")]
        public string SplitterPositionStr
        {
            get { return (SplitterPosition.HasValue) ? SplitterPosition.ToString() : null; }
            set { SplitterPosition = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute("splitterfixed")]
        public string SplitterFixedStr
        {
            get { return (SplitterFixed.HasValue) ? SplitterFixed.ToString() : null; }
            set { SplitterFixed = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("saveformsize")]
        public string SaveFormSizeStr
        {
            get { return (SaveFormSize.HasValue) ? SaveFormSize.ToString() : null; }
            set { SaveFormSize = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("closeformonescape")]
        public string CloseFormOnEscape { get; set; }

        [XmlElement("parameter")]
        public JEventActionParameter Parameters { get; set; }

        [XmlElement("panelbuttons")]
        public JPanel PanelButtons { get; set; }

        [XmlElement("actionpanel")]
        public JPanel ActionPanel { get; set; }

        [XmlElement("datalist")]
        public JDataList DataList { get; set; }

        [XmlElement("event")]
        public List<JEvent> Event { get; set; }

        [XmlElement("method")]
        public List<JCustomMethodDef> CustomMethod { get; set; }
    }
}
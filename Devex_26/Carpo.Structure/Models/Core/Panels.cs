using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JActionItems
    {
        [XmlElement("item")]
        public List<JActionItem> JActionItem { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }

    public class JActionItem
    {
        [XmlElement("button")]
        public JButton Button { get; set; }

        [XmlElement("labeledit")]
        public JLabelEdit LabelEdit { get; set; }

        [XmlElement("radioedit")]
        public JRadioEdit RadioEdit { get; set; }

        [XmlElement("checkedit")]
        public JCheckEdit CheckEdit { get; set; }

        [XmlElement("textedit")]
        public JTextEdit TextEdit { get; set; }
    }

    public class JPanel
    {
        [XmlElement("items")]
        public List<JActionItems> ActionItems { get; set; }

        //[XmlElement("buttons")]
        //public List<JButtons> Buttons { get; set; }

        [XmlElement("buttons")]
        public List<JButtons> Buttons { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlIgnore]
        public bool? DisplayControlsPrint { get; set; }

        [XmlIgnore]
        public bool? DisplayHelp { get; set; }

        [XmlAttribute("displayhelp")]
        public string DisplayHelpStr
        {
            get { return (DisplayHelp.HasValue) ? DisplayHelp.ToString() : null; }
            set { DisplayHelp = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlAttribute("displaycontrolsprint")]
        public string DisplayControlsPrintStr
        {
            get { return (DisplayControlsPrint.HasValue) ? DisplayControlsPrint.ToString() : null; }
            set { DisplayControlsPrint = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }
}
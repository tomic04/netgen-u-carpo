using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    //public class JMenus
    //{
    //    [XmlElement("menu")]
    //    public List<JMenu> Menu { get; set; }
    //}

    public class JMenu
    {
        [XmlAttribute("control")]
        public string Control { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("showalways")]
        public bool ShowAlways { get; set; }

        [XmlElement("menuitem")]
        public List<JMenuItem> MenuItem { get; set; }
    }

    public class JMenuItem : Base.JCondition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("group")]
        public string Group { get; set; }

        [XmlAttribute("control")]
        public string Control { get; set; }

        [XmlAttribute("sort")]
        public string Sort { get; set; }

        [XmlElement("event")]
        public List<JEvent> Event { get; set; }

        [XmlAttribute("function")]
        public string Function { get; set; } //ssPermission -> FunctionName
    }

    public class JMenuItemList
    {
        public List<ToolStripItem> Item { get; set; } = new List<ToolStripItem>();
        public string Type { get; set; }
        public bool ShowAlways { get; set; } = false;
    }
}
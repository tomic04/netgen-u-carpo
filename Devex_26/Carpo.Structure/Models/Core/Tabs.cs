using Carpo.Structure.Models.Core.Base;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JTabs: JModelEditorDataBase
    {

        [XmlElement("tab")]
        public List<JTab> TabList { get; set; }

        [XmlAttribute("tabslocation")]
        public string TabsLocation { get; set; }

        [XmlAttribute("tabsorientation")]
        public string TabsOrientation { get; set; }

    }

    public class JTab : JCondition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlElement("grid")]
        public JGrid Grid { get; set; }

        [XmlAttribute("enabled")]
        public string Enabled { get; set; }
    }
}
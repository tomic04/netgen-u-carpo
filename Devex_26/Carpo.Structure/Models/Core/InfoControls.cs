using Carpo.Structure.Models.Core.Base;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    //ideja je da se defaultna definicija može pregaziti definicijom unutar xml-a

    public class JInfoControls
    {
        [XmlElement("tabs")]
        public JTabs Tabs { get; set; }

        [XmlElement("search")]
        public JAutoSuggest AutoSuggest { get; set; }

        [XmlElement("navigation")]
        public JDataNavigator DataNavigator { get; set; }

        [XmlElement("info")]
        public JInfo Info { get; set; }

        [XmlElement("newbutton")]
        public JButton NewButton { get; set; }

        [XmlAttribute("entity")]
        public string Entity { get; set; }

        [XmlAttribute("searchtype")]
        public string SearchType { get; set; }
    }
}
using Carpo.Structure.Models.Core.Base;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JStyle : JCondition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    public class JGridFormatRuleList
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("field")]
        public string Field { get; set; }

        [XmlAttribute("showinfield")]
        public string ShowInField { get; set; } = "";

        [XmlAttribute("shownegativebar")]
        public bool ShowNegativeBar { get; set; } = true;

        [XmlAttribute("showbaronly")]
        public bool ShowBarOnly { get; set; } = false;

        [XmlAttribute("valuetype")]
        public string ValueType { get; set; }

        [XmlElement("parameter")]
        public List<GridFormatRuleParameter> GridFormatRuleParameterList { get; set; }
    }

    public class JStyles
    {
        [XmlElement("style")]
        public List<JStyle> StyleList { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("formatrule")]
        public List<JGridFormatRuleList> GridFormatRuleList { get; set; }
    }

    public class GridFormatRuleParameter
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public int Value { get; set; }
    }
}
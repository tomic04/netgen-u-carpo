using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JCustomDef
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("dll")]
        public string Dll { get; set; }

        [XmlAttribute("class")]
        public string Class { get; set; }

        [XmlElement("parameter")]
        public List<JCustomParameter> Parameter { get; set; }

        [XmlAttribute("initonmaster")]
        public string InitOnMaster { get; set; }
    }

    public class JCustomUCDef : JCustomDef
    {
    }

    public class JCustomFormDef : JCustomDef
    {
    }

    public class JCustomMethodDef : JCustomDef
    {
        [XmlAttribute("method")]
        public string Method { get; set; }
    }

    public class JCustomParameter
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }
}
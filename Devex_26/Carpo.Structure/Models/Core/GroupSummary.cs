using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JSummary
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("fieldname")]
        public string FieldName { get; set; }

        [XmlAttribute("format")]
        public string Format { get; set; }

        [XmlAttribute("visible")]
        public bool Visible { get; set; } = true;
    }

    public class JGroupSummary
    {
        [XmlElement("summary")]
        public List<JSummary> SummaryList { get; set; }
    }
}
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    [XmlRoot("root")]
    public class JRoot
    {
        [XmlElement("form")]
        public List<JForm> Form { get; set; }

        [XmlElement("datalist")]
        public JDataList DataList { get; set; }

        [XmlAttribute("loglevel")]
        public string LogLevel { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("translatortruncate")]
        public bool TranslatorTruncate { get; set; }

        [XmlAttribute("translatoruselog")]
        public bool TranslatorUseLog { get; set; }
    }
}
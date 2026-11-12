using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JContainer
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("buttons")]
        public JButtons Buttons { get; set; }
    }
}
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JPrint
    {
        [XmlAttribute("caption")]
        public string Caption { get; set; }

        [XmlAttribute("captionalign")]
        public string CaptionAlign { get; set; }

        [XmlAttribute("header")]
        public string Header { get; set; }

        [XmlAttribute("headeralign")]
        public string HeaderAlign { get; set; }

        [XmlAttribute("subheader")]
        public string SubHeader { get; set; }

        [XmlAttribute("subheaderalign")]
        public string SubHeaderAlign { get; set; }

        [XmlAttribute("headerscale")]
        public string HeaderScale { get; set; }

        [XmlAttribute("imagepath")]
        public string ImagePath { get; set; }
    }
}
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core.XmlGen
{
    public class JXmlGenRoot
    {
        [XmlElement("xml")]
        public XDocument Xml { get; set; }

        public JDataList DataList { get; set; }

        [XmlElement("validate")]
        public XElement Validate { get; set; }
    }

    // MISLIM DA NAM NE TREBA JER "xml" mora biti XElement a u njemu ne znamo šta možemo očekivati

    //public class JXmlGen
    //{
    //    [XmlElement("gen")]
    //    public List<JGen> Gen { get; set; }
    //}

    public class JDataList
    {
        [XmlElement("data")]
        public List<JData> Data { get; set; }
    }

    public class JData
    {
        [XmlAttribute("command")]
        public string Command { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("parameter")]
        public List<JDataParameter> DataParameterList { get; set; }

        [XmlAttribute("timeout")]
        public string Timeout { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("parentdataset")]
        public string ParentDataset { get; set; }
    }

    public class JDataParameter
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("valuefield")]
        public string ValueField { get; set; }
    }

    public class JGen
    {
        [XmlAttribute("if")]
        public string If { get; set; }

        [XmlAttribute("elseif")]
        public string ElseIf { get; set; }

        [XmlAttribute("else")]
        public string Else { get; set; }

        [XmlAttribute("foreach")]
        public string Foreach { get; set; }

        public List<XElement> JGenXmlElements { get; set; }
    }
}
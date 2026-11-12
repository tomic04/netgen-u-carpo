using Carpo.Structure.Models.Core.Base;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JDataList
    {
        [XmlElement("data")]
        public List<JData> Data { get; set; }

        [XmlElement("datalinqlist")]
        public List<JDataLinq> DataLinq { get; set; }
    }

    public class JData : JCondition
    {
        [XmlAttribute("command")]
        public string Command { get; set; }

        [XmlAttribute("key")]
        public string Key { get; set; }

        //[XmlAttribute("type")]
        //public JDataType Type { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("parameter")]
        public List<JDataParameter> DataParameterList { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("timeout")]
        public string Timeout { get; set; }
    }

    public class JDataDef
    {
        [XmlAttribute("dataname")]
        public string DataName { get; set; }

        [XmlAttribute("datavalue")]
        public string DataValue { get; set; }

        [XmlAttribute("if")]
        public string If { get; set; }
    }

    public class JDataParameter : JCondition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlElement("tablepar")]
        public List<JTablePar> TableParList { get; set; }

        [XmlAttribute("variablename")]
        public string VariableName { get; set; }

        [XmlAttribute("valuetransform")]
        public string ValueTransform { get; set; }
    }

    public class JTablePar
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }

    public class JDataLinq
    {
        [XmlAttribute("tablename")]
        public string TableName { get; set; }

        [XmlAttribute("key")]
        public string Key { get; set; }

        public List<JDataLinqCriteria> DataLinqCriteria { get; set; }
    }

    public class JDataLinqCriteria
    {
        [XmlAttribute("criteria")]
        public string Criteria { get; set; }

        [XmlAttribute("if")]
        public string If { get; set; }
    }
}
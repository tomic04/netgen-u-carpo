using Carpo.Structure.Models.Core.Base;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JNotifications : JCondition
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("waittype")]
        public string WaitType { get; set; }

        [XmlAttribute("waittext")]
        public string WaitText { get; set; }

        [XmlAttribute("waitcaption")]
        public string WaitCaption { get; set; }

        [XmlAttribute("singleprint")]
        public string SinglePrint { get; set; }

        [XmlIgnore]
        public int? Sleep { get; set; }

        [XmlAttribute("sleep")]
        public string SleepStr
        {
            get { return (Sleep.HasValue) ? Sleep.ToString() : null; }
            set { Sleep = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlElement("action")]
        public List<JEventAction> Actions { get; set; }
    }

    public class JEvent : JNotifications
    {
        [XmlIgnore]
        public bool? RaiseOnLoad { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("raiseonload")]
        public string RaiseOnLoadStr
        {
            get { return (RaiseOnLoad.HasValue) ? RaiseOnLoad.ToString() : null; }
            set { RaiseOnLoad = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }
    }

    public class JEventAction : JNotifications
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlIgnore]
        public bool? StopOnConfirm { get; set; }

        [XmlAttribute("stoponconfirm")]
        public string StopOnConfirmStr
        {
            get { return (StopOnConfirm.HasValue) ? StopOnConfirm.ToString() : null; }
            set { StopOnConfirm = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlIgnore]
        public bool? Stop { get; set; }

        [XmlAttribute("stop")]
        public string StopStr
        {
            get { return (Stop.HasValue) ? Stop.ToString() : null; }
            set { Stop = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
        }

        [XmlElement("parameter")]
        public List<JEventActionParameter> Parameter 
        {
            get;
            set;
        }
    }

    //public class JEventActionParameters
    //{
    //    [XmlElement("parameter")]
    //    public List<JEventActionParameter> Parameter { get; set; }
    //}

    public class JEventActionParameter : JCondition
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("text")]
        public string Text { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("valuetype")]
        public string ValueType { get; set; }

        [XmlAttribute("condition")]
        public string Condition { get; set; }

        [XmlAttribute("arithmetictype")]
        public string ArithmeticType { get; set; }
    }
}
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Carpo.Structure.Models.Core
{
    public class JManagerLayout
    {
        [XmlElement("grid")]
        public List<JGrid> GridList { get; set; }

        [XmlElement("tree")]
        public List<JTree> TreeList { get; set; }

        [XmlElement("lookup")]
        public List<JComboEdit> LookUpList { get; set; }

        [XmlElement("tabs")]
        public List<JTabs> Tabs { get; set; } //lista - ako će se kasnije primijeniti i drugdje osim detail grida
    }

    //public class JControl
    //{
    //    [XmlAttribute("name")]
    //    public string Name { get; set; }

    //    [XmlIgnore]
    //    public bool? Editable { get; set; }

    //    [XmlAttribute("editable")]
    //    public string EditableStr
    //    {
    //        get { return (Editable.HasValue) ? Editable.ToString() : null; }
    //        set { Editable = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?); }
    //    }

    //    [XmlAttribute("key")]
    //    public string Key { get; set; }

    //    [XmlElement("data")]
    //    public List<JData> Data { get; set; }

    //    [XmlElement("fields")]
    //    public JFields Fields { get; set; }

    //    [XmlElement("item")]
    //    public List<JItem> Items { get; set; }
    //}
}
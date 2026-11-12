using System.Xml;

namespace Carpo.CMS.Models
{
    public class Element
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public XmlNode Node { get; set; }

        public string Info { get; set; }
    }

    public class ElementCheck
    {
        public string Name { get; set; }
        public string ParentName { get; set; }
    }

    public class CheckXml
    {
        public int RowNumber { get; set; }
        public string Element { get; set; }
        public string Attribute { get; set; }
        public string Message { get; set; }
        public string File { get; set; }
    }

    internal class Attribute
    {
        public int CarpoDsAttributeId { get; set; }
        public string AttrName { get; set; }
        public string AttrDescription { get; set; }
        public int CarpoDsAttributeTypeId { get; set; }
        public string AttrType { get; set; }
        public string AttrValue { get; set; }
        public int CarpoDsElementId { get; set; }
        public string ElementName { get; set; }
    }
}
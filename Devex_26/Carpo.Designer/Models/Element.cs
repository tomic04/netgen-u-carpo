using System.Xml;

namespace Carpo.Designer.Models
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
}
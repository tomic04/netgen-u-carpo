using System.Xml;

namespace Carpo.Designer.Models
{
    public class FileDef
    {
        public XmlDocument xDoc { get; set; } = new XmlDocument();
        public string xDocFile { get; set; }
    }
}
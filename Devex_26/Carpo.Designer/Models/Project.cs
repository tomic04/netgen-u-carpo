using System.Xml;

namespace Carpo.Designer.Models
{
    public class ProjectTreeNode
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public XmlNode Node { get; set; }
        public Enums.Project.ProjectItem ItemType { get; set; }
        public FileDef ProjectFile { get; set; }

        public FileDef ItemFile { get; set; }
    }
}
using System.Windows.Forms;

namespace Carpo.Structure.Models
{
    public class JControlDefinition : IJControlDefinition
    {
        public Control JControl { get; set; }
        public string CustomName { get; set; }
        public object Value { get; set; }
    }

    public class JRepositoryDefinition : IJControlDefinition
    {
        public DevExpress.XtraEditors.Repository.RepositoryItemComboBox JControl { get; set; }
        public string CustomName { get; set; }
        public object Value { get; set; }
    }

    public interface IJControlDefinition
    {
    }
}
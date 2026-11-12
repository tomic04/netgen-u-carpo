using System.Data;
using System.Windows.Forms;

namespace Carpo.Structure.Models
{
    public class JBarButtonItemTag
    {
        public DataRow DataRowRef { get; set; }
        public Control ControlRef { get; set; }
    }

    public class NameType
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
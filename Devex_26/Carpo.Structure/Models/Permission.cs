using System.Collections.Generic;

namespace Carpo.Structure.Models
{
    public class Permission
    {
        public string ObjectName { get; set; }
        public string Function { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public List<string> SubObjects { get; set; }
    }
}
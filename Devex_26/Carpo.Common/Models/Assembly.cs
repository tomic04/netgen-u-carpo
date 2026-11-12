using System.Collections.Generic;

namespace Carpo.Common.Models
{
    public class Assembly
    {
        public string Dll { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public List<object> Params { get; set; }
    }
}
using System.Collections.Generic;
using System.Xml.Linq;

namespace Carpo.XmlGen
{
    public class Global
    {
        public static string ConnectionString { get; set; }
        public static string CreatePath { get; set; }
        public static string ObjectType { get; set; }
        public static string OperateriId { get; set; }
        public static string RepxFolder { get; set; }
        public static Dictionary<string, object> VariablesList { get; set; }
        public static XDocument XmlSchema { get; set; }
    }
}
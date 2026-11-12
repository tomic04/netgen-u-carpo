namespace Carpo.Structure.Models
{
    public class Vars
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string FormName { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public object ValueObj { get; set; } //za potrebe SaveValue="session" vrijednosti
        public string ValueText { get; set; } //za potrebe SaveValue="session" vrijednosti
        public string ValueXml { get; set; }
        public string Version { get; set; }
    }
}
using System.Collections.Generic;
namespace Carpo.NetGen.Log
{
    public static class NgDnevnik
    {
        public static List<string> Linije = new List<string>();
        public static void Sql(string s) { Linije.Add("SQL " + s); }
        public static void Greska(string s) { Linije.Add("ERR " + s); }
        public static void Akcija(string s) { Linije.Add("AKC " + s); }
        public static void Info(string s) { Linije.Add("INF " + s); }
        public static void Preskok(string s) { Linije.Add("PRE " + s); }
    }
}

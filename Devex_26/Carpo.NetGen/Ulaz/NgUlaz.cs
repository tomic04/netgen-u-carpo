using Carpo.NetGen.Model;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen
{
    internal sealed class NgUlazRezultat
    {
        public NgProject Projekt;
        public string Putanja;
        public string WCaption;
        public string Greska;
        public bool Ucitano;
    }

    /// <summary>
    /// Zajednicki load za Loader (podloga) i frm (dijalog).
    /// Nema DMS-a, nema baze netgenxml (to je M5) i nema SQL-a.
    /// moduleId se ne skracuje na ime fajla: ssMeni zna imati podfolder (MER\....xml).
    /// </summary>
    internal static class NgUlaz
    {
        public static NgUlazRezultat Pokreni(string testXmlPath, string moduleId, string remoteDllPath)
        {
            var o = new NgUlazRezultat();
            string putanja = null;
            if (!string.IsNullOrWhiteSpace(testXmlPath))
                putanja = testXmlPath;
            else if (!string.IsNullOrWhiteSpace(remoteDllPath) && !string.IsNullOrWhiteSpace(moduleId))
                putanja = remoteDllPath.TrimEnd('\\', '/') + "\\SharedDLLs\\xml\\" + moduleId;

            if (string.IsNullOrEmpty(putanja))
                return o;

            var r = NetGenXmlReader.Ucitaj(putanja);
            o.Ucitano = true;
            o.Putanja = putanja;
            if (!r.Uspjeh)
            {
                o.Greska = r.Greska;
                return o;
            }

            o.Projekt = r.Projekt;
            o.WCaption = r.Projekt.Naslov ?? "";
            return o;
        }
    }
}

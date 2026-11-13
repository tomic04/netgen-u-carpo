using System.Windows.Forms;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Ulaz granule. NetGen ima jednu podlogu (Form1) za svaku granulu: bez stabla i detalja ti se
    /// paneli sakriju, a ONLOAD ":Forma" otvori dijalog nad podlogom, pa FILLEXPLORER puni master
    /// (Form1.vb 3879-3882, 5445-5453). Zato sve ide u Podlogu (RUT-01). Prije su granule bez stabla,
    /// detalja ili gumba (235) išle u skraćenu formu, a ulaz ":Forma" (230, od toga 228 s FILLEXPLORER)
    /// u samostalan dijalog, pa rezultat filtra nije imao gdje.
    /// </summary>
    public static class Prozor
    {
        public static Form Otvori(string xml, string server, string baza, int operater, out string greska)
        {
            greska = null;
            var r = NetGenXmlReader.Ucitaj(xml);
            if (!r.Uspjeh || r.Projekt == null)
            {
                greska = r.Greska ?? "XML nije učitan.";
                return null;
            }
            var podloga = new Podloga();
            if (!podloga.Prikazi(xml, server, baza, operater))
                greska = podloga.Text;
            return podloga;
        }
    }
}

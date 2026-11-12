using System.Collections.Generic;
using System.IO;
using Carpo.NetGen.TestHooks;

namespace Carpo.NetGen.Granule
{
    /// <summary>
    /// Tri gumba iz retka gumbi/lanac kojima checklista nije imala ID.
    /// ONK-04 je slučaj 4 iz docs/granule/Onclick-dijalog-testovi.md na PR #60.
    /// </summary>
    internal static class OnkGumbi
    {
        public static List<Stavka> Provjeri(string root)
        {
            var l = new List<Stavka>();
            string xml = Path.Combine(root, "definitions", "netgen", "Agronom granule", "ZakljucivanjeTemNG.xml");
            const string opis = "Zaključi otvara Zakljucavanje. MAINFORM ostaje. spKntZakljucivanjeTem se ne šalje";
            const string izvor = "netgen/Form2.vb:6882-6883";
            if (!File.Exists(xml))
            {
                l.Add(Qa.P("ZakljucivanjeTemNG", "ONK-04", opis, izvor, "nema XML"));
                return l;
            }
            Stavka forma = Qa.Runtime("ZakljucivanjeTemNG", "ONK-04", opis, izvor, xml, "kontrola", "MAINFORM.buttonOK", "otvaraFormu", "Zakljucavanje");
            if (forma.Ishod != Ishod.Zeleno)
            {
                l.Add(forma);
                return l;
            }
            Stavka sql = Qa.Runtime("ZakljucivanjeTemNG", "ONK-04", opis, izvor, xml, "kontrola", "MAINFORM.buttonOK", "saljeSubmit", "false");
            if (sql.Ishod != Ishod.Zeleno)
            {
                l.Add(sql);
                return l;
            }
            System.Console.WriteLine("[Z] ZakljucivanjeTemNG ONK-04 otvaraFormu=Zakljucavanje saljeSubmit=false");
            l.Add(Qa.Z("ZakljucivanjeTemNG", "ONK-04", opis, izvor));
            return l;
        }
    }
}

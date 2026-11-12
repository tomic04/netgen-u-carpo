using System.Collections.Generic;

namespace Carpo.NetGen.Granule
{
    internal static class Stablo
    {
        public static Stavka Meni(string granula, string id, string fajl)
        {
            string greska;
            string safe = fajl.Replace("'", "");
            string sql = "select Parametar from ssMeni where Parametar like '%:" + safe
                + "' or Parametar like '%\\" + safe + "' or Parametar like '%/" + safe + "'";
            List<string> redovi = Citanje.Stupac(sql, out greska);
            if (redovi == null)
                return Qa.O(granula, id, "ssMeni (samo citanje) za " + fajl, "dbo.ssMeni", greska);
            if (redovi.Count == 0)
                return Qa.O(granula, id, fajl + " nije u ssMeni.Parametar", "dbo.ssMeni", "0 redova");
            return Qa.Z(granula, id, fajl + " je u ssMeni: " + string.Join(" ; ", redovi.ToArray()), "dbo.ssMeni.Parametar");
        }
    }
}

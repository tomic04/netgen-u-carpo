using System;
using System.Globalization;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Form2.ssPermissionProcitaj 5657. objectname je xml.forma, sort je desc.
    /// Form1.ssPermissionProvjeri ostaje na PR #81 (Form1Ulaz.DozvolaKomande). Ovdje se ne kopira.
    /// </summary>
    public static class DozvolaDijaloga
    {
        public static string Sql(string xmlPut, string forma, int operater)
        {
            string p = xmlPut ?? "";
            int z = p.LastIndexOf('\\');
            string ime = (z < 0 ? p : p.Substring(z + 1)).Trim();
            string obj = ime + "." + (forma ?? "").Trim();
            string op = operater.ToString(CultureInfo.InvariantCulture);
            return "select *,1 as sort from ssPermission where objectname='" + obj
                + "' and ((OperateriID = " + op
                + ") or (GrupeID = (select top 1 GrupeId from ssOperateri where OperateriId = " + op
                + ") )) union select *,2 as sort from ssPermission where objectname='" + obj
                + "' and OperateriID = 0 and grupeid=0 order by sort desc,isgrupa desc,grupeid,operateriid";
        }

        /// <summary>Form2.vb 5705–5712. smije true prekida. false gasi kontrolu.</summary>
        public static bool Smije(bool? red)
        {
            if (red == true)
                return true;
            if (red == false)
                return false;
            return true;
        }

        public static bool Vidljiva(bool? vidljivost, bool sad)
        {
            if (vidljivost == false)
                return false;
            if (vidljivost == true)
                return true;
            return sad;
        }
    }
}

using System;

namespace Carpo.NetGen.Poziv
{
    /// <summary>
    /// ASSEMBLYLOAD i totalgen. Tri potpisa, tilda bira router, povrat se ne čita.
    /// </summary>
    public static class Motora
    {
        public static bool OnLoadedPrijePrikaza(int brojKontrola)
        {
            return brojKontrola == 0;
        }

        public static bool OnLoadedULoadu()
        {
            return true;
        }

        public static string[] Totalgen(string put, string id, string operater, string konekcija, string zadnjiSql, string parametriGrida, string parametriSvi)
        {
            if (put == "dijalog")
                return new[] { id ?? "", operater ?? "", konekcija ?? "", parametriSvi ?? "", parametriSvi ?? "" };
            if (put == "rafalno")
                return new[] { id ?? "", operater ?? "", konekcija ?? "", zadnjiSql ?? "", "" };
            return new[] { id ?? "", operater ?? "", konekcija ?? "", zadnjiSql ?? "", parametriGrida ?? "" };
        }

        public static bool CitaPovrat()
        {
            return false;
        }

        /// <summary>Tilda postaje # i otvara lanac. Bez tilde ostaje jedan poziv. Form1.vb 4558, 5743–5760.</summary>
        public static string Router(string naredba)
        {
            string sirovo = naredba ?? "";
            bool tilda = sirovo.IndexOf('~') >= 0;
            string s = sirovo.Replace("~", "#");
            if (tilda)
                return "lanac:" + s;
            return "jedan:" + s;
        }

        public static string CarpoFormIz(string onLoaded, string defaultForme)
        {
            if (onLoaded == null || onLoaded.IndexOf("ASSEMBLYLOAD", StringComparison.OrdinalIgnoreCase) < 0)
                return "";
            return defaultForme ?? "";
        }
    }
}

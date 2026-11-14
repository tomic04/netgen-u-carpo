using System;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// Klik gumba na podlozi. Form1.Button1_Click 5664 zove NovaGenerickaForma.
    /// REFRESHMASTERGRID osvježi odmah (4570). PRINT* izađe prije forme (4945).
    /// Ostalo je ime forme (4906). Forma bez kontrola ima OKClick i PokaziUpit (5369, 5519),
    /// pa nepostojeći refreshmaster ipak osvježi master.
    /// </summary>
    public static class GumbTok
    {
        public static string Akcija(NgProject projekt, string segment)
        {
            string s = (segment ?? "").Trim();
            if (s.StartsWith(":", StringComparison.Ordinal))
                s = s.Substring(1).Trim();
            if (s.Length == 0)
                return "";
            string u = s.ToUpperInvariant();
            if (FiskalNaredba.Tablica(u) != 0)
                return "fiskal";
            if (ProstorNaredba.Je(u))
                return "prostor";
            if (u.IndexOf("BLAGAJNAENABLE(", StringComparison.Ordinal) >= 0)
                return "blagajna";
            if (OdznaciNaredba.Je(u))
                return "odznaci";
            if (RoditeljNaredba.Je(u))
                return "roditelj";
            if (PraznoNaredba.Je(u))
                return "prazno";
            if (StupacVrijednost.Je(u) || (u.IndexOf("CLICK(", StringComparison.Ordinal) >= 0 && !KlikNaredba.NaPodlozi()))
                return "nista";
            if (u.IndexOf("MSGBOX(", StringComparison.Ordinal) >= 0)
                return "poruka";
            if (u.IndexOf("OPENFORM(", StringComparison.Ordinal) >= 0)
                return "openform";
            if (u.IndexOf("HIDECOLUMNS(", StringComparison.Ordinal) >= 0 || u.IndexOf("SHOWCOLUMNS(", StringComparison.Ordinal) >= 0)
                return "stupci";
            if (u.StartsWith("WORDDOT1", StringComparison.Ordinal))
                return "word";
            if (u.IndexOf("REFRESHMASTERGRID", StringComparison.Ordinal) >= 0)
                return "osvjezi";
            if (u.IndexOf("ASSEMBLYLOAD(", StringComparison.Ordinal) >= 0)
                return "assembly";
            if (u == "CLOSE" || u == "CANCEL")
                return "zatvori";
            if (s.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                return "sql";
            if (u.StartsWith("PRINT", StringComparison.Ordinal))
                return "ispis";
            string ime = s;
            int z = ime.IndexOf('(');
            if (z > 0)
                ime = ime.Substring(0, z).Trim();
            if (ImaFormu(projekt, ime))
                return "forma";
            return "osvjezi";
        }

        private static bool ImaFormu(NgProject projekt, string ime)
        {
            // Forma može biti u INCLUDE datoteci (XML-02).
            return Carpo.NetGen.Xml.IncludeForme.Nadji(projekt, ime) != null;
        }
    }
}

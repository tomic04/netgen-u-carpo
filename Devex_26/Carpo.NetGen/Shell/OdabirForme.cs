using System;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Shell
{
    /// <summary>
    /// Manager predložak samo kad NetGen raspored stvarno jest stablo lijevo,
    /// master-detail grid i gumbi dolje. Inače ostaje obična forma ili dijalog.
    /// </summary>
    public static class OdabirForme
    {
        public static bool JeManager(NgProject projekt)
        {
            if (projekt == null)
                return false;
            ShellLayout s = ShellLayout.Iz(projekt);
            string detail = (s.DetailSql ?? "").Trim();
            bool detailGrid = detail.Length > 0 && !detail.StartsWith(":", StringComparison.Ordinal);
            bool stablo = s.ImaStablo && s.StabloVidljivo;
            bool gumbi = projekt.Gumbi != null && projekt.Gumbi.Count > 0;
            if (!gumbi && !string.IsNullOrWhiteSpace(projekt.Buttons))
                gumbi = true;
            return stablo && detailGrid && gumbi;
        }

        public static string Ulaz(NgProject projekt)
        {
            if (projekt == null || projekt.Atributi == null)
                return "";
            string stora = projekt.Atributi.Ima("STARTUPSTORA") ? (projekt.Atributi.Vrijednost("STARTUPSTORA") ?? "").Trim() : "";
            if (projekt.Atributi.Ima("ONLOAD"))
                return (projekt.Atributi.Vrijednost("ONLOAD") ?? "").Trim();
            return stora;
        }
    }
}

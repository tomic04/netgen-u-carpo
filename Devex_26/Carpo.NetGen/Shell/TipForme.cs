using Carpo.NetGen.Model;

namespace Carpo.NetGen.Shell
{
    /// <summary>
    /// NetGen ima malo obrazaca. Manager (stablo lijevo, master, detalj, gumbi dolje)
    /// vrijedi samo kad XML stvarno ima lijevi panel, DETAILGRID i gumbe.
    /// </summary>
    public static class TipForme
    {
        public const string Manager = "manager";
        public const string StabloMaster = "stablo-master";
        public const string SamoDetail = "samo-detail";
        public const string SamoGrid = "samo-grid";
        public const string Dijalog = "dijalog";
        public const string Ostalo = "ostalo";

        public static bool JeManager(NgProject projekt)
        {
            return Odredi(projekt) == Manager;
        }

        public static string Odredi(NgProject projekt)
        {
            if (projekt == null)
                return Ostalo;
            bool lijevo = Ima(projekt.Tree) || ImaAtribut(projekt, "COMBO");
            bool detail = Ima(projekt.DetailGrid);
            bool gumbi = (projekt.Gumbi != null && projekt.Gumbi.Count > 0) || Ima(projekt.Buttons);
            string on = (projekt.OnLoad ?? "").Trim();
            if (lijevo && detail && gumbi)
                return Manager;
            if (lijevo && !detail)
                return StabloMaster;
            if (!lijevo && detail)
                return SamoDetail;
            if (!lijevo && !detail && on.StartsWith(":"))
                return Dijalog;
            if (!lijevo && !detail)
                return SamoGrid;
            return Ostalo;
        }

        private static bool Ima(string tekst)
        {
            return !string.IsNullOrWhiteSpace(tekst);
        }

        private static bool ImaAtribut(NgProject projekt, string ime)
        {
            if (projekt.Atributi == null || !projekt.Atributi.Ima(ime))
                return false;
            return !string.IsNullOrWhiteSpace(projekt.Atributi.Vrijednost(ime));
        }
    }
}

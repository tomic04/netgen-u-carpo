using Carpo.NetGen.Model;

namespace Carpo.NetGen.Shell
{
    /// <summary>
    /// Podloga NetGena (Form1) je uvijek isti okvir: lijevo stablo, desno master grid,
    /// ispod njega detail grid, gumbi ispod. Oko 90% granula zivi u tom okviru.
    /// M1 crta prvo ovo. Prazan TREE ne znaci drugi raspored, samo prazno stablo.
    /// Piksel TOP/LEFT nije cilj.
    /// </summary>
    public sealed class ShellLayout
    {
        public const string Obrazac = "LijevoStabloMasterDetail";

        private ShellLayout(string treeSql, string masterSql, string detailSql, string buttons)
        {
            TreeSql = treeSql ?? "";
            MasterSql = masterSql ?? "";
            DetailSql = detailSql ?? "";
            Buttons = buttons ?? "";
        }

        public string ObrazacIme { get { return Obrazac; } }

        /// <summary>Lijeva regija postoji uvijek, kao TreeView na Form1.</summary>
        public bool LijevoStablo { get { return true; } }

        public bool MasterGrid { get { return true; } }
        public bool DetailGrid { get { return true; } }

        public string TreeSql { get; private set; }
        public string MasterSql { get; private set; }
        public string DetailSql { get; private set; }
        public string Buttons { get; private set; }

        public bool ImaStablo { get { return TreeSql.Length > 0; } }
        public bool ImaDetail { get { return DetailSql.Length > 0; } }
        public bool ImaMasterSql { get { return MasterSql.Length > 0; } }

        public static ShellLayout Iz(NgProject projekt)
        {
            if (projekt == null)
                return new ShellLayout("", "", "", "");
            return new ShellLayout(projekt.Tree, projekt.OnLoad, projekt.DetailGrid, projekt.Buttons);
        }
    }
}

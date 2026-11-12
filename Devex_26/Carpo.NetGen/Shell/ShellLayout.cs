using Carpo.NetGen.Model;

namespace Carpo.NetGen.Shell
{
    /// <summary>
    /// Podloga NetGena (Form1): lijevo stablo, desno master, ispod detail.
    /// Prazan TREE i prazan COMBO sakriju TreeView2 i lijevi splitter stave na 0
    /// (Form1.vb 4310–4316). To nije „stablo ostaje, samo prazno“.
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

        /// <summary>TreeView2 je vidljiv. Prazan TREE uz prazan COMBO ovo gasi.</summary>
        public bool LijevoStablo { get { return StabloVidljivo; } }

        public bool MasterGrid { get { return true; } }
        public bool DetailGrid { get { return true; } }

        public string TreeSql { get; private set; }
        public string MasterSql { get; private set; }
        public string DetailSql { get; private set; }
        public string Buttons { get; private set; }

        public bool ImaStablo { get { return TreeSql.Trim().Length > 0; } }
        public bool ImaDetail { get { return DetailSql.Trim().Length > 0; } }
        public bool ImaMasterSql { get { return MasterSql.Trim().Length > 0; } }

        public bool StabloVidljivo { get; private set; }
        public int LijeviSplitter { get; private set; }
        public bool GumbDrvoEnabled { get; private set; }
        public bool ComboVidljiv { get; private set; }

        /// <summary>Nema DETAILGRID: SplitContainer2 ide na visinu forme minus 50 (Form1.vb 2111–2113).</summary>
        public bool DetailSplitterNaDno { get; private set; }

        public static ShellLayout Iz(NgProject projekt)
        {
            if (projekt == null)
                return new ShellLayout("", "", "", "");
            var s = new ShellLayout(projekt.Tree ?? "", projekt.OnLoad ?? "", projekt.DetailGrid ?? "", projekt.Buttons ?? "");
            string combo = "";
            if (projekt.Atributi != null && projekt.Atributi.Ima("COMBO"))
                combo = projekt.Atributi.Vrijednost("COMBO") ?? "";
            s.ComboVidljiv = combo.Trim().Length > 0;
            bool treePrazan = s.TreeSql.Trim().Length == 0;
            bool sakrij = treePrazan && !s.ComboVidljiv;
            s.StabloVidljivo = !sakrij;
            if (sakrij)
            {
                s.GumbDrvoEnabled = false;
                s.LijeviSplitter = 0;
            }
            else if (treePrazan)
            {
                s.GumbDrvoEnabled = false;
                s.LijeviSplitter = 243;
            }
            else
            {
                s.GumbDrvoEnabled = true;
                int d = 290;
                string raw = projekt.Atributi == null ? null : projekt.Atributi.Vrijednost("SPLITTERDISTANCE");
                int parsed;
                if (!string.IsNullOrEmpty(raw) && int.TryParse(raw.Trim(), out parsed))
                    d = parsed;
                s.LijeviSplitter = d;
            }
            s.DetailSplitterNaDno = s.DetailSql.Trim().Length == 0;
            return s;
        }
    }
}

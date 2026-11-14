using System;
using System.Data;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;
using DevExpress.XtraGrid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Ispis grida dijaloga (ISP-07): PRINTGRID(ime) u lancu ili gumbu (Form2.vb 6950-6955 → IspisGRIDREPX
    /// 9041) i "Ispis" u izborniku grida (IspisNovi 9015). TempGrid je imenovani grid, zaglavlja su
    /// PAGEHEADERLEFT/CENTER/RIGHT tog grida kroz SrediSQL2 i SrediSQLPosljeNextForme s datumom za ispis
    /// (prazan centar je naslov forme), a PAGEORIENTATION je atribut grida (LANDSCAPE u 344 grida).
    /// Kao podloga, ide kroz Spin.Printing PrintLink (50-50 → Carpo).
    /// </summary>
    public sealed partial class Dijalog
    {
        private bool IspisiGrid(string naredba, GridControl grid)
        {
            if (grid == null)
            {
                foreach (Control c in SveKontrole())
                {
                    grid = c as GridControl;
                    if (grid != null)
                        break;
                }
            }
            NgControl k = grid == null ? null : NadjiKontrolu(grid.Name);
            string orij = Attr(k, "PAGEORIENTATION");
            if (orij.Length == 0)
                orij = AtributForme("PAGEORIENTATION");
            DataTable tab = grid == null ? null : grid.DataSource as DataTable;
            return Carpo.NetGen.Print.PrintBridge.Ispisi(naredba, true, Id, Operater, Konekcija, "", ZadnjiSql, "", SviParametri(),
                ZaglavljeGrida(k, "PAGEHEADERLEFT"), ZaglavljeGrida(k, "PAGEHEADERCENTER"), ZaglavljeGrida(k, "PAGEHEADERRIGHT"),
                Text, grid, orij, tab);
        }

        /// <summary>Form2.vb 6951-6954: PRINTGRID(ime) → Controls(ime). Bez imena ili kontrole: null.</summary>
        private GridControl GridIzNaredbe(string naredba)
        {
            string ime = ImeGridaIspisa(naredba);
            if (ime.Length == 0)
                return null;
            Control[] nadjeno = Controls.Find(ime, true);
            return nadjeno.Length > 0 ? nadjeno[0] as GridControl : null;
        }

        /// <summary>"PRINTGRID(karticagrid)" → "karticagrid"; isto za PRINTREPXGRID.</summary>
        private static string ImeGridaIspisa(string naredba)
        {
            string n = (naredba ?? "").Trim();
            int p = n.IndexOf("GRID(", StringComparison.OrdinalIgnoreCase);
            if (p < 0)
                return "";
            string ime = n.Substring(p + 5);
            int z = ime.IndexOf(')');
            if (z >= 0)
                ime = ime.Substring(0, z);
            return ime.Trim();
        }

        /// <summary>Rafalna_Priprema(SrediSQLPosljeNextForme(SrediSQL2(atribut), , True)); ## i !! su novi red u PrintBridge.</summary>
        private string ZaglavljeGrida(NgControl k, string atribut)
        {
            string sirovo = Attr(k, atribut);
            if (sirovo.Length == 0)
                return "";
            return SrediPoslije.Zamijeni(sirovo, PokupiParametre(), _kontekst ?? new SqlKontekst(), _kontekst == null ? null : _kontekst.Clipboard, true);
        }
    }
}

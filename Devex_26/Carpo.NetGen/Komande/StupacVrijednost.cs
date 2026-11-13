using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// GRID_COLUMN_VALUE. Form1.vb 6785–6807 je cijelo komentirano.
    /// Segment se ne izvršava i ne osvježava master.
    /// </summary>
    public static class StupacVrijednost
    {
        public static bool Je(string naredba)
        {
            return (naredba ?? "").ToUpperInvariant().IndexOf("GRID_COLUMN_VALUE(", StringComparison.Ordinal) >= 0;
        }

        public static bool Izvrsava()
        {
            return false;
        }
    }
}

using System.Collections.Generic;
using System.Data;
using Carpo.NetGen.Grid;

namespace Carpo.NetGen.Granule
{
    public static class GridRed
    {
        public static List<Stavka> Provjeri()
        {
            var l = new List<Stavka>();
            var t = new DataTable();
            t.Columns.Add("id", typeof(int));
            t.Columns.Add("statusiid", typeof(int));
            t.Columns.Add("Korak", typeof(string));
            t.Rows.Add(7, 5, "Radni");
            string parametri = SqlTekst.IzReda(t.Rows[0]);
            var k = new SqlKontekst();
            k.ParametriSaGrida = parametri;
            string sql = SqlTekst.Sredi("spStatusiQuery @action=2, @ID=@@statusiid", k);
            bool ima = sql.IndexOf("@@statusiid", System.StringComparison.OrdinalIgnoreCase) < 0
                && sql.IndexOf("@ID='5'", System.StringComparison.Ordinal) >= 0
                && parametri.IndexOf("@statusiid='5'", System.StringComparison.Ordinal) >= 0;
            l.Add(ima
                ? Qa.Z("GridRed", "GRD-01", "@@statusiid je stupac retka, vrijednost je '5'", "netgen/Form1.vb:2556-2576; netgen/Class1.vb:765-849")
                : Qa.P("GridRed", "GRD-01", "@@statusiid iz grida", "netgen/Form1.vb:2556-2576; netgen/Class1.vb:765-849", sql + " | " + parametri));
            return l;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;

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

            var kontrole = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            kontrole["Naziv"] = "klik-proba";
            string unos = DijalogTok.ZamijeniTokene("spJeziciAzur @action=0, @id=@@id, @Naziv='@@Naziv'", "0", kontrole);
            bool token = unos.IndexOf("@@", StringComparison.Ordinal) < 0
                && unos.IndexOf("@Naziv='klik-proba'", StringComparison.Ordinal) >= 0
                && unos.IndexOf("@id=0", StringComparison.Ordinal) >= 0;
            l.Add(token
                ? Qa.Z("GridRed", "GRD-02", "@@Naziv iz kontrole uđe u storu kao 'klik-proba'", "netgen/Form2.vb:3367,3406,3655")
                : Qa.P("GridRed", "GRD-02", "@@Naziv iz kontrole", "netgen/Form2.vb:3367,3406,3655", unos));
            return l;
        }
    }
}

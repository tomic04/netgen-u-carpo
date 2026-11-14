using System;

namespace Carpo.NetGen.Grid
{
    /// <summary>
    /// Janus AggregateFunction u ukupnom retku (TotalRow). Class1.CreateGridColumns 1171: decimal/double
    /// stupac ima Sum i TotalFormatString "#,##0.00". COLUMN AGREGATEFUNCTION (Form1.vb 7507) to mijenja:
    /// 0 None (454 stupca), 1 Count, 2 Sum (količine, vrijednosti), 3 Average (cijene, "pros."),
    /// 4 Min, 5 Max, ili ime ("SUM"). TOT-01.
    /// </summary>
    public static class GridAgregat
    {
        public const string Nema = "";

        public static string Vrsta(string agregat)
        {
            string a = (agregat ?? "").Trim().ToLowerInvariant();
            switch (a)
            {
                case "1":
                case "count":
                    return "Count";
                case "2":
                case "sum":
                    return "Sum";
                case "3":
                case "average":
                case "avg":
                    return "Average";
                case "4":
                case "min":
                    return "Min";
                case "5":
                case "max":
                    return "Max";
                default:
                    return Nema;
            }
        }

        /// <summary>Decimalni stupac zadrži "#,##0.00" (TotalFormatString), ostali format stupca.</summary>
        public static string Format(string vrsta, Type tip, string formatStupca)
        {
            if (vrsta == "Count")
                return "{0}";
            string t = tip == null ? "" : tip.Name.ToLowerInvariant();
            if (t.Contains("decimal") || t.Contains("double") || t.Contains("single"))
                return "{0:#,##0.00}";
            if (!string.IsNullOrEmpty(formatStupca))
                return "{0:" + formatStupca + "}";
            return "{0}";
        }

        /// <summary>
        /// Janus FrozenColumns broji i stupac ikone i kvačicu (CreateGridColumns 1009-1030): u DevExpressu
        /// ostaje toliko podatkovnih stupaca s lijeva.
        /// </summary>
        public static int ZamrznutiPodaci(int? frozen, bool kvacica)
        {
            if (!frozen.HasValue || frozen.Value <= 0)
                return 0;
            int n = frozen.Value - 1 - (kvacica ? 1 : 0);
            return n < 0 ? 0 : n;
        }
    }
}

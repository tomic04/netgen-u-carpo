// Stub PrintBridge za diferencijalni test: samo ciste funkcije iz Print/PrintBridge.cs.
using System;
namespace Carpo.NetGen.Print
{
    public static class PrintBridge
    {
        public static string Vrsta(string naredba, bool dijalog)
        {
            string u = (naredba ?? "").Trim();
            if (u.StartsWith(":", StringComparison.Ordinal))
                u = u.Substring(1).Trim();
            if (dijalog && (u.StartsWith("PRINTGRID", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXGRID", StringComparison.OrdinalIgnoreCase)))
                return "lodani";
            if (u.StartsWith("PRINTDET", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXDET", StringComparison.OrdinalIgnoreCase))
                return "detail";
            if (u.StartsWith("PRINTMASTER", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXMASTER", StringComparison.OrdinalIgnoreCase)
                || u.StartsWith("PRINTGRID", StringComparison.OrdinalIgnoreCase) || u.StartsWith("PRINTREPXGRID", StringComparison.OrdinalIgnoreCase))
                return "master";
            return "";
        }
        public static bool JePredlozak(string naredba)
        {
            string u = (naredba ?? "").ToLowerInvariant();
            return u.IndexOf("repx:", StringComparison.Ordinal) >= 0 || u.IndexOf("repx/", StringComparison.Ordinal) >= 0;
        }
    }
}

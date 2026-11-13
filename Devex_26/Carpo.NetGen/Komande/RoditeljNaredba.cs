using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>CLOSEPARENT. Form1.vb 6838–6840. bCloseParent i zatvaranje.</summary>
    public static class RoditeljNaredba
    {
        public static bool Je(string naredba)
        {
            return string.Equals((naredba ?? "").Trim(), "CLOSEPARENT", StringComparison.OrdinalIgnoreCase);
        }
    }
}

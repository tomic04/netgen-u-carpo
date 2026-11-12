using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>UNCHECKMASTERGRID. Form1.vb 4626–4633. Sve kvačice se gase.</summary>
    public static class OdznaciNaredba
    {
        public static bool Je(string naredba)
        {
            return string.Equals((naredba ?? "").Trim(), "UNCHECKMASTERGRID", StringComparison.OrdinalIgnoreCase);
        }

        public static void Odznaci(bool[] cekirano)
        {
            if (cekirano == null)
                return;
            for (int i = 0; i < cekirano.Length; i++)
                cekirano[i] = false;
        }
    }
}

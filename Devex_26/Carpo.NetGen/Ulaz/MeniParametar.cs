using System;

namespace Carpo.NetGen
{
    /// <summary>
    /// Jupiter iz ssMeni.Parametar uzme tekst iza druge dvotocke i preda ga kao moduleId.
    /// Form1.ModuleID (1251–1258) odsiječe argumente u zagradi i spremi Mid.
    /// </summary>
    public static class MeniParametar
    {
        public static string ModuleId(string parametar)
        {
            if (string.IsNullOrEmpty(parametar))
                return "";
            int a = parametar.IndexOf(':');
            if (a < 0 || a >= parametar.Length - 1)
                return "";
            int b = parametar.IndexOf(':', a + 1);
            if (b < 0 || b >= parametar.Length - 1)
                return "";
            string module = parametar.Substring(b + 1).Trim();
            if (module.EndsWith(")", StringComparison.Ordinal))
            {
                int z = module.IndexOf('(');
                if (z > 0)
                    module = module.Substring(0, z);
            }
            return module;
        }
    }
}

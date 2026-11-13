using System;

namespace Carpo.NetGen.Ui
{
    /// <summary>Form2.SrediSQL2 2888: @@VERSION je "NetGen v" + Major.Minor.Revision sklopa (STS-01).</summary>
    public static class NgVerzija
    {
        public static string Tekst()
        {
            Version v = typeof(NgVerzija).Assembly.GetName().Version;
            return "NetGen v" + v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Revision.ToString();
        }
    }
}

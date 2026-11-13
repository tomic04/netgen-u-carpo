using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// ONLOAD2: redak SQL-a, stupac je natpis gumba. Form1.vb 3914–3928, 4013–4035.
    /// Prazno se preskače. enable/disable/visible/invisible/refresh su naredbe.
    /// Sve ostalo postaje novi natpis. Traženje riječi u tekstu SQL-a nije to.
    /// </summary>
    public static class OnLoad2
    {
        public static string Akcija(string celija)
        {
            string t = (celija ?? "").Trim().ToLowerInvariant();
            if (t.Length == 0)
                return "";
            if (t == "enable" || t == "disable" || t == "visible" || t == "invisible" || t == "refresh")
                return t;
            return "natpis";
        }
    }
}

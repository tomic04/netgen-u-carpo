using System.Collections.Generic;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Shell
{
    /// <summary>
    /// CMD se složi prvi. BUTTONS segment j prepiše gumb j+1.
    /// Button7 (j=6) samo ako je CMD7 prazan. Form1.vb 3314–3330 i 3770–3824.
    /// Segment bez dvotočke se preskoči (On Error Resume Next na 3065).
    /// </summary>
    public static class GumbiNiz
    {
        public static List<NgButton> Slozi(IList<NgButton> cmd, string buttons)
        {
            var natpis = new string[10];
            var naredba = new string[10];
            for (int i = 0; i < 10; i++)
            {
                natpis[i] = "";
                naredba[i] = "";
            }
            if (cmd != null)
            {
                for (int i = 0; i < cmd.Count; i++)
                {
                    NgButton g = cmd[i];
                    if (g == null || g.Redni < 1 || g.Redni > 10)
                        continue;
                    natpis[g.Redni - 1] = g.Natpis ?? "";
                    naredba[g.Redni - 1] = g.Naredba ?? "";
                }
            }
            if (!string.IsNullOrEmpty(buttons))
            {
                string[] a = buttons.Split('#');
                int kraj = a.Length < 10 ? a.Length : 10;
                for (int j = 0; j < kraj; j++)
                {
                    if (j == 6 && natpis[6].Trim().Length > 0)
                        continue;
                    int kolon = a[j].IndexOf(':');
                    if (kolon < 0)
                        continue;
                    natpis[j] = a[j].Substring(0, kolon);
                    naredba[j] = a[j].Substring(kolon + 1);
                }
            }
            var izlaz = new List<NgButton>();
            for (int i = 0; i < 10; i++)
            {
                if (natpis[i].Length == 0)
                    continue;
                izlaz.Add(new NgButton(i + 1, natpis[i], naredba[i]));
            }
            return izlaz;
        }
    }
}

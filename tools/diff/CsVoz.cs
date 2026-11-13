// Pokretac C# SQL sloja za diferencijalni test (mono). Ulaz i izlaz su base64 da prijelomi i tabovi prezive.
// Red ulaza: id \t sql \t svi \t cb \t datum   (cb: kljuc\x1Fvrijednost\x1E...)
// Red izlaza: id \t rezultat  ili  id \t !ImeIznimke
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;

internal static class CsVoz
{
    private static string D(string b) { return Encoding.UTF8.GetString(Convert.FromBase64String(b)); }
    private static string E(string s) { return Convert.ToBase64String(Encoding.UTF8.GetBytes(s ?? "")); }

    private static int Main(string[] args)
    {
        var k = new SqlKontekst();
        k.Id = "12"; k.OperaterId = "7"; k.DetId = "3"; k.Node = "N1"; k.TempString = "TMP";
        k.EditedColumn = "Kol"; k.LastValue = "LV"; k.DblClickedColumn = "Dbl"; k.Version = "NetGen v10.1.17";
        k.OpisniParametri = "OPIS"; k.FocusedControl = "ctl"; k.SelectedFile = "F.txt"; k.SelectedPath = "C:\\p";
        k.XmlName = "Test.xml"; k.JupiterSrvPath = "\\\\srv\\j"; k.SelectedColor = "255";
        string nacin = args.Length > 0 ? args[0] : "zamijeni";
        string red;
        var izlaz = new StringBuilder();
        while ((red = Console.In.ReadLine()) != null)
        {
            string[] p = red.Split('\t');
            if (p.Length < 5) continue;
            string sql = D(p[1]), svi = D(p[2]), cbTekst = D(p[3]);
            bool datum = p[4] == "1";
            var cb = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string par in cbTekst.Split('\x1E'))
            {
                if (par.Length == 0) continue;
                string[] kv = par.Split('\x1F');
                if (!cb.ContainsKey(kv[0])) cb[kv[0]] = kv.Length > 1 ? kv[1] : "";
            }
            string rez;
            try
            {
                if (nacin == "zamijeni")
                    rez = SrediPoslije.Zamijeni(sql, svi, k, cb, datum);
                else if (nacin == "detail")
                {
                    // DetailGrid.Slozi 75-77: kontekst dijeli ParametriSaGrida s masterom.
                    k.ParametriSaGrida = svi;
                    rez = SrediPoslije.Petlja(SqlTekst.Sredi(sql, k), svi, null, false).Replace("~~", ",");
                    k.ParametriSaGrida = "";
                }
                else if (nacin == "nadji")
                    rez = SrediPoslije.Nadji(sql, svi, cb, datum);
                else
                    rez = "!nepoznat nacin";
                izlaz.Append(p[0]).Append('\t').Append(E(rez)).Append('\n');
            }
            catch (Exception ex)
            {
                izlaz.Append(p[0]).Append('\t').Append("!" + ex.GetType().Name).Append('\n');
            }
        }
        Console.Out.Write(izlaz.ToString());
        return 0;
    }
}

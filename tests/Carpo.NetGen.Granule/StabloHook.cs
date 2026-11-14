using System;
using System.Collections.Generic;
using System.IO;

namespace Carpo.NetGen.Granule
{
    internal static class Stablo
    {
        public static Stavka Meni(string granula, string id, string fajl)
        {
            string greska;
            string safe = fajl.Replace("'", "");
            string sql = "select Parametar from ssMeni where Parametar like '%:" + safe
                + "' or Parametar like '%\\" + safe + "' or Parametar like '%/" + safe + "'";
            List<string> redovi = Citanje.Stupac(sql, out greska);
            if (redovi == null)
                return Qa.O(granula, id, "ssMeni (samo citanje) za " + fajl, "dbo.ssMeni", greska);
            if (redovi.Count == 0)
                return Qa.O(granula, id, fajl + " nije u ssMeni.Parametar", "dbo.ssMeni", "0 redova");
            return Qa.O(granula, id, fajl + " je u ssMeni, ali to jos nije otvaranje granule", "netgen/Form1.vb:1139,1251-1258", redovi[0]);
        }

        /// <summary>
        /// Jupiter iz Parametra uzme tekst iza druge dvotocke i preda ga kao moduleId.
        /// NetGen: Loader.vb 183 stavi ModuleID, Form1.vb 1251-1258 spremi Mid,
        /// Form1.vb 1139 otvori remotedllpath\SharedDLLs\xml\ + Mid.
        /// </summary>
        public static Stavka Otvori(string granula, string id, string fajl, string root)
        {
            const string izvor = "netgen/Loader.vb:183; netgen/Form1.vb:1139,1251-1258";
            string greska;
            string safe = fajl.Replace("'", "");
            List<string> redovi = Citanje.Stupac(
                "select Parametar from ssMeni where Parametar like '%:" + safe + "'", out greska);
            if (redovi == null)
                return Qa.O(granula, id, "ssMeni", "dbo.ssMeni", greska);
            if (redovi.Count == 0)
                return Qa.O(granula, id, fajl + " nije u ssMeni.Parametar", "dbo.ssMeni", "0 redova");
            string parametar = redovi[0];
            string moduleId = Modul(parametar);
            if (moduleId.Length == 0 || !moduleId.EndsWith(fajl, StringComparison.OrdinalIgnoreCase))
                return Qa.P(granula, id, "Parametar daje ime XML-a", izvor, parametar);
            string datoteka = Nadji(Path.Combine(root, "definitions", "netgen"), fajl);
            if (datoteka == null)
                return Qa.O(granula, id, fajl + " nije u repou", izvor, "nema datoteke");
            string remote = Path.Combine(Path.GetTempPath(), "ngmeni-" + Guid.NewGuid().ToString("N"));
            string cilj = Path.Combine(remote, "SharedDLLs", "xml", moduleId.Replace('/', '\\').Replace('/', '\\'));
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(cilj));
                File.Copy(datoteka, cilj, true);
                var loader = new Loader();
                // Provjera puta iz Parametra: samo model, bez prozora (M1 Run8 inace otvori Podlogu).
                loader.BezProzora = true;
                loader.moduleId = moduleId;
                loader.Run8("localhost", "spin", "", "", 7, 0, 0, 0, remote);
                if (loader.Projekt == null)
                    return Qa.P(granula, id, "Loader otvori XML iz Parametra", izvor, loader.ZadnjaGreska ?? "nema projekta");
                string g2;
                var izvorXml = Qa.Ucitaj(datoteka, out g2);
                if (izvorXml == null)
                    return Qa.P(granula, id, "XML iz repoa", izvor, g2);
                if ((loader.Projekt.OnLoad ?? "") != (izvorXml.OnLoad ?? ""))
                    return Qa.P(granula, id, "ONLOAD ucitanog XML-a", izvor, loader.Projekt.OnLoad ?? "");
                return Qa.Z(granula, id, parametar + " -> " + moduleId + " otvoren", izvor);
            }
            finally
            {
                try { if (Directory.Exists(remote)) Directory.Delete(remote, true); } catch (Exception) { }
            }
        }

        private static string Modul(string parametar)
        {
            if (string.IsNullOrEmpty(parametar))
                return "";
            int a = parametar.IndexOf(':');
            if (a < 0)
                return "";
            int b = parametar.IndexOf(':', a + 1);
            if (b < 0 || b + 1 >= parametar.Length)
                return "";
            return parametar.Substring(b + 1).Trim();
        }

        private static string Nadji(string korijen, string fajl)
        {
            if (!Directory.Exists(korijen))
                return null;
            string[] svi = Directory.GetFiles(korijen, fajl, SearchOption.AllDirectories);
            return svi.Length == 0 ? null : svi[0];
        }
    }
}

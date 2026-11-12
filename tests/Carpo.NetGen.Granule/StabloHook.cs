using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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
            string parametar = Parametar(fajl, out greska);
            if (greska != null && parametar == null)
                return Qa.O(granula, id, "ssMeni", "dbo.ssMeni", greska);
            if (parametar == null)
                return Qa.O(granula, id, fajl + " nije u ssMeni.Parametar", "dbo.ssMeni", "0 redova");
            string moduleId = Modul(parametar);
            if (moduleId.Length == 0 || !moduleId.EndsWith(fajl, StringComparison.OrdinalIgnoreCase))
                return Qa.P(granula, id, "Parametar daje ime XML-a", izvor, parametar);
            string host = Host(parametar);
            if (host.IndexOf("carpo.start", StringComparison.OrdinalIgnoreCase) >= 0)
                return OtvoriCarpo(granula, id, fajl, root, parametar, moduleId);
            string datoteka = Nadji(Path.Combine(root, "definitions", "netgen"), fajl, moduleId);
            if (datoteka == null)
                return Qa.O(granula, id, fajl + " nije u repou", izvor, "nema datoteke");
            string remote = Path.Combine(Path.GetTempPath(), "ngmeni-" + Guid.NewGuid().ToString("N"));
            string cilj = Path.Combine(remote, "SharedDLLs", "xml", moduleId.Replace('/', '\\').Replace('/', '\\'));
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(cilj));
                File.Copy(datoteka, cilj, true);
                var loader = new Loader();
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

        /// <summary>
        /// asm:Carpo.Start:&lt;xml&gt; ne ide u NetGen. Starter.cs 169-172:
        /// bez podfoldera putanja je RemoteDllPath\Carpo\ + ModuleId.
        /// </summary>
        private static Stavka OtvoriCarpo(string granula, string id, string fajl, string root, string parametar, string moduleId)
        {
            const string izvor = "Devex_26/Carpo.UI/Connector/Abstract/Starter.cs:169-172";
            string list = Path.GetFileName(moduleId.Replace('/', '\\'));
            string datoteka = Path.Combine(root, "definitions", "carpo", list);
            if (!File.Exists(datoteka))
                datoteka = Nadji(Path.Combine(root, "definitions", "carpo"), list, moduleId);
            if (datoteka == null)
                return Qa.O(granula, id, fajl + " nije u definitions/carpo", izvor, parametar);
            string remote = Path.Combine(Path.GetTempPath(), "ngmeni-" + Guid.NewGuid().ToString("N"));
            string rel = moduleId.Replace('/', '\\');
            string cilj = rel.IndexOf('\\') >= 0
                ? Path.Combine(remote, rel)
                : Path.Combine(remote, "Carpo", rel);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(cilj));
                File.Copy(datoteka, cilj, true);
                string a = Master(datoteka);
                string b = Master(cilj);
                if (a == null || b == null)
                    return Qa.P(granula, id, "Carpo master forma", izvor, parametar);
                if (a != b)
                    return Qa.P(granula, id, "master forma se ne slaze", izvor, b);
                return Qa.Z(granula, id, parametar + " -> Carpo\\" + moduleId + " " + a, izvor);
            }
            catch (Exception ex)
            {
                return Qa.P(granula, id, "Carpo XML se ucita", izvor, ex.Message);
            }
            finally
            {
                try { if (Directory.Exists(remote)) Directory.Delete(remote, true); } catch (Exception) { }
            }
        }

        private static string Master(string putanja)
        {
            var doc = new XmlDocument();
            doc.Load(putanja);
            XmlNode n = doc.SelectSingleNode("//form[@type='master']");
            if (n == null || n.Attributes == null)
                return null;
            string ime = n.Attributes["name"] == null ? "" : n.Attributes["name"].Value;
            string cap = n.Attributes["caption"] == null ? "" : n.Attributes["caption"].Value;
            if (ime.Length == 0)
                return null;
            return ime + " / " + cap;
        }

        private static string Parametar(string fajl, out string greska)
        {
            string safe = fajl.Replace("'", "");
            List<string> redovi = Citanje.Stupac(
                "select Parametar from ssMeni where Parametar like '%:" + safe + "'", out greska);
            if (redovi == null)
                return null;
            if (redovi.Count == 0)
            {
                redovi = Citanje.Stupac(
                    "select Parametar from ssMeni where Parametar like '%\\" + safe + "' or Parametar like '%/" + safe + "'",
                    out greska);
                if (redovi == null)
                    return null;
            }
            for (int i = 0; i < redovi.Count; i++)
            {
                string m = Modul(redovi[i]);
                if (m.EndsWith(fajl, StringComparison.OrdinalIgnoreCase))
                    return redovi[i];
            }
            return redovi.Count == 0 ? null : redovi[0];
        }

        private static string Host(string parametar)
        {
            int a = parametar.IndexOf(':');
            if (a < 0)
                return "";
            int b = parametar.IndexOf(':', a + 1);
            if (b < 0)
                return "";
            return parametar.Substring(a + 1, b - a - 1);
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

        private static string Nadji(string korijen, string fajl, string moduleId)
        {
            if (!Directory.Exists(korijen))
                return null;
            string[] svi = Directory.GetFiles(korijen, fajl, SearchOption.AllDirectories);
            if (svi.Length == 0)
                return null;
            string rel = moduleId.Replace('/', '\\');
            for (int i = 0; i < svi.Length; i++)
            {
                if (svi[i].EndsWith(rel, StringComparison.OrdinalIgnoreCase))
                    return svi[i];
            }
            Array.Sort(svi, StringComparer.OrdinalIgnoreCase);
            return svi[0];
        }
    }
}

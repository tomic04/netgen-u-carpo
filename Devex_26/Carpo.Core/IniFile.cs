using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Carpo.Core
{
    public static class IniFile
    {
        private static Dictionary<string, string> ini = new Dictionary<string, string>();
        private static List<string> fromDb = new List<string>();

        public static string JupiterClientIniPath { get; internal set; }

        public static void Reload()
        {
            ini.Clear();
        }

        public static Dictionary<string, string> Ini
        {
            get
            {
                if (ini.Count == 0)
                {
                    try
                    {
                        using (StreamReader sr = ReadIni())
                        {
                            if (sr != null)
                            {
                                string lne;
                                while ((lne = sr.ReadLine()) != null)
                                {
                                    lne = lne.Trim();
                                    if (lne != "")
                                    {
                                        string[] slne = lne.Split('=');
                                        try
                                        {
                                            if (slne.GetUpperBound(0) == 0)
                                                ini.Add("Server", lne.Trim());
                                            else
                                                ini.Add(slne[0].Trim(), slne[1].Trim());
                                        }
                                        catch
                                        {
                                            throw;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                if (ini["Server"] != "")
                {
                }
                return ini;
            }
        }

        private static StreamReader ReadIni()
        {
            try
            {
                if (!File.Exists(SpecialFolders.JupiterAppDataFolder + "JupiterClient.ini") && File.Exists(SpecialFolders.AllUsersJupiterAppDataFolder + "JupiterClient.ini"))
                {
                    File.Copy(SpecialFolders.AllUsersJupiterAppDataFolder + "JupiterClient.ini", SpecialFolders.JupiterAppDataFolder + "JupiterClient.ini");
                }
            }
            catch { }

            StreamReader sr;
            try
            {
                if (File.Exists(SpecialFolders.JupiterAppDataFolder + "JupiterClient.ini"))
                {
                    sr = File.OpenText(SpecialFolders.JupiterAppDataFolder + "JupiterClient.ini");
                    if (sr != null)
                    {
                        JupiterClientIniPath = SpecialFolders.JupiterAppDataFolder + "JupiterClient.ini";
                        return sr;
                    }
                }
            }
            catch
            {
                throw;
            }
            try
            {
                if (File.Exists("c:\\JupiterClient\\JupiterClient.ini"))
                {
                    sr = File.OpenText("c:\\JupiterClient\\JupiterClient.ini");
                    if (sr != null)
                    {
                        JupiterClientIniPath = "c:\\JupiterClient\\JupiterClient.ini";
                        return sr;
                    }
                }
            }
            catch
            {
                throw;
            }
            try
            {
                if (File.Exists(SpecialFolders.AllUsersJupiterAppDataFolder + "JupiterClient.ini"))
                {
                    sr = File.OpenText(SpecialFolders.AllUsersJupiterAppDataFolder + "JupiterClient.ini");
                    if (sr != null)
                    {
                        JupiterClientIniPath = SpecialFolders.AllUsersJupiterAppDataFolder + "JupiterClient.ini";
                        return sr;
                    }
                }
            }
            catch
            {
                throw;
            }

            return null;
        }

        public static bool SettingReadOnly(string key)
        {
            return fromDb.Contains(key);
        }

        public static XmlDocument GetInfo()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlElement rootNode = xmlDoc.CreateElement("JupiterClientIni");
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
            xmlDoc.AppendChild(rootNode);
            foreach (string s in ini.Keys)
            {
                XmlElement nod = xmlDoc.CreateElement(s);
                XmlText txt = xmlDoc.CreateTextNode(ini[s]);
                rootNode.AppendChild(nod);
                nod.AppendChild(txt);
            }
            return xmlDoc;
        }
    }
}
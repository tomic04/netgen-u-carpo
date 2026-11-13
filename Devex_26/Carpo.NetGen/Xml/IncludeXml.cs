using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Xml
{
    /// <summary>
    /// INCLUDE. Form1.vb 3383 stavi PROJECT atribut u global (velika slova).
    /// Form2.vb 844–934: cijepa po #, zamijeni ime datoteke, prvo NetGenFinder.GetFile,
    /// pa XmlDocument.Load. Djeca /PROJECT se uvezu. Datoteka koje nema ostavi
    /// formu kakva je napisana (On Error Resume Next, 706 i 913).
    /// </summary>
    public static class IncludeXml
    {
        /// <summary>Test ili host može dati GetFile. Inače se zove SpinDMSUtil.NetGenFinder.</summary>
        public static Func<string, XmlDocument> Finder;

        public static string Putanja(string glavni, string ime)
        {
            string g = glavni ?? "";
            int kosa = g.LastIndexOf('\\');
            int kosa2 = g.LastIndexOf('/');
            if (kosa2 > kosa)
                kosa = kosa2;
            string datoteka = (ime ?? "").Trim();
            if (kosa < 0)
                return datoteka;
            return g.Substring(0, kosa + 1) + datoteka;
        }

        /// <summary>
        /// Form2.vb 844–850. Include s forme samo ako je globalni prazan.
        /// Inače se uvozi već postavljeni globalni. Ucitaj projekta ovo ne zove:
        /// Form1 samo zapamti atribut, ne miješa tuđi XML u brojač.
        /// </summary>
        public static void SpojiFormu(XmlDocument doc, string putanja, string imeForme, string globalni)
        {
            if (doc == null || doc.DocumentElement == null)
                return;
            string izvor = (globalni ?? "").Trim();
            if (izvor.Length == 0)
            {
                XmlNode forma = null;
                foreach (XmlNode d in doc.DocumentElement.ChildNodes)
                {
                    if (string.Equals(d.LocalName, imeForme, StringComparison.OrdinalIgnoreCase))
                    {
                        forma = d;
                        break;
                    }
                }
                if (forma == null || forma.Attributes == null)
                    return;
                XmlAttribute a = forma.Attributes["INCLUDE"];
                if (a == null)
                    return;
                izvor = (a.Value ?? "").Trim();
            }
            if (izvor.Length == 0)
                return;
            string[] komadi = izvor.Split('#');
            var vidjeno = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int k = 0; k < komadi.Length; k++)
            {
                string ime = (komadi[k] ?? "").Trim();
                if (ime.Length == 0)
                    continue;
                string st = Putanja(putanja, ime);
                if (!vidjeno.Add(st))
                    continue;
                Uvezi(doc, st);
            }
        }

        private static void Uvezi(XmlDocument glavni, string stIncl)
        {
            XmlDocument inc = IzFindera(stIncl);
            if (inc == null || string.IsNullOrEmpty(inc.InnerXml))
            {
                inc = new XmlDocument();
                inc.XmlResolver = null;
                try
                {
                    inc.Load(stIncl);
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska(" m_xmldINCLUDE2.Load  , " + ex.Message);
                    return;
                }
            }
            XmlNodeList projekti = inc.SelectNodes("/PROJECT");
            if (projekti == null)
                return;
            foreach (XmlNode projekt in projekti)
            {
                var djeca = new List<XmlNode>();
                foreach (XmlNode d in projekt.ChildNodes)
                    djeca.Add(d);
                for (int i = 0; i < djeca.Count; i++)
                {
                    if (djeca[i].NodeType != XmlNodeType.Element)
                        continue;
                    glavni.DocumentElement.AppendChild(glavni.ImportNode(djeca[i], true));
                }
            }
        }

        private static XmlDocument IzFindera(string stIncl)
        {
            if (Finder != null)
            {
                try
                {
                    return Finder(stIncl);
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska("Greška kod FORM2 INCLUDE GetFile: " + ex.Message);
                    return null;
                }
            }
            try
            {
                Assembly a = Assembly.Load("SpinDMSUtil");
                object o = a.CreateInstance("SpinDMSUtil.NetGenFinder");
                if (o == null)
                    return null;
                object r = o.GetType().InvokeMember(
                    "GetFile",
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    o,
                    new object[] { stIncl });
                return r as XmlDocument;
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("Greška kod FORM2 INCLUDE Assembly.Load(SpinDMSUtil) : " + ex.Message);
                return null;
            }
        }
    }
}

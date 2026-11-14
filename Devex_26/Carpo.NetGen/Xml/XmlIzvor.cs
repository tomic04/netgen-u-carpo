using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Xml
{
    /// <summary>
    /// M5 (docs/analiza/08, XML-01). Form1.ProcitajXML 3160-3274 traži XML granule redom:
    /// SpinDMSUtil.NetGenFinder.GetFile(put), pa JupiterXML.dbo.netgenxml (body je DES s ključem "tan123ge",
    /// KriptirajDekriptiraj.vb), pa disk. Uključuje ga Jupiter ulaz (Loader, frm) kad nema TestXmlPath;
    /// TestShell i testovi čitaju samo disk. Isti put se u jednom otvaranju čita više puta (ulaz, Prozor,
    /// Podloga), pa se pročitani tekst pamti 30 s.
    /// </summary>
    public static class XmlIzvor
    {
        /// <summary>Ključ iz Form1.vb 3223.</summary>
        public const string Kljuc = "tan123ge";

        private const int SekundiPamcenja = 30;

        /// <summary>Uključuje Loader/frm (Ukljuci). Isključen znači samo disk.</summary>
        public static bool Ukljucen;

        /// <summary>Spoj na JupiterXML (isti server, Windows prijava, Form1 1544). Null preskače bazu.</summary>
        public static string SpojBaze;

        /// <summary>Test ili host daje GetFile. Null zove SpinDMSUtil.NetGenFinder (kao IncludeXml).</summary>
        public static Func<string, XmlDocument> Dms;

        /// <summary>Test daje body iz netgenxml po imenu XML-a. Null čita bazu.</summary>
        public static Func<string, byte[]> Baza;

        private sealed class Zapis
        {
            public DateTime Kad;
            public string Tekst;
        }

        private static readonly Dictionary<string, Zapis> Procitano = new Dictionary<string, Zapis>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Loader.Run8 i frm.Run8 bez TestXmlPath: DMS i JupiterXML na serveru granule.</summary>
        public static void Ukljuci(string server)
        {
            Ukljucen = true;
            if (string.IsNullOrWhiteSpace(server))
            {
                SpojBaze = null;
                return;
            }
            var b = new SqlConnectionStringBuilder();
            b.DataSource = server.Trim();
            b.InitialCatalog = "JupiterXML";
            b.IntegratedSecurity = true;
            b.ApplicationName = "Carpo.NetGen";
            b.ConnectTimeout = 5;
            SpojBaze = b.ConnectionString;
        }

        /// <summary>
        /// Dokument iz DMS-a ili baze. Null znači disk (i kad izvor nije uključen), kao VB kad DMS vrati prazan
        /// dokument, a baza nema reda ili se ne može otvoriti.
        /// </summary>
        public static XmlDocument Dohvati(string putanja)
        {
            if (!Ukljucen || string.IsNullOrWhiteSpace(putanja)
                || putanja.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) < 0)
                return null;
            string tekst = null;
            lock (Procitano)
            {
                Zapis z;
                if (Procitano.TryGetValue(putanja, out z) && (DateTime.UtcNow - z.Kad).TotalSeconds < SekundiPamcenja)
                    tekst = z.Tekst;
            }
            if (tekst == null)
            {
                tekst = "";
                XmlDocument doc = IzDms(putanja);
                string izvor = "SpinDMSUtil";
                if (Prazan(doc))
                {
                    doc = IzBaze(putanja);
                    izvor = "JupiterXML Database .. NetGenXML table";
                }
                if (Prazan(doc))
                    NgDnevnik.Akcija("Učitavam XML na stari način sa file sistema ... " + putanja);
                else
                {
                    tekst = doc.OuterXml;
                    NgDnevnik.Akcija(izvor + " : " + putanja);
                }
                lock (Procitano)
                    Procitano[putanja] = new Zapis { Kad = DateTime.UtcNow, Tekst = tekst };
            }
            if (tekst.Length == 0)
                return null;
            var gotov = new XmlDocument();
            gotov.XmlResolver = null;
            gotov.LoadXml(tekst);
            return gotov;
        }

        private static bool Prazan(XmlDocument doc)
        {
            return doc == null || string.IsNullOrEmpty(doc.InnerXml);
        }

        /// <summary>3170-3189: Assembly.Load("SpinDMSUtil"), NetGenFinder.GetFile; greška ide u log i dalje.</summary>
        private static XmlDocument IzDms(string putanja)
        {
            if (Dms != null)
            {
                try
                {
                    return Dms(putanja);
                }
                catch (Exception ex)
                {
                    NgDnevnik.Greska("Greška kod GetFile: " + putanja + " - " + ex.Message);
                    return null;
                }
            }
            try
            {
                Assembly a = Assembly.Load("SpinDMSUtil");
                object o = a.CreateInstance("SpinDMSUtil.NetGenFinder");
                if (o == null)
                    return null;
                object r = o.GetType().InvokeMember("GetFile", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null, o, new object[] { putanja });
                Svojstvo(o, "source");
                Svojstvo(o, "exception");
                return r as XmlDocument;
            }
            catch (Exception ex)
            {
                Exception u = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
                NgDnevnik.Greska("Greška kod Assembly.Load(SpinDMSUtil) ili GetFile: " + u.Message);
                return null;
            }
        }

        private static void Svojstvo(object o, string ime)
        {
            try
            {
                PropertyInfo p = o.GetType().GetProperty(ime, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                FieldInfo f = p == null ? o.GetType().GetField(ime, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) : null;
                object v = p != null ? p.GetValue(o, null) : (f != null ? f.GetValue(o) : null);
                if (v != null)
                    NgDnevnik.Akcija("SpinDMSUtil " + ime + " : " + v);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 3199-3240: ime datoteke iz puta, select body from netgenxml where xml = ime, Decrypt s "tan123ge".
        /// Nema reda, greška baze ili dekripcije znači disk.
        /// </summary>
        private static XmlDocument IzBaze(string putanja)
        {
            string ime = putanja.Replace('/', '\\');
            ime = ime.Substring(ime.LastIndexOf('\\') + 1).Trim();
            byte[] body;
            if (Baza != null)
                body = Baza(ime);
            else
                body = CitajBazu(ime);
            if (body == null || body.Length == 0)
                return null;
            try
            {
                var doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.LoadXml(Dekriptiraj(body, Kljuc));
                return doc;
            }
            catch (Exception ex)
            {
                if (!(ex is CryptographicException || ex is XmlException || ex is IOException || ex is ArgumentException))
                    throw;
                NgDnevnik.Greska("XML nije pronađen u bazi JUPITERXML DB .. NetGenXML table : " + ime + "\r\n\r\n" + ex.Message);
                return null;
            }
        }

        private static byte[] CitajBazu(string ime)
        {
            if (string.IsNullOrEmpty(SpojBaze))
                return null;
            try
            {
                using (var conn = new SqlConnection(SpojBaze))
                using (var cmd = new SqlCommand("select body from netgenxml where xml = @xml", conn))
                {
                    cmd.Parameters.AddWithValue("@xml", ime);
                    cmd.CommandTimeout = 30;
                    conn.Open();
                    return cmd.ExecuteScalar() as byte[];
                }
            }
            catch (SqlException ex)
            {
                NgDnevnik.Greska(" JupiterXML DB problem : " + ex.Message);
                return null;
            }
            catch (InvalidOperationException ex)
            {
                NgDnevnik.Greska(" JupiterXML DB problem : " + ex.Message);
                return null;
            }
        }

        /// <summary>KriptirajDekriptiraj.EncryptDecrypt.Decrypt: DES, Key = IV = ključ, StreamReader do kraja.</summary>
        public static string Dekriptiraj(byte[] sifra, string kljuc)
        {
            byte[] k = KljucUBajtove(kljuc);
            using (var des = new DESCryptoServiceProvider())
            {
                des.Key = k;
                des.IV = k;
                using (var ms = new MemoryStream(sifra))
                using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                    return sr.ReadToEnd();
            }
        }

        /// <summary>KeyToBytes: trim, dopuna s X do 8, rez na 8, UTF-8.</summary>
        private static byte[] KljucUBajtove(string k)
        {
            k = (k ?? "").Trim();
            if (k.Length < 8)
                k = k.PadRight(8, 'X').Substring(0, 8);
            if (k.Length > 8)
                k = k.Substring(0, 8);
            return Encoding.UTF8.GetBytes(k);
        }
    }
}

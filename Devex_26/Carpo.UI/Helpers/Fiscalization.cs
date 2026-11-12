using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Carpo.UI.Helpers
{
    internal class Fiscalization
    {
        public static string Calc(X509Certificate2 certifikat, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna)
        {
            var sb = new StringBuilder();
            sb.Append(oibObveznika);
            sb.Append(datumVrijemeIzdavanjaRacuna);
            sb.Append(brojcanaOznakaRacuna);
            sb.Append(oznakaPoslovnogProstora);
            sb.Append(oznakaNaplatnogUredaja);
            sb.Append(ukupniIznosRacuna.Replace(',', '.'));

            return CalculateZKI(certifikat, sb.ToString());
        }
        public static string CalculateZKI(X509Certificate2 certificate, string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            using var rsa = certificate.GetRSAPrivateKey();
            using var md5 = MD5.Create();
            var signed = rsa.SignData(data, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            var hash = md5.ComputeHash(signed);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        public static void SignXmlDoc(XmlDocument doc, X509Certificate2 cert)
        {
            SignedXml signedXml = new SignedXml(doc);
            signedXml.SigningKey = cert.GetRSAPrivateKey();
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;

            Reference reference = new Reference("#signXmlId");
            reference.DigestMethod = SignedXml.XmlDsigSHA256Url;
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform(false));
            reference.AddTransform(new XmlDsigExcC14NTransform(false));

            signedXml.AddReference(reference);


            KeyInfo keyInfo = new KeyInfo();
            var data = new KeyInfoX509Data(cert);

            data.AddIssuerSerial(cert.Issuer, cert.GetSerialNumberString());
            keyInfo.AddClause(data);
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();
            XmlElement signature = signedXml.GetXml();
            doc.DocumentElement.AppendChild(signature);
        }
    }
    internal class SoapWebContext
    {

        private readonly X509Certificate2 clientCert;
        private readonly string webRequestUrl;

        public SoapWebContext(X509Certificate2 clientCert, string url)
        {
            this.clientCert = clientCert;
            webRequestUrl = url;
        }

        public async Task<string> SendSoapRequest(string soapBodyXml, int timeout = 30000)
        {
            var soapEnvelope = $@"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                   <soapenv:Body>
                      {soapBodyXml}
                   </soapenv:Body>
                </soapenv:Envelope>";

            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
            };

            handler.ClientCertificates.Add(clientCert);

            handler.ServerCertificateCustomValidationCallback = (msg, srvCert, chain, errors) =>
            {
                if (errors == System.Net.Security.SslPolicyErrors.None)
                    return true;

                if (msg.RequestUri == null || msg.RequestUri.Host != "cistest.apis-it.hr")
                    return false;

                if ((errors & System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
                    return false;
                if ((errors & System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable) != 0)
                    return false;

                if (chain != null)
                {
                    foreach (var status in chain.ChainStatus)
                    {
                        if (status.Status == X509ChainStatusFlags.NoError)
                            continue;
                        if (status.Status == X509ChainStatusFlags.UntrustedRoot)
                            continue;
                        if (status.Status == X509ChainStatusFlags.PartialChain)
                            continue;
                        return false;
                    }
                }
                return true;
            };

            using var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
            using var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

            content.Headers.Clear();
            content.Headers.Add("Content-Type", "text/xml; charset=utf-8");

            using var request = new HttpRequestMessage(HttpMethod.Post, webRequestUrl) { Content = content };


            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"SOAP call failed: {result}");

            return result;

        }
    }
}

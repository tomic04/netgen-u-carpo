using Carpo.Core;
using Carpo.Data.Data.Context;
using Carpo.Data.Models;
using Carpo.Structure.Models;
using Carpo.Structure.Models.Core;
using Carpo.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Carpo.UI.Logic
{
    /// <summary>
    /// Extracted from Events.RunActions (wave 8): Outlook contact + fiscal ZKI/JIR.
    /// Behavior preserved — no rewrite of fiscal/SOAP contracts.
    /// Opt-in demo dry-run: parameter demo="true" (smoke path without cert/Outlook/SOAP).
    /// </summary>
    internal sealed class FiscalOutlookActions
    {
        internal enum HandleResult
        {
            NotHandled,
            /// <summary>Action consumed; caller should continue the foreach.</summary>
            Continue,
            /// <summary>Action consumed; caller should return from RunActions.</summary>
            Stop
        }

        private readonly Forms.Base.JupiterShared form;
        private readonly string connectionString;
        private readonly Control thisControl;

        /// <summary>Shared between ZKI then JIR in the same RunActions pass.</summary>
        private string zki = "";

        public FiscalOutlookActions(Forms.Base.JupiterShared form, string connectionString, Control thisControl)
        {
            this.form = form;
            this.connectionString = connectionString;
            this.thisControl = thisControl;
        }

        public HandleResult TryHandle(JEventAction action)
        {
            if (action?.Type == null)
                return HandleResult.NotHandled;

            if (action.Type == "outlook")
            {
                RunOutlook(action);
                return HandleResult.Stop;
            }

            if (action.Type == "ZKI")
            {
                RunZki(action);
                return HandleResult.Continue;
            }

            if (action.Type == "JIR")
            {
                RunJir(action);
                return HandleResult.Continue;
            }

            return HandleResult.NotHandled;
        }

        private static bool IsDemo(JEventAction action)
        {
            var p = action.Parameter?.FirstOrDefault(x =>
                string.Equals(x.Name, "demo", StringComparison.OrdinalIgnoreCase));
            if (p == null || string.IsNullOrEmpty(p.Value))
                return false;
            return string.Equals(p.Value, "true", StringComparison.OrdinalIgnoreCase)
                || p.Value == "1";
        }

        private void RunOutlook(JEventAction action)
        {
            if (IsDemo(action))
            {
                form.Logger.Info("FiscalOutlookActions", "outlook demo dry-run (no Outlook COM)");
                Common.Helpers.JMessages.Info(
                    "Demo OK: action type=outlook stigao do FiscalOutlookActions.\n" +
                    "Dry-run — Outlook COM nije zvan.\n" +
                    "Za pravi test: ukloni demo=\"true\" i postavi procedure (+ Outlook instaliran).");
                return;
            }

            var p = new Parser(form, thisControl);

            var procedure = action.Parameter.First(x => x.Name == "procedure").Value;
            var cmdType = CommandType.StoredProcedure;
            var typePar = action.Parameter.FirstOrDefault(x =>
                string.Equals(x.Name, "commandtype", StringComparison.OrdinalIgnoreCase));
            if (typePar != null && string.Equals(typePar.Value, "text", StringComparison.OrdinalIgnoreCase))
                cmdType = CommandType.Text;

            var parms = action.Parameter
                .Where(x =>
                    !string.Equals(x.Name, "procedure", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(x.Name, "commandtype", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(x.Name, "demo", StringComparison.OrdinalIgnoreCase))
                .Select(x => new KeyValuePair<string, object>(x.Name, p.GetLogicPartValue(x.Value)))
                .ToArray();

            var context = new SqlContext(connectionString);
            var contact = context.ExecuteReader<OutlookModel>(procedure, parms, cmdType);
            if (contact == null)
            {
                Common.Helpers.JMessages.Info("Outlook: ExecuteReader nije vratio kontakt (null).");
                return;
            }

            MicrosoftAPI.AddContactToOutlook(contact);
            Common.Helpers.JMessages.Info("Outlook: kontakt dodan/ažuriran (" + (contact.PrivateEmail ?? contact.Name) + ").");
        }

        private void RunZki(JEventAction action)
        {
            if (IsDemo(action))
            {
                zki = "DEMO-ZKI-" + DateTime.Now.ToString("HHmmss");
                form.Logger.Info("FiscalOutlookActions", "ZKI demo dry-run; zki=" + zki);
                Common.Helpers.JMessages.Info(
                    "Demo OK: action type=ZKI → FiscalOutlookActions.\n" +
                    "Dry-run zki=" + zki + "\n" +
                    "(bez mpfiskalizacija / certifikata).\n" +
                    "Sljedeći korak u istom clicku može biti JIR demo.");
                return;
            }

            var load = action.Parameter.First(x => x.Name == "loadZKI");
            //var save = action.Parameter.First(x => x.Name == "saveZKI");

            var context = new SqlContext(connectionString);
            var certificateInfo = context.ExecuteReader<CertificateModel>("select * from mpfiskalizacija where partneriid=(select varset from pubvars where varime='NasID')", type: CommandType.Text);

            if (load == null)
                return;

            var core = new Logic.Data.Core()
            {
                ConnectionString = form.ConnectionString,
                ParameterControlValue = "",
                CurrentForm = form
            };

            var data = core.GetDataByName(load.Value);

            DataSet dataSet = core.GetDataSet(data, "ZKILoad", out bool stop);

            var row = dataSet.Tables[0].Rows[0];
            string s = row.Field<string>("datumivrijeme");

            DateTime? datum = null;
            if (!string.IsNullOrWhiteSpace(s) &&
                DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                datum = dt;
            }

            string zaZki = datum?.ToString("dd.MM.yyyy HH:mm:ss");

            using var cert = new X509Certificate2($"{IniFile.Ini["RemoteDllPath"]}\\Certificates\\{certificateInfo.CertificateName}", certificateInfo.CertificatePassword);

            zki = Fiscalization.Calc(cert, row.Field<string>("OIB"), zaZki, row.Field<string>("brojcanaOznakaRacuna"), row.Field<string>("oznakaPoslovnogProstora"), row.Field<string>("oznakaNaplatnogUredaja"), row.Field<string>("ukupniIznosRacuna"));

            //data = core.GetDataByName(save.Value);

            //var param = data.DataParameterList.First(x => x.VariableName == "ZKI");
            //param.Value = zki;

            core.ExecuteData(data, "ZKISave", out stop);
        }

        private void RunJir(JEventAction action)
        {
            if (IsDemo(action))
            {
                form.Logger.Info("FiscalOutlookActions", "JIR demo dry-run; prior zki=" + zki);
                Common.Helpers.JMessages.Info(
                    "Demo OK: action type=JIR → FiscalOutlookActions.\n" +
                    "Prior ZKI (iz ovog RunActions): " + (string.IsNullOrEmpty(zki) ? "(prazno — pokreni ZKI prije JIR)" : zki) + "\n" +
                    "Dry-run — nema SOAP / cert / save SP.");
                return;
            }

            var load = action.Parameter.First(x => x.Name == "loadJIR");
            var save = action.Parameter.First(x => x.Name == "saveZKIJIRUUD");

            var context = new SqlContext(connectionString);
            var certificateInfo = context.ExecuteReader<CertificateModel>("select * from mpfiskalizacija where partneriid=(select varset from pubvars where varime='NasID')", type: CommandType.Text);

            if (load == null || save == null)
                return;

            var core = new Logic.Data.Core()
            {
                ConnectionString = form.ConnectionString,
                ParameterControlValue = "",
                CurrentForm = form
            };

            var data = core.GetDataByName(load.Value);

            DataSet dataSet = core.GetDataSet(data, "JIRLoad", out bool stop);

            var row = dataSet.Tables[0].Rows[0];

            var debug1 = $"{IniFile.Ini["RemoteDllPath"]}\\Certificates\\{certificateInfo.CertificateName}";
            var cert = new X509Certificate2($"{IniFile.Ini["RemoteDllPath"]}\\Certificates\\{certificateInfo.CertificateName}", certificateInfo.CertificatePassword);

            var doc = new XmlDocument();

            var uid = Guid.NewGuid();

            var xml = (row.ItemArray[0] as string).Replace("UUUUUUUUUU", uid.ToString());
            xml = xml.Replace("ZZZZZZZZZZ", zki);

            //xml = Regex.Replace(xml, @"\d{2}\.\d{2}\.\d{4}T\d{2}:\d{2}:\d{2}", datestr);
            xml = Regex.Replace(xml, @"^\s*<\?xml[^?]*\?>\s*", "");

            doc.LoadXml(xml);

            Fiscalization.SignXmlDoc(doc, cert);

            var web = new SoapWebContext(cert, certificateInfo.FiscalURL);
            var response = Task.Run(() => web.SendSoapRequest(doc.OuterXml)).GetAwaiter().GetResult();

            var responseXml = new XmlDocument();
            responseXml.LoadXml(response);

            var jir = responseXml.GetElementsByTagName("tns:Jir")[0]?.InnerText;

            data = core.GetDataByName(save.Value);

            var param = data.DataParameterList.First(x => x.VariableName == "JIR");
            param.Value = jir;

            var param1 = data.DataParameterList.First(x => x.VariableName == "ZKI");
            param1.Value = zki;

            var param2 = data.DataParameterList.First(x => x.VariableName == "UUID");
            param2.Value = uid.ToString();

            core.ExecuteData(data, "UpdateAll", out stop);
        }
    }
}

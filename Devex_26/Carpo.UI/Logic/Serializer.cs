using Carpo.Core;
using Carpo.Structure.Models.Core;
using SpinDMSUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Carpo.UI.Logic
{
    public class Serializer : IDisposable
    {
        private const string subFolder = "sub";

        private enum type
        { forms, parts }

        private Logger logger;
        private string xmlPath;
        private CarpoFinder finder = null;
        private bool dmsCompatibilityErrorShown;

        public Serializer(Logger logger, string xmlPath)
        {
            this.logger = logger;
            this.xmlPath = xmlPath;
            this.finder = new CarpoFinder(xmlPath, Carpo.Data.Connection.DataBase, "Carpo");
        }

        private void serializerRecursion(XDocument doc)
        {
            try
            {
                if (doc.Descendants("partsql").Count() > 0)
                    GetPartsSql(doc);

                if (doc.Descendants("part").Count() > 0)
                    GetParts(doc);

                if (doc.Descendants("form").Where(formEl => formEl.Attribute("path") != null).Count() > 0)
                    GetForms(doc);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JRoot DeserializeXml(string moduleId)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                //if (!File.Exists(xmlPath))
                //{
                //    Common.Helpers.JMessages.Warning("putanja: '" + xmlPath + "'", "Neispravna putanja");
                //    return null;
                //}

                //var doc = XDocument.Load(xmlPath);
                logger.InfoTest("Carpo finder", "prije poziva", startTime);
                XDocument doc = null;
                try
                {
                    doc = finder.GetCarpoRootXml();
                }
                catch (MissingMethodException ex)
                {
                    HandleDmsCompatibilityError(ex, "GetCarpoRootXml");
                    return null;
                }
                startTime = DateTime.Now;
                logger.InfoTest("Carpo finder", "poslije poziva", startTime);

                if (doc == null)
                {
                    string message = "Na odabranoj putanji: '" + Path.GetDirectoryName(xmlPath) + "' ne pronalazi se datoteka " + Path.GetFileName(xmlPath) + ".";
                    
                    // Create exception with details so "Prikaži više" button is visible
                    Exception detailsException = null;
                    if (!string.IsNullOrEmpty(finder.exception))
                        detailsException = new Exception(finder.exception);
                    
                    Common.Helpers.JMessages.Error(detailsException, message, "Neuspješni dohvat granule");
                    return null;
                }

                GetPartsSql(doc);
                GetForms(doc);
                GetParts(doc);
                GetCustomControls(doc);
                GetDatalistInfo(doc, moduleId);

                try
                {
                    var serializer = new XmlSerializer(typeof(JRoot));
                    using (var reader = doc.Root.CreateReader())
                    {
                        var jRoot = (JRoot)serializer.Deserialize(reader);
                        logger.InfoData("DeserializeXml", moduleId + "#", startTime);
                        return jRoot;
                    }
                }
                catch (Exception ex)
                {
                    Common.Helpers.JMessages.Error(ex, "putanja: " + xmlPath + Environment.NewLine + ex.Message, "Neispravan xml!");
                    return null;
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, "putanja: " + xmlPath + Environment.NewLine + ex.Message, "Neuspješna deserializacija xml-a");
            }
            logger.InfoData("DeserializeXml", moduleId + "##", startTime);
            return null;
        }

        public void GetParts(XDocument doc)
        {
            DateTime startTime = DateTime.Now;
            var listElements = new List<Structure.Models.Xml.ElementReplacing>();

            foreach (XElement partEl in doc.Descendants("part"))
            {
                string requiredStr = "";
                try
                {
                    var elPath = partEl.Attribute("path");
                    var required = partEl.Attribute("required");

                    if (required != null)
                        requiredStr = required.Value.ToString();

                    XElement part = null;

                    if (elPath != null)
                    {
                        XDocument docPart = null; // GetFile(elPath, type.parts, requiredStr);

                        //if we already have this part use it
                        var el = listElements.Find(e => e.FormPart.Attribute("path").Value == elPath.Value.ToString() && e.FormPart.Attribute("name").Value == partEl.Attribute("name").Value);
                        if (el != null)
                            docPart = new XDocument(new XElement("parts", el.Part));
                        if (docPart == null)
                            docPart = GetCarpoFileSafe(elPath, CarpoFinder.type.parts, requiredStr, "GetParts");

                        if (docPart != null)
                            part = docPart.Elements("parts").Descendants("part").Where(x => x.Attribute("name").Value == partEl.Attribute("name").Value && x.Attribute("path") == null).FirstOrDefault();
                    }
                    if (part == null)
                    {
                        //provjeri u trenutnoj datoteci
                        part = doc.Elements("root").Elements("parts").Descendants("part").Where(x => x.Attribute("name").Value == partEl.Attribute("name").Value).FirstOrDefault();
                    }

                    listElements.Add(new Structure.Models.Xml.ElementReplacing() { FormPart = partEl, Part = part });
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                        throw;

                    if (requiredStr != "false")
                        logger.Error("GetParts", ex.Message, ex);
                }
            }

            foreach (Structure.Models.Xml.ElementReplacing element in listElements)
            {
                // ako nije našao part, svakako ga dodajem u listElements, jer ću ga ovdje onda null checkirati i zamijeniti s praznim stringom
                // kako bih izbjegao infinite petlju nastalu rekurzijom (GetParts bi nastavila pozivati samu sebe jer nije zamijenila <part> s ničime
                if (element.Part == null)
                    element.FormPart.ReplaceWith("");
                else
                    element.FormPart.ReplaceWith(element.Part.Elements());
            }
            //logger.InfoData("GetParts", "", startTime);
            serializerRecursion(doc);
        }

        private void GetPartsSql(XDocument doc)
        {
            DateTime startTime = DateTime.Now;
            if (doc.Descendants("partsql").ToList().Count() > 0)
            {
                var listElements = new List<Structure.Models.Xml.ElementReplacing>();
                JRoot jRoot = new JRoot();

                try
                {
                    var serializer = new XmlSerializer(typeof(JRoot));
                    using (var reader = doc.Root.CreateReader())
                    {
                        jRoot = (JRoot)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    Common.Helpers.JMessages.Error(ex, "putanja: " + xmlPath + Environment.NewLine + ex.Message, "Neispravan xml!");
                    jRoot = null;
                }

                foreach (XElement formElement in doc.Descendants("root"))
                {
                    List<XDocument> xDocs = new List<XDocument>();
                    List<string> dataNames = new List<string>();
                    foreach (XElement formPart in formElement.Descendants("partsql"))
                    {
                        string requiredStr = "";
                        try
                        {
                            var required = formPart.Attribute("required");
                            var dataName = formPart.Attribute("dataname");
                            var name = formPart.Attribute("name");

                            if (required != null)
                                requiredStr = required.Value.ToString();

                            XElement part = null;

                            foreach (var xDoc in xDocs)
                            {
                                part = xDoc.Elements("parts").Descendants("part").Where(x => x.Attribute("name").Value == name.Value).FirstOrDefault();
                                if (part != null)
                                    break;
                            }
                            if (!dataNames.Contains(dataName.Value) && part == null)
                            {
                                bool stopActions = false;
                                Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(null, dataName.Value.ToString());
                                dataDefinition.DefinitionRoot = jRoot;
                                dataDefinition.Log = logger;
                                using (DataSet ds = dataDefinition.GetDataSet("Serializer > PartsSql", out stopActions))
                                {
                                    dataNames.Add(dataName.Value);
                                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                    {
                                        var value = ds.Tables[0].Rows[0][0];
                                        if (value == DBNull.Value)
                                        {
                                            logger.Warn("GetPartsSql", "vrijednost partsql varijable je null");
                                        }
                                        else
                                        {
                                            try
                                            {
                                                var xDoc = XDocument.Parse(value.ToString());
                                                xDocs.Add(xDoc);
                                                part = xDoc.Elements("parts").Descendants("part").Where(x => x.Attribute("name").Value == name.Value).FirstOrDefault();
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error("GetPartsSql", ex.Message, ex);
                                            }
                                        }
                                    }
                                }
                            }

                            listElements.Add(new Structure.Models.Xml.ElementReplacing() { FormPart = formPart, Part = part });
                        }
                        catch (Exception ex)
                        {
                            if (requiredStr != "false")
                                logger.Error("GetPartsSql", ex.Message, ex);
                        }
                    }
                }

                foreach (Structure.Models.Xml.ElementReplacing element in listElements)
                {
                    // ako nije našao partsql, svakako ga dodajem u listElements, jer ću ga ovdje onda null checkirati i zamijeniti s praznim stringom
                    // kako bih izbjegao infinite petlju nastalu rekurzijom (GetPartSql bi nastavila pozivati samu sebe jer nije zamijenila <part> s ničime
                    if (element.Part == null)
                        element.FormPart.ReplaceWith("");
                    else
                        element.FormPart.ReplaceWith(element.Part.Elements());
                }

                jRoot = null;
            }
            //logger.InfoData("GetPartsSql", "", startTime);
        }

        private void GetForms(XDocument doc)
        {
            DateTime startTime = DateTime.Now;
            string subDbFolder = subFolder + Carpo.Data.Connection.DataBase;

            var listElements = new List<Structure.Models.Xml.ElementReplacing>();

            //nađi sve form elemente koji sadrže path atribut
            var xFrm = doc.Descendants("form")
                .Where(el => el.Attribute("path") != null);

            if (xFrm != null)
            {
                foreach (XElement formElement in doc.Descendants("root"))
                {
                    foreach (XElement el in xFrm)
                    {
                        var elPath = el.Attribute("path");
                        XElement part = null;

                        if (elPath != null)
                        {
                            XDocument docPart = GetCarpoFileSafe(elPath, CarpoFinder.type.forms, "", "GetForms");
                            //XDocument docPart = GetFile(elPath, type.forms, "");

                            //string formName = elPath.Value.ToString();
                            string formName = el.Attribute("name").Value;

                            var formNameArr = formName.Split(':');

                            if (docPart != null)
                                part = docPart.Elements("root").Descendants("form").Where(x => x.Attribute("name").Value == formNameArr[0]).FirstOrDefault();

                            if (formNameArr.Length > 1 && part != null)
                            {
                                part.SetAttributeValue("name", formNameArr[1]);
                            }
                        }

                        listElements.Add(new Structure.Models.Xml.ElementReplacing() { FormPart = el, Part = part });
                    }
                }
            }

            foreach (Structure.Models.Xml.ElementReplacing element in listElements)
            {
                // ako nije našao form, svakako ga dodajem u listElements, jer ću ga ovdje onda null checkirati i zamijeniti s praznim stringom
                // kako bih izbjegao infinite petlju nastalu rekurzijom (GetForms bi nastavila pozivati samu sebe jer nije zamijenila <part> s ničime
                if (element.Part == null)
                    element.FormPart.ReplaceWith("");
                else
                    element.FormPart.ReplaceWith(element.Part);
            }
            logger.InfoData("GetForms", "", startTime);
            serializerRecursion(doc);
        }

        private XDocument GetCarpoFileSafe(XAttribute attribute, CarpoFinder.type typeFolder, string partRequired, string context)
        {
            try
            {
                return finder.GetCarpoFile(attribute, typeFolder, partRequired);
            }
            catch (MissingMethodException ex)
            {
                HandleDmsCompatibilityError(ex, $"{context}, attr={attribute?.Value}, type={typeFolder}");
                throw new OperationCanceledException("SpinDMS compatibility error.", ex);
            }
        }

        private void HandleDmsCompatibilityError(MissingMethodException ex, string context)
        {
            if (dmsCompatibilityErrorShown)
                return;

            dmsCompatibilityErrorShown = true;
            string details = BuildDmsCompatibilityDetails(context, ex);

            try { logger?.Error("DMS compatibility", details, ex); } catch { }
            Common.Helpers.JMessages.Error(ex, details, "Neusklađene DMS biblioteke");
        }

        private string BuildDmsCompatibilityDetails(string context, Exception ex)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Otkriven je mismatch između SpinDMSUtil i SpinDMSData biblioteka.");
            sb.AppendLine($"Context: {context}");
            sb.AppendLine($"Exception: {ex.GetType().Name}: {ex.Message}");
            sb.AppendLine();
            sb.AppendLine("Loaded assemblies:");
            AppendLoadedAssemblyInfo(sb, "SpinDMSUtil");
            AppendLoadedAssemblyInfo(sb, "SpinDMSData");
            sb.AppendLine();
            sb.AppendLine("Probe paths:");
            AppendProbePathInfo(sb, "SpinDMSUtil.dll");
            AppendProbePathInfo(sb, "SpinDMSData.dll");
            sb.AppendLine();
            sb.AppendLine("Akcija: deployaj SpinDMSUtil.dll i SpinDMSData.dll iz istog builda/checkina,");
            sb.AppendLine("obriši stare kopije i restartaj Jupiter/Carpo proces.");

            return sb.ToString();
        }

        private static void AppendLoadedAssemblyInfo(StringBuilder sb, string assemblyName)
        {
            var asm = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));

            if (asm == null)
            {
                sb.AppendLine($"  {assemblyName}: not loaded");
                return;
            }

            string location;
            try { location = asm.Location; } catch { location = "unknown"; }

            sb.AppendLine($"  {assemblyName}: {asm.GetName().Version} | {location}");
        }

        private static void AppendProbePathInfo(StringBuilder sb, string fileName)
        {
            foreach (var path in GetProbePaths(fileName).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!File.Exists(path))
                    continue;

                try
                {
                    var an = AssemblyName.GetAssemblyName(path);
                    sb.AppendLine($"  {path} => {an.Name} {an.Version}");
                }
                catch (Exception probeEx)
                {
                    sb.AppendLine($"  {path} => cannot read ({probeEx.GetType().Name})");
                }
            }
        }

        private static IEnumerable<string> GetProbePaths(string fileName)
        {
            var list = new List<string>();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(baseDir))
            {
                list.Add(Path.Combine(baseDir, fileName));
                list.Add(Path.Combine(baseDir, "SharedAssemblies", "Spin", fileName));
            }

            list.Add(Path.Combine(@"C:\Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies\Spin", fileName));
            list.Add(Path.Combine(@"C:\Program Files (x86)\Spin Informatica\Jupiter Client\Programs\SharedAssemblies\Spin", fileName));
            list.Add(Path.Combine(@"C:\Program Files (x86)\Spin Informatica\Jupiter Client\Programs", fileName));

            return list;
        }

        #region DatalistInfo

        private void GetDatalistInfo(XDocument doc, string moduleId)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                var dataModellist = Carpo.Common.Helpers.SpecificElementList.GetDatalistInfoModels(doc, moduleId);

                if (Carpo.Common.Helpers.SpecificElementList.DatalistModelList == null)
                    Carpo.Common.Helpers.SpecificElementList.DatalistModelList = new List<List<Common.Models.DatalistInfo>>();

                Carpo.Common.Helpers.SpecificElementList.DatalistModelList.Add(dataModellist);

                DataTable dt = CreateDataTable();

                for (int i = 0; i < dataModellist.Count; i++)
                {
                    dt.Rows.Add(CreateDataRow(dt, dataModellist, i));
                }

                logger.DatalistInfo = dt;
            }
            catch (Exception)
            {
                logger.Warn("method: GetDatalistInfo", "Greška punjenja datalist info");
            }
            // logger.InfoData("GetDatalistInfo", "", startTime);
        }

        private DataRow CreateDataRow(DataTable dt, List<Carpo.Common.Models.DatalistInfo> dataModellist, int i)
        {
            DataRow dr = dt.NewRow();
            dr[0] = dataModellist[i].StoredProcedure;
            dr[1] = dataModellist[i].Type;
            dr[2] = dataModellist[i].Form;
            dr[3] = dataModellist[i].Name;
            dr[4] = dataModellist[i].Scope;
            dr[5] = dataModellist[i].Parameters;
            return dr;
        }

        private DataTable CreateDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("StoredProcedure", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Form", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Scope", typeof(string));
            dt.Columns.Add("Parameters", typeof(string));
            return dt;
        }

        #endregion DatalistInfo

        private void GetCustomControls(XDocument doc)
        {
            try
            {
                if (Carpo.UI.Forms.Base.JupiterShared.CustomControlsElement == null)
                    return;

                // morao sam dodati čišnjenje u slučaju da nam su nam Carpo dll-ovi i dalje aktivni, a to će biti slučaj u Jupiteru
                Carpo.UI.Forms.Base.JupiterShared.CustomControlsElement.Clear();

                var elementsList = Carpo.Common.Helpers.SpecificElementList.GetSpecificElementList(doc, "customcontrol");

                foreach (var element in elementsList)
                {
                    //ako postoji ne dodavaj na listu ponovo
                    if (Carpo.UI.Forms.Base.JupiterShared.CustomControlsElement.Exists(
                            x => x.Name == element.Attribute("name").Value.ToString()
                            ))
                        continue;

                    Carpo.UI.Forms.Base.JupiterShared.CustomControlsElement.Add(new JCustomDef
                    {
                        Name = element.Attribute("name").Value.ToString(),
                        Dll = element.Attribute("dll").Value.ToString(),
                        Class = element.Attribute("class").Value.ToString(),
                        InitOnMaster = element.Attribute("initonmaster")?.Value.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error("GetCustomControls:", ex.Message);
            }
        }

        private XDocument GetFile(XAttribute attribute, type typeFolder, string partRequired)
        {
            //DateTime startTime = DateTime.Now;
            try
            {
                string subDbFolder = subFolder + Carpo.Data.Connection.DataBase;

                string formName = attribute.Value.ToString();

                bool isMasterForm = false;
                if (formName.StartsWith("\\") || formName.StartsWith("/"))
                {
                    isMasterForm = true;
                    formName = formName.Substring(1);
                }

                string fileNamePath = "";
                if (typeFolder == type.forms)
                {
                    int dots = formName.Count(x => x == '.');
                    if (dots > 1 || isMasterForm == true)
                        fileNamePath = formName; //+ ".xml";
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(xmlPath);
                        fileNamePath = fileName + "." + formName; //+ ".xml";
                    }
                }
                else
                {
                    fileNamePath = attribute.Value.ToString();
                }

                string xmlFolder = Path.GetDirectoryName(xmlPath);
                string xmlFileSubBazaType = Path.Combine(xmlFolder, subDbFolder, typeFolder.ToString(), fileNamePath);
                string xmlFileSubBaza = Path.Combine(xmlFolder, subDbFolder, fileNamePath);
                string xmlFileSubType = Path.Combine(xmlFolder, subFolder, typeFolder.ToString(), fileNamePath);
                string xmlFileSub = Path.Combine(xmlFolder, subFolder, fileNamePath);
                string xmlFileType = Path.Combine(xmlFolder, typeFolder.ToString(), fileNamePath);
                string xmlFile = Path.Combine(xmlFolder, fileNamePath);

                XDocument docPart = null;

                try
                {
                    if (isMasterForm == false)
                    {
                        //prema prioritetu
                        if (File.Exists(xmlFileSubBazaType)) //partsFirma folder
                            docPart = XDocument.Load(xmlFileSubBazaType);
                        else if (File.Exists(xmlFileSubBaza)) //partsFirma folder
                            docPart = XDocument.Load(xmlFileSubBaza);
                        else if (File.Exists(xmlFileSubType)) //parts folder
                            docPart = XDocument.Load(xmlFileSubType);
                        else if (File.Exists(xmlFileSub)) //parts folder
                            docPart = XDocument.Load(xmlFileSub);
                        else if (File.Exists(xmlFileType)) //parts folder
                            docPart = XDocument.Load(xmlFileType);
                        else if (File.Exists(xmlFile)) //parts folder
                            docPart = XDocument.Load(xmlFile);
                    }
                    else
                    {
                        if (File.Exists(xmlFile)) //parts folder
                            docPart = XDocument.Load(xmlFile);
                    }
                }
                catch (StackOverflowException ex)
                {
                    Carpo.Common.Helpers.JMessages.Error(ex, ex.Message);
                }
                catch (Exception ex)
                {
                    Carpo.Common.Helpers.JMessages.Error(ex, ex.Message);
                }

                if (docPart == null)
                {
                    if (typeFolder == type.forms)
                        logger.Warn("event: GetFile", "Forma '" + fileNamePath + "' nije pronađena");
                    else if (typeFolder == type.parts && partRequired != "false")
                        logger.Warn("event: GetFile", "Part '" + fileNamePath + "' nije pronađen");
                }
                //logger.InfoData("GetFile", "", startTime);
                return docPart;
            }
            catch (Exception ex)
            {
                logger.Error("event: Serializer(GetFile)", ex.Message, ex);
            }
            return null;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}
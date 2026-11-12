using Carpo.UI.Logic;
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Carpo.XmlGen.XmlParse
{
    public class Parse : IDisposable
    {
        private XDocument xml;
        private Dictionary<string, DataTable> listDataTable;
        private SplashScreenManager splashScreenManager;

        public Parse(XDocument Xml, Dictionary<string, DataTable> ListDataTable, SplashScreenManager SplashScreenManager)
        {
            xml = Xml;
            listDataTable = ListDataTable;
            splashScreenManager = SplashScreenManager;
        }

        public void ParseForeach()
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Parse.ParseForeach()";

            try
            {
                var replaceList = FillReplaceList();
                ReplaceElements(replaceList);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Ova metoda parsira <gen/> elemente koji sadrže atribut foreach da dobijemo isti element izgeneriran za svaki red u datasetu ali sa vrijednostima [0],[1][2] itd..
        /// Ovdje se puni replaceList objekt, sprema ga u novi element imena <foreach/> kojeg ćemo kasnije znati dodatno isparsirati tj. pobrisati iz njega sve što ne trebamo
        /// </summary>
        public List<XElement> FillReplaceList()
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Parse.FillReplaceList()";
            string hlpLeftPart = "", hlpRightPart = "";

            try
            {
                List<XElement> replaceList = new List<XElement>();
                int index = 1;

                foreach (XElement xEl in xml.Descendants().Where(x => x.Name.LocalName.ToLower() == "gen"))
                {
                    if (xEl.Attribute("foreach") != null)
                    {
                        List<XElement> tempList = new List<XElement>();
                        string datatableName = xEl.Attribute("foreach").Value.Replace("~", "");
                        DataTable dt = listDataTable.Where(x => x.Key == datatableName).FirstOrDefault().Value;

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            XElement clonedElement = new XElement(xEl); // sačuvao sam originalni element

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                foreach (var subEl in clonedElement.Descendants())
                                {
                                    if (subEl.HasElements == false && !string.IsNullOrEmpty(subEl.Value) && subEl.Value.Contains(datatableName))
                                    {
                                        if (i == 0)
                                            subEl.Value = subEl.Value.Replace(datatableName, datatableName + "[0]");
                                        else
                                        {
                                            hlpLeftPart = subEl.Value.Substring(0, subEl.Value.IndexOf("["));
                                            hlpRightPart = subEl.Value.Substring((subEl.Value.IndexOf("]") + 1), subEl.Value.Length - 1 - subEl.Value.IndexOf("]"));
                                            subEl.Value = hlpLeftPart + "[" + i.ToString() + "]" + hlpRightPart;
                                        }
                                    }

                                    foreach (var subAtt in subEl.Attributes())
                                    {
                                        if (subAtt.Value.Contains(datatableName))
                                        {
                                            if (i == 0)
                                                subAtt.Value = subAtt.Value.Replace(datatableName, datatableName + "[0]");
                                            else
                                            {
                                                hlpLeftPart = subAtt.Value.Substring(0, subAtt.Value.IndexOf("["));
                                                hlpRightPart = subAtt.Value.Substring((subAtt.Value.IndexOf("]") + 1), subAtt.Value.Length - 1 - subAtt.Value.IndexOf("]"));
                                                subAtt.Value = hlpLeftPart + "[" + i.ToString() + "]" + hlpRightPart;
                                            }
                                        }
                                    }
                                }

                                tempList.Add(new XElement(clonedElement));
                            }

                            string newElement = "foreach" + index.ToString();
                            XElement newXEl = new XElement(newElement);

                            foreach (var el in tempList)
                            {
                                newXEl.Add(el);
                            }

                            replaceList.Add(newXEl);

                            XAttribute att = new XAttribute("replace", newElement);
                            xEl.Add(att);
                        }
                        else
                        {
                            var delAttribute = xEl.Attribute("foreach");
                            delAttribute.Value = "deleteelement";
                        }

                        index++;
                    }
                }

                return replaceList;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Ova metoda parsira zamijenjuje sve <gen/> elemente koje sadrže atribut foreach sa novim izgeneriranim kroz foreach datatablice
        /// </summary>
        private void ReplaceElements(List<XElement> replaceList)
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Parse.ReplaceElements()";

            try
            {
                // sve elemente koji imaju definiran atribut replace mijenjam sa izgeneriranim kroz foreach
                foreach (var el in replaceList)
                {
                    var elName = (el as XElement).Name.ToString();

                    var elementToBeReplaced = xml.Descendants().Where(x => x.Name.LocalName.ToLower() == "gen").Where(x => x.Attribute("replace") != null && x.Attribute("replace").Value == elName).First();
                    elementToBeReplaced.ReplaceWith(el);
                }

                // brišem sve <gen/> elemente koji imaju definiran foreachatribut pritom im ostavljajući sve child node netaknute
                var genNodes = xml.Descendants().Where(x => x.Name.LocalName.ToLower() == "gen").Where(z => z.Attribute("foreach") != null).ToList();
                foreach (var genNode in genNodes)
                {
                    if (genNode.Attribute("foreach").Value == "deleteelement") // za gen element kojem je foreach vratio 0 redova - brišemo mu childove, a potom i njega samog
                    {
                        genNode.RemoveAll();
                        genNode.Remove();
                    }
                    else
                    {
                        genNode.AddAfterSelf(genNode.Nodes());
                        genNode.Remove();
                    }
                }

                // brišem <foreach/> elemente pritom im ostavljajući sve child node netaknute
                var foreachNodes = xml.Descendants().Where(z1 => z1.Name.ToString().StartsWith("foreach")).ToList();
                foreach (var foreachNode in foreachNodes)
                {
                    foreachNode.AddAfterSelf(foreachNode.Nodes());
                    foreachNode.Remove();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Parsiranje svih vrijednosti + pregled uvjetnih operatora
        /// </summary>
        public XDocument ParseXml()
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Parse.ParseXml()";

            try
            {
                bool checkIfResult = false; // false poništava na novi IF

                foreach (XElement el in xml.Descendants())
                {
                    foreach (XAttribute at in el.Attributes())
                    {
                        if (at.Name == "if" || at.Name == "elseif" || at.Name == "else")
                        {
                            if (at.Name == "if")
                                checkIfResult = false;

                            at.Value = CheckCondition(at, listDataTable, ref checkIfResult);
                        }

                        if (at.Value == "createelement" || at.Value == "deleteelement")
                        {
                            el.Name = at.Value;
                            continue;
                        }

                        at.Value = ParseValue(at.Value, listDataTable).ToString();
                    }

                    if (el.Value != null && el.Value.ToString() == "~dtGlava1Pdv2Action2.@PayableAmount")
                    {
                    }

                    if (el.HasElements == false && !string.IsNullOrEmpty(el.Value))
                        el.Value = ParseValue(el.Value, listDataTable).ToString();
                }

                xml = ParseGenElements(xml); // parsiram gen elemente
                xml = ParseSpecifics(xml);
                xml = ParseRepx(xml);
                xml.DescendantNodes().OfType<XComment>().Remove();

                return xml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string CheckCondition(XAttribute at, Dictionary<string, DataTable> listDataTable, ref bool checkIfResult)
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Parse.CheckCondition()";

            try
            {
                if (Carpo.UI.Logic.Conditions.CheckConditionXmlGen(ref checkIfResult, at.Value, listDataTable) == false)
                    return "deleteelement";

                return "createelement";
            }
            catch
            {
                throw;
            }
        }

        private object ParseValue(string value, Dictionary<string, DataTable> listDataTable)
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Parse.ParseValue(" + value + ")";

            try
            {
                using (Parser parser = new Parser(listDataTable))
                {
                    parser.IsXmlGen = true;

                    var valueObj = parser.GetLogicPartValue(value);

                    if (valueObj != null)
                        return valueObj.ToString();
                    else
                        return value;
                }
            }
            catch
            {
                throw;
            }
        }

        private XDocument ParseGenElements(XDocument xDoc)
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Parse.ParseGenElements()";

            try
            {
                XElement xmlElement = xDoc.Element("xml");
                List<XElement> deleteElementList = xmlElement.Descendants("deleteelement").ToList();

                // brišem elemente kojima sam postavio vrijednost na "deleteelements"
                foreach (var el in deleteElementList)
                {
                    el.Remove();
                }

                // elementima kojima sam vrijednost stavio na "createelement" brišem samo root element dok sadržaj ostavljam
                while (xmlElement.Descendants("createelement").Count() > 0)
                {
                    var x = xmlElement.Descendants("createelement").First();
                    x.AddAfterSelf(x.Nodes());
                    x.Remove();
                }

                // vraćam bez prvog <xml/> elementa
                return new XDocument(new XDocument(xmlElement).Root.Elements().First());
            }
            catch
            {
                throw;
            }
        }

        private XDocument ParseSpecifics(XDocument xDoc)
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Parse.ParseSpecifics()";

            foreach (XElement el in xml.Descendants())
            {
                if (el.HasElements == false && !string.IsNullOrEmpty(el.Value))
                {
                    if (el.Value.ToLower().StartsWith("##attfromfs::", StringComparison.InvariantCultureIgnoreCase))
                    {
                        SetWaitFormDescription("Dohvat i parsiranje attachmenta sa file systema");

                        var regex = new Regex("##attFromFs::", RegexOptions.IgnoreCase);
                        string fsLocation = regex.Replace(el.Value, "");

                        try
                        {
                            string locationStr = ParseValue(fsLocation, listDataTable).ToString();
                            el.Value = GetBase64FromFile(locationStr);
                        }
                        catch
                        {
                            Carpo.XmlGen.Start.ErrorMethodStr += "::attFromFs";
                            throw;
                        }
                    }
                    else if (el.Value.ToLower().StartsWith("##attfromdb::", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // ovdje samo moramo makniti #attFromDb jer nam objekt dolazi već isparsiran kroz Carpo Parser koji smo doradili
                        var regex = new Regex("##attFromDb::", RegexOptions.IgnoreCase);
                        el.Value = regex.Replace(el.Value, "");
                    }
                }
            }

            return xml;
        }

        private XDocument ParseRepx(XDocument xDoc)
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Parse.ParseRepx()";

            string template = "", parameters = "";
            string tempPathFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".pdf";

            var repxInit = new Carpo.XmlGen.XmlParse.Repx.Initialize();

            foreach (XElement el in xml.Descendants())
            {
                if (el.HasElements == false && !string.IsNullOrEmpty(el.Value))
                {
                    if (el.Value.ToLower().StartsWith("##repx::"))
                    {
                        SetWaitFormDescription("Dohvat i parsiranje RepX-a");
                        string repxValue = el.Value.Replace("##repx::", "");
                        try
                        {
                            string[] repStr = repxValue.Split(';');
                            if (repStr.Length != 2)
                                return null;

                            template = ParseValue(repStr[0], listDataTable).ToString();
                            parameters = ParseValue(repStr[1], listDataTable).ToString();

                            if (!string.IsNullOrEmpty(template))
                            {
                                bool repxSuccess = repxInit.Init(template, parameters, tempPathFileName);
                                if (repxSuccess)
                                {
                                    el.Value = GetBase64FromFile(tempPathFileName);
                                }
                            }
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }

            return xml;
        }

        private void SetWaitFormDescription(string text)
        {
            if (splashScreenManager != null)
                splashScreenManager.SetWaitFormDescription(text);
        }

        public string GetBase64FromFile(string fileName)
        {
            byte[] pdfBytes = File.ReadAllBytes(fileName);
            return Convert.ToBase64String(pdfBytes);
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;
            else
            {
                disposedValue = true;

                if (this != null)
                    this.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
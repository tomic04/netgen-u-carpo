using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Carpo.Common.Helpers
{
    public class SpecificElementList
    {
        public static List<List<Models.DatalistInfo>> DatalistModelList { get; set; }

        // traži specifični element
        public static List<XElement> GetSpecificElementList(XDocument deserializedXml, string elementName)
        {
            var list = new List<XElement>();

            // tražim sve forme
            foreach (XElement formElement in deserializedXml.Descendants("root"))
            {
                foreach (XElement el in formElement.Descendants())
                {
                    if (el.Name == elementName)
                        list.Add(el);
                }
            }

            return list;
        }

        #region DatalistInfo

        public static List<Models.DatalistInfo> GetDatalistInfoModels(XDocument deserializedXml, string moduleId)
        {
            var modelList = new List<Models.DatalistInfo>();

            try
            {
                foreach (XElement rootDescendant in deserializedXml.Descendants("root"))   // nađi sve rootove
                {
                    foreach (XElement rootChildElement in rootDescendant.Descendants()) // root child elementi
                    {
                        if (rootChildElement.Name == "form")    // nađi samo form elemente
                        {
                            var form = "";
                            if (rootChildElement.Attribute("name") == null)    // ako ne postoji form name atribut
                                form = Translator.TranslateString("Atribut form name nije definiran.");
                            else
                                form = rootChildElement.Attribute("name").Value.ToString();

                            foreach (XElement element in rootChildElement.Descendants())        // form child elementi
                            {
                                if (element.Name == "data")
                                    GetDatasets(element, form, modelList, moduleId);
                            }
                        }
                        if (rootChildElement.Name == "datalist" && rootChildElement.Parent.Name == "root")  //ako je globalna, ima root za parenta
                        {
                            foreach (XElement element in rootChildElement.Descendants())
                            {
                                if (element.Name == "data")
                                {
                                    var formName = "";
                                    if (rootDescendant.Descendants().Where(el => el.Name == "form").FirstOrDefault().Attribute("name") == null)
                                        formName = Translator.TranslateString("Atribut form name nije definiran.");
                                    else
                                        formName = rootDescendant.Descendants().Where(el => el.Name == "form").FirstOrDefault().Attribute("name").Value.ToString();
                                    GetDatasets(element, formName, modelList, moduleId);
                                }
                            }
                        }
                    }
                }
                return modelList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void GetDatasets(XElement element, string form, List<Models.DatalistInfo> modelList, string moduleId)
        {
            var dataElement = element;
            var datalistModel = new Models.DatalistInfo()
            {
                StoredProcedure = dataElement.Attribute("command").Value.ToString(),
                Name = dataElement.Attribute("name") != null ? dataElement.Attribute("name").Value.ToString() : "",
                XMLvalue = dataElement.ToString(),
                ModuleId = moduleId
            };

            GetDataType(dataElement, datalistModel);
            GetDataScope(dataElement, datalistModel);
            datalistModel.Form = form;

            var parameterInfoList = new List<string>();

            foreach (XElement elementOfDataElement in dataElement.Descendants())
            {
                parameterInfoList.Add(elementOfDataElement.Attribute("name").Value.ToString()); // get parameter elements
            }
            datalistModel.Parameters = string.Join(", ", parameterInfoList.ToArray());

            modelList.Add(datalistModel);
        }

        private static void GetDataType(XElement dataElement, Models.DatalistInfo datalistModel)
        {
            if (dataElement.Attribute("type") != null && dataElement.Attribute("type").ToString().ToLower().Contains("text"))
                datalistModel.Type = "text";
            else if (dataElement.Attribute("type") != null && dataElement.Attribute("type").ToString().ToLower().Contains("integrated"))
                datalistModel.Type = "integrated";
            else
                datalistModel.Type = "stored procedure";
        }

        private static void GetDataScope(XElement dataElement, Models.DatalistInfo datalistModel)
        {
            if (dataElement.Ancestors().Where(x => x.Name == "form").FirstOrDefault() != null)  // ako je unutar form elementa
                datalistModel.Scope = "local";
            else
                datalistModel.Scope = "global";
        }
    }

    #endregion DatalistInfo
}
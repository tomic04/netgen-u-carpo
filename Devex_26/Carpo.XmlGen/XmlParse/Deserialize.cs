using Carpo.Structure.Models.Core.XmlGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Carpo.XmlGen
{
    public class Deserialize : IDisposable
    {
        public JXmlGenRoot DeserializeXml(XDocument xDocument)
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Deserialize.DeserializeXml()";
            try
            {
                // tražim partove
                using (var clsSerializer = new Carpo.UI.Logic.Serializer(new Core.Logger(Core.Log.LogLevel.None), @"D:\XmlGen\xmlGenTest.xml"))
                {
                    clsSerializer.GetParts(xDocument);
                }

                // <xml> element
                var elXml = xDocument.Descendants("xml").FirstOrDefault();
                var xml = new XDocument(elXml);

                // <datalist> element

                var elDatalist = xDocument.Descendants("datalist").FirstOrDefault();
                var datalist = GetData(elDatalist);

                // <validate> element
                // TO DO

                return new JXmlGenRoot()
                {
                    Xml = xml,
                    DataList = datalist
                };
            }
            catch
            {
                throw;
            }
        }

        public JDataList GetData(XElement elDatalist)
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "XmlParse.Deserialize.GetData()";
            try
            {
                var jDataList = new Carpo.Structure.Models.Core.XmlGen.JDataList()
                {
                    Data = new List<JData>()
                };

                foreach (var elData in elDatalist.Descendants("data"))
                {
                    var dataModel = new Carpo.Structure.Models.Core.XmlGen.JData()
                    {
                        Command = elData.Attribute("command")?.Value,
                        DataParameterList = new List<JDataParameter>(),
                        Type = elData.Attribute("type")?.Value,
                        Name = elData.Attribute("name")?.Value,
                        Timeout = elData.Attribute("timeout")?.Value,
                        ParentDataset = elData.Attribute("parentdataset")?.Value
                    };

                    foreach (var elParameter in elData.Descendants("parameter"))
                    {
                        var paramModel = new Carpo.Structure.Models.Core.XmlGen.JDataParameter()
                        {
                            Type = elParameter.Attribute("type")?.Value,
                            Name = elParameter.Attribute("name")?.Value,
                            Value = elParameter.Attribute("value")?.Value,
                            ValueField = elParameter.Attribute("valuefield")?.Value
                        };

                        dataModel.DataParameterList.Add(paramModel);
                    }

                    jDataList.Data.Add(dataModel);
                }

                return jDataList;
            }
            catch
            {
                throw;
            }
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
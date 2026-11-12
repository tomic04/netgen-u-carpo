using Carpo.Structure.Models.Core;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Carpo.UI.Connector
{
    public class Xml
    {
        private string assemblyFolder = "";
        private string assemblyName = "";

        public Xml(string assemblyFolder, string assemblyName)
        {
            this.assemblyFolder = assemblyFolder;
            this.assemblyName = assemblyName;
        }

        public JRoot ReadXml()
        {
            try
            {
                // Generate properties to represent primitive values.
                string xml = assemblyFolder + "\\" + assemblyName + ".xml";
                if (File.Exists(xml))
                {
                    using (FileStream stream = File.Open(xml, FileMode.Open))
                    {
                        XmlReader reader = new XmlTextReader(stream);
                        XmlSerializer serializer = new XmlSerializer(typeof(Carpo.Structure.Models.Core.JRoot));
                        var plugins = (Carpo.Structure.Models.Core.JRoot)serializer.Deserialize(reader);
                        return plugins;
                    }
                }
                else
                    return null;
            }
            catch
            {
                throw;
            }
        }

        public void ExportXml(JRoot jRoot)
        {
            ExportXml((object)jRoot);
        }

        public void ExportXml(JForm jForm)
        {
            ExportXml((object)jForm);
        }

        private void ExportXml(object jObject)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string xmlName = assemblyFolder + "\\" + assemblyName + ".xml";

            saveFileDialog.FileName = xmlName;
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var writer = new StreamWriter(saveFileDialog.FileName))
                {
                    if (jObject is JForm)
                    {
                        var serializer = new XmlSerializer(typeof(JForm));
                        serializer.Serialize(writer, jObject);
                    }
                    else if (jObject is JRoot)
                    {
                        var serializer = new XmlSerializer(typeof(JRoot));
                        serializer.Serialize(writer, jObject);
                    }
                }
            }
        }
    }
}
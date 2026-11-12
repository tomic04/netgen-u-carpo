using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Carpo.Core.Logic
{
    public class XmlPath
    {
        public enum FileType { Forms, Parts }

        private const string subFolder = "sub";

        public static string GetPath(string xmlPath, FileType fileType, string attrPath)
        {
            string subDbFolder = subFolder + Carpo.Data.Connection.DataBase;

            string formName = attrPath;
            string fileNamePath = "";

            if (fileType == FileType.Forms)
            {
                int dots = formName.Count(x => x == '.');
                if (dots > 1)
                    fileNamePath = formName; //+ ".xml";
                else
                {
                    string fileName = Path.GetFileNameWithoutExtension(xmlPath);
                    fileNamePath = fileName + "." + formName; //+ ".xml";
                }
            }
            else
            {
                fileNamePath = formName.ToString();
            }

            string xmlFolder = Path.GetDirectoryName(xmlPath);

            List<string> paths = new List<string>();

            string xmlFileSubBazaType = Path.Combine(xmlFolder, subDbFolder, fileType.ToString(), fileNamePath);
            string xmlFileSubBaza = Path.Combine(xmlFolder, subDbFolder, fileNamePath);
            string xmlFileSubType = Path.Combine(xmlFolder, subFolder, fileType.ToString(), fileNamePath);
            string xmlFileSub = Path.Combine(xmlFolder, subFolder, fileNamePath);
            string xmlFileType = Path.Combine(xmlFolder, fileType.ToString(), fileNamePath);
            string xmlFile = Path.Combine(xmlFolder, fileNamePath);

            paths.AddRange(new string[] { xmlFileSubBazaType, xmlFileSubBaza, xmlFileSubType, xmlFileSub, xmlFileType, xmlFile });

            foreach (string path in paths)
            {
                if (File.Exists(path))
                    return path;
            }

            return null;
        }
    }
}
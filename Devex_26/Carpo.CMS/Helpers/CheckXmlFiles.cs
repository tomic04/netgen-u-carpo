using System.Collections.Generic;
using System.Xml;

namespace Carpo.CMS.Helpers
{
    internal class CheckXmlFiles
    {
        private List<Models.CheckXml> checkXmlList = new List<Models.CheckXml>();

        public CheckXmlFiles()
        {
        }

        private void Check()
        {
            //checkXmlList.Clear();
            //foreach (Models.ProjectTreeNode projectTreeNode in treeList)
            //{
            //    if (projectTreeNode.Node != null)
            //    {
            //        CheckNode(projectTreeNode.Node, projectTreeNode.ItemFile.xDocFile, null);
            //        TraverseNodesCheck(projectTreeNode.Node.ChildNodes, projectTreeNode.Node, projectTreeNode.ItemFile.xDocFile);
            //    }

            //}

            using (Forms.CheckXml form = new Forms.CheckXml())
            {
                form.gcCheck.DataSource = checkXmlList;
                form.SetCnt = checkXmlList.Count;
                form.ShowDialog();
            }
        }

        private void TraverseNodesCheck(XmlNodeList nodes, XmlNode parentNode, string itemFile)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    CheckNode(node, itemFile, parentNode);
                    TraverseNodesCheck(node.ChildNodes, node, itemFile);
                }
            }
        }

        private void CheckNode(XmlNode node, string itemFile, XmlNode parentNode)
        {
            string elementName = node.Name;

            List<Models.ElementCheck> element = Logic.DsElement.GetElementByName(elementName);
            if (element.Count == 0)
            {
                if (!checkXmlList.Exists(x => x.Element == elementName))
                {
                    checkXmlList.Add(new Models.CheckXml() { Element = elementName, Message = "nedefinirano", RowNumber = 0 });
                    //Data.Analiza.InsertElement(elementName);
                }
            }
            else
            {
                List<Models.Attribute> attribute = Logic.DsAttribute.GetAttrByElement(elementName);
                foreach (XmlAttribute a in node.Attributes)
                {
                    if (node.Name == "event" && a.Name == "type")
                    {
                        Data.Analiza.InsertEvent(parentNode.Name, a.Value);
                    }

                    if (node.Name == "action" && a.Name == "type")
                    {
                        Data.Analiza.InsertActionType(a.Value);
                    }

                    if (!attribute.Exists(x => x.AttrName == a.Name))
                        if (!checkXmlList.Exists(x => x.Attribute == a.Name && x.Element == elementName))
                        {
                            //Data.Analiza.InsertAttribute(elementName, a.Name);
                            checkXmlList.Add(new Models.CheckXml() { Element = elementName, Attribute = a.Name, Message = "nedefinirano", RowNumber = 0 });
                        }
                }
            }
        }
    }
}
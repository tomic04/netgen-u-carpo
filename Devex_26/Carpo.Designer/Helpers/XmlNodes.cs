using System;
using System.Collections.Generic;
using System.Xml;

namespace Carpo.Designer.Helpers
{
    internal class XmlNodes
    {
        public static string SetNode(XmlNode node, string parentNodeId, List<Models.Element> model)
        {
            string id = Guid.NewGuid().ToString();

            string info = "";
            if (node.Attributes["name"] != null)
                info = node.Attributes["name"].Value;
            else if (node.Attributes["type"] != null)
                info = node.Attributes["type"].Value;

            model.Add(new Models.Element() { Id = id, Name = node.Name, ParentId = parentNodeId, Node = node, Info = info });

            return id;
        }
    }
}
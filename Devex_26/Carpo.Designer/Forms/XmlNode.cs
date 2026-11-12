using Carpo.Designer.Logic;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Carpo.Designer.Forms
{
    public partial class XmlNode : Form
    {
        private Models.FileDef fileDef;
        private System.Xml.XmlNode xNode;

        private List<Models.Element> treeModel = new List<Models.Element>();

        public XmlNode()
        {
            InitializeComponent();
        }

        public XmlNode(Models.FileDef fileDef, System.Xml.XmlNode xNode)
        {
            InitializeComponent();

            this.fileDef = fileDef;
            this.xNode = xNode;
        }

        private void XmlNode_Load(object sender, EventArgs e)
        {
            PopulateElements();
        }

        #region Populate

        private void PopulateAttributes()
        {
            DataTable table = new DataTable();

            vGrdAttr.Rows.Clear();
            vGrdAttr.DataSource = null;

            System.Xml.XmlNode node = GetNodeElement();

            //filter Attr..
            List<Models.Attribute> attributes = DsAttribute.GetAttrByElement(node.Name);

            foreach (var item in attributes)
            {
                string attributeName = item.AttrName;

                EditorRow er = new EditorRow();
                er.Properties.FieldName = attributeName;
                er.Properties.Caption = attributeName;
                table.Columns.Add(attributeName, typeof(string));
                vGrdAttr.Rows.Add(er);
            }

            DataRow row = table.NewRow();
            foreach (var attr in node.Attributes)
            {
                XmlAttribute a = (attr as XmlAttribute);
                if (table.Columns.Contains(a.Name))
                {
                    row[a.Name] = a.Value;
                }
            }

            table.Rows.Add(row);
            vGrdAttr.DataSource = table;
        }

        private void PopulateElements()
        {
            try
            {
                string nodeId = SetNode(xNode, null);
                TraverseNodes(xNode.ChildNodes, xNode, nodeId);

                SetTree();
                treeXml.DataSource = treeModel;
                treeXml.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion Populate

        private void SetTree()
        {
            treeXml.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn() { Visible = true, FieldName = "Name" });
            treeXml.ParentFieldName = "ParentId";
            treeXml.KeyFieldName = "Id";
        }

        private void TraverseNodes(XmlNodeList nodes, System.Xml.XmlNode parentNode, string parentNodeId)
        {
            foreach (System.Xml.XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    string nodeId = SetNode(node, parentNodeId);
                    TraverseNodes(node.ChildNodes, node, nodeId);
                }
            }
        }

        private string SetNode(System.Xml.XmlNode node, string parentNodeId)
        {
            string id = Guid.NewGuid().ToString();

            string info = "";
            if (node.Attributes["Name"] != null)
                info = node.Attributes["Name"].Value;
            else if (node.Attributes["Type"] != null)
                info = node.Attributes["Type"].Value;

            treeModel.Add(new Models.Element() { Id = id, Name = node.Name, ParentId = parentNodeId, Node = node, Info = info });

            return id;
        }

        private void treeXml_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (treeXml.FocusedNode != null)
                PopulateAttributes();
        }

        private void vGrdAttr_CellValueChanged(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            vGrdAttr.GetCellValue(vGrdAttr.GetRowByHandle(vGrdAttr.FocusedRow.Index), 0);

            System.Xml.XmlNode node = GetNodeElement();
            string attr = e.Row.Properties.FieldName;
            string value = e.Value.ToString();

            if (node.Attributes[attr] != null)
            {
                node.Attributes[attr].Value = value.ToString();
            }
            else
            {
                XmlAttribute newAttr = fileDef.xDoc.CreateAttribute(attr);
                newAttr.Value = value;
                node.Attributes.SetNamedItem(newAttr);
            }

            StreamWriter outStream = System.IO.File.CreateText(fileDef.xDocFile);
            fileDef.xDoc.Save(outStream);
            outStream.Close();

            //xDoc.Save(xDocFile);
        }

        private System.Xml.XmlNode GetNodeElement()
        {
            TreeListNode focusedNode = treeXml.FocusedNode;
            System.Xml.XmlNode node = (treeXml.GetDataRecordByNode(focusedNode) as Models.Element).Node;
            return node;
        }

        private void vGrdAttr_FocusedRowChanged(object sender, DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e)
        {
        }
    }
}
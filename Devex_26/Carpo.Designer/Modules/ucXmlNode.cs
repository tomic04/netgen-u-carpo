using Carpo.Designer.Logic;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Carpo.Designer.Modules
{
    public partial class ucXmlNode : UserControl
    {
        public Models.FileDef fileDef;
        private XmlNode xNode;
        public string docId;

        private int xmlVersionCount = 0;
        private int indexOfEditedElement;

        private List<Models.Element> treeModel = new List<Models.Element>();
        private List<Models.Element> partOfTreeModel = new List<Models.Element>();
        private IEnumerable<Models.ElementCheck> parentElementModel;
        private List<Models.ElementCheck> childElementList = new List<Models.ElementCheck>();
        private List<Models.Element> editedElement = new List<Models.Element>();
        private List<string> listOfToBeDeleted = new List<string>();

        public ucXmlNode()
        {
            InitializeComponent();
        }

        public ucXmlNode(Models.FileDef fileDef, XmlNode xNode, string docId)
        {
            InitializeComponent();

            this.fileDef = fileDef;
            this.xNode = xNode;
            this.docId = docId;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateElements();
            parentElementModel = DsElement.GetChildElements();
        }

        #region Populate

        private void PopulateAttributes()
        {
            DataTable table = new DataTable();

            vGrdAttr.Rows.Clear();
            vGrdAttr.DataSource = null;

            XmlNode node = GetNodeElement();

            //filter Attr..
            IEnumerable<Models.Attribute> attributes = DsAttribute.GetAttrByElement(node.Name);

            foreach (var item in attributes)
            {
                string attributeName = item.AttrName;

                EditorRow er = new EditorRow();
                er.Properties.FieldName = attributeName;
                er.Properties.Caption = attributeName;
                er.Properties.Value = item.AttrValue;
                table.Columns.Add(attributeName, GetAttrType(item.AttrType));
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

            //DataRow row2 = table.NewRow();
            //foreach (EditorRow r in vGrdAttr.Rows)
            //{
            //    string grdAttr = r.Properties.FieldName;
            //    if (!String.IsNullOrEmpty(r.Properties.Value.ToString()) && table.Columns.Contains(grdAttr))
            //    {
            //        row2[grdAttr] = r.Properties.Value;
            //    }
            //}

            //table.Rows.Add(row2);

            vGrdAttr.DataSource = table;
            vGrdAttr.BestFit();
        }

        private Type GetAttrType(string type)
        {
            switch (type)
            {
                case "bool":
                    type = "System.Boolean";
                    break;

                case "int":
                    type = "System.Int32";
                    break;

                default:
                    type = "System.String";
                    break;
            }

            return System.Type.GetType(type);
        }

        private void PopulateElements()
        {
            try
            {
                string nodeId = Helpers.XmlNodes.SetNode(xNode, null, treeModel);
                TraverseNodes(xNode.ChildNodes, xNode, nodeId, treeModel);

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
            treeXml.OptionsView.ShowAutoFilterRow = true;

            treeXml.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn() { Visible = true, FieldName = "Name" });
            treeXml.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn() { Visible = true, FieldName = "Info" });

            treeXml.Columns[0].OptionsColumn.AllowSort = false;
            treeXml.Columns[1].OptionsColumn.AllowSort = false;

            treeXml.Columns[1].AppearanceHeader.Font = new Font(treeXml.Columns[1].AppearanceHeader.Font, FontStyle.Italic);
            treeXml.Columns[1].AppearanceCell.ForeColor = Color.FromArgb(100, 100, 100);

            treeXml.ParentFieldName = "ParentId";
            treeXml.KeyFieldName = "Id";
            treeXml.Columns[0].BestFit();
        }

        private void TraverseNodes(XmlNodeList nodes, XmlNode parentNode, string parentNodeId, List<Models.Element> model)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    string nodeId = Helpers.XmlNodes.SetNode(node, parentNodeId, model);
                    TraverseNodes(node.ChildNodes, node, nodeId, model);
                }
            }
        }

        private void vGrdAttr_CellValueChanged(object sender, DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            //if (e.RecordIndex == 1)
            //    return;
            string value = vGrdAttr.GetCellValue(vGrdAttr.GetRowByHandle(vGrdAttr.FocusedRow.Index), 0).ToString();
            //string defaultvalue = vGrdAttr.GetCellValue(vGrdAttr.GetRowByHandle(vGrdAttr.FocusedRow.Index), 1).ToString();

            XmlNode node = GetNodeElement();
            string attr = e.Row.Properties.FieldName;
            //string value = e.Value.ToString();

            if (node.Attributes[attr] != null)
            {
                if (!String.IsNullOrEmpty(value))
                    node.Attributes[attr].Value = value.ToString();
                else
                    node.Attributes.Remove(node.Attributes[attr]);
            }
            else
            {
                XmlAttribute newAttr = fileDef.xDoc.CreateAttribute(attr);
                newAttr.Value = value;
                node.Attributes.SetNamedItem(newAttr);
            }

            // čistim model
            treeModel.Clear();
            // čistim datasource drveta
            ClearTreeDatasource();

            // punim model ponovo
            string nodeId = Helpers.XmlNodes.SetNode(xNode, null, treeModel);
            TraverseNodes(xNode.ChildNodes, xNode, nodeId, treeModel);

            // osvježavam datasource drveta sa osvježenim modelom (i snimam)
            RefreshTreeData();
            SaveTree(false, null, null);
        }

        private XmlNode GetNodeElement()
        {
            TreeListNode focusedNode = treeXml.FocusedNode;
            XmlNode node = (treeXml.GetDataRecordByNode(focusedNode) as Models.Element).Node;
            return node;
        }

        private string GetNodeId()
        {
            TreeListNode focusedNode = treeXml.FocusedNode;
            return (treeXml.GetDataRecordByNode(focusedNode) as Models.Element).Id;
        }

        private string GetElementName()
        {
            XmlNode node = GetNodeElement();
            return node.Name;
        }

        private void vGrdAttr_RecordCellStyle(object sender, DevExpress.XtraVerticalGrid.Events.GetCustomRowCellStyleEventArgs e)
        {
            if (e.Row.Properties.RowType.FullName != "System.Boolean")
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);

            e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
        }

        private void vGrdAttr_FocusedRowChanged(object sender, DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e)
        {
            try
            {
                string attribute = e.Row.Properties.FieldName;
                string element = GetElementName();

                object description = DsAttribute.GetAttrDescription(element, attribute);

                if (description != null)
                    txtAttributeMemo.Text = description.ToString();
            }
            catch (Exception)
            {
            }
        }

        private void treeXml_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (treeXml.FocusedNode != null && treeXml.FocusedNode != treeXml.Nodes.AutoFilterNode)
                PopulateAttributes();
        }

        #region POPUP

        private void treeXml_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            TreeList tree = sender as TreeList;
            TreeListHitInfo hitInfo = tree.CalcHitInfo(e.Point);
            if (hitInfo.Node != null)
            {
                tree.FocusedNode = hitInfo.Node;
                SetTreePopup(hitInfo.Node);
                popupTree.ShowPopup(barManager1, Control.MousePosition);
            }
        }

        private void SetTreePopup(TreeListNode node)
        {
            PopupEnableAllItems(popupTree);

            XmlNode focusedNode = GetNodeElement();
            childElementList = DsElement.GetChildElements(parentElementModel, focusedNode.Name);

            SetButtonsEnabledState(childElementList, focusedNode);
        }

        private void SetButtonsEnabledState(List<Models.ElementCheck> list, XmlNode node)
        {
            if (list.Count == 0)
                mnuTreeEdit.Enabled = false;

            if (treeXml.FocusedNode.Level == 0 && treeXml.FocusedNode.HasChildren == true)
                mnuTreeDelete.Enabled = false;
        }

        private void PopupEnableAllItems(PopupMenu popup)
        {
            foreach (var item in popup.ItemLinks)
            {
                if (item is BarButtonItemLink)
                {
                    (item as BarButtonItemLink).Item.Enabled = true;
                }
            }
        }

        private void mnuTreeAddInside_ItemClick(object sender, ItemClickEventArgs e)
        {
            partOfTreeModel.Clear();
            XmlNode node = GetNodeElement();

            List<string> newIdList = new List<string>();
            string parentNodeId = (treeXml.GetDataRecordByNode(treeXml.FocusedNode.ParentNode) as Models.Element).Id;
            string oldNodeId = GetNodeId();
            indexOfEditedElement = treeModel.IndexOf(treeModel.First(x => x.Id == oldNodeId));

            string newNodeId = Helpers.XmlNodes.SetNode(node, parentNodeId, partOfTreeModel);
            TraverseNodes(node.ChildNodes, node, newNodeId, partOfTreeModel);

            using (var newElement = new Forms.XmlElementNew(childElementList, partOfTreeModel, fileDef.xDoc))
            {
                var result = newElement.ShowDialog();

                if (result == DialogResult.OK)
                {
                    Helpers.ModelDataTransfer.CopyElementModel(editedElement, newElement.returnList);
                    DeleteNodeFromModel(treeModel, oldNodeId);
                    AddEditedModel(treeModel, editedElement, indexOfEditedElement);
                    xmlVersionCount++;
                }
            }
            XmlNode nju = GetNodeElement();

            ClearTreeDatasource();
            RefreshTreeData();
            SaveTree(true, node, nju);
        }

        private void mnuTreeDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            XmlNode node = GetNodeElement();

            if (node != null)
            {
                XmlNode parent = node.ParentNode;

                DeleteNodeFromModel(treeModel, GetNodeId()); // obrisali iz modela
                parent.RemoveChild(node); // obrisali nodu
                xmlVersionCount++;
                ClearTreeDatasource();
                RefreshTreeData();
                SaveTree(false, null, null);
            }
        }

        private void vGrdAttr_CustomRecordCellEdit(object sender, DevExpress.XtraVerticalGrid.Events.GetCustomRowCellEditEventArgs e)
        {
            //e.RepositoryItem = repositoryItemRadioGroup;

            string rowtype = e.Row.Properties.RowType.FullName;
            int row = e.RecordIndex;

            if (row < 1)
                switch (rowtype)
                {
                    case "System.Boolean":
                        e.RepositoryItem = new RepositoryItemCheckEdit();
                        break;

                    case "System.String":
                        if (e.Row.Properties.FieldName == "padding" || e.Row.Properties.FieldName == "spacing")
                            e.RepositoryItem = repositoryItemButtonEdit;
                        else
                            e.RepositoryItem = repositoryItemTextEdit;
                        break;

                    case "System.Int32":
                        e.RepositoryItem = repositoryItemTextEditNumbers;
                        break;
                        //...
                }
        }

        private void repositoryItemButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DevExpress.XtraEditors.ButtonEdit edit = sender as DevExpress.XtraEditors.ButtonEdit;

            string promptValue = Prompt.ShowDialog("Upišite vrijednosti", "Caption", edit.EditValue.ToString());

            if (promptValue != null)
                edit.EditValue = promptValue;
        }

        private void DeleteNodeFromModel(List<Models.Element> list, string nodeId)
        {
            list.Remove(treeModel.First(x => x.Id == nodeId));
            FillDeleteList(treeModel, nodeId);

            foreach (string str in listOfToBeDeleted)
            {
                treeModel.RemoveAll(x => x.Id == str);
            }
        }

        private void FillDeleteList(List<Models.Element> list, string parentId)
        {
            foreach (Models.Element el in list)
            {
                if (el.ParentId == parentId)
                {
                    listOfToBeDeleted.Add(el.Id);
                    FillDeleteList(list, el.Id);
                }
            }
        }

        private void AddEditedModel(List<Models.Element> parentModel, List<Models.Element> childModel, int index)
        {
            int j = 0;
            for (int i = index; i < (index + childModel.Count); i++)
            {
                parentModel.Insert(i, childModel[j]);
                j++;
            }
        }

        private void SaveTree(bool replace, XmlNode node, XmlNode nju)
        {
            // ovdje treba sortirati xDoc prema drvetu

            StreamWriter outStream = System.IO.File.CreateText(fileDef.xDocFile);
            fileDef.xDoc.Save(outStream);
            outStream.Close();
        }

        private void ClearTreeDatasource()
        {
            treeXml.DataSource = null;
            treeXml.ViewInfo.RowsInfo.Clear();
        }

        private void RefreshTreeData()
        {
            treeXml.DataSource = treeModel;
            treeXml.ExpandAll();
        }

        #endregion POPUP
    }
}
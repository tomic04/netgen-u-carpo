using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Carpo.Designer
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        private List<Models.FileDef> xDocList = new List<Models.FileDef>();
        private List<Models.CheckXml> checkXmlList = new List<Models.CheckXml>();

        private List<Models.ProjectTreeNode> treeList = new List<Models.ProjectTreeNode>();

        public Main()
        {
            InitializeComponent();

            //Icon = Properties.Resources.spinDev;
        }

        private void frmMain_Load(object sender, System.EventArgs e)
        {
        }

        private void iOpen_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void solutionExplorer_TreeViewItemClick(object sender, System.EventArgs e)
        {
            DevExpress.XtraTreeList.TreeList treeView = sender as DevExpress.XtraTreeList.TreeList;

            var treeViewNode = treeView.FocusedNode;
            var record = (Models.ProjectTreeNode)treeView.GetDataRecordByNode(treeViewNode);
            var docId = record.Id;
            XmlNode node = record.Node;
            Models.FileDef itemFile = record.ItemFile;

            if (itemFile != null)
                AddNewDocument(itemFile, record.Name, node, docId);
        }

        private void AddNewDocument(Models.FileDef itemFile, string formName, XmlNode node, string docId)
        {
            if (tabbedView.Documents.Count > 0)
            {
                foreach (var item in tabbedView.Documents)
                {
                    //ako su istog naziva i lokacije pozovi
                    if (item.Caption == formName)
                    {
                        if (itemFile.xDocFile.ToLower() == ((Carpo.Designer.Modules.ucXmlNode)item.Control).fileDef.xDocFile.ToLower())
                        {
                            tabbedView.Controller.Activate(item);
                            return;
                        }
                    }
                }
            }

            tabbedView.BeginUpdate();
            Modules.ucXmlNode control = new Modules.ucXmlNode(itemFile, node, docId);
            BaseDocument document = tabbedView.AddDocument(control);
            document.Caption = formName;
            document.Image = fileTypeImages.Images[10];
            document.Footer = Directory.GetCurrentDirectory();
            tabbedView.EndUpdate();
            tabbedView.Controller.Activate(document);
        }

        private void iOpenFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                try
                {
                    if (dlg.ShowDialog() != DialogResult.OK)
                        return;

                    string safeFileName = dlg.SafeFileName;
                    string fileName = dlg.FileName;

                    OpenXmlInSolutionExplorer(fileName, safeFileName);

                    //Models.FileDef projectFile = SetFileDef(fileName);

                    ////provjera da li lista sadrži xml

                    //bool containsItem = treeList.Exists(item => item.ItemFile != null && item.ItemFile.xDocFile == projectFile.xDocFile);
                    //if (containsItem)
                    //    return;

                    ////xDocList.Add(projectFile);

                    //XmlNodeList xnList = projectFile.xDoc.SelectNodes("/root/form");

                    ////dodaj project u tree (root)

                    //string id = Guid.NewGuid().ToString();
                    //treeList.Add(new Models.ProjectTreeNode() { Id = id, Name = safeFileName, ItemType = Enums.Project.ProjectItem.Project });
                    //foreach (XmlNode xn in xnList)
                    //{
                    //    string name = "nedefinirano";
                    //    XmlAttribute attrName = xn.Attributes["name"];

                    //    XmlAttribute attrPath = xn.Attributes["path"];

                    //    Models.FileDef itemFile = projectFile;

                    //    XmlNode itemXn = xn;

                    //    if (attrPath != null)
                    //    {
                    //        string formPath = Core.Logic.XmlPath.GetPath(fileName, Core.Logic.XmlPath.FileType.Forms, attrPath.Value);
                    //        if (!string.IsNullOrEmpty(formPath))
                    //        {
                    //            itemFile = SetFileDef(formPath);
                    //            itemXn = GetNodeByName(itemFile.xDoc, attrName.Value);
                    //        }
                    //    }

                    //    if (attrName != null)
                    //        name = attrName.Value.ToString();

                    //    //dodaj formu iz projekta
                    //    string sId = Guid.NewGuid().ToString();
                    //    treeList.Add(new Models.ProjectTreeNode() { Id = sId, ItemType = Enums.Project.ProjectItem.FormExternal, Name = name, Node = itemXn, ParentId = id, ProjectFile = projectFile, ItemFile = itemFile });

                    //    //pronaði partove forme
                    //    findFormParts(itemFile.xDocFile, sId);
                    //}

                    //solutionExplorer.PopulateNodes(treeList);

                    //iAnaliza.Enabled = true;
                    //iStart.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private Models.FileDef SetFileDef(string filePath)
        {
            Models.FileDef project = new Models.FileDef();

            FileInfo fInfo = new FileInfo(filePath);

            string fileName = filePath;
            string safeFileName = fInfo.Name;
            project.xDocFile = fileName;
            project.xDoc.Load(fileName);
            return project;
        }

        private void findFormParts(string fileName, string parentId)
        {
            var doc = XDocument.Load(fileName);
            foreach (XElement formElement in doc.Descendants("root"))
            {
                foreach (XElement formPart in formElement.Descendants("part"))
                {
                    try
                    {
                        //dohvati part xml

                        var attrName = formPart.Attribute("name");

                        var attrPath = formPart.Attribute("path");

                        string formPath = Core.Logic.XmlPath.GetPath(fileName, Core.Logic.XmlPath.FileType.Parts, attrPath.Value);

                        Models.FileDef projectFile = SetFileDef(formPath);

                        Models.FileDef itemFile = projectFile;

                        //dohvati sve partove unutar xml.a
                        XmlNodeList xnList = projectFile.xDoc.SelectNodes("/parts");

                        foreach (XmlNode xn in xnList)
                        {
                            string name = "nedefinirano";

                            XmlNode itemXn = xn;

                            if (attrPath != null)
                            {
                                formPath = Core.Logic.XmlPath.GetPath(fileName, Core.Logic.XmlPath.FileType.Parts, attrPath.Value);
                                if (!string.IsNullOrEmpty(formPath))
                                {
                                    itemFile = SetFileDef(formPath);
                                    itemXn = GetPartByName(itemFile.xDoc, attrName.Value);
                                }
                            }

                            if (attrName != null)
                                name = attrName.Value.ToString();

                            object exists = treeList.Find(item => item.ParentId == parentId && item.ItemFile.xDocFile == projectFile.xDocFile);

                            string sId = Guid.NewGuid().ToString();

                            //ako veæ postoji taj part unutar drveta a nalazi se u drugoj formi dodaj ga s istim Id.em
                            if (exists != null)
                                sId = ((Carpo.Designer.Models.ProjectTreeNode)exists).Id;

                            bool containsItem = treeList.Exists(item => item.ParentId == parentId && item.ItemFile.xDocFile == projectFile.xDocFile);
                            //bool containsItem = xDocList.Exists(item => item.xDocFile == projectFile.xDocFile);
                            if (!containsItem)
                            {
                                //dodaj razlièite partove koji se pojavljuju u formi
                                //xDocList.Add(projectFile);
                                treeList.Add(new Models.ProjectTreeNode() { Id = sId, ItemType = Enums.Project.ProjectItem.FormSub, Name = name, Node = itemXn, ParentId = parentId, ProjectFile = projectFile, ItemFile = itemFile });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private XmlNode GetNodeByName(XmlDocument xDoc, string formName)
        {
            XmlNodeList xnList = xDoc.SelectNodes("/root/form[@name='" + formName + "']");
            if (xnList.Count > 0)
                return xnList[0];

            return null;
        }

        private XmlNode GetPartByName(XmlDocument xDoc, string partName)
        {
            XmlNodeList xnList = xDoc.SelectNodes("/parts/part[@name='" + partName + "']");
            if (xnList.Count > 0)
                return xnList[0];

            return null;
        }

        private void iAnaliza_ItemClick(object sender, ItemClickEventArgs e)
        {
            checkXmlList.Clear();
            foreach (Models.ProjectTreeNode projectTreeNode in treeList)
            {
                if (projectTreeNode.Node != null)
                {
                    CheckNode(projectTreeNode.Node, projectTreeNode.ItemFile.xDocFile, null);
                    TraverseNodesCheck(projectTreeNode.Node.ChildNodes, projectTreeNode.Node, projectTreeNode.ItemFile.xDocFile);
                }
            }

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

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            foreach (DevExpress.XtraBars.Docking2010.Views.Tabbed.Document document in tabbedView.Documents)
            {
                document.Tooltip = ((Carpo.Designer.Modules.ucXmlNode)document.Control).fileDef.xDocFile;
            }
        }

        private void iAddNewItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            //otvori formu sa predlošcima

            using (Forms.XmlTemplates frm = new Forms.XmlTemplates())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    OpenXmlInSolutionExplorer(frm.filePath, frm.Name);
                }
            }
        }

        private void OpenXmlInSolutionExplorer(string path, string safeName)
        {
            try
            {
                string safeFileName = safeName;
                string fileName = path;

                Models.FileDef projectFile = SetFileDef(fileName);

                //provjera da li lista sadrži xml

                bool containsItem = treeList.Exists(item => item.ItemFile != null && item.ItemFile.xDocFile == projectFile.xDocFile);
                if (containsItem)
                    return;

                //xDocList.Add(projectFile);

                XmlNodeList xnList = projectFile.xDoc.SelectNodes("/root/form");

                //dodaj project u tree (root)

                string id = Guid.NewGuid().ToString();
                treeList.Add(new Models.ProjectTreeNode() { Id = id, Name = safeFileName, ItemType = Enums.Project.ProjectItem.Project });
                foreach (XmlNode xn in xnList)
                {
                    string name = "nedefinirano";
                    XmlAttribute attrName = xn.Attributes["name"];

                    XmlAttribute attrPath = xn.Attributes["path"];

                    Models.FileDef itemFile = projectFile;

                    XmlNode itemXn = xn;

                    if (attrPath != null)
                    {
                        string formPath = Core.Logic.XmlPath.GetPath(fileName, Core.Logic.XmlPath.FileType.Forms, attrPath.Value);
                        if (!string.IsNullOrEmpty(formPath))
                        {
                            itemFile = SetFileDef(formPath);
                            itemXn = GetNodeByName(itemFile.xDoc, attrName.Value);
                        }
                    }

                    if (attrName != null)
                        name = attrName.Value.ToString();

                    //dodaj formu iz projekta
                    string sId = Guid.NewGuid().ToString();
                    treeList.Add(new Models.ProjectTreeNode() { Id = sId, ItemType = Enums.Project.ProjectItem.FormExternal, Name = name, Node = itemXn, ParentId = id, ProjectFile = projectFile, ItemFile = itemFile });

                    //pronaði partove forme
                    findFormParts(itemFile.xDocFile, sId);
                }

                solutionExplorer.PopulateNodes(treeList);

                iAnaliza.Enabled = true;
                iStart.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
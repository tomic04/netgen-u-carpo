using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Carpo.Designer.Forms
{
    public partial class XmlTemplates : Form
    {
        private static List<Models.XmlTemplate> templates = new List<Models.XmlTemplate>();

        public string filePath { get; set; }
        public string Name { get; set; }

        public XmlTemplates()
        {
            InitializeComponent();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void XmlTemplates_Load(object sender, EventArgs e)
        {
            PopulateTemplates();
        }

        private void PopulateTemplates()
        {
            try
            {
                SetTree();
                treeXmlTemplates.Nodes.Clear();
                templates.Clear();
                //templates.Add(new Models.XmlTemplate() { Name = "Forma", File = "" });
                ListDirectory(@"D:\Spin\Carpo.TestShell\Sokac\CarpoDesigner\templates");

                treeXmlTemplates.DataSource = templates;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void ListDirectory(string path)
        {
            var rootDirectory = new DirectoryInfo(path);
            var node = new TreeNode(rootDirectory.Name) { Tag = rootDirectory };

            var directoryInfo = (DirectoryInfo)node.Tag;

            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Extension == ".xml")
                    templates.Add(new Models.XmlTemplate() { Name = file.Name, File = file.FullName, Image = path + "\\" + Path.GetFileNameWithoutExtension(file.Name) + ".jpg" });
            }
        }

        private void SetTree()
        {
            treeXmlTemplates.OptionsView.ShowAutoFilterRow = true;

            treeXmlTemplates.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn() { Visible = true, FieldName = "Name" });
            treeXmlTemplates.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn() { Visible = true, FieldName = "File" });
            treeXmlTemplates.Columns[0].OptionsColumn.AllowSort = false;
            treeXmlTemplates.Columns[1].OptionsColumn.AllowSort = false;

            treeXmlTemplates.Columns[1].AppearanceHeader.Font = new Font(treeXmlTemplates.Columns[1].AppearanceHeader.Font, FontStyle.Italic);
            treeXmlTemplates.Columns[1].AppearanceCell.ForeColor = Color.FromArgb(100, 100, 100);
            treeXmlTemplates.Columns[1].Visible = false;

            treeXmlTemplates.OptionsBehavior.Editable = false;
        }

        private void treeXmlTemplates_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (treeXmlTemplates.FocusedNode != null && treeXmlTemplates.FocusedNode != treeXmlTemplates.Nodes.AutoFilterNode)
            {
                TreeListNode focusedNode = treeXmlTemplates.FocusedNode;
                var record = (Models.XmlTemplate)treeXmlTemplates.GetDataRecordByNode(focusedNode);
                PopulatePicture(record.Image);
                txtName.Text = record.Name;
                txtLocation.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
        }

        private void PopulatePicture(string imagPath)
        {
            try
            {
                picEdit.Image = Image.FromFile(@imagPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmdCreate_Click(object sender, EventArgs e)
        {
            try
            {
                TreeListNode focusedNode = treeXmlTemplates.FocusedNode;
                var record = (Models.XmlTemplate)treeXmlTemplates.GetDataRecordByNode(focusedNode);
                string location = txtLocation.Text;
                Name = txtName.Text;

                if (string.IsNullOrEmpty(location))
                    location = @"C:\Users\isokcevic\Desktop";

                if (string.IsNullOrEmpty(Name))
                    Name = Path.GetFileNameWithoutExtension(record.Name);

                //kreiraj xml file iz predloška sa zadanim nazivom i na zadanoj lokaciji
                var document = XDocument.Load(record.File);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(document.ToString());

                filePath = location + "\\" + Name;

                //provjeri da li postoji file
                if (File.Exists(filePath))
                    if (MessageBox.Show("File već postoji! Želite li ga prekopirati? ", "Poruka", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                        return;

                StreamWriter outStream = System.IO.File.CreateText(filePath);

                doc.Save(outStream);

                outStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //otvori dokument u solution exploreru
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void cmbBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtLocation.EditValue = fbd.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
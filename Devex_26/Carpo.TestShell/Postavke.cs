using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpo.TestShell
{
    public partial class Postavke : DevExpress.XtraEditors.XtraForm
    {
        public delegate void InvokeDelegate();

        internal string server = "";
        internal string database = "";
        internal string username = "";
        internal string password = "";
        internal string xml = "";
        internal int operateriId = 0;
        internal string operater = "";
        internal bool isXmlGen = false;
        internal string xmlGenParamList = "";

        private DataTable dtOperateri = null;

        private const int serverTimeOut = 15;

        private string connStr = "";

        public Postavke()
        {
            InitializeComponent();

            cmbOperater.Properties.Columns[0].Visible = false;
        }

        private void Postavke_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }

            cmbServer.EditValue = server = Properties.Settings.Default.Server;
            cmbBaza.EditValue = database = Properties.Settings.Default.Baza;
            txtLogin.EditValue = username = Properties.Settings.Default.Uid;
            txtPassword.EditValue = password = Properties.Settings.Default.Pwd;
            chkXmlGen.Checked = isXmlGen = Properties.Settings.Default.IsXmlGen;
            memoXmlGenParams.EditValue = xmlGenParamList = Properties.Settings.Default.XmlGenParameters;

            if (!string.IsNullOrEmpty(username))
                rdoAuth.SelectedIndex = 1;

            if (Properties.Settings.Default.Xml.Count() > 0)
                   txtXml.EditValue = xml = Properties.Settings.Default.Xml[Properties.Settings.Default.Xml.Count() - 1].XmlPath;

                operateriId = Properties.Settings.Default.OperateriId;

            try
            {
                GetOperateri();
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Info(ex.Message);
            }

            SetFocus();
            CheckHistoryButton();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (dxValidationProvider1.Validate())
            {
                server = cmbServer.Text.Trim();
                database = cmbServer.Text.Trim();
                if (rdoAuth.SelectedIndex == 1)
                {
                    username = txtLogin.Text.Trim();
                    password = txtPassword.Text.Trim();
                }
                else
                {
                    username = "";
                    password = "";
                }
                xml = txtXml.Text.Trim();
                operateriId = (int)cmbOperater.EditValue;
                operater = cmbOperater.Text.ToString();

                isXmlGen = chkXmlGen.Checked;
                xmlGenParamList = memoXmlGenParams.EditValue == null ? "" : memoXmlGenParams.EditValue.ToString();

                try
                {
                    Check();
                    Properties.Settings.Default.Server = server;
                    Properties.Settings.Default.Baza = database;
                    Properties.Settings.Default.Uid = username;
                    Properties.Settings.Default.Pwd = password;
                    Properties.Settings.Default.IsXmlGen = isXmlGen;
                    Properties.Settings.Default.XmlGenParameters = xmlGenParamList;
                    XmlDio(txtXml.Text.ToString(), server, database);
                    Properties.Settings.Default.OperateriId = operateriId;

                    Properties.Settings.Default.Save();

                    if (isXmlGen && memoXmlGenParams.EditValue != null)
                        xmlGenParamList = memoXmlGenParams.EditValue.ToString();

                    DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    Common.Helpers.JMessages.Error(ex, ex.Message);
                }
            }
        }

        private void XmlDio(string xmlPath, string server, string database)
        {
            bool xmlExist = false;
            int? index = null;

            int parseHelpIndex = xmlPath.LastIndexOf("\\");
            string xml = xmlPath.Substring(parseHelpIndex + 1);

            // Provjerit da li postoji identičan element u modelu
            foreach (Models.XmlHistory x in Properties.Settings.Default.Xml)
            {
                if (x.XmlPath == xmlPath && x.Server == server && x.Database == database)
                {
                    try
                    {
                        index = Properties.Settings.Default.Xml.FindIndex(y => y.Xml == x.Xml && y.Server == x.Server && y.Database == x.Database);
                        xmlExist = true;
                    }
                    catch { }

                    break;
                }
            }

            // Ako postoji - obrisati ga sa liste
            if (xmlExist)
                Properties.Settings.Default.Xml.RemoveAt((int)index);

            Properties.Settings.Default.Xml.Add(new Models.XmlHistory() { Xml = xml, XmlPath = xmlPath, Server = server, Database = database, Time = DateTime.Now });
        }

        private void CheckHistoryButton()
        {
            if (Properties.Settings.Default.Xml.Count > 0)
                txtXml.Properties.Buttons[1].Enabled = true;
            else
                txtXml.Properties.Buttons[1].Enabled = false;
        }

        private void Check()
        {
            try
            {
                CheckConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //if (!System.IO.File.Exists(xml))
            //{
            //throw new Exception("Xml file - neispravna putanja");
            //}
        }

        private void CheckConnection()
        {
            SetConnString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                }
            }
            catch
            {
                throw new Exception("Neispravna konekcija");
            }
        }

        private void SetConnString()
        {
            server = cmbServer.Text;
            database = cmbBaza.Text;
            if (rdoAuth.SelectedIndex == 1)
            {
                username = txtLogin.Text;
                password = txtPassword.Text;
            }
            else
            {
                username = "";
                password = "";
            }

            connStr = Carpo.Data.Constructor.SetConnectionString(server, database, username, password, serverTimeOut);
        }

        private void txtXml_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GetXmlPath();
        }

        private void txtXml_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
                GetXmlPath();
            else
            {
                using (var xmlHistory = new Forms.XmlHistory(Properties.Settings.Default.Xml))
                {
                    var result = xmlHistory.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        txtXml.EditValue = xmlHistory.ReturnXmlPath;

                        if (xmlHistory.SendConnection)
                        {
                            SetPassedConnection(xmlHistory.ReturnServer, xmlHistory.ReturnDatabase);
                        }
                    }
                    CheckHistoryButton();
                }
            }
        }

        private void SetPassedConnection(string server, string database)
        {
            cmbServer.EditValue = server; // postavio server
            AddDbNames(); // osvježio baze
            cmbBaza.EditValue = database; // postavio vrijednost baze
            SetConnString(); // osvježio connection string
            GetOperateri();// osvježio operatere
        }

        private void GetXmlPath()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML Files (*.xml)|*.xml";
            dlg.FilterIndex = 0;
            dlg.DefaultExt = "xml";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtXml.Text = dlg.FileName;
            }
        }

        private void SetFocus()
        {
            int focused = 0;

            foreach (Control c in this.Controls.Cast<Control>().OrderBy(x => x.TabIndex))
            {
                if (c is BaseEdit)
                {
                    if (c.Text != "")
                        continue;
                    else
                    {
                        c.Select();
                        focused++;
                        break;
                    }
                }
            }
            if (focused == 0)
                cmdOk.Select();
        }

        private void Osvjezi()
        {
            GetOperateri();
        }

        private void SetOperaterControl(bool enabled)
        {
            if (enabled == false)
                cmbOperater.Properties.DataSource = null;
            cmbOperater.Enabled = enabled;
        }

        private Task<int> GetOperateri()
        {
            SetConnString();
            //var task = Task.Factory.StartNew(() => GetOperateriTask());
            return Task.Factory.StartNew(() =>
            {
                GetOperateriTask();
                return 1;
            });
        }

        private void GetOperateriTask()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT OperateriId, RTrim(Naziv) as Naziv FROM ssoperateri where Active = 1 order by Naziv";

                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        SqlDataReader dr = cmd.ExecuteReader();

                        dtOperateri = new DataTable();
                        dtOperateri.Load(dr);

                        if (dtOperateri == null)
                        {
                            SetOperaterControl(false);
                            cmbOperater.BeginInvoke(new InvokeDelegate(InvokeIregularConn));
                            return;
                        }
                        else
                        {
                            SetOperaterControl(true);
                            cmbOperater.BeginInvoke(new InvokeDelegate(InvokeRegularConn));
                            return;
                        }
                    }
                }
            }
            catch
            { }
        }

        public void InvokeRegularConn()
        {
            cmbOperater.Properties.DataSource = dtOperateri;
            cmbOperater.Visible = true;
            lblOperaterErr.Visible = false;
            int id;
            if (Common.Helpers.Conversion.TryCast(operateriId, out id) == true)
                cmbOperater.EditValue = id;
        }

        public void InvokeIregularConn()
        {
            cmbOperater.Visible = false;
            lblOperaterErr.Visible = true;
        }

        private void rdoAuth_EditValueChanged(object sender, EventArgs e)
        {
            txtLogin.Enabled = rdoAuth.SelectedIndex == 1;
            txtPassword.Enabled = rdoAuth.SelectedIndex == 1;
        }

        private void pnlBaza_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            AddDbNames();
        }

        private void AddDbNames()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetServerConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("select name from master..sysdatabases ORDER BY Name", conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            cmbBaza.Properties.Items.Clear();
                            while (reader.Read())
                            {
                                string name = reader.GetString(0);
                                if ("master;model;tempdb;msdb;pubs".IndexOf(name) < 0)
                                {
                                    cmbBaza.Properties.Items.Add(name);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
        }

        public string GetServerConnectionString()
        {
            string connectionString = string.Format("data source={0};integrated security=SSPI; Connect Timeout={1}; Application Name=CrossGenDesigner", cmbServer.Text, serverTimeOut.ToString());
            if (rdoAuth.SelectedIndex == 1)
            {
                connectionString = string.Format("data source={0};user id={1};password={2}; Connect Timeout={3}; Application Name=CrossGenDesigner", cmbServer.Text, txtLogin.Text, txtPassword.Text, serverTimeOut.ToString());
            }
            return connectionString;
        }

        private void cmbBaza_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GetOperateri();
        }

        private void pnlOperater_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            GetOperateri();
        }

        private void Postavke_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void chkXmlGen_CheckedChanged(object sender, EventArgs e)
        {
            memoXmlGenParams.Enabled = isXmlGen = chkXmlGen.Checked;
        }
    }
}
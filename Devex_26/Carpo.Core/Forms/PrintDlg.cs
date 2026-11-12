using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace Carpo.Core.Forms
{
    public partial class PrintDlg : XtraForm
    {
        public enum Orientations { Free, Portrait, Landscape }

        public int copies = 0;
        public Duplex duplex = Duplex.Default;
        private List<Structure.Models.Print> printList;
        private bool enableDmsButton = false;
        private bool sendToDms = false;

        private DataTable dtCombo;
        private DataSet ds;
        private MemoryStream currentLayout;
        private GridView gv;

        public PrintDlg(GridView Gv, DataTable DtCombo, DataSet Ds, MemoryStream CurrentLayout, bool panelDuplexVisible = false)
        {
            Initialize(null);

            dtCombo = DtCombo;
            ds = Ds;
            currentLayout = CurrentLayout;
            gv = Gv;

            if (dtCombo != null && dtCombo.Rows.Count > 0)
                SetCombo(dtCombo);

            gbOrientation.Enabled = true;
            rgOrientation.SelectedIndex = 1;

            if (panelDuplexVisible)
                panelDuplex.Visible = panelDuplexVisible;
        }

        public PrintDlg(List<Structure.Models.Print> printList)
        {
            Initialize(printList);
        }

        public void Initialize(List<Structure.Models.Print> printList)
        {
            this.enableDmsButton = CheckForDmsParameters(printList);
            //SkinManager.EnableFormSkins();
            InitializeComponent();

            chkDms.Enabled = enableDmsButton;

            if (printList != null)
                this.printList = printList;

            SetPrinters();
            Settings();
            TranslateControls();
        }

        #region Methods

        private void SetCombo(DataTable dtCombo)
        {
            lePredlosci.Properties.ShowFooter = false;
            lePredlosci.Properties.DisplayMember = "Naziv";
            lePredlosci.Properties.ValueMember = "CarpoGridGlaId";
            lePredlosci.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CarpoGridGlaId") { Visible = false });
            lePredlosci.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Naziv"));
            lePredlosci.Properties.ActionButtonIndex = 1;
            lePredlosci.Properties.DataSource = dtCombo;
            lePredlosci.Enabled = true;
        }

        private void SetPrinters()
        {
            string lastSelectedPrinter = "";
            string defaultPrinter = "";

            foreach (string p in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                //RepositoryItemComboBox item = new RepositoryItemComboBox();

                cbPisaci.Properties.Items.Add(p);
                System.Drawing.Printing.PrinterSettings printer = new System.Drawing.Printing.PrinterSettings();
                printer.PrinterName = p;

                if (printer.IsDefaultPrinter)
                    defaultPrinter = p;
            }

            SetSelectedPrinter(defaultPrinter, lastSelectedPrinter);
        }

        private void SetSelectedPrinter(string defaultPrinter, string lastSelectedPrinter)
        {
            using (RegistryKey sckey = Registry.CurrentUser.CreateSubKey(@"Software\SPIN Informatica\Jupiter Software\PrintDlg"))
            {
                try
                {
                    lastSelectedPrinter = sckey.GetValue("LastSelectedPrinter").ToString();
                }
                catch { }
            }

            if (!(string.IsNullOrEmpty(lastSelectedPrinter))) // tražim zadnje označeni printer u registryu
            {
                cbPisaci.Text = lastSelectedPrinter;
            }
            else if (!(string.IsNullOrEmpty(defaultPrinter))) // ako ne postoji zadnje označen tražim defaultni printer
            {
                cbPisaci.Text = defaultPrinter;
            }
            else // ako ne postoji defaultni onda uzimam prvi sa liste
            {
                cbPisaci.Text = System.Drawing.Printing.PrinterSettings.InstalledPrinters[0];
            }
        }

        private void Settings()
        {
            using (RegistryKey sckey = Registry.CurrentUser.CreateSubKey(@"Software\SPIN Informatica\Jupiter Software\PrintDlg"))
            {
                try
                {
                    bool preview = bool.Parse(sckey.GetValue("Preview", "true").ToString());

                    if (preview == false || (printList != null && printList.Count > 1))
                        chkIspis.Checked = true;
                    else
                        chkPrikaz.Checked = true;

                    if (printList != null && printList.Count > 1) //za multiprint dozvoli samo print, ne i prikaz na monitor
                        chkPrikaz.Enabled = false;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message, "Carpo.Core.Forms PrintDlg.Settings");
                }
            }
        }

        public void Print()
        {
            if (!enableDmsButton || !chkDms.Checked)
                SetDefault();

            int copiesLoc = chkIspis.Checked ? Convert.ToInt32(numCopies.EditValue) : 0;
            this.copies = Convert.ToInt32(numCopies.EditValue);
            string printerName = cbPisaci.Text;

            if (printList != null)
            {
                foreach (var print in printList)
                {
                    try
                    {
                        Print.DoPrint doPrint = new Print.DoPrint(print.Template, print.Parameters, printerName, copiesLoc,
                            Core.PubVars.OperateriId, chkDms.Checked);
                        doPrint.Run();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(ex.Message, "Carpo.Core.Forms PrintDlg.Print");
                    }
                }
            }
        }

        private void SetDefault()
        {
            using (RegistryKey sckey = Registry.CurrentUser.CreateSubKey(@"Software\SPIN Informatica\Jupiter Software\PrintDlg"))
            {
                sckey.SetValue("Preview", chkPrikaz.Checked.ToString());
                sckey.SetValue("LastSelectedPrinter", cbPisaci.EditValue.ToString());
            }
        }

        private void GetPrinterProperties()
        {
            PrinterSettings ps = new PrinterSettings();
            ps.PrinterName = cbPisaci.Text; // Load the appropriate printer's setting
        }

        public bool Preview
        {
            get
            {
                return chkPrikaz.Checked;
            }
        }

        public Orientations Orientation
        {
            get
            {
                if (rgOrientation.SelectedIndex == 0)
                    return Orientations.Portrait;
                else
                    return Orientations.Landscape;
            }
            set
            {
                gbOrientation.Enabled = (value == Orientations.Free);
            }
        }

        public string Device
        {
            get
            {
                if (chkIspis.Checked)
                    return cbPisaci.Text;
                else
                    return "";
            }
            set
            {
                cbPisaci.Text = value;
            }
        }

        private void TranslateControls()
        {
            this.Text = Common.Translator.TranslateString("Parametri ispisa");
            chkPrikaz.Text = Common.Translator.TranslateString("Prikaz na monitor");
            chkIspis.Text = Common.Translator.TranslateString("Ispis na pisač");
            chkDms.Text = Common.Translator.TranslateString("Pohrani u DMS");
            gbPredlosci.Text = Common.Translator.TranslateString("PREDLOŽAK");
            gbPisac.Text = Common.Translator.TranslateString("PISAČ");
            labelControl1.Text = Common.Translator.TranslateString("Status:");
            labelControl3.Text = Common.Translator.TranslateString("Tip:");
            labelControl4.Text = Common.Translator.TranslateString("Lokacija:");
            gbOrientation.Text = Common.Translator.TranslateString("ORIJENTACIJA");
            gbKopije.Text = Common.Translator.TranslateString("KOPIJE");
            labelControl7.Text = Common.Translator.TranslateString("Broj kopija:");
            gbDuplex.Text = Common.Translator.TranslateString("JEDNOSTRANO/OBOSTRANO");
            btnOk.Text = Common.Translator.TranslateString("PRIKAŽI");
            cmdOdustani.Text = Common.Translator.TranslateString("ODUSTANI");
        }

        #endregion Methods

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void chkIspis_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIspis.Checked == true)
            {
                btnOk.Text = Common.Translator.TranslateString("ISPIŠI");
                chkIspis.Image = Properties.Resources.check_32;
                chkPrikaz.Image = null;
                chkDms.Image = null;
            }
        }

        private void chkPrikaz_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPrikaz.Checked == true)
            {
                btnOk.Text = Common.Translator.TranslateString("PRIKAŽI");
                chkPrikaz.Image = Properties.Resources.check_32;
                chkIspis.Image = null;
                chkDms.Image = null;
            }
        }

        private void chkDms_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDms.Checked == true)
            {
                btnOk.Text = Common.Translator.TranslateString("POHRANI");
                chkDms.Image = Properties.Resources.check_32;
                chkIspis.Image = null;
                chkPrikaz.Image = null;
            }

            sendToDms = chkDms.Checked;
        }

        private void lePredlosci_EditValueChanged(object sender, EventArgs e)
        {
            if (lePredlosci.EditValue == null)
            {
                currentLayout.Seek(0, SeekOrigin.Begin);
                gv.RestoreLayoutFromStream(currentLayout);
            }
            else
            {
                foreach (DataTable dataTable in ds.Tables)
                {
                    if (dataTable.Rows.Count > 0
                        && Convert.ToInt32(dataTable.Rows[0]["CarpoGridGlaId"]) == Convert.ToInt32(lePredlosci.EditValue))
                    {
                        Carpo.Common.Helpers.GridTemplates.SetGridColumnsFromTemplate(dataTable, gv);
                        return;
                    }
                }
            }
        }

        private void lePredlosci_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete && lePredlosci.EditValue != null)
                lePredlosci.EditValue = null;
        }

        private void PrintDlg_Load(object sender, EventArgs e)
        {
            cmbDuplex.Properties.Items.Add(Common.Translator.TranslateString("Defaultne postavke pisača"));
            cmbDuplex.Properties.Items.Add(Common.Translator.TranslateString("Jednostrano"));
            cmbDuplex.Properties.Items.Add(Common.Translator.TranslateString("Obostrano"));

            cmbDuplex.SelectedIndex = 0;
        }

        private void cmbDuplex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDuplex.Text == Common.Translator.TranslateString("Defaultne postavke pisača"))
                this.duplex = Duplex.Default;
            else if (cmbDuplex.Text == Common.Translator.TranslateString("Jednostrano"))
                this.duplex = Duplex.Simplex;
            else if (cmbDuplex.Text == Common.Translator.TranslateString("Obostrano"))
                this.duplex = Duplex.Horizontal;
        }

        #endregion Events

        private bool CheckForDmsParameters(List<Structure.Models.Print> printList)
        {
            if (printList == null || printList.Count == 0)
                return false;

            foreach (var list in printList)
            {
                if (!string.IsNullOrEmpty(list.Parameters))
                {
                    string[] listArr = list.Parameters.Split(',');
                    if (listArr.Length > 0)
                    {
                        foreach (var parameter in listArr)
                        {
                            string[] parameterSplited = parameter.Split('=');
                            if (parameterSplited.Length > 0)
                            {
                                if (parameterSplited[0].Trim().ToLower() == "@dmsid" || parameterSplited[0].Trim().ToLower() == "@dmsentityname")
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.Specific.Cjenik
{
    public partial class Kalkulacija : XtraUserControl, IDisposable
    {
        #region properties

        private List<Control> controls = new List<Control>();
        private string action = "", direction = "neto_bruto", controlForFocus = string.Empty; // controlForFocus J.K.;
        private int selectedState = 0, isInit = 1, rabatButtonEnabled = 0, rabatCassaEnableDisable = 0, disableKalk = 0;
        private decimal rabatEnterValue, kolicina, cijenaBod, cijena, rabatPosto = 0, cassaPosto = 0, porezPosto = 0;
        private bool resetcalcvalues = false, chkKalk = false, isRabatChanged = false, infiniteLoopHelper = false;

        private string valutaTekst, bodTekst;

        public bool InitOnMaster { get; set; }

        //******input parametri - start

        public int SelectedState
        {
            get
            {
                return selectedState;
            }
            set
            {
                selectedState = value;
                SetSelectedState(selectedState);
            }
        }

        public int RabatButtonEnabled
        {
            get { return rabatButtonEnabled; }
            set
            {
                rabatButtonEnabled = value;
                RabatEnable();
            }
        }

        public int IsInit
        {
            get { return isInit; }
            set
            {
                rdoCijenaPDV.Checked = false; // dodao J.K. zbog casea 87112
                direction = "neto_bruto"; // dodao J.K. zbog casea 87112
                isInit = value;
            }
        }

        public int RabatCassaEnableDisable
        {
            get { return rabatCassaEnableDisable; }
            set
            {
                rabatCassaEnableDisable = value;
            }
        }

        public decimal Kolicina
        {
            get
            {
                return kolicina;
            }
            set
            {
                kolicina = value;
                ValueChanging("Kolicina");
            }
        }

        public bool ChkKalk
        {
            get { return chkKalk; }
            set
            {
                chkKalk = value;
                SetChkKalk();
            }
        }

        public int DisableKalk // dodao J.K. da se pomoću parametra iz Carpa može enableat ili disableat checkbox Kalkulacija 
        {
            get { return disableKalk; }
            set
            {
                disableKalk = value;
                chkKalkulacija.Enabled = !Convert.ToBoolean(disableKalk);
            }
        }

        public string ControlForFocus // dodao J.K. da se pomoću parametra iz Carpa može staviti fokus na neku kontrolu
        {
            get { return controlForFocus; }
            set
            {
                controlForFocus = value;

                if (!Convert.ToBoolean(disableKalk))
                    SetFocusToControl(controlForFocus);
            }
        }

        public bool ResetCalcValues
        {
            get { return resetcalcvalues; }
            set
            {
                resetcalcvalues = value;
                if (resetcalcvalues == true)
                    ResetAllValues();
            }
        }

        public decimal CijenaBod
        {
            get { return cijenaBod; }
            set
            {
                cijenaBod = value;
                ValueChanging("CijenaBod");
            }
        }

        public decimal Cijena
        {
            get { return cijena; }
            set
            {
                cijena = value;
                ValueChanging("Cijena");
            }
        }

        public decimal RabatPosto
        {
            get { return rabatPosto; }
            set
            {
                rabatPosto = value;
                ValueChanging("RabatPosto");
            }
        }

        public decimal CassaPosto
        {
            get { return cassaPosto; }
            set
            {
                cassaPosto = value;
                ValueChanging("CassaPosto");
            }
        }

        public decimal PorezPosto
        {
            get { return porezPosto; }
            set
            {
                porezPosto = value;
                ValueChanging("PorezPosto");
            }
        }

        public decimal Iznos { get; private set; } = 0;
        public decimal IznosBod { get; private set; } = 0;
        public decimal Rabat { get; private set; } = 0;
        public decimal RabatBod { get; private set; } = 0;
        public decimal Cassa { get; private set; } = 0;
        public decimal CassaBod { get; private set; } = 0;
        public decimal Porez { get; private set; } = 0;
        public decimal PorezBod { get; private set; } = 0;
        public decimal CijenaPDV { get; private set; } = 0;
        public decimal CijenaPDVBod { get; private set; } = 0;
        public decimal IznosPDV { get; private set; } = 0;
        public decimal IznosPDVBod { get; private set; } = 0;
        public decimal RabatVrijednost1 { get; set; } = 0;
        public decimal RabatVrijednost2 { get; set; } = 0;
        public decimal RabatVrijednost3 { get; set; } = 0;
        public decimal RabatVrijednost4 { get; set; } = 0;
        public decimal RabatVrijednost5 { get; set; } = 0;
        public decimal Bod { get; set; } = 1;
        public string ConnectionString { get; set; } = "";

        //****Novi parametri***** //
        public decimal NaknadaTR { get; set; }

        public decimal NaknadaPO { get; set; }
        public decimal NaknadaNZ { get; set; }
        public decimal Trosarine { get; set; }
        public decimal TrosarineBod { get; set; }
        public decimal OsnovnaCijena { get; set; }
        public decimal IznosBezPDV { get; set; }
        public decimal IznosBezPDVBod { get; set; }
        //******input parametri - end

        public string ValutaTekst
        {
            get { return valutaTekst; }
            set
            {
                ProcessLayoutControls("lblKn", value);
            }
        }

        public string BodTekst
        {
            get { return bodTekst; }
            set
            {
                ProcessLayoutControls("lblBod", value);
            }
        }

        #endregion properties

        public Kalkulacija()
        {
            InitializeComponent();

            //UnRegisterEvents();
            RegisterEvents();

            InitControls();
            CalculateEnabled();
            //Formulas();
            FormulasNew();

            //foreach (LayoutControlItem item in layoutControlGroup1.Items)
            //{
            //    item.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 7);
            //}

            //rdoCijenaNeto.TabStop = false;
        }

        private void Kalkulacija_Load(object sender, EventArgs e)
        {
            ForceTabStop();
        }

        private void RegisterEvents()
        {
            this.layoutControl1.Enter += layoutControl1_Enter;
            this.Load += Kalkulacija_Load;
            //-------------------------------------------------------------------
            this.btnRabat.Click += btnRabat_Click;
            //-------------------------------------------------------------------
            this.chkKalkulacija.CheckedChanged += chkKalkulacija_CheckedChanged;
            //-------------------------------------------------------------------
            this.rdoCijena.CheckedChanged += rdoTip_CheckedChanged;
            this.rdoCijenaPDV.CheckedChanged += rdoTip_CheckedChanged;
            this.rdoIznos.CheckedChanged += rdoTip_CheckedChanged;
            //-------------------------------------------------------------------
            this.txtPorezPosto.EditValueChanged += txt_EditValueChanged;
            this.txtCassaPosto.EditValueChanged += txt_EditValueChanged;
            this.txtRabatPosto.EditValueChanged += txt_EditValueChanged;
            this.txtRabatPosto.Enter += txtRabatPosto_Enter;
            this.txtRabatPosto.Leave += txtRabatPosto_Leave;
            //-------------------------------------------------------------------
            this.txtCijena.EditValueChanged += txt_EditValueChanged;
            this.txtIznos.EditValueChanged += txt_EditValueChanged;
            this.txtRabat.EditValueChanged += txt_EditValueChanged;
            this.txtCassa.EditValueChanged += txt_EditValueChanged;
            this.txtCijenaPDV.EditValueChanged += txt_EditValueChanged;
            //-------------------------------------------------------------------
            this.txtCijenaBod.EditValueChanged += txt_EditValueChanged;
            this.txtIznosBod.EditValueChanged += txt_EditValueChanged;
            this.txtRabatBod.EditValueChanged += txt_EditValueChanged;
            this.txtCassaBod.EditValueChanged += txt_EditValueChanged;
            this.txtCijenaPDVBod.EditValueChanged += txt_EditValueChanged;
        }

        private void UnRegisterEvents()
        {
            this.layoutControl1.Enter -= layoutControl1_Enter;
            this.Load -= Kalkulacija_Load;
            //-------------------------------------------------------------------
            this.btnRabat.Click -= btnRabat_Click;
            //-------------------------------------------------------------------
            this.chkKalkulacija.CheckedChanged -= chkKalkulacija_CheckedChanged;
            //-------------------------------------------------------------------
            this.rdoCijena.CheckedChanged -= rdoTip_CheckedChanged;
            this.rdoCijenaPDV.CheckedChanged -= rdoTip_CheckedChanged;
            this.rdoIznos.CheckedChanged -= rdoTip_CheckedChanged;
            //-------------------------------------------------------------------
            this.txtPorezPosto.EditValueChanged -= txt_EditValueChanged;
            this.txtCassaPosto.EditValueChanged -= txt_EditValueChanged;
            this.txtRabatPosto.EditValueChanged -= txt_EditValueChanged;
            this.txtRabatPosto.Enter -= txtRabatPosto_Enter;
            this.txtRabatPosto.Leave -= txtRabatPosto_Leave;
            //-------------------------------------------------------------------
            this.txtCijena.EditValueChanged -= txt_EditValueChanged;
            this.txtIznos.EditValueChanged -= txt_EditValueChanged;
            this.txtRabat.EditValueChanged -= txt_EditValueChanged;
            this.txtCassa.EditValueChanged -= txt_EditValueChanged;
            this.txtCijenaPDV.EditValueChanged -= txt_EditValueChanged;
            //-------------------------------------------------------------------
            this.txtCijenaBod.EditValueChanged -= txt_EditValueChanged;
            this.txtIznosBod.EditValueChanged -= txt_EditValueChanged;
            this.txtRabatBod.EditValueChanged -= txt_EditValueChanged;
            this.txtCassaBod.EditValueChanged -= txt_EditValueChanged;
            this.txtCijenaPDVBod.EditValueChanged -= txt_EditValueChanged;
        }

        private void InitControls()
        {
            foreach (LayoutControlItem item in layoutControl1.Root.Items)
            {
                if (item.Control != null)
                {
                    controls.Add(item.Control);
                }
            }

            FormatTextControls();
        }

        private void ResetAllValues()
        {
            RabatPosto = 0;
            CassaPosto = 0;
            PorezPosto = 0;
            Cijena = 0;
            resetcalcvalues = false;
            RabatVrijednost1 = 0;
            RabatVrijednost2 = 0;
            RabatVrijednost3 = 0;
            RabatVrijednost4 = 0;
            RabatVrijednost5 = 0;
        }

        private void ValueChanging(string var)
        {
            if (action == "")
            {
                if (infiniteLoopHelper == false)
                {
                    infiniteLoopHelper = true;

                    if (var == "Cijena")
                        CijenaBod = Cijena / Bod;
                    else if (var == "CijenaBod")
                        Cijena = CijenaBod * Bod;
                }

                action = "TextChanging";
                try
                {
                    //Formulas();
                    FormulasNew();
                    CalculateEnabled();
                }
                finally
                {
                    action = "";
                    infiniteLoopHelper = false;
                }
            }
        }

        public void SetFocusToControl(string controlName) // dodao J.K. da se iz Carpa može staviti fokus na neku kontrolu
        {
            Control[] controlForFocus = Controls.Find(controlName, true);

            if (controlForFocus.Length == 1)
            {
                chkKalkulacija.Checked = true;
                controlForFocus.First().Enabled = true;
                controlForFocus.First().Focus();
            }
        }

        private void FormatTextControls()
        {
            var textControls = controls.OfType<TextEdit>();
            foreach (Control control in textControls)
            {
                TextEdit textEdit = (control as TextEdit);
                textEdit.Tag = textEdit.Name.Replace("txt", "");
                textEdit.EnterMoveNextControl = true;
                textEdit.Properties.AppearanceDisabled.ForeColor = Color.Black;
                Common.Helpers.Formats.FormatTextEdit("N2", textEdit.Properties);
                textEdit.Enter += (s, e) =>
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        textEdit.SelectionStart = 0;
                        textEdit.SelectionLength = textEdit.Text.Length;
                    }));
                };
            }

            //ove kontrole se formatiraju na 4 decimale, ostale su na 2
            Common.Helpers.Formats.FormatTextEdit("N4", txtCijena.Properties);
            Common.Helpers.Formats.FormatTextEdit("N4", txtCijenaBod.Properties);
            Common.Helpers.Formats.FormatTextEdit("N4", txtCijenaPDVBod.Properties);
            Common.Helpers.Formats.FormatTextEdit("N4", txtCijenaPDV.Properties);
            Common.Helpers.Formats.FormatTextEdit("N4", txtRabatPosto.Properties);

            //ako je direction brutto-netto onda iznosbod i iznos postavi na 4 decimale
            if (direction == "bruto_neto")
            {
                Common.Helpers.Formats.FormatTextEdit("N4", txtIznos.Properties);
                Common.Helpers.Formats.FormatTextEdit("N4", txtIznosBod.Properties);
            }
        }

        private void CalculateEnabled()
        {
            layoutControl1.BeginUpdate();
            try
            {
                CalculateEnabled(chkKalkulacija.Checked);
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
            finally
            {
                layoutControl1.EndUpdate();
            }
        }

        private void SetChkKalk()
        {
            if (chkKalk == true && chkKalkulacija.Checked == false)
                chkKalkulacija.Checked = true;
            else if (chkKalk == false && chkKalkulacija.Checked)
                chkKalkulacija.Checked = false;
        }

        private void SetSelectedState(int state)
        {
            switch (state)
            {
                case 0:
                    rdoCijena.Checked = true;
                    break;

                case -1:
                    rdoIznos.Checked = true;
                    break;

                case -2:
                    rdoCijenaPDV.Checked = true;
                    break;

                default:
                    break;
            }
        }

        private void CalculateEnabled(bool calculate)
        {
            txtCijenaBod.Enabled = calculate && rdoCijena.Checked;
            txtCijena.Enabled = calculate && rdoCijena.Checked;

            txtIznosBod.Enabled = calculate && rdoIznos.Checked;
            txtIznos.Enabled = calculate && rdoIznos.Checked;

            txtRabatPosto.Enabled = (calculate && CijenaBod != 0 && (rabatButtonEnabled == 0 || rabatButtonEnabled == 2)) || (calculate && RabatCassaEnableDisable == 1);
            txtRabatBod.Enabled = calculate && CijenaBod != 0 && rabatButtonEnabled == 0;
            txtRabat.Enabled = calculate && CijenaBod != 0 && rabatButtonEnabled == 0;
            txtCassaPosto.Enabled = (calculate && CijenaBod != 0) || (calculate && RabatCassaEnableDisable == 1);
            txtCassaBod.Enabled = calculate && CijenaBod != 0;
            txtCassa.Enabled = calculate && CijenaBod != 0;
            txtPorezPosto.Enabled = calculate && CijenaBod != 0;
            //txtPorezBod.Enabled = calculate && CijenaNetoBod != 0;
            //txtPorez.Enabled = calculate && CijenaNetoBod != 0;

            txtCijenaPDVBod.Enabled = calculate && rdoCijenaPDV.Checked;
            txtCijenaPDV.Enabled = calculate && rdoCijenaPDV.Checked;

            rdoCijena.Enabled = calculate;
            rdoCijenaPDV.Enabled = calculate;
            rdoIznos.Enabled = calculate;

            //txtRabatPosto.Enabled = calculate &&  RabatCassaEnableDisable == 1;
            //txtCassaPosto.Enabled = calculate &&  RabatCassaEnableDisable == 1;
        }

        private void FormulasNew()
        {
            txtRabatPosto.EditValue = rabatPosto;
            txtCassaPosto.EditValue = cassaPosto;
            txtPorezPosto.EditValue = porezPosto;

            //novo
            Trosarine = NaknadaNZ + NaknadaPO + NaknadaTR;
            TrosarineBod = (NaknadaNZ + NaknadaPO + NaknadaTR) / Bod;

            //prvi slučaj - Model cijena
            if (direction == "neto_bruto")
            {
                //bodovi calc
                txtCijenaBod.EditValue = CijenaBod;
                txtIznosBod.EditValue = IznosBod = Decimal.Round(CijenaBod * kolicina, 2, MidpointRounding.AwayFromZero);
                txtRabatBod.EditValue = RabatBod = Decimal.Round(Decimal.Round((IznosBod - TrosarineBod * Kolicina) * rabatPosto, 4, MidpointRounding.AwayFromZero) / 100, 4, MidpointRounding.AwayFromZero);
                txtCassaBod.EditValue = CassaBod = Decimal.Round((Decimal.Round((((IznosBod - TrosarineBod * Kolicina) - RabatBod) * cassaPosto), 4, MidpointRounding.AwayFromZero) / 100), 4, MidpointRounding.AwayFromZero);
                txtPorezBod.EditValue = PorezBod = Decimal.Round((Decimal.Round(((IznosBod - RabatBod - CassaBod) * porezPosto), 4, MidpointRounding.AwayFromZero) / 100), 4, MidpointRounding.AwayFromZero);
                txtIznosBezPDVBod.EditValue = IznosBezPDVBod = IznosBod - RabatBod - CassaBod;
                txtIznosPDVBod.EditValue = IznosPDVBod = IznosBod + PorezBod - RabatBod - CassaBod;

                //iznosi calc
                txtCijena.EditValue = Cijena;
                txtIznos.EditValue = Iznos = Decimal.Round(Cijena * Kolicina, 2, MidpointRounding.AwayFromZero);
                txtRabat.EditValue = Rabat = Decimal.Round((Iznos - Trosarine * Kolicina) * (RabatPosto / 100), 4, MidpointRounding.AwayFromZero);
                txtCassa.EditValue = Cassa = Decimal.Round(((Iznos - Trosarine * Kolicina) - Rabat) * (CassaPosto / 100), 4, MidpointRounding.AwayFromZero);
                txtIznosBezPDV.EditValue = IznosBezPDV = Iznos - Rabat - Cassa;
                txtPorez.EditValue = Porez = Decimal.Round(IznosBezPDV * (PorezPosto / 100), 4, MidpointRounding.AwayFromZero);
                txtIznosPDV.EditValue = IznosPDV = IznosBezPDV + Porez;

                if (kolicina != 0)
                {
                    txtCijenaPDV.EditValue = CijenaPDV = IznosPDV / Kolicina;
                    txtCijenaPDVBod.EditValue = CijenaPDVBod = IznosPDVBod / kolicina;
                }
                else
                {
                    txtCijenaPDVBod.EditValue = CijenaPDVBod = 0;
                    txtCijenaPDV.EditValue = CijenaPDV = 0;
                }
            }
            else if (direction == "bruto_neto")
            {
                //bodovi calc

                txtCijenaPDVBod.EditValue = CijenaPDVBod;
                txtIznosPDVBod.EditValue = IznosPDVBod = CijenaPDVBod * kolicina;
                txtPorezBod.EditValue = PorezBod = Decimal.Round(IznosPDVBod - IznosPDVBod / (1 + PorezPosto / 100), 2, MidpointRounding.AwayFromZero);
                txtIznosBezPDVBod.EditValue = IznosBezPDVBod = IznosPDVBod - PorezBod;
                txtCassaBod.EditValue = CassaBod = Decimal.Round(((IznosBezPDVBod - TrosarineBod * Kolicina) / (1 - CassaPosto / 100)) - (IznosBezPDVBod - TrosarineBod * Kolicina), 2, MidpointRounding.AwayFromZero);
                txtRabatBod.EditValue = RabatBod = Decimal.Round((((IznosBezPDVBod - TrosarineBod * Kolicina) + CassaBod) / (1 - RabatPosto / 100)) - ((IznosBezPDVBod - TrosarineBod * Kolicina) + CassaBod), 4, MidpointRounding.AwayFromZero);
                txtIznosBod.EditValue = IznosBod = Decimal.Round(IznosBezPDVBod + RabatBod + CassaBod, 4, MidpointRounding.AwayFromZero);

                //vrijednosti calc
                txtCijenaPDV.EditValue = CijenaPDV;
                txtIznosPDV.EditValue = IznosPDV = Decimal.Round(CijenaPDV * Kolicina, 2, MidpointRounding.AwayFromZero);
                txtPorez.EditValue = Porez = Decimal.Round(IznosPDV - IznosPDV / (1 + PorezPosto / 100), 2, MidpointRounding.AwayFromZero);
                txtIznosBezPDV.EditValue = IznosBezPDV = IznosPDV - Porez;
                txtCassa.EditValue = Cassa = Decimal.Round(((IznosBezPDV - Trosarine * Kolicina) / (1 - CassaPosto / 100)) - (IznosBezPDV - Trosarine * Kolicina), 2, MidpointRounding.AwayFromZero);
                txtRabat.EditValue = Rabat = Decimal.Round((((IznosBezPDV - Trosarine * Kolicina) + Cassa) / (1 - RabatPosto / 100)) - ((IznosBezPDV - Trosarine * Kolicina) + Cassa), 4, MidpointRounding.AwayFromZero);
                txtIznos.EditValue = Iznos = IznosBezPDV + Rabat + Cassa;

                if (kolicina != 0)
                {
                    txtCijena.EditValue = Cijena = Iznos / Kolicina;
                    txtCijenaBod.EditValue = CijenaBod = IznosBod / Kolicina;
                }
                else
                {
                    txtCijena.EditValue = Cijena = 0;
                    txtCijenaBod.EditValue = CijenaBod = 0;
                }

                //vrati round
                txtRabatBod.EditValue = Decimal.Round(RabatBod, 2, MidpointRounding.AwayFromZero);
                txtRabat.EditValue = Decimal.Round(Rabat, 2, MidpointRounding.AwayFromZero);
            }

            OsnovnaCijena = Cijena - Trosarine;
        }

        private void Formulas()
        {
            txtRabatPosto.EditValue = rabatPosto;
            txtCassaPosto.EditValue = cassaPosto;
            txtPorezPosto.EditValue = porezPosto;

            if (direction == "bruto_neto")
            {
                CijenaBod = Decimal.Round(CijenaPDVBod / ((1 - (rabatPosto / 100)) * (1 - (cassaPosto / 100)) * (1 + (porezPosto / 100))), 4, MidpointRounding.AwayFromZero);
                Cijena = Decimal.Round(CijenaPDV / ((1 - (rabatPosto / 100)) * (1 - (cassaPosto / 100)) * (1 + (porezPosto / 100))), 4, MidpointRounding.AwayFromZero);
            }

            txtCijenaBod.EditValue = CijenaBod;
            txtIznosBod.EditValue = IznosBod = Decimal.Round(CijenaBod * kolicina, 2, MidpointRounding.AwayFromZero);
            txtRabatBod.EditValue = RabatBod = Decimal.Round(Decimal.Round(IznosBod * rabatPosto, 2, MidpointRounding.AwayFromZero) / 100, 4, MidpointRounding.AwayFromZero);
            txtCassaBod.EditValue = CassaBod = Decimal.Round((Decimal.Round(((IznosBod - RabatBod) * cassaPosto), 2, MidpointRounding.AwayFromZero) / 100), 4, MidpointRounding.AwayFromZero);
            txtPorezBod.EditValue = PorezBod = Decimal.Round((Decimal.Round(((IznosBod - RabatBod - CassaBod) * porezPosto), 2, MidpointRounding.AwayFromZero) / 100), 4, MidpointRounding.AwayFromZero);

            txtCijena.EditValue = Cijena;
            txtIznos.EditValue = Iznos = Decimal.Round(Cijena * kolicina, 2, MidpointRounding.AwayFromZero);
            txtRabat.EditValue = Rabat = Decimal.Round(Decimal.Round(Iznos * rabatPosto, 2, MidpointRounding.AwayFromZero) / 100, 4, MidpointRounding.AwayFromZero);
            txtCassa.EditValue = Cassa = Decimal.Round((Decimal.Round(((Iznos - Rabat) * cassaPosto), 2, MidpointRounding.AwayFromZero) / 100), 4, MidpointRounding.AwayFromZero);
            txtPorez.EditValue = Porez = Decimal.Round((Decimal.Round(((Iznos - Rabat - Cassa) * porezPosto), 2, MidpointRounding.AwayFromZero) / 100), 4, MidpointRounding.AwayFromZero);

            if (direction == "neto_bruto")
            {
                //if (kolicina != 0)
                //{
                //    txtCijenaBrutoBod.EditValue = CijenaBrutoBod = CijenaNetoBod + ((PorezBod - RabatBod - CassaBod) / kolicina);
                //    txtCijenaBruto.EditValue = CijenaBruto = CijenaNeto + ((Porez - Rabat - Cassa) / kolicina);
                //}
                //else
                //{
                //    txtCijenaBrutoBod.EditValue = CijenaBrutoBod = 0;
                //    txtCijenaBruto.EditValue = CijenaBruto = 0;
                //}

                txtIznosPDVBod.EditValue = IznosPDVBod = IznosBod + PorezBod - RabatBod - CassaBod;
                txtIznosPDV.EditValue = IznosPDV = Iznos + Porez - Rabat - Cassa;

                if (kolicina != 0)
                {
                    txtCijenaPDVBod.EditValue = CijenaPDVBod = Decimal.Round(IznosPDVBod / kolicina, 4, MidpointRounding.AwayFromZero);
                    txtCijenaPDV.EditValue = CijenaPDV = Decimal.Round(IznosPDV / kolicina, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    txtCijenaPDVBod.EditValue = CijenaPDVBod = 0;
                    txtCijenaPDV.EditValue = CijenaPDV = 0;
                }
            }
            else
            {
                txtCijenaPDVBod.EditValue = CijenaPDVBod;
                txtCijenaPDV.EditValue = CijenaPDV;

                txtIznosPDVBod.EditValue = CijenaPDVBod * kolicina;
                txtIznosPDV.EditValue = CijenaPDV * kolicina;
            }
        }

        private void RabatEnable()
        {
            if (rabatButtonEnabled > 0 && chkKalkulacija.Checked)
                btnRabat.Enabled = true;
            else
                btnRabat.Enabled = false;
        }

        private void ForceTabStop()
        {
            //DevEx bug?!
            //forsiraj tabfocus = false jer radio automatski dobiva fokus prilikom inicijalnog čekiranja
            //i prilikom čekiranja i prilikom load-a (u nekim situacijama - kada je inicijalno dijsejblano)
            rdoCijena.TabStop = rdoCijenaPDV.TabStop = rdoIznos.TabStop = false;
        }

        private void rdoTip_CheckedChanged(object sender, EventArgs e)
        {
            selectedState = Convert.ToInt32((sender as CheckEdit).Tag);

            if (selectedState == -2)
                direction = "bruto_neto";
            else
                direction = "neto_bruto";

            CalculateEnabled();
            FormatTextControls();
            ForceTabStop();
        }

        private void chkKalkulacija_CheckedChanged(object sender, EventArgs e)
        {
            chkKalk = chkKalkulacija.Checked;
            RabatEnable();
            CalculateEnabled();
        }

        private void txt_EditValueChanged(object sender, EventArgs e)
        {
            if (action == "TextChanging")
                return;

            action = "TextChanging";
            try
            {
                TextEdit textEdit = sender as TextEdit;
                decimal value = Convert.ToDecimal(textEdit.EditValue);
                decimal calc_temp = 0;
                object tag = textEdit.Tag;

                if (tag == null)
                    return;
                switch (tag.ToString())
                {
                    case "CijenaBod":
                        CijenaBod = value;
                        Cijena = Decimal.Round(CijenaBod * Bod, 4, MidpointRounding.AwayFromZero);
                        CalculateEnabled();
                        break;

                    case "Cijena":
                        Cijena = value;
                        CijenaBod = Decimal.Round(Cijena / Bod, 4, MidpointRounding.AwayFromZero);
                        CalculateEnabled();
                        break;

                    case "Iznos":
                        if (Kolicina != 0)
                        {
                            Cijena = value / Kolicina;
                            CijenaBod = Cijena / Bod;
                            CalculateEnabled();
                        }
                        break;

                    case "IznosBod":
                        if (Kolicina != 0)
                        {
                            CijenaBod = value / Kolicina;
                            Cijena = Decimal.Round(CijenaBod * Bod, 4, MidpointRounding.AwayFromZero);
                            CalculateEnabled();
                        }
                        break;

                    case "RabatPosto":
                        RabatPosto = value;
                        RabatVrijednost1 = Convert.ToDecimal(txtRabatPosto.Text);
                        RabatVrijednost2 = 0;
                        RabatVrijednost3 = 0;
                        RabatVrijednost4 = 0;
                        RabatVrijednost5 = 0;
                        break;

                    case "Rabat":
                        if (Kolicina != 0 && value != 0)
                            RabatPosto = value == 0 ? 0 : 100 / (Iznos / value);
                        break;

                    case "RabatBod":
                        if (Kolicina != 0 && value != 0)
                            RabatPosto = value == 0 ? 0 : 100 / (IznosBod / value);
                        break;

                    case "CassaPosto":
                        CassaPosto = value;
                        break;

                    case "Cassa":
                        if (value != 0)
                            calc_temp = (Iznos - Rabat) / value;
                        if (calc_temp != 0)
                            CassaPosto = value == 0 ? 0 : 100 / calc_temp;
                        break;

                    case "CassaBod":
                        if (value != 0)
                            calc_temp = (IznosBod - RabatBod) / value;
                        if (calc_temp != 0)
                            CassaPosto = value == 0 ? 0 : 100 / calc_temp;
                        break;

                    case "PorezPosto":
                        PorezPosto = value;
                        break;

                    case "CijenaPDV":
                        CijenaPDV = value;
                        CijenaPDVBod = Decimal.Round(CijenaPDV / Bod, 4, MidpointRounding.AwayFromZero);
                        CalculateEnabled();
                        break;

                    case "CijenaPDVBod":
                        CijenaPDVBod = value;
                        CijenaPDV = Decimal.Round(CijenaPDVBod * Bod, 4, MidpointRounding.AwayFromZero);
                        CalculateEnabled();
                        break;
                }
                //Formulas();
                FormulasNew();
            }
            catch (Exception ex)
            {
                Common.Helpers.JMessages.Error(ex, ex.Message);
            }
            finally
            {
                action = "";
            }
        }

        private void btnRabat_Click(object sender, EventArgs e)
        {
            using (var rabati = new Forms.Rabati())
            {
                if (rabatButtonEnabled == 2 && isRabatChanged == true && isInit == 0)
                    rabati.RabatVrijednost1 = Convert.ToDecimal(txtRabatPosto.Text);
                else
                {
                    rabati.RabatVrijednost1 = RabatVrijednost1;
                    rabati.RabatVrijednost2 = RabatVrijednost2;
                    rabati.RabatVrijednost3 = RabatVrijednost3;
                    rabati.RabatVrijednost4 = RabatVrijednost4;
                    rabati.RabatVrijednost5 = RabatVrijednost5;
                }
                rabati.ConnectionString = ConnectionString;
                var result = rabati.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtRabatPosto.EditValue = rabati.ZbrojeniRabat;
                    Rabat = rabati.ZbrojeniRabat;
                    RabatVrijednost1 = rabati.RabatVrijednost1;
                    RabatVrijednost2 = rabati.RabatVrijednost2;
                    RabatVrijednost3 = rabati.RabatVrijednost3;
                    RabatVrijednost4 = rabati.RabatVrijednost4;
                    RabatVrijednost5 = rabati.RabatVrijednost5;
                    isRabatChanged = false;
                    if (isInit != 0)
                        isInit = 0;
                }
            }
        }

        private void txtRabatPosto_Leave(object sender, EventArgs e)
        {
            if (Convert.ToDecimal(txtRabatPosto.EditValue) != rabatEnterValue)
            {
                isRabatChanged = true;
                isInit = 0;
            }
        }

        private void txtRabatPosto_Enter(object sender, EventArgs e)
        {
            rabatEnterValue = Convert.ToDecimal(txtRabatPosto.EditValue);
        }

        // zakomentirano na zahtjev Z.Kasalo (HD-52)

        private bool layoutControlActivated = false;

        private void layoutControl1_Enter(object sender, EventArgs e)
        {
            if (layoutControlActivated == false)
            {
                if (chkKalkulacija.Checked == false)
                    chkKalkulacija.Focus();
            }

            layoutControlActivated = true;
        }

        private void ProcessLayoutControls(string prefix, string value)
        {
            foreach (var item in layoutControlGroup1.Items)
            {
                if (item is LayoutControlItem)
                {
                    var v = (item as LayoutControlItem);

                    if (v.Control is LabelControl)
                    {
                        var c = v.Control as LabelControl;
                        if (c.Name.StartsWith(prefix))
                        {
                            c.Text = value;
                        }
                    }
                }
            }
        }
    }
}
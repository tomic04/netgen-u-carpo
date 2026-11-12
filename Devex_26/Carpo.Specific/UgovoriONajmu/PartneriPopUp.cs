using Carpo.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.Specific.UgovoriONajmu
{
    public partial class PartneriPopUp : Form
    {
        public string LnPartneriIdArrayString { get; set; }
        private DataTable GridDS { get; set; }
        public int ugovorinajamglaid { get; set; } = 0;

        private int? partneriid { get; set; } = null;
        private int? lastKnownPartneriId { get; set; }
        private string lastKnownPartneriText { get; set; }

        private Ugovori ugovori = new Ugovori();

        public PartneriPopUp()
        {
            InitializeComponent();
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            InsertPartner();
        }

        private void btnSpremi_Click(object sender, EventArgs e)
        {
            LnPartneriIdArrayString = "";
            foreach (DataRow partner in GridDS.Rows)
                LnPartneriIdArrayString = LnPartneriIdArrayString + "," + partner[1];

            if (!string.IsNullOrEmpty(LnPartneriIdArrayString))
                LnPartneriIdArrayString = LnPartneriIdArrayString.Substring(1);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SearchEditLookupEditButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Search)
            {
                FilterDTandOpenSearchForm(sender);
            }
            else if (e.Button.Kind == ButtonPredefines.Delete)
            {
                nazivid.EditValue = string.Empty; // ako sam poslao idName onda je search, moram u null
                this.partneriid = null;
                nazivid.Properties.Buttons[0].Enabled = false;
            }
        }

        private void PartneriPopUp_Load(object sender, EventArgs e)
        {
            nazivid.ButtonClick += (s, ev) =>
            {
                SearchEditLookupEditButtonClick(s, ev);
            };
            nazivid.Properties.Buttons[1].IsDefaultButton = true;

            GridDS = new DataTable();

            GridDS.Columns.Add("Partner");
            GridDS.Columns.Add("PartneriId");

            if (!string.IsNullOrEmpty(LnPartneriIdArrayString))
            {
                var partneriidlist = new List<int>();
                foreach (DataRow partner in GridDS.Rows)
                    partneriidlist.Add(Convert.ToInt32(partner[1]));

                var dt = GetSessionPartneri();
                foreach (DataRow dr in dt.Rows)
                {
                    if (partneriidlist.Contains(Convert.ToInt32(dr[1])))
                        continue;
                    GridDS.Rows.Add();
                    GridDS.Rows[GridDS.Rows.Count - 1][0] = dr[0];
                    GridDS.Rows[GridDS.Rows.Count - 1][1] = dr[1];
                }
            }

            // ako je postavljen string onda znači da je nešto mijenjano i ne želim više učitavati iz baze. String sada ima prednost
            if (this.ugovorinajamglaid != 0 && string.IsNullOrEmpty(LnPartneriIdArrayString))
            {
                var dt = GetUneseniPartneri();
                foreach (DataRow dr in dt.Rows)
                {
                    GridDS.Rows.Add();
                    GridDS.Rows[GridDS.Rows.Count - 1][0] = dr[0];
                    GridDS.Rows[GridDS.Rows.Count - 1][1] = dr[1];
                }
            }
            this.gridControl1.DataSource = this.GridDS;
        }

        private DataTable TraziPartneri(string searchString)
        {
            try
            {
                string sql = "exec spTraziPartneri @Action='Partneri', @broj_lokacija=1, @Uvjet='" + searchString + "'";
                return Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
            }
            catch
            { return null; }
        }

        // odabrani na ovoj formi od prije (ako se odaberu, ugasi forma i ponovno otvara, želim ih zapamtiti da ne moram ponovno iste odabrati)
        private DataTable GetSessionPartneri()
        {
            string sql = "select naziv,partneriid from partneri where partneriid in (";
            var partneriidArray = LnPartneriIdArrayString.Split(',').ToArray();
            foreach (var partneriid in partneriidArray)
                sql = sql + partneriid + ",";
            sql = sql.Substring(0, sql.Length - 1) + ")";
            return Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
        }

        // uneseno u tablicu u bazi - želim na formi za izmjenu učitati odabrane partnere prilikom unosa
        private DataTable GetUneseniPartneri()
        {
            string sql = "select p.Naziv as Partner, ln.partneriid from UgovoriNajamGla_lnpartneri ln left join Partneri p on p.PartneriId = ln.partneriid where UgovoriNajamGlaId = " + this.ugovorinajamglaid;
            return Carpo.Data.Common.ExecuteQuery(Connection.ConnectionString, sql, Carpo.Data.Common.DataType.Text, null).Tables[0];
        }

        private void InsertPartner()
        {
            if (string.IsNullOrEmpty(nazivid.Text))
                return;

            var partneriIdList = new List<int>();
            foreach (DataRow row in GridDS.Rows)
                partneriIdList.Add(Convert.ToInt32(row[1]));

            if (partneriIdList.Contains(Convert.ToInt32(this.partneriid)))
                MessageBox.Show("Partner je već odabran.");
            else if (this.partneriid != null)
            {
                GridDS.Rows.Add();
                GridDS.Rows[GridDS.Rows.Count - 1][0] = this.nazivid.Text;
                GridDS.Rows[GridDS.Rows.Count - 1][1] = this.partneriid;

                this.nazivid.EditValue = null;
                this.partneriid = null;
            }
            this.btnSpremi.Focus();
        }

        private void btnUkloni_Click(object sender, EventArgs e)
        {
            this.GridDS.Rows.RemoveAt(this.gridView1.FocusedRowHandle);
        }

        private void nazivid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && (sender as TextEdit).Text.Length > 0)
            {
                FilterDTandOpenSearchForm(sender);
            }
        }

        private void FilterDTandOpenSearchForm(object sender)
        {
            try
            {
                if (!string.IsNullOrEmpty((sender as ButtonEdit).Text))
                {
                    var tblFiltered = TraziPartneri((sender as ButtonEdit).Text);

                    Forms.Search searchForm = new Forms.Search(tblFiltered, "", "partneriid");
                    searchForm.ShowDialog();

                    if (searchForm.DialogResult == DialogResult.OK && searchForm.SearchId != null)
                    {
                        // setiraj textualnu vrijednost kada nađem partnera po zatvaranju search forme
                        (sender as ButtonEdit).EditValue = tblFiltered.AsEnumerable()
                            .Where(row => row.Field<Int32>("partneriid") == searchForm.SearchId).FirstOrDefault()["nazivid"].ToString();
                        // setiraj id partnera kada nađem partnera po zatvaranju search forme
                        this.partneriid = searchForm.SearchId;
                        // tag - indikator da li je našao partnera
                        (sender as ButtonEdit).Tag = (sender as ButtonEdit).EditValue.ToString();

                        this.lastKnownPartneriId = Convert.ToInt32(searchForm.SearchId);
                        this.lastKnownPartneriText = (sender as ButtonEdit).EditValue.ToString();
                        this.btnDodaj.Focus();
                    }
                    else
                    {
                        (sender as ButtonEdit).Tag = "noSearchId";
                        this.btnDodaj.Focus();
                    }
                }
                else
                {
                    if (this.lastKnownPartneriId != null)
                        MessageBox.Show("Niste unijeli pojam pretraživanja.", "Upozorenje");
                }
            }
            catch (Exception) { }
        }

        private void nazivid_Leave(object sender, EventArgs e)
        {
            OnLeave(sender, e);
        }

        private void OnLeave(object s, EventArgs er)
        {
            try
            {
                // nije došao iz metode tražilice (tab ili clear)
                if ((s as ButtonEdit).Tag == null || string.IsNullOrEmpty((s as ButtonEdit).Tag.ToString()))
                {
                    // ako je prije bio prazan string - (samo prelazim preko kontrole (tab))
                    if (this.partneriid == null
                        && (s as ButtonEdit).EditValue != null && !string.IsNullOrEmpty((s as ButtonEdit).EditValue.ToString()))
                    {
                        FilterDTandOpenSearchForm(s);
                        OnLeave(s, er);
                    }
                    // ako je prije bila postavljena vrijednost
                    else
                    {
                        // clear
                        if ((s as ButtonEdit).EditValue != null && string.IsNullOrEmpty((s as ButtonEdit).EditValue.ToString()))
                            SetSearchEditValues(null, string.Empty); // - dinamički index
                        // tab
                        else
                            SetSearchEditValues(lastKnownPartneriId, lastKnownPartneriText); // - dinamički index
                    }
                }
                // bio je u tražilici
                //else if ((s as ButtonEdit).Tag != null && !string.IsNullOrEmpty((s as ButtonEdit).Tag.ToString()))
                else
                {
                    //nije našao Id u tražilici
                    if ((s as ButtonEdit).Tag.ToString().ToLower() == "nosearchid")
                    {
                        // ako je prije bio prazan string
                        if (this.partneriid == null)
                            SetSearchEditValues(null, string.Empty); // - dinamički index
                        else
                            SetSearchEditValues(lastKnownPartneriId, lastKnownPartneriText); // - dinamički index
                    }
                    //našao je Id u tražilici -> samo odi dalje
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("OnLeave(): " + ex.Message, "Upozorenje");
            }
            finally
            {
                nazivid.Properties.Buttons[0].Enabled = nazivid.Text == string.Empty ? false : true;
            }
        }

        private void SetSearchEditValues(int? partneriid, string nazivid)
        {
            this.partneriid = partneriid;
            this.nazivid.Text = nazivid;

            lastKnownPartneriText = nazivid;
            if (partneriid == null)
                lastKnownPartneriId = null;
            else
                lastKnownPartneriId = Convert.ToInt32(partneriid);
        }

        private void nazivid_Enter(object sender, EventArgs e)
        {
            nazivid.Tag = null;
        }

        private void PartneriPopUp_Shown(object sender, EventArgs e)
        {
            this.nazivid.Focus();
        }
    }
}
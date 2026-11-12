using System;
using System.Data;

namespace Carpo.Specific.Matpos.Cjenik
{
    public partial class Cjenik : Shared.Forms.Jupiter
    {
        public Cjenik()
        {
            InitializeComponent();
        }

        private bool running = false;

        /** input params**/
        public string ConnectionString { get; set; }
        public int SSOperateriId { get; set; } = 0;
        public decimal nabCijena { get; set; }
        public int skladisteId { get; set; }
        public int robaId { get; set; }

        public int nabpriglaid { get; set; }

        public int MPModel { get; set; } = 0;
        public int IndMPFix { get; set; }

        /**output params**/

        //za spremanje u storu
        public decimal nettoCijenaKn { get; set; }

        public decimal MPCijena { get; set; }
        public decimal MPCijenaStara { get; set; }
        public decimal VPMarza { get; set; }
        public decimal MPMarza { get; set; }
        public int Action { get; set; }
        public int vrCjenikId { get; set; }
        public int cjenikId { get; set; }
        /**/

        public decimal mpm { get; set; }
        public decimal vpm { get; set; }
        public decimal mpc { get; set; }
        public decimal ntc { get; set; }

        object porez;
        decimal porezDecimal;

        private void Cjenik_Load(object sender, EventArgs e)
        {
            //populate grid
            populateGrid();
        }

        private void populateGrid()
        {
            try
            {
                grdCjenik.DataSource = null;
                grdCjenik.DataSource = Data.Matpos.PriListKalkCjenik.GetCjenikData(this.robaId, this.skladisteId, this.nabCijena);
            }
            finally
            {
            }
        }

        private void gvCjenik_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (running) // OCD
                return;

            running = true;

            MPModel = (Int32)gvCjenik.GetFocusedRowCellValue(gridColumn14);
            IndMPFix = (Int32)gvCjenik.GetFocusedRowCellValue(gridColumn15);
            //col2
            nabCijena = gvCjenik.GetFocusedRowCellValue(gridColumn2).ToString() == "" ? 0 : Convert.ToDecimal(gvCjenik.GetFocusedRowCellValue(gridColumn2));
            //col4
            nettoCijenaKn = gvCjenik.GetFocusedRowCellValue(gridColumn4).ToString() == "" ? 0 : Convert.ToDecimal(gvCjenik.GetFocusedRowCellValue(gridColumn4));
            //col6
            MPCijena = gvCjenik.GetFocusedRowCellValue(gridColumn6).ToString() == "" ? 0 : Convert.ToDecimal(gvCjenik.GetFocusedRowCellValue(gridColumn6));
            //col7
            MPCijenaStara = gvCjenik.GetFocusedRowCellValue(gridColumn7).ToString() == "" ? 0 : Convert.ToDecimal(gvCjenik.GetFocusedRowCellValue(gridColumn7));
            //col8
            VPMarza = gvCjenik.GetFocusedRowCellValue(gridColumn8).ToString() == "" ? 0 : Convert.ToDecimal(gvCjenik.GetFocusedRowCellValue(gridColumn8));
            //col10
            MPMarza = gvCjenik.GetFocusedRowCellValue(gridColumn10).ToString() == "" ? 0 : Convert.ToDecimal(gvCjenik.GetFocusedRowCellValue(gridColumn10));

            int EditedColumn = e.Column.AbsoluteIndex + 1;

            //if (e.Column.ColumnHandle != EditedColumn) return;

            if (MPModel == 0)
            {
                if (EditedColumn == 4)
                {
                    if (nabCijena == 0)
                    {
                        vpm = 100;
                        mpm = MPMarza;
                    }
                    else
                    {
                        vpm = ((nettoCijenaKn - nabCijena) / nabCijena) * 100;
                        mpm = MPMarza;
                    }
                    updateGrid(nettoCijenaKn, MPCijena, vpm, mpm);
                }
                else if (EditedColumn == 6)
                {
                    if (nabCijena == 0)
                    {
                        vpm = VPMarza;
                        mpm = 100;
                    }
                    else
                    {
                        vpm = VPMarza;
                        // mpm = ((MPCijena / (1 + MPCijenaStara / 100) - nabCijena) / nabCijena) * 100; // J.K. dodao na Martinin zahtjev (mail) u sklopu casea 88473

                        porez = ((DataTable)grdCjenik.DataSource).Rows[e.RowHandle]["porez"];
                        porezDecimal = Decimal.Parse(porez.ToString());
                        //mpm = ((MPCijena / (1 + ((int)porez / 100)) - nabCijena) / nabCijena) * 100; // J.K. dodao na Martinin zahtjev (mail) u sklopu casea 88473
                        mpm = ((MPCijena / (1 + (porezDecimal / 100)) - nabCijena) / nabCijena) * 100; // J.K. izmijenio na Sinišin zahtjev - u sklopu casea 89217
                    }

                    updateGrid(nettoCijenaKn, MPCijena, vpm, mpm);
                }
                else if (EditedColumn == 8)
                {
                    vpm = VPMarza;
                    nettoCijenaKn = (vpm * nabCijena) / 100 + nabCijena;
                    updateGrid(nettoCijenaKn, MPCijena, VPMarza, MPMarza);
                }
                else if (EditedColumn == 10)
                {
                    mpm = MPMarza;

                    porez = ((DataTable)grdCjenik.DataSource).Rows[e.RowHandle]["porez"];
                    porezDecimal = Decimal.Parse(porez.ToString());

                    //MPCijena = (mpm * nabCijena / 100 + nabCijena) * (1 + MPCijenaStara / 100);
                    MPCijena = (((mpm * nabCijena) / 100) + nabCijena) * (1 + (porezDecimal / 100)); // J.K. izmijenio na Sinišin zahtjev - u sklopu casea 89217
                    updateGrid(nettoCijenaKn, MPCijena, VPMarza, MPMarza);
                }
            }
            else
            {
                if (EditedColumn == 4)
                {
                    if (nabCijena == 0)
                    {
                        vpm = 100;
                        mpm = MPMarza;
                    }
                    else
                    {
                        vpm = ((nettoCijenaKn - nabCijena) / nabCijena) * 100;
                        if (IndMPFix == 0)
                        {
                            mpm = MPMarza;
                            MPCijena = nettoCijenaKn * (1 + MPCijenaStara / 100) * (1 + MPMarza / 100);
                        }
                        else
                        {
                            mpm = (MPCijena / (nettoCijenaKn * (1 + MPMarza / 100)) - 1) * 100;
                        }
                    }
                    updateGrid(nettoCijenaKn, MPCijena, vpm, mpm);
                }
                else if (EditedColumn == 6)
                {
                    if (nabCijena == 0)
                    {
                        mpm = 100;
                        vpm = VPMarza;
                    }
                    else
                    {
                        vpm = VPMarza;
                        mpm = ((MPCijena / (1 + MPCijenaStara / 100)) / nettoCijenaKn - 1) * 100;
                    }
                    updateGrid(nettoCijenaKn, MPCijena, vpm, mpm);
                }
                else if (EditedColumn == 8)
                {
                    vpm = VPMarza;
                    nettoCijenaKn = (vpm * nabCijena) / 100 + nabCijena;

                    if (IndMPFix == 0)
                    {
                        mpc = nettoCijenaKn * (MPCijenaStara / 100) * (1 + MPMarza / 100);
                    }
                    else
                    {
                        mpm = (MPCijena / (1 + MPCijenaStara / 100)) - MPCijena;
                    }
                    updateGrid(nettoCijenaKn, MPCijena, VPMarza, MPMarza);
                }
                else if (EditedColumn == 10)
                {
                    mpm = MPMarza;
                    MPCijena = nettoCijenaKn * (1 + MPMarza / 100) * (1 + MPCijenaStara / 100);
                    updateGrid(nettoCijenaKn, MPCijena, VPMarza, MPMarza);
                }
            }

            //bind values in grid

            running = false;
        }

        private void updateGrid(decimal ntc, decimal mpc, decimal vpm, decimal mpm)
        {
            gvCjenik.SetFocusedRowCellValue(gridColumn4, ntc);
            gvCjenik.SetFocusedRowCellValue(gridColumn6, mpc);
            gvCjenik.SetFocusedRowCellValue(gridColumn8, vpm);
            gvCjenik.SetFocusedRowCellValue(gridColumn10, mpm);
        }

        private void btnOdustani_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSpremi_Click(object sender, EventArgs e)
        {
            string sqlText = "";
            //spremi za svaki cjenik u bazu
            if (gvCjenik.RowCount > 0)
            {
                for (int i = 0; i < gvCjenik.DataRowCount; i++)
                {
                    int handle = Convert.ToInt32(gvCjenik.GetRowHandle(i));

                    vrCjenikId = gvCjenik.GetRowCellValue(handle, gridColumn16) != DBNull.Value ? Convert.ToInt32(gvCjenik.GetRowCellValue(handle, gridColumn16)) : 0;
                    nettoCijenaKn = gvCjenik.GetRowCellValue(handle, gridColumn4).ToString() == "" ? 0 : Convert.ToDecimal(gvCjenik.GetRowCellValue(handle, gridColumn4));
                    MPCijena = gvCjenik.GetRowCellValue(handle, gridColumn6).ToString() == "" ? 0 : Convert.ToDecimal(gvCjenik.GetRowCellValue(handle, gridColumn6));
                    cjenikId = gvCjenik.GetRowCellValue(handle, gridColumn17) != DBNull.Value ? Convert.ToInt32(gvCjenik.GetRowCellValue(handle, gridColumn17)) : 0;

                    try
                    {
                        Data.Matpos.PriListKalkCjenik.SpremiCjenik(1, cjenikId, nettoCijenaKn, MPCijena, robaId, skladisteId, SSOperateriId, vrCjenikId, nabpriglaid, out sqlText);
                        Logger.InfoData("Cjenik: Spremi cjenik", sqlText, DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Cjenik: Greska: ", ex.Message, ex);
                        //MessageBox.Show(ex.Message);
                    }
                }
            }

            this.Close();
        }
    }
}
namespace Carpo.Specific.Cjenik
{
    public partial class Calculation : DevExpress.XtraEditors.XtraUserControl
    {
        private decimal cijena;
        public decimal Cijena { get { return cijena; } set { cijena = value; textEdit1.Text = cijena.ToString(); } }

        public Calculation()
        {
            InitializeComponent();
        }
    }
}
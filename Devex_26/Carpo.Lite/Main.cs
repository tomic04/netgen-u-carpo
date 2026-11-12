using System.IO;
using System.Windows.Forms;

namespace Carpo.Lite
{
    public partial class Main : Form
    {
        public Main()
        {
            string server = Properties.Settings.Default.Server;
            string baza = Properties.Settings.Default.Baza;
            string Uid = Properties.Settings.Default.Uid;
            string Pwd = Properties.Settings.Default.Pwd;
            int OperateriId = Properties.Settings.Default.OperateriId;
            InitializeComponent();

            Gen.Start start = new Gen.Start();
            string xml = "C:\\Users\\ntomic\\Desktop\\ComponentTracker.xml";
            start.TestXmlPath = xml;
            start.ModuleId = Path.GetFileName(xml);
            start.Run(server, baza, Uid, Pwd, OperateriId, 1837690, 0, 0, "");

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }
    }
}
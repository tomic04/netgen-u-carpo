using DevExpress.XtraSplashScreen;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Carpo.TestShell.Forms
{
    public partial class XmlGen : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        private SplashScreenManager splashScreenManager;

        public XDocument parsedXdoc;

        private string connectionStr { get; set; }
        private string operateriId { get; set; }
        private string variablesList { get; set; }
        private string xmlSchemePath { get; set; }
        private XDocument xmlScheme { get; set; }

        public XmlGen(string ConnectionStr, string OperateriId, string VariablesList, IntPtr ParentHandle,
            SplashScreenManager SplashScreenManager, string XmlSchemePath = "", XDocument XmlScheme = null)
        {
            connectionStr = ConnectionStr;
            operateriId = OperateriId;
            variablesList = VariablesList;
            SetParent(this.Handle, ParentHandle);
            splashScreenManager = SplashScreenManager;
            xmlSchemePath = XmlSchemePath;
            xmlScheme = XmlScheme;

            InitializeComponent();
        }

        public bool Start()
        {
            try
            {
                using (var cls = new Carpo.XmlGen.Start())
                {
                    cls.IsTestShell = true;
                    var xDocument = cls.GetXmlObject(connectionStr, operateriId, variablesList, "xdocument",
                        xmlScheme: xmlScheme, splashScreenManager: splashScreenManager);

                    if (xDocument != null)
                    {
                        PopulateWebBrowser(xDocument as XDocument);
                        return true;
                    }

                    MessageBox.Show("Neuspješna obrada XML-a", "Carpo.TestShell.Forms.Start()");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        private void PopulateWebBrowser(XDocument parsedXdoc)
        {
            try
            {
                string filePath = xmlSchemePath.Substring(0, xmlSchemePath.Length - 4) + "_XmlGen.xml";
                parsedXdoc.Save(filePath);

                webBrowser1.Navigate(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Carpo.TestShell.Forms.PopulateWebBrowser()");
            }
        }
    }
}
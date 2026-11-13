using Carpo.Core;
using Carpo.Structure.Models;
using Carpo.Structure.Models.Core;
using Carpo.UI.Connector;
using Carpo.UI.Logic;
using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Forms.Base
{
    public partial class JupiterShared : Jupiter.Base
    {
        public string FormName { get; set; }
        public bool IsSettingLayout { get; set; } = false;
        public bool IsLoadEvent { get; set; } = false;
        public bool IsInitEvent { get; set; } = false;
        private bool isF5clicked = false;

        private int? keyCode = null;
        private Keys modifier;

        public int? KeyCode
        {
            get
            {
                return keyCode;
            }
            set
            {
                if (value != null)
                {
                    keyCode = value;
                }
            }
        }

        public Keys Modifier
        {
            get
            {
                return modifier;
            }
            set
            {
                modifier = value;
            }
        }

        public List<ControlEvents> EventList { get; set; } = new List<ControlEvents>();

        //**** ako postoji definiran name na dropdownitem-u spremem u listu da bi mogao pronaći
        //**** enable/disable potrebe
        public List<BarButtonItem> BarButtonItems = new List<BarButtonItem>();

        //**** ako postoji definiran name na menuitemu (grid, tree), spremem u listu da bi mogao pronaći
        //**** enable/disable potrebe
        public List<ToolStripMenuItem> MenuItems = new List<ToolStripMenuItem>();

        public List<Vars> CarpoVarsLayout { get; set; } = new List<Vars>(); //potrebno za snimljene varijable (layout..)

        //public List<Vars> CarpoVarsGlobal { get; set; } = new List<Vars>(); //potrebno za globalne session varijable
        public List<Vars> CarpoVarsSession { get; set; } = new List<Vars>(); //potrebno za session varijable

        public List<Vars> CarpoVarsApp { get; set; } = new List<Vars>(); // potrebno za app varijable

        public List<Components.JCustomMethod> CustomMethods { get; set; } = new List<Components.JCustomMethod>();

        /// <summary>
        /// TempAction - koristi se nakon zatvaranja forme i otvaranja nove preko actiona
        /// </summary>
        internal JEventAction EventAction { get; set; }

        //public string MainParameter { get; set; } = ""; //kod otvaranja nove forme (trenutno edit) i reference prema DataSet-u
        //public List<JControlDefinition> JControlList { get; set; } = new List<JControlDefinition>();
        //public List<JRepositoryDefinition> JRepositoryList { get; set; } = new List<JRepositoryDefinition>();

        public JRoot DefinitionRoot { get; set; } = new JRoot();
        public JForm DefinitionForm { get; set; }

        public Dictionary<string, DataTable> DataList { get; set; } = new Dictionary<string, DataTable>(StringComparer.OrdinalIgnoreCase);

        public List<IJControlDefinition> JControlList { get; set; } = new List<IJControlDefinition>();

        //private enum FontDpiMode {Dpi075, Dpi100, Dpi125, Dpi150};

        protected string AssemblyName { get; set; }
        protected string AssemblyFolder { get; set; }

        //pomoćna varijabla - npr. za SearchDoc
        public string Action { get; set; }

        public DataRow SearchDataRow { get; set; } //koristi se i za drvo i za grid; ide uz Action="searchdoc"

        private DataTable dt = new DataTable();

        public string LastDialogResult { get; set; } //potrebno za DialogResult; pošto se forma šalje u Conditions.CheckCondition
        public string LastTransactionSuccess { get; set; } = ""; // potrebno za transakciju da možemu u IF-u ispitati da li je prošla
        //private float fontScale = 1f; //to do

        public JupiterShared()
        {
            IsInitEvent = true;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                InitializeComponent();

                //string assemblyPath = this.GetType().Assembly.CodeBase;
                //AssemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
                //UriBuilder uri = new UriBuilder(assemblyPath);
                //string path = Uri.UnescapeDataString(uri.Path);
                //AssemblyFolder = Path.GetDirectoryName(path);
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                Logger.Error("JupiterShared", ex.Message);
            }
        }

        public void SetEvents(List<JEvent> events)
        {
            if (events == null || events.Count == 0)
                return;

            var onInit = events.Where(x => x.Type.Contains("init")).FirstOrDefault();
            if (onInit != null)
                LoadEvent(onInit);

            var onLoad = events.Where(x => x.Type.Contains("load")).FirstOrDefault();
            if (onLoad != null)
                OnLoad(onLoad);

            var onShown = events.Where(x => x.Type.Contains("shown")).FirstOrDefault();
            
            if (onShown != null)
                OnShown(onShown);

            var onClose = events.Where(x => x.Type.Contains("close")).FirstOrDefault();
            if (onClose != null)
                OnClose(onClose);

            var onKeyDown = events.Where(x => x.Type.Contains("keydown")).FirstOrDefault();
            if (onKeyDown != null)
                OnKeyDown(onKeyDown);
            if (DefinitionForm.DisposeOnClose)
            {
                this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JupiterShared_FormClosing);
            }
        }

        private void OnKeyDown(JEvent loadEvent)
        {
            this.KeyDown += (s, e) =>
            {
                OnKeyDownHelper(loadEvent, e);
            };
        }

        private void OnKeyDownHelper(JEvent loadEvent, KeyEventArgs e)
        {
            if (e != null)
            {
                KeyCode = e.KeyValue;
                Modifier = e.Modifiers;
            }

            LoadEvent(loadEvent);
        }

        private void OnLoad(JEvent loadEvent)
        {
            this.Load += (s, e) =>
            {
                LoadEvent(loadEvent);
            };
        }

        private void OnShown(JEvent loadEvent)
        {
            this.Shown += (s, e) =>
            {
                LoadEvent(loadEvent);
            };
        }

        private void LoadEvent(JEvent loadEvent)
        {
            using (Logic.Events ev = new Logic.Events(loadEvent, null, this))
            {
                ev.RunNotification();
            }
        }

        private void OnClose(JEvent loadEvent)
        {
            this.Closed += (s, e) =>
            {
                if (isF5clicked == false)
                    LoadEvent(loadEvent);
            };
        }

        #region FontDpiSize

        //private void SetDpiMode(FontDpiMode fontDpiMode)
        //{
        //    switch (fontDpiMode)
        //    {
        //        case FontDpiMode.Dpi075:
        //            fontScale = 0.75f;
        //            break;
        //        case FontDpiMode.Dpi125:
        //            fontScale = 1.25f;
        //            break;
        //        case FontDpiMode.Dpi150:
        //            fontScale = 1.5f;
        //            break;
        //        default:
        //            fontScale = 1f;
        //            break;
        //    }
        //}

        //protected void ChangedSize(FontDpiMode fontDpiMode)
        //{
        //    SetDpiMode(fontDpiMode);
        //    //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    foreach (Control control in this.Controls) {
        //        if (control is LayoutControl)
        //        {
        //            var layoutControl = (LayoutControl)control;
        //            Traverse(layoutControl.Items);
        //        }

        //    }
        //}
        //private void SetFontSize(LayoutControlItem layoutItem)
        //{
        //    layoutItem.AppearanceItemCaption.Font = new System.Drawing.Font(layoutItem.AppearanceItemCaption.Font.FontFamily, layoutItem.AppearanceItemCaption.Font.Size * fontScale);
        //}

        //public void Traverse(BaseItemCollection col)
        //{
        //    foreach (BaseLayoutItem item in col)
        //    {
        //        if (item is LayoutControlGroup) AssignGroup(item as LayoutControlGroup);
        //        if (item is LayoutControlItem)
        //            SetFontSize(item as LayoutControlItem);
        //    }
        //}

        //public void AssignGroup(LayoutControlGroup g)
        //{
        //    Traverse(g.Items);
        //}

        #endregion FontDpiSize

        public void SetControlValue(string controlName, object value)
        {
            foreach (var control in JControlList)
            {
                if (control is JRepositoryDefinition)
                {
                    if (((JRepositoryDefinition)control).CustomName == controlName)
                    {
                        ((JRepositoryDefinition)control).Value = value;
                    }
                }
            }
        }

        private void JupiterShared_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (!string.IsNullOrEmpty(DefinitionForm.CloseFormOnEscape) && DefinitionForm.CloseFormOnEscape.ToLower() == "false")
                    return;
                if (this.Owner != null)
                    DialogResult = DialogResult.Cancel;
                else
                {
                    this.Close();
                }
            }
            //else if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.F5)
            else if (e.KeyCode == Keys.F5)
            {
                isF5clicked = true;
                DialogResult = DialogResult.Retry;
            }
            else if (e.Control && e.KeyCode == Keys.F11)
            {
                using (Carpo.Core.Log.Log frm = new Carpo.Core.Log.Log(Logger))
                {
                    frm.FormVars = VariablesForm;
                    frm.GlobalVars = VariablesGlobal;
                    frm.ControlList = ControlList;
                    frm.InitVars = Variables.GetAllPubVars(this);
                    frm.Version = this.DefinitionRoot.Version;
                    frm.ActiveFormName = this.FormName;

                    if (this.Owner != null)
                    {
                        Forms.Base.JupiterShared frmOwner = (this.Owner as Forms.Base.JupiterShared);
                        frm.ParentFormVars = frmOwner.VariablesForm;
                        frm.ParentControlList = frmOwner.ControlList;
                        frm.ParentFormExists = true;
                    }
                    else
                        frm.ParentFormExists = false;

                    frm.ShowDialog(this);
                }
            }
            else if (e.Control && e.KeyCode == Keys.F12)
            {
                GC.Collect();
            }
            else if (e.Control && e.KeyCode == Keys.F1) // test
            {
                var j = FindFocussedControl(this.ActiveControl);
            }
            else if (e.KeyCode == Keys.F8)
            {
                Xml jRootXml = new Xml(AssemblyFolder, AssemblyName);
                jRootXml.ExportXml(DefinitionRoot);
            }

            if (this.DefinitionForm.Template != null && this.DefinitionForm.Template.ToLower().Contains("splitter"))
            {
                try
                {
                    var spliterForm = this as Forms.Edit.Splitter;
                    if (spliterForm != null && spliterForm.collapsedPanelCount > 0)
                    {
                        var collapsablePanel = spliterForm.splitter1.CollapsePanel;
                        if (Control.ModifierKeys == Keys.Control)
                        {
                            if (!this.DefinitionForm.Template.ToLower().Contains("vertical"))
                            {
                                if ((e.KeyCode == Keys.Left && collapsablePanel.ToString().ToLower() == "panel1") ||
                                    (e.KeyCode == Keys.Right && collapsablePanel.ToString().ToLower() == "panel2"))
                                    spliterForm.splitter1.Collapsed = true;
                                else if ((e.KeyCode == Keys.Left && collapsablePanel.ToString().ToLower() == "panel2") ||
                                    (e.KeyCode == Keys.Right && collapsablePanel.ToString().ToLower() == "panel1"))
                                    spliterForm.splitter1.Collapsed = false;
                            }
                            else
                            {
                                if ((e.KeyCode == Keys.Up && collapsablePanel.ToString().ToLower() == "panel1") ||
                                    (e.KeyCode == Keys.Down && collapsablePanel.ToString().ToLower() == "panel2"))
                                    spliterForm.splitter1.Collapsed = true;
                                else if ((e.KeyCode == Keys.Up && collapsablePanel.ToString().ToLower() == "panel2") ||
                                    (e.KeyCode == Keys.Down && collapsablePanel.ToString().ToLower() == "panel1"))
                                    spliterForm.splitter1.Collapsed = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Control FindFocussedControl(Control ctr)
        {
            ContainerControl container = ctr as ContainerControl;
            while ((container != null))
            {
                ctr = container.ActiveControl;
                container = ctr as ContainerControl;
            }
            return ctr;
        }

        //public void DeserializeXml()
        //{
        //    try
        //    {
        //        string xmlPath = "";

        //        if (!string.IsNullOrEmpty(TestXmlPath))
        //            xmlPath = TestXmlPath;
        //        else if (!string.IsNullOrEmpty(ModuleId))
        //            xmlPath = ModuleId;

        //        if (!File.Exists(xmlPath))
        //        {
        //            Logger.Warn("method: DeserializeXml", "Neispravna putanja: '" + xmlPath + "'");
        //            return;
        //        }

        //        Serializer parts = new Serializer(Logger, xmlPath);
        //        DefinitionRoot = parts.DeserializeXml();

        //        //var doc = XDocument.Load(xmlPath);

        //        //var listElements = new List<Structure.Models.Xml.ElementReplacing>();

        //        //if (doc.Elements("parts") != null)
        //        //{
        //        //    foreach (XElement formElement in doc.Descendants("form"))
        //        //    {
        //        //        foreach (XElement formPart in formElement.Descendants("part"))
        //        //        {
        //        //            var xmlPartPath = formPart.Attribute("path");
        //        //            XElement part = null;

        //        //            if (xmlPartPath != null)
        //        //            {
        //        //                string xmlFolder = Path.GetDirectoryName(xmlPath);
        //        //                string partPathBaza = Path.Combine(xmlFolder, "parts" + Carpo.Data.Connection.DataBase, xmlPartPath.Value.ToString());
        //        //                string partPath = Path.Combine(xmlFolder, "parts", xmlPartPath.Value.ToString());

        //        //                XDocument docPart;
        //        //                //prema prioritetu
        //        //                if (File.Exists(partPathBaza)) //partsFirma folder
        //        //                {
        //        //                    docPart = XDocument.Load(partPathBaza);
        //        //                    part = docPart.Elements("parts").Descendants("part").Where(x => x.Attribute("name").Value == formPart.Attribute("name").Value).FirstOrDefault();
        //        //                }
        //        //                else if (File.Exists(partPath)) //parts folder
        //        //                {
        //        //                    docPart = XDocument.Load(partPath);
        //        //                    part = docPart.Elements("parts").Descendants("part").Where(x => x.Attribute("name").Value == formPart.Attribute("name").Value).FirstOrDefault();
        //        //                }
        //        //            }

        //        //            if (part == null)
        //        //            {
        //        //                //provjeri u trenutnoj datoteci
        //        //                part = doc.Elements("root").Elements("parts").Descendants("part").Where(x => x.Attribute("name").Value == formPart.Attribute("name").Value).FirstOrDefault();
        //        //            }

        //        //            if (part != null)
        //        //                listElements.Add(new Structure.Models.Xml.ElementReplacing() { FormPart = formPart, Part = part });

        //        //        }
        //        //    }
        //        //}

        //        //foreach (Structure.Models.Xml.ElementReplacing element in listElements)
        //        //{
        //        //    element.FormPart.ReplaceWith(element.Part.Elements());
        //        //}

        //        //if (!System.IO.File.Exists(xmlPath))
        //        //{
        //        //    Logger.Warn("Nepostojeća datoteka: '" + xmlPath + "'");
        //        //    return;
        //        //}
        //        //var serializer = new XmlSerializer(typeof(JRoot));
        //        //using (var reader = doc.Root.CreateReader())
        //        //{
        //        //    var jRoot = (JRoot)serializer.Deserialize(reader);

        //        //    DefinitionRoot = jRoot;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("method: DeserializeXml", ex.Message);
        //    }
        //}

        protected override void OnLoad(EventArgs e)
        {
            bbiVerzija.Caption = "v. " + this.AssamblyVersion;
            bbiVerzija.Hint = this.AssamblyVersionToolTip;

            base.OnLoad(e);

            if (!DesignMode)
            {
                SetCarpoVars();

                IsInitEvent = false;
                IsLoadEvent = true;

                if (!(this is Forms.Master.Manager))
                {
                    Events events = new Events(this);
                    events.CreateEvents(EventList);
                }
            }
        }

        //private void JupiterShared_Load(object sender, EventArgs e)
        //{
        //    if (!DesignMode)
        //    {
        //        SetCarpoVars();
        //        IsInitEvent = false;
        //        IsLoadEvent = true;

        //        if (!(this is Forms.Master.Manager))
        //        {
        //            Events events = new Events(this);
        //            events.CreateEvents(EventList);
        //        }
        //    }
        //}

        private void JupiterShared_Shown(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                IsLoadEvent = false;
                if (this is Forms.Master.Manager)
                {
                    Events events = new Events(this);
                    events.CreateEvents(EventList);
                }

                Cursor.Current = Cursors.Default;
            }

            SetCaption();
        }

        private void SetCarpoVars()
        {
            try
            {
                Logic.VarsLayout carpoVars = new VarsLayout(this);
                carpoVars.SetLayout();
                SetLayout();
            }
            catch (Exception ex)
            {
                Logger.Error("event: JupiterShared.SetCarpoVars", ex.Message, ex);
            }
        }

        private void SetLayout()
        {
            var splitters = CarpoVarsLayout.Where(x => x.Name.ToLower().StartsWith("split"));
            if (splitters != null)
            {
                Logic.VarsLayout carpoVars = new VarsLayout(this);
                carpoVars.SetSplitters(splitters.ToList());
            }
        }

        public void SetMethods(Forms.Base.JupiterShared form)
        {
            var methods = DefinitionForm.CustomMethod;
            if (methods != null && methods.Count > 0)
            {
                foreach (var m in methods)
                {
                    CustomMethods.Add(new Components.JCustomMethod(m, form));
                }
            }
        }

        private void SetCaption()
        {
            var startTime = DateTime.Now;
            string captionStr = "";
            string versionStr = "";

            if (DefinitionForm != null && !string.IsNullOrEmpty(DefinitionForm.Caption))
            {
                using (Parser parser = new Parser(this, null))
                {
                    object caption = parser.GetLogicPartValue(DefinitionForm.Caption.Trim());
                    if (caption != null)
                    {
                        captionStr = Common.Translator.TranslateString(caption.ToString());
                    }
                    else
                        captionStr = DefinitionForm.Caption;

                    if (!string.IsNullOrEmpty(DefinitionForm.Version))
                    {
                        object version = parser.GetLogicPartValue(DefinitionForm.Version.Trim());
                        if (version != null)
                        {
                            versionStr = version.ToString();
                        }
                        else
                            versionStr = DefinitionForm.Version;
                    }

                    if (DefinitionForm.ShowVersion == true && !string.IsNullOrEmpty(DefinitionForm.Version))
                        this.Text = captionStr + " v" + versionStr;
                    else
                        this.Text = captionStr;
                }
            }
            ApplyCaptionAppearance();
            if (Logger != null)
                Logger.InfoTest("SetCaption", "Postavljanje naziva forme", startTime);
        }

        // Vlastiti controller samo za forme koje u XML-u traže drugačiji izgled naslova;
        // ostale forme ostaju na globalnom default controlleru (ribbonControl1.Controller == null).
        private BarAndDockingController captionController;

        /// <summary>
        /// Opt-in atributi na &lt;form&gt;: captionfontsizedelta, captionfontbold, captionforecolor.
        /// Ako nijedan nije postavljen, ništa se ne mijenja (default izgled naslova iz skina).
        /// </summary>
        private void ApplyCaptionAppearance()
        {
            if (DefinitionForm == null)
                return;

            bool hasDelta = DefinitionForm.CaptionFontSizeDelta.HasValue;
            bool hasBold = DefinitionForm.CaptionFontBold == true;
            bool hasColor = !string.IsNullOrWhiteSpace(DefinitionForm.CaptionForeColor);

            if (!hasDelta && !hasBold && !hasColor)
                return;

            try
            {
                if (ribbonControl1 == null)
                    return;

                if (captionController == null)
                {
                    captionController = ribbonControl1.Controller ?? new BarAndDockingController();
                    ribbonControl1.Controller = captionController;
                }

                var formCaption = captionController.AppearancesRibbon.FormCaption;

                if (hasDelta)
                    formCaption.FontSizeDelta = DefinitionForm.CaptionFontSizeDelta.Value;

                if (hasBold)
                    formCaption.FontStyleDelta = FontStyle.Bold;

                if (hasColor)
                {
                    Color color = Common.Helpers.Colors.HtmlToColor(DefinitionForm.CaptionForeColor.Trim(), Color.Empty);
                    if (color == Color.Empty)
                    {
                        Logger.Warn("JupiterShared.ApplyCaptionAppearance",
                            "Nevaljana vrijednost captionforecolor='" + DefinitionForm.CaptionForeColor + "' – boja naslova nije promijenjena.");
                    }
                    else
                    {
                        formCaption.ForeColor = color;
                        captionController.AppearancesRibbon.FormCaptionForeColorInactive = color;
                    }
                }

                ribbonControl1.Refresh();
            }
            catch (Exception ex)
            {
                Logger.Warn("JupiterShared.ApplyCaptionAppearance", ex.Message);
            }
        }

        private void JupiterShared_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captionController != null)
            {
                try { captionController.Dispose(); } catch { }
                captionController = null;
            }
            Dispose();
            // Do not force GC.Collect() on every form close — stalls UI and fights the GC.
        }
    }
}
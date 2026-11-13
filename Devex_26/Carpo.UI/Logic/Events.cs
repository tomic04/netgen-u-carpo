using Carpo.Common.Helpers;
using Carpo.Core;
using Carpo.Structure.Models;
using Carpo.Structure.Models.Core;
using Carpo.UI.Components;
using Carpo.UI.Components.Container;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Carpo.UI.Logic
{
    public class Events : IDisposable
    {
        private enum WaitType { None, WaitCursor, WaitProgress }

        // dodao Ognjen - za potrebe wait-a na event i action(dodatno)
        private WaitType waitType = WaitType.None;

        private string waitText = "";
        private string waitCaption = "";
        private int? sleep = null;
        private Structure.Models.ConnWithTran connTranModel;
        private List<string> transactionVariables = new List<string>();

        //za potrebe print actiona;
        //čuvam string, preuzimam unutar INotifyPrint-a (JGridControl --> ComponentGrida)
        public Structure.Models.Print PrintDefinition { get; set; }

        private enum AllowPermission { None, Allow, Disallow };

        private JEvent jEvent;

        //private List<JNotifs> notifications = new List<JNotification>();
        private Control thisControl;

        private Forms.Base.JupiterShared form;
        private string connectionString;
        private bool stopActions = false;
        private object thisControlValue;

        private Dictionary<string, object> parentFormParameters = new Dictionary<string, object>();
        private bool closingForm = false;

        private string lastDialogResult;

        public Events(JEvent jEvent, Control thisControl, Forms.Base.JupiterShared form, object thisControlValue)
        {
            this.jEvent = jEvent;
            this.thisControl = thisControl;
            this.form = form;
            this.connectionString = form.ConnectionString;
            this.thisControlValue = thisControlValue;
        }

        public Events(JEvent jEvent, Control thisControl, Forms.Base.JupiterShared form)
        {
            this.jEvent = jEvent;
            this.thisControl = thisControl;
            this.form = form;
            this.connectionString = form.ConnectionString;
            this.form.XmlPath = form.XmlPath;
        }

        //direktni poziv akcija (grid - foreachrow)
        public Events(Control thisControl, Forms.Base.JupiterShared form)
        {
            this.thisControl = thisControl;
            this.form = form;
            this.connectionString = form.ConnectionString;
        }

        public Events(JEvent jEvent, Forms.Base.JupiterShared form)
        {
            this.jEvent = jEvent;
            this.form = form;
            this.connectionString = form.ConnectionString;
        }

        public Events(Forms.Base.JupiterShared form)
        {
            this.form = form;
            this.connectionString = form.ConnectionString;
        }

        #region LayoutNotifications

        public void CreateEvents(List<Structure.Models.ControlEvents> eventList)
        {
            var startTime = DateTime.Now;
            foreach (Structure.Models.ControlEvents e in eventList)
            {
                if (e.Events != null && e.Events.Count > 0)
                    SetNotification(e);
            }
            var log = Carpo.Core.Log.DependencyContainer.Get<Logger>();
            if (log != null)
                log.InfoTest("CreateEvents", "dodavanje eventova na formu", startTime);

        }

        private void SetNotification(Structure.Models.ControlEvents e)
        {
            form.Logger.Info("Control.Name" + e.Control.Name, "SetNotification");
            try
            {
                if (e.Control is Structure.Interfaces.IJControl)
                    (e.Control as Structure.Interfaces.IJControl).SetEvents(e.Events);
            }
            catch (Exception ex)
            {
                form.Logger.Error("method: SetNotification; control name: " + e.Control.Name, ex.Message, ex);
            }
        }

        #endregion LayoutNotifications

        public void RunNotification()
        {
            try
            {
                if (jEvent != null)
                {
                    WaitFormRun(jEvent);

                    if (thisControl is JXtraTabControl)
                    {
                        if ((thisControl as JXtraTabControl).RunActions == false)
                            return;

                    }

                    RunActions(jEvent.Actions);
                }
            }
            catch (ThreadAbortException ex)
            {

            }
            catch (Exception ex)
            {
                form.Logger.Error("method: RunNotification", ex.Message, ex);
                if (ex.InnerException != null)
                {
                    form.Logger.Error("method: RunNotification", ex.InnerException.Message);
                }
            }
            finally
            {
                WaitFormClose(jEvent);
            }
        }

        private void WaitFormRun(JNotifications notif)
        {
            if (notif == null || string.IsNullOrEmpty(notif.WaitType))
                return;

            waitType = WaitType.None;
            waitText = notif.WaitText;
            waitCaption = notif.WaitCaption;
            sleep = notif.Sleep;

            if (!string.IsNullOrEmpty(notif.WaitType))
            {
                if (notif.WaitType == "progress")
                    waitType = WaitType.WaitProgress;
                else if (notif.WaitType == "cursor")
                    waitType = WaitType.WaitCursor;
            }

            if (waitType == WaitType.WaitCursor)
                form.Cursor = Cursors.WaitCursor;
            else if (waitType == WaitType.WaitProgress)
            {
                (form as Forms.Base.JupiterShared).splashScreenManager1.ShowWaitForm();
                if (!string.IsNullOrEmpty(waitText))
                    (form as Forms.Base.JupiterShared).splashScreenManager1.SetWaitFormDescription(waitText);
                if (!string.IsNullOrEmpty(waitCaption))
                    (form as Forms.Base.JupiterShared).splashScreenManager1.SetWaitFormCaption(waitCaption);

                if (sleep != null)
                    System.Threading.Thread.Sleep((int)sleep);
            }
        }

        private void WaitFormClose(JNotifications notif)
        {
            if (notif == null || string.IsNullOrEmpty(notif.WaitType))
                return;

            if (waitType != WaitType.None)
            {
                if (waitType == WaitType.WaitCursor)
                    form.Cursor = Cursors.Default;
                else if (waitType == WaitType.WaitProgress)
                {
                    if ((form as Forms.Base.JupiterShared).splashScreenManager1.IsSplashFormVisible == true)
                        (form as Forms.Base.JupiterShared).splashScreenManager1.CloseWaitForm();
                }
            }
        }

        public object RunActions(JEvent jEvent, string searchType)
        {
            if (jEvent == null || jEvent.Actions == null)
                return null;

            bool? defaultValue = null;

            IEnumerable<JEventAction> actionList = null;
            if (jEvent.Type.StartsWith("dragnode"))
                actionList = jEvent.Actions.Where(x => x.Type.StartsWith("drag"));
            else if (jEvent.Type.StartsWith("drag"))
                actionList = jEvent.Actions.Where(x => x.Type.StartsWith("drop"));
            else if (jEvent.Type.StartsWith("showing"))
                actionList = jEvent.Actions.Where(x => x.Type.StartsWith("editor"));

            if (actionList == null || actionList.Count() == 0)
                return defaultValue;

            defaultValue = false;

            try
            {
                bool checkIfResult = true;

                foreach (JEventAction action in actionList)
                {
                    try
                    {
                        if (Conditions.CheckCondition(ref checkIfResult, action, form, thisControl) == false)
                            continue;

                        return checkIfResult;
                        //if (searchType == "drop")
                        return CheckAllow(action, defaultValue);
                        //else if (searchType == "editor")
                        //    return CheckAllow(action, defaultValue);
                    }
                    catch { throw; }
                }
                return defaultValue;
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: RunActions - SearchType=" + searchType, ex.Message, ex);
                return defaultValue;
            }
        }
        public void RunActions(List<JEventAction> actionList)
        {
            bool checkIfResult = true;
            stopActions = false;
            var sw = Stopwatch.StartNew();
            string firstType = actionList != null && actionList.Count > 0 ? actionList[0].Type : null;
            try
            {
            // Wave 8: Outlook / ZKI / JIR extracted — same semantics, thinner Events.
            var fiscalOutlook = new FiscalOutlookActions(form, connectionString, thisControl);
            foreach (JEventAction action in actionList)
            {
                try
                {
                    var fiscalResult = fiscalOutlook.TryHandle(action);
                    if (fiscalResult == FiscalOutlookActions.HandleResult.Stop)
                        return;
                    if (fiscalResult == FiscalOutlookActions.HandleResult.Continue)
                        continue;

                    WaitFormRun(action);

                    if (stopActions == true)
                        return;

                    if (Conditions.CheckCondition(ref checkIfResult, action, form, thisControl) == false)
                    {
                        if (action.Stop == true)
                            return;
                        continue;
                    }

                    if (action.Type != null)
                    {
                        if (action.Type.Replace(" ", String.Empty).ToLower() == "textexport")
                        {
                            ExportTextToFileSystem(action);
                            continue;
                        }

                        if (thisControl is JXtraTabControl && action.Type.ToLower() == "cancel")
                        {
                            (thisControl as JXtraTabControl).ComponentExt.PreventTabChanging = true;
                            return;
                        }
                    }

                    if (CheckConfirm(action) == false)
                    {
                        if (action.StopOnConfirm == false)
                            continue;
                        else
                            return;
                    }

                    if (CheckValidation(action) == false)
                        return;

                    Checking(action);

                    bool isForEach = false;
                    if (action.Type != null && action.Type.ToLower().Contains("foreach"))
                        isForEach = true;

                    bool isTransaction = false;
                    if (action.Type != null && action.Type.ToLower().Contains("transaction"))
                        isTransaction = true;

                    // da ne odrađuje akcije unutar transakcije (datarun-ove odrađujemo kroz CheckTransaction -> CheckData)
                    if (action.Actions != null && action.Actions.Count > 0 && isForEach == false && isTransaction == false)
                        RunActions(action.Actions);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    WaitFormClose(action);
                }
            }

            if (form != null)
            {
                form.LastDialogResult = string.Empty;
                form.LastTransactionSuccess = string.Empty;
            }
            }
            finally
            {
                try
                {
                    form?.Logger?.Performance("Events",
                        $"RunActions type={firstType} count={(actionList == null ? 0 : actionList.Count)}", sw, "Action");
                }
                catch { }
            }
        }

        private void ExportTextToFileSystem(JEventAction action)
        {
            var filepath = action.Parameter.Where(x => x.Name.ToLower().Replace(" ", string.Empty) == "filepath").FirstOrDefault();
            var filename = action.Parameter.Where(x => x.Name.ToLower().Replace(" ", string.Empty) == "filename").FirstOrDefault();
            var extension = action.Parameter.Where(x => x.Name.ToLower().Replace(" ", string.Empty) == "extension").FirstOrDefault();
            var textcontent = action.Parameter.Where(x => x.Name.ToLower().Replace(" ", string.Empty) == "textcontent").FirstOrDefault();
            var showOpenFileDialog = action.Parameter.Where(x => x.Name.ToLower().Replace(" ", string.Empty) == "showopenfiledialog").FirstOrDefault();
            // opcionalno: encoding datoteke (npr. windows-1250 za ANSI latinicu); bez parametra ostaje UTF-8 bez BOM-a
            var encoding = action.Parameter.Where(x => x.Name.ToLower().Replace(" ", string.Empty) == "encoding").FirstOrDefault();

            string filePathStr = "";
            string filenameStr = "";
            string extensionStr = "";
            string textcontentStr = "";
            string showOpenFileDialogStr = "";
            string encodingStr = null;

            using (Parser p = new Parser(form, thisControl))
            {
                if (filepath != null)
                    filePathStr = (string)p.GetLogicPartValue(filepath.Value);
                else
                    form.Logger.Error("Events.RunActions()", "Akciji 'textExport' nedostaje parametar 'filePath'");

                if (filename != null)
                    filenameStr = (string)p.GetLogicPartValue(filename.Value);
                else
                    form.Logger.Error("Events.RunActions()", "Akciji 'textExport' nedostaje parametar 'fileName'");

                if (extension != null)
                    extensionStr = (string)p.GetLogicPartValue(extension.Value);
                else
                    form.Logger.Error("Events.RunActions()", "Akciji 'textExport' nedostaje parametar 'extension'");

                if (textcontent != null)
                    textcontentStr = (string)p.GetLogicPartValue(textcontent.Value);
                else
                    form.Logger.Error("Events.RunActions()", "Akciji 'textExport' nedostaje parametar 'textContent'");

                if (showOpenFileDialog != null)
                    showOpenFileDialogStr = showOpenFileDialog.Value;

                if (encoding != null && !string.IsNullOrWhiteSpace(encoding.Value))
                {
                    string encVal = encoding.Value.Trim();
                    // dozvoljeno i kao literal ('windows-1250') i kao referenca na kontrolu/varijablu (#cmbEncoding, Global.$Enc)
                    if (encVal.StartsWith("#") || encVal.StartsWith("~") || encVal.StartsWith("Global.") || encVal.StartsWith("$") || encVal.StartsWith("'"))
                        encodingStr = p.GetLogicPartValue(encVal) as string;
                    else
                        encodingStr = encVal;
                }

                if (filepath != null && filename != null && extension != null && textcontent != null)
                {
                    using (var export = new Export())
                    {
                        string fullPath = Path.Combine(filePathStr, filenameStr) + "." + extensionStr;
                        export.ExportTxt(fullPath, textcontentStr, showOpenFileDialogStr == "true" ? true : false, encodingStr);
                    }
                }
                else
                    form.Logger.Error("Events.RunActions()", "Akcija 'textExport' nije pokrenuta. Provjerite jeste li dobro upisali sve parametre.");
            }
        }

        private void Checking(JEventAction action)
        {
            if (action.Type == null)
                return;

            if (CheckExit(action) == true)
                return;
            if (CheckMethod(action) == true)
                return;
            else if (CheckProcess(action) == true)
                return;
            else if (CheckPrint(action) == true)
                return;
            else if (CheckControls(action) == true)
                return;
            else if (CheckMenu(action) == true)
                return;
            else if (CheckVariable(action) == true)
                return;
            else if (CheckData(action) == true)
                return;
            else if (CheckForm(action) == true)
                return;
            else if (CheckMsgBox(action) == true)
                return;
            else if (CheckComDll(action) == true)
                return;
            else if (CheckFooter(action) == true)
                return;
            else if (CheckTabs(action) == true)
                return;
            else if (CheckSearch(action) == true)
                return;
            else if (CheckPanel(action) == true)
                return;
            else if (CheckTransaction(action) == true)
                return;
            else if (CheckSetLayout(action) == true)
                return;
            else if (CheckGenerate(action) == true)
                return;
            else if (CheckInfoControls(action) == true)
                return;
        }

        #region ActionChecker

        //private bool? CheckAllowDrop(JEventAction action, bool? defaultValue)
        //{
        //    if (action.Type == null || !action.Type.ToLower().StartsWith("drop"))
        //        return null;

        //    AllowPermission dropPermission;
        //    if (action.Type.Contains("disallow"))
        //        dropPermission = AllowPermission.Disallow;
        //    else if (action.Type.Contains("allow"))
        //        dropPermission = AllowPermission.Allow;
        //    else
        //        return defaultValue;

        //    Parser parser = new Parser(form, null);
        //    object value = parser.GetLogicPartValue(action.Value);

        //    object valueObj = Common.Helpers.Conversion.Parse("bool", value);
        //    if (valueObj != null)
        //    {
        //        bool valueBool = (bool)valueObj;
        //        if (valueBool == true)
        //            return dropPermission == AllowPermission.Allow;
        //        else
        //            return dropPermission == AllowPermission.Disallow;
        //    }

        //    return defaultValue;
        //}

        private bool? CheckAllow(JEventAction action, bool? defaultValue)
        {
            if (action.Type == null)
                return null;
            else if
                (!action.Type.ToLower().StartsWith("drop") &&
                !action.Type.ToLower().StartsWith("drag") &&
                !action.Type.ToLower().StartsWith("editor"))
                return null;

            AllowPermission permission = AllowPermission.None;

            if (!string.IsNullOrEmpty(action.Value))
            {
                if (action.Value.ToLower() == "false")
                    permission = AllowPermission.Disallow;
                else if (action.Value.ToLower() == "true")
                    permission = AllowPermission.Allow;
            }

            if (permission == AllowPermission.None)
                return defaultValue;

            Parser parser = new Parser(form, null);
            object value = parser.GetLogicPartValue(action.Value);

            object valueObj = Common.Helpers.Conversion.Parse("bool", value);
            if (valueObj != null)
            {
                bool valueBool = (bool)valueObj;
                if (valueBool == true)
                    return permission == AllowPermission.Allow;
                else
                    return permission == AllowPermission.Disallow;
            }

            return defaultValue;
        }

        private bool? CheckConfirm(JEventAction action)
        {
            string valueStr = "", buttonYesValueStr = "", buttonNoValueStr = "";

            if (action.Type == null || !action.Type.ToLower().StartsWith("confirm"))
                return null;

            object buttonYes = null, buttonNo = null;

            using (Parser parser = new Parser(form, null))
            {
                object value = parser.GetLogicPartValue(action.Value);
                if (value != null)
                    valueStr = value.ToString();
                else
                {
                    form.Logger.Warn("method: CheckConfirm", "Neuspješno parsiranje vrijednosti; " + action.Value.ToString());
                    valueStr = action.Value.ToString();
                }

                foreach (JEventActionParameter p in action.Parameter)
                {
                    if (p.Name.ToLower() == "captionyes")
                    {
                        buttonYes = parser.GetLogicPartValue(p.Value);
                        if (buttonYes != null)
                            buttonYesValueStr = buttonYes.ToString();
                        else
                        {
                            buttonYesValueStr = p.Value.ToString();
                            form.Logger.Warn("method: CheckConfirm", "Neuspješno parsiranje vrijednosti; " + p.Value.ToString());
                        }
                    }
                    else if (p.Name.ToLower() == "captionno")
                    {
                        buttonNo = parser.GetLogicPartValue(p.Value);
                        if (buttonNo != null)
                            buttonNoValueStr = buttonNo.ToString();
                        else
                        {
                            buttonNoValueStr = p.Value.ToString();
                            form.Logger.Warn("method: CheckConfirm", "Neuspješno parsiranje vrijednosti; " + p.Value.ToString());
                        }
                    }
                }
            }

            if (Common.Helpers.JConfirm.Confirm(valueStr, "", buttonYesValueStr, buttonNoValueStr) == DialogResult.No)
                return false;

            return true;
        }

        //private bool? CheckOperaterLock()
        //{
        //    //if (form.op)
        //}

        private bool? CheckPrint(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("print"))
                return null;

            try
            {
                Parser parser = new Parser(form, null);
                object printTemplate = parser.GetLogicPartValue(action.Value);

                if (printTemplate != null && printTemplate.ToString().StartsWith("repx"))
                {
                    string repx = printTemplate.ToString().Replace("repx:", "").Replace("repx/", "");

                    string parameters = "";
                    //string s = Logic.Controls.FindControl(form, action.Parameter[0].Value);
                    if (action.Parameter != null)
                    {
                        parameters = Logic.Print.GetVariables(form, action.Parameter);
                        form.Logger.InfoData("Parametri repx štampe (" + repx + ")", parameters);

                        PrintDefinition = new Structure.Models.Print() { Template = repx, Parameters = parameters };
                    }

                    //ako je type samo "print" -> slijedi direktan print, bez poveznice s kontrolom
                    if (action.Type.ToLower() == "print")
                    {
                        List<Structure.Models.Print> printList = new List<Structure.Models.Print>();
                        printList.Add(PrintDefinition);
                        Logic.PrintDialog printDialog = new Logic.PrintDialog(form, printList);
                    }
                }
                else
                    form.Logger.Warn("event: CheckPrint", "Neuspješan poziv repx štampe. Očekivan naziv 'repx...'");

                return true;
            }
            catch { throw; }
        }

        private bool? CheckPanel(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("panel"))
                return null;

            try
            {
                Parser parser = new Parser(form, null);
                object value = parser.GetLogicPartValue(action.Value);
                if (value == null)
                    return false;

                if (form is Forms.Edit.Splitter || form is Forms.Master.Manager)
                {
                    SplitContainerControl splitControl = GetSplitterControl(form);
                    if (splitControl == null)
                        return null;

                    if (action.Type.ToLower().Contains("visible"))
                    {
                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            switch (value.ToString().ToLower())
                            {
                                case "panel1":
                                case "panelmaster":
                                    splitControl.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
                                    break;

                                case "panel2":
                                case "paneldetail":
                                    splitControl.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;
                                    break;

                                case "all":
                                    splitControl.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both;
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                            form.Logger.Warn("event: CheckPanel", "Vrijednost akcije nije definirana; " + action.Value.ToString(), "");
                    }

                    if (action.Type.ToLower().Contains("collapse"))
                    {
                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            switch (value.ToString().ToLower())
                            {
                                case "panel1":
                                case "panelMaster":
                                    if (splitControl.CollapsePanel == DevExpress.XtraEditors.SplitCollapsePanel.Panel1)
                                        splitControl.Collapsed = true;
                                    break;

                                case "panel2":
                                case "panelDetail":
                                    if (splitControl.CollapsePanel == DevExpress.XtraEditors.SplitCollapsePanel.Panel2)
                                        splitControl.Collapsed = true;
                                    break;

                                case "none":
                                    splitControl.Collapsed = false;
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                            form.Logger.Warn("event: CheckPanel", "Vrijednost akcije nije definiran; " + action.Value.ToString(), "");
                    }
                }
                else
                    form.Logger.Warn("event: CheckPanel", "Atribut 'template' nije (dobro) definiran!");
                return true;
            }
            catch { throw; }
        }

        private SplitContainerControl GetSplitterControl(Forms.Base.JupiterShared form)
        {
            if (form is Forms.Edit.Splitter)
                return (form as Forms.Edit.Splitter).splitter1;
            else if (form is Forms.Master.Manager)
                return (form as Forms.Master.Manager).splitGrid;

            return null;
        }

        private bool? CheckMsgBox(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("message"))
                return null;

            try
            {
                Parser parser = new Parser(form, null);
                object value = parser.GetLogicPartValue(action.Value);
                if (value != null)
                    Common.Helpers.JMessages.Info(value.ToString().Replace("\\n", Environment.NewLine));
                else
                    form.Logger.Warn("event: CheckMsgBox", "Neuspješno parsiranje vrijednosti ili je vrijednost null; " + action.Value.ToString(), "");

                return true;
            }
            catch { throw; }
        }

        private bool? CheckValidation(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("validate"))
                return null;
            try
            {
                if (form is Forms.Base.JupiterEditor)
                {
                    bool isValid = true;
                    isValid = (form as Forms.Base.JupiterEditor).Validation();
                    var doValidateControls = form.ControlList.Where(x => x.DoValidate == true);
                    if (doValidateControls != null && doValidateControls.Count() > 0)
                    {
                        foreach (var ctrl in doValidateControls)
                        {
                            if (ctrl.Control is JTextEditControl)
                            {
                                if ((ctrl.Control as JTextEditControl).DoValidate() == false)
                                    isValid = false;
                            }
                        }
                    }
                    if (isValid == false)
                    {
                        if (form is Forms.Base.JupiterEditor)
                        {
                            Control control = (form as Forms.Base.JupiterEditor).dxValidationProvider1.GetInvalidControls().OfType<Control>().OrderBy(c => c.TabIndex).FirstOrDefault();
                            if (control != null)
                                control.Focus();
                        }

                        return false;
                    }
                }
                else
                    form.Logger.Warn("event: CheckValidation", "Forma ne omogućuje validaciju. Dostupno samo na 'Standard' formama.");

                return true;
            }
            catch { throw; }
        }

        private bool CheckControls(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("control"))
                return false;

            try
            {
                if (action.Value != null)
                {
                    string[] controlsStr = action.Value.Split(new string[] { "," }, StringSplitOptions.None);
                    if (controlsStr.Count() > 0)
                    {
                        if (controlsStr.Contains("##"))
                        {
                            List<string> excludedControls = new List<string>();
                            foreach (string controlStr in controlsStr)
                            {
                                //! = not - u ovom slučaju - exclude control
                                if (controlStr.StartsWith("!#"))
                                {
                                    excludedControls.Add(controlStr.Substring(2).ToLower());
                                }
                            }
                            //Notificiraj sve kontrole
                            NotifyAllControls(action, excludedControls);
                            //setiraj listu na null - radi disposea;

                            excludedControls = null;
                            return true;
                        }

                        foreach (string controlStr in controlsStr)
                        {
                            string controlStrTrimed = controlStr.Trim();
                            form.Logger.Info("method: CheckControls", "Action.Value=" + controlStrTrimed);
                            Control[] control = Controls.FindControl(FormsVar.GetSearchForm(form, controlStrTrimed), Controls.GetSearchConotrolStr(controlStrTrimed, form));
                            if (control != null && control.Count() > 0)
                            {
                                NotifyControl(control[0], action, thisControlValue);
                            }
                            // ovo mora ovdje pošto BBI nije kontrola i ne možemo ispitati kontrolu gore u FindControl
                            else if (form is Carpo.UI.Forms.Master.Manager &&
                                (action.Value).Replace("#", "") == (form as Carpo.UI.Forms.Master.Manager).bbiLookUpFilter.Edit.Name)
                            {
                                if (action.Type.ToLower() == "control value" && action.Parameter.Count > 0)
                                {
                                    using (Parser parser = new Parser(form, null))
                                    {
                                        object value = parser.GetLogicPartValue(action.Parameter[0].Value);
                                        if (value != null)
                                        {
                                            (form as Carpo.UI.Forms.Master.Manager).bbiLookUpFilter.EditValue = value.ToString();
                                        }
                                    }
                                }
                                else if (action.Type.ToLower() == "control populate")
                                {
                                    (form as Carpo.UI.Forms.Master.Manager).RePopulateTreeFilter();
                                }
                            }
                            else
                            {
                                form.Logger.Warn("method: CheckControls", "Kontrola '" + controlStr + "' nije pronađena!");
                            }
                        }
                    }
                }
                else if (action.Type.ToLower().Contains("value"))
                {
                    ActionValue actionValue = new ActionValue();
                    actionValue.Process(ActionValue.ActionValueType.Control, form, action);
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool? CheckMenu(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("menu"))
                return null;

            if (!(form.BarButtonItems.Count + form.MenuItems.Count > 0))
                return CheckMenuInvalid(action.Value);

            try
            {
                if (action.Value != null)
                {
                    string[] menusStr = action.Value.Split(new string[] { "," }, StringSplitOptions.None);
                    if (menusStr.Count() > 0)
                    {
                        foreach (string menuStr in menusStr)
                        {
                            string menuTrimed = menuStr.Trim().Replace("#", "");
                            form.Logger.Info("method: CheckMenu", "Action.Value=" + menuTrimed);

                            var menu = form.BarButtonItems.Where(x => x.Name == menuTrimed).FirstOrDefault();
                            if (menu != null)
                                NotifyMenu(menu, action);
                            else
                            {
                                var menu2 = form.MenuItems.Where(x => x.Name == menuTrimed).FirstOrDefault();
                                if (menu2 != null)
                                    NotifyMenu(menu2, action);
                                else
                                    CheckMenuInvalid(menuTrimed);
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckMenuInvalid(string name)
        {
            form.Logger.Warn("method: CheckMenu", "Meni '" + name + "' nije pronađen!");
            return false;
        }

        private void NotifyAllControls(JEventAction action, List<string> excludedControls)
        {
            foreach (JControl item in form.ControlList)
            {
                if (!excludedControls.Contains(item.Control.Name.ToLower()))
                    NotifyControl(item.Control, action, null);
            }
        }

        private bool? CheckFooter(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("footer"))
                return null;

            try
            {
                Parser parser = new Parser(form, null);
                object value = parser.GetLogicPartValue(action.Value);

                if (action.Type.ToLower().Contains("text"))
                {
                    Forms.Base.JupiterShared editor = form as Forms.Base.JupiterShared;
                    if (value == null)
                    {
                        form.Logger.Warn("event: CheckFooter", "Neuspješno parsiranje vrijednosti ili je vrijednost null; " + action.Value.ToString(), "");
                        return true;
                    }

                    if (value.ToString().Trim() == "")
                    {
                        editor.bbiLabel.Caption = "";
                        editor.bbiLabel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }
                    else
                    {
                        editor.bbiLabel.Caption = value.ToString();
                        editor.bbiLabel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                }

                return true; // ne znam šta ću s ovim, podvalio sam jer mi je javljao error da ne dobije povratnu vrijednost uvijek ... definitivno ne može ovako ostati
            }
            catch { throw; }
        }

        private bool? CheckTabs(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("tab"))
                return null;

            try
            {
                Parser parser = new Parser(form, null);
                object value = parser.GetLogicPartValue(action.Value);

                if (action.Type.ToLower().Contains("focus"))
                {
                    if (form as Forms.Base.JupiterEditor != null)
                    {
                        foreach (var layoutControl in (form as Forms.Base.JupiterEditor).LayoutControlList)
                        {
                            var tab = layoutControl.Items.FindByName(value.ToString());

                            if (tab != null)
                            {
                                if (tab is LayoutControlGroup)
                                {
                                    if ((tab as LayoutControlGroup).ParentTabbedGroup is TabbedControlGroup)
                                    {
                                        (tab as LayoutControlGroup).ParentTabbedGroup.SelectedTabPage = (tab as LayoutControlGroup);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                form.Logger.Warn("tab focus", "Tab '" + value.ToString() + "' ne postoji!");
                                return null;
                            }
                        }
                    }
                    else if (form as Forms.Base.JupiterManager != null)
                    {
                        var frmVar = form as Carpo.UI.Forms.Master.Manager;
                        var tabList = frmVar.JSettings.TabsDetail.TabList;

                        for (int i = 0; i < tabList.Count; i++)
                        {
                            if (tabList[i].Name == value.ToString())
                            {
                                frmVar.tabControl.SelectedTabPageIndex = i;
                                return true;
                            }
                        }

                        form.Logger.Warn("tab focus", "Tab '" + value.ToString() + "' ne postoji!");
                        return null;
                    }
                    else
                        return null;
                }

                return true;
            }
            catch { throw; }
        }

        private bool? CheckSearch(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("search"))
                return null;

            //za sada search ide samo preko JButtonKontrole (u nju je ugrađeno Get/SetSearchText)
            if (!(this.thisControl is JButtonControl))
                return null;

            try
            {
                string filterText = null;
                string frmFilterName = action.Name;
                using (Forms.SearchDocFilter frmFilter = new Forms.SearchDocFilter(frmFilterName))
                {

                    if (frmFilter.ShowDialog() == DialogResult.OK)
                    {
                        filterText = frmFilter.FilterText;
                        (this.thisControl as JButtonControl).SetSearchText(filterText);
                    }

                }
                if (!string.IsNullOrEmpty(filterText))
                {
                    //npr. <action type = "search" value = "~dtSearch">
                    Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, action.Value);
                    DataSet ds = dataDefinition.GetDataSet("dohvat 'pronađi' podataka '" + action.Value + "'");
                    if (ds.Tables.Count > 0)
                    {
                        bool searchFormACanceled = false;
                        DataRow dr = null;
                        if (ds.Tables[0].Rows.Count == 1)
                            dr = ds.Tables[0].Rows[0];
                        else if (ds.Tables[0].Rows.Count > 1)
                        {
                            using (Forms.SearchDoc frm = new Forms.SearchDoc(form, ds.Tables[0]))
                            {
                                if (frm.ShowDialog() == DialogResult.OK && frm.FocusedRow != null)
                                {
                                    if (form is Forms.Master.Manager)
                                    {
                                        dr = frm.FocusedRow;
                                    }
                                }
                                else
                                {
                                    searchFormACanceled = true;
                                }
                            }
                        }

                        if (searchFormACanceled == false)
                        {
                            if (dr != null)
                            {
                                (form as Forms.Master.Manager).trMain.SearchDoc(dr);
                            }
                            else
                            {
                                Common.Helpers.JMessages.Info("Dokument nije pronađen.");
                            }
                        }
                    }
                }

                return true;
            }
            catch { throw; }
        }

        private bool CheckExit(JEventAction action)
        {
            if (action.Type == null || action.Type.ToLower() != "exit")
                return false;

            try
            {
                form.Logger.Info("method: CheckExit", "Action.Value=" + action.Value);

                Form frm = form;
                Form formClosing;
                while (frm != null)
                {
                    formClosing = frm;
                    formClosing.Close();
                    frm = form.Owner;
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckMethod(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("method"))
                return false;

            try
            {
                form.Logger.Info("method: CheckMethod", "Action.Value=" + action.Value);

                JCustomMethod method = Methods.GetMethod(form, action.Value);
                if (method != null)
                {
                    method.SetParameters(action);
                    method.Run();
                }
                else
                {
                    form.Logger.Warn("method: CheckMethod", "Metoda '" + action.Value + "' nije pronađena!");
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckProcess(JEventAction action)
        {
            if (action.Type == null || action.Type.ToLower() != "process.run")
                return false;

            try
            {
                form.Logger.Info("method: CheckProcess", "Action.Value=" + action.Value);

                if (action.Parameter.Count > 0)
                {
                    string processValue = "";

                    using (Parser parser = new Parser(form, null))
                    {
                        object valueObj = parser.GetLogicPartValue(action.Parameter[0].Value);
                        if (valueObj != null)
                            processValue = valueObj.ToString();
                        else
                        {
                            form.Logger.Warn("method: CheckProcess", "Vrijednost parametra 'value' u akciji 'process.run' je null");
                            return true;
                        }
                    }

                    using (Process additionalProcess = new Process())
                    {
                        additionalProcess.StartInfo.FileName = processValue;
                        additionalProcess.Start();
                    }
                }
                else
                {
                    form.Logger.Warn("method: CheckProcess", "Akcija 'process.run' mora primiti parametar sa vrijednošću koju poziva");
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckVariable(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("variable"))
                return false;

            try
            {
                if (action.Type.ToLower().Contains("value"))
                {
                    FormsVar.SetVariableValue(form, action);
                }
                else
                {
                    form.Logger.Warn("method: CheckFormVariable", "Atribut varijable 'value' ne postoji.");
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckComDll(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("comdll"))
                return false;

            try
            {
                if (!string.IsNullOrEmpty(action.Value))
                {
                    ComDll.Run(form, action);
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckForm(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("form"))
                return false;

            try
            {
                if (action.Type.Contains("caption"))
                {
                    using (Carpo.UI.Logic.Parser parser = new Carpo.UI.Logic.Parser(form, null))
                    {
                        string valueStr = action.Value;
                        object value = parser.GetLogicPartValue(valueStr);
                        if (value != null)
                            form.Text = value.ToString();
                        else
                            form.Text = action.Value;
                    }
                }
                else if (action.Type.Contains("open"))
                {
                    if (closingForm == true)
                    {
                        (form.Owner as Forms.Base.JupiterShared).EventAction = action;
                    }
                    else
                    {
                        NotifyForm(action);
                    }
                }
                else if (action.Type.Contains("close"))
                {
                    if (form.Owner != null)
                    {
                        closingForm = true;
                        form.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        form.Close();
                    }
                }
                else if (action.Type.Contains("dialogresult"))
                {
                    if (form.Owner != null)
                    {
                        if (string.IsNullOrEmpty(action.Value))
                            form.Logger.Error("method: CheckForm", "Akcija tipa 'form dialogresult' mora sadržavati atribut 'value'!");
                        else
                        {
                            closingForm = true;
                            form.LastDialogResult = action.Value.ToLower();
                            switch (form.LastDialogResult)
                            {
                                case "ok":
                                    form.DialogResult = DialogResult.OK;
                                    break;

                                case "cancel":
                                    form.DialogResult = DialogResult.Cancel;
                                    break;

                                case "retry":
                                    form.DialogResult = DialogResult.Retry;
                                    break;

                                default:
                                    closingForm = false;
                                    form.Logger.Error("method: CheckForm", "Neispravan atribut 'value' akcije tipa 'form dialogresult'!");
                                    break;
                            }
                        }
                    }
                    else
                        form.Logger.Warn("method: CheckForm", "Akcija tipa 'form dialogresult' može se primijeniti isključivo na sub formama!");
                }
                else if (action.Type.Contains("clear"))
                {
                    FormsVar.FormClear(form);
                }
                else
                {
                    form.Logger.Info("method: CheckForm", "Niti jedan atribut forme (open, close, clear) ne postoji");
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckData(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("datarun"))
                return false;

            try
            {
                form.Logger.Info("method: CheckData ", "Action.Type=" + action.Type);

                Data.Core.DataType dataType;

                if (action.Type.Contains("reader"))
                    dataType = Data.Core.DataType.Reader;
                else if (action.Type.Contains("scalar"))
                    dataType = Data.Core.DataType.Scalar;
                else
                    dataType = Data.Core.DataType.NonQuery;

                NotifyData(action, dataType, out stopActions, connTranModel);
                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool? CheckTransaction(JEventAction action)
        {
            if (action.Type == null || !(action.Type.ToLower() == "transaction") || action.Actions.Count == 0)
                return null;

            try
            {
                var dataActionsList = new List<JEventAction>();

                foreach (var dataAction in action.Actions)
                {
                    //if (!((dataAction.Type.Contains("reader")) || (dataAction.Type.Contains("scalar")) || (dataAction.Type.Contains("nonquery"))))
                    //    continue;

                    //dataActionsList.Add(dataAction);

                    //if (dataAction.Type == null) // dodana akcija IF
                    //{
                    //    if (string.IsNullOrEmpty(dataAction.If) && string.IsNullOrEmpty(dataAction.ElseIf) && string.IsNullOrEmpty(dataAction.Else))
                    //        continue;
                    //}
                    //else if (!((dataAction.Type.Contains("reader")) || (dataAction.Type.Contains("scalar")) || (dataAction.Type.Contains("nonquery"))))
                    //    continue;

                    // TO DO - Check IF

                    dataActionsList.Add(dataAction);
                }

                if (dataActionsList.Count > 0)
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        SqlTransaction transaction = null;
                        try
                        {
                            connection.Open();
                            transaction = connection.BeginTransaction();
                            connTranModel = new ConnWithTran() { Connection = connection, Transaction = transaction };

                            RunActions(dataActionsList);

                            transaction.Commit();
                            connection.Close();
                            form.LastTransactionSuccess = "success";
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                transaction?.Rollback();
                            }
                            catch (Exception rollbackEx)
                            {
                                form?.Logger?.Error("Events.CheckTransaction rollback failed", rollbackEx.Message, rollbackEx);
                            }
                            try { connection.Close(); } catch { /* ignore */ }
                            form.LastTransactionSuccess = "fail";
                            form?.Logger?.Error("Events.CheckTransaction failed", ex.Message, ex);
                            foreach (var parameter in transactionVariables)
                            {
                                FormsVar.SetVariable(form, parameter, null, true);
                            }
                        }
                        finally
                        {
                            connTranModel = null;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex) { throw; }
        }

        private bool CheckSetLayout(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().EndsWith("layout"))
                return false;

            try
            {
                var gridName = action.Value.Replace("#", "");
                if (!string.IsNullOrEmpty(gridName))
                {
                    var grid = Logic.Controls.FindControl(form, gridName)[0];
                    if (grid != null)
                    {
                        if (action.Type.Contains("set"))
                        {
                            NotifyControl(grid, action, "dynamicLayout");    //save layout
                            NotifyControl(grid, action, null);   // clear datasource + columns

                            // parsaj vrijednost layoutName(layoutName - vrijednost novog dataseta grida)
                            using (Parser parser = new Parser(this.form, grid))
                            {
                                string layoutName = "";
                                try
                                {
                                    layoutName = parser.GetLogicPartValue((grid as JGridControl).Definition.LayoutName).ToString();
                                }
                                catch (Exception ex)
                                {
                                    form.Logger.Error("method: CheckSetLayout", ex.Message);
                                }
                                if (action.Parameter.Count > 0)
                                {
                                    JEventActionParameter param = new JEventActionParameter();
                                    bool checkIfResult = false;
                                    foreach (var parameter in action.Parameter)
                                    {
                                        if (Conditions.CheckCondition(ref checkIfResult, parameter, form, null, true) == true)
                                            param = parameter;
                                    }

                                    object findDataDef = parser.GetLogicPartValue(param.Value);
                                    string infoOpis = "NotifyData - Action.Value: " + action.Value;

                                    Logic.Data.Core data = new Logic.Data.Core()
                                    { ConnectionString = connectionString, ParameterControlValue = "", CurrentForm = form };
                                    data.LoadingFormData(findDataDef as JData, infoOpis, connTranModel);

                                    var Jgrid = grid as JGridControl;
                                    Jgrid.Definition.DataName = param.Value;
                                    Jgrid.Populate(action, true, false, Structure.Enums.Controls.FocusType.KeepFocus);
                                    Jgrid.Definition.PreviousLayoutName = layoutName.ToString();

                                    // setiraj novi layout // resetaj ako jos ne postoji parametrizirani layout
                                    VarsLayout layout = new VarsLayout(form);   // restore layout
                                    layout.SetLayout(gridName, layoutName.ToString(), Jgrid.Definition.PreviousLayoutName);
                                }
                                else
                                    form.Logger.Warn("method: CheckSetLayout", "Dataset grida nije definiran.");
                            }
                        }
                        else
                        {
                            form.Logger.Warn("method: CheckSetLayout", "Krivo definiran action name. '" + action.Type + "' ne postoji.");
                        }
                        return true;
                    }
                }
                else
                    form.Logger.Warn("method: CheckSetLayout", "Name atribut grida nije definiran.");

                return true;
            }
            catch
            {
                throw;
            }
        }

        private bool CheckGenerate(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("generate"))
                return false;

            try
            {
                using (var cs = new Carpo.UI.Logic.Generate(action, form))
                {
                    if (action.Type.ToLower() == ("generate datastring"))
                        return cs.GenerateDatastring();
                    else if (action.Type.ToLower() == ("generate string"))
                        return cs.GenerateString();
                    else if (action.Type.ToLower() == ("generate number"))
                        return cs.GenerateNumber();
                }

                return false;
            }
            catch (Exception ex)
            {
                form.Logger.Error("method: CheckGenerate", ex.Message);
                return false;
            }
        }

        #endregion ActionChecker


        private bool CheckInfoControls(JEventAction action)
        {
            if (action.Type == null || !action.Type.ToLower().StartsWith("infocontrols"))
                return false;

            try
            {
                if (action.Type.ToLower().Contains("refresh"))
                {
                    if (form is Forms.Info)
                    {
                        (form as Forms.Info).Repopulate();
                        return true;
                    }
                    else if (form.Owner != null && form.Owner is Forms.Info)
                    {
                        (form.Owner as Forms.Info).Repopulate();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                form.Logger.Error("method: CheckInfoControls", ex.Message);
                return false;
            }
        }

        #region Notifications

        private void NotifyControl(Control control, JEventAction action, object thisControlValue)
        {
            try
            {
                form.Logger.Info("method: NotifyControl", "Control.Name=" + control.Name, thisControlValue);

                if (control is XtraTabPage && action.Type.ToLower().Replace(" ", "") == "controlcaption") // dodao T.A. - tabovi nisu u kontrolama, moramo ovako setat tab caption
                {
                    using (Parser parser = new Parser(this.form, control))
                        control.Text = parser.GetLogicPartValue(action.Parameter[0].Value).ToString();

                    return;
                }

                if (action != null && control is JGridControl && action.Type.ToLower().Contains("foreachrow"))
                    NotifyGridControlForEachRow((control as JGridControl), action);
                // šaljem thisControlValue=null kako bi zaobišao ovaj uvjet
                else if (action != null && control is JGridControl && action.Type.ToLower().Contains("setlayout") && thisControlValue != null)
                {
                    Logic.VarsLayout layout = new Logic.VarsLayout(this.form, (control as JGridControl).Definition.LayoutVersion);
                    layout.SaveXml((control as JGridControl).Views[0],
                        form is Carpo.UI.Forms.Master.Manager ? "grid - manager gla" : "grid - layout",
                        null, (control as JGridControl).Definition.PreviousLayoutName);
                }
                else if (control is Structure.Interfaces.IJControl)
                    (control as Structure.Interfaces.IJControl).SetNotifications(action, thisControlValue);
                else
                {
                    var customObjectGlobal = Carpo.UI.Components.Custom.GlobalCustomControls.CustomControls.Where(x => x.ControlName == control.Name).FirstOrDefault();

                    if (customObjectGlobal != null)
                        NotifyCustomControl((customObjectGlobal as JCustomUC), action, thisControlValue);
                    else
                    {
                        var customObjectLocal = ((Forms.Base.JupiterEditor)form).CustomControls.Where(x => x.ControlName == control.Name).FirstOrDefault();

                        if (customObjectLocal != null)
                            NotifyCustomControl((customObjectLocal as JCustomUC), action, thisControlValue);
                    }
                }
            }
            catch { throw; }
        }

        public void NotifyForm(JEventAction action)
        {
            if (action.Type.Contains("clear"))
            {
                FormsVar.FormClear(form);
                return;
            }

            if (form.Owner != null)
                (form.Owner as Forms.Base.JupiterShared).EventAction = null;

            string formName = "";

            using (Carpo.UI.Logic.Parser parser = new Carpo.UI.Logic.Parser(form, null))
            {
                string valueStr = action.Value;
                object value = parser.GetLogicPartValue(valueStr);
                if (value != null)
                    formName = value.ToString();
                else
                    formName = action.Value;
            }

            var definitionRoot = ((UI.Forms.Base.JupiterShared)form).DefinitionRoot;
            var definitionForm = definitionRoot.Form.Where(x => x.Name == formName).FirstOrDefault();

            OperaterLock operaterLock = new OperaterLock(definitionForm, form);
            if (operaterLock.CheckOperaterLock(definitionForm) == true)
                return;

            if (definitionForm != null)
            {
                string connectionString = ((UI.Forms.Base.JupiterShared)form).ConnectionString;
                DialogResult dialogResult = DialogResult.None;

                Forms.Jupiter.Base formMaster;
                if (form.FormMaster != null)
                    formMaster = form.FormMaster;
                else
                    formMaster = form;

                Forms.Base.JupiterEditor editForm;
                //if (definitionForm.Template.ToLower().Contains("splitter"))
                if (definitionForm.Template != null)
                {
                    if (definitionForm.Template.ToLower().Contains("splitter"))
                        editForm = new Forms.Edit.Splitter(form, definitionForm);
                    else if (definitionForm.Template.ToLower().Contains("navbar"))
                        editForm = new Forms.Edit.NavBar(form, definitionForm);
                    else if (definitionForm.Template.ToLower().Contains("sidebar"))
                        editForm = new Forms.Edit.SideBar(form, definitionForm);
                    else
                    {
                        editForm = null;
                        form.Logger.Warn("event: NotifyForm", "Atribut 'template' nije (dobro) definiran!");
                    }
                }
                else
                    editForm = new Forms.Edit.Standard(form, definitionForm);

                //using (UI.Forms.Base.JupiterEditor editForm = new UI.Forms.Base.JupiterEditor(form, definitionForm))
                //using (UI.Forms.Edit.Splitter editForm = new UI.Forms.Edit.Splitter(form, definitionForm))
                using (editForm)
                {
                    try
                    {
                        editForm.DefinitionRoot = form.DefinitionRoot;
                        editForm.XmlPath = form.XmlPath;
                        FormsVar.SetFormParametersOnClosing(form, editForm, action, parentFormParameters);
                        parentFormParameters = null;
                        closingForm = false;
                        editForm.SetEvents(definitionForm.Event);
                        editForm.SetForm();
                        editForm.SetLayout();
                        form.EventAction = null;
                        //editForm.MinimizeBox = false;


                        dialogResult = editForm.ShowDialog(form);


                        // dialogResult = editForm.ShowDialog(form);

                        if (form != null)
                        {
                            //da se može kontrolirati sljedeća akcija preko if-a... npr dialogresult!=cancel
                            form.LastDialogResult = dialogResult.ToString().ToLower();
                        }
                        operaterLock.UnLock();
                        parentFormParameters = editForm.VariablesForm;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (form.EventAction != null)
                {
                    NotifyForm(form.EventAction);
                }
                else if (dialogResult == DialogResult.Retry)
                {
                    Serializer s = new Serializer(form.Logger, form.XmlPath);
                    form.DefinitionRoot = s.DeserializeXml(form.ModuleId);
                    //form.DeserializeXml();
                    NotifyForm(action);
                }
            }
        }

        private bool NotifyData(JEventAction action, Data.Core.DataType dataType, out bool stopActions, ConnWithTran connTranModel)
        {
            stopActions = false;
            try
            {
                //JForm formDefinition;
                if (form is UI.Forms.Base.JupiterShared)
                {
                    //formDefinition = (form as UI.Forms.Base.JupiterShared).DefinitionForm;

                    Parser parser = new Parser(form, null);
                    parser.Type = Parser.ParserType.Date;
                    object findDataDef = parser.GetLogicPartValue(action.Value);

                    Dictionary<string, int> dataTablesList = GetDataTablesList(action);

                    //JData jDataDef = null;

                    //if (findDataDef == null)
                    //{
                    //if (formDefinition.DataList == null || formDefinition.DataList.Data == null || formDefinition.DataList.Data.Count == 0)
                    //        return false;

                    string actionValue = action.Value.Replace("~", "");

                    //var jDataDef = definition.DataList.Data.First(x => x.Action == "save");
                    //jDataDef = formDefinition.DataList.Data.Where(x => x.Name == action.Value).FirstOrDefault();

                    if (findDataDef == null)
                        return false;
                    //} else
                    //{
                    //    if (findDataDef is JData)
                    //        jDataDef = findDataDef as JData;
                    //    else
                    //        return false;
                    //}

                    string infoOpis = "NotifyData - Action.Value: " + action.Value;

                    Logic.Data.Core data = new Logic.Data.Core()
                    { ConnectionString = connectionString, ParameterControlValue = "", CurrentForm = form };

                    if (dataType == Data.Core.DataType.NonQuery)
                        data.ExecuteData(findDataDef as JData, infoOpis, out stopActions, connTranModel);
                    else if (dataType == Data.Core.DataType.Reader)
                        data.LoadingFormData(findDataDef as JData, infoOpis, connTranModel, dataTablesList);
                    else if (dataType == Data.Core.DataType.Scalar)
                    {
                        object value = data.ExecuteScalar(findDataDef as JData, infoOpis, out stopActions, connTranModel);

                        // osim setanja kreiranja skalarnih varijabli, ovdje punimo i listu temporari skalarnih varijabli za transakciju (čistimo ih na rollbacku ako puca)
                        string transactionVariable = FormsVar.SetScalarParameter(form as Forms.Base.JupiterShared, action, value);
                        if (connTranModel != null && !string.IsNullOrEmpty(transactionVariable))
                            transactionVariables.Add(transactionVariable);
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                form.Logger.Error("method: NotifyData", ex.Message, ex);
                if (connTranModel != null)
                    throw;
                return false;
            }
        }

        private Dictionary<string, int> GetDataTablesList(JEventAction action)
        {
            Dictionary<string, int> list = new Dictionary<string, int>();

            try
            {
                var splitList = action.Value.Split('.');
                if (splitList.Length > 1)
                {
                    if (splitList[1].ToString() == "dataset")
                    {
                        if (action.Parameter != null && action.Parameter.Count > 0)
                        {
                            foreach (var parameter in action.Parameter)
                            {
                                if (parameter.Type.ToLower() == "datatable")
                                {
                                    var listHlp = parameter.Value.Split('[');
                                    string tableName = parameter.Name;
                                    int indexHlp;
                                    bool isNum = int.TryParse(listHlp[1].Replace("]", ""), out indexHlp);
                                    if (isNum)
                                    {
                                        list.Add(tableName, indexHlp);
                                    }
                                    else
                                    {
                                        //form.Logger.Warn("method: CheckConfirm", "Neuspješno parsiranje vrijednosti; " + action.Value.ToString());
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                //form.Logger.Error("method: SetNotification; control name: " + e.Control.Name, ex.Message, ex);
                return null;
            }

            return list;
        }

        private void NotifyMenu(ToolStripMenuItem item, JEventAction action)
        {
            if (action == null || string.IsNullOrEmpty(action.Value))
                return;

            if (action.Type.Contains("enable"))
                item.Enabled = true;
            else if (action.Type.Contains("disable"))
                item.Enabled = false;
        }

        private void NotifyMenu(BarButtonItem item, JEventAction action)
        {
            if (action == null || string.IsNullOrEmpty(action.Value))
                return;

            if (action.Type.Contains("enable"))
                item.Enabled = true;
            else if (action.Type.Contains("disable"))
                item.Enabled = false;
        }

        #region NotifyControlCollection

        private void NotifyGridControlForEachRow(JGridControl control, JEventAction action)
        {
            if (action.Type.ToLower().Contains("foreachrow")) //za svaki red
            {
                bool onlySelected = false;
                if (action.Type.ToLower().Contains("selected"))
                    onlySelected = true;
                (control as JGridControl).SetNotificationsForEachRow(action, onlySelected);
                return;
            }

            //control.ComponentExt.Populate(action, false, null, null);
        }

        private void NotifyCustomControl(JCustomUC customObject, JEventAction action, object thisControlValue)
        {
            customObject.SetNotifications(action, thisControlValue);
        }

        #endregion NotifyControlCollection

        #endregion Notifications

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Notifications() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
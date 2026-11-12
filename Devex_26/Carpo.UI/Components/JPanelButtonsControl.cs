using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Components
{
    public partial class JPanelButtonsControl : DevExpress.XtraEditors.PanelControl
    {
        private const int SeparatorWidth = 22;
        private const int SpacerWidth = 4;

        private enum PanelType { None, PanelButtons, PanelActions }

        private PanelType panelType = PanelType.None;

        //private List<Control> treePanelComponents;

        private DevExpress.XtraEditors.PanelControl parentPanel;

        private string type = "";
        public JPanel Definition { get; set; }

        private Forms.Base.JupiterShared form;
        private bool standardButtonsVisible = false;

        public JPanelButtonsControl()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            FindParentForm();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void Init()
        {
            Init(true);
        }

        public void Init(bool standardButtonsVisible)
        {
            FindParentForm();
            this.standardButtonsVisible = standardButtonsVisible;

            if (Definition == null)
                return;

            if (Definition.Buttons != null && Definition.Buttons.Count > 0)
                panelType = PanelType.PanelButtons;
            else if (Definition.ActionItems != null && Definition.ActionItems.Count > 0)
                panelType = PanelType.PanelActions;
            else
                return;

            if (panelType == PanelType.PanelButtons)
                Populate();
            else
                PopulateByItem();
        }

        private void FindParentForm()
        {
            if (form == null)
                form = (this.FindForm() as Forms.Base.JupiterShared);
        }

        #region PanelButtonsSpecific

        private void Populate()
        {
            JButtons buttons = GetButtons();

            if (buttons != null)
            {
                PopulateTreeActions(buttons);
                int rn = 1;
                foreach (JButton button in buttons.ButtonList)
                {
                    JButtonControl jButton = AddButton(button, null, rn);

                    if (button.Type != null && button.Type.StartsWith("selfdrag"))
                        jButton.Enabled = false;

                    rn++;
                }
                if (standardButtonsVisible)
                    AddStandardButtons();
            }
        }

        private void PopulateTreeActions(JButtons buttons)
        {
            Forms.Master.Manager frmManager = form as Forms.Master.Manager;
            if (type == "tree" && buttons.ButtonList.Count > 0)
            {
                frmManager.chkTreeReorder.Enabled = true;
                if (frmManager != null)
                    parentPanel = frmManager.pnlTreeSpec;
            }
        }

        private JButtons GetButtons()
        {
            if (this.Tag != null && this.Tag.ToString().ToLower() == "tree")
            {
                type = "tree";
                return Definition.Buttons.Where(x => x.Type == "tree").FirstOrDefault();
            }
            return Definition.Buttons.Where(x => x.Type == null || x.Type != "tree").FirstOrDefault();
        }

        #endregion PanelButtonsSpecific

        #region ActionPanelSpecific

        private void PopulateByItem()
        {
            JActionItems items = GetItems();

            if (items != null)
            {
                PopulateTreeActions(items);
                int rn = 1;
                foreach (var actionItem in items.JActionItem)
                {
                    CreateActionItem(actionItem, rn);
                    rn++;
                }
                if (standardButtonsVisible)
                    AddStandardButtons();
            }
        }

        private void PopulateTreeActions(JActionItems actionItems)
        {
            Forms.Master.Manager frmManager = form as Forms.Master.Manager;
            if (type == "tree" && actionItems.JActionItem.Count > 0)
            {
                frmManager.chkTreeReorder.Enabled = true;
                if (frmManager != null)
                    parentPanel = frmManager.pnlTreeSpec;
            }
        }

        private void CreateActionItem(JActionItem actionItem, int rn)
        {
            if (actionItem.Button != null)
            {
                JButtonControl buttonControl = AddButton(actionItem.Button, actionItem.Button.Type, rn);
                if (actionItem.Button.Type != null && actionItem.Button.Type.StartsWith("selfdrag"))
                    buttonControl.Enabled = false;
            }
            else if (actionItem.LabelEdit != null)
                AddLabel(actionItem.LabelEdit, actionItem.LabelEdit.Type, rn);
            else if (actionItem.CheckEdit != null)
                AddCheckBox(actionItem.CheckEdit, actionItem.CheckEdit.Type, rn);
            else if (actionItem.RadioEdit != null)
                AddRadioButton(actionItem.RadioEdit, actionItem.RadioEdit.Type, rn);
        }

        #endregion ActionPanelSpecific

        private JButtonControl AddButton(JButton button, string type, int rn)
        {
            DockStyle dockStyle = GetDockStyle(button.Dock);

            JButtonControl buttonControl = new JButtonControl(button);

            if (form is Forms.Master.Manager && (button.Type == "add" || button.Type == "edit" || button.Type == "delete"))
            {
                (form as Forms.Master.Manager).CrudButtons.Add(buttonControl);
            }
            else if (button.Type != null && button.Type.ToLower().StartsWith("selfdrag"))
            {
                (form as Forms.Master.Manager).SelfDragButtons.Add(buttonControl);
            }

            List<JEvent> events = new List<JEvent>();
            if (button.Event != null)
                events = button.Event;

            SetEvent(events, buttonControl);

            AddSeparator(rn, button.BeginGroup, dockStyle);

            AddControl(buttonControl);

            //if (buttonControl.Handle != null)
            //    if (buttonControl.ComponentExt != null && buttonControl.ComponentExt.DefaultVisibility != false)
            //        AddSeparator(rn, button.BeginGroup, dockStyle);

            DockControlEnd(buttonControl, dockStyle);

            return buttonControl;
        }

        private JRadioEditControl AddRadioButton(JRadioEdit radioEdit, string type, int rn)
        {
            var control = new JRadioEditControl(radioEdit);

            DockStyle dockStyle = GetDockStyle(radioEdit.Dock);

            List<JEvent> events = new List<JEvent>();
            if (radioEdit.Event != null)
                events = radioEdit.Event;

            SetEvent(events, control);
            AddSeparator(rn, false, dockStyle);
            AddControl(control);
            DockControlEnd(control, dockStyle);

            return control;
        }

        private JCheckEditControl AddCheckBox(JCheckEdit checkEdit, string type, int rn)
        {
            var control = new JCheckEditControl(checkEdit);

            DockStyle dockStyle = GetDockStyle(checkEdit.Dock);

            List<JEvent> events = new List<JEvent>();
            if (checkEdit.Event != null)
                events = checkEdit.Event;

            SetEvent(events, control);

            //control.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            ////control.Width = ;
            //control.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            AddSeparator(rn, false, dockStyle);

            AddControl(control);
            DockControlEnd(control, dockStyle);

            return control;
        }

        private JLabelEditControl AddLabel(JLabelEdit labelEdit, string type, int rn)
        {
            var control = new JLabelEditControl(labelEdit);

            DockStyle dockStyle = GetDockStyle(labelEdit.Dock);

            List<JEvent> events = new List<JEvent>();
            if (labelEdit.Event != null)
                events = labelEdit.Event;

            SetEvent(events, control);

            control.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            //control.Width = ;
            control.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            AddSeparator(rn, false, dockStyle);

            AddControl(control);
            DockControlEnd(control, dockStyle);

            return control;
        }

        private void AddSeparator(int rn, bool? beginGroup, DockStyle dockStyle)
        {
            if (rn > 1)
            {
                if (beginGroup == true)
                    AddSeparator(dockStyle);
                else
                    AddSpacer(dockStyle);
            }
        }

        private DockStyle GetDockStyle(string dockStyleStr)
        {
            DockStyle dockStyle = DockStyle.Left;
            if (!string.IsNullOrEmpty(dockStyleStr))
            {
                if (dockStyleStr.ToLower() == "right")
                    dockStyle = DockStyle.Right;
                else if (dockStyleStr.ToLower() == "fill")
                    dockStyle = DockStyle.Fill;
            }
            return dockStyle;
        }

        private void DockControlEnd(Control control, DockStyle dockStyle)
        {
            control.Dock = dockStyle;
            if (dockStyle == DockStyle.Left)
                control.BringToFront();
            else
                control.SendToBack();
        }

        private void SetEvent(List<JEvent> events, Control control)
        {
            if (events != null && events.Count > 0)
                form.EventList.Add(new Structure.Models.ControlEvents() { Control = control, Events = events });
        }

        private JActionItems GetItems()
        {
            if (this.Tag != null && this.Tag.ToString().ToLower() == "tree")
            {
                type = "tree";
                return Definition.ActionItems.Where(x => x.Type == "tree").FirstOrDefault();
            }
            return Definition.ActionItems.Where(x => x.Type == null || x.Type != "tree").FirstOrDefault();
        }

        private void AddSpacer(DockStyle dockStyle)
        {
            Panel panel = new Panel() { BorderStyle = System.Windows.Forms.BorderStyle.None, Width = SpacerWidth, Dock = dockStyle };
            AddControl(panel);
            DockControl(panel, dockStyle);
        }

        private void AddSeparator(DockStyle dockStyle)
        {
            JSeparatorVertButtons separator = new JSeparatorVertButtons() { Dock = dockStyle };
            separator.Width = SeparatorWidth;
            AddControl(separator);
            DockControl(separator, dockStyle);
        }

        private void AddControl(Control control)
        {
            if (type == "tree")
                parentPanel.Controls.Add(control);
            else
                Controls.Add(control);
        }

        //private void AddDragDropButton(JButtonControl button)
        //{
        //    if (form is Forms.Master.Manager)
        //        (form as Forms.Master.Manager).trMain.ComponentExt.SetSelfReorderderButton(button);

        //}

        private void DockControl(Control control, DockStyle dockStyle)
        {
            if (dockStyle == DockStyle.Left)
                control.BringToFront();
            else
                control.SendToBack();
        }

        private void AddStandardButtons()
        {
            if (Definition.DisplayControlsPrint == false && Definition.DisplayHelp == false)
                return;

            JStandardButtons standardButtons = new JStandardButtons();
            if (Definition.DisplayHelp == false)
            {
                standardButtons.cmdStandardHelp.Visible = false;
                standardButtons.pnlHelpSeparator.Visible = false;
            }
            if (Definition.DisplayControlsPrint == true)
            {
                standardButtons.cmdStandardIspisKontrola.Visible = true;
                standardButtons.pnlHelpSeparator.Visible = true;
            }

            AddSeparator(DockStyle.Left);
            standardButtons.Dock = DockStyle.Left;
            AddControl(standardButtons);
            standardButtons.BringToFront();
        }
    }
}
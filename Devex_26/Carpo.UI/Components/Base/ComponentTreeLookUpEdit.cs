using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentTreeLookUpEdit : ComponentLookUpEdit
    {
        private JTreeLookUpControl treeLookUpControl;

        public TreeListNode BeforeFocusNode { get; set; } // za blokiranje klika na red
        private TreeListNode nodeCellStyle = null;
        public bool BeforeFocusing { get; set; }  //ako je uključeno blokiranje klika na red
        public JEvent DisableNodeEvent { get; set; }

        public ComponentTreeLookUpEdit() : base()
        {
        }

        public ComponentTreeLookUpEdit(Control control, object def) : base(control, def)
        {
            treeLookUpControl = (JTreeLookUpControl)control;
        }

        internal override void Init()
        {
            DateTime startTime = DateTime.Now;
            base.Init();
            SetPrivateProperties();
            form.Logger.InfoData("Init TreeLookUpEdit ", definition.Name, startTime);
            //treeLookUpControl.PreviewKeyDown += (s, e) =>
            //{
            //    PreviewKeyDown(s as LookUpEdit, e);
            //};

        }

        private void SetPrivateProperties()
        {
            treeLookUpControl.Properties.AllowSelectOnHover = false;
            if (definition.FilterMode != "startswith")
                treeLookUpControl.Properties.PopupFilterMode = PopupFilterMode.Contains;
            // Numerički minus je i collapse prečica. CollapseAnimation.PrepareAnimation pada s
            // NullReference dok je popup filter već suzio redove (npr. org. shema "5100-").
            treeLookUpControl.Properties.TreeList.OptionsBehavior.AllowExpandAnimation = DefaultBoolean.False;
            if (definition.NodeFilterMode == "extended")
                treeLookUpControl.Properties.TreeList.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Extended;
            // klasa nasljeđuje property iz comboedit klase, ali iz nekog razloga seta vrijednost u prazan string umjesto null kada
            // se obriše text value kontrole te time dopušta null unatoč propertyu. U nastavku je custom handlanje allownull=false situacije
            if (definition.AllowNull == false)
            {
                using (DataTable dt = treeLookUpControl.Properties.DataSource as DataTable)
                {
                    treeLookUpControl.EditValue = dt.Rows[0][treeLookUpControl.Properties.ValueMember];
                }
                treeLookUpControl.Leave += (s, e) =>
                {
                    using (DataTable ds = treeLookUpControl.Properties.DataSource as DataTable)
                    {
                        if (treeLookUpControl.Properties.GetIndexByKeyValue(treeLookUpControl.EditValue) < 0)
                            treeLookUpControl.EditValue = treeLookUpControl.OldEditValue;
                    }
                };
            }

            treeLookUpControl.Properties.TreeList.NodeCellStyle += (s, e) =>
            {
                NodeCellStyle(s, e);
            };
        }

        private void NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                nodeCellStyle = e.Node;

                if (BeforeFocusing == true)
                    SetDisabledRowCellStyle(e.Appearance);
            }
            catch
            {
                throw;
            }
        }

        private bool SetDisabledRowCellStyle(AppearanceObject appearanceObject)
        {
            if (DisableNodeEvent == null)
                return false;

            bool checkIfResult = true;

            if (Conditions.CheckCondition(ref checkIfResult, DisableNodeEvent.Actions[0], form, "NodeCellStyle", false) == false)
                return false;

            Style.SetStyle(appearanceObject, "fontcolor", "darkgray");
            return true;
        }

        internal override void CreateColumns()
        {
            treeLookUpControl.Properties.TreeList.Columns.Clear();
            if (definition.Fields != null && definition.Fields.FieldList != null)
            {
                foreach (JField field in definition.Fields.FieldList)
                {
                    treeLookUpControl.Properties.TreeList.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn { FieldName = field.Name, Caption = field.Caption, Visible = true });
                }
            }
            else
                treeLookUpControl.Properties.TreeList.Columns.Add(new DevExpress.XtraTreeList.Columns.TreeListColumn { FieldName = definition.DisplayField, Visible = true });
        }

        internal override void PreviewKeyDown(LookUpEdit editL, PreviewKeyDownEventArgs e)
        {
            TreeListLookUpEdit edit = treeLookUpControl as TreeListLookUpEdit;
            if (edit.Properties.DataSource == null)
            {
                MethodInfo activateMethod = typeof(RepositoryItemGridLookUpEdit).GetMethod("ActivateGridDataSource", (BindingFlags.NonPublic | BindingFlags.Instance));
                activateMethod.Invoke(edit.Properties, null);
            }
            if (e.KeyCode == Keys.Delete) edit.EditValue = null;
            if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Up))
            {
                int RowHandle = edit.Properties.GetIndexByKeyValue(edit.EditValue);

                if ((RowHandle < 0) && (e.KeyCode == Keys.Down))
                    RowHandle = 0;
                else if (e.KeyCode == Keys.Down)
                    RowHandle = (RowHandle + 1);
                else
                    RowHandle = (RowHandle - 1);

                if (((RowHandle >= 0) && (RowHandle < edit.Properties.TreeList.AllNodesCount)))
                    edit.EditValue = edit.Properties.GetKeyValueByIndex(RowHandle);

                if (e.KeyCode == Keys.Down)
                    (edit as TextEdit).Select(edit.Text.Length, 0);
            }
        }

        public object GetValue(string field, List<string> helperStr)
        {
            DateTime startTime = DateTime.Now;
            if (!BeforeFocusing)
            {
                DataRowView rowView = (DataRowView)treeLookUpControl.GetSelectedDataRow();
                if (rowView != null)
                {
                    DataRow row = rowView.Row;
                    var column = Logic.Data.Core.FindDataColumn(field, DataColumns);
                    if (row.Table.Columns.Contains(column))
                    {
                        return row[column];
                    }
                }
                return null;
            }
            else
            {
                form.Logger.InfoData("GetValue", definition.Name, startTime);
                return GetValueBeforeFocusing(field, helperStr);
            }
        }

        private object GetValueBeforeFocusing(string field, List<string> helperStr)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                string helperStr1 = "";
                if (helperStr != null && helperStr.Count > 0)
                    helperStr1 = helperStr[0];

                TreeListNode node = null;

                if (helperStr1 == "NodeCellStyle" && nodeCellStyle != null)
                    node = nodeCellStyle;
                else if (BeforeFocusNode != null)
                    node = BeforeFocusNode;

                if (!string.IsNullOrEmpty(field))
                {
                    field = Logic.Data.Core.FindDataColumn(field, DataColumns);

                    if (treeLookUpControl.DataRowRef != null)
                        return treeLookUpControl.DataRowRef[field];
                    else if (node != null)
                        return node[field];
                }
                else if (node != null)
                {
                    return node.GetValue(definition.ValueField);
                }
            }
            catch { }
            form.Logger.InfoData("GetValueBeforeFocusing ", definition.Name, startTime);
            return null;
        }

        public object GetValueSpecial(string name, List<string> helperStr)
        {
            string helperStr1 = "";
            if (helperStr != null && helperStr.Count > 0)
                helperStr1 = helperStr[0];

            name = name.Trim().ToLower();

            if (name == "level")
                return GetNodeLevel(helperStr).ToString();

            return null;
        }

        public int GetNodeLevel(List<string> helperStr)
        {
            string helperStr1 = "";
            if (helperStr != null && helperStr.Count > 0)
                helperStr1 = helperStr[0];

            if (helperStr1 == "NodeCellStyle" && nodeCellStyle != null)
                return nodeCellStyle.Level;
            else if (BeforeFocusing == true && BeforeFocusNode != null)
                return BeforeFocusNode.Level;
            else
                return -1;
        }
    }
}
using Carpo.Structure.Models.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Carpo.UI.Components
{
    public partial class JRepositoryLookUpControl : DevExpress.XtraEditors.Repository.RepositoryItemComboBox
    {
        public List<Structure.Models.FilterTreeComboEditItem> itemList = new List<Structure.Models.FilterTreeComboEditItem>();
        public Forms.Base.JupiterShared CurrentForm { get; set; }
        public JComboEdit Definition { get; set; }

        private JEvent eventValueChanged = null;

        public JRepositoryLookUpControl()
        {
            InitializeComponent();

            NullText = "";
            NullValuePrompt = "...";
            TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            NullValuePromptShowForEmptyValue = true;
            ShowNullValuePromptWhenFocused = true;
        }

        public void EventEditValueChanged()
        {
            if (eventValueChanged != null)
            {
                Logic.Events jEvent = new Logic.Events(eventValueChanged, null, CurrentForm, null);
                jEvent.RunNotification();
            }
        }

        public JRepositoryLookUpControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void Init()
        {
            eventValueChanged = Definition.Event.Where(x => x.Type == "valuechanged").FirstOrDefault();
            if (Definition.Enabled != null && Definition.Enabled == false)
            {
                this.Enabled = false;
            }
            if (!string.IsNullOrEmpty(Definition.Name))
                this.Name = Definition.Name;
            PopulateItems();
        }


        //public void Populate(bool force)
        //{
        //    DateTime startTime = DateTime.Now;
        //    if (force == true)
        //    {
        //        jLookUpControl.Properties.DataSource = null;
        //        jLookUpControl.EditValue = null;
        //        //ValueChanged();
        //    }
        //    if (form == null)
        //        return;
        //    if (jLookUpControl.Properties.DataSource != null)
        //        return;

        //    bool stopActions = false;
        //    Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);
        //    DataSet dataSet = dataDefinition.GetDataSet("punjenje kontrole '" + definition.Name + "'", out stopActions);
        //    if (dataSet == null || dataSet.Tables.Count == 0)
        //        return;
        //    else
        //    {
        //        string[] arrDtColumns = dataSet.Tables[0].Columns.Cast<DataColumn>()
        //             .Select(x => x.ColumnName)
        //             .ToArray();
        //        this.DataColumns = arrDtColumns;
        //    }

        //    jLookUpControl.Properties.DisplayMember = Logic.Data.Core.FindDataColumn(definition.DisplayField, DataColumns);
        //    jLookUpControl.Properties.ValueMember = Logic.Data.Core.FindDataColumn(definition.ValueField, DataColumns);
        //    if (!(string.IsNullOrEmpty(jLookUpControl.Properties.ValueMember)))
        //        ValueFieldColumnType = dataSet.Tables[0].Columns[jLookUpControl.Properties.ValueMember].DataType;

        //    if (dataSet.Tables.Count > 0)
        //    {
        //        jLookUpControl.Properties.DataSource = dataSet.Tables[0];
        //        CreateColumns();

        //        //SetValue(jLookUpControl as IJEditor);

        //        //if (this.Name == "lokacija")
        //        //{
        //        //    this.ShowPopup();

        //        //    if (dataSet.Tables[0].Rows.Count > 0)
        //        //        this.PopupForm.SelectedIndex = 0;
        //        //}
        //        form.Logger.InfoData("Populate LookUpEdit ", definition.Name, startTime);
        //    }
        //}

        public void PopulateItems()
        {
            itemList.Clear();
            Items.Clear();

            bool stopActions = false;
            Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(CurrentForm, Definition);
            DataSet dataSet = dataDefinition.GetDataSet("punjenje kontrole '" + Definition.Name + "'", out stopActions);

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    JComboEditItem item = new JComboEditItem();
                    if (row["value"] != null)
                    {
                        item.Value = row["value"].ToString();
                    }
                    if (row["searchfield"] != null && row["searchfield"] != System.DBNull.Value)
                    {
                        item.SearchField = row["searchfield"].ToString();
                    }
                    if (row["text"] != null && row["text"] != System.DBNull.Value)
                    {
                        item.Text = row["text"].ToString();
                    }
                    itemList.Add(new Structure.Models.FilterTreeComboEditItem() { Definition = item, Value = item.Value });
                    Items.Add(item.Value);
                }
                return;
            }

            if (Definition.ComboItemList != null && Definition.ComboItemList.Count > 0)
            {
                foreach (JComboEditItem item in Definition.ComboItemList)
                {
                    itemList.Add(new Structure.Models.FilterTreeComboEditItem() { Definition = item, Value = item.Value });
                    Items.Add(item.Value);
                }
            }
        }
    }
}
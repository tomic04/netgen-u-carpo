using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentSearh : ComponentBase
    {
        public enum SenderSearch { Key, Button }

        public static bool MoveNextControl { get; set; } = true;
        private JSearchControl buttonEdit;
        private new JSearchEdit definition;

        private string fieldValue, fieldText;

        private int minSearchLength = 1;

        public string[] DataColumns { get; set; } = new string[] { };

        public enum Type { Custom, Partneri, Roba }

        private object id;

        //public DataRow DataRow { get; set; } //selektirana stavka

        public object Id
        {
            get { return id; }
            set
            {
                if (form == null)
                    id = null;
                else if (form.IsSettingLayout == false)
                {
                    if (id != value)
                    {
                        if (value == null)
                            buttonEdit.DataRowRef = null;
                        id = value;
                        ValueChanged();
                        SetClearButton();

                        if (this.form is Forms.Info)
                        {
                            ((Forms.Info)(this.form)).PopulateNav(false);
                            ((Forms.Info)(this.form)).SrcPopupEditValueChanged(false);
                        }
                    }
                }
                //OnPropertyChanged("Id");
            }
        }

        public string DisplayText { get; set; }

        public DataRow SelectedDataRow { get; set; }

        public ComponentSearh() : base()
        {
        }

        public ComponentSearh(Control control, object def) : base(control, def)
        {
            definition = def as JSearchEdit;
            if (!(control is TextEdit))
                return;

            buttonEdit = (control as JSearchControl);
        }

        public void Init()
        {
            DateTime startTime = DateTime.Now;
            buttonEdit.Properties.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;
            if (definition != null && definition.ReadOnly == true)
                buttonEdit.Properties.Buttons[0].Enabled = false;

            buttonEdit.Properties.Buttons[0].Click += (s, e) =>
            {
                Search(SenderSearch.Button);
            };

            if (definition.MinSearchLength != null)
                minSearchLength = (int)definition.MinSearchLength;

            SetButtons();
            SetEvents();

            if (definition.FontSizeDelta != null)
                buttonEdit.Properties.Appearance.FontSizeDelta = (int)definition.FontSizeDelta;

            if (definition.FontBold == true)
                buttonEdit.Properties.Appearance.Font = new System.Drawing.Font(buttonEdit.Properties.Appearance.Font, System.Drawing.FontStyle.Bold);

            Common.Helpers.Formats.FormatTextEdit(definition.Format, buttonEdit.Properties);
            if (definition.NullText != null)
            {
                buttonEdit.Properties.NullText = definition.NullText;
                buttonEdit.Properties.NullValuePrompt = definition.NullText;
            } 
            else
            {
                buttonEdit.Properties.NullText = "...";
                buttonEdit.Properties.NullValuePrompt = "...";
            }
            buttonEdit.Properties.NullValuePromptShowForEmptyValue = true;
            buttonEdit.Properties.ShowNullValuePromptWhenFocused = true;


            if (definition.AllowNull != false)
            {
                buttonEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                buttonEdit.Properties.Buttons[0].Enabled = false;
            }
            if (!string.IsNullOrEmpty(definition.MaxLength))
            {
                using (Parser parser = new Parser(form, null))
                {
                    object value = parser.GetLogicPartValue(definition.MaxLength);
                    int n;
                    bool isNumeric = int.TryParse(value.ToString(), out n);
                    if (isNumeric)
                        buttonEdit.Properties.MaxLength = n;
                }
            }

            buttonEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;

            form.Logger.InfoData("Init search ", definition.Name, startTime);
        }

        private void SetEvents()
        {
            buttonEdit.Leave += (s, e) =>
            {
                if (string.IsNullOrEmpty(definition.searchmode))
                {
                    if (DisplayText != null)
                        buttonEdit.Text = DisplayText.ToString().Trim();
                    else
                        buttonEdit.Text = "";
                }
                else
                    buttonEdit.SetValue(buttonEdit.Text.Trim(), buttonEdit.Text.Trim());
            };

            buttonEdit.KeyDown += (s, e) =>
            {
                OnKeyDown(e);
            };

            buttonEdit.Enter += (s, e) =>
            {
                buttonEdit.BeginInvoke(new MethodInvoker(() =>
                {
                    buttonEdit.SelectionStart = 0;
                    buttonEdit.SelectionLength = buttonEdit.Text.Length;

                    MoveNextControl = SetMoveNextControl();
                }));
            };

            buttonEdit.TextChanged += (s, e) =>
            {
                MoveNextControl = SetMoveNextControl();
            };
        }

        private bool SetMoveNextControl()
        {
            string controlText = buttonEdit.Text.Trim();

            //if (definition.EnterMoveNextControl == false)
            //    return false;
            if (controlText.Length >= minSearchLength && this.DisplayText != null && controlText.Trim() == this.DisplayText.Trim() && Id != null)
                return true;
            else if (controlText.Length == 0)
                return true;
            else
                return false;
        }

        public void OnKeyDown(KeyEventArgs e)
        {
            //if ((e.KeyCode == Keys.Enter && controlText.Length > 1) || (controlText.Length == 1 && controlText == "%"))

            if ((e.KeyData == Keys.Tab || e.KeyData == Keys.Enter || e.KeyData == Keys.Return))
            {
                if (MoveNextControl == false)
                     Search(SenderSearch.Key);
                else
                    SetNext(false);
            }
        }

        public void Search(SenderSearch senderSearch)
        {
            string controlText = buttonEdit.Text.Trim();

            if (controlText.Length >= minSearchLength)
            {
                if (this.DisplayText != null && controlText.Trim() == this.DisplayText.Trim() && Id != null)
                {
                    if (senderSearch == SenderSearch.Key)
                        SetNext(false);
                    return;
                }

                //DataSet

                string sqlText;
                DataSet dataSet;
                DateTime startTime;

                if (definition.Type != null && definition.Type.StartsWith("rob", StringComparison.OrdinalIgnoreCase))
                {
                    object parStruId = Logic.FormsVar.GetFormParameterValue(form, "ParStruId", false);
                    startTime = DateTime.Now;
                    int parStruIdInt = (parStruId == null || Convert.IsDBNull(parStruId)) ? 0 : Convert.ToInt32(parStruId);
                    dataSet = Carpo.Data.Get.Search.Roba(connectionString, buttonEdit.Text, parStruIdInt, out sqlText);
                    form.Logger.InfoData("roba search", sqlText, startTime);
                }
                else if (definition.Type != null && definition.Type.StartsWith("par", StringComparison.OrdinalIgnoreCase))
                {
                    startTime = DateTime.Now;
                    dataSet = Carpo.Data.Get.Search.Partneri(connectionString, buttonEdit.Text, out sqlText);
                    form.Logger.InfoData("partneri search", sqlText, startTime);
                }

                // Dodao Ognjen
                else
                {
                    //provjera da li je DataName definiran
                    if (definition.DataName != null)
                    {
                        bool stopActions = false;
                        startTime = DateTime.Now;
                        Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);
                        dataSet = dataDefinition.GetDataSet("custom search", out stopActions);
                    }
                    else
                    {
                        form.Logger.Warn("method: Search; kontrola: " + control.Name, @"atribut 'DataName nije defeniran'");
                        return;
                    }
                }
                //

                using (dataSet)
                {
                    if (dataSet != null && dataSet.Tables.Count > 0 && !string.IsNullOrEmpty(definition.searchmode)
                        && definition.searchmode.ToLower() == "custom" && dataSet.Tables[0].Rows.Count == 0)
                    {
                        buttonEdit.DataRowRef = null;
                        buttonEdit.SetValue(buttonEdit.Text, buttonEdit.Text);
                        SendKeys.Send("{TAB}");
                        return;
                    }

                    if (dataSet != null && dataSet.Tables.Count > 0)
                    {
                        using (DataTable dataTable = dataSet.Tables[0])
                        {
                            string[] arrDtColumns = dataTable.Columns.Cast<DataColumn>()
                                     .Select(x => x.ColumnName)
                                     .ToArray();
                            this.DataColumns = arrDtColumns;

                            fieldValue = definition.FieldValue;
                            if (string.IsNullOrEmpty(fieldValue))
                                if (dataTable.Columns.Count > 0)
                                    fieldValue = dataTable.Columns[0].ColumnName;
                                else
                                    fieldValue = Logic.Data.Core.FindDataColumn(fieldValue, DataColumns);

                            fieldText = definition.FieldText;
                            if (string.IsNullOrEmpty(fieldText))
                                if (dataTable.Columns.Count > 1)
                                    fieldText = dataTable.Columns[1].ColumnName;
                                else
                                    fieldText = Logic.Data.Core.FindDataColumn(fieldText, DataColumns);

                            if (dataTable.Rows.Count == 1 && dataTable.Columns.Contains(fieldValue) && dataTable.Columns.Contains(fieldText))
                            {
                                buttonEdit.DataRowRef = dataTable.Rows[0];
                                object idVal = SafeToIntOrNull(dataTable.Rows[0][fieldValue]);
                                string text = (dataTable.Rows[0][fieldText] == null || Convert.IsDBNull(dataTable.Rows[0][fieldText]))
                                    ? string.Empty : dataTable.Rows[0][fieldText].ToString().Trim();
                                buttonEdit.SetValue(idVal, text);
                                if (senderSearch == SenderSearch.Key)
                                {
                                    SetNext(true);
                                    buttonEdit.SelectionStart = 0;
                                    buttonEdit.SelectionLength = buttonEdit.Text.Length;
                                }

                                return;
                            }
                            using (Forms.Search search = new Forms.Search(form, dataTable, definition, controlText))
                            {
                                if (search.ShowDialog() == DialogResult.OK)
                                {
                                    GridView gv = ((GridView)search.gridControl1.MainView);
                                    if (gv.FocusedRowHandle >= 0 && dataTable.Columns.Contains(fieldValue) && dataTable.Columns.Contains(fieldText))
                                    {
                                        DataRow row = gv.GetDataRow(gv.FocusedRowHandle);
                                        buttonEdit.DataRowRef = row;
                                        object idVal = SafeToIntOrNull(row[fieldValue]);
                                        string text = (row[fieldText] == null || Convert.IsDBNull(row[fieldText]))
                                            ? string.Empty : row[fieldText].ToString().Trim();
                                        buttonEdit.SetValue(idVal, text);

                                        SetNext(definition.EnterMoveNextControl == false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (controlText.Length == 0)
            {
                SetNext(true);
            }
        }

        private static object SafeToIntOrNull(object value)
        {
            if (value == null || Convert.IsDBNull(value))
                return null;
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return null;
            }
        }

        private void SetNext(bool checkId)
        {
            MoveNextControl = SetMoveNextControl();

            //if (definition.EnterMoveNextControl == false)
            //    return;
            if (Id == null)
            {
                if (checkId == true || definition.IgnoreEnterOnNull == true)
                    return;
            }
            else if (checkId == true)
                return;

            SendKeys.Send("{TAB}");
        }

        //private void SetValue(int id, string text)
        //{
        //    DisplayText = text;
        //    buttonEdit.EditValue = text;
        //    Id = id;
        //}

        private void SetButtons()
        {
            DevExpress.XtraEditors.Controls.EditorButton clearButton = new DevExpress.XtraEditors.Controls.EditorButton();
            clearButton.IsDefaultButton = false;
            clearButton.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Delete;
            clearButton.ToolTip = "Ukloni";

            clearButton.Click += (s, e) =>
            {
                OnClearButtonClick();
            };

            buttonEdit.Properties.Buttons.Insert(0, clearButton);
            buttonEdit.Properties.Buttons[0].Enabled = false;
        }

        internal void SetClearButton()
        {
            if (definition.AllowNull == false)
                buttonEdit.Properties.Buttons[0].Enabled = false;
            else if (buttonEdit.Properties.Buttons.Count > 1 && definition.ReadOnly != true)
                buttonEdit.Properties.Buttons[0].Enabled = !Equals(Id, null) && !Equals(Id, System.DBNull.Value);
        }

        private void OnClearButtonClick()
        {
            buttonEdit.SetValue(null, "");
            SetClearButton();
        }

        public object GetValue(string field)
        {
            if (buttonEdit.DataRowRef != null)
                return buttonEdit.DataRowRef[field];
            else
                return null;
        }
    }
}
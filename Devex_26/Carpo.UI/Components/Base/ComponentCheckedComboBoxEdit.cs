using Carpo.Structure.Interfaces;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentCheckedComboBoxEdit : ComponentBase
    {
        private int selectedRowsCount = 0;
        private JComboMultiEditControl comboMultiEditControl;
        public string[] DataColumns { get; set; } = new string[] { };

        public ComponentCheckedComboBoxEdit() : base()
        {
        }

        public ComponentCheckedComboBoxEdit(Control control, object def) : base(control, def)
        {
            comboMultiEditControl = (JComboMultiEditControl)control;
        }

        internal virtual void Init()
        {
            SetButtons();
            SetProperties();
            if (comboMultiEditControl.Enabled == true)
            {
                Populate(false);
                SetValue(comboMultiEditControl as IJEditor);
            }

            comboMultiEditControl.Leave += (s, e) =>
            {
                OnLeave();
            };

            comboMultiEditControl.Enter += (s, e) =>
            {
                comboMultiEditControl.BeginInvoke(new MethodInvoker(() =>
                {
                    comboMultiEditControl.SelectionStart = 0;
                    comboMultiEditControl.SelectionLength = comboMultiEditControl.Text.Length;
                }));
            };

            comboMultiEditControl.CloseUp += (s, e) =>
            {
                control.BeginInvoke(new MethodInvoker(delegate { SetClearButton(); }));
            };

            if (definition.ShowMultiSelectText != false)
            {
                comboMultiEditControl.CustomDisplayText += (s, e) =>
                {
                    GetCustomDisplayText(s, e);
                };
            }
        }

        private void SetButtons()
        {
            DevExpress.XtraEditors.Controls.EditorButton clearButton = new DevExpress.XtraEditors.Controls.EditorButton();
            clearButton.IsDefaultButton = false;
            clearButton.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Delete;
            clearButton.ToolTip = Common.Translator.TranslateString("Ukloni");

            clearButton.Click += (s, e) =>
            {
                OnClearButtonClick();
            };

            comboMultiEditControl.Properties.Buttons.Insert(0, clearButton);
            comboMultiEditControl.Properties.ActionButtonIndex = 1;
            comboMultiEditControl.Properties.Buttons[0].Enabled = false;
        }

        private void SetProperties()
        {
            comboMultiEditControl.EnterMoveNextControl = true;
            comboMultiEditControl.Properties.NullText = "...";
            comboMultiEditControl.Properties.NullValuePrompt = "...";
            comboMultiEditControl.Properties.NullValuePromptShowForEmptyValue = true;
            comboMultiEditControl.Properties.ShowNullValuePromptWhenFocused = true;
            comboMultiEditControl.Properties.IncrementalSearch = true;
        }

        private void OnClearButtonClick()
        {
            comboMultiEditControl.SetEditValue("");
            SetClearButton();
        }

        private void OnLeave()
        {
            SetClearButton();
        }

        private void GetCustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (comboMultiEditControl.EditValue != null)
            {
                string value = comboMultiEditControl.EditValue.ToString();
                selectedRowsCount = value.Count(x => x == ',');

                // T.A. - izmijenio prvi uvjet i dodao drugi jer je stavljao <označeno sve> string s null vrijednosti kada je 1 rezultat u DS-u -
                // selectedRowsCount == comboMultiEditControl.Properties.Items.Count - 1 je u tom slučaju zadovoljen, a ne treba biti
                if (comboMultiEditControl.Properties.Items.Count > 1 && selectedRowsCount == comboMultiEditControl.Properties.Items.Count - 1)
                    e.DisplayText = Common.Translator.TranslateString("<označeno sve>");
                else if (comboMultiEditControl.Properties.Items.Count == 1 && !string.IsNullOrEmpty(comboMultiEditControl.EditValue.ToString()))
                    return;
                else if (selectedRowsCount > 0)
                {
                    string tekstNaKontroli = Common.Translator.TranslateString("<višestruki odabir>");

                    if (!string.IsNullOrEmpty(definition.MultiSelectText))
                        tekstNaKontroli = definition.MultiSelectText;

                    e.DisplayText = tekstNaKontroli;
                }
            }
        }

        public void SetClearButton()
        {
            if (comboMultiEditControl.Properties.Buttons.Count > 1 && definition.ReadOnly != true)
                comboMultiEditControl.Properties.Buttons[0].Enabled = !Equals(comboMultiEditControl.EditValue, null) && !Equals(comboMultiEditControl.EditValue, "");
        }

        public void Populate(bool force)
        {
            if (force == true)
            {
                comboMultiEditControl.Properties.DataSource = null;
                comboMultiEditControl.SetEditValue(null);
                //ValueChanged();
            }
            if (form == null)
                return;
            if (comboMultiEditControl.Properties.DataSource != null)
                return;

            bool stopActions = false;
            Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);
            DataSet dataSet = dataDefinition.GetDataSet("punjenje kontrole '" + definition.Name + "'", out stopActions);
            if (dataSet == null || dataSet.Tables.Count == 0)
                return;
            else
            {
                string[] arrDtColumns = dataSet.Tables[0].Columns.Cast<DataColumn>()
                     .Select(x => x.ColumnName)
                     .ToArray();
                this.DataColumns = arrDtColumns;
            }

            // Isto kao comboedit (ComponentLookUpEdit): displayfield/valuefield iz XML-a mapiraju se na kolonu
            // iz rezultata bez obzira na velika/mala slova. Ako kolone nema, ostaje XML naziv (staro ponašanje) + Warn.
            string displayMember = Logic.Data.Core.FindDataColumn(definition.DisplayField, DataColumns);
            string valueMember = Logic.Data.Core.FindDataColumn(definition.ValueField, DataColumns);

            if (displayMember == null && !string.IsNullOrEmpty(definition.DisplayField))
            {
                form.Logger.Warn("Populate ComboMultiEdit - '" + definition.Name + "'",
                    "Kolona displayfield='" + definition.DisplayField + "' ne postoji u rezultatu upita. Kolone: " + string.Join(", ", DataColumns));
                displayMember = definition.DisplayField;
            }
            if (valueMember == null && !string.IsNullOrEmpty(definition.ValueField))
            {
                form.Logger.Warn("Populate ComboMultiEdit - '" + definition.Name + "'",
                    "Kolona valuefield='" + definition.ValueField + "' ne postoji u rezultatu upita. Kolone: " + string.Join(", ", DataColumns));
                valueMember = definition.ValueField;
            }

            comboMultiEditControl.Properties.DisplayMember = displayMember;
            comboMultiEditControl.Properties.ValueMember = valueMember;

            if (dataSet.Tables.Count > 0)
            {
                comboMultiEditControl.Properties.DataSource = dataSet.Tables[0];

                // ako ima samo 1 podatak, želimo ga selektirati
                if (dataSet.Tables[0].Rows.Count == 1)
                    comboMultiEditControl.SetValue(dataSet.Tables[0].Rows[0][comboMultiEditControl.Properties.ValueMember], dataSet.Tables[0].Rows[0][comboMultiEditControl.Properties.DisplayMember].ToString());
                else
                    SetValue(comboMultiEditControl as IJEditor);

                //if (this.Name == "lokacija")
                //{
                //    this.ShowPopup();

                //    if (dataSet.Tables[0].Rows.Count > 0)
                //        this.PopupForm.SelectedIndex = 0;
                //}
            }
        }

        public void OpenPopup()
        {
            if (comboMultiEditControl.Properties.DataSource != null)
            {
                System.Data.DataTable dt = ((System.Data.DataTable)comboMultiEditControl.Properties.DataSource);
                if (dt.Rows.Count == 1)
                {
                    comboMultiEditControl.SetEditValue(dt.Rows[0][comboMultiEditControl.Properties.ValueMember]);
                    comboMultiEditControl.SelectAll();
                }
                else
                {
                    if (form.IsLoadEvent != true)
                        comboMultiEditControl.ShowPopup();
                }
            }
        }
    }
}
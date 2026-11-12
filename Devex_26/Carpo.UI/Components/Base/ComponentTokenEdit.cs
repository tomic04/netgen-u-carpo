using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    /// <summary>B9: DX TokenEdit wrapper. EditValue is separator-joined string.</summary>
    public class ComponentTokenEdit : ComponentBase
    {
        private readonly JTokenEditControl tokenEdit;
        public string[] DataColumns { get; set; } = new string[] { };

        public ComponentTokenEdit() : base()
        {
        }

        public ComponentTokenEdit(Control control, object def) : base(control, def)
        {
            tokenEdit = (JTokenEditControl)control;
        }

        internal virtual void Init()
        {
            SetProperties();
            if (tokenEdit.Enabled)
            {
                Populate(false);
                SetValue(tokenEdit as IJEditor);
            }

            tokenEdit.Leave += (s, e) => { };
        }

        private void SetProperties()
        {
            var def = definition as JTokenEdit;
            tokenEdit.EnterMoveNextControl = true;

            char sep = ',';
            if (def != null && !string.IsNullOrEmpty(def.Separator) && def.Separator.Length > 0)
                sep = def.Separator[0];
            tokenEdit.Properties.EditValueSeparatorChar = sep;
            tokenEdit.Properties.Separators.Clear();
            tokenEdit.Properties.Separators.Add(sep.ToString());

            bool allowCustom = def != null && def.AllowCustom == true;
            tokenEdit.Properties.EditMode = allowCustom
                ? TokenEditMode.Default
                : TokenEditMode.TokenList;

            string nullPrompt = !string.IsNullOrEmpty(definition.NullText) ? definition.NullText : "...";
            tokenEdit.Properties.NullText = nullPrompt;

            if (allowCustom)
            {
                tokenEdit.ProcessNewValue += (s, e) =>
                {
                    string text = e.Text ?? Convert.ToString(e.Value);
                    if (string.IsNullOrWhiteSpace(text))
                        return;
                    bool exists = tokenEdit.Properties.Tokens.Any(t =>
                        string.Equals(Convert.ToString(t.Value), text, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(t.Description, text, StringComparison.OrdinalIgnoreCase));
                    if (!exists)
                        tokenEdit.Properties.Tokens.Add(new TokenEditToken(text, text));
                    e.Handled = true;
                };
            }

            // Static comboitem tokens (no dataname).
            if (def?.ComboItemList != null && def.ComboItemList.Count > 0)
            {
                tokenEdit.Properties.Tokens.Clear();
                foreach (var item in def.ComboItemList)
                {
                    string display = item.Text ?? item.Value;
                    string value = item.Value ?? item.Text;
                    if (!string.IsNullOrEmpty(display))
                        tokenEdit.Properties.Tokens.Add(new TokenEditToken(display, value));
                }
            }
        }

        public void Populate(bool force)
        {
            if (force)
            {
                tokenEdit.Properties.DataSource = null;
                tokenEdit.EditValue = null;
            }
            if (form == null)
                return;
            if (tokenEdit.Properties.DataSource != null)
                return;

            var def = definition as JTokenEdit;
            if (def?.ComboItemList != null && def.ComboItemList.Count > 0
                && string.IsNullOrEmpty(definition.DataName) && definition.Data == null)
                return;

            bool stopActions;
            var dataDefinition = new Logic.Data.DataDefinition(form, definition);
            DataSet dataSet = dataDefinition.GetDataSet("punjenje tokenedit '" + definition.Name + "'", out stopActions);
            if (dataSet == null || dataSet.Tables.Count == 0)
                return;

            DataColumns = dataSet.Tables[0].Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            tokenEdit.Properties.DisplayMember = Logic.Data.Core.FindDataColumn(definition.DisplayField, DataColumns);
            tokenEdit.Properties.ValueMember = Logic.Data.Core.FindDataColumn(definition.ValueField, DataColumns);
            tokenEdit.Properties.DataSource = dataSet.Tables[0];
            SetValue(tokenEdit as IJEditor);
        }
    }
}

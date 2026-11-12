using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentLookUpEdit : ComponentBase
    {
        #region LookUpEdit

        public System.Type ValueFieldColumnType { get; set; }
        private readonly LookUpEditBase jLookUpControl;
        public string[] DataColumns { get; set; } = new string[] { };
        private bool autoSuggestWired;

        public ComponentLookUpEdit() : base()
        {
        }

        public ComponentLookUpEdit(Control control, object def) : base(control, def)
        {
            jLookUpControl = control as LookUpEditBase;
        }

        internal virtual void Init()
        {
            DateTime startTime = DateTime.Now;
            SetButtons();

            SetProperties();
            // mislim da je ovo bila zabuna i da treba ici != false, a ne == false. Ne zelimo da se puni samo u slucaju
            // false permissiona. Maknuo sam i uvjet da se puni samo ako je enableana kontrola... želimo puniti i ako
            // je disableana, za slučaj ako se kroz akciju enablea kasnije
            if (PermissionEnabled != false)
            {
                Populate(false);
                SetValue(jLookUpControl as IJEditor);
            }

            jLookUpControl.Leave += (s, e) =>
            {
                OnLeave();
            };

            jLookUpControl.PreviewKeyDown += (s, e) =>
            {
                PreviewKeyDown(s as LookUpEdit, e);
            };

            jLookUpControl.Enter += (s, e) =>
            {
                jLookUpControl.BeginInvoke(new MethodInvoker(() =>
                {
                    jLookUpControl.SelectionStart = 0;
                    jLookUpControl.SelectionLength = jLookUpControl.Text.Length;
                }));
            };

            jLookUpControl.CloseUp += (s, e) =>
            {
                control.BeginInvoke(new MethodInvoker(delegate { SetClearButton(); }));
            };
            form.Logger.InfoData("Init LookUpEdit ", definition.Name, startTime);
        }

        private void OnLeave()
        {
            SetClearButton();
        }

        private void OnClearButtonClick()
        {
            jLookUpControl.EditValue = null;
            SetClearButton();
        }

        private void SetButtons()
        {
           
            if (definition.AllowNull != false)
            {
                DevExpress.XtraEditors.Controls.EditorButton clearButton = new DevExpress.XtraEditors.Controls.EditorButton();
                clearButton.IsDefaultButton = false;
                clearButton.Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Delete;
                clearButton.ToolTip = Common.Translator.TranslateString("Ukloni");

                clearButton.Click += (s, e) =>
                {
                    OnClearButtonClick();
                };

                jLookUpControl.Properties.Buttons.Insert(0, clearButton);
                jLookUpControl.Properties.ActionButtonIndex = 1;
                jLookUpControl.Properties.Buttons[0].Enabled = false;
            }
           
        }

        private void SetProperties()
        {
            jLookUpControl.EnterMoveNextControl = true;
            var lookUpDefEarly = definition as Structure.Models.Core.Base.JModelEditorLookUp;
            string nullPrompt = !string.IsNullOrEmpty(definition.NullText) ? definition.NullText : "...";
            jLookUpControl.Properties.NullText = nullPrompt;
            jLookUpControl.Properties.NullValuePrompt = nullPrompt;

            jLookUpControl.Properties.NullValuePromptShowForEmptyValue = true;
            jLookUpControl.Properties.ShowNullValuePromptWhenFocused = false;

            jLookUpControl.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            // Default true (historical Carpo). Opt-out: immediatepopup="false".
            jLookUpControl.Properties.ImmediatePopup = lookUpDefEarly == null || lookUpDefEarly.ImmediatePopup != false;

            if (definition.AllowNull == false)
                jLookUpControl.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            else
                jLookUpControl.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;

            if (jLookUpControl is GridLookUpEdit) // izdvajam jer <combotreeedit> nasljeđuje <comboedit> properties
            {
                GridLookUpEdit lookUp = jLookUpControl as GridLookUpEdit;

                if (definition.FilterMode != "startswith")
                    lookUp.Properties.PopupFilterMode = PopupFilterMode.Contains;
                lookUp.Properties.View.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
                lookUp.Properties.View.OptionsView.ShowIndicator = false;

                // Opt-in DX GridLookUp search mode (XML dxsearchmode); default leaves control as-is.
                var lookUpDef = definition as Structure.Models.Core.Base.JModelEditorLookUp;
                if (lookUpDef != null && !string.IsNullOrWhiteSpace(lookUpDef.DxSearchMode))
                {
                    switch (lookUpDef.DxSearchMode.Trim().ToLowerInvariant())
                    {
                        case "none":
                            lookUp.Properties.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.None;
                            break;
                        case "autocomplete":
                            lookUp.Properties.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoComplete;
                            break;
                        case "autosuggest":
                            lookUp.Properties.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSuggest;
                            WireAutoSuggest(lookUp);
                            break;
                        case "autosearch":
                        case "autofilter":
                            lookUp.Properties.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSearch;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// B10: DX AutoSuggest needs QuerySuggestions. Sets $DxLookupSearch for SQL params,
        /// re-fetches data (server filter if command uses it), then client-filters by display field.
        /// </summary>
        private void WireAutoSuggest(GridLookUpEdit lookUp)
        {
            if (autoSuggestWired)
                return;
            autoSuggestWired = true;
            lookUp.AutoSuggest += (s, e) =>
            {
                e.SetMinimumAnimationDuration(TimeSpan.FromMilliseconds(250));
                string text = e.Text ?? string.Empty;
                e.QuerySuggestions = QueryAutoSuggestAsync(text, e.CancellationToken);
            };
        }

        private Task<ICollection> QueryAutoSuggestAsync(string text, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                cancellation.ThrowIfCancellationRequested();
                FormsVar.SetVariable(form, "DxLookupSearch", text);

                bool stopActions;
                var dataDefinition = new Logic.Data.DataDefinition(form, definition);
                DataSet dataSet = dataDefinition.GetDataSet("autosuggest '" + definition.Name + "'", out stopActions);
                if (dataSet == null || dataSet.Tables.Count == 0)
                    return (ICollection)new DataView(new DataTable());

                DataTable dt = dataSet.Tables[0];
                string[] cols = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
                string display = Logic.Data.Core.FindDataColumn(definition.DisplayField, cols);
                if (string.IsNullOrEmpty(display) || string.IsNullOrEmpty(text))
                    return dt.DefaultView;

                var matches = dt.AsEnumerable().Where(r =>
                {
                    cancellation.ThrowIfCancellationRequested();
                    object v = r[display];
                    return v != null && v != DBNull.Value
                        && v.ToString().IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
                });
                if (!matches.Any())
                    return (ICollection)new DataView(dt.Clone());
                return matches.CopyToDataTable().DefaultView;
            }, cancellation);
        }

        public void Populate(bool force)
        {
            DateTime startTime = DateTime.Now;
            if (force == true)
            {
                jLookUpControl.Properties.DataSource = null;
                jLookUpControl.EditValue = null;
                //ValueChanged();
            }
            if (form == null)
                return;
            if (jLookUpControl.Properties.DataSource != null)
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

            jLookUpControl.Properties.DisplayMember = Logic.Data.Core.FindDataColumn(definition.DisplayField, DataColumns);
            jLookUpControl.Properties.ValueMember = Logic.Data.Core.FindDataColumn(definition.ValueField, DataColumns);
            if (!(string.IsNullOrEmpty(jLookUpControl.Properties.ValueMember)))
                ValueFieldColumnType = dataSet.Tables[0].Columns[jLookUpControl.Properties.ValueMember].DataType;

            if (dataSet.Tables.Count > 0)
            {
                jLookUpControl.Properties.DataSource = dataSet.Tables[0];
                CreateColumns();

                //SetValue(jLookUpControl as IJEditor);

                //if (this.Name == "lokacija")
                //{
                //    this.ShowPopup();

                //    if (dataSet.Tables[0].Rows.Count > 0)
                //        this.PopupForm.SelectedIndex = 0;
                //}
                form.Logger.InfoData("Populate LookUpEdit ", definition.Name, startTime);
            }
        }

        internal virtual void CreateColumns()
        {
            JLookUpControl lookUp = jLookUpControl as JLookUpControl;

            if (lookUp.Properties.View.Columns.Count > 0)
                return;
            lookUp.Properties.View.Columns.Clear();
            if (definition.Fields != null && definition.Fields.FieldList != null)
            {
                for (int i = 0; i < definition.Fields.FieldList.Count; i++)
                {
                    JField field = definition.Fields.FieldList[i];
                    lookUp.Properties.View.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = field.Name, Caption = field.Caption });
                    lookUp.Properties.View.Columns[i].VisibleIndex = i;
                }
            }
            else
            {
                lookUp.Properties.View.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn() { FieldName = Logic.Data.Core.FindDataColumn(definition.DisplayField, DataColumns) });
                lookUp.Properties.View.Columns[0].VisibleIndex = 0;
                lookUp.Properties.View.OptionsView.ShowColumnHeaders = false;
            }
        }

        internal void SetClearButton()
        {
            if (jLookUpControl.Properties.Buttons.Count > 1 && definition.ReadOnly != true && definition.AllowNull != false)
            {
                jLookUpControl.Properties.Buttons[0].Enabled =
                    jLookUpControl.EditValue != null
                    && jLookUpControl.EditValue != System.DBNull.Value;
            }
        }

        public void OpenPopup()
        {
            if (jLookUpControl.Properties.DataSource != null)
            {
                System.Data.DataTable dt = ((System.Data.DataTable)jLookUpControl.Properties.DataSource);
                if (dt.Rows.Count == 1)
                {
                    jLookUpControl.EditValue = dt.Rows[0][jLookUpControl.Properties.ValueMember];
                    jLookUpControl.SelectAll();
                }
                else
                {
                    if (form.IsLoadEvent != true)
                        jLookUpControl.ShowPopup();
                }
            }
        }

        #endregion LookUpEdit

        public object GetValue(string field)
        {
            DataRowView rowView = (DataRowView)jLookUpControl.GetSelectedDataRow();
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

        internal virtual void PreviewKeyDown(LookUpEdit le, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                OnClearButtonClick();
            }
            //    GridLookUpEdit edit = jLookUpControl as GridLookUpEdit;
            //    if (edit.Properties.View.DataSource == null)
            //    {
            //        MethodInfo activateMethod = typeof(RepositoryItemGridLookUpEdit).GetMethod("ActivateGridDataSource", (BindingFlags.NonPublic | BindingFlags.Instance));
            //        activateMethod.Invoke(edit.Properties, null);
            //    }

            //    if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Up))
            //    {
            //        int RowHandle = edit.Properties.GetIndexByKeyValue(edit.EditValue);

            //        if ((RowHandle < 0) && (e.KeyCode == Keys.Down))
            //            RowHandle = 0;
            //        else if (e.KeyCode == Keys.Down)
            //            RowHandle = (RowHandle + 1);
            //        else
            //            RowHandle = (RowHandle - 1);

            //        if (((RowHandle >= 0) && (RowHandle < edit.Properties.View.RowCount)))
            //            edit.EditValue = edit.Properties.GetKeyValue(RowHandle);

            //        if (e.KeyCode == Keys.Down)
            //            (edit as TextEdit).Select(edit.Text.Length, 0);
            //    }
        }
    }
}
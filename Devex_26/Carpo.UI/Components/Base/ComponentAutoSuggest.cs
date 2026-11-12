using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.Utils;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentAutoSuggest : ComponentBase
    {
        #region LookUpEdit

        public System.Type ValueFieldColumnType { get; set; }
        private readonly LookUpEdit jLookUpControl;
        public string[] DataColumns { get; set; } = new string[] { };

        private List<JField> definitionFields;




        public ComponentAutoSuggest() : base()
        {
        }

        public ComponentAutoSuggest(Control control, object def) : base(control, def)
        {
            definition = def as JAutoSuggest;

            jLookUpControl = control as LookUpEdit;
        }

        internal virtual void Init()
        {

            if (definition.NullText != null)
            {
                jLookUpControl.Properties.NullValuePrompt = definition.NullText;
                jLookUpControl.Properties.NullValuePromptShowForEmptyValue = true;
                jLookUpControl.Properties.ShowNullValuePromptWhenFocused = true;
            }

            if (!string.IsNullOrEmpty((definition as JAutoSuggest).FieldText))
            {
                jLookUpControl.Properties.DisplayMember = (definition as JAutoSuggest).FieldText;
            }
            
            definitionFields = PopulateFieldList();
            CreateColumns();




            //DateTime startTime = DateTime.Now;
            //SetButtons();

            //SetProperties();
            //// mislim da je ovo bila zabuna i da treba ici != false, a ne == false. Ne zelimo da se puni samo u slucaju
            //// false permissiona. Maknuo sam i uvjet da se puni samo ako je enableana kontrola... želimo puniti i ako
            //// je disableana, za slučaj ako se kroz akciju enablea kasnije
            //if (PermissionEnabled != false)
            //{
            //    Populate(false);
            //    SetValue(jLookUpControl as IJEditor);
            //}

            //jLookUpControl.Leave += (s, e) =>
            //{
            //    OnLeave();
            //};

            //(jLookUpControl as LookUpEdit).AutoSuggest += (s, e) =>
            //{
            //    // Set delay (if needed)
            //    e.SetMinimumAnimationDuration(TimeSpan.FromMilliseconds(1000));
            //    //// Set Custom Highlight Strategy
            //    //e.SetHighlightRanges(HighlightTags(e.Text));
            //    //// Create a Task that return suggestions
            //    //e.QuerySuggestions = WorldCities.QueryAsync(e.Text, e.CancellationToken);
            //};

            //jLookUpControl.PreviewKeyDown += (s, e) =>
            //{
            //    PreviewKeyDown(s as LookUpEdit, e);
            //};

            //jLookUpControl.Enter += (s, e) =>
            //{
            //    jLookUpControl.BeginInvoke(new MethodInvoker(() =>
            //    {
            //        jLookUpControl.SelectionStart = 0;
            //        jLookUpControl.SelectionLength = jLookUpControl.Text.Length;
            //    }));
            //};

            //jLookUpControl.CloseUp += (s, e) =>
            //{
            //    control.BeginInvoke(new MethodInvoker(delegate { SetClearButton(); }));
            //};
            //form.Logger.InfoData("Init LookUpEdit ", definition.Name, startTime);
        }


        private List<JField> PopulateFieldList()
        {
            bool checkIfResult = true;

            List<JField> fields = new List<JField>();
            if (definition.Fields != null)
            {
                foreach (var item in definition.Fields.FieldList)
                {
                    if (Conditions.CheckCondition(ref checkIfResult, item, form, null, false) == false)
                        continue;
                    fields.Add(item);
                }
            }
            return fields;
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

            if (dataSet.Tables.Count > 0)
            {
                if (this.form is Forms.Info)
                {
                    ((Forms.Info)(this.form)).PopulateNav(false);
                    //((Forms.Info)(this.form)).SrcPopupEditValueChanged();
                }

                DataTable dt = dataSet.Tables[0];

                jLookUpControl.Properties.DataSource = dt;
                DataRowView drv = dt.DefaultView[0];
                jLookUpControl.EditValue = drv;

                form.Logger.InfoData("Populate AutoSuggest ", definition.Name, startTime);
            }
        }



        private void CreateColumns()
        {
            if (definitionFields == null || definitionFields.Count == 0)
            {
                jLookUpControl.Properties.Columns.Add(new LookUpColumnInfo() { FieldName = jLookUpControl.Properties.DisplayMember, Caption = "Naziv" });
                return;
            }
                

            jLookUpControl.Properties.ShowHeader = true;

            foreach (JField col in definitionFields)
            {
                string fieldName = col.Name;

                LookUpColumnInfo column = new LookUpColumnInfo();

                column.FieldName = fieldName;
                if (!string.IsNullOrEmpty(col.Caption))
                {
                    using (Parser parser = new Parser(form, null))
                    {
                        string valueStr = col.Caption;
                        object value = parser.GetLogicPartValue(valueStr);

                        if (value != null)
                            column.Caption = value.ToString().Replace("\\n", "\r\n");
                        else
                            column.Caption = col.Caption.Replace("\\n", "\r\n");
                    }
                }
                else
                {
                    column.Caption = Common.Helpers.Strings.AddSpacesToSentence(fieldName);
                }
                    

                if (col.MaxWidth != null)
                    column.MaxWidth = (int)col.MaxWidth;


                jLookUpControl.Properties.Columns.Add(column);

                column.Visible = true;


                if (col.FormatString != null)
                {
                    if (col.FormatString.Length <= 2 && col.FormatString.Substring(1, 1).ToLower() == "n")
                    {
                        column.FormatString = col.FormatString;
                    }
                }
                    

                if (!string.IsNullOrEmpty(col.Align))
                {
                    if (col.Align.ToLower() == "left")
                        column.Alignment = HorzAlignment.Near;
                    else if (col.Align.ToLower() == "right")
                        column.Alignment = HorzAlignment.Far;
                    else if (col.Align.ToLower() == "center")
                        column.Alignment = HorzAlignment.Center;
                }

                if (col.FixWidth != null)
                {
                    column.Width = (int)col.FixWidth;   
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

    }
}
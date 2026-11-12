using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    public class Statemants
    {
        private enum FilterOperator
        { Equal, NotEqual, GreaterThen, LowerThen, GreaterOrEqualThen, LowerOrEqualThen };

        private enum ConditionalOperator
        { None, AndAlso, OrElse };

        public enum PartSide { Left, Right }

        private string logicIf = null;
        private Forms.Base.JupiterShared currentForm = null;
        private Control control = null;
        private object controlValue = null;

        public Statemants(string logicIf, Forms.Base.JupiterShared currentForm, Control control)
        {
            this.logicIf = logicIf;
            this.currentForm = currentForm;
            this.control = control;
        }

        public Statemants(string logicIf, Forms.Base.JupiterShared currentForm, object controlValue)
        {
            this.logicIf = logicIf;
            this.currentForm = currentForm;
            this.controlValue = controlValue;
        }

        public Statemants(string logicIf) //FieldName potrebe
        {
            this.logicIf = logicIf;
        }

        public bool CheckIf(bool isXmlGen = false, Dictionary<string, DataTable> dtList = null)
        {
            if (String.IsNullOrEmpty(logicIf))
                return true;

            List<Structure.Models.Operators> statemantList = new List<Structure.Models.Operators>();

            ConditionalOperator condOperator;
            string[] condStrList;

            FindCondDelimiter(logicIf, out condStrList, out condOperator);

            bool returnedStatemant = false;

            foreach (string condStr in condStrList)
            {
                string filterOperatorStr;
                FilterOperator filterOperator;

                FindDelimiter(condStr, out filterOperatorStr, out filterOperator);
                string[] parsLR = condStr.Split(new string[] { filterOperatorStr }, StringSplitOptions.None);

                if (parsLR.Length != 2)
                    return false;

                string leftPart = "", rightPart = "";

                leftPart = parsLR[0];
                rightPart = parsLR[1];

                object leftPartValue, rightPartValue;

                if (!isXmlGen)
                {
                    // A10: one Parser for both sides — paint path (RowCellStyle) hits this heavily.
                    using (Parser parser = new Parser(currentForm, control, controlValue))
                    {
                        leftPartValue = parser.GetLogicPartValue(leftPart);
                        rightPartValue = parser.GetLogicPartValue(rightPart);
                    }
                }
                else
                {
                    using (Parser parser = new Parser(dtList))
                    {
                        parser.IsXmlGen = true;
                        leftPartValue = parser.GetLogicPartValue(leftPart);
                        rightPartValue = parser.GetLogicPartValue(rightPart);
                    }
                }

                bool checkStatemant;

                if (leftPart == "key")
                {
                    string[] keyCodesLeft = null, keyCodesRight = null;

                    keyCodesLeft = leftPartValue.ToString().Split('+');
                    keyCodesRight = rightPartValue.ToString().Split('+');

                    bool equal = keyCodesLeft.Length == keyCodesRight.Length && new HashSet<string>(keyCodesLeft).SetEquals(keyCodesRight);

                    checkStatemant = equal;
                }
                else
                {
                    leftPartValue = GetValueByObject(filterOperator, leftPartValue);
                    rightPartValue = GetValueByObject(filterOperator, rightPartValue);

                    checkStatemant = CheckStatemant(filterOperator, leftPartValue, rightPartValue);
                }

                if (condOperator == ConditionalOperator.None)
                {
                    returnedStatemant = checkStatemant;
                    break;
                }
                else if (condOperator == ConditionalOperator.AndAlso)
                {
                    if (checkStatemant == true)
                        returnedStatemant = true;
                    else
                    {
                        returnedStatemant = false;
                        break;
                    }
                }
                else if (condOperator == ConditionalOperator.OrElse)
                {
                    if (checkStatemant == true)
                    {
                        returnedStatemant = true;
                        break;
                    }
                }
            }

            return returnedStatemant;
        }

        private object GetValueByObject(FilterOperator filterOperator, object objectValue)
        {
            // ako je objekt null vraćamo odmah null
            if (objectValue == null || objectValue == DBNull.Value)
                return null;

            // ako je string vrijednost null također vraćamo null
            string text = objectValue.ToString();
            if (text == "null")
                return null;

            // prvi pokušaj parsiranja je u numeričku (decimalnu) vrijednost
            object value = Common.Helpers.Conversion.Parse(typeof(decimal), text);
            if (value != null)
                return value;

            // ako objekt nije string vraćamo ga u izvornom obliku
            if (!(objectValue is string))
                return objectValue;

            //ako je jednako ili različito uvijek je string; jer i drugu stranu konvertiram u string
            //pripaziti da je i druga strana CaseSensitive > ToLower()
            if (filterOperator == FilterOperator.Equal || filterOperator == FilterOperator.NotEqual)
                return text.ToLower();

            // provjera - da li je vrijednost datum | EDIT :: do ovog koda nebi trebalo doći uopće jer će datumsku
            // vratit u redu 154, ali zasad neka ostane ovdje
            value = Common.Helpers.Conversion.Parse(typeof(DateTime), text);
            if (value != null)
                return value;

            // posljednja akcija, ako ništa nije zadovoljio vraća string
            return text.ToLower();
        }

        private bool CheckStatemant(FilterOperator filterOperator, object leftPartValue, object rightPartValue)
        {
            if (filterOperator == FilterOperator.GreaterThen ||
                filterOperator == FilterOperator.LowerThen ||
                filterOperator == FilterOperator.LowerOrEqualThen ||
                filterOperator == FilterOperator.GreaterOrEqualThen)
            {
                if (leftPartValue == null && rightPartValue == null)
                {
                    leftPartValue = 0;
                    rightPartValue = 0;
                }
                else if (leftPartValue == null || rightPartValue == null)
                    return false;

                //moraju biti istog tipa
                if (!(leftPartValue.GetType() == rightPartValue.GetType()))
                    return false;
                //moraju biti ili datumsko polje ili numeričko
                if (!(leftPartValue is DateTime || leftPartValue is decimal))
                    return false;

                if (filterOperator == FilterOperator.GreaterThen)
                {
                    if (leftPartValue is DateTime && ((DateTime)leftPartValue > (DateTime)rightPartValue))
                        return true;
                    if (leftPartValue is decimal && ((decimal)leftPartValue > (decimal)rightPartValue))
                        return true;
                }
                else if (filterOperator == FilterOperator.GreaterOrEqualThen)
                {
                    if (leftPartValue is DateTime && ((DateTime)leftPartValue >= (DateTime)rightPartValue))
                        return true;
                    if (leftPartValue is decimal && ((decimal)leftPartValue >= (decimal)rightPartValue))
                        return true;
                }
                else if (filterOperator == FilterOperator.LowerThen)
                {
                    if (leftPartValue is DateTime && ((DateTime)leftPartValue < (DateTime)rightPartValue))
                        return true;
                    if (leftPartValue is decimal && ((decimal)leftPartValue < (decimal)rightPartValue))
                        return true;
                }
                else if (filterOperator == FilterOperator.LowerOrEqualThen)
                {
                    if (leftPartValue is DateTime && ((DateTime)leftPartValue <= (DateTime)rightPartValue))
                        return true;
                    if (leftPartValue is decimal && ((decimal)leftPartValue <= (decimal)rightPartValue))
                        return true;
                }

                return false;
            }
            else
            {
                bool areEquals;
                areEquals = Object.Equals(leftPartValue, rightPartValue);
                if (areEquals == false && leftPartValue != null && rightPartValue != null)
                {
                    try
                    {
                        leftPartValue = leftPartValue.ToString();
                        rightPartValue = rightPartValue.ToString();
                    }
                    catch { }
                }

                if (Common.Helpers.Conversion.IsString(leftPartValue) && Common.Helpers.Conversion.IsString(rightPartValue))
                {
                    //provjera - numeričke vrijednosti
                    decimal outDecLeft, outDecRight = 0;
                    bool isNumLeft, isNumRight = false;
                    isNumLeft = decimal.TryParse(leftPartValue.ToString(), out outDecLeft);
                    if (isNumLeft == true)
                    {
                        isNumRight = decimal.TryParse(rightPartValue.ToString(), out outDecRight);
                    }
                    if (isNumLeft == true && isNumRight == true)
                        areEquals = decimal.Equals(outDecLeft, outDecRight);
                    else
                        areEquals = string.Equals((leftPartValue as string).Trim(), (rightPartValue as string).Trim(), StringComparison.OrdinalIgnoreCase);
                    //}
                    //else
                    //{
                    //    areEquals = Object.Equals(leftPartValue, rightPartValue);
                }

                if (filterOperator == FilterOperator.Equal && areEquals == true)
                    return true;
                else if (filterOperator == FilterOperator.NotEqual && areEquals == false)
                    return true;

                return false;
            }
        }

        public string GetDirectByPart(PartSide partSide, string logicIf, Parser.DirectType type)
        {
            string[] parsLR = logicIf.Split(new string[] { "==" }, StringSplitOptions.None);
            if (partSide == PartSide.Left)
                return Parser.GetDirectByPart(parsLR[0], type);
            else if (parsLR.Length > 1)
                return Parser.GetDirectByPart(parsLR[1], type);
            return null;
        }

        private void FindDelimiter(string logicIf, out string filterOperatorStr, out FilterOperator filterOperator)
        {
            filterOperatorStr = "==";
            filterOperator = FilterOperator.Equal;

            if (logicIf.Contains(@"!="))
            {
                filterOperatorStr = @"!=";
                filterOperator = FilterOperator.NotEqual;
            }
            else if (logicIf.Contains("<="))
            {
                filterOperatorStr = "<=";
                filterOperator = FilterOperator.LowerOrEqualThen;
            }
            else if (logicIf.Contains(">="))
            {
                filterOperatorStr = ">=";
                filterOperator = FilterOperator.GreaterOrEqualThen;
            }
            else if (logicIf.Contains("<"))
            {
                filterOperatorStr = "<";
                filterOperator = FilterOperator.LowerThen;
            }
            else if (logicIf.Contains(">"))
            {
                filterOperatorStr = ">";
                filterOperator = FilterOperator.GreaterThen;
            }
        }

        private void FindCondDelimiter(string logicIf, out string[] str, out ConditionalOperator cond)
        {
            str = new[] { logicIf };
            cond = ConditionalOperator.None;

            if (logicIf.Contains(@"&&"))
            {
                cond = ConditionalOperator.AndAlso;
                str = logicIf.Split(new[] { "&&" }, StringSplitOptions.None);
            }
            else if (logicIf.Contains("||"))
            {
                cond = ConditionalOperator.OrElse;
                str = logicIf.Split(new[] { "||" }, StringSplitOptions.None);
            }
        }

        //private bool LogicIf(FilterOperator filterOperator)
        //{
        //    if (logicIf == null)
        //        return true;

        //    string[] parsLR = logicIf.Split(new string[] { "==" }, StringSplitOptions.None);

        //    if (parsLR.Length != 2)
        //        return false;

        //    string leftPart = "", rightPart = "";

        //    leftPart = parsLR[0];
        //    rightPart = parsLR[1];

        //    using (Parser parser = new Parser(currentForm, control))
        //    {
        //        object leftPartValue = parser.GetLogicPartValue(leftPart);
        //        object rightPartValue = parser.GetLogicPartValue(rightPart);

        //        if (leftPartValue.Equals(rightPartValue))
        //            return true;

        //        return false;
        //    }
        //}

        //private object GetLogicPartValue(string logicPart)
        //{
        //    string controlName = "";
        //    string[] parsStr = logicPart.Split(new string[] { "." }, StringSplitOptions.None);

        //    for (int i = 0; i < parsStr.Length; i++)
        //    {
        //        string str = parsStr[i];
        //        if (i == 0 && str.StartsWith("#")) //naziv kontrole
        //        {
        //            controlName = str.Replace("#", "");
        //        }
        //        else if (i == 0 && str.StartsWith("@"))
        //        {
        //            object value = FindOutValueBySecondPart(str);
        //            return value;
        //        }
        //        else if (i == 0 && parsStr.Length == 1) //ako nije kontrola, postavi vrijednost
        //        {
        //            return str;
        //        }
        //        else if (i > 0 && controlName != "") //ako postoji više definicija (npr. #control.level == 2), saznaj o kojoj se konotroli radi;
        //        {
        //            control = Controls.GetControl(currentForm, controlName);
        //            object value = FindOutValueBySecondPart(str);
        //            return value;
        //        }
        //    }
        //    if (controlName != "")
        //    {
        //        object value = Controls.GetControlValue(currentForm, controlName, "");

        //        if (value == null)
        //        {
        //            if (currentForm is Forms.Base.JupiterShared)
        //            {
        //                List<Models.IJControlDefinition> controlList = (currentForm as Forms.Base.JupiterShared).JControlList;
        //                foreach (var controlDefinition in controlList)
        //                {
        //                    if (controlDefinition is Models.JRepositoryDefinition)
        //                    {
        //                        return (controlDefinition as Models.JRepositoryDefinition).Value;
        //                        //Models.JRepositoryDefinition repositoryDefinition = ((Models.JRepositoryDefinition)controlDefinition);
        //                        //if (repositoryDefinition.JControl is DevExpress.XtraEditors.Repository.RepositoryItemComboBox)
        //                        //{
        //                        //    RepositoryItemComboBox repositoryItemComboBox = (repositoryDefinition.JControl as DevExpress.XtraEditors.Repository.RepositoryItemComboBox);

        //                        //    if (repositoryItemComboBox != null)
        //                        //    {
        //                        //        //object edit = (repositoryItemComboBox as ComboBoxEdit);
        //                        //        return "VremenskaPodjela";
        //                        //    }

        //                        //    // DevExpress.XtraEditors.Repository.RepositoryItem item = (repositoryDefinition.JControl as );
        //                        //}
        //                    }
        //                }
        //            }

        //        }

        //        return value;
        //    }

        //    return null;
        //}

        //private object FindOutValueBySecondPart(string definition)
        //{
        //    definition = definition.Replace("@", "");

        //    if (definition.ToLower() == "level" && control is JTreeListControl)
        //    {
        //        return (control as JTreeListControl).FocusedNode.Level.ToString();
        //    }
        //    else if (control is JGridControl)
        //    {
        //        GridView gridView = ((control as JGridControl).MainView as GridView);
        //        return gridView.GetDataRow(gridView.FocusedRowHandle)[definition];
        //    }
        //    return null;
        //}

        #region OnlyFieldName

        //private string GetDirectByPart(string logicPart, DirectType type)
        //{
        //    string[] parsStr = logicPart.Split(new string[] { "." }, StringSplitOptions.None);

        //    for (int i = 0; i < parsStr.Length; i++)
        //    {
        //        string str = parsStr[i];
        //        if (str.StartsWith("@") && type == DirectType.FieldName)
        //        {
        //            return str.Replace("@", "");
        //        }
        //        else if (str.StartsWith("#") && type == DirectType.ControlName)
        //        {
        //            return str.Replace("@", "");
        //        }
        //        else if (!Regex.IsMatch(str, "^(@|#)") && type == DirectType.Value)
        //            return str;
        //    }

        //    return null;
        //}

        #endregion OnlyFieldName
    }
}
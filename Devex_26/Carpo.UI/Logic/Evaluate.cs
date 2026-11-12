using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    public class Evaluate
    {
        private Forms.Base.JupiterShared form;
        private string expression;
        private bool isXmlGen = false;
        private DataRow dataRow;
        public Dictionary<string, DataTable> ForwardedDataTable { get; set; }

        public Evaluate(Form form, string expression)
        {
            this.form = form as Forms.Base.JupiterShared;
            this.expression = expression;
        }

        public Evaluate(Dictionary<string, DataTable> forwardedDataTable, string expression)
        {
            isXmlGen = true;
            ForwardedDataTable = forwardedDataTable;
            this.expression = expression;
        }

        public Evaluate(DataRow dataRow, string expression)
        {
            this.expression = expression;
            this.dataRow = dataRow;
        }

        public object Expression()
        {
            if (!expression.Contains("{"))
                return expression;

            List<Match> matches = GetSubStrings(expression, "{", "}").ToList();
            foreach (Match match in matches.OrderByDescending(x => x.Index))
            {
                string expressionStr = match.ToString().Replace("{", "").Replace("}", "");

                var expressionSplit = expressionStr.Split(new string[] { "::" }, StringSplitOptions.None);
                expressionStr = expressionSplit[0];
                string format = "";
                if (expressionSplit.Count() > 1)
                {
                    format = expressionSplit[1];
                }

                object value = null;
                if (dataRow != null)
                    value = ExpressionDataRow(expressionStr);
                else
                    value = ExpressionString(expressionStr);

                string valueStr = "";
                if (value != null)
                {
                    if (!string.IsNullOrEmpty(format))
                    {
                        if (format.Length <= 2 && format.Substring(0, 1).ToLower() == "n")
                        {
                            string helper = value.ToString().Replace(".", ",");
                            decimal valueDec = 0;
                            try
                            {
                                valueDec = Convert.ToDecimal(helper);
                            }
                            catch
                            {
                                form.Logger.Warn("Evaluate.Expression", "Neuspješno parsiranje vrijednosti!");
                            }
                            valueStr = string.Format("{0:" + format + "}", valueDec);
                        }
                        else
                            valueStr = string.Format("{0:" + format + "}", value);
                    }
                    else
                    {
                        if (!(value is byte[]))
                            valueStr = value.ToString().Trim();
                        else
                        {
                            valueStr = Convert.ToBase64String(value as byte[]);
                        }
                    }
                }

                string matchStr = match.ToString();
                expression = SubsituteString(expression, match.Index, match.Length, valueStr);
            }

            return expression;
        }

        private object ExpressionDataRow(string expression)
        {
            expression = expression.Replace("@", "");
            if (!dataRow.Table.Columns.Contains(expression))
                return expression;

            return dataRow[expression];
        }

        private object ExpressionString(string expression)
        {
            if (!isXmlGen)
            {
                using (Parser parser = new Parser(form, null))
                {
                    return parser.GetLogicPartValue(expression);
                };
            }
            else
            {
                using (Parser parser = new Parser(ForwardedDataTable))
                {
                    parser.IsXmlGen = true;
                    return parser.GetLogicPartValue(expression);
                };
            }
        }

        public override string ToString()
        {
            object value = Expression();
            if (value == null)
                return "";
            else
                return value.ToString();
        }

        //public string Expression(DataRow dataRow, string expression)
        //{
        //    if (!expression.Contains("{"))
        //        return expression;

        //    List<Match> matches = GetSubStrings(expression, "{", "}").ToList();
        //    foreach (Match match in matches.OrderByDescending(x => x.Index))
        //    {
        //        string field = match.ToString().Replace("{", "").Replace("}", "").Replace("@", "");

        //        if (!dataRow.Table.Columns.Contains(field))
        //            return expression;

        //        object value = dataRow[field];
        //        string valueStr = "";
        //        if (value != null)
        //            valueStr = value.ToString();
        //        string matchStr = match.ToString();
        //        expression = SubsituteString(expression, match.Index, match.Length, valueStr);
        //    }

        //    return expression;
        //}

        //public static string Expression(Forms.Base.JupiterShared form, string expression)
        //{
        //    List<Match> matches = GetSubStrings(expression, "{", "}").ToList();
        //    using (Parser parser = new Parser(form, null))
        //    {
        //        foreach (Match match in matches.OrderByDescending(x => x.Index))
        //        {
        //            object value = parser.GetLogicPartValue(match.ToString().Replace("{", "").Replace("}", ""), null);
        //            string valueStr = "";
        //            if (value != null)
        //                valueStr = value.ToString();
        //                string matchStr = match.ToString();
        //                expression = SubsituteString(expression, match.Index, match.Length, valueStr);
        //            }
        //        }

        //    return expression;
        //}

        private IEnumerable<Match> GetSubStrings(string input, string start, string end)
        {
            //Regex r = new Regex(Regex.Escape(start) + "(.*?)" + Regex.Escape(end));
            Regex r = new Regex(start + "(.*?)" + end);
            MatchCollection matches = r.Matches(input);
            foreach (Match match in matches)
                yield return match;
        }

        private string SubsituteString(string OriginalStr, int index, int length, string SubsituteStr)
        {
            return new StringBuilder(OriginalStr).Remove(index, length).Insert(index, SubsituteStr).ToString();
        }
    }
}
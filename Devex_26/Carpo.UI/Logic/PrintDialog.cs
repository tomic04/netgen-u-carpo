using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Carpo.UI.Logic
{
    internal class PrintDialog
    {
        public PrintDialog(Forms.Base.JupiterShared form, List<Structure.Models.Print> printDefinition)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                if (printDefinition.Count == 1)
                {
                    using (var clsPrint = new Spin.Printing.Start())
                    {
                        clsPrint.Print(form.ConnectionString, printDefinition[0].Template, printDefinition[0].Parameters, "Carpo");
                    }
                }
                else if (printDefinition.Count > 1)
                {
                    var rowsParamsList = new List<string>();

                    foreach (var p in printDefinition)
                    {
                        rowsParamsList.Add(p.Parameters);
                    }

                    using (var clsPrint = new Spin.Printing.Start())
                    {
                        clsPrint.PrintMulti(form.ConnectionString, printDefinition[0].Template, rowsParamsList, "Carpo");
                    }
                }
                string template = printDefinition.Count > 0 ? printDefinition[0].Template : "";
                form.Logger?.Performance("PrintDialog",
                    $"Print template={template} jobs={printDefinition.Count}", sw, "Print");
            }
            catch (Exception ex)
            {
                form.Logger.Error("method: PrintDialog", ex.Message, ex);
            }
        }
    }
}
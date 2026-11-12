using Carpo.Common.Helpers;
using Carpo.Structure.Models;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    public class VarsLayout
    {
        private Forms.Base.JupiterShared form;
        private string version;

        public VarsLayout(Forms.Base.JupiterShared form)
        {
            this.form = form;
        }

        public VarsLayout(Forms.Base.JupiterShared form, string version)
        {
            this.form = form;
            this.version = version;
        }

        public void SaveValue(string name, object value, string componentType, bool isSearchForm = false)
        {
            if (value != null)
            {
                string valueStr = value.ToString();

                string sqlText;
                Carpo.Data.Set.Vars.SetSource(form.ConnectionString, Core.PubVars.OperateriId, form.ModuleId, form.FormName, name, componentType, valueStr, null, version, out sqlText, isSearchForm);
            }
        }

        public void SaveXml(Component component, string componentType, string componentName = null, string previousLayoutName = "", Stream stream = null)
        {
            if (!string.IsNullOrEmpty(previousLayoutName))
            {
                using (Parser parser = new Parser(form, null))
                {
                    previousLayoutName = parser.GetLogicPartValue(previousLayoutName).ToString();
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                string name;
                if (component is GridView)
                {
                    name = (component as GridView).GridControl.Name;
                    (component as GridView).SaveLayoutToStream(ms);
                }
                else if (component is TreeList)
                {
                    name = (component as TreeList).Name;
                    (component as TreeList).SaveLayoutToStream(ms);
                }
                else
                    return;
                StreamReader reader = null;

                var tempStream = ((MemoryStream)stream)?.ToArray();

                ms.Seek(0, System.IO.SeekOrigin.Begin);
                if (tempStream != null && tempStream?.Length != 0) { stream.Seek(0, SeekOrigin.Begin); reader = new StreamReader(stream); }
                else reader = new StreamReader(ms);
                string text = reader.ReadToEnd().Replace("utf-8", "utf-16");

                var isSearchForm = false;
                if (componentName != null)
                {
                    name = componentName;
                    isSearchForm = true;    // ako je primio komponent name onda je search forma, jedino se od tamo prosljeđuje taj parametar
                }

                string sqlText;
                Carpo.Data.Set.Vars.SetSource(form.ConnectionString, Core.PubVars.OperateriId, form.ModuleId, form.FormName, name, componentType, null, text, version, out sqlText, isSearchForm, previousLayoutName);
            }
        }

        public string GetFormSizeStr()
        {
            if (form.WindowState == FormWindowState.Maximized)
                return "maximized";
            else if (form.WindowState != FormWindowState.Minimized)
            {
                return form.Width.ToString() + ";" + form.Height.ToString();
            }
            return null;
        }

        public object SetFormSize()
        {
            if (form.CarpoVarsLayout == null || form.CarpoVarsLayout.Count == 0)
                return null;

            var formSizeStr = form.CarpoVarsLayout.Where(x => x.Name.ToLower() == "formsize").FirstOrDefault();
            if (formSizeStr == null)
                return null;

            if (formSizeStr.Value.ToLower() == "maximized")
                return "maximized";

            string[] formSizeArr = formSizeStr.Value.ToString().Split(';');
            if (formSizeArr.Length == 2)
            {
                int? width = formSizeArr[0].CoalesceInt();
                int? height = formSizeArr[1].CoalesceInt();
                if (width != null && height != null)
                    return new System.Drawing.Size((int)width, (int)height);
            }
            return null;
        }

        public void SetLayout()
        {
            var startTime1 = DateTime.Now;
            if (form.ModuleId == null)
                return;

            string formName = form.DefinitionForm.Name;
            if (string.IsNullOrEmpty(formName))
                return;

            string sqlText;
            DateTime startTime = DateTime.Now;
            DataSet ds = Carpo.Data.Get.Vars.GetBySource(form.ConnectionString, Core.PubVars.OperateriId, form.ModuleId, formName, out sqlText);
            form.Logger.InfoData("data reader; CarpoVars", sqlText, startTime);

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string version = null;
                    if (ds.Tables[0].Columns.Contains("Version"))
                        version = dr["Version"].ToString();

                    var control = form.CarpoVarsLayout.Where(x => x.Name == (dr["ComponentName"]).ToString()).FirstOrDefault();
                    if (control != null && control.Name.Contains("#") && form.CarpoVarsLayout.Contains(control))
                        form.CarpoVarsLayout.Remove(form.CarpoVarsLayout.Where(x => x.Name == control.Name).FirstOrDefault());

                    if (form.CarpoVarsLayout.Contains(control))
                        continue;
                    else
                    {
                        form.CarpoVarsLayout.Add(new Structure.Models.Vars()
                        {
                            Name = dr["ComponentName"].ToString(),
                            Type = dr["ComponentType"].ToString(),
                            Value = dr["Value"].ToString(),
                            ValueXml = dr["ValueXml"].ToString(),
                            Version = version
                        });
                    }
                }
            }
            form.Logger.InfoTest("SetLayout", "učitavanje layouta", startTime1);
        }

        public void SetLayout(string controlName, string layoutName = "", string previousLayoutName = "")
        {
            try
            {
                Control[] control = Logic.Controls.FindControl(form, controlName);
                if (control.Length > 0)
                {
                    using (Parser parser = new Parser(form, null))
                    {
                        previousLayoutName = parser.GetLogicPartValue(previousLayoutName).ToString();
                        layoutName = parser.GetLogicPartValue(layoutName).ToString();
                    }
                    Vars cVar = null;
                    // set new layout grida (dynamic)
                    if (!string.IsNullOrEmpty(layoutName))
                    {
                        SetLayout();
                        cVar = form.CarpoVarsLayout.Where(x => x.Name == controlName + "#" + layoutName).FirstOrDefault();
                    }
                    // restore layout grida na initu forme
                    else if (!string.IsNullOrEmpty(previousLayoutName))
                    {
                        cVar = form.CarpoVarsLayout.Where(x => x.Name == controlName + "#" + previousLayoutName).FirstOrDefault();
                    }
                    else
                        cVar = form.CarpoVarsLayout.Where(x => x.Name == controlName).FirstOrDefault();

                    if (cVar == null)

                        cVar = form.CarpoVarsLayout.Where(x => x.Name == controlName).FirstOrDefault();

                    if (cVar != null)
                    {
                        if (string.IsNullOrEmpty(version) || cVar.Version == version)
                        {
                            var cGrid = control[0] as GridControl;
                            if (cGrid != null)
                            {
                                if (!string.IsNullOrEmpty(cVar.ValueXml))
                                {
                                    MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(cVar.ValueXml.ToString()));
                                    cGrid.DefaultView.RestoreLayoutFromStream(mStream);
                                }
                                //cGrid.DefaultView.RestoreLayoutFromXml(cVar.ValueXml.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: CarpoVars.SetLayout", ex.Message, ex);
            }
        }

        public string GetLayout(string controlName, string layoutName = "", string previousLayoutName = "")
        {
            try
            {
                Control[] control = Logic.Controls.FindControl(form, controlName);
                if (control.Length > 0)
                {
                    using (Parser parser = new Parser(form, null))
                    {
                        previousLayoutName = parser.GetLogicPartValue(previousLayoutName).ToString();
                        layoutName = parser.GetLogicPartValue(layoutName).ToString();
                    }
                    Vars cVar = null;
                    // set new layout grida (dynamic)
                    if (!string.IsNullOrEmpty(layoutName))
                    {
                        SetLayout();
                        cVar = form.CarpoVarsLayout.Where(x => x.Name == controlName + "#" + layoutName).FirstOrDefault();
                    }
                    // restore layout grida na initu forme
                    else if (!string.IsNullOrEmpty(previousLayoutName))
                    {
                        cVar = form.CarpoVarsLayout.Where(x => x.Name == controlName + "#" + previousLayoutName).FirstOrDefault();
                    }
                    else
                        cVar = form.CarpoVarsLayout.Where(x => x.Name == controlName).FirstOrDefault();

                    if (cVar == null)

                        cVar = form.CarpoVarsLayout.Where(x => x.Name == controlName).FirstOrDefault();

                    if (cVar != null)
                    {
                        if (string.IsNullOrEmpty(version) || cVar.Version == version)
                        {
                            var cGrid = control[0] as GridControl;
                            if (cGrid != null)
                            {
                                return cVar.ValueXml;
                            }
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                form.Logger.Error("event: CarpoVars.GetLayout", ex.Message, ex);
                return "";
            }
        }


        public void SetSplitters(List<Vars> vars)
        {
            foreach (var var in vars)
            {
                Control[] control = Logic.Controls.FindControl(form, var.Name);
                object value = var.Value;

                int i;
                if (value != null && int.TryParse(value.ToString(), out i))
                {
                    if (control.Length > 0)
                    {
                        var cSplitter = control[0] as SplitContainerControl;
                        cSplitter.SplitterPosition = Convert.ToInt32(value);
                    }
                }
            }
        }
    }
}
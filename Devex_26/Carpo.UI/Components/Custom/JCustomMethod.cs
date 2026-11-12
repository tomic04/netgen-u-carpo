using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Carpo.UI.Components
{
    public class JCustomMethod
    {
        private JCustomMethodDef definition;
        public string Name { get; set; }
        public string ModuleId { get; set; }
        private Forms.Base.JupiterShared form;

        private Dictionary<string, object> variablesGlobal = new Dictionary<string, object>();
        private Dictionary<string, object> variablesForm = new Dictionary<string, object>();

        //output values
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        //input values
        public List<JEventActionParameter> Parameters { get; set; } = new List<JEventActionParameter>();

        private Assembly assembly;
        private Type type;

        public JCustomMethod(JCustomMethodDef definition, Forms.Base.JupiterShared form)
        {
            this.definition = definition;
            this.Name = definition.Name;
            this.form = form;
        }

        internal void Run()
        {
            variablesForm = new Dictionary<string, object>();
            variablesGlobal = new Dictionary<string, object>();

            if (assembly == null)
                assembly = Assembly.Load(definition.Dll);

            if (type == null)
                type = assembly.GetType(definition.Class);

            if (type != null)
            {
                MethodInfo methodInfo = null;

                try
                {
                    // provjeri sve metode XML definiranog imena u XML definiranoj klasi
                    var methods = from method in type.GetMethods()
                                  where method.Name == definition.Method
                                  select method;

                    if (methods.Count() == 1)
                        methodInfo = type.GetMethod(definition.Method);
                    else if (methods.Count() > 1) // overloads
                    {
                        try
                        {
                            methodInfo = type.GetMethod(definition.Method, GetParametersTypesList().ToArray());
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
                catch
                {
                    throw;
                }

                if (methodInfo != null)
                {
                    object result = null;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    object classInstance = Activator.CreateInstance(type, null);

                    SetJupiterProperties(classInstance);

                    List<object> parametersArray = new List<object>();
                    if (parameters.Length == 0)
                    {
                        //This works fine
                        result = methodInfo.Invoke(classInstance, null);
                    }
                    else
                    {
                        //object[] parametersArray = new object[] { 1, (decimal)12.12, (decimal)14.12 };
                        //object[] parametersArray = new object[] { };
                        bool checkIfResult = true;

                        foreach (var par in parameters)
                        {
                            if (Parameters.Any(s => s.Name.Equals(par.Name, StringComparison.OrdinalIgnoreCase) && (s.Type == null || s.Type.ToLower() != "var")))
                            {
                                var p = Parameters.FindAll(s => s.Name.ToLower() == par.Name.ToLower());
                                if (p.Count > 0)
                                {
                                    foreach (var pp in p)
                                    {
                                        if (Conditions.CheckCondition(ref checkIfResult, pp, form, null) == false)
                                            continue;

                                        string valueType = pp.Type;
                                        string valueStr = pp.Value;

                                        using (Parser parser = new Parser(FormsVar.GetActiveForm(form), null))
                                        {
                                            object value = parser.GetLogicPartValue(valueStr, null);
                                            value = Common.Helpers.Conversion.Parse(par.ParameterType.Name, value);
                                            parametersArray.Add(value);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                parametersArray.Add(JupiterParameter(par.Name));
                            }
                        }

                        SetCarpoVariables(classInstance);

                        try
                        {
                            PropertyInfo pLogger = type.GetProperty("Logger");

                            if (pLogger != null)
                            {
                                pLogger.SetValue(classInstance, form.Logger, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        // SetProperties - mora biti prije Invoke-a zbog form.ShowDialog(). Sa Show() radi super jer nastavi odrađivati
                        // invoke dalje, ali sa ShowDialog() ne-> kada nakon zatvaranja invokeane forme pokuša setat propertye, tada je instanca
                        // već disposeana i puca
                        SetProperties(classInstance, methodInfo, parametersArray);                        


                        result = methodInfo.Invoke(classInstance, parametersArray.ToArray());

                        if (result != null && form != null && !string.IsNullOrEmpty(Name)) // ako metoda vraća vrijednost automatski kreiramo varijablu
                        {
                            FormsVar.SetVariable(form, Name, result);
                        }

                        // zbog mijenjanja vrijednosti tokom rada na custom DLL-ovima morali smo ponovo resetati parametre
                        SetOrResetProperties(classInstance);
                    }
                }
            }
        }

        private List<Type> GetParametersTypesList()
        {
            List<Type> typesList = new List<Type>();

            for (int i = 0; i < this.Parameters.Count; i++)
            {
                if (this.Parameters[i].Type == null)
                {
                    form.Logger.Warn("GetParametersTypesList()", "Niste definirali tip parametra - " + this.Parameters[i].Name);
                    typesList = null;
                    break;
                }
                else
                {
                    if (this.Parameters[i].Type.ToLower() == "int")
                        typesList.Add(typeof(int));
                    else if (this.Parameters[i].Type.ToLower() == "string")
                        typesList.Add(typeof(string));
                    else if (this.Parameters[i].Type.ToLower() == "short")
                        typesList.Add(typeof(short));
                    else if (this.Parameters[i].Type.ToLower().Contains("bool"))
                        typesList.Add(typeof(bool));
                    else if (this.Parameters[i].Type.ToLower() == "decimal")
                        typesList.Add(typeof(decimal));
                    else
                        typesList.Add(typeof(string));
                }
            }
            return typesList;
        }

        private void SetJupiterProperties(object classInstance)
        {
            var jupiterPar = Parameters.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower() == "moduleid");
            if (jupiterPar != null && jupiterPar.Count() > 0)
            {
                FieldInfo fi = GetField(type, "moduleId");
                if (fi == null)
                {
                    PropertyInfo pi = GetProperty(type, "moduleId");
                    if (pi != null)
                        pi.SetValue(classInstance, jupiterPar.FirstOrDefault().Value, null);
                }
            }
        }

        private void SetCarpoVariables(object classInstance)
        {
            if (Parameters.Count == 0)
                return;

            var vars = Parameters.FindAll(s => s.Type != null && s.Type.ToLower() == "var");
            if (vars.Count > 0)
            {
                foreach (var v in vars)
                {
                    object value;
                    using (Parser parser = new Parser(form, null))
                    {
                        value = parser.GetLogicPartValue(v.Value);
                        value = Common.Helpers.Conversion.Parse(v.ValueType, value);
                    }

                    SetVariableValue(v.Name, value);
                }

                if (variablesGlobal.Count > 0)
                {
                    PropertyInfo pi = GetProperty(type, "VariablesGlobal");
                    if (pi != null)
                        pi.SetValue(classInstance, variablesGlobal, null);
                }
                if (variablesForm.Count > 0)
                {
                    PropertyInfo pi = GetProperty(type, "VariablesForm");
                    if (pi != null)
                        pi.SetValue(classInstance, variablesForm, null);
                }
            }
        }

        private void SetVariableValue(string name, object value)
        {
            bool isParent = false, isGlobal = false;
            name = Parser.CheckScope(name, out isParent, out isGlobal);

            if (name.StartsWith("$"))
                name = name.Substring(1);

            Dictionary<string, object> dic;
            if (isGlobal == true)
                dic = variablesGlobal;
            else
                dic = variablesForm;

            if (dic.ContainsKey(name))
                dic[name] = value;
            else
                dic.Add(name, value);
        }

        private object JupiterParameter(string name)
        {
            //if (name.ToLower() == "parent")
            //    return form.ParentHandle;
            if (name.ToLower() == "ptop" || name.ToLower() == "pleft")
                return (Int16)10;

            return Variables.GetPubVar("@@" + name, form);
        }

        internal static FieldInfo GetField(Type t, string fieldName)
        {
            if (t == null)
                return null;
            try
            {
                FieldInfo f = t.GetField(fieldName);
                if (f != null)
                    return f;

                foreach (FieldInfo fi in t.GetFields())
                {
                    if (fi.Name.ToLower() == fieldName.ToLower())
                        return fi;
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        internal static PropertyInfo GetProperty(Type t, string propertyName)
        {
            if (t == null)
                return null;
            try
            {
                PropertyInfo p = t.GetProperty(propertyName);
                if (p != null)
                    return p;

                foreach (PropertyInfo pi in t.GetProperties())
                {
                    if (pi.Name.ToLower() == propertyName.ToLower())
                        return pi;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void SetProperties(object classInstance, MethodInfo methodInfo, List<object> parametersArray)
        {
            SetOrResetProperties(classInstance);

            foreach (var par in Parameters)
            {
                if (!string.IsNullOrEmpty(par.Type) && par.Type.ToLower() == "property")
                {
                    try
                    {
                        SetProperty(classInstance, par.Name, par.Value);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void SetOrResetProperties(object classInstance)
        {
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Properties.Clear();

            foreach (PropertyInfo prp in props)
            {
                if (prp.GetGetMethod() != null)
                {
                    object value = prp.GetValue(classInstance, new object[] { });
                    Properties.Add(prp.Name, value);
                }
            }
        }

        private void SetProperty(object classInstance, string name, object value)
        {
            PropertyInfo methodProp = type.GetProperty(name);

            if (methodProp != null && methodProp.GetSetMethod() != null)
            {
                methodProp.SetValue(classInstance, value, null);
            }
        }

        internal void SetParameters(JEventAction action)
        {
            Parameters.Clear();
            if (action.Parameter != null && action.Parameter.Count > 0)
            {
                Parameters.AddRange(action.Parameter);
            }
        }

        public object GetValue(string propertyName)
        {
            if (Properties.ContainsKey(propertyName))
            {
                return Properties[propertyName];
            }
            return null;
        }
    }
}
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Carpo.UI.Components.Custom
{
    internal class JCustomtForm
    {
        private JCustomMethodDef definition;
        public string Name { get; set; }
        private Forms.Base.JupiterShared form;

        //output values
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        //input values
        public List<JEventActionParameter> Parameters { get; set; } = new List<JEventActionParameter>();

        private Assembly assembly;
        private Type type;

        public JCustomtForm(JCustomMethodDef definition, Forms.Base.JupiterShared form)
        {
            this.definition = definition;
            this.Name = definition.Name;
            this.form = form;
        }

        internal void Run()
        {
            if (assembly == null)
                assembly = Assembly.Load(definition.Dll);

            if (type == null)
                type = assembly.GetType(definition.Class);

            if (type != null)
            {
                MethodInfo methodInfo = type.GetMethod(definition.Method);
                if (methodInfo != null)
                {
                    object result = null;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    object classInstance = Activator.CreateInstance(type, null);
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
                        List<object> parametersArray = new List<object>();
                        foreach (var par in parameters)
                        {
                            var varC = Parameters.Any(s => s.Name.Equals(par.Name, StringComparison.OrdinalIgnoreCase));

                            if (Parameters.Any(s => s.Name.Equals(par.Name, StringComparison.OrdinalIgnoreCase)))
                            {
                                var p = Parameters.FindAll(s => s.Name.ToLower() == par.Name.ToLower());
                                if (p.Count > 0)
                                {
                                    //string valueType = Parameters.FirstOrDefault(s => s.Name.ToLower() == par.Name.ToLower()).Type;
                                    //string valueStr = Parameters.FirstOrDefault(s => s.Name.ToLower() == par.Name.ToLower()).Value;

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
                        }

                        //The invoke does NOT work it throws "Object does not match target type"
                        result = methodInfo.Invoke(classInstance, parametersArray.ToArray());
                    }

                    PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                    Properties.Clear();
                    foreach (PropertyInfo prp in props)
                    {
                        object value = prp.GetValue(classInstance, new object[] { });
                        Properties.Add(prp.Name, value);
                    }
                }
            }
        }

        internal void SetParameters(JEventAction action)
        {
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
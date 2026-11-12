using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    public class Finder
    {
        public static bool FindForm(string className, out object foundClass)
        {
            foundClass = null;

            if (!String.IsNullOrEmpty(className))
            {
                foreach (Form frm in Application.OpenForms)
                {
                    if (frm.GetType().Name.ToUpper() == className.ToUpper())
                    {
                        foundClass = frm;
                        return true;
                    }
                }
            }
            return false;
        }

        public Form TryGetFormByName(string formName)
        {
            var formType = Assembly.GetExecutingAssembly().GetTypes()
                .Where(a => a.BaseType == typeof(Form) && a.Name == formName)
                .FirstOrDefault();

            if (formType == null) // If there is no form with the given frmname
                return null;

            return (Form)Activator.CreateInstance(formType);
        }
    }
}
using Carpo.UI.Components;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Logic
{
    public class Methods
    {
        public static JCustomMethod GetMethod(Forms.Base.JupiterShared form, string methodName)
        {
            Form formActive = FormsVar.GetActiveForm(form);

            if (form is Forms.Base.JupiterShared)
            {
                var customObject = (formActive as Forms.Base.JupiterShared).CustomMethods.Where(x => x.Name.ToLower() == methodName.ToLower()).FirstOrDefault();
                return customObject;
            }
            return null;
        }
    }
}
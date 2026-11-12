using System.Reflection;
using System.Windows.Forms;

namespace Carpo.Common.Helpers
{
    public class Controls
    {
        //omogućuje svim kontrolama aktiviranje protexted propertya "DubleBuffered"
        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }
    }
}
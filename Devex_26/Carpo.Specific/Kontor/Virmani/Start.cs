using Carpo.Core;

namespace Carpo.Specific.Kontor.Virmani
{
    public class Start
    {
        public Logger Logger { get; set; }

        public void VirmaniWithId(string connectionstring, int ssoperateriid, int izbornik, int action, int id)
        {
            Run(connectionstring, ssoperateriid, izbornik, action, id);
        }

        public void VirmaniWithoudId(string connectionstring, int ssoperateriid, int izbornik, int action)
        {
            Run(connectionstring, ssoperateriid, izbornik, action, null);
        }

        private bool Run(string connectionstring, int ssoperateriid, int izbornik, int action, int? id)
        {
            using (Virmani virmani = new Virmani())
            {
                virmani.Logger = Logger;
                virmani.ConnectionString = connectionstring;
                virmani.SSOperateriId = ssoperateriid;
                virmani.Izbornik = izbornik;
                virmani.Action = action;

                if (izbornik == 2) // Virmani
                {
                    if (action == 0) // unos
                        virmani.VirmaniVrsteId = (int)id;
                    else // izmijena
                        virmani.VirmaniId = (int)id;
                }
                else if ((izbornik == 1) && (action == 1)) // Virmani vrste izmijena
                    virmani.VirmaniVrsteId = (int)id;

                virmani.ShowDialog();

                return true;
            }
        }
    }
}
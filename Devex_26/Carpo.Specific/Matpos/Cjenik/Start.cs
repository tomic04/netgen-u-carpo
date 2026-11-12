using Carpo.Core;

namespace Carpo.Specific.Matpos.Cjenik
{
    public class Start
    {
        public Logger Logger { get; set; }

        public void Run(string connectionstring, int ssoperateriid, int robaId, int skladisteId, decimal nabCijena, int nabpriglaid)
        {
            using (Cjenik cjenik = new Cjenik())
            {
                cjenik.ConnectionString = connectionstring;
                cjenik.Logger = Logger;
                cjenik.SSOperateriId = ssoperateriid;
                cjenik.robaId = robaId;
                cjenik.skladisteId = skladisteId;
                cjenik.nabCijena = nabCijena;
                cjenik.nabpriglaid = nabpriglaid;

                cjenik.ShowDialog();
            }
        }
    }
}
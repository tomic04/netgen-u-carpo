using System.Data.SqlClient;

namespace Carpo.Structure.Models
{
    public class ConnWithTran
    {
        public SqlConnection Connection { get; set; }
        public SqlTransaction Transaction { get; set; }
    }
}
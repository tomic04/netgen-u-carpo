using Carpo.Data.Data.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.Data.Data.Service
{
    public class SettingsService
    {
        SqlContext context;
        public SettingsService(SqlContext context)
        {
            this.context = context;
        }
        public DataTable LoadSettings(int operatorID, string menuID)
        {
            return context.ExecuteStoredProcedure("spCarpoFilterPostavke", new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("action", 1),
                new KeyValuePair<string, object>("operaterid", operatorID),
                new KeyValuePair<string, object>("meniid", menuID)
            });
        }
        public void SaveSettings(string nodeID, int operatorID, string menuID, byte[] settings)
        {
            context.ExecuteNonQuery("spCarpoFilterPostavke", new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("action", 2),
                new KeyValuePair<string, object>("cvorid", nodeID),
                new KeyValuePair<string, object>("operaterid", operatorID),
                new KeyValuePair<string, object>("postavke", settings),
                new KeyValuePair<string, object>("meniid", menuID)
            });
        }
        public void DeleteSettings(int operatorID, string menuID)
        {
            context.ExecuteStoredProcedure("spCarpoFilterPostavke", new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("action", 3),
                new KeyValuePair<string, object>("operaterid", operatorID),
                new KeyValuePair<string, object>("meniid", menuID)
            });
        }
    }
}

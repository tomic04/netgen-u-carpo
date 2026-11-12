using System.Data.SqlClient;

namespace Carpo.Data
{
    public class Constructor
    {
        private static int defaultTimeout = 10;

        public static string SetConnectionString(string server, string database, string userId, string password)
        {
            return SetConnectionString(server, database, userId, password, "", defaultTimeout);
        }

        public static string SetConnectionString(string server, string database, string userId, string password, int timeout)
        {
            return SetConnectionString(server, database, userId, password, "", timeout);
        }

        public static string SetConnectionString(string server, string database, string userId, string password, string applicationName)
        {
            return SetConnectionString(server, database, userId, password, applicationName, defaultTimeout);
        }

        public static string SetConnectionString(string server, string database, string userId, string password, string applicationName, int timeout)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.ApplicationName = applicationName;
            builder.ConnectTimeout = timeout;
            builder.DataSource = server;
            builder.InitialCatalog = database;

            if (string.IsNullOrEmpty(userId))
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = userId;
                builder.Password = password;
            }

            return builder.ToString();
        }
    }
}
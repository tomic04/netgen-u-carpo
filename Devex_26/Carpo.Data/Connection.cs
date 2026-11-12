using System;

namespace Carpo.Data
{
    public class Connection
    {
        [ThreadStatic]
        private static string connectionString;
        [ThreadStatic]
        private static string connectionStringADO;
        [ThreadStatic]
        private static string connStrResources;
        [ThreadStatic]
        private static string dataBase;
        [ThreadStatic]
        private static string server;
        [ThreadStatic]
        private static string uid;
        [ThreadStatic]
        private static string pwd;
        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
        public static string ConnectionStringADO
        {
            get { return connectionStringADO; }
            set { connectionStringADO = value; }
        }
        public static string ConnStrResources
        {
            get { return connStrResources; }
            set { connStrResources = value; }
        }
        public static string DataBase
        {
            get { return dataBase; }
            set { dataBase = value; }
        }
        public static string Server
        {
            get { return server; }
            set { server = value; }
        }
        public static string Uid
        {
            get { return uid; }
            set { uid = value; }
        }
        public static string Pwd
        {
            get { return pwd; }
            set { pwd = value; }
        }
    }
}
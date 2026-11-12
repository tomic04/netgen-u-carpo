using System;

namespace Carpo.Core
{
    public class PubVars
    {
        [ThreadStatic]
        private static int operateriId;
        [ThreadStatic]
        private static bool dmsExists;
        [ThreadStatic]
        private static string menuID; //= "455";
        public static int OperateriId
        {
            get { return operateriId; }
            set { operateriId = value; }
        }
        public static bool DmsExists
        {
            get { return dmsExists; }
            set { dmsExists = value; }
        }
        public static string MenuID
        {
            get { return menuID; }
            set { menuID = value; }
        }
    }
}
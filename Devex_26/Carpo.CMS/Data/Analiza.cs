using System.Data;
using System.Data.SqlClient;

namespace Carpo.CMS.Data
{
    internal class Analiza
    {
        public static void InsertElement(string element)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoDsAnalizaAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Action", "insertElement");
                    command.Parameters.AddWithValue("element", element);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void InsertAttribute(string element, string attribute)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoDsAnalizaAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Action", "insertAttribute");
                    command.Parameters.AddWithValue("element", element);
                    command.Parameters.AddWithValue("attribute", attribute);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void InsertEvent(string element, string eventType)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoDsAnalizaAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Action", "insertEvent");
                    command.Parameters.AddWithValue("element", element);
                    command.Parameters.AddWithValue("event", eventType);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void InsertActionType(string value)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoDsAnalizaAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Action", "insertActionType");
                    command.Parameters.AddWithValue("value", value);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
    }
}
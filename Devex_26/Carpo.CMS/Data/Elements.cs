using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Carpo.CMS.Data
{
    internal class Elements
    {
        public static DataTable GetElements(int? carpoDsAttributeId)
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getelements" },
                    { "@CarpoDsAttributeId", carpoDsAttributeId}
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetElement(int? carpoDsElementId)
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getelement" },
                    { "@CarpoDsElementId", carpoDsElementId}
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetElementsForParent(int? carpoDsElementId)
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getElementsForParent" },
                    { "@CarpoDsElementId", carpoDsElementId}
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetParentsForElement(int? carpoDsElementId)
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getParentsForElement" },
                    { "@CarpoDsElementId", carpoDsElementId}
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetAttributes(int carpoDsElementId)
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getattributes" },
                    { "@carpoDsElementId", carpoDsElementId }
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetTypes()
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "gettypes" }
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        public static DataTable GetElementTypes()
        {
            try
            {
                string spName = "spCarpoCmsElementsQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "getelementtypes" }
                };

                string sqlText = "";
                DataSet ds = Carpo.Data.Common.ExecuteQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText);
                if (ds.Tables.Count > 0)
                    return ds.Tables[0];

                return null;
            }
            catch
            {
                throw;
            }
        }

        #region Azur

        public static void SetElemAttr(int carpoDsElementid, int carpoDsAttributeId, bool chkValue)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsElementsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Action", "setElemAttr");
                    command.Parameters.AddWithValue("carpoDsElementid", carpoDsElementid);
                    command.Parameters.AddWithValue("carpoDsAttributeId", carpoDsAttributeId);
                    command.Parameters.AddWithValue("chkValue", chkValue);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void SetElemParent(int carpoDsElementIdParent, int carpoDsElementid, bool chkValue)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsElementsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Action", "setElemParent");
                    command.Parameters.AddWithValue("carpoDsElementid", carpoDsElementid);
                    command.Parameters.AddWithValue("carpoDsElementIdParent", carpoDsElementIdParent);
                    command.Parameters.AddWithValue("chkValue", chkValue);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void SetAttr(string attrName)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsElementsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Action", "setAttr");
                    command.Parameters.AddWithValue("Name", attrName);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void SetElem(string elemName, string description, string title, byte[] contentImg, byte[] contentFile, string contentFileName, int? elementTypeId)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsElementsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Action", "setElem");
                    command.Parameters.AddWithValue("Name", elemName);
                    command.Parameters.AddWithValue("Title", title);
                    command.Parameters.AddWithValue("Description", description);
                    command.Parameters.AddWithValue("ContentImg", contentImg);
                    command.Parameters.AddWithValue("ContentFile", contentFile);
                    command.Parameters.AddWithValue("ContentFileName", contentFileName);
                    command.Parameters.AddWithValue("CarpoDsElementTypeId", elementTypeId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void UpdateElem(string elemName, int carpoDsElementId, string description, string title, byte[] contentImg, byte[] contentFile, string contentFileName, int? elementTypeId)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsElementsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Action", "updateElem");
                    command.Parameters.AddWithValue("Name", elemName);
                    command.Parameters.AddWithValue("CarpoDsElementId", carpoDsElementId);
                    command.Parameters.AddWithValue("Title", title);
                    command.Parameters.AddWithValue("Description", description);
                    command.Parameters.AddWithValue("ContentImg", contentImg);
                    command.Parameters.AddWithValue("ContentFile", contentFile);
                    command.Parameters.AddWithValue("ContentFileName", contentFileName);
                    command.Parameters.AddWithValue("CarpoDsElementTypeId", elementTypeId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void UpdateAttr(string attrName, int carpoDsAttributeId)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsElementsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Action", "updateAttr");
                    command.Parameters.AddWithValue("Name", attrName);
                    command.Parameters.AddWithValue("CarpoDsAttributeId", carpoDsAttributeId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void DelElem(int carpoDsElementId, int carpoDsAttributeId)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsElementsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Action", "delElem");
                    command.Parameters.AddWithValue("CarpoDsElementId", carpoDsElementId);
                    command.Parameters.AddWithValue("CarpoDsAttributeId", carpoDsAttributeId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void DelAttr(int carpoDsAttributeId, int carpoDsElementId)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsElementsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Action", "delAttr");
                    command.Parameters.AddWithValue("CarpoDsAttributeId", carpoDsAttributeId);
                    command.Parameters.AddWithValue("CarpoDsElementId", carpoDsElementId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void setAttrDescValue(int carpoDsAttributeId, string description, string value, int typeId)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsElementsAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Action", "setAttrDescValue");
                    command.Parameters.AddWithValue("carpoDsAttributeId", carpoDsAttributeId);
                    command.Parameters.AddWithValue("description", description);
                    command.Parameters.AddWithValue("value", value);
                    if (typeId != 0)
                        command.Parameters.AddWithValue("typeId", typeId);
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        #endregion Azur
    }
}
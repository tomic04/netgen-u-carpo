using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Carpo.CMS.Data
{
    internal class Examples
    {
        private const int CommandTimeoutValue = 20;

        public static DataTable Tree()
        {
            try
            {
                string spName = "spCarpoCmsExampleQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "tree" },
                    { "@sender", "cms" }
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

        public static DataTable Files(int carpoCmsExampleId)
        {
            try
            {
                string spName = "spCarpoCmsExampleQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "files" },
                    { "@CarpoCmsExampleId", carpoCmsExampleId }
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

        public static DataTable Images(int carpoCmsExampleId)
        {
            try
            {
                string spName = "spCarpoCmsExampleQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "images" },
                    { "@CarpoCmsExampleId", carpoCmsExampleId }
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

        public static DataTable Image(int carpoCmsExampleImageId)
        {
            try
            {
                string spName = "spCarpoCmsExampleQuery";
                Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "image" },
                    { "@carpoCmsExampleImageId", carpoCmsExampleImageId }
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

        #region azur

        public static void AddFile(int carpoCmsExampleId, byte[] content, string contentType, string fileName)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsExampleAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Action", "addFile");
                    command.Parameters.AddWithValue("CarpoCmsExampleId", carpoCmsExampleId);
                    command.Parameters.AddWithValue("Content", content);
                    command.Parameters.AddWithValue("ContentType", contentType);
                    command.Parameters.AddWithValue("FileName", fileName);

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        public static void AddImage(int carpoCmsExampleId, byte[] content, byte[] contentThumb, string contentType, string fileName)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsExampleAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Action", "addImage");
                    command.Parameters.AddWithValue("CarpoCmsExampleId", carpoCmsExampleId);
                    command.Parameters.AddWithValue("Content", content);
                    command.Parameters.AddWithValue("ContentThumb", contentThumb);
                    command.Parameters.AddWithValue("ContentType", contentType);
                    command.Parameters.AddWithValue("FileName", fileName);

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        public static void DeleteExample(int carpoCmsExampleId)
        {
            string spName = "spCarpoCmsExampleAzur";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "deleteExample" },
                    { "@carpoCmsExampleId", carpoCmsExampleId}
                };

            string sqlText = "";
            Carpo.Data.Common.ExecuteNonQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 20);
        }

        public static void DeleteFile(int carpoCmsExampleFileId)
        {
            string spName = "spCarpoCmsExampleAzur";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "deleteFile" },
                    { "@carpoCmsExampleFileId", carpoCmsExampleFileId}
                };

            string sqlText = "";
            Carpo.Data.Common.ExecuteNonQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 20);
        }

        public static void DeleteImage(int carpoCmsExampleImageId)
        {
            string spName = "spCarpoCmsExampleAzur";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "deleteImage" },
                    { "@carpoCmsExampleImageId", carpoCmsExampleImageId}
                };

            string sqlText = "";
            Carpo.Data.Common.ExecuteNonQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, 20);
        }

        public static void EditExampleDescription(int carpoCmsExampleId, string description)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsExampleAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("action", "editExampleDescription");
                    command.Parameters.AddWithValue("carpoCmsExampleId", carpoCmsExampleId);
                    command.Parameters.AddWithValue("description", Common.Helpers.Db.SetDbValue(description));
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        public static void EditExampleName(int carpoCmsExampleId, string name)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsExampleAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("action", "editExampleName");
                    command.Parameters.AddWithValue("carpoCmsExampleId", carpoCmsExampleId);
                    command.Parameters.AddWithValue("name", name);
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        public static void EditExampleVisible(int carpoCmsExampleId, bool visible)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsExampleAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("action", "editExampleVisible");
                    command.Parameters.AddWithValue("carpoCmsExampleId", carpoCmsExampleId);
                    command.Parameters.AddWithValue("visible", visible);
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        public static void EditFileName(int carpoCmsExampleFileId, string name)
        {
            SqlConnection conn = new SqlConnection(Carpo.Data.Connection.ConnectionString);
            using (var connection = conn)
            {
                using (var command = new SqlCommand("spCarpoCmsExampleAzur", connection))
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("Action", "editFileName");
                    command.Parameters.AddWithValue("carpoCmsExampleFileId", carpoCmsExampleFileId);
                    command.Parameters.AddWithValue("Name", name);
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        public static void EditImageName(int carpoCmsExampleImageId, string name)
        {
            string spName = "spCarpoCmsExampleAzur";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "editImageName" },
                    { "@name", name },
                    { "@carpoCmsExampleImageId", carpoCmsExampleImageId }
                };

            string sqlText = "";
            Carpo.Data.Common.ExecuteNonQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, CommandTimeoutValue);
        }

        public static void AddExample(int? carpoCmsExampleId, string name)
        {
            string spName = "spCarpoCmsExampleAzur";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "addExample" },
                    { "@name", name },
                    { "@parentId", Common.Helpers.Db.SetDbValue(carpoCmsExampleId)}
                };

            string sqlText = "";
            Carpo.Data.Common.ExecuteNonQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, CommandTimeoutValue);
        }

        #region sort

        public static void SortFile(int carpoCmsExampleFileId, string sortDirection)
        {
            string spName = "spCarpoCmsExampleAzur";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "fileSort" },
                    { "@carpoCmsExampleFileId", carpoCmsExampleFileId },
                    { "@SortDirection", sortDirection },
                };

            string sqlText = "";
            Carpo.Data.Common.ExecuteNonQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, CommandTimeoutValue);
        }

        public static void SortImage(int carpoCmsExampleImageId, string sortDirection)
        {
            string spName = "spCarpoCmsExampleAzur";
            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                    { "@action", "imageSort" },
                    { "@carpoCmsExampleImageId", carpoCmsExampleImageId },
                    { "@SortDirection", sortDirection },
                };

            string sqlText = "";
            Carpo.Data.Common.ExecuteNonQuery(Carpo.Data.Connection.ConnectionString, spName, Carpo.Data.Common.DataType.StoredProcedure, parameters, out sqlText, CommandTimeoutValue);
        }

        #endregion sort

        #endregion azur
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carpo.Data.Data.Headers;
using Carpo.Data.Data.Extensions;

namespace Carpo.Data.Data.Context
{
    public class SqlContext
    {
        string connectionString;
        public SqlContext(string connectionString)
        {
            this.connectionString = connectionString;
        }
        internal DataTable ExecuteStoredProcedure(string storedProcedureName, KeyValuePair<string, object>[] parameters = null)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = CreateCommand(storedProcedureName, CommandType.StoredProcedure, connection, parameters);

            connection.Open();

            var dataTable = new DataTable();
            using var reader = command.ExecuteReader();
            dataTable.Load(reader);

            return dataTable;
        }
        internal Dictionary<string, string> ExecuteReader(string storedProcedureName, KeyValuePair<string, object>[] parameters = null)
        {
            var dictonary = new Dictionary<string, string>();
            using var connection = new SqlConnection(connectionString);
            using var command = CreateCommand(storedProcedureName, CommandType.StoredProcedure, connection, parameters);

            connection.Open();

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string original = reader.GetString(1).ToLower();
                string translation = reader.IsDBNull(2) ? null : reader.GetString(2);
                dictonary[original] = translation;
            }

            connection.Close();
            return dictonary;
        }
        public T ExecuteScalar<T>(string storedProcedureName, KeyValuePair<string, object>[] parameters)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = CreateCommand(storedProcedureName, CommandType.StoredProcedure, connection, parameters);

            connection.Open();

            var result = command.ExecuteScalar();
            if (Convert.IsDBNull(result)) return default;
            connection.Close();

            return (T)result;
        }
        internal int ExecuteNonQuery(string storedProcedureName, KeyValuePair<string, object>[] parameters)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = CreateCommand(storedProcedureName, CommandType.StoredProcedure, connection, parameters);

            connection.Open();
            var rows = command.ExecuteNonQuery();

            return rows;
        }
        public T ExecuteReader<T>(string storedProcedureName, KeyValuePair<string, object>[] parameters = null, CommandType type = CommandType.StoredProcedure) where T : class, new()
        {
            using var connection = new SqlConnection(connectionString);
            using var command = CreateCommand(storedProcedureName, type, connection, parameters);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read()) return null;

            T result = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));
                string columnName = columnAttribute?.Name ?? property.Name;

                if (!reader.HasColumn(columnName) || reader[columnName] == DBNull.Value) continue;

                property.SetValue(result, reader[columnName]);
            }

            return result;
        }
        //public IEnumerable<T> ExecuteReader<T>(string storedProcedureName, KeyValuePair<string, object>[] parameters = null) where T : class, new()
        //{
        //    using var connection = new SqlConnection(connectionString);
        //    using var command = CreateCommand(storedProcedureName, CommandType.StoredProcedure, connection, parameters);

        //    connection.Open();
        //    using var reader = command.ExecuteReader();

        //    var dataset = new List<T>();
        //    while (reader.Read())
        //    {
        //        var item = new T();
        //        var properties = typeof(T).GetProperties();

        //        foreach (var property in properties)
        //        {
        //            var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));
        //            string columnName = columnAttribute?.Name ?? property.Name;

        //            if (!reader.HasColumn(columnName) || reader[columnName] == DBNull.Value) continue;

        //            property.SetValue(item, reader[columnName]);
        //        }
        //        dataset.Add(item);
        //    }

        //    return dataset;
        //}
        private SqlCommand CreateCommand(string storedProcedureName, CommandType type, SqlConnection connection, KeyValuePair<string, object>[] parameters)
        {
            // Caller owns disposal (typically via using at call site). Do not wrap in using here.
            var command = new SqlCommand(storedProcedureName, connection);
            command.CommandType = type;

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    command.Parameters.AddWithValue($"@{parameters[i].Key}", parameters[i].Value ?? DBNull.Value);
                }
            }
            return command;
        }
    }
}

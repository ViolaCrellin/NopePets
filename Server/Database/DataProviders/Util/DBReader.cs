using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Server.MasterData.Model;

namespace Server.Database.DataProviders.Util
{
    // ReSharper disable once InconsistentNaming
    public static class DBReader
    {
        public static IList<T> ReadAll<T>(Func<IDataReader, T> constructor, string connectionStr) where T : new()
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                var query = new Query<T>().SelectAll();

                var command = new SqlCommand(query, connection);
                connection.Open();
                return Read(command, constructor).ToArray();
            }
         }

        public static IList<T> ReadById<T>(Func<IDataReader, T> constructor, string connectionStr, int id) where T : new()
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                var query = new Query<T>().SelectById(id);

                var command = new SqlCommand(query, connection);
                connection.Open();
                return Read(command, constructor).ToArray();
            }
        }

        public static IList<ColumnInfo<T>> ReadColumns<T>(string connectionStr) where T : new()
        {
            using (var connection = new SqlConnection(connectionStr))
            {
                var columnInfoList = new List<ColumnInfo<T>>();
                connection.Open();
                var query = new Query<T>().SelectAll();
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var info = new ColumnInfo<T>(reader.GetName(i), reader.GetDataTypeName(i), reader.GetName(i).Contains("Id"));
                    columnInfoList.Add(info);
                }
                return columnInfoList;
            }
        }

        private static DbDataReader ExecuteReader(SqlCommand command)
        {
            try
            {
                var result = command.ExecuteReader();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing READER command '" + command.CommandText + "'", ex);
            }
        }


        private static IEnumerable<T> Read<T>(SqlCommand command, Func<IDataReader, T> constructor) where T: new()
        {
            using (var reader = ExecuteReader(command))
            {
                var rowRead = false;
                do
                {
                    T result = default(T);
                    try
                    {
                        rowRead = reader.Read();
                        if (rowRead)
                        {
                           result = constructor(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error executing QUERY command '" + command.CommandText + "'", ex);
                    }
                    if (rowRead)
                    {
                        yield return result;
                    }
                    else
                    {
                        yield break;
                    }
                }
                while (rowRead);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.WebSockets;
using Server.Configuration;
using Server.Database.DataProviders.Util;
using Server.MasterData.Model;

namespace Server.Database.DataProviders
{
    public interface IDataProvider
    {

    }

    public interface IDataProvider<T> : IDataProvider
    {
        Func<IDataReader, T> DataMapper { get; } 
        IList<T> LoadAll();
        IList<T> LoadById(int id);

        IList<ColumnInfo<T>> LoadAllColumns();
    }

    public class DataProvider<T> : IDataProvider<T> where T : new()
    {
        private readonly string _connectionStr;

        //I thought it might be helpful if we wanted to 
        //split up the databases to have this level between the data reader and the provider.
        //This way each data provider can have a different configuration and point to a different Db
        public DataProvider(IConfiguration config, Func<IDataReader, T> dataMapper)
        {
            _connectionStr = config.DbConnectionString;
            DataMapper = dataMapper;
        }

        public Func<IDataReader, T> DataMapper { get; }

        public IList<T> LoadAll()
        {
            return DBReader.ReadAll(DataMapper, _connectionStr);
        }

        public IList<T> LoadById(int id)
        {
            return DBReader.ReadById(DataMapper, _connectionStr, id);
        }

        public IList<ColumnInfo<T>> LoadAllColumns()
        {
            return DBReader.ReadColumns<T>(_connectionStr);
        }

    }
}
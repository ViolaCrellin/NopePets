using System.Collections.Generic;
using System.Data.SqlClient;
using Server.Configuration;
using Server.Database.DataProviders.Util;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;

namespace Server.Database.DataPersisters
{
    public interface IRecordPersister
    {

    }

    public interface IRecordPersister<T> : IRecordPersister
    {
        bool TryPersist(ref T data, out ErrorMessage error);
        bool TryPersistUpdate(T petMetric, out ErrorMessage error);
    }

    //Todo = we could change this structurally so we have record persisters of different types based on key constraints
    // Join table, primary data and secondary data
    public abstract class RecordPersister<T> : IRecordPersister<T> where T : new()
    {

        protected RecordPersister(IList<ColumnInfo<T>> columnInfo, IConfiguration config)
        {
        }

        public abstract bool TryPersist(ref T data, out ErrorMessage error);

        public abstract bool TryPersistUpdate(T data, out ErrorMessage error);
    }
}

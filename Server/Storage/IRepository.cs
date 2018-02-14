using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.Database.DataProviders;
using Server.Database.DataProviders.Util;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.Model;

namespace Server.Storage
{

    public interface IRepository<T1, T2> 
        where T1 : IDatabaseRow 
        where T2 : IDatabaseJoinRow
    {
        IList<ColumnInfo<T1>> ColumnInfo { get; }
        IList<ColumnInfo<T2>> JoinTableColumnInfo { get; }

        T1 Find(int id);
        IList<T1> FindMany(IList<int> ids);
        IList<int> FindAllPrimaryIds();
         
        T2 FindAssociatedItem(T1 thing);
        IList<T2> FindAssociatedById(int thingId);
        IList<T2> FindAssociated(T1 thing);
        IList<T1> FindAssociated(T2 thing);
        IList<T1> FindAssociated(IList<T2> thing);
        IList<T2> FindAssociated(IList<T1> thing);
        IList<int> GetAssociatedIds(int id);
        IList<int> GetAssociatedIds(IList<int> ids);

        void Add(T1 data);
        void AddAssociated(IList<T2> things);
        void AddAssociated(T2 thing);
        void UpdateAssociated(int primaryId, int secondaryId, T2 update);
        void Update(T1 update);

        T1 GetUserByEmail(string userEmail);
        bool TryFindUserByEmail(string email, out T1 foundUser);
    }

    public class Repository<T1, T2> : IRepository<T1, T2> 
        where T1 : IDatabaseRow
        where T2 : IDatabaseJoinRow
    {
        protected readonly IList<T1> _dataSet1;
        protected readonly IList<T2> _dataSet2;

        public Repository(IDataProvider<T1> dataProvider1, IDataProvider<T2> dataProvider2)
        {
            ColumnInfo = dataProvider1.LoadAllColumns();
            JoinTableColumnInfo = dataProvider2.LoadAllColumns();
            _dataSet1 = dataProvider1.LoadAll();
            _dataSet2 = dataProvider2.LoadAll();
        }

        public IList<ColumnInfo<T1>> ColumnInfo { get; }
        public IList<ColumnInfo<T2>> JoinTableColumnInfo { get; }

        public T1 Find(int id)
        {
             return _dataSet1.FirstOrDefault(record => ((IDatabaseRow) record).PrimaryId == id);
        }


        public IList<T1> FindMany(IList<int> ids)
        {
            return _dataSet1.Where(record => ids.Contains((record as IDatabaseRow).PrimaryId)).ToList();
        }

        public IList<int> FindAllPrimaryIds()
        {
            return _dataSet1.Select(item => item.PrimaryId).ToList();
        }


        public T2 FindAssociatedItem(T1 thing)
        {
            return _dataSet2.FirstOrDefault(record =>
                ((IDatabaseRow) record).PrimaryId == (thing as IDatabaseRow).PrimaryId);
        }

        public IList<T2> FindAssociatedById(int thingId)
        {
            return _dataSet2.Where(record => record.PrimaryId == thingId).ToList();
        }


        public IList<T2> FindAssociated(T1 thing)
        {
            return _dataSet2.Where(record =>
                ((IDatabaseRow) record).PrimaryId == (thing as IDatabaseRow).PrimaryId).ToList();
        }

        public IList<T1> FindAssociated(T2 thing)
        {
            return _dataSet1.Where(record =>
                (record as IDatabaseRow).PrimaryId == ((IDatabaseRow) thing).PrimaryId).ToList();
        }

        public IList<T1> FindAssociated(IList<T2> things)
        {
            return _dataSet1.Where(record => things.Select(thing => thing.PrimaryId).Contains(record.PrimaryId))
                .ToList();
        }

        public IList<T2> FindAssociated(IList<T1> things)
        {
            return _dataSet2.Where(record => things.Select(thing => thing.PrimaryId).Contains(record.PrimaryId))
                .ToList();
        }

        public IList<int> GetAssociatedIds(int id)
        {
            return _dataSet2.Where(record =>
                ((IDatabaseRow) record).PrimaryId == id).Select(record => ((IDatabaseJoinRow) record).SecondaryId).ToList();
        }

        public IList<int> GetAssociatedIds(IList<int> ids)
        {
            return _dataSet2.Where(record => ids.Contains(((IDatabaseRow) record).PrimaryId))
                .Select(record => ((IDatabaseJoinRow) record).SecondaryId).ToList();
        }

        public void Add(T1 data)
        {
            _dataSet1.Add(data);
        }

        public void AddAssociated(IList<T2> things)
        {
            ((List<T2>)_dataSet2).AddRange(things);
        }


        public void AddAssociated(T2 thing)
        {
            _dataSet2.Add(thing);
        }

        public void Update(T1 update)
        {
            var recordToUpdate = _dataSet1.FirstOrDefault(item => item.PrimaryId == update.PrimaryId);
            _dataSet1.Remove(recordToUpdate);
            _dataSet1.Add(update);
        }

        public T1 GetUserByEmail(string userEmail)
        {
            return _dataSet1.FirstOrDefault(user =>
                string.Equals((user as User)?.Email, userEmail, StringComparison.InvariantCulture));
        }

        public bool TryFindUserByEmail(string email, out T1 foundUser) 
        {
            foundUser = _dataSet1.FirstOrDefault(user =>
                string.Equals((user as User)?.Email, email, StringComparison.InvariantCulture));
            return foundUser != null;
        }

        public void UpdateAssociated(int primaryId, int secondaryId, T2 update)
        {
            var recordToUpdate = _dataSet2.FirstOrDefault(item => item.PrimaryId == primaryId && item.SecondaryId == secondaryId);
            _dataSet2.Remove(recordToUpdate);
            _dataSet2.Add(update);
        }
    }


}
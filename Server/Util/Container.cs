using System;
using System.Collections.Generic;
using System.Linq;
using Server.Configuration;
using Server.Database.DataPersisters;
using Server.Database.DataProviders;
using Server.Database.DataProviders.Util;
using Server.MasterData.Model;
using Server.RequestProcessors;
using Server.Storage;

namespace Server.Util
{


    public class Container
    {
        public IList<IDataProvider> DataProviders { get; }
        public UserSessionContainer UserSessionContainer { get; }
        private readonly IConfiguration _config;

        public Container(IConfiguration config)
        {
            _config = config;
            DataProviders = new List<IDataProvider>()
            {
                new DataProvider<User>(config, User.ToDomainConverter),
                new DataProvider<UserPet>(config, UserPet.ToDomainConverter),
                new DataProvider<Pet>(config, Pet.ToDomainConverter),
                new DataProvider<PetMetric>(config, PetMetric.ToDomainConverter),
                new DataProvider<Animal>(config, Animal.ToDomainConverter),
                new DataProvider<AnimalMetric>(config, AnimalMetric.ToDomainConverter),
                new DataProvider<Metric>(config, Metric.ToDomainConverter),
                new DataProvider<MetricInteraction>(config, MetricInteraction.ToDomainConverter),
                new DataProvider<Interaction>(config, Interaction.ToDomainConverter),
            };
            UserSessionContainer = new UserSessionContainer(this, config);
        }


        public IRepository<T1, T2> SiteRepository<T1, T2>() 
            where T1 : IDatabaseRow 
            where T2 : IDatabaseJoinRow
        {
            var dataProvider1 = DataProviders.First(dataProvider => dataProvider.GetType().GenericTypeArguments[0] == typeof(T1));
            var dataProvider2 = DataProviders.First(dataProvider => dataProvider.GetType().GenericTypeArguments[0] == typeof(T2));
            return new Repository<T1, T2>((IDataProvider<T1>)dataProvider1, (IDataProvider<T2>)dataProvider2);
        }

        public IDataProvider<T> DataProvider<T>()
        {
            return (IDataProvider<T>)DataProviders.First(dataProvider => dataProvider.GetType().GenericTypeArguments[0] == typeof(T));
        }


        public IRecordPersister<User> UserRecordPersister()
        {
            var userDataProvider =
                (IDataProvider<User>)DataProviders.First(provider =>
                    provider.GetType().GenericTypeArguments[0] == typeof(User));
            return new UserPersister(userDataProvider.LoadAllColumns(), _config);
        }


        public ISiteRequestProcessor SiteRequestProcessor()
        {
            return new SiteRequestProcessor(UserRecordPersister());
        }

    }
}
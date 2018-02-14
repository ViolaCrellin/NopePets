using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Server.Configuration;
using Server.Database.DataPersisters;
using Server.MasterData.Model;

namespace Server.Util
{
    public class UserSessionContainer : IContainer
    {
        public List<IRecordPersister> Persisters { get; set; }

        public UserSessionContainer(Container container, IConfiguration config)
        {
            Persisters = new List<IRecordPersister>()
            {
                new PetPersister(container.DataProvider<Pet>().LoadAllColumns(), config),
                new UserPetPersister(container.DataProvider<UserPet>().LoadAllColumns(), config),
                new PetMetricPersister(container.DataProvider<PetMetric>().LoadAllColumns(), config)
            };
        }

        public IRecordPersister<T> RecordPersister<T>()
        {
            return (IRecordPersister<T>)Persisters.First(persister =>
            {
                var memberInfo = persister.GetType().BaseType;
                return memberInfo != null && memberInfo.GenericTypeArguments[0] == typeof(T);
            });
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.Configuration;
using Server.Database;
using Server.Database.DataPersisters;
using Server.MasterData.Model;

namespace Server.Util
{
    public class GameDataContainer : IContainer
    {
        public IList<IRecordPersister> Persisters { get; set; }

        public GameDataContainer(Container container, IConfiguration config)
        {
            Persisters = new List<IRecordPersister>()
            {
                new AnimalPersister(container.DataProvider<Animal>().LoadAllColumns(), config),
                new AnimalMetricPersister(container.DataProvider<AnimalMetric>().LoadAllColumns(), config),
                new InteractionPersister(container.DataProvider<Interaction>().LoadAllColumns(), config),
                new MetricInteractionPersister(container.DataProvider<MetricInteraction>().LoadAllColumns(), config),
                new MetricPersister(container.DataProvider<Metric>().LoadAllColumns(), config)
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
using System;
using System.Data;
using Server.MasterData.DTO.Data.CrossService;

namespace Server.MasterData.Model
{

    public class AnimalMetric : IDatabaseJoinRow
    {
        public int AnimalId { get; }
        public int MetricId { get; }

        public int PrimaryId => AnimalId;
        public int SecondaryId => MetricId;
        public RequiredAttentiveness RequiredAttentiveness { get; }

        public AnimalMetric()
        {
        }

        public AnimalMetric(int animalId, int metricId, RequiredAttentiveness requiredAttentiveness)
        {
            AnimalId = animalId;
            MetricId = metricId;
            RequiredAttentiveness = requiredAttentiveness;
        }

        public static Func<IDataReader, AnimalMetric> ToDomainConverter
        {
            get
            {
                return reader => new AnimalMetric(animalId: (int)reader["AnimalId"],
                    metricId: (int)reader["MetricId"],
                    requiredAttentiveness: (RequiredAttentiveness)reader["RequiredAttentiveness"]);
            }
        }
    }
}
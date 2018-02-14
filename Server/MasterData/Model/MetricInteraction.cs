using System;
using System.Data;

namespace Server.MasterData.Model
{
    public class MetricInteraction : IDatabaseJoinRow
    {
        public int MetricId { get; }
        public int InteractionId { get; }

        public int PrimaryId => MetricId;
        public int SecondaryId => InteractionId;

        public MetricInteraction()
        {

        }

        public MetricInteraction(int metricId, int interactionId)
        {
            MetricId = metricId;
            InteractionId = interactionId;
        }


        public static Func<IDataReader, MetricInteraction> ToDomainConverter
        {
            get
            {
                return reader => new MetricInteraction(metricId: (int)reader["MetricId"],
                    interactionId: (int)reader["InteractionId"]);
            }
        }

        protected bool Equals(MetricInteraction other)
        {
            return MetricId == other.MetricId && InteractionId == other.InteractionId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MetricInteraction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (MetricId * 397) ^ InteractionId;
            }
        }
    }
}
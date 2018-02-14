using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Server.Util;

namespace Server.MasterData.Model
{
    public class PetMetric : IDatabaseJoinRow
    {
        public int PetId { get; }
        public int MetricId { get; }
        public int Value { get; }
        public DateTime LastInteractionTime { get; }

        public int PrimaryId => PetId;
        public int SecondaryId => MetricId;

        public PetMetric()
        {
        }

        public PetMetric(int petId, int metricId, int value, DateTime lastInteractionTime)
        {
            PetId = petId;
            MetricId = metricId;
            Value = value;
            LastInteractionTime = lastInteractionTime.ToUniversalTime();
        }


        public static Func<IDataReader, PetMetric> ToDomainConverter
        {
            get
            {
                return reader => new PetMetric(petId: (int)reader["PetId"],
                    metricId: (int)reader["MetricId"],
                    value: (int)reader["Value"],
                    lastInteractionTime: (DateTime)reader["LastInteractionTime"]);
            }
        }

        protected bool Equals(PetMetric other)
        {
            return PetId == other.PetId && MetricId == other.MetricId && Value == other.Value &&
                  LastInteractionTime.HumanEquals(other.LastInteractionTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PetMetric) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PetId;
                hashCode = (hashCode * 397) ^ MetricId;
                hashCode = (hashCode * 397) ^ Value;
                hashCode = (hashCode * 397) ^ LastInteractionTime.GetHashCode();
                return hashCode;
            }
        }


    }
}
using System;
using System.Data;

namespace Server.MasterData.Model
{
    public enum MetricType
    {
        IncreasesWithTime = 0,
        DecreasesWithTime = 1,
    }

    public class Metric : IDatabaseRow
    {
        public int MetricId { get; set; }
        public string Name { get; }
        public string Description { get; }
        public MetricType Type { get; }
        public int NaturalChangeOverTime { get; }

        public int PrimaryId => MetricId;

        public Metric()
        {
        }

        public Metric(int metricId, string name, string description, MetricType type, int naturalChangeOverTime)
        {
            MetricId = metricId;
            Name = name;
            Description = description;
            Type = type;
            NaturalChangeOverTime = naturalChangeOverTime;
        }


        public static Func<IDataReader, Metric> ToDomainConverter
        {
            get
            {
                return reader => new Metric(metricId: (int)reader["MetricId"],
                    name: (string)reader["Name"],
                    description: (string)reader["Description"],
                    type: (MetricType)reader["Type"], 
                    naturalChangeOverTime: (int)reader["NaturalChangeOverTime"]);
            }
        }

        protected bool Equals(Metric other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Description, other.Description) &&
                   Type == other.Type && NaturalChangeOverTime == other.NaturalChangeOverTime;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Metric) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Type;
                hashCode = (hashCode * 397) ^ NaturalChangeOverTime;
                return hashCode;
            }
        }
    }
}
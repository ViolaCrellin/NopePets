using System;
using System.Data;
using Server.MasterData.DTO.Data.CrossService;

namespace Server.MasterData.Model
{
    public class Interaction : IDatabaseRow
    {
        public int InteractionId { get; set; }
        public string Name { get; }
        public string Description { get; }
        public int Value { get; }
        public int CooldownTime { get; }
        public CooldownTimeUnit CooldownTimeUnit { get; }

        public int PrimaryId => InteractionId;

        public Interaction() { }

        public Interaction(int interactionId, string name, string description, int value, int cooldownTime, CooldownTimeUnit cooldownTimeUnit)
        {
            InteractionId = interactionId;
            Name = name;
            Description = description;
            Value = value;
            CooldownTime = cooldownTime;
            CooldownTimeUnit = cooldownTimeUnit;
        }

        protected bool Equals(Interaction other)
        {
            return InteractionId == other.InteractionId && string.Equals(Name, other.Name) &&
                   string.Equals(Description, other.Description) && Value == other.Value &&
                   CooldownTime == other.CooldownTime && CooldownTimeUnit == other.CooldownTimeUnit;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Interaction) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = InteractionId;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Value;
                hashCode = (hashCode * 397) ^ CooldownTime;
                hashCode = (hashCode * 397) ^ (int) CooldownTimeUnit;
                return hashCode;
            }
        }

        public static Func<IDataReader, Interaction> ToDomainConverter
        {
            get
            {
                return reader => new Interaction(interactionId: (int)reader["InteractionId"],
                    name: (string)reader["Name"],
                    description: (string)reader["Description"], 
                    value: (int)reader["Value"], 
                    cooldownTime: (int)reader["CooldownTime"], 
                    cooldownTimeUnit: (CooldownTimeUnit)reader["CooldownTimeUnit"]);
            }
        }
    }
}
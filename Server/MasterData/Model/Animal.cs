using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Server.MasterData.Model
{

    public class Animal : IDatabaseRow
    {
        public int AnimalId { get; set; }
        public string SpeciesName { get; }
        public string Description { get; }

        public int PrimaryId => AnimalId;

        public Animal() { }

        public Animal(int animalId, string speciesName, string description)
        {
            AnimalId = animalId;
            SpeciesName = speciesName;
            Description = description;
        }

        public static Func<IDataReader, Animal> ToDomainConverter
        {
            get
            {
                return reader => new Animal(animalId: (int) reader["AnimalId"],
                    speciesName: (string) reader["SpeciesName"],
                    description: (string) reader["Description"]);
            }
        }

        protected bool Equals(Animal other)
        {
            return string.Equals(SpeciesName, other.SpeciesName) && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Animal) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AnimalId;
                hashCode = (hashCode * 397) ^ (SpeciesName != null ? SpeciesName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
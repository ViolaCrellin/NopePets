using System;
using System.Data;

namespace Server.MasterData.Model
{
    public class Pet : IDatabaseRow
    {
        public int PetId { get; set; }
        public int AnimalId { get; }
        public string Name { get; }

        public int PrimaryId => PetId;

        public Pet()
        {
        }

        public Pet(int animalId, string name)
        {
            AnimalId = animalId;
            Name = name;
        }

        public Pet(int petId, int animalId, string name)
        {
            PetId = petId;
            AnimalId = animalId;
            Name = name;
        }

        protected bool Equals(Pet other)
        {
            return PetId == other.PetId && AnimalId == other.AnimalId && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PetId;
                hashCode = (hashCode * 397) ^ AnimalId;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }


        public static Func<IDataReader, Pet> ToDomainConverter
        {
            get
            {
                return reader => new Pet(petId: (int)reader[nameof(PetId)],
                    animalId: (int)reader[nameof(AnimalId)],
                    name: (string)reader[nameof(Name)]);
            }
        }
    }
}
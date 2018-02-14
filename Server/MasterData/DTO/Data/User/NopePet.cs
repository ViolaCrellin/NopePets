using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.CrossService;

namespace Server.MasterData.DTO.Data.User
{
    [DataContract]
    public class NopePet : IUserSessionData
    {
        [DataMember]
        public string PetName { get; set; }

        [DataMember]
        public Species Species { get; set; }

        [DataMember]
        public List<PetVital> PetHealth { get; set; }

        [DataMember]
        public UserProfile Owner { get; set; }

        [DataMember]
        public DateTime Birthday { get; set; }

        [DataMember]
        public int SpeciesId { get; set; }

        public int PetId { get; set; }

        public static IList<NopePet> Empty = new List<NopePet>()
        {
            new NopePet()
        };

        public static NopePet NewPet(string name, int speciesId) => new NopePet()
        {
            PetName = name,
            SpeciesId = speciesId
        };

        protected bool Equals(NopePet other)
        {
            return string.Equals(PetName, other.PetName) && Equals(Species, other.Species) &&
                   Equals(PetHealth, other.PetHealth) && Equals(Owner, other.Owner) &&
                   Birthday.Equals(other.Birthday) && PetId == other.PetId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NopePet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (PetName != null ? PetName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Species != null ? Species.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PetHealth != null ? PetHealth.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Owner != null ? Owner.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Birthday.GetHashCode();
                hashCode = (hashCode * 397) ^ PetId;
                return hashCode;
            }
        }

    }
}
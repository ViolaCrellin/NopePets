using System;
using System.Data;
using Server.Util;

namespace Server.MasterData.Model
{
    public class UserPet : IDatabaseJoinRow
    {
        public int UserId { get; }
        public int PetId { get; }
        public DateTime DateBorn { get; }

        public int PrimaryId => UserId;
        public int SecondaryId => PetId;

        public UserPet()
        {

        }

        public UserPet(int userId, int petId, DateTime dateBorn)
        {
            UserId = userId;
            PetId = petId;
            DateBorn = dateBorn.ToUniversalTime();
        }

        public static Func<IDataReader, UserPet> ToDomainConverter
        {
            get
            {
                return reader => new UserPet(userId: (int)reader["UserId"],
                    petId: (int)reader["PetId"],
                    dateBorn: (DateTime)reader["DateBorn"]);
            }
        }

        protected bool Equals(UserPet other)
        {
            return UserId == other.UserId && PetId == other.PetId && DateBorn.HumanEquals(other.DateBorn);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserPet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = UserId;
                hashCode = (hashCode * 397) ^ PetId;
                hashCode = (hashCode * 397) ^ DateBorn.GetHashCode();
                return hashCode;
            }
        }

    }
}
using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.User;

namespace Server.MasterData.DTO.Data.Site
{
    [DataContract]
    public class NewUser : ISiteData, IUserSessionData
    {
        public static NewUser EmptyForm = new NewUser();

        public NewUser()
        {

        }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string SecondName { get; set; }


        //We do not compare passwords as in many case we do not wish to include the password
        public bool Equals(NewUser other)
        {
            return string.Equals(Email, other.Email) && string.Equals(Username, other.Username) &&
                   string.Equals(FirstName, other.FirstName) && string.Equals(SecondName, other.SecondName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NewUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SecondName != null ? SecondName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
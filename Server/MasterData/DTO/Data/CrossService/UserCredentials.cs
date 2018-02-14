using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;

namespace Server.MasterData.DTO.Data.CrossService
{
    [DataContract]
    public class UserCredentials : IUserSessionData, ISiteData
    {
        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }

        public int UserId { get; }

        public static UserCredentials Empty => new UserCredentials();

        public UserCredentials()
        {

        }


    }
}
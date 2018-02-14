using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Server.MasterData.DTO.Data.User
{
    [DataContract]
    public class UserProfile : IUserSessionData
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        public int UserId { get; set; }

    }
}
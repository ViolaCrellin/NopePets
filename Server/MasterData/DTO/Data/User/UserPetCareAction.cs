using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Server.MasterData.DTO.Data.User
{
    [DataContract]
    public class UserPetCareAction : IUserSessionData
    {
        public UserPetCareAction()
        {

        }

        [DataMember]
        public int UserId { get; set; }
         
        [DataMember]
        public int PetId { get; set; }

        [DataMember]
        public int  InteractionId { get; set; }
    }
}
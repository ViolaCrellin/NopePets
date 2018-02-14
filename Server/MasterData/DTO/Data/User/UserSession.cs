using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.ModelBinding;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.Model;

namespace Server.MasterData.DTO.Data.User
{
    [DataContract]
    public class UserSession : IUserSessionData, ISiteData
    {
        [DataMember]
        public IList<NopePet> Pets { get; set; }

        [DataMember]
        public UserProfile UserProfile { get; set; }

        [DataMember]
        public DateTime SessionStart { get; set; }

        [DataMember]
        public Guid SessionId { get; set; }

    }
}
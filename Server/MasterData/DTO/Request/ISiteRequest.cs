using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Server.MasterData.DTO.Data;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;

namespace Server.MasterData.DTO.Request
{
    /// <summary>
    /// Site requests are concerned with navigating the API 
    /// </summary>
    public interface ISiteRequest<T> : IRequest<T>
    {

    }

    [DataContract]
    public class SiteRequest<T> : ISiteRequest<ISiteData>
    {
        [DataMember]
        public ISiteData Payload { get; set; }

        public RequestType RequestType { get; set; }

        public Type PayloadType { get; set; }
    }

    [DataContract]
    public class HomePageRequest : SiteRequest<MenuData>
    {

    }

    [DataContract]
    public class RegistrationRequest : SiteRequest<NewUser>
    {
    }

    [DataContract]
    public class UserSessionRequest : SiteRequest<UserCredentials>
    {
    }

}
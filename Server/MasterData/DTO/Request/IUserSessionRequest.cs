using System;
using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.Model;

namespace Server.MasterData.DTO.Request
{
    /// <summary>
    /// User session requests are concerned with data and actions relating to a specific user
    /// </summary>
    public interface IUserSessionRequest<T> : IRequest<T>
    {
        int? UserId { get; set; }
    }

    [DataContract]
    public class UserSessionRequest<T> : IUserSessionRequest<IUserSessionData>
    {
        [DataMember]
        public IUserSessionData Payload { get; set; }
        [DataMember]
        public int? UserId { get; set; }

        //Not necessary for client to supply. These are inferred from the API routes and used 
        // to make processing flow easier
        public Type PayloadType { get; set; }

        public RequestType RequestType { get; set; }
    }

    [DataContract]
    public class UserSessionDataRequest : UserSessionRequest<UserSession>
    {
    }

    [DataContract]
    public class UserSessionPetRequest : UserSessionRequest<NopePet>
    {
    }

    [DataContract]
    public class UserSessionPetCareRequest : UserSessionRequest<UserPetCareAction>
    {
    }

}
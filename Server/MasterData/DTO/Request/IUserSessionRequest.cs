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
    public class UserSessionRequest<T> : IUserSessionRequest<T>
    {
        [DataMember]
        public RequestType RequestType { get; set; }
        [DataMember]
        public T RequestParams { get; set; }
        [DataMember]
        public int? UserId { get; set; }
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
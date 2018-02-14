using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Server.MasterData.DTO.Data;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Response;

namespace Server.MasterData.DTO.Request
{
    /// <summary>
    /// Game requests relate to actions or queries that relate to non-user specific game data
    /// </summary>
    public interface IGameDataRequest<T> : IRequest<T>
    {

    }

    [DataContract]
    public class GameDataRequest<T> : IGameDataRequest<T>
    {
        [DataMember]
        public RequestType RequestType { get; set; }
        [DataMember]
        public T RequestParams { get; set; }
    }


//
//    public class GameDataRequest : , IGameDataRequest
//    {
//        public GameDataRequest(RequestType requestType)
//        {
//            RequestType = requestType;
//        }
//
//        public GameData RequestParams { get; set; }
//    }
}
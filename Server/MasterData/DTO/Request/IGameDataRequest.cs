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
        int? AdminId { get; set; }
    }

    [DataContract]
    public class GameDataRequest<T> : IGameDataRequest<IGameData>
    {
        public RequestType RequestType { get; set; }
        public IGameData Payload { get; set; }
        public Type PayloadType { get; set; }
        public int? AdminId { get; set; }
    }


//
//    public class GameDataRequest : , IGameDataRequest
//    {
//        public GameDataRequest(RequestType requestType)
//        {
//            RequestType = requestType;
//        }
//
//        public GameData Payload { get; set; }
//    }
}
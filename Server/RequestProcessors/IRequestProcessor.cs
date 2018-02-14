using System;
using System.Collections.Generic;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;

namespace Server.RequestProcessors
{
    public interface IRequestProcessor
    {
    }

    public interface ISiteRequestProcessor : IRequestProcessor
    {
        bool TryProcess(RequestType requestType, ISiteData requestData, out IResponse response);
    }

    public interface ISessionRequestProcessor : IRequestProcessor
    {
        bool TryProcess(IUserSessionRequest<IUserSessionData> request, out IResponse response);

        UserSession ProcessSessionDataRequest(IList<Pet> pets, IList<UserPet> userPets, DateTime sessionStart,
            Guid sessionId);
    }

    public interface IGameDataRequestProcessor : IRequestProcessor
    {
        bool TryProcess(IGameDataRequest<IGameData> request, out IResponse response);
    }
}

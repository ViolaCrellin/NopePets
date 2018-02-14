using System;
using System.Runtime.CompilerServices;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;

namespace Server
{
    //Todo - Ideally I would have my services talking to each other - doing it like this means I can rip them appart at a later date
    public partial class SiteService : IGameDataService
    {
        public SpeciesDataResponse GetAllSpeciesInfo()
        {
            var request = new GameDataRequest<IGameData>()
            {
                RequestType = RequestType.Read,
                Payload = null
            };

            IResponse response;
            _state.GetGameState().ProcessRequest(request, out response);
            return (SpeciesDataResponse) response;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;

namespace Server.Validation.GameDataValidators
{
    public class GameDataRequestValidator : IGameDataRequestValidator
    {
        public bool IsValid(IGameDataRequest<IGameData> request, out ErrorMessage errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
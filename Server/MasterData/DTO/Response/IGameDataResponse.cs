using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Game;

namespace Server.MasterData.DTO.Response
{
    [DataContract]
    public class GameDataResponse : Response<IGameData>
    {
        [DataMember] public GameData GameInfo { get; set; }

        public GameDataResponse SetSuccessResponse(GameData data)
        {
            Result = ResponseResult.Success;
            DataType = nameof(GameData);
            GameInfo = data;
            Error = null;
            return this;
        }

        public new GameDataResponse SetErrorResponse(ErrorMessage errorMessage)
        {
            Result = ResponseResult.Failure;
            DataType = nameof(errorMessage.Code);
            Error = errorMessage;
            return this;
        }
    }

    [DataContract]
    public class SpeciesDataResponse : Response<Species>
    {
        [DataMember]
        public Species SpeciesInfo { get; set; }

        public new SpeciesDataResponse SetSuccessResponse(Species data)
        {
            Result = ResponseResult.Success;
            DataType = nameof(Species);
            SpeciesInfo = data;
            Error = null;
            return this;
        }
    }
}

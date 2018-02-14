using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;

namespace Server.Validation
{
    public interface ISiteRequestValidator
    {
        bool IsValid(ISiteRequest<ISiteData> request, out ErrorMessage errorMessage);
    }

    public interface ISiteRequestDataValidator<in T>
    {
        bool IsValid(T request, out ErrorMessage errorMessage);
    }

    public interface IUserSessionRequestValidator
    {
        bool IsValid(IUserSessionRequest<IUserSessionData> request, out ErrorMessage errorMessage);
    }

    public interface IUserSessionRequestDataValidator<in T>
    {
        bool IsValid(User user, T request, out ErrorMessage errorMessage);
    }

    public interface IGameDataRequestValidator 
    {
        bool IsValid(IGameDataRequest<IGameData> request, out ErrorMessage errorMessage);
    }

}

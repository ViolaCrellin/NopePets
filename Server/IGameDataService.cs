using System.ServiceModel;
using System.ServiceModel.Web;
using Server.MasterData.DTO.Response;

namespace Server
{
    [ServiceContract]
    public interface IGameDataService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = UriRoutes.GameData.Animals)]
        SpeciesDataResponse GetAllSpeciesInfo();

        //Todo - Be able to add your own animals - or perhaps this could be a SysAdmin aspect to the API

    }
}

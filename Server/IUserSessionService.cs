using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;

namespace Server
{
    [ServiceContract]
    public interface IUserSessionService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "")]
        UserSessionResponse UserHome();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = UriRoutes.Session.PetRegistration)]
        UserSessionPetResponse PetRegistrationForm();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = UriRoutes.Session.AllMyPets)]
        UserSessionPetResponse GetMyPets(int userId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = UriRoutes.Session.PetRegistration)]
        UserSessionPetResponse CreatePet(Stream stream);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = UriRoutes.Session.LoginRequest)]
        UserSessionResponse Login(Stream createPetRequest);

        [OperationContract]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = UriRoutes.Session.InteractWithMyPet)]
        UserSessionPetCareResponse UpdatePetMetrics(int userId, int petId, int interactionId);

    }
}
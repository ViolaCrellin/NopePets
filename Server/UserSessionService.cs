using System.IO;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.Util;

namespace Server
{
    public partial class SiteService : IUserSessionService
    {
        public UserSessionResponse Login(Stream stream)
        {
            var sessionRequest = stream.DeserializeSiteRequest();
            sessionRequest.RequestType = RequestType.Read;
            IResponse response;
            _state.ProcessRequest(sessionRequest, out response);
            return (UserSessionResponse) response;
        }

        public UserSessionPetCareResponse UpdatePetMetrics(int userId, int petId, int interactionId)
        {
            var userSessionState = _state.GetUserSession(userId);
            if (userSessionState == null)
            {
                var errorResponse = new UserSessionPetCareResponse();
                return errorResponse.SetErrorResponse(new ErrorMessage(ErrorCode.UserSessionNotFound));
            }

            var payload = new UserPetCareAction()
            {
                UserId = userId,
                PetId = petId,
                InteractionId = interactionId
            };

            var sessionRequest = new UserSessionRequest<IUserSessionData>()
            {
                Payload = payload,
                RequestType = RequestType.Update,
                UserId = userId
            };

            IResponse response;
            userSessionState.ProcessRequest(sessionRequest, out response);
            return (UserSessionPetCareResponse) response;
        }

        public UserSessionPetResponse GetMyPets(int userId)
        {
            var sessionRequest = new UserSessionRequest<IUserSessionData>()
            {
                RequestType = RequestType.Read

            };
            var userSessionState = _state.GetUserSession(userId);
            if (userSessionState == null)
            {
                var response = new UserSessionPetResponse();
                return (UserSessionPetResponse) response.SetErrorResponse(new ErrorMessage(ErrorCode.UserSessionNotFound));
            }
            {
                IResponse response;
                userSessionState.ProcessRequest(sessionRequest, out response);
                return (UserSessionPetResponse) response;
            }
        }

        public UserSessionPetResponse CreatePet(Stream stream)
        {
            var newPetRequest = stream.DeserializeUserSessionRequest();
            newPetRequest.RequestType = RequestType.Create;
            var userSessionState = _state.GetUserSession(newPetRequest.UserId);

            IResponse response;
            userSessionState.ProcessRequest(newPetRequest, out response);
            return (UserSessionPetResponse) response;
        }

        public UserSessionResponse UserHome()
        {
            var response = new UserSessionResponse();
            return response.SetSuccessResponse(new UserSession());
        }

        public UserSessionPetResponse PetRegistrationForm()
        {
            var response = new UserSessionPetResponse();
            return response.SetSuccessResponse(NopePet.Empty);
        }
    }
}
using System.IO;
using System.ServiceModel;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.State;
using Server.Util;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class SiteService : ISiteService
    {
        private readonly SiteState _state;

        public SiteService()
        {
            _state = Global.SiteState;
        }

        public SiteResponse GetHome()
        {
            var response = new SiteResponse();
            return response.SetSuccessResponse(MenuData.LandingPage);
        }

        public RegistrationResponse GetRegistrationForm()
        {
           var response = new RegistrationResponse();
           return response.SetSuccessResponse(NewUser.EmptyForm);
        }

        public RegistrationResponse CreateUser(Stream stream)
        {
            var newUserRequest = stream.DeserializeSiteRequest();
            IResponse response;
            _state.ProcessRequest(newUserRequest, out response);
            return (RegistrationResponse)response;
        }

        public LoginFormResponse GetLoginForm()
        {
            var response = new LoginFormResponse();
            return response.SetSuccessResponse(UserCredentials.Empty);
        }
    }
}
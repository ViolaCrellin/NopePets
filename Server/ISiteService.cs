using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;

namespace Server
{
    [ServiceContract]
    [ServiceKnownType(typeof(ResponseResult))]
    public interface ISiteService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = UriRoutes.Site.Home)]
        SiteResponse GetHome();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = UriRoutes.Site.Register)]
        RegistrationResponse GetRegistrationForm();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = UriRoutes.Site.Register)]
        RegistrationResponse CreateUser(Stream stream);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = UriRoutes.Site.LoginForm)]
        LoginFormResponse GetLoginForm();
    }
}

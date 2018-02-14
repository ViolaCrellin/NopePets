using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;

namespace Server.MasterData.DTO.Response
{
    [DataContract]
    public class SiteResponse : Response<MenuData>
    {
        [DataMember]
        public MenuData MenuInfo { get; set; }

        public new SiteResponse SetSuccessResponse(MenuData data)
        {
            Result = ResponseResult.Success;
            DataType = nameof(MenuData);
            MenuInfo = data;
            Error = null;
            return this;
        }

    }

    [DataContract]
    public class RegistrationResponse : Response<NewUser>
    {
        [DataMember]
        public NewUser RegistrationForm { get; set; }

        [DataMember]
        public int? NewUserId { get; set; }

        public new RegistrationResponse SetSuccessResponse(NewUser data)
        {
            DataType = nameof(NewUser);
            Result = ResponseResult.Success;
            RegistrationForm = data;
            return this;
        }
    }


    [DataContract]
    public class LoginFormResponse : Response<UserCredentials>
    {
        [DataMember]
        public UserCredentials LoginForm { get; set; }

        public new LoginFormResponse SetSuccessResponse(UserCredentials data)
        {
            DataType = nameof(UserCredentials);
            Result = ResponseResult.Success;
            LoginForm = data;
            return this;
        }

    }
}
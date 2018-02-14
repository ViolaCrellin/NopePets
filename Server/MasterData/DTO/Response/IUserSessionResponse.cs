using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Server.MasterData.DTO.Data;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;

namespace Server.MasterData.DTO.Response
{
    [DataContract]
    public class UserSessionResponse : Response<UserSession>
    {
        [DataMember]
        public UserSession UserSession { get; set; }

        [DataMember]
        public DateTime SessionStart { get; set; }

        public new UserSessionResponse SetSuccessResponse(UserSession data)
        {
            Result = ResponseResult.Success;
            UserSession = data;
            Error = null;
            return this;
        }

        public new UserSessionResponse SetErrorResponse(ErrorMessage errorMessage)
        {
            Result = ResponseResult.Failure;
            DataType = errorMessage.Code.ToString();
            Error = errorMessage;
            return this;
        }
    }

    [DataContract]
    public class UserSessionPetResponse : Response<NopePet>
    {
        [DataMember]
        public IList<NopePet> Pets { get; set; }

        public UserSessionPetResponse SetSuccessResponse(IList<NopePet> data)
        {
            Result = ResponseResult.Success;
            Pets = data;
            Error = null;
            return this;
        }

        public new UserSessionPetResponse SetErrorResponse(ErrorMessage errorMessage)
        {
            Result = ResponseResult.Failure;
            DataType = nameof(errorMessage.Code);
            Error = errorMessage;
            return this;
        }
    }

    [DataContract]
    public class UserSessionPetCareResponse : Response<PetVital>
    {
        [DataMember]
        public PetVital PetVitals { get; set; }

        public new UserSessionPetCareResponse SetSuccessResponse(PetVital data)
        {
            Result = ResponseResult.Success;
            PetVitals = data;
            Error = null;
            return this;
        }

        public new UserSessionPetCareResponse SetErrorResponse(ErrorMessage errorMessage)
        {
            Result = ResponseResult.Failure;
            DataType = nameof(errorMessage.Code);
            Error = errorMessage;
            return this;
        }
    }

}

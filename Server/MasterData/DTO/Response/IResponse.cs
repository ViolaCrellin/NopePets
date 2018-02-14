using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;

namespace Server.MasterData.DTO.Response
{
    [DataContract]
    public enum ResponseResult
    {
        [EnumMember]
        Success,
        [EnumMember]
        Failure
    }


    public interface IResponse
    {
        IResponse SetErrorResponse(ErrorMessage errorMessage);
        ErrorMessage Error { get; set; }
    }

    public interface IResponse<T> : IResponse
    {
        ResponseResult Result { get; set; }
        string DataType { get; set; }
        IResponse<T> SetSuccessResponse(T data);
        T Data { get; set; }
    }

    [DataContract]
    public class Response<T> : IResponse<T>
    {
        [DataMember]
        public ResponseResult Result { get; set; }
        [DataMember]
        public ErrorMessage Error { get; set; }
        [DataMember]
        public string DataType { get; set; }
        [DataMember]
        public virtual T Data { get; set; }

        public IResponse<T> SetErrorResponse(ErrorMessage errorMessage)
        {
            Result = ResponseResult.Failure;
            DataType = errorMessage.Code.ToString();
            Error = errorMessage;
            return this;
        }

        public virtual IResponse<T> SetSuccessResponse(T data)
        {
            Result = ResponseResult.Success;
            Error = null;
            return this;
        }

        IResponse IResponse.SetErrorResponse(ErrorMessage errorMessage)
        {
            Result = ResponseResult.Success;
            Error = null;
            return this;
        }
    }

}
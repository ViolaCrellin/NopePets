using System;
using System.Runtime.Serialization;

namespace Server.MasterData.DTO.Request
{
    [DataContract]
    public enum RequestType
    {
        [EnumMember]
        Create = 0,
        [EnumMember]
        Read = 1,
        [EnumMember]
        Update = 2,
        [EnumMember]
        Delete = 3,
        [EnumMember]
        ReadAll
    }

    public interface IRequest<T>
    {
        RequestType RequestType { get; set; }
        T Payload { get; set; }
        Type PayloadType { get; set; }
    }

    [DataContract]
    public class Request<T> : IRequest<T>
    {
        [DataMember]
        public T Payload { get; set; }

        public Type PayloadType
        {
            get { return typeof(T); }
            set { value = typeof(T); }
        }

        public RequestType RequestType { get; set; }
    }

}
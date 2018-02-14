using System;
using System.Runtime.Serialization;
using System.Text;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using static System.String;

namespace Server.MasterData.DTO.Response
{
    //Todo ensure all error codes are catered for
    [DataContract]
    public enum ErrorCode
    {
        [EnumMember] RequestTypeNotSupported,
        [EnumMember] RequestPayloadParseError,
        [EnumMember] MissingField,
        [EnumMember] EmailAddressAlreadyInUse,
        [EnumMember] EmailAddressMalformed,
        [EnumMember] PasswordNotStrongEnough,
        [EnumMember] RequestDataNotRecognised,
        [EnumMember] DbPersistenceError,
        [EnumMember] PasswordEncryptionFailure,
        [EnumMember] EmailAddressNotFound,
        [EnumMember] PasswordIncorrect,
        [EnumMember] UserSessionNotFound,
        [EnumMember] SpeciesDoesNotExist,
        [EnumMember] PetAlreadyExists,
        [EnumMember] UserPetNotFound,
        [EnumMember] PetNotResponsive,
        [EnumMember] CareActionNotCooledDown
    }

    [DataContract]
    public class ErrorMessage : IGameData, IUserSessionData, ISiteData
    {
        [DataMember] public Exception ExceptionDetails { get; set; }

        [DataMember] public string Message { get; set; }

        [DataMember] public ErrorCode Code { get; set; }

        public ErrorMessage(ErrorCode code)
        {
            Code = code;
            Message = FromCode(code);
        }

        public ErrorMessage(ErrorCode code, Exception e)
        {
            Code = code;
            Message = FromCode(code);
            ExceptionDetails = e;
        }

        public ErrorMessage(ErrorCode code, string[] messageParams)
        {
            Code = code;
            Message = FromCode(code, messageParams);
        }

        private string FromCode(ErrorCode code)
        {
            switch (code)
            {
                case ErrorCode.RequestTypeNotSupported:
                    return "The supplied HTTP verb is not supported. Don't go making up your own HTTP verbs now";
                case ErrorCode.EmailAddressMalformed:
                    return "The supplied email address looks a bit weird. We can't accept it";
                case ErrorCode.PasswordNotStrongEnough:
                    return "Your password received an F (for FAIL) in our strength test. Try again. Beef it up";
                case ErrorCode.PasswordEncryptionFailure:
                    return "We weren't able to encrypt this password. Maybe because you're psyching us out.";
                case ErrorCode.PasswordIncorrect:
                    return
                        "Have you forgotten your password? We think so. Or you're trying to hack into somebody else's account";
                case ErrorCode.EmailAddressNotFound:
                    return "Are you sure we've seen you before? We can't recognise your email address";
                case ErrorCode.UserSessionNotFound:
                    return "You need to login to see that page";
                case ErrorCode.DbPersistenceError:
                    return "Our bad, we seem to have had an issue saving those details";
                default:
                    return "Unknown Error";
            }
        }

        private string FromCode(ErrorCode code, object[] messageParams)
        {
            switch (code)
            {
                case ErrorCode.EmailAddressAlreadyInUse:
                    return Format("The supplied email address {0} is already in use - are you {1} {2}",
                        messageParams);
                case ErrorCode.MissingField:
                {
                    var sb = new StringBuilder("Er. Are you forgetting something important. Like... ");
                    foreach (var forgottenField in messageParams)
                    {
                        sb.AppendLine($"{forgottenField}");
                    }

                    return sb.ToString();
                }
                case ErrorCode.UserPetNotFound:
                    return Format("The pet with PetId {0} is not associated with the user with UserId {1}",
                        messageParams);
                case ErrorCode.CareActionNotCooledDown:
                    return Format(
                        "Your pet isn't in the mood that ({0}). Your pet only did that {1} ago - come back in {2} minutes",
                        messageParams);
                case ErrorCode.PetNotResponsive:
                    return Format("Are you sure you want to do {0}? I've heard they really hate it", messageParams);
                case ErrorCode.SpeciesDoesNotExist:
                    return Format("The species with the SpeciesId {0} does not exist", messageParams);
                default:
                    return "Unknown Error";
            }
        }
    }
}
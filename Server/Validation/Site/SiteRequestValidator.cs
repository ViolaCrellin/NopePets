using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;

namespace Server.Validation.Site
{

    public class SiteRequestRequestValidator : ISiteRequestValidator
    {
        private readonly RegistrationValidator _registrationValidator;
        private readonly NewUserSessionValidator _newUserSessionRequestRequestValidator;

        public SiteRequestRequestValidator(RegistrationValidator registrationValidator, NewUserSessionValidator newUserSessionRequestRequestValidator)
        {
            _registrationValidator = registrationValidator;
            _newUserSessionRequestRequestValidator = newUserSessionRequestRequestValidator;
        }

        public bool IsValid(ISiteRequest<ISiteData> request, out ErrorMessage errorMessage)
        {
            errorMessage = null;
            var requestData = request.Payload;

            if (requestData is UserCredentials)
            {
                return _newUserSessionRequestRequestValidator.IsValid((UserCredentials)requestData, out errorMessage);
            }
            if (requestData is NewUser)
            {
                return _registrationValidator.IsValid((NewUser)requestData, out errorMessage);
            }

            errorMessage = new ErrorMessage(ErrorCode.RequestTypeNotSupported);
            return false;
        }


    }
}
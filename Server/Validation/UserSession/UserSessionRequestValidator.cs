using System.Linq;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Storage;

namespace Server.Validation.UserSession
{
    public class UserSessionRequestValidator : IUserSessionRequestValidator
    {
        private readonly IUserSessionRequestDataValidator<NopePet> _petRegistrationValidator;
        private readonly IUserSessionRequestDataValidator<UserPetCareAction> _petCareActionValidator;
        private readonly User _user;
        private readonly IRepository<Pet, PetMetric> _pets;
        private readonly IRepository<User, UserPet> _users;

        public UserSessionRequestValidator(User user, IRepository<User, UserPet> users,
            IRepository<Pet, PetMetric> pets, IUserSessionRequestDataValidator<NopePet> petRegistrationValidator,
            IUserSessionRequestDataValidator<UserPetCareAction> petCareActionValidator)
        {
            _petRegistrationValidator = petRegistrationValidator;
            _petCareActionValidator = petCareActionValidator;
            _user = user;
            _users = users;
            _pets = pets;
        } 

        public bool IsValid(IUserSessionRequest<IUserSessionData> request, out ErrorMessage errorMessage)
        {
            errorMessage = null;
            var requestData = request.Payload;
            var requestUserId = request.UserId;

            if (!requestUserId.HasValue || requestUserId != _user.UserId)
            {
                errorMessage = new ErrorMessage(ErrorCode.UserSessionNotFound);
                return false;
            }

            if (requestData == null && request.RequestType == RequestType.ReadAll)
            {
                //This is a request for all data of a particular kind and requires no validation
                return true;
            }

            if (requestData is NopePet)
            {
                return _petRegistrationValidator.IsValid(_user, (NopePet) request.Payload, out errorMessage);
            }

            if (requestData is UserPetCareAction)
            {
                var petCareActionData = (UserPetCareAction) requestData;
                return IsUsersPet(petCareActionData.PetId, requestUserId.Value, out errorMessage) 
                       && _petCareActionValidator.IsValid(_user, petCareActionData, out errorMessage);
            }
            
            errorMessage = new ErrorMessage(ErrorCode.RequestTypeNotSupported);
            return false;
        }

        private bool IsUsersPet(int petId, int userId, out ErrorMessage errorMessage)
        {
            var usersPets = _pets.FindMany(_users.GetAssociatedIds(_user.UserId));
            if (usersPets.All(pet => pet.PetId != petId))
            {
                errorMessage = new ErrorMessage(ErrorCode.UserPetNotFound, new[] {$"{petId}", $"{userId}"});
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Storage;
using Server.Util;
using Server.Validation.Util;

namespace Server.Validation.Site
{
    public class NewUserSessionValidator : ISiteRequestDataValidator<UserCredentials>
    {
        private readonly IRepository<User, UserPet> _users;
        private readonly Encrypter _encrypter;

        public NewUserSessionValidator(IRepository<User, UserPet> users, Encrypter encrypter)
        {
            _users = users;
            _encrypter = encrypter;
        }


        public bool IsValid(UserCredentials userCredentials, out ErrorMessage errorMessage)
        {
            return AreFieldsPopulated(userCredentials, out errorMessage)
                   && AreCredentialsValid(userCredentials, out errorMessage);
        }

        private bool AreFieldsPopulated(UserCredentials userCredentials, out ErrorMessage errorMessage)
        {
            var missingFields = new List<string>();
            userCredentials.Email.CheckValueForNull(nameof(userCredentials.Email), ref missingFields);
            userCredentials.Password.CheckValueForNull(nameof(userCredentials.Password), ref missingFields);
            if (missingFields.Any())
            {
                errorMessage = new ErrorMessage(ErrorCode.MissingField, missingFields.ToArray());
                return false;
            }

            errorMessage = null;
            return true;
        }

        private bool AreCredentialsValid(UserCredentials userCredentials, out ErrorMessage errorMessage)
        {
            User foundUser;
            if (!_users.TryFindUserByEmail(userCredentials.Email, out foundUser))
            {
                errorMessage = new ErrorMessage(ErrorCode.EmailAddressNotFound);
                return false;
            }

            return IsPasswordValid(userCredentials, foundUser, out errorMessage);
        }

        private bool IsPasswordValid(UserCredentials userCredentials, User foundUser, out ErrorMessage errorMessage)
        {
            if (!string.Equals(_encrypter.Encrypt(userCredentials.Password), foundUser.Password,
                StringComparison.InvariantCulture))
            {
                errorMessage = new ErrorMessage(ErrorCode.PasswordIncorrect);
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
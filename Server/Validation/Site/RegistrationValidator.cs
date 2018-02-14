using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Storage;
using Server.Validation.Util;

namespace Server.Validation.Site
{
    public class RegistrationValidator : ISiteRequestDataValidator<NewUser>
    {
        private readonly IRepository<User, UserPet> _users;

        public RegistrationValidator(IRepository<User, UserPet> users)
        {
            _users = users;
        }

        public bool IsValid(NewUser registrationData, out ErrorMessage errorMessage)
        {
            errorMessage = null;
            return AreFieldsPopulated(registrationData, out errorMessage) &&
                   // ReSharper disable once PossibleNullReferenceException - we've already checked above that the data is there
                   IsValidEmail(registrationData.Email, out errorMessage) &&
                   IsPasswordStrong(registrationData.Password, out errorMessage);

        }

        private bool AreFieldsPopulated(NewUser payload, out ErrorMessage errorMessage)
        {
            var missingFields = new List<string>();
            if (payload == null)
            {
                missingFields.Add(nameof(NewUser));
                errorMessage = new ErrorMessage(ErrorCode.MissingField, missingFields.ToArray());
                return false;
            }

            payload.Email.CheckValueForNull(nameof(payload.Email), ref missingFields);
            payload.FirstName.CheckValueForNull(nameof(payload.FirstName), ref missingFields);
            payload.SecondName.CheckValueForNull(nameof(payload.SecondName), ref missingFields);
            payload.Password.CheckValueForNull(nameof(payload.Password), ref missingFields);

            if (missingFields.Any())
            {
                errorMessage = new ErrorMessage(ErrorCode.MissingField, missingFields.ToArray());
                return false;
            }

            errorMessage = null;
            return true;
        }

        private bool IsPasswordStrong(string password, out ErrorMessage errorMessage)
        {
            var passwordScore = PasswordValidator.PasswordComplexityScore(password);
            if (passwordScore < 4)
            {
                var errorMessageValue = Enum.GetName(typeof(PasswordComplexity), passwordScore);
                errorMessage = new ErrorMessage(ErrorCode.PasswordNotStrongEnough, new[] { errorMessageValue });
                return false;
            }

            errorMessage = null;
            return true;
        }

        private bool IsValidEmail(string email, out ErrorMessage errorMessage)
        {
            User foundUser;
            if (_users.TryFindUserByEmail(email, out foundUser))
            {
                errorMessage = new ErrorMessage(ErrorCode.EmailAddressAlreadyInUse, new[]
                {
                    foundUser.Email,
                    foundUser.FirstName,
                    foundUser.LastName
                });
                return false;
            }

            try
            {
                MailAddress m = new MailAddress(email);
                errorMessage = null;
                return true;
            }
            catch (FormatException e)
            {
                errorMessage = new ErrorMessage(ErrorCode.EmailAddressMalformed, e);
                return false;
            }
        }

    }
}
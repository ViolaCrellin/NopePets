using System.Collections.Generic;
using System.Linq;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Storage;
using Server.Validation.Util;

namespace Server.Validation.UserSession
{
    public class PetRegistrationValidator : IUserSessionRequestDataValidator<NopePet>
    {
        private readonly IRepository<Animal, AnimalMetric> _animals;
        private readonly IRepository<Pet, PetMetric> _pets;
        private readonly IRepository<User, UserPet> _users;

        public PetRegistrationValidator(IRepository<Animal, AnimalMetric> animals, IRepository<Pet, PetMetric> pets, IRepository<User, UserPet> users)
        {
            _animals = animals;
            _pets = pets;
            _users = users;
        }

        public bool IsValid(User user, NopePet nopePetData, out ErrorMessage errorMessage)
        {
            errorMessage = null;
            return AreFieldsPopulated(nopePetData, out errorMessage)
                   && IsValidSpecies(nopePetData, out errorMessage)
                   && HasUniqueName(user, nopePetData, out errorMessage);

        }

        private bool HasUniqueName(User user, NopePet nopePetData, out ErrorMessage errorMessage)
        {
           var userPets = _pets.FindMany(_users.GetAssociatedIds(user.UserId));
            if (userPets.Any(pet => pet.Name == nopePetData.PetName))
            {
                errorMessage = new ErrorMessage(ErrorCode.PetAlreadyExists, new[] {nopePetData.PetName});
                return false;
            }

            errorMessage = null;
            return true;
        }

        private bool AreFieldsPopulated(NopePet nopePetData, out ErrorMessage errorMessage)
        {
            var missingFields = new List<string>();
            if (nopePetData == null)
            {
                missingFields.Add(nameof(NopePet));
                errorMessage = new ErrorMessage(ErrorCode.MissingField, missingFields.ToArray());
                return false;
            }

            nopePetData.PetName.CheckValueForNull(nameof(nopePetData.PetName), ref missingFields);
            nopePetData.SpeciesId.CheckValueForDefault(nameof(nopePetData.SpeciesId), ref missingFields);

            if (missingFields.Any())
            {
                errorMessage = new ErrorMessage(ErrorCode.MissingField, missingFields.ToArray());
                return false;
            }

            errorMessage = null;
            return true;
        }

        private bool IsValidSpecies(NopePet nopePetData, out ErrorMessage errorMessage)
        {
            var result = _animals.Find(nopePetData.SpeciesId);
            if (result == null)
            {
                errorMessage = new ErrorMessage(ErrorCode.SpeciesDoesNotExist, new[] { nopePetData.SpeciesId.ToString()});
                return false;
            }

            errorMessage = null;
            return true;

        }
    }
}
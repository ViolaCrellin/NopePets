using System;
using System.Collections.Generic;
using System.Linq;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.Model;
using Server.Util;

namespace Server.Builders
{
    public class UserSessionBuilder
    {
        private readonly NopePetBuilder _nopePetBuilder;

        public UserSessionBuilder(NopePetBuilder nopePetBuilder)
        {
            _nopePetBuilder = nopePetBuilder;
        }

        public UserSession Create(IList<Pet> pets, IList<UserPet> userPets, User user, DateTime sessionStart, Guid sessionId)
        {
            return new UserSession()
            {
                Pets = BuildPets(pets, userPets, user),
                UserProfile = user.ToUserProfile(),
                SessionId = sessionId,
                SessionStart = sessionStart,
            };
        }

        public IList<NopePet> BuildPets(IList<Pet> pets, IList<UserPet> userPets, User user)
        {
            var result = new List<NopePet>();
            foreach (var pet in pets)
            {
                var dateBorn = userPets.First(up => up.PetId == pet.PetId).DateBorn;
                result.Add(_nopePetBuilder.Create(pet, user, dateBorn));
            }
            return result;
        }


        public IList<NopePet> CreatePet(Pet pet, User user)
        {
            return new List<NopePet>()
            {
                _nopePetBuilder.CreateNew(pet, user)
            };
        }

        public PetVital RebuildPetVital(IList<MetricInteraction> metricInteraction, UserPetCareAction petCareAction)
        {
           return  _nopePetBuilder.UpdatePetVital(metricInteraction, petCareAction);
        }
    }
}
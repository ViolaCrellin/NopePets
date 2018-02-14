using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.Model;

namespace Server.Util
{
    public static class Converters
    {
        public static User ToUserModel(this NewUser newUser)
        {
            return new User(newUser.FirstName, newUser.SecondName, newUser.Email, newUser.Password, newUser.Username);
        }

        public static PetMetric ToPetMetricModel(this PetVital petVital)
        {
            return new PetMetric(petVital.PetId, petVital.PetVitalId, petVital.Health, petVital.LastTimeCaredFor);
        }

        public static PetMetric ToNewPetMetricModel(this PetVital petVital)
        {
            return new PetMetric(petVital.PetId, petVital.PetVitalId, petVital.Health, petVital.LastTimeCaredFor);
        }

        public static UserPet ToUserPetModel(this NopePet pet)
        {
            return new UserPet(pet.Owner.UserId, pet.PetId, pet.Birthday);
        }

        public static NewUser ToRegistrationResponseData(this User user)
        {
            return new NewUser()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                SecondName = user.LastName,
                Username = user.Username
            };
        }

        public static UserProfile ToUserProfile(this User user)
        {
            return new UserProfile()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                UserId = user.UserId
            };
        }

        public static ISiteRequest<ISiteData> DeserializeSiteRequest(this Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<ISiteRequest<ISiteData>>(jsonTextReader);
            }
        }

        public static IUserSessionRequest<IUserSessionData> DeserializeUserSessionRequest(this Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<IUserSessionRequest<IUserSessionData>>(jsonTextReader);
            }
        }

        public static SpeciesCareAction ToCareAction(this Interaction interaction)
        {
            return new SpeciesCareAction()
            {
                Value = interaction.Value,
                CooldownTime = interaction.CooldownTime,
                Name = interaction.Name,
                Description = interaction.Description,
                CooldownTimeUnit = interaction.CooldownTimeUnit,
                InteractionId = interaction.InteractionId
            };
        }

        public static Pet ToPetModel(this NopePet nopePet)
        {
            return new Pet(nopePet.PetId, nopePet.Species.SpeciesId, nopePet.PetName);
        }

        public static Pet ToNewPetModel(this NopePet nopePet)
        {
            return new Pet(nopePet.SpeciesId, nopePet.PetName);
        }


    }
}
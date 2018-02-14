using System;
using System.Collections.Generic;
using System.Linq;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.Model;
using Server.Storage;
using Server.Util;

namespace Server.Builders
{
    public class NopePetBuilder
    {
        private readonly SpeciesBuilder _speciesBuilder;
        private readonly PetVitalBuilder _petVitalBuilder;
        private readonly IRepository<Pet, PetMetric> _petRepository;

        public NopePetBuilder(SpeciesBuilder speciesBuilder, PetVitalBuilder petVitalBuilder, IRepository<Pet, PetMetric> petRepository)
        {
            _petRepository = petRepository;
            _speciesBuilder = speciesBuilder;
            _petVitalBuilder = petVitalBuilder;
        }


        public NopePet Create(Pet pet, User user, DateTime dateBorn)
        {
            var petMetrics = _petRepository.FindAssociated(pet);
            var species = _speciesBuilder.Create(pet.AnimalId);

            return new NopePet()
            {
                Species = species,
                PetHealth = _petVitalBuilder.Build(pet, petMetrics, species.HealthMetrics),
                PetName = pet.Name,
                Owner = user.ToUserProfile(),
                Birthday = dateBorn, 
                PetId = pet.PetId
            };

        }

        public NopePet CreateNew(Pet newPet, User user)
        {
            var species = _speciesBuilder.Create(newPet.AnimalId);

            return new NopePet()
            {
                Species = species,
                PetHealth = _petVitalBuilder.CreateNew(newPet, species.HealthMetrics),
                PetName = newPet.Name,
                Owner = user.ToUserProfile(),
                Birthday = DateTime.UtcNow,
                PetId = newPet.PetId
            };
        }

        public PetVital UpdatePetVital(IList<MetricInteraction> metricInteraction, UserPetCareAction petCareAction)
        {
            var pet = _petRepository.Find(petCareAction.PetId);
            var metricIds = metricInteraction.Select(m => m.MetricId);

            var petMetric = _petRepository.FindAssociated(pet)
                .FirstOrDefault(pm => metricIds.Contains(pm.MetricId));

            var speciesVital = _speciesBuilder.Create(pet.AnimalId).HealthMetrics
                .FirstOrDefault(vital => metricIds.Contains(vital.VitalId));

            var speciesCareAction = speciesVital?.RequiredCareActions
                .FirstOrDefault(careAction => careAction.InteractionId == petCareAction.InteractionId);

            return _petVitalBuilder.Rebuild(petMetric, speciesVital, speciesCareAction);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.Model;
using Server.Storage;

namespace Server.Builders
{
    public class SpeciesBuilder
    {
        private readonly SpeciesVitalBuilder _speciesVitalBuilder;
        private readonly IRepository<Animal, AnimalMetric> _animalRepository;

        public SpeciesBuilder(SpeciesVitalBuilder speciesVitalBuilder, IRepository<Animal, AnimalMetric> animalRepository)
        {
            _speciesVitalBuilder = speciesVitalBuilder;
            _animalRepository = animalRepository;
        }

        public Species Create(int animalId)
        {
            var animal = _animalRepository.Find(animalId);
            var animalMetrics = _animalRepository.FindAssociated(animal);
            var healthMetrics = _speciesVitalBuilder.Create(animalMetrics);
            var careActions = healthMetrics.SelectMany(healthMetric => healthMetric.RequiredCareActions).ToList();

            return new Species()
            {
                Name = animal.SpeciesName,
                Description = animal.Description,
                HealthMetrics = healthMetrics,
                CareActions = careActions,
            };
        }

        public IList<Species> BuildAll()
        {
            var animalIds = _animalRepository.FindAllPrimaryIds();
            var result = new List<Species>();

            foreach (var animalId in animalIds)
            {
                result.Add(Create(animalId));
            }

            return result;
        }
    
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.Model;
using Server.Storage;

namespace Server.Builders
{ 
    public class ResponseBuilder
    {
        public SpeciesVitalBuilder SpeciesVitalsBuilder { get; }
        public SpeciesBuilder SpeciesBuilder { get; }
        public PetVitalBuilder PetVitalBuilder { get; }
        public NopePetBuilder NopePetBuilder { get; }
        public UserSessionBuilder UserSessionBuilder { get; }

        public static bool IsInitialised { get; set; }


        private ResponseBuilder(SpeciesVitalBuilder speciesVitalsBuilder, SpeciesBuilder speciesBuilder, PetVitalBuilder petVitalBuilder, NopePetBuilder nopePetBuilder, UserSessionBuilder userSessionBuilder)
        {
            SpeciesVitalsBuilder = speciesVitalsBuilder;
            SpeciesBuilder = speciesBuilder;
            PetVitalBuilder = petVitalBuilder;
            NopePetBuilder = nopePetBuilder;
            UserSessionBuilder = userSessionBuilder;
        }

        public static ResponseBuilder Initialise(IRepository<Animal, AnimalMetric> animalRepository,
            IRepository<Metric, MetricInteraction> metricRepository, IRepository<Pet, PetMetric> petRepository,
            IRepository<Interaction, MetricInteraction> interactionRepository)
        {
            if (IsInitialised)
            {
                throw new Exception("The builders have already been initialised");
            }

            var speciesCareActionBuilder = new SpeciesCareActionBuilder(interactionRepository);
            var speciesVitalsBuilder = new SpeciesVitalBuilder(speciesCareActionBuilder, metricRepository);
            var speciesBuilder = new SpeciesBuilder(speciesVitalsBuilder, animalRepository);
            var petVitalBuilder = new PetVitalBuilder(metricRepository);
            var nopePetBuilder = new NopePetBuilder(speciesBuilder, petVitalBuilder, petRepository);
            var userSessionBuilder = new UserSessionBuilder(nopePetBuilder);

            IsInitialised = true;
            return new ResponseBuilder(speciesVitalsBuilder, speciesBuilder, petVitalBuilder, nopePetBuilder, userSessionBuilder);
        }

        public IList<Species> BuildAllSpecies() => SpeciesBuilder.BuildAll();

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.Model;
using Server.Storage;
using Server.Util;

namespace Server.Builders
{
    public class PetVitalBuilder
    {
        private readonly IRepository<Metric, MetricInteraction> _metricRepository;

        public PetVitalBuilder(IRepository<Metric, MetricInteraction> metricRepository)
        {
            _metricRepository = metricRepository;
        }

        public List<PetVital> Build(Pet pet, IList<PetMetric> petMetrics, IList<SpeciesVital> speciesVitals)
        {
            var result = new List<PetVital>();

            foreach (var petMetric in petMetrics)
            {
                var speciesVital = speciesVitals.First(vital => vital.VitalId == petMetric.MetricId);
                result.Add(Build(petMetric, speciesVital));
            }

            return result;
        }

        private PetVital Build(PetMetric petMetric, SpeciesVital speciesVital)
        {          
            var neglectPeriod = DateTime.UtcNow - petMetric.LastInteractionTime;
            var healthDeclineDuringNeglect = CalculateNeglectImpact(petMetric, neglectPeriod, speciesVital);
            return new PetVital()
            {
                Health = petMetric.Value - healthDeclineDuringNeglect,
                LastTimeCaredFor = petMetric.LastInteractionTime,
                TimeNeglectedFor = neglectPeriod,
                HealthDeclineDuringNeglect = healthDeclineDuringNeglect,
                VitalName = speciesVital.Name,
                VitalStats = speciesVital,
                PetVitalId = petMetric.MetricId,
                PetId = petMetric.PetId,
            };
        }

        public List<PetVital> CreateNew(Pet pet, IList<SpeciesVital> speciesVitals)
        {
            var result = new List<PetVital>();

            var petSpeciesVitals = speciesVitals.Where(vital => vital.SpeciesId == pet.AnimalId).ToList();

            foreach (var petSpeciesVital in petSpeciesVitals)
            {
                result.Add(new PetVital()
                {
                    Health = 0, //We always start at neutral
                    LastTimeCaredFor = DateTime.UtcNow, //Any rate of decline in health starts now
                    TimeNeglectedFor = TimeSpan.FromSeconds(1), 
                    HealthDeclineDuringNeglect = 0, //No decline yet
                    VitalName = petSpeciesVital.Name,
                    VitalStats = petSpeciesVital,
                    PetVitalId = petSpeciesVital.VitalId,
                    PetId = pet.PetId
                });
            }

            return result;
        }


        //TODO UNIT TEST
        private int CalculateNeglectImpact(PetMetric petMetric, TimeSpan neglectPeriod, SpeciesVital speciesVital)
        {
            var metric = _metricRepository.Find(petMetric.MetricId);
            var value = petMetric.Value;
            var rate = 1 / metric.NaturalChangeOverTime;
            //How frequently we apply the NaturalChangeOverTime rate (not an even distribution)
            var frequencyTimespan = speciesVital.RequiredAttentiveness.ToTimeSpan();

            var numberOfNeglectedPeriods = neglectPeriod.DividedBy(frequencyTimespan);
            for (int i = 0; i == numberOfNeglectedPeriods; i++)
            {
                if (metric.Type == MetricType.DecreasesWithTime)
                {
                    value -= (value * rate);
                }

                if (metric.Type == MetricType.IncreasesWithTime)
                {
                    value += (value * rate);
                }
            }

            return value;
        }

        public PetVital Rebuild(PetMetric petMetric, SpeciesVital animalVital, SpeciesCareAction speciesCareAction)
        {
            //This will recalculate the health based on last time interacted with. We then add the positive affect of the interaction
            var oldVital = Build(petMetric, animalVital);

            //Todo - straight return, this is just for debug
            var newVital = new PetVital()
            {
                Health = RecalculateMetric(oldVital.Health, petMetric, speciesCareAction), //We recalculate this
                LastTimeCaredFor = DateTime.UtcNow, //Gets refreshed
                TimeNeglectedFor = TimeSpan.FromSeconds(1),
                HealthDeclineDuringNeglect = 0, //No decline yet
                VitalName = animalVital.Name,
                VitalStats = animalVital,
                PetVitalId = animalVital.VitalId,
                PetId = petMetric.PetId
            };

            return newVital;
        }

        private int RecalculateMetric(int oldHealth, PetMetric petMetric, SpeciesCareAction careAction)
        {
            var metric = _metricRepository.Find(petMetric.MetricId);
            return metric.Type == MetricType.DecreasesWithTime
                ? oldHealth + careAction.Value
                : oldHealth - careAction.Value;
        }
    }
}
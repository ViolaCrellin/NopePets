using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.Model;
using Server.Storage;

namespace Server.Builders
{
    public class SpeciesVitalBuilder
    {
        private readonly IRepository<Metric, MetricInteraction> _metricsRepository;
        private readonly SpeciesCareActionBuilder _speciesCareActionBuilder;

        public SpeciesVitalBuilder(SpeciesCareActionBuilder speciesCareActionBuilder,
            IRepository<Metric, MetricInteraction> metricsRepository)
        {
            _metricsRepository = metricsRepository;
            _speciesCareActionBuilder = speciesCareActionBuilder;
        }

        public List<SpeciesVital> Create(IList<AnimalMetric> animalMetrics)
        {
            var result = new List<SpeciesVital>();
            foreach (var animalMetric in animalMetrics)
            {
                var metric = _metricsRepository.Find(animalMetric.MetricId);
                var metricInteraction = _metricsRepository.FindAssociated(metric);

                result.Add(new SpeciesVital()
                {
                    RequiredCareActions = _speciesCareActionBuilder.Create(metricInteraction),
                    Description = metric.Description,
                    Name = metric.Name,
                    VitalId = metric.MetricId,
                    RequiredAttentiveness = animalMetric.RequiredAttentiveness, 
                    SpeciesId = animalMetric.AnimalId                                       
                });
            }

            return result;
        }
    }
}
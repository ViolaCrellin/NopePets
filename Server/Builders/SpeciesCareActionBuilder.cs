using System.Collections.Generic;
using System.Linq;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.Model;
using Server.Storage;
using Server.Util;

namespace Server.Builders
{
    public class SpeciesCareActionBuilder
    {
        private readonly IRepository<Interaction, MetricInteraction> _interactionRepository;

        public SpeciesCareActionBuilder(IRepository<Interaction, MetricInteraction> interactionRepository)
        {
            _interactionRepository = interactionRepository;
        }

        public List<SpeciesCareAction> Create(IList<MetricInteraction> metricInteractions)
        {
            var result = new List<SpeciesCareAction>();
            foreach (var metricInteraction in metricInteractions)
            {
                var interaction = _interactionRepository.Find(metricInteraction.InteractionId);
                result.Add(interaction.ToCareAction());
            }
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Storage;
using Server.Validation.Util;

namespace Server.Validation.UserSession
{
    public class PetCareActionValidator : IUserSessionRequestDataValidator<UserPetCareAction>
    {
        private readonly IRepository<Pet, PetMetric> _pets;
        private readonly IRepository<Interaction, MetricInteraction> _interactions;

        public PetCareActionValidator(IRepository<Pet, PetMetric> pets,
            IRepository<Interaction, MetricInteraction> interactions)
        {
            _pets = pets;
            _interactions = interactions;
        }

        public bool IsValid(User user, UserPetCareAction petCareAction, out ErrorMessage errorMessage)
        {
            if (!AreFieldsPopulated(petCareAction, out errorMessage))
                return false;

            var responsiveMetricsIds = _interactions.GetAssociatedIds(petCareAction.InteractionId);

            return 
                IsPetResponsive(petCareAction, responsiveMetricsIds, out errorMessage) &&
                   HasCooledDown(petCareAction, responsiveMetricsIds, out errorMessage);
        }

        private bool IsPetResponsive(UserPetCareAction petCareAction, IList<int> responsiveMetricsIds,
            out ErrorMessage errorMessage)
        {
            var petMetrics = _pets.FindAssociatedById(petCareAction.PetId);
            if (!petMetrics.Any(petMetric => responsiveMetricsIds.Contains(petMetric.MetricId)))
            {
                var interactionName = _interactions.Find(petCareAction.InteractionId).Name;
                errorMessage = new ErrorMessage(ErrorCode.PetNotResponsive, new[] {interactionName});
                return false;
            }

            errorMessage = null;
            return true;
        }

        private bool AreFieldsPopulated(UserPetCareAction petCareActionData, out ErrorMessage errorMessage)
        {
            var missingFields = new List<string>();
            if (petCareActionData == null)
            {
                missingFields.Add(nameof(UserPetCareAction));
                errorMessage = new ErrorMessage(ErrorCode.MissingField, missingFields.ToArray());
                return false;
            }
            petCareActionData.UserId.CheckValueForDefault(nameof(petCareActionData.UserId), ref missingFields);
            petCareActionData.PetId.CheckValueForDefault(nameof(petCareActionData.PetId), ref missingFields);
            petCareActionData.InteractionId.CheckValueForDefault(nameof(petCareActionData.InteractionId), ref missingFields);

            if (missingFields.Any())
            {
                errorMessage = new ErrorMessage(ErrorCode.MissingField, missingFields.ToArray());
                return false;
            }

            errorMessage = null;
            return true;
        }


        private bool HasCooledDown(UserPetCareAction petCareAction, IList<int> responsiveMetricsIds,
            out ErrorMessage errorMessage)
        {
            var petMetrics = _pets.FindAssociatedById(petCareAction.PetId);
            var interaction = _interactions.Find(petCareAction.InteractionId);
            var lastInteractionTime = petMetrics.FirstOrDefault(pm => responsiveMetricsIds.Contains(pm.MetricId))
                ?.LastInteractionTime;
            var actionCooldownTime = CalculateCoolDownTime(interaction);
            var timeSinceLastInteraction = DateTime.UtcNow - lastInteractionTime;

            if (timeSinceLastInteraction.HasValue && timeSinceLastInteraction < actionCooldownTime)
            {
                var minutesSinceInteraction =
                    Math.Round(timeSinceLastInteraction.Value.TotalMinutes, 0, MidpointRounding.AwayFromZero);
                var timeUntilCooledDown = actionCooldownTime - TimeSpan.FromMinutes(minutesSinceInteraction);
                var minutesToWait = Math.Round(timeUntilCooledDown.TotalMinutes, 0, MidpointRounding.AwayFromZero);
                errorMessage = new ErrorMessage(ErrorCode.CareActionNotCooledDown,
                    new[]
                    {
                        interaction.Name, $"{minutesSinceInteraction} minutes", $"{minutesToWait}"
                    });
                return false;
            }

            errorMessage = null;
            return true;
        }

        private TimeSpan CalculateCoolDownTime(Interaction interaction)
        {
            var cooldown = interaction.CooldownTime;
            switch (interaction.CooldownTimeUnit)
            {
                case CooldownTimeUnit.Second:
                    return TimeSpan.FromSeconds(cooldown);
                case CooldownTimeUnit.Minute:
                    return TimeSpan.FromMinutes(cooldown);
                case CooldownTimeUnit.Hour:
                    return TimeSpan.FromHours(cooldown);
                case CooldownTimeUnit.Day:
                    return TimeSpan.FromDays(cooldown);
                case CooldownTimeUnit.Week:
                    return TimeSpan.FromDays(cooldown * 7);
                default:
                    return TimeSpan.Zero;
            }
        }
    }
}
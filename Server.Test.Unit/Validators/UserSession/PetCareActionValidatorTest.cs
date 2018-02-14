using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Storage;
using Server.TestingShared;
using Server.Validation.UserSession;

namespace Server.Test.Unit.Validators.UserSession
{
    [TestFixture]
    public class PetCareActionValidatorTest
    {
        private PetCareActionValidator _sut;
        private Mock<IRepository<Interaction, MetricInteraction>> _mockInteractionRepository;
        private IList<int> _nonReponsiveMetricIds;
        private IList<int> _reponsiveMetricIds;
        private UserPetCareAction _validPayload;
        private List<PetMetric> _petMetrics;
        private Mock<IRepository<Pet, PetMetric>> _mockPetRepository;


        [SetUp]
        public void Setup()
        {
            _petMetrics = new List<PetMetric>();
            _petMetrics.AddRange(TestData.UsersPets.BurberryPetMetrics);
            _petMetrics.AddRange(TestData.UsersPets.VersacePetMetrics);

            _nonReponsiveMetricIds = _petMetrics.Select(petMetric => petMetric.MetricId + 5).ToList();
            _reponsiveMetricIds = _petMetrics.Select(petMetric => petMetric.MetricId).ToList();

            _mockInteractionRepository = new Mock<IRepository<Interaction, MetricInteraction>>();
            _mockPetRepository = new Mock<IRepository<Pet, PetMetric>>();

            _mockPetRepository.Setup(m => m.FindAssociatedById(It.IsAny<int>()))
                .Returns(TestData.UsersPets.VersacePetMetrics);

            _validPayload = new UserPetCareAction()
            {
                PetId = TestData.UsersPets.VersacePetId,
                InteractionId = TestData.Interactions.TakeSelfieWithInteractionId,
                UserId = TestData.Users.MeJulieUserId
            };

            _sut = new PetCareActionValidator(_mockPetRepository.Object, _mockInteractionRepository.Object);
        }

        public static IEnumerable PetInteractionCooldownTestCases
        {
            get
            {
                yield return new TestCaseData(TestData.Interactions.TakeSelfieWithPet, TimeSpan.FromMinutes(50));
                yield return new TestCaseData(TestData.Interactions.BuyNewOutfitForPet, TimeSpan.FromMinutes(30));
                yield return new TestCaseData(TestData.Interactions.FeedMeal, TimeSpan.FromMinutes(10));
                yield return new TestCaseData(TestData.Interactions.FeedTreat, TimeSpan.FromMinutes(25));
            }

        }

        public static IEnumerable Interactions
        {
            get
            {
                yield return new TestCaseData(TestData.Interactions.TakeSelfieWithPet);
                yield return new TestCaseData(TestData.Interactions.BuyNewOutfitForPet);
                yield return new TestCaseData(TestData.Interactions.FeedMeal);
                yield return new TestCaseData(TestData.Interactions.FeedTreat);
            }

        }


        [Test]
        public void
            Given_a_UserPetCareAction_request_with_no_payload_then_when_validated_does_not_throw_NRE_and_returns_false_with_correct_error()
        {
            ErrorMessage error;
            Assert.DoesNotThrow(() => _sut.IsValid(TestData.Users.MeJulie, null, out error));
            var result = _sut.IsValid(TestData.Users.MeJulie, null, out error);

            Assert.IsFalse(result);
            Assert.AreEqual(error.Message,
                new ErrorMessage(ErrorCode.MissingField, new[] {nameof(UserPetCareAction)}).Message);
        }

        [TestCase(0, 0, 0)]
        [TestCase(1, 0, 0)]
        [TestCase(0, 1, 0)]
        [TestCase(0, 0, 1)]
        [TestCase(1, 1, 0)]
        [TestCase(1, 0, 1)]
        [TestCase(1, 1, 0)]
        public void
            Given_a_UserPetCareAction_request_with_default_payload_fields_then_when_validated_returns_false_with_correct_error(
                int petId, int interactionId, int userId)
        {
            //Arrange
            var payload = new UserPetCareAction()
            {
                UserId = userId,
                PetId = petId,
                InteractionId = interactionId
            };

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(TestData.Users.MeJulie, payload, out error);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(error.Code, ErrorCode.MissingField);

            if (userId == default(int))
                StringAssert.Contains(nameof(payload.UserId), error.Message);
            if (interactionId == default(int))
                StringAssert.Contains(nameof(payload.InteractionId), error.Message);
            if (petId == default(int))
                StringAssert.Contains(nameof(payload.PetId), error.Message);
        }

        [Test]
        public void
            Given_a_UserPetCareAction_request_interactionId_does_not_correspond_to_a_PetMetric_then_when_validated_returns_false_with_correct_error()
        {
            //Arrange
            //Even though it is I have incremented the mock interaction repository MetricInteractionIds by one
            var payload = _validPayload;
            _mockInteractionRepository.Setup(m => m.GetAssociatedIds(It.IsAny<int>())).Returns(_nonReponsiveMetricIds);
            var interaction = TestData.Interactions.TakeSelfieWithPet;
            _mockInteractionRepository.Setup(m => m.Find(payload.InteractionId))
                .Returns(interaction);
            _mockPetRepository.Setup(m => m.FindAssociatedById(payload.PetId))
                .Returns(_petMetrics);

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(TestData.Users.MeJulie, payload, out error);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(error.Code, ErrorCode.PetNotResponsive);
            StringAssert.Contains(interaction.Name, error.Message);
        }


        [TestCaseSource(nameof(PetInteractionCooldownTestCases))]
        public void
            Given_a_UserPetCareAction_request_for_an_interaction_that_has_not_reached_cooldown_then_when_validated_returns_false_with_correct_error(Interaction interaction, TimeSpan minutesUntilCanInteract)
        {
            //Arrange
            var tooSoonInteractionTime = CalculateCooldown(interaction) - minutesUntilCanInteract;
            
            var payload = _validPayload; 
            _mockInteractionRepository.Setup(m => m.GetAssociatedIds(It.IsAny<int>())).Returns(_reponsiveMetricIds);
            _mockInteractionRepository.Setup(m => m.Find(It.IsAny<int>()))
                .Returns(interaction);
            _mockPetRepository.Setup(m => m.FindAssociatedById(It.IsAny<int>()))
                .Returns(_petMetrics.Select(metric => AdjustLastInteractionTime(metric, tooSoonInteractionTime)).ToList);
            //Act
            ErrorMessage error;
            var result = _sut.IsValid(TestData.Users.MeJulie, payload, out error);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(error.Code, ErrorCode.CareActionNotCooledDown);
            StringAssert.Contains($"{minutesUntilCanInteract.Minutes}", error.Message);
        }

        [TestCaseSource(nameof(Interactions))]
        public void Given_a_valid_UserPetCareAction_request_then_when_validated_returns_true_with_no_error(Interaction interaction)
        {
            //Arrange
            var tooSoonInteractionTime = CalculateCooldown(interaction) + TimeSpan.FromHours(1);

            var payload = _validPayload;
            _mockInteractionRepository.Setup(m => m.GetAssociatedIds(It.IsAny<int>())).Returns(_reponsiveMetricIds);
            _mockInteractionRepository.Setup(m => m.Find(It.IsAny<int>()))
                .Returns(interaction);
            _mockPetRepository.Setup(m => m.FindAssociatedById(It.IsAny<int>()))
                .Returns(_petMetrics.Select(metric => AdjustLastInteractionTime(metric, tooSoonInteractionTime)).ToList);
            //Act
            ErrorMessage error;
            var result = _sut.IsValid(TestData.Users.MeJulie, payload, out error);

            //Assert
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        private TimeSpan CalculateCooldown(Interaction interaction)
        {
            var cooldown = interaction.CooldownTime;
            TimeSpan coolDownTime;
            switch (interaction.CooldownTimeUnit)
            {
                case CooldownTimeUnit.Second:
                    coolDownTime = TimeSpan.FromSeconds(cooldown);
                    break;
                case CooldownTimeUnit.Minute:
                    coolDownTime = TimeSpan.FromMinutes(cooldown);
                    break;
                case CooldownTimeUnit.Hour:
                    coolDownTime = TimeSpan.FromHours(cooldown);
                    break;
                case CooldownTimeUnit.Day:
                    coolDownTime = TimeSpan.FromDays(cooldown);
                    break;
                case CooldownTimeUnit.Week:
                    coolDownTime = TimeSpan.FromDays(cooldown * 7);
                    break;
                default:
                    return TimeSpan.Zero;
            }

            return coolDownTime;
        }

        private PetMetric AdjustLastInteractionTime(PetMetric petMetric, TimeSpan minutesUntilCanInteract)
        {
            return new PetMetric(petMetric.PetId, petMetric.MetricId, 10, DateTime.UtcNow - minutesUntilCanInteract);
        }

    }
}

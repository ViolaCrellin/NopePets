using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Server.Builders;
using Server.Configuration;
using Server.Database.DataPersisters;
using Server.Database.DataProviders;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.RequestProcessors;
using Server.State;
using Server.Storage;
using Server.TestingShared;
using Server.Util;

namespace Server.Test.Integration.RequestToStateProcessing
{
    [TestFixture]
    public class RequestProcessingTestBase
    {
        internal IConfiguration Config;
        internal Mock<IRepository<User, UserPet>> MockUsers;
        internal Mock<IRepository<Pet, PetMetric>> MockPets;
        internal Mock<IRepository<Animal, AnimalMetric>> MockAnimals;
        internal Mock<IRepository<Interaction, MetricInteraction>> MockInteractions;
        internal Mock<IRepository<Metric, MetricInteraction>> MockMetrics;
        internal Mock<IDataProvider<User>> MockUserDataProvider;
        internal Mock<IRecordPersister<User>> MockUserPersister;
        internal Container Container;

        [SetUp]
        public void SetUp()
        {
            SiteState.IsInitalised = false;
            ResponseBuilder.IsInitialised = false;

            MockUsers = new Mock<IRepository<User, UserPet>>();
            MockUsers.Setup(m => m.Add(It.IsAny<User>()));
            MockUsers.Setup(m => m.JoinTableColumnInfo).Returns(TestData.Columns.UserPetColumns);

            MockPets = new Mock<IRepository<Pet, PetMetric>>();
            MockPets.Setup(m => m.ColumnInfo).Returns(TestData.Columns.PetColumns);

            MockAnimals = new Mock<IRepository<Animal, AnimalMetric>>();
            MockMetrics = new Mock<IRepository<Metric, MetricInteraction>>();
            MockInteractions = new Mock<IRepository<Interaction, MetricInteraction>>();

            MockUserDataProvider = new Mock<IDataProvider<User>>();
            MockUserDataProvider.Setup(m => m.LoadAllColumns()).Returns(TestData.Columns.UserColumns);
            MockUserPersister = new Mock<IRecordPersister<User>>();

            Config = new TestConfiguration();
            Container = new Container(Config);
        }

        internal SiteState InitialiseSiteState()
        {
            SiteState.IsInitalised = false;
            ResponseBuilder.IsInitialised = false;

            return SiteState.Initialise(Config, MockUsers.Object, MockPets.Object, MockAnimals.Object,
                MockInteractions.Object, MockMetrics.Object, MockUserDataProvider.Object,
                new SiteRequestProcessor(MockUserPersister.Object), Container.UserSessionContainer);
        }

        internal UserSessionState InitialiseUserSessionState(User user, ResponseBuilder responseBuilder)
        {
            if(!SiteState.IsInitalised)
                Assert.Fail("The site state is not yet initialised");

            if(Container == null)
                Container = new Container(new TestConfiguration());
            //var responseBuilder = ResponseBuilder.Initialise(MockAnimals.Object, MockMetrics.Object, MockPets.Object, MockInteractions.Object);

            return UserSessionState.Initialise(user, MockUsers.Object, MockPets.Object, MockAnimals.Object,
                MockInteractions.Object, Config, responseBuilder.UserSessionBuilder,
                Container.UserSessionContainer);
        }

        internal UserSessionState SetupMockMeJulieLogin()
        {
            var siteSitate = InitialiseSiteState();

            var sut = InitialiseUserSessionState(TestData.Users.MeJulie, siteSitate.ResponseBuilder);
            var loginRequest = new SiteRequest<ISiteData>()
            {
                RequestParams = TestData.Users.MeJuliesLogin,
                RequestType = RequestType.Read
            };

            User foundUser = TestData.Users.MeJulie;
            MockUsers.Setup(m => m.TryFindUserByEmail(TestData.Users.MeJuliesLogin.Email, out foundUser)).Returns(true);
            MockUsers.Setup(m => m.GetUserByEmail(TestData.Users.MeJuliesLogin.Email)).Returns(TestData.Users.MeJulie);
            SetupMockUserSessionData();

            IResponse loginResponse;
            siteSitate.ProcessRequest(loginRequest, out loginResponse);
            Assert.AreEqual(1, siteSitate.GetUserSessions().Count);
            Assert.IsNull(loginResponse.Error);
            return sut;
        }

        /// <summary>
        /// Test User is Ali G's Me Julie. Don't ask why.
        /// Both her pets are Chihuahuas, they are Versace and Burberry
        /// Chihuahuas have two metrics, Hunger and Confidence
        /// An animals confidence is bolstered by Taking a Selfie or buying them a new outfit
        /// An animals hunger is satiated by feeding the pet either a treat or a meal
        /// </summary>
        internal void SetupMockUserSessionData()
        {
            var meJulie = TestData.Users.MeJulie;
            var meJuliesPetIds = TestData.UsersPets.MeJuliesPetIds;
            var meJuliesPets = TestData.UsersPets.MeJuliesPets;
            MockUsers.Setup(m => m.GetAssociatedIds(meJulie.UserId)).Returns(meJuliesPetIds);
            MockUsers.Setup(m => m.FindAssociated(meJulie)).Returns(new List<UserPet>()
            {
                TestData.UsersPets.MeJuliesVersace,
                TestData.UsersPets.MeJuliesBurberry
            });
            MockPets.Setup(m => m.FindMany(meJuliesPetIds)).Returns(meJuliesPets);
            MockPets.Setup(m => m.FindAssociated(meJuliesPets[0])).Returns(TestData.UsersPets.VersacePetMetrics);
            MockPets.Setup(m => m.FindAssociated(meJuliesPets[1])).Returns(TestData.UsersPets.BurberryPetMetrics);

            var chihuahua = TestData.Animals.Chihuahua;
            var chihuahuaMetrics = TestData.AnimalMetrics.ChihuahuaMetrics;
            MockAnimals.Setup(m => m.Find(chihuahua.AnimalId)).Returns(chihuahua);
            MockAnimals.Setup(m => m.FindAssociated(chihuahua)).Returns(chihuahuaMetrics);

            var confidenceMetric = TestData.AnimalMetrics.Confidence;
            var hungerMetric = TestData.AnimalMetrics.Hunger;

            MockMetrics.Setup(m => m.Find(confidenceMetric.MetricId))
                .Returns(confidenceMetric);

            MockMetrics.Setup(m => m.Find(hungerMetric.MetricId))
                .Returns(hungerMetric);

            MockMetrics.Setup(m => m.FindAssociated(confidenceMetric))
                .Returns(TestData.Interactions.ConfidenceMetricInteractions);
            MockMetrics.Setup(m => m.FindAssociated(hungerMetric))
                .Returns(TestData.Interactions.HungerMetricInteractions);

            var confidenceBuyOutfit = TestData.Interactions.ConfidenceBuyNewOutfit;
            MockInteractions.Setup(m => m.Find(confidenceBuyOutfit.InteractionId))
                .Returns(TestData.Interactions.BuyNewOutfitForPet);
            var confidenceTakeSelfie = TestData.Interactions.ConfidenceTakeSelfieWithPet;
            MockInteractions.Setup(m => m.Find(confidenceTakeSelfie.InteractionId))
                .Returns(TestData.Interactions.TakeSelfieWithPet);

            var hungerFeedMeal = TestData.Interactions.HungerFeedMeal;
            MockInteractions.Setup(m => m.Find(hungerFeedMeal.InteractionId))
                .Returns(TestData.Interactions.FeedMeal);
            var hungerFeedTreat = TestData.Interactions.HungerFeedTreat;
            MockInteractions.Setup(m => m.Find(hungerFeedTreat.InteractionId))
                .Returns(TestData.Interactions.FeedTreat);
        }

        internal void AssertMeJuliesSessionDataIsCorrect(UserSession responseData)
        {
            Assert.IsNotNull(responseData);
            Assert.IsInstanceOf<UserSession>(responseData);

            var userPets = responseData.Pets;
            Assert.AreEqual(2, userPets.Count);

            var expectedInteractions = new List<Interaction>();
            expectedInteractions.AddRange(TestData.Interactions.ConfidenceInteractions);
            expectedInteractions.AddRange(TestData.Interactions.HungerInteractions);

            var expectedMetricInteractions = new List<MetricInteraction>();
            expectedMetricInteractions.AddRange(TestData.Interactions.ConfidenceMetricInteractions);
            expectedMetricInteractions.AddRange(TestData.Interactions.HungerMetricInteractions);

            var expectedPetMetrics = new List<PetMetric>();
            expectedPetMetrics.AddRange(TestData.UsersPets.BurberryPetMetrics);
            expectedPetMetrics.AddRange(TestData.UsersPets.VersacePetMetrics);

            var expectedMetrics = new List<Metric>()
            {
                TestData.AnimalMetrics.Confidence,
                TestData.AnimalMetrics.Hunger
            };

            Assert.Multiple(() => AssertUserPetsAreCorrect(
                actualUserPets: userPets,
                expectedUserPets: TestData.UsersPets.MeJuliesPets,
                owner: TestData.Users.MeJulie, expectedInteractions: expectedInteractions,
                expectedAnimalMetrics: TestData.AnimalMetrics.ChihuahuaMetrics,
                expectedMetricInteractions: expectedMetricInteractions, expectedPetMetrics: expectedPetMetrics, expectedMetrics: expectedMetrics));
        }

        /// <summary>
        /// Cascading assert where all the inner objects of a User's NopePet are asserted against for presence, count and values
        /// </summary>
        /// <param name="actualUserPets"></param>
        /// <param name="expectedUserPets"></param>
        /// <param name="owner"></param>
        /// <param name="expectedPetMetrics"></param>
        /// <param name="expectedMetrics"></param>
        /// <param name="expectedAnimalMetrics"></param>
        /// <param name="expectedMetricInteractions"></param>
        /// <param name="expectedInteractions"></param>
        internal void AssertUserPetsAreCorrect(IList<NopePet> actualUserPets, IList<Pet> expectedUserPets, User owner,
            IList<PetMetric> expectedPetMetrics,
            IList<Metric> expectedMetrics, IList<AnimalMetric> expectedAnimalMetrics,
            IList<MetricInteraction> expectedMetricInteractions,
            IList<Interaction> expectedInteractions)
        {
            foreach (var actualUserPet in actualUserPets)
            {
                var expectedMatch = expectedUserPets.FirstOrDefault(userPet => userPet.PetId == actualUserPet.PetId);
                Assert.IsNotNull(expectedMatch,
                    $"No match found in expected NopePet result for pet with PetId = {actualUserPet.PetId}");
                Assert.AreEqual(actualUserPet.PetName, expectedMatch.Name);
                AssertPetOwnerIsCorrect(actualUserPet.Owner, owner);

                var expectedPetMetricMatches =
                    expectedPetMetrics.Where(petMetric => petMetric.PetId == actualUserPet.PetId).ToList();

                CollectionAssert.IsNotEmpty(expectedPetMetricMatches);
                AssertPetVitalsAreCorrect(actualUserPet.PetHealth, expectedPetMetricMatches, expectedMetrics,
                    expectedAnimalMetrics, expectedMetricInteractions, expectedInteractions);
            }
        }

        internal void AssertPetOwnerIsCorrect(UserProfile actualOwner, User expectedOwner)
        {
            Assert.AreEqual(actualOwner.UserId, expectedOwner.UserId);
            Assert.AreEqual(actualOwner.FirstName, expectedOwner.FirstName);
            Assert.AreEqual(actualOwner.LastName, expectedOwner.LastName);
            Assert.AreEqual(actualOwner.Username, expectedOwner.Username);
        }

        internal void AssertPetVitalsAreCorrect(IList<PetVital> actualPetVitals, IList<PetMetric> expectedPetMetrics,
            IList<Metric> expectedMetrics, IList<AnimalMetric> expectedAnimalMetrics,
            IList<MetricInteraction> expectedMetricInteractions,
            IList<Interaction> expectedInteractions)
        {
            foreach (var actualPetVital in actualPetVitals)
            {
                var expectedPetMetricMatch = expectedPetMetrics.FirstOrDefault(petMetric =>
                    petMetric.PetId == actualPetVital.PetId && petMetric.MetricId == actualPetVital.PetVitalId);

                Assert.IsNotNull(expectedPetMetricMatch,
                    $"No match found in expected PetVitals result for PetMetric with PetId = {actualPetVital.PetId} and MetricId = {actualPetVital.PetVitalId}");
                Assert.AreEqual(actualPetVital.Health, expectedPetMetricMatch.Value);

                var expectedMetricMatch =
                    expectedMetrics.FirstOrDefault(metric => actualPetVital.PetVitalId == metric.MetricId);
                Assert.IsNotNull(expectedMetricMatch);
                Assert.AreEqual(actualPetVital.VitalStats.Name, expectedMetricMatch.Name);
                Assert.AreEqual(actualPetVital.VitalStats.Name, actualPetVital.VitalName);
                Assert.AreEqual(actualPetVital.VitalStats.Description, expectedMetricMatch.Description);
                Assert.AreEqual(actualPetVital.LastTimeCaredFor, expectedPetMetricMatch.LastInteractionTime);

                var expectedAnimalMetricMatches =
                    expectedAnimalMetrics.Where(animalMetric =>
                        animalMetric.MetricId == actualPetVital.PetVitalId &&
                        animalMetric.AnimalId == actualPetVital.VitalStats.SpeciesId).ToList();

                CollectionAssert.IsNotEmpty(expectedAnimalMetricMatches);
                AssertSpeciesVitalIsCorrect(actualPetVital.VitalStats, expectedAnimalMetricMatches,
                    expectedMetricInteractions, expectedInteractions);
            }
        }

        internal void AssertSpeciesVitalIsCorrect(SpeciesVital actualSpeciesVital,
            IList<AnimalMetric> expectedAnimalMetrics,
            IList<MetricInteraction> expectedMetricInteractions, IList<Interaction> expectedInteractions)
        {
            var expectedAnimalMetricMatch = expectedAnimalMetrics.FirstOrDefault(animalMetric =>
                animalMetric.MetricId == actualSpeciesVital.VitalId &&
                animalMetric.AnimalId == actualSpeciesVital.SpeciesId);

            Assert.IsNotNull(expectedAnimalMetricMatch,
                $"No match found in expected AnimalMetrics for SpeciesVital with SpeciedVitalId = {actualSpeciesVital.VitalId} and SpeciesId = {actualSpeciesVital.SpeciesId}");

            Assert.AreEqual(actualSpeciesVital.RequiredAttentiveness, expectedAnimalMetricMatch.RequiredAttentiveness);
            AssertSpeciesCareActionsAreCorrect(actualSpeciesVital.VitalId, actualSpeciesVital.RequiredCareActions,
                expectedMetricInteractions, expectedInteractions);
        }

        internal void AssertSpeciesCareActionsAreCorrect(int actualVitalId,
            IList<SpeciesCareAction> actualSpeciesCareActions, IList<MetricInteraction> expectedMetricInteractions,
            IList<Interaction> expectedInteractions)
        {
            var expectedMetricInteractionMatches = expectedMetricInteractions.Where(metricInteraction =>
                metricInteraction.MetricId == actualVitalId).ToList();

            CollectionAssert.IsNotEmpty(expectedMetricInteractionMatches,
                $"No match found in expected MetricInteractions for SpeciesCareAction where SpeciesVitalId = {actualVitalId}");

            var expectedMatchCount = expectedInteractions.Count / actualSpeciesCareActions.Count;
            Assert.AreEqual(expectedMatchCount, expectedMetricInteractionMatches.Count,
                $"There should be {expectedMatchCount} Interactions for SpeciesVitalId = {actualVitalId}");

            foreach (var speciesCareAction in actualSpeciesCareActions)
            {
                var expectedMetricInteractionMatch = expectedMetricInteractionMatches.FirstOrDefault(
                    metricInteraction => metricInteraction.InteractionId == speciesCareAction.InteractionId);

                Assert.IsNotNull(expectedMetricInteractionMatch,
                    $"No match found in expected MetricInteraction for SpecialCareAction with InteractionId = {speciesCareAction.InteractionId}");

                var expectedInteraction = expectedInteractions.FirstOrDefault(interaction => interaction.InteractionId == speciesCareAction.InteractionId);

                Assert.IsNotNull(expectedInteraction);
                Assert.IsNotNull(expectedMetricInteractionMatch,
                    $"No match found in expected Interactions for SpecialCareAction with InteractionId = {speciesCareAction.InteractionId}");
                AssertCareActionIsCorrect(speciesCareAction, expectedInteraction);
            }
        }

        private void AssertCareActionIsCorrect(SpeciesCareAction actualSpeciesCareAction, Interaction expectedInteraction)
        {
            Assert.AreEqual(actualSpeciesCareAction.Value, expectedInteraction.Value);
            Assert.AreEqual(actualSpeciesCareAction.CooldownTime, expectedInteraction.CooldownTime);
            Assert.AreEqual(actualSpeciesCareAction.CooldownTimeUnit, expectedInteraction.CooldownTimeUnit);
            Assert.AreEqual(actualSpeciesCareAction.Name, expectedInteraction.Name);
            Assert.AreEqual(actualSpeciesCareAction.Description, expectedInteraction.Description);
        }
    }
}

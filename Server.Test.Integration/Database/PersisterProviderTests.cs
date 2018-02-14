using System;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using Server.Configuration;
using Server.Database;
using Server.Database.DataPersisters;
using Server.Database.DataProviders;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Util;

namespace Server.Test.Integration.Database
{
    [TestFixture]
    public class PersisterProviderTests
    {
        private static IConfiguration _config;
        private Encrypter _encrypter;

        [SetUp]
        public void SetUp()
        {
            _config = TestData.TestConfiguration;
            DeleteAllData();
            _encrypter = new Encrypter(_config);
        }

        [TearDown]
        public void TearDown()
        {
            DeleteAllData();
        }

        //Todo - think about dependencies here
        private void DeleteAllData()
        {
            //Join tables
            DeleteData("PetMetrics");
            DeleteData("UserPets");
            DeleteData("MetricInteractions");
            //'Secondary' tables
            DeleteData("Pets");
            //'Primary' tables
            DeleteData("Animals");
            DeleteData("Users");
            DeleteData("Metrics");
            DeleteData("Interactions");
        }

        [Test]
        public void Users_can_be_persisted_and_retrieved()
        {
            //Arrange
            var sutProvider = new DataProvider<User>(_config, User.ToDomainConverter);
            var columns = sutProvider.LoadAllColumns();
            var sutPersister = new UserPersister(columns, _config);
            
            //Act
            ErrorMessage error;
            var persistResult = sutPersister.TryPersist(ref TestData.Users.NewMeJulie, out error);

            //Assert
            Assert.IsTrue(persistResult);
            Assert.IsNull(error);

            //The new row id is assigned to ref of the object
            Assert.IsNotNull(TestData.Users.MeJulie.UserId);
            Assert.IsInstanceOf<int>(TestData.Users.MeJulie.UserId);

            var readResult = sutProvider.LoadAll().First();
            
            Assert.AreEqual(TestData.Users.MeJulie.Email, readResult.Email);
            Assert.AreEqual(TestData.Users.MeJulie.Username, readResult.Username);
            Assert.AreEqual(TestData.Users.MeJulie.FirstName, readResult.FirstName);
            Assert.AreEqual(TestData.Users.MeJulie.LastName, readResult.LastName);

            Assert.AreNotEqual(TestData.Users.MeJulie.Password, readResult.Password);
            Assert.AreEqual(_encrypter.Encrypt(TestData.Users.MeJulie.Password), readResult.Password);
            Assert.AreEqual(TestData.Users.MeJulie.Password, _encrypter.Decrypt(readResult.Password));
        }

        [Test]
        public void Animals_can_be_persisted_and_retrieved()
        {
            //Arrange
            var sutProvider = new DataProvider<Animal>(_config, Animal.ToDomainConverter);
            var columns = sutProvider.LoadAllColumns();

            var sutPersister = new AnimalPersister(columns, _config);
            var animalRecord = TestData.Animals.Chihuahua;

            //Act
            ErrorMessage error;
            var persistResult = sutPersister.TryPersist(ref animalRecord, out error);

            //Assert
            Assert.IsTrue(persistResult);
            Assert.IsNull(error);

            //The new row id is assigned to ref of the object
            Assert.IsNotNull(animalRecord.AnimalId);
            Assert.IsInstanceOf<int>(animalRecord.AnimalId);

            var readResult = sutProvider.LoadAll().First();
            Assert.AreEqual(TestData.Animals.Chihuahua, readResult);
            Assert.AreEqual(animalRecord.AnimalId, readResult.AnimalId);
        }

        [Test]
        public void Metrics_can_be_persisted_and_retrieved()
        {
            //Arrange
            var sutProvider = new DataProvider<Metric>(_config, Metric.ToDomainConverter);
            var columns = sutProvider.LoadAllColumns();

            var sutPersister = new MetricPersister(columns, _config);
            var metricRecord = TestData.AnimalMetrics.Confidence;

            //Act
            ErrorMessage error;
            var persistResult = sutPersister.TryPersist(ref metricRecord, out error);

            //Assert
            Assert.IsTrue(persistResult);
            Assert.IsNull(error);

            //The new row id is assigned to ref of the object
            Assert.IsNotNull(metricRecord.MetricId);
            Assert.IsInstanceOf<int>(metricRecord.MetricId);

            var readResult = sutProvider.LoadAll().First();
            Assert.AreEqual(TestData.AnimalMetrics.Confidence, readResult);
            Assert.AreEqual(metricRecord.MetricId, readResult.MetricId);
        }

        [Test]
        public void Interactions_can_be_persisted_and_retrieved()
        {
            //Arrange
            var sutProvider = new DataProvider<Interaction>(_config, Interaction.ToDomainConverter);
            var columns = sutProvider.LoadAllColumns();

            var sutPersister = new InteractionPersister(columns, _config);
            var interactionRecord = TestData.Interactions.TakeSelfieWithPet;

            //Act
            ErrorMessage error;
            var persistResult = sutPersister.TryPersist(ref interactionRecord, out error);

            //Assert
            Assert.IsTrue(persistResult);
            Assert.IsNull(error);

            //The new row id is assigned to ref of the object
            Assert.IsNotNull(interactionRecord.InteractionId);
            Assert.IsInstanceOf<int>(interactionRecord.InteractionId);

            var readResult = sutProvider.LoadAll().First();
            Assert.AreEqual(interactionRecord, readResult);
        }

        [Test]
        public void Given_an_existing_Metric_and_Interaction_MetricInteractions_can_be_persisted_and_retrieved()
        {
            //Arrange
            var sutProvider = new DataProvider<MetricInteraction>(_config, MetricInteraction.ToDomainConverter);
            var columns = sutProvider.LoadAllColumns();

            var sutPersister = new MetricInteractionPersister(columns, _config);

            var metric = TestData.AnimalMetrics.Confidence;
            var interaction = TestData.Interactions.TakeSelfieWithPet;

            ErrorMessage error;
            if (!PersistMetric(out error, ref metric))
                Assert.Fail(error.Message);

            if(!PersistInteraction(out error, ref interaction))
                Assert.Fail(error.Message);

            //Act
            var metricInteraction = new MetricInteraction(metric.MetricId, interaction.InteractionId);
            var persistResult = sutPersister.TryPersist(ref metricInteraction, out error);

            //Assert
            Assert.IsTrue(persistResult);
            Assert.IsNull(error);

            var readResult = sutProvider.LoadAll().First();
            Assert.AreEqual(metricInteraction, readResult);
        }

        [Test]
        public void Given_a_corresponding_Animal_Pets_can_be_persisted_and_retrieved()
        {
            //Arrange
            var sutProvider = new DataProvider<Pet>(_config, Pet.ToDomainConverter);
            var columns = sutProvider.LoadAllColumns();
            var sutPersister = new PetPersister(columns, _config);
            var animalRecord = TestData.Animals.Chihuahua;


            ErrorMessage error;
            if(!PersistAnimal(out error, ref animalRecord))
                Assert.Fail(error.Message);

            var petRecord = new Pet(animalRecord.AnimalId, "Princess");
            var persistResult = sutPersister.TryPersist(ref petRecord, out error);
            //Assert
            Assert.IsTrue(persistResult);
            Assert.IsNull(error);
            Assert.IsNotNull(petRecord.PetId);
            Assert.IsInstanceOf<int>(petRecord.PetId);

            var readResult = sutProvider.LoadAll().First();
            Assert.AreEqual(petRecord, readResult);
            Assert.AreEqual(petRecord.PetId, readResult.PetId);
        }

        [Test]
        public void Given_an_existing_Pet_and_Metric_PetMetrics_can_be_persisted_and_retrieved()
        {
            //Arrange
            var sutProvider = new DataProvider<PetMetric>(_config, PetMetric.ToDomainConverter);
            var columns = sutProvider.LoadAllColumns();
            var sutPersister = new PetMetricPersister(columns, _config);

            //Database row dependencies
            var metric = TestData.AnimalMetrics.Confidence;
            var animal = TestData.Animals.Chihuahua;

            ErrorMessage error;
            if (!PersistMetric(out error, ref metric))
                Assert.Fail(error.Message);

            if (!PersistAnimal(out error, ref animal))
                Assert.Fail(error.Message);

            var pet = new Pet(animal.AnimalId, "Princess");
            if (!PersistPet(out error, ref pet))
                Assert.Fail(error.Message);

            var petMetricRecord = new PetMetric(pet.PetId, metric.MetricId, 0, DateTime.UtcNow);

            //Act
            var persistResult = sutPersister.TryPersist(ref petMetricRecord, out error);

            //Assert
            Assert.IsTrue(persistResult);
            Assert.IsNull(error);
            var readResult = sutProvider.LoadAll().First();
            Assert.AreEqual(petMetricRecord, readResult);
        }


        [Test]
        public void Given_an_existing_User_and_Pet_UserPets_can_be_persisted_and_retrieved()
        {
            //Arrange
            var sutProvider = new DataProvider<UserPet>(_config, UserPet.ToDomainConverter);
            var columns = sutProvider.LoadAllColumns();
            var sutPersister = new UserPetPersister(columns, _config);

            //Database row dependencies
            var animal = TestData.Animals.Chihuahua;
            var user = TestData.Users.NewMeJulie;

            ErrorMessage error;
            if (!PersistAnimal(out error, ref animal))
                Assert.Fail(error.Message);

            if(!PersistUser(out error, ref user))
                Assert.Fail(error.Message);

            var pet = new Pet(animal.AnimalId, "Princess");   
            if (!PersistPet(out error, ref pet))
                Assert.Fail(error.Message);

            //Create the user pet join with the returned foreign keys
            var userPetRecord = new UserPet(user.UserId, pet.PetId, DateTime.UtcNow);
            var persistResult = sutPersister.TryPersist(ref userPetRecord, out error);

            //Assert
            Assert.IsTrue(persistResult);
            Assert.IsNull(error);
            var readResult = sutProvider.LoadAll().First();
            Assert.AreEqual(userPetRecord, readResult);
        }


        private void DeleteData(string tableName)
        {
            var connection = new SqlConnection(_config.DbConnectionString);
            var command = new SqlCommand($"DELETE FROM {tableName}", connection);
            using (connection)
            {
                connection.Open();
                command.ExecuteNonQuery();
            }

        }

        private bool PersistPet(out ErrorMessage error, ref Pet pet)
        {
            var provider = new DataProvider<Pet>(_config, Pet.ToDomainConverter);
            var columns = provider.LoadAllColumns();
            var persister = new PetPersister(columns, _config);

            return persister.TryPersist(ref pet, out error);
        }

        private bool PersistUser(out ErrorMessage error, ref User persistedUser)
        {
            var provider = new DataProvider<User>(_config, User.ToDomainConverter);
            var columns = provider.LoadAllColumns();
            var persister = new UserPersister(columns, _config);

            return persister.TryPersist(ref persistedUser, out error);
        }

        private bool PersistAnimal(out ErrorMessage error, ref Animal persistedAnimal)
        {
            var provider = new DataProvider<Animal>(_config, Animal.ToDomainConverter);
            var columns = provider.LoadAllColumns();
            var persister = new AnimalPersister(columns, _config);

            return persister.TryPersist(ref persistedAnimal, out error);
        }

        private bool PersistMetric(out ErrorMessage error, ref Metric persistedMetric)
        {
            var provider = new DataProvider<Metric>(_config, Metric.ToDomainConverter);
            var columns = provider.LoadAllColumns();
            var persister = new MetricPersister(columns, _config);

            return persister.TryPersist(ref persistedMetric, out error);
        }

        private bool PersistInteraction(out ErrorMessage error, ref Interaction persistedInteraction)
        {
            var provider = new DataProvider<Interaction>(_config, Interaction.ToDomainConverter);
            var columns = provider.LoadAllColumns();
            var persister = new InteractionPersister(columns, _config);

            return persister.TryPersist(ref persistedInteraction, out error);
        }

    }
}

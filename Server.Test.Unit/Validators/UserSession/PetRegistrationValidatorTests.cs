using Moq;
using NUnit.Framework;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Storage;
using Server.TestingShared;
using Server.Validation.UserSession;

namespace Server.Test.Unit.Validators.UserSession
{
    [TestFixture]
    public class PetRegistrationValidatorTests
    {
        private PetRegistrationValidator _sut;
        private Mock<IRepository<Animal, AnimalMetric>> _mockAnimalRepository;
        private Mock<IRepository<User, UserPet>> _mockUserRepository;
        private Mock<IRepository<Pet, PetMetric>> _mockPetRepository;
        private NopePet _validNewPet;

        [SetUp]
        public void SetUp()
        {
            _mockAnimalRepository = new Mock<IRepository<Animal, AnimalMetric>>();
            _mockUserRepository = new Mock<IRepository<User, UserPet>>();
            _mockPetRepository = new Mock<IRepository<Pet, PetMetric>>();

            _mockAnimalRepository.Setup(m => m.Find(It.IsAny<int>())).Returns(TestData.Animals.Chihuahua);
            _mockUserRepository.Setup(m => m.GetAssociatedIds(It.IsAny<int>()))
                .Returns(TestData.UsersPets.MeJuliesPetIds);
            _mockPetRepository.Setup(m => m.FindMany(TestData.UsersPets.MeJuliesPetIds))
                .Returns(TestData.UsersPets.MeJuliesPets);


            _sut = new PetRegistrationValidator(_mockAnimalRepository.Object, _mockPetRepository.Object, _mockUserRepository.Object);
            _validNewPet = NopePet.NewPet("Princess", TestData.Animals.ChihuahuaAnimalId);
        }

        [Test]
        public void
            Given_a_New_NopePet_request_with_no_payload_then_when_validated_does_not_throw_NRE_and_returns_false_with_correct_error()
        {
            ErrorMessage error;
            Assert.DoesNotThrow(() => _sut.IsValid(TestData.Users.MeJulie, null, out error));
            var result = _sut.IsValid(TestData.Users.MeJulie, null, out error);

            Assert.IsFalse(result);
            Assert.AreEqual(error.Message,
                new ErrorMessage(ErrorCode.MissingField, new[] { nameof(NopePet) }).Message);
        }


        [TestCase(null, 0)]
        [TestCase("", 0)]
        [TestCase(null, 1)]
        [TestCase("", 1)]
        [TestCase("ok", 0)]
        public void
            Given_a_New_NopePet_request_with_missing_payload_fields_then_when_validated_returns_false_with_correct_error(string petName, int speciesId)
        {
            //Arrange
            var payload = new NopePet()
            {
               PetName = petName,
               SpeciesId = speciesId
            };

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(TestData.Users.MeJulie, payload, out error);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(error.Code, ErrorCode.MissingField);

            if (speciesId == default(int))
                StringAssert.Contains(nameof(payload.SpeciesId), error.Message);
            if (string.IsNullOrEmpty(petName))
                StringAssert.Contains(nameof(payload.PetName), error.Message);
        }

        [Test]
        public void
            Given_a_New_NopePet_request_is_not_associated_with_a_known_Animal_then_when_validated_returns_false_with_correct_error()
        {
            //Arrange
            var bogusSpeciesId = 2345;
            var payload = NopePet.NewPet("Princess", bogusSpeciesId);
            _mockAnimalRepository.Setup(m => m.Find(bogusSpeciesId)).Returns((Animal)null);

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(TestData.Users.MeJulie, payload, out error);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(error.Code, ErrorCode.SpeciesDoesNotExist);
        }

        [Test]
        public void
            Given_a_New_NopePet_request_for_which_the_user_already_has_a_NopePet_with_the_same_name_then_when_validated_returns_false_with_correct_error()
        {
            //Arrange
            var payload = NopePet.NewPet(TestData.UsersPets.Versace.Name, TestData.Animals.ChihuahuaAnimalId);

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(TestData.Users.MeJulie, payload, out error);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(error.Code, ErrorCode.PetAlreadyExists);
        }

        [Test]
        public void Given_a_valid_New_NopePet_request_then_when_validated_returns_true_with_no_error()
        {
            //Arrange
            var payload = _validNewPet;

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(TestData.Users.MeJulie, payload, out error);

            //Assert
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

    }
}

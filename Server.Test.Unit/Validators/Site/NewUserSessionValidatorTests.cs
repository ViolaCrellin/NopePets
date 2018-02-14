using Moq;
using NUnit.Framework;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Storage;
using Server.TestingShared;
using Server.Util;
using Server.Validation.Site;

namespace Server.Test.Unit.Validators.Site
{
    [TestFixture]
    public class NewUserSessionValidatorTests
    {
        private NewUserSessionValidator _sut;
        private Mock<IRepository<User, UserPet>> _mockUserRepository;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IRepository<User, UserPet>>();
            var encrypter = new Encrypter(new TestConfiguration());
            _sut = new NewUserSessionValidator(_mockUserRepository.Object, encrypter);
        }

        [TestCase("", null)]
        [TestCase(null, "")]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("User", null)]
        [TestCase(null, "Password")]
        public void
            Given_a_login_request_is_missing_UserCredentials_fields_then_when_validated_returns_false_and_correct_error(string userEmail, string userPassword)
        {
            //Arrange
            var payload = new UserCredentials()
            {
                Email = userEmail,
                Password = userPassword
            };

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(payload, out error);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(error.Code, ErrorCode.MissingField);

            if (string.IsNullOrEmpty(userEmail))
                StringAssert.Contains(nameof(payload.Email), error.Message);
            if (string.IsNullOrEmpty(userPassword))
                StringAssert.Contains(nameof(payload.Password), error.Message);
        }

        [Test]
        public void
            Given_a_login_request_with_an_unrecognised_email_address_then_when_validated_returns_false_and_correct_error()
        {
            //Arrange
            var payload = new UserCredentials()
            {
                Email = "random@random.com",
                Password = "dkflj"
            };

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(payload, out error);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(error.Code, ErrorCode.EmailAddressNotFound);  
        }

        [Test]
        public void
            Given_a_login_request_with_an_incorrect_password_then_when_validated_returns_false_and_correct_error()
        {
            //Arrange
            var payload = new UserCredentials()
            {
                Email = "random@random.com",
                Password = "dkflj"
            };

            User foundUser = TestData.Users.MeJulie;
            _mockUserRepository.Setup(m => m.TryFindUserByEmail(payload.Email, out foundUser)).Returns(true);

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(payload, out error);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(error.Code, ErrorCode.PasswordIncorrect);
        }

        [Test]
        public void
            Given_a_valid_request_then_when_validated_returns_true_and_no_error()
        {
            //Arrange
            var payload = TestData.Users.MeJuliesLogin;

            User foundUser = TestData.Users.MeJulie;
            _mockUserRepository.Setup(m => m.TryFindUserByEmail(payload.Email, out foundUser)).Returns(true);

            //Act
            ErrorMessage error;
            var result = _sut.IsValid(payload, out error);

            //Assert
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

    }
}

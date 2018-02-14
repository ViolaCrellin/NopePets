using Moq;
using NUnit.Framework;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.Model;
using Server.Storage;
using Server.TestingShared;
using Server.Validation;
using Server.Validation.UserSession;

namespace Server.Test.Unit.Validators.UserSession
{
    [TestFixture]
    public class UserSessionRequestValidatorTests
    {
        private UserSessionRequestValidator _sut;
        private Mock<IUserSessionRequestDataValidator<NopePet>> _mockPetRegistrationValidator;
        private Mock<IUserSessionRequestDataValidator<UserPetCareAction>> _mockPetCareActionValidator;
        private User _testUser;
        private Mock<IRepository<User, UserPet>> _mockUserRepository;
        private Mock<IRepository<Pet, PetMetric>> _mockPetRepository;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IRepository<User, UserPet>>();
            _mockPetRepository = new Mock<IRepository<Pet, PetMetric>>();
            _mockPetRegistrationValidator = new Mock<IUserSessionRequestDataValidator<NopePet>>();
            _mockPetCareActionValidator =  new Mock<IUserSessionRequestDataValidator<UserPetCareAction>>();
            _testUser = TestData.Users.MeJulie;

            _sut = new UserSessionRequestValidator(_testUser, _mockUserRepository.Object, _mockPetRepository.Object,
                _mockPetRegistrationValidator.Object, _mockPetCareActionValidator.Object);
        }

        [Test]
        public void
            Given_a_request_with_no_userId_supplied_or_does_not_match_the_validators_user_then_when_validated_returns_false_and_correct_error()
        {

        }

        [Test]
        public void Given_the_request_has_been_given_by_a_recognised_user_a_ReadAll_request_with_no_message_parameters_it_is_not_validated_and_returns_true()
        {

        }

        [Test]
        public void Given_a_new_pet_registration_request_then_this_is_validated_by_the_PetRegistrationValidator()
        {

        }

        [Test]
        public void Given_a_pet_care_action_request_for_a_pet_that_does_not_belong_to_the_user_then_when_validated_returns_false_and_correct_error()
        {

        }

        [Test]
        public void Given_a_pet_care_action_request_for_a_pet_belonging_to_the_user_then_this_is_validated_by_the_PetCareActionValidator()
        {

        }

        [Test]
        public void Given_a_request_of_an_unsupported_data_type_then_when_validated_returns_false_and_correct_error()
        {

        }


    }
}

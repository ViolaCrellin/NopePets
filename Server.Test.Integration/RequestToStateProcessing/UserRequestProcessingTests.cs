using NUnit.Framework;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;

namespace Server.Test.Integration.RequestToStateProcessing
{
    [TestFixture]
    public class UserRequestProcessingTests : RequestProcessingTestBase
    {

        [Test]
        public void Given_a_request_for_a_users_session_data_then_when_processed_returns_correct_data()
        {
            var sut = SetupMockMeJulieLogin();
            var request = new UserSessionRequest<IUserSessionData>()
            {
                RequestType = RequestType.ReadAll,
                RequestParams = null,
                UserId = TestData.Users.MeJulie.UserId
            };

            IResponse response;
            var result = sut.ProcessRequest(request, out response);

            Assert.IsTrue(result);

            Assert.IsNotNull(response as UserSessionResponse);
            var responseData = (response as UserSessionResponse).UserSession;
            AssertMeJuliesSessionDataIsCorrect(responseData);

        }

        [Test]
        public void Given_a_valid_and_persisted_pet_registration_request_then_when_processed_returns_the_new_pet_data_and_updates_the_user_and_pet_repositories()
        {

        }

        [Test]
        public void
            Given_a_valid_and_persisted_pet_interaction_request_then_when_processed_returns_the_new_petMetric_data_and_updates_the_pet_repository()
        {

        }
    }
}

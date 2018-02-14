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
            var siteSitate = InitialiseSiteState();
            
            var sut = InitialiseUserSessionState(TestData.Users.MeJulie, siteSitate.ResponseBuilder);
            var loginRequest = new SiteRequest<ISiteData>()
            {
                RequestParams = TestData.Users.MeJuliesLogin,
                RequestType = RequestType.Read
            };

            IResponse loginResponse;
            siteSitate.ProcessRequest(loginRequest, out loginResponse);
            var request = new UserSessionRequest<IUserSessionData>()
            {
                RequestType = RequestType.ReadAll,
                RequestParams = null
            };

            User foundUser = TestData.Users.MeJulie;
            MockUsers.Setup(m => m.TryFindUserByEmail(TestData.Users.MeJuliesLogin.Email, out foundUser)).Returns(true);

            Assert.IsNull(loginResponse.Error);

            IResponse response;
            var result = sut.ProcessRequest(request, out response);

            Assert.IsFalse(result);

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

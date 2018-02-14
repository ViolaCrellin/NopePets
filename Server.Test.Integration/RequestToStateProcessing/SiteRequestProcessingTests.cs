using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;

namespace Server.Test.Integration.RequestToStateProcessing
{
    [TestFixture]
    public class SiteRequestProcessingTests : RequestProcessingTestBase
    {

        public static IEnumerable InvalidRegistrationRequests
        {
            get
            {
                yield return new TestCaseData(NewUser.EmptyForm, ErrorCode.MissingField);
                yield return new TestCaseData(TestData.NewUsers.InvalidEmailNewUser, ErrorCode.EmailAddressMalformed);
                yield return new TestCaseData(TestData.NewUsers.WeakPasswordNewUser, ErrorCode.PasswordNotStrongEnough);
            }
        }

        public static IEnumerable InvalidLoginRequests
        {
            get
            {
                yield return new TestCaseData(NewUser.EmptyForm, ErrorCode.MissingField);
                yield return new TestCaseData(TestData.NewUsers.InvalidEmailNewUser, ErrorCode.EmailAddressMalformed);
                yield return new TestCaseData(TestData.NewUsers.WeakPasswordNewUser, ErrorCode.PasswordNotStrongEnough);
            }
        }

        [TestCaseSource(nameof(InvalidRegistrationRequests))]
        public void Given_an_invalid_RegistrationRequest_is_received_when_processed_the_correct_error_is_returned_and_UserRepository_is_not_updated(NewUser newUser, ErrorCode errorCode)
        {
            //Arrange 
            var sut = InitialiseSiteState();

            //Act
            var request = new SiteRequest<ISiteData>()
            {
                RequestType = RequestType.Create,
                RequestParams = newUser
            };

            IResponse response;
             var result = sut.ProcessRequest(request, out response);

            Assert.IsFalse(result);
            Assert.IsNotNull(response);
            var errorMessage = (response as SiteResponse)?.Error;
            Assert.AreEqual(errorCode, errorMessage.Code);
            MockUsers.Verify(m => m.Add(It.IsAny<User>()), Times.Never);
        }

        //Some funky Moq stuff I learnt about today :)
        delegate void CallbackUser(ref User user, out ErrorMessage errorMessage);

        [Test]
        public void Given_a_valid_RegistrationRequest_is_received_when_processed_the_correct_data_is_returned_is_returned_and_UserRepository_is_updated()
        {
            //Arrange 
            var sut = InitialiseSiteState();

            ErrorMessage error;
            MockUserPersister.Setup(m => m.TryPersist(ref It.Ref<User>.IsAny, out error)).Callback(new CallbackUser((ref User user, out ErrorMessage errorCallback) =>
            {
                user.UserId = 0;
                errorCallback = null;
            })).Returns(true);

            var request = new SiteRequest<ISiteData>()
            {
                RequestType = RequestType.Create,
                RequestParams = TestData.NewUsers.ValidNewUser
            };

            //Act
            IResponse response;
            var result = sut.ProcessRequest(request, out response);

            Assert.IsTrue(result);
            Assert.IsInstanceOf<RegistrationResponse>(response);
            var registrationResponse = response as RegistrationResponse;
            Assert.IsNotNull(registrationResponse);

            Assert.AreEqual(ResponseResult.Success, registrationResponse?.Result);
            Assert.AreEqual(nameof(NewUser), registrationResponse.DataType);

            var dataResponse = registrationResponse.RegistrationForm;
            Assert.IsNotNull(dataResponse);
            Assert.IsInstanceOf<NewUser>(dataResponse);

            Assert.IsNull(dataResponse.Password);
            Assert.AreEqual(TestData.NewUsers.ValidNewUser, dataResponse);

            MockUsers.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
        }

        [TestCase(false, true, ErrorCode.EmailAddressNotFound)]
        [TestCase(true, false, ErrorCode.PasswordIncorrect)]
        public void Given_an_invalid_NewSessionRequest_is_received_when_processed_the_correct_ErrorMessage_is_returned_is_returned_and_UserRepository_is_updated(bool isEmailMatch, bool isPasswordMatch, ErrorCode errorCode)
        {
            //Arrange 
            var sut = InitialiseSiteState();

            var meJulie = TestData.Users.MeJulie;
            var request = new SiteRequest<ISiteData>()
            {
                RequestType = RequestType.Read,
                RequestParams = new UserCredentials()
                {
                    Email = meJulie.Email,
                    Password = "B1gB@Bylons" //Her password is actually "JungleIsMassive"
                }
            };

            User outUser = meJulie;
            MockUsers.Setup(m => m.TryFindUserByEmail(It.IsAny<string>(), out outUser)).Returns(isEmailMatch);

            //Act
            IResponse response;
            var result = sut.ProcessRequest(request, out response);

            Assert.IsFalse(result);
            Assert.IsNotNull(response);
            var errorMessage = (response as IResponse).Error;
            Assert.AreEqual(errorCode, errorMessage.Code);
            Assert.IsEmpty(sut.GetUserSessions());
        }

 
        [Test]
        public void Given_an_valid_NewSessionRequest_is_received_when_processed_the_users_session_data_is_returned_and_a_UserSession_is_added_to_CurrentUserSessions()
        {
            //Arrange 
            var sut = InitialiseSiteState();

            var meJulie = TestData.Users.MeJulie;
            var request = new SiteRequest<ISiteData>()
            {
                RequestType = RequestType.Read,
                RequestParams = new UserCredentials()
                {
                    Email = TestData.Users.MeJulie.Email,
                    Password = "JungleIsMassive"
                }
            };

            User outUser = meJulie;
            MockUsers.Setup(m => m.TryFindUserByEmail(TestData.Users.MeJulie.Email, out outUser)).Returns(true);
            MockUsers.Setup(m => m.GetUserByEmail(meJulie.Email)).Returns(meJulie);
            SetupMockUserSessionData();

            //Act
            IResponse response;
            var result = sut.ProcessRequest(request, out response);

            Assert.IsTrue(result);
            Assert.IsNotNull(response as UserSessionResponse);
            var responseData = (response as UserSessionResponse).UserSession;
            Assert.AreEqual(1, sut.GetUserSessions().Count);
            AssertMeJuliesSessionDataIsCorrect(responseData);
        }

        

    }
}

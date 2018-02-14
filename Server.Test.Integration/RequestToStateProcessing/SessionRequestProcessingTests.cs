using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;

namespace Server.Test.Integration.RequestToStateProcessing
{
    [TestFixture]
    public class SessionRequestProcessingTests : RequestProcessingTestBase
    {
        [Test]
        public void Given_a_request_for_a_users_session_data_then_when_processed_returns_correct_data()
        {
            var sut = SetupMockMeJulieLogin();
            var request = new UserSessionRequest<UserSession>()
            {
                RequestType = RequestType.ReadAll,
                Payload = null,
                UserId = TestData.Users.MeJulie.UserId,
                PayloadType = typeof(UserSession)
            };

            IResponse response;
            var result = sut.ProcessRequest(request, out response);

            Assert.IsTrue(result);

            Assert.IsNotNull(response as UserSessionResponse);
            var responseData = (response as UserSessionResponse).UserSession;
            AssertMeJuliesSessionDataIsCorrect(responseData);
        }

        //Some funky Moq stuff I learnt about today :)
        delegate void CallbackPet(ref Pet pet, out ErrorMessage errorMessage);

        delegate void CallbackPetMetric(ref PetMetric pet, out ErrorMessage errorMessage);

        [Test]
        public void
            Given_a_valid_and_persisted_pet_registration_request_then_when_processed_returns_the_new_pet_data_and_updates_the_user_and_pet_repositories()
        {
            var sut = SetupMockMeJulieLogin();

            ErrorMessage error;
            MockPetPersister.Setup(m => m.TryPersist(ref It.Ref<Pet>.IsAny, out error))
                .Callback(new CallbackPet((ref Pet pet, out ErrorMessage errorCallback) =>
                {
                    pet.PetId = TestData.UsersPets.BurberryPetId + 1;
                    errorCallback = null;
                })).Returns(true);

            MockPetMetricPersister.Setup(m => m.TryPersist(ref It.Ref<PetMetric>.IsAny, out error))
                .Callback(new CallbackPetMetric((ref PetMetric metric, out ErrorMessage errorCallback) =>
                {
                    metric = new PetMetric(TestData.UsersPets.BurberryPetId + 1,
                        TestData.AnimalMetrics.Confidence.MetricId, 0, DateTime.UtcNow);
                    errorCallback = null;
                })).Returns(true);


            var request = new UserSessionRequest<NopePet>()
            {
                RequestType = RequestType.Create,
                Payload = NopePet.NewPet("Smeagle", TestData.Animals.ChihuahuaAnimalId),
                UserId = TestData.Users.MeJulie.UserId,
                PayloadType = typeof(NopePet)
            };

            IResponse response;

            var result = sut.ProcessRequest(request, out response);

            Assert.IsTrue(result);

            Assert.IsNotNull(response as UserSessionPetResponse);
            var responseData = (response as UserSessionPetResponse).Pets;

            Assert.AreEqual(responseData.Count, 1);
            var newPet = responseData.First(item => item.PetName == "Smeagle");
            AssertPetOwnerIsCorrect(newPet.Owner, TestData.Users.MeJulie);

            //Todo additional asserts      
        }

        [Test]
        public void
            Given_a_valid_and_persisted_pet_interaction_request_then_when_processed_returns_the_new_petMetric_data_and_updates_the_pet_repository()
        {
            //Todo
        }
    }
}

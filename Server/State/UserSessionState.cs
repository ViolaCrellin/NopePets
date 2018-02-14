using System;
using System.Collections.Generic;
using System.Linq;
using Server.Builders;
using Server.Configuration;
using Server.Database.DataPersisters;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.RequestProcessors;
using Server.Storage;
using Server.Util;
using Server.Validation;
using Server.Validation.UserSession;

namespace Server.State
{
    /// <summary>
    /// Is responsible for managing the state of a User and their pets
    /// </summary>
    public class UserSessionState : IState
    {
        public User User { get; set; }
        public IList<Pet> Pets { get; }
        public IList<UserPet> UserPets { get; }
        private readonly Guid _sessionId;
        private readonly DateTime _sessionStart;

        private readonly IRepository<Pet, PetMetric> _petRepository;
        private readonly IRepository<User, UserPet> _userRepository;
        private readonly IUserSessionRequestValidator _validator;
        private readonly ISessionRequestProcessor _sessionRequestProcessor;

        private UserSessionState(User user, IRepository<User, UserPet> userRepository, IRepository<Pet, PetMetric> petRepository,
            IUserSessionRequestValidator validator, ISessionRequestProcessor sessionRequestProcessor)
        {
            User = user;
            Pets = petRepository.FindMany(userRepository.GetAssociatedIds(user.UserId));
            UserPets = userRepository.FindAssociated(user);
            _userRepository = userRepository;
            _petRepository = petRepository;
            _validator = validator;
            _sessionRequestProcessor = sessionRequestProcessor;
            _sessionStart = DateTime.UtcNow;
            _sessionId = Guid.NewGuid();
        }

        /// <summary>
        /// Initialised when user logs in
        /// I probably would have created a container module for this
        /// </summary>
        public static UserSessionState Initialise(User user, IRepository<User, UserPet> userRepository,
            IRepository<Pet, PetMetric> petRepository, IRepository<Animal, AnimalMetric> animalRepository,
            IRepository<Interaction, MetricInteraction> interactionRepository, IConfiguration config,
            UserSessionBuilder userSessionBuilder, UserSessionContainer container)
        {
            //Validator
            var petRegistrationValidator = new PetRegistrationValidator(animalRepository, petRepository, userRepository);

            var petCareActionValidator =
                new PetCareActionValidator(petRepository, interactionRepository);
            var validator = new UserSessionRequestValidator(user, userRepository, petRepository, petRegistrationValidator, petCareActionValidator);

            //Persisters
            var petPersister = container.RecordPersister<Pet>();
            var userPetPersister = container.RecordPersister<UserPet>();
            var petMetricPersister = container.RecordPersister<PetMetric>();

            //Processor
            var sessionRequestProcessor = new SessionRequestProcessor(user, userRepository, petRepository,
                interactionRepository, (PetPersister)petPersister, (UserPetPersister)userPetPersister, (PetMetricPersister)petMetricPersister, userSessionBuilder);
           return new UserSessionState(user, userRepository, petRepository, validator, sessionRequestProcessor);
        }

        public UserSession GetUserSession()
        {
            return _sessionRequestProcessor.ProcessSessionDataRequest(Pets, UserPets, _sessionStart, _sessionId);
        }

        public bool ProcessRequest(IUserSessionRequest<IUserSessionData> request, out IResponse response)
        {
            ErrorMessage error;
            if (!_validator.IsValid(request, out error))
            {
                response = new UserSessionResponse().SetErrorResponse(error);
                return false;
            }

            if (!_sessionRequestProcessor.TryProcess(request, out response) && response != null)
                return false;

            var requestType = request.RequestType;
            if (requestType == RequestType.ReadAll || requestType == RequestType.Read)
                response = ReadFromState(request);
            else
                ApplyToState(requestType, response);

            return true;
        }

        private void ApplyToState(RequestType requestType, IResponse response)
        {
            switch (requestType)
            {
                case RequestType.Create:
                    AddToState(response);
                    break;
                case RequestType.Delete:
                     DeleteFromState(response);
                    break;
                case RequestType.Update:
                     UpdateState(response);
                    break;
                default:
                    return;
            }
        }

        private IResponse ReadFromState(IUserSessionRequest<IUserSessionData> request)
        {
            if (request.RequestParams is UserSession)
            {
                var sessionData = GetUserSession();
                return new UserSessionResponse().SetSuccessResponse(sessionData);
            }
            var error = new ErrorMessage(ErrorCode.UserSessionNotFound);
            return new UserSessionResponse().SetErrorResponse(error);
        }

        private void ReadAllFromState()
        {

        }


        private void UpdateState(IResponse response)
        {
            if (response is UserSessionPetCareResponse)
            {
                var data = (response as UserSessionPetCareResponse).Data;
                var petMetric = data.ToPetMetricModel();
                _petRepository.UpdateAssociated(petMetric.PrimaryId, petMetric.SecondaryId, petMetric);
            }
        }

        private void DeleteFromState(IResponse response)
        {
            throw new NotImplementedException();
        }

        private void AddToState(IResponse response)
        {
            if (response is UserSessionPetResponse)
            {
                var data = (response as UserSessionPetResponse).Pets.First();
                var userPet = data.ToUserPetModel();
                var pet = data.ToPetModel();
                var petMetrics = data.PetHealth.Select(petVital => petVital.ToPetMetricModel()).ToList();
                _petRepository.AddAssociated(petMetrics);
                _userRepository.AddAssociated(userPet);
                _petRepository.Add(pet);
            }
        }

    }

}
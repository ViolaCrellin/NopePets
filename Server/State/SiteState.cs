using System;
using System.Collections.Generic;
using System.Linq;
using Server.Builders;
using Server.Configuration;
using Server.Database.DataProviders;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.RequestProcessors;
using Server.Storage;
using Server.Util;
using Server.Validation;
using Server.Validation.Site;

namespace Server.State
{
    public interface IState
    {
    }

    public class SiteState : IState
    {
        private readonly GameDataState _gameState;
        private readonly IList<UserSessionState> _currentUserSessions;
        private readonly IRepository<User, UserPet> _users;
        private readonly IRepository<Pet, PetMetric> _pets;
        private readonly IRepository<Animal, AnimalMetric> _animals;
        private readonly IRepository<Interaction, MetricInteraction> _interactions;
        private readonly IContainer _userSessionContainer;

        public static bool IsInitalised { get; set; }

        private readonly ISiteRequestProcessor _processor;
        private readonly ISiteRequestValidator _siteRequestValidator;
        private readonly IConfiguration _config;
        public ResponseBuilder ResponseBuilder { get; }

        private SiteState(GameDataState gameState, IRepository<User, UserPet> users, IRepository<Pet, PetMetric> pets,
            IRepository<Animal, AnimalMetric> animals, IRepository<Interaction, MetricInteraction> interactions,
            ISiteRequestProcessor processor,
            ISiteRequestValidator siteRequestValidator, IConfiguration config,
            ResponseBuilder responseBuilder,
            IContainer userSessionContainer)
        {
            _gameState = gameState;
            _currentUserSessions = new List<UserSessionState>();
            _users = users;
            _pets = pets;
            _animals = animals;
            _interactions = interactions;
            ResponseBuilder = responseBuilder;
            _processor = processor;
            _siteRequestValidator = siteRequestValidator;
            _config = config;
            _userSessionContainer = userSessionContainer;
        }


        /// <summary>
        /// State is intialised on applicaton start.
        /// Usually I would use Autofac for this - but it wasn't compatible with WCF without the SVC file.
        /// I opted to not use an SVC file because I like to be able to explicitly state my routes to ensure they are REST
        /// </summary>
        public static SiteState Initialise(IConfiguration config, IRepository<User, UserPet> users,
            IRepository<Pet, PetMetric> pets, IRepository<Animal, AnimalMetric> animals,
            IRepository<Interaction, MetricInteraction> interactions,
            IRepository<Metric, MetricInteraction> metrics, IDataProvider<User> userDataProvider,
            ISiteRequestProcessor requestProcessor, IContainer userSessionContainer)
        {
            if (IsInitalised)
            {
                throw new Exception("The site state is already initialised");
            }

            //Builder 
            var responseBuilder = ResponseBuilder.Initialise(animals, metrics, pets, interactions);

            //GameData
            var gameState = GameDataState.Initialize(config, animals, metrics, interactions, responseBuilder);

            //Validators
            var registrationValidator = new RegistrationValidator(users);
            var userSessionRequestValidator = new NewUserSessionValidator(users, new Encrypter(config));
            var siteValidator = new SiteRequestRequestValidator(registrationValidator, userSessionRequestValidator);

            IsInitalised = true;
            return new SiteState(gameState, users, pets, animals, interactions, requestProcessor, siteValidator, config,
                responseBuilder, userSessionContainer);
        }

        public bool ProcessRequest(ISiteRequest<ISiteData> request, out IResponse response)
        {
            ErrorMessage error;
            if (!_siteRequestValidator.IsValid(request, out error))
            {
                response = new SiteResponse().SetErrorResponse(error);
                return false;
            }

            if (!_processor.TryProcess(request.RequestType, request.Payload, out response) && response != null)
                return false;

            if (request.RequestType == RequestType.Read || request.RequestType == RequestType.ReadAll)
            {
                return TryReadFromState(request.Payload ?? (ISiteData) Activator.CreateInstance(request.PayloadType),
                    out response);
            }


            return TryApplyToState(request, response);
        }

        /// <summary>
        /// Returns the UserSessionState for the given UserRequest
        /// If no UserId is supplied, null is returned. (So check for null!)
        /// </summary>
        /// <param name="sessionRequestUserId"></param>
        /// <returns></returns>
        public UserSessionState GetUserSession(int? sessionRequestUserId)
        {
            return _currentUserSessions.FirstOrDefault(userSession => userSession.User.UserId == sessionRequestUserId);
        }

        public GameDataState GetGameState()
        {
            return _gameState;
        }

        public IList<UserSessionState> GetUserSessions() => _currentUserSessions;

        private bool TryApplyToState<T>(ISiteRequest<T> request, IResponse response)
        {
            switch (request.RequestType)
            {
                case RequestType.Create:
                    AddToState(response);
                    return true;
                case RequestType.Delete:
                    return TryDeleteFromState(out response);
                case RequestType.Update:
                    return TryUpdateState(out response);
                default:
                    return false;
            }
        }

        private bool TryReadFromState(ISiteData requestData, out IResponse response)
        {
            if (requestData is UserCredentials)
            {
                return TryLogin((UserCredentials) requestData, out response);
            }


            response = null;
            return false;
        }

        //I've considered how I would manage state across nodes and services if NopePets was to become some sort 
        // of insane beginners luck hit (LOL). 
        // Hence I left the updating of state to be internal to the server and made the assumption that we would have only one server 
        // so that it would be easier to implement either a message bus system or shard the database appropriately to scale. 
        // So yeah YAGNI until the sudden unexplainable phenomenal success of NopePets
        private void AddToState(dynamic response)
        {
            if (response is RegistrationResponse)
            {
                var data = (response as RegistrationResponse).RegistrationForm;
                _users.Add(data.ToUserModel());
            }
        }

        private bool TryUpdateState(out IResponse response)
        {
            throw new NotImplementedException();
        }

        private bool TryDeleteFromState(out IResponse response)
        {
            throw new NotImplementedException();
        }

        private bool TryLogin(UserCredentials userCredentials, out IResponse response)
        {
            //We have already validated that we've found the user
            var foundUser = _users.GetUserByEmail(userCredentials.Email);

            //But just in case we should check for null
            if (foundUser != null)
            {
                var userSessionState = UserSessionState.Initialise(foundUser, _users, _pets, _animals, _interactions,
                    _config,
                    ResponseBuilder.UserSessionBuilder,
                    _userSessionContainer);

                _currentUserSessions.Add(userSessionState);
                var userSession = userSessionState.GetUserSession();
                response = new UserSessionResponse().SetSuccessResponse(userSession);
                return true;
            }

            var error = new ErrorMessage(ErrorCode.UserSessionNotFound);
            response = new UserSessionResponse().SetErrorResponse(error);
            return false;
        }
    }
}
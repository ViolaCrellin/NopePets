using System;
using System.Collections.Generic;
using System.Linq;
using Server.Builders;
using Server.Database.DataPersisters;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Storage;
using Server.Util;

namespace Server.RequestProcessors
{
    /// <summary>
    /// Processes requests to Create and Delete User Sessions
    /// </summary>
    public class SessionRequestProcessor : ISessionRequestProcessor
    {
        private readonly IRecordPersister<Pet> _petPersister;
        private readonly IRecordPersister<PetMetric> _petMetricPersister;
        private readonly IRecordPersister<UserPet> _userPetPersister;
        private readonly UserSessionBuilder _userSessionBuilder;
        private readonly User _user;
        private readonly IRepository<User, UserPet> _users;
        private readonly IRepository<Pet, PetMetric> _pets;
        private readonly IRepository<Interaction, MetricInteraction> _interactionRepository;

        public SessionRequestProcessor(User user,
            IRepository<User, UserPet> users, IRepository<Pet, PetMetric> pets, IRepository<Interaction, MetricInteraction> interactionRepository,
            IRecordPersister<Pet> petPersister, IRecordPersister<UserPet> userPetPersister, IRecordPersister<PetMetric> petMetricPersister,
            UserSessionBuilder userSessionBuilder)
        {
            _user = user;
            _users = users;
            _pets = pets;
            _interactionRepository = interactionRepository;
            _petPersister = petPersister;
            _userPetPersister = userPetPersister;
            _petMetricPersister = petMetricPersister;
            _userSessionBuilder = userSessionBuilder;
        }

        public UserSession ProcessSessionDataRequest(IList<Pet> pets, IList<UserPet> userPets, DateTime sessionStart, Guid sessionId)
        {
            return _userSessionBuilder.Create(pets, userPets, _user, sessionStart, sessionId);
        }


        public bool TryProcess(IUserSessionRequest<IUserSessionData> request, out IResponse response)
        {
            switch (request.RequestType)
            {
                case RequestType.Create:
                    return TryPersistRecord(request.Payload, out response);
                case RequestType.Update:
                    return TryUpdateRecord(request.Payload, out response);
                case RequestType.Delete:
                    return TryDeleteRecord(request.Payload, out response);
                case RequestType.Read:
                case RequestType.ReadAll:
                {
                    //There's no real processing to do here
                    response = null;
                    return true;
                }
                default:
                {
                    var errorMessage = new ErrorMessage(ErrorCode.RequestTypeNotSupported);
                    response = new UserSessionResponse().SetErrorResponse(errorMessage);
                    return false;
                }

            }
        }

        private bool TryDeleteRecord(IUserSessionData request, out IResponse response)
        {
            throw new NotImplementedException();
        }

        private bool TryUpdateRecord(IUserSessionData requestData, out IResponse response)
        {
            if (requestData is UserPetCareAction)
            {
                var userPetCareAction = requestData  as UserPetCareAction;
                var requestedInteraction = _interactionRepository.Find(userPetCareAction.InteractionId);
                var metricInteraction = _interactionRepository.FindAssociated(requestedInteraction);
                var newPetVital = _userSessionBuilder.RebuildPetVital(metricInteraction, userPetCareAction);
                return TryPersistRecordUpdate(newPetVital, out response);
            }

            response = new UserSessionResponse().SetErrorResponse(new ErrorMessage(ErrorCode.RequestDataNotRecognised));
            return false;
        }

        private bool TryPersistRecordUpdate(IUserSessionData processedData, out IResponse response)
        {
            if (processedData is PetVital)
            {
                var petCareResponse = new UserSessionPetCareResponse();
                var petMetric = (processedData as PetVital).ToPetMetricModel();
                ErrorMessage error;
                if(!_petMetricPersister.TryPersistUpdate(petMetric, out error))
                {
                    response = petCareResponse.SetErrorResponse(error);
                    return false;
                }

                response = petCareResponse.SetSuccessResponse(processedData as PetVital);
                return true;
            }

            var genericError = new ErrorMessage(ErrorCode.RequestDataNotRecognised);
            response = new UserSessionResponse().SetErrorResponse(genericError);
            return false;

        }

        private bool TryPersistRecord(IUserSessionData requestData, out IResponse response)
        {
            if (requestData is NopePet)
            {
               return TryCreateNewPetDbRecord(requestData as NopePet, out response);
            }

            var error = new ErrorMessage(ErrorCode.RequestDataNotRecognised);
            response = new UserSessionResponse().SetErrorResponse(error);
            return false;
        }

        private bool TryCreateNewPetDbRecord(NopePet requestData, out IResponse response)
        {
            var newPet = requestData.ToNewPetModel();

            ErrorMessage errorMessage;
            if (!_petPersister.TryPersist(ref newPet, out errorMessage))
            {
                response = new UserSessionPetResponse().SetErrorResponse(errorMessage);
                return false;
            }

            return  TryCreateNewPetMetricsDbRecord(ref newPet, out response);

        }

        private bool TryCreateNewPetMetricsDbRecord(ref Pet newPet, out IResponse response)
        {
            var nopePet = _userSessionBuilder.CreatePet(newPet, _user);
            var petName = newPet.Name;
            var petVitals = nopePet.First(pet => pet.PetName == petName).PetHealth;
            foreach (var petVital in petVitals)
            {
                var newPetVitals = petVital.ToPetMetricModel();
                ErrorMessage errorMessage;
                if (!_petMetricPersister.TryPersist(ref newPetVitals, out errorMessage))
                {
                    response = new UserSessionPetResponse().SetErrorResponse(errorMessage);
                    return false;
                }
            }
            response = new UserSessionPetResponse().SetSuccessResponse(nopePet);
            return true;
        }
    }
}
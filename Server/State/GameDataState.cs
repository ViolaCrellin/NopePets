using Server.Builders;
using Server.Configuration;
using Server.Database;
using Server.Database.DataPersisters;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.RequestProcessors;
using Server.Storage;
using Server.Validation.GameDataValidators;


namespace Server.State
{
    /// <summary>
    /// GameState is responsible for managing the state of non-userSession specific data definitions
    /// Animals
    /// Metrics
    /// Interactions
    /// </summary>
    public class GameDataState : IState
    {
        private readonly IRepository<Animal, AnimalMetric> _animals;
        private readonly IRepository<Metric, MetricInteraction> _metrics;
        private readonly IRepository<Interaction, MetricInteraction> _interactions;
        private readonly IGameDataRequestProcessor _processor;
        private readonly ResponseBuilder _responseBuilder;
        private readonly GameDataRequestValidator _gameDataRequestValidator;

        private GameDataState(IRepository<Animal, AnimalMetric> animals, IRepository<Metric, MetricInteraction> metrics,
            IRepository<Interaction, MetricInteraction> interactions, IGameDataRequestProcessor processor,
            ResponseBuilder responseBuilder, GameDataRequestValidator gameDataRequestValidator)
        {
            _animals = animals;
            _metrics = metrics;
            _interactions = interactions;
            _processor = processor;
            _responseBuilder = responseBuilder;
            _gameDataRequestValidator = gameDataRequestValidator;
        }

        public static GameDataState Initialize(IConfiguration config, IRepository<Animal, AnimalMetric> animals,
            IRepository<Metric, MetricInteraction> metrics, IRepository<Interaction, MetricInteraction> interactions,
            ResponseBuilder responseBuilder)
        {
            //Persisters
            var animalPersister = new AnimalPersister(animals.ColumnInfo, config);
            var animalMetricPersister = new AnimalMetricPersister(animals.JoinTableColumnInfo, config);
            var metricPersister = new MetricPersister(metrics.ColumnInfo, config);
            var metricInteractionPersister = new MetricInteractionPersister(metrics.JoinTableColumnInfo, config);
            var interactionPersister = new InteractionPersister(interactions.ColumnInfo, config);

            //Validators - TODO
            var gameDataRequestValidator = new GameDataRequestValidator();

            var requestProcessor = new GameDataRequestProcessor(animalPersister, animalMetricPersister, metricPersister,
                metricInteractionPersister, interactionPersister);
            return new GameDataState(animals, metrics, interactions, requestProcessor, responseBuilder,
                gameDataRequestValidator);
        }

        public bool ProcessRequest(IGameDataRequest<IGameData> request, out IResponse response)
        {
            ErrorMessage error;
            if (!_gameDataRequestValidator.IsValid(request, out error))
            {
                response = new GameDataResponse().SetErrorResponse(error);
                return false;
            }

            if (!_processor.TryProcess(request, out response) && response != null)
                return false;

            if (request.RequestType == RequestType.Read)
            {
                response = ReadFromState(request.Payload);
                return true;
            }


            response = ApplyToState(request, response);
            return true;
        }

        private IResponse ApplyToState(IGameDataRequest<IGameData> request, IResponse response)
        {
            switch (request.RequestType)
            {
                case RequestType.Create:
                    return AddToState(request.Payload);
                case RequestType.Delete:
                    return DeleteFromState(response);
                case RequestType.Update:
                    return UpdateState(response);
                default:
                    return null;
            }
        }

        private IResponse ReadFromState(IGameData requestData)
        {
            if (requestData is AnimalData)
            {
                var data = (AnimalData) requestData;
                //Todo
            }

            return new Response<IGameData>();
        }

        private IResponse AddToState(IGameData response)
        {
            if (response is AnimalData)
            {
                var data = (response as AnimalData);
                return new GameDataResponse();
                //Todo add animals and any db dependencies;
            }

            var error = new ErrorMessage(ErrorCode.RequestDataNotRecognised);
            return new GameDataResponse().SetErrorResponse(error);
        }

        private IResponse UpdateState(IResponse response)
        {
            //Todo
            return response;
        }

        private IResponse DeleteFromState(IResponse response)
        {
            //Todo
            return response;
        }
    }
}
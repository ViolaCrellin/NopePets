using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.Builders;
using Server.Database;
using Server.Database.DataPersisters;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Data.User;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Util;

namespace Server.RequestProcessors
{
    public class GameDataRequestProcessor : IGameDataRequestProcessor
    {
        private readonly ResponseBuilder _responseBuilder;
        private AnimalPersister _animalPersister;
        private AnimalMetricPersister _animalMetricPersister;
        private MetricPersister _metricPersister;
        private MetricInteractionPersister _metricInteractionPersister;
        private InteractionPersister _interactionPersister;

        public GameDataRequestProcessor(ResponseBuilder responseBuilder)
        {
            _responseBuilder = responseBuilder;
        }

        public GameDataRequestProcessor(AnimalPersister animalPersister, AnimalMetricPersister animalMetricPersister, MetricPersister metricPersister, MetricInteractionPersister metricInteractionPersister, InteractionPersister interactionPersister)
        {
            this._animalPersister = animalPersister;
            this._animalMetricPersister = animalMetricPersister;
            this._metricPersister = metricPersister;
            this._metricInteractionPersister = metricInteractionPersister;
            this._interactionPersister = interactionPersister;
        }

        public bool TryProcess(IGameDataRequest<IGameData> request, out IResponse response)
        {
            switch (request.RequestType)
            {
                case RequestType.Create:
                    return TryPersistRecord(request, out response);
                case RequestType.Update:
                    return TryUpdateRecord(request, out response);
                case RequestType.Delete:
                    return TryDeleteRecord(request, out response);
                case RequestType.Read:
                {
                    //There's no real processing to do here
                    response = null;
                    return true;
                }
                default:
                {
                    var errorMessage = new ErrorMessage(ErrorCode.RequestTypeNotSupported);
                    response = new GameDataResponse().SetErrorResponse(errorMessage);
                    return false;
                }

            }
        }

        private bool TryPersistRecord(IGameDataRequest<IGameData> request, out IResponse response)
        {
            throw new NotImplementedException();
        }

        private bool TryDeleteRecord(IGameDataRequest<IGameData> request, out IResponse errorMessage)
        {
            throw new NotImplementedException();
        }

        private bool TryUpdateRecord(IGameDataRequest<IGameData> request, out IResponse errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
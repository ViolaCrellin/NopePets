using System;
using Server.Database.DataPersisters;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.DTO.Request;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Util;

namespace Server.RequestProcessors
{
    public class SiteRequestProcessor : ISiteRequestProcessor
    {
        private readonly IRecordPersister<User> _userPersister;

        public SiteRequestProcessor(IRecordPersister<User> userPersister)
        {
            _userPersister = userPersister;
        }

        public bool TryProcess(RequestType requestType, ISiteData requestData, out IResponse response)
        {
            switch (requestType)
            {
                case RequestType.Create:
                    return TryPersistRecord(requestData, out response);
                case RequestType.Update:
                    return TryUpdateRecord(requestData, out response);
                case RequestType.Delete:
                    return TryDeleteRecord(requestData, out response);
                case RequestType.Read:
                {
                    //There's no real processing to do here
                    response = null;
                    return true;
                }
                default:
                {
                    var errorMessage = new ErrorMessage(ErrorCode.RequestTypeNotSupported);
                    response = new Response<ISiteData>();
                    response.SetErrorResponse(errorMessage);
                    return false;
                }
            }
        }


        private bool TryPersistRecord(ISiteData requestData, out IResponse response)
        {
            if (requestData is NewUser)
            {
                var data = ((NewUser) requestData).ToUserModel();
                ErrorMessage errorMessage;
                if (_userPersister.TryPersist(ref data, out errorMessage))
                {              
                    response = new RegistrationResponse().SetSuccessResponse(data.ToRegistrationResponseData());
                    return true;
                }
                response = new RegistrationResponse().SetErrorResponse(errorMessage);
                return false;
            }

            response = null;
            return false;
        }


        private bool TryDeleteRecord(ISiteData requestData, out IResponse response)
        {
            throw new NotImplementedException();
        }


        private bool TryUpdateRecord(ISiteData requestData, out IResponse response)
        {
            throw new NotImplementedException();
        }
    }
}

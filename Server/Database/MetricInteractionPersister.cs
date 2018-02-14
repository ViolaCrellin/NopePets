using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Server.Configuration;
using Server.Database.DataPersisters;
using Server.Database.DataPersisters.Util;
using Server.Database.DataProviders.Util;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;

namespace Server.Database
{
    public class MetricInteractionPersister : RecordPersister<MetricInteraction>
    {
        private readonly SqlConnection _connection;
        private readonly CommandBase<MetricInteraction> _commandBase;

        public MetricInteractionPersister(IList<ColumnInfo<MetricInteraction>> columnInfo, IConfiguration config) : base(columnInfo, config)
        {
            _connection = new SqlConnection(config.DbConnectionString);
            _commandBase = new CommandBase<MetricInteraction>(columnInfo);
        }

        public override bool TryPersist(ref MetricInteraction data, out ErrorMessage error)
        {
            var command = _commandBase.InsertJoinData();
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(MetricInteraction) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.MetricId)}", data.MetricId);
            command.Parameters.AddWithValue($"@{nameof(data.InteractionId)}", data.InteractionId);
            try
            {
                command.Persist();
                error = null;
                return true;
            }
            catch (SqlException e)
            {
                error = new ErrorMessage(ErrorCode.DbPersistenceError, e);
                return false;
            }
        }

        public override bool TryPersistUpdate(MetricInteraction data, out ErrorMessage error)
        {
            throw new NotImplementedException();
        }
    }
}
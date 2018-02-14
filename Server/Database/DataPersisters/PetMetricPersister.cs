using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Server.Configuration;
using Server.Database.DataPersisters.Util;
using Server.Database.DataProviders.Util;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;

namespace Server.Database.DataPersisters
{
    public class PetMetricPersister : RecordPersister<PetMetric>
    {
        private readonly SqlConnection _connection;
        private readonly CommandBase<PetMetric> _commandBase;

        public PetMetricPersister(IList<ColumnInfo<PetMetric>> columnInfo, IConfiguration config) : base(columnInfo, config)
        {
            _connection = new SqlConnection(config.DbConnectionString);
            _commandBase = new CommandBase<PetMetric>(columnInfo);
        }

        public override bool TryPersist(ref PetMetric data, out ErrorMessage error)
        {
            var command = _commandBase.InsertJoinData();
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(PetMetric) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.LastInteractionTime)}", data.LastInteractionTime);
            command.Parameters.AddWithValue($"@{nameof(data.MetricId)}", data.MetricId);
            command.Parameters.AddWithValue($"@{nameof(data.PetId)}", data.PetId);
            command.Parameters.AddWithValue($"@{nameof(data.Value)}", data.Value);
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


        public override bool TryPersistUpdate(PetMetric data, out ErrorMessage error)
        {
            var command = _commandBase.UpdateRow();
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(PetMetric) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.LastInteractionTime)}", data.LastInteractionTime);
            command.Parameters.AddWithValue($"@{nameof(data.MetricId)}Value", data.MetricId);
            command.Parameters.AddWithValue($"@{nameof(data.PetId)}Value", data.PetId);
            command.Parameters.AddWithValue($"@{nameof(data.Value)}", data.Value);
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
    }
}
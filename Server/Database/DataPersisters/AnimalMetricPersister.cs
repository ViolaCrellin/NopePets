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
    public class AnimalMetricPersister : RecordPersister<AnimalMetric>
    {
        private readonly SqlConnection _connection;
        private readonly CommandBase<AnimalMetric> _commandBase;

        public AnimalMetricPersister(IList<ColumnInfo<AnimalMetric>> columnInfo, IConfiguration config) : base(columnInfo, config)
        {
            _connection = new SqlConnection(config.DbConnectionString);
            _commandBase = new CommandBase<AnimalMetric>(columnInfo);
        }

        public override bool TryPersist(ref AnimalMetric data, out ErrorMessage error)
        {
            var command = _commandBase.InsertJoinData();
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(AnimalMetric) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.AnimalId)}", data.AnimalId);
            command.Parameters.AddWithValue($"@{nameof(data.MetricId)}", data.MetricId);
            command.Parameters.AddWithValue($"@{nameof(data.RequiredAttentiveness)}", data.RequiredAttentiveness);
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

        public override bool TryPersistUpdate(AnimalMetric data, out ErrorMessage error)
        {
            throw new NotImplementedException();
        }
    }
}
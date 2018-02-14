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
    public class MetricPersister : RecordPersister<Metric>
    {
        private readonly SqlConnection _connection;
        private readonly CommandBase<Metric> _commandBase;

        public MetricPersister(IList<ColumnInfo<Metric>> columnInfo, IConfiguration config) : base(columnInfo, config)
        {
            _connection = new SqlConnection(config.DbConnectionString);
            _commandBase = new CommandBase<Metric>(columnInfo);
        }

        public override bool TryPersist(ref Metric data, out ErrorMessage error)
        {
            var command = _commandBase.Insert();
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(Metric) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.Name)}", data.Name);
            command.Parameters.AddWithValue($"@{nameof(data.Description)}", data.Description);
            command.Parameters.AddWithValue($"@{nameof(data.NaturalChangeOverTime)}", data.NaturalChangeOverTime);
            command.Parameters.AddWithValue($"@{nameof(data.Type)}", data.Type);
            try
            {
                var metricId = command.PersistReturnId();
                data.MetricId = metricId;
                error = null;
                return true;
            }
            catch (SqlException e)
            {
                error = new ErrorMessage(ErrorCode.DbPersistenceError, e);
                return false;
            }
        }

        public override bool TryPersistUpdate(Metric data, out ErrorMessage error)
        {
            throw new NotImplementedException();
        }
    }
}
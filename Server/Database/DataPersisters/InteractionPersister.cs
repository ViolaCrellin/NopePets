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
    public class InteractionPersister : RecordPersister<Interaction>
    {
        private readonly SqlConnection _connection;
        private readonly CommandBase<Interaction> _commandBase;

        public InteractionPersister(IList<ColumnInfo<Interaction>> columnInfo, IConfiguration config) : base(columnInfo, config)
        {
            _connection = new SqlConnection(config.DbConnectionString);
            _commandBase = new CommandBase<Interaction>(columnInfo);
        }

        public override bool TryPersist(ref Interaction data, out ErrorMessage error)
        {
            var command = _commandBase.Insert();
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(Interaction) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.Name)}", data.Name);
            command.Parameters.AddWithValue($"@{nameof(data.Description)}", data.Description);
            command.Parameters.AddWithValue($"@{nameof(data.CooldownTime)}", data.CooldownTime);
            command.Parameters.AddWithValue($"@{nameof(data.CooldownTimeUnit)}", data.CooldownTimeUnit);
            command.Parameters.AddWithValue($"@{nameof(data.Value)}", data.Value);
            try
            {
                var interactionId = command.PersistReturnId();
                data.InteractionId = interactionId;
                error = null;
                return true;
            }
            catch (SqlException e)
            {
                error = new ErrorMessage(ErrorCode.DbPersistenceError, e);
                return false;
            }
        }

        public override bool TryPersistUpdate(Interaction data, out ErrorMessage error)
        {
            throw new NotImplementedException();
        }
    }
}
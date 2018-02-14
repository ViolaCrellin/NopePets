using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Server.Configuration;
using Server.Database.DataPersisters.Util;
using Server.Database.DataProviders.Util;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;

namespace Server.Database.DataPersisters
{
    public class UserPetPersister : RecordPersister<UserPet>
    {
        private readonly SqlConnection _connection;
        private readonly CommandBase<UserPet> _commandBase;

        public UserPetPersister(IList<ColumnInfo<UserPet>> columnInfo, IConfiguration config) : base(columnInfo, config)
        {
            _connection = new SqlConnection(config.DbConnectionString);
            _commandBase = new CommandBase<UserPet>(columnInfo);
        }

        public override bool TryPersist(ref UserPet data, out ErrorMessage error)
        {
            var command = _commandBase.InsertJoinData();
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(UserPet) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.UserId)}", data.UserId);
            command.Parameters.AddWithValue($"@{nameof(data.PetId)}", data.PetId);
            command.Parameters.AddWithValue($"@{nameof(data.DateBorn)}", data.DateBorn);
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

        public override bool TryPersistUpdate(UserPet data, out ErrorMessage error)
        {
            throw new NotImplementedException();
        }
    }
}
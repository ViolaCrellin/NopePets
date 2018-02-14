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
    public class PetPersister : RecordPersister<Pet>
    {
        private readonly SqlConnection _connection;
        private readonly CommandBase<Pet> _commandBase;

        public PetPersister(IList<ColumnInfo<Pet>> columnInfo, IConfiguration config) : base(columnInfo, config)
        {
            _connection = new SqlConnection(config.DbConnectionString);
            _commandBase = new CommandBase<Pet>(columnInfo);
        }

        public override bool TryPersist(ref Pet data, out ErrorMessage error)
        {
            var command = _commandBase.InsertDataWithForeignKey(nameof(Pet.AnimalId));
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(Pet) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.Name)}", data.Name);
            command.Parameters.AddWithValue($"@{nameof(data.AnimalId)}", data.AnimalId);
            try
            {
                var petId = command.PersistReturnId();
                data.PetId = petId;
                error = null;
                return true;
            }
            catch (SqlException e)
            {
                error = new ErrorMessage(ErrorCode.DbPersistenceError, e);
                return false;
            }
        }

        public override bool TryPersistUpdate(Pet data, out ErrorMessage error)
        {
            throw new NotImplementedException();
        }
    }
}
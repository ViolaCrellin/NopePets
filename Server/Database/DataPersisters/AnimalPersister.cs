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
    public class AnimalPersister : RecordPersister<Animal>
    {
        private readonly SqlConnection _connection;
        private readonly CommandBase<Animal> _commandBase;

        public AnimalPersister(IList<ColumnInfo<Animal>> columnInfo, IConfiguration config) : base(columnInfo, config)
        {
            _connection = new SqlConnection(config.DbConnectionString);
            _commandBase = new CommandBase<Animal>(columnInfo);
        }

        public override bool TryPersist(ref Animal data, out ErrorMessage error)
        {
            var command = _commandBase.Insert();
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(Animal) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.Description)}", data.Description);
            command.Parameters.AddWithValue($"@{nameof(data.SpeciesName)}", data.SpeciesName);
            try
            {
                var animalId = command.PersistReturnId();
                data.AnimalId = animalId;
                error = null;
                return true;
            }
            catch (SqlException e)
            {
                error = new ErrorMessage(ErrorCode.DbPersistenceError, e);
                return false;
            }
        }

        public override bool TryPersistUpdate(Animal data, out ErrorMessage error)
        {
            throw new NotImplementedException();
        }
    }
}
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Server.Configuration;
using Server.Database.DataPersisters.Util;
using Server.Database.DataProviders.Util;
using Server.MasterData.DTO.Response;
using Server.MasterData.Model;
using Server.Util;

namespace Server.Database.DataPersisters
{
    public class UserPersister : RecordPersister<User>
    {
        private readonly SqlConnection _connection;
        private readonly CommandBase<User> _commandBase;
        private readonly Encrypter _encrypter;

        public UserPersister(IList<ColumnInfo<User>> columnInfo, IConfiguration config) : base(columnInfo, config)
        {
            _connection = new SqlConnection(config.DbConnectionString);
           _commandBase = new CommandBase<User>(columnInfo);
            _encrypter = new Encrypter(config);
        }

        public override bool TryPersist(ref User data, out ErrorMessage error)
        {
            string encryptedPassword;
            try
            {
                encryptedPassword = _encrypter.Encrypt(data.Password);
            }
            catch (CryptographicException e)
            {
                error = new ErrorMessage(ErrorCode.PasswordEncryptionFailure, e);
                return false;
            }

            var command = _commandBase.Insert();
            command.Connection = _connection;
            command.Parameters.AddWithValue($@"tableName", nameof(User) + 's');
            command.Parameters.AddWithValue($"@{nameof(data.Email)}", data.Email);
            command.Parameters.AddWithValue($"@{nameof(data.Password)}", encryptedPassword);
            command.Parameters.AddWithValue($"@{nameof(data.FirstName)}", data.FirstName);
            command.Parameters.AddWithValue($"@{nameof(data.LastName)}", data.LastName);
            command.Parameters.AddWithValue($"@{nameof(data.Username)}", data.Username);
            try
            {
                var userId = command.PersistReturnId();
                data.UserId = userId;
                error = null;
                return true;
            }
            catch (SqlException e)
            {
                error = new ErrorMessage(ErrorCode.DbPersistenceError, e);
                return false;
            }
        }

        public override bool TryPersistUpdate(User data, out ErrorMessage error)
        {
            throw new System.NotImplementedException();
        }
    }
}
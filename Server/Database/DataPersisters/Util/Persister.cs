using System.Data;
using System.Data.SqlClient;
using Server.Configuration;
using Server.MasterData.DTO.Response;

namespace Server.Database.DataPersisters.Util
{
    public static class Persister
    {
        public static int PersistReturnId(this SqlCommand command)
        {
            if(command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            var result = command.ExecuteScalar();
            return int.Parse(result.ToString());
        }

        public static void Persist(this SqlCommand command)
        {
            if(command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            command.ExecuteNonQuery();
        }
    }
}
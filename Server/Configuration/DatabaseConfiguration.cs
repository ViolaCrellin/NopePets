using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Server.Configuration
{
    public interface IConfiguration
    {
        string DbConnectionString { get; set; }
        string Key { get; }
        string Vector { get; }
    }

    //Ideally this would be read out of a config file 
    public class DatabaseConfiguration : IConfiguration
    {
        public string DbConnectionString { get; set; }
        public string Key { get; }
        public string Vector { get; }

        public DatabaseConfiguration(string dbName)
        {
            Key = "Sup3rS3cr3tK3%";
            Vector = "Wh@t'5Y0urV3ct0rV1ct0r?";
            DbConnectionString = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;          
        }
      
    }
}
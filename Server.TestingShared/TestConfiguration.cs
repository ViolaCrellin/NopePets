using Server.Configuration;

namespace Server.TestingShared
{
    public class TestConfiguration : IConfiguration
    {

        public string DbConnectionString { get; set; }
        public string Key { get; }
        public string Vector { get; }

        public TestConfiguration()
        {
            Key = "Sup3rS3cr3tK3%";
            Vector = "Wh@t'5Y0urV3ct0rV1ct0r?";
            DbConnectionString =
                "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Viola Creliin-Davies\\Desktop\\NopePets\\Server.Test.Integration\\App_Data\\NopePets_Test.mdf\";Integrated Security=True";
        }

    }
}

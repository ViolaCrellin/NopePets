using System.Runtime.Serialization;

namespace Server.MasterData.DTO.Data.Site
{
    [DataContract]
    public class MenuData : ISiteData
    {
        public static MenuData LandingPage = new MenuData();

        public MenuData()
        {
            Header = "Welcome To NopePets";
            RegisterPath = UriRoutes.Site.Register;
            GameData = UriRoutes.GameData.All;
        }

        [DataMember]
        public string Header { get; set; }

        [DataMember]
        public string RegisterPath { get; set; }

        [DataMember]
        public string GameData { get; set; }

    }
}
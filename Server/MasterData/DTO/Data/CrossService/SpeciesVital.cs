using System.Collections.Generic;
using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Data.User;

namespace Server.MasterData.DTO.Data.CrossService
{
    public enum RequiredAttentiveness
    {
        Constantly,
        Minutely,
        Hourly,
        Daily,
        Weekly,
    }

    [DataContract]
    public class SpeciesVital : IUserSessionData, IGameData
    {
        public SpeciesVital()
        {
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public RequiredAttentiveness RequiredAttentiveness { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<SpeciesCareAction> RequiredCareActions { get; set; }

        public int VitalId { get; set; }
        public int SpeciesId { get; set; }
    }
}
using System.Collections.Generic;
using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.Game;

namespace Server.MasterData.DTO.Data.CrossService
{
    [DataContract]
    public class Species : IGameData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<SpeciesVital> HealthMetrics { get; set; }

        [DataMember]
        public List<SpeciesCareAction> CareActions { get; set; }

        [DataMember]
        public int SpeciesId { get; set; }
    }
}
using System.Collections.Generic;
using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.CrossService;

namespace Server.MasterData.DTO.Data.Game
{
    [DataContract]
    public class AnimalData : IGameData
    {
        [DataMember]
        public List<Species> Animals { get; }
    }
}
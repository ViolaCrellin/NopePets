using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Server.MasterData.DTO.Data.Game;
using Server.MasterData.DTO.Data.User;

namespace Server.MasterData.DTO.Data.CrossService
{
    public enum CooldownTimeUnit
    {
        Second,
        Minute,
        Hour,
        Day,
        Week
    }

    [DataContract]
    public class SpeciesCareAction : IUserSessionData, IGameData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public CooldownTimeUnit CooldownTimeUnit { get; set; }

        [DataMember]
        public int CooldownTime { get; set; }

        [DataMember]
        public int Value { get; set; }

        public int InteractionId { get; set; }
    }
}
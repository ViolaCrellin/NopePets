using System;
using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.CrossService;

namespace Server.MasterData.DTO.Data.User
{
    [DataContract]
    public class PetVital : IUserSessionData
    {
        [DataMember]
        public string VitalName { get; set; }

        [DataMember]
        public int Health { get; set; }

        [DataMember]
        public TimeSpan TimeNeglectedFor { get; set; }

        [DataMember]
        public int HealthDeclineDuringNeglect { get; set; }

        [DataMember]
        public DateTime LastTimeCaredFor { get; set; }

        [DataMember]
        public SpeciesVital VitalStats { get; set; }

        public int PetVitalId { get; set; }

        public int PetId { get; set; }
    }
}
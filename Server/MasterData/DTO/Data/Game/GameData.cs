using System.Collections.Generic;
using System.Runtime.Serialization;
using Server.MasterData.DTO.Data.User;

namespace Server.MasterData.DTO.Data.Game
{
    [DataContract]
    public class GameData
    {
        [DataMember]
        public AnimalData Animals { get; }

        [DataMember]
        public List<UserProfile> Users { get; }

        [DataMember]
        public List<NopePet> UsersNopePets { get; }

    }
}
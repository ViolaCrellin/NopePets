using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server
{
    public static class UriRoutes
    {
        public static class Site
        {
            public const string Home = "";
            public const string Register = "/Register";
            public const string LoginForm = "/LoginForm";
        }

        public static class Session
        {
            public const string User = "/";
            public const string LoginRequest = "/Login";
            public const string PetRegistration = "/PetRegistration";
            public const string AllMyPets = "/User?Username={userName}/MyPets";
            public const string MyPet = "/User?Username={userName}/MyPets?PetName={Name}";
            public const string InteractWithMyPet = "/User?UserId={userId}/MyPets?PetId={petId}/Interaction?InteractionId={interactionId}";
        }

        public static class GameData
        {
            public const string Animals = "/Animals";
            public const string All = "/GameData";
        }
    }
}
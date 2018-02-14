using System;
using System.Collections.Generic;
using Server.Configuration;
using Server.Database.DataProviders.Util;
using Server.MasterData.DTO.Data.CrossService;
using Server.MasterData.DTO.Data.Site;
using Server.MasterData.Model;

namespace Server.TestingShared
{
    public static class TestData
    {
        public static IConfiguration TestConfiguration = new TestConfiguration();

        public static class Columns
        {
            public static IList<ColumnInfo<User>> UserColumns()
            {
                return new List<ColumnInfo<User>>()
                {
                    new ColumnInfo<User>("UserId", "int", true),
                    new ColumnInfo<User>("Email", "string", false),
                    new ColumnInfo<User>("Username", "string", false),
                    new ColumnInfo<User>("Password", "string", false),
                    new ColumnInfo<User>("FirstName", "string", false),
                    new ColumnInfo<User>("LastName", "string", false)
                };
            }

            public static IList<ColumnInfo<Pet>> PetColumns()
            {
                return new List<ColumnInfo<Pet>>()
                {
                    new ColumnInfo<Pet>("PetId", "int", true),
                    new ColumnInfo<Pet>("AnimalId", "int", true),
                    new ColumnInfo<Pet>("Name", "string", false)
                };
            }

            public static IList<ColumnInfo<UserPet>> UserPetColumns()
            {
                return new List<ColumnInfo<UserPet>>()
                {
                    new ColumnInfo<UserPet>("UserId", "int", true),
                    new ColumnInfo<UserPet>("PetId", "int", true),
                    new ColumnInfo<UserPet>("DateBorn", "DateTime", false)
                };
            }
        }

        public static class NewUsers
        {
            public static NewUser InvalidEmailNewUser = new NewUser()
            {
                Email = "bogus email",
                Password = "Zerg4Lyfe",
                FirstName = "Sasha",
                SecondName = "Hostyn",
                Username = "Scarlett"
            };

            public static NewUser WeakPasswordNewUser = new NewUser()
            {
                Email = "Tom.Muggins@gmail.com",
                Password = "tom",
                FirstName = "Tom",
                SecondName = "Muggins",
                Username = "HackersDelight"
            };

            public static NewUser ValidNewUser = new NewUser()
            {
                Email = "actualemail@realemail.com",
                Password = "R@nd0mP@55w0rd",
                FirstName = "Justin",
                SecondName = "Credible",
                Username = "UBetterBelieveIt"
            };
        }

        public static class Users
        {
            public static UserCredentials MeJuliesLogin = new UserCredentials()
            {
                Password = "JungleIsMassive",
                Email = "MeJulie@AliGsCrib.com"
            };
            public static int MeJulieUserId = 1000;
            public static User MeJulie = new User("Me", "Julie", MeJulieUserId, "MeJulie@AliGsCrib.com", "RD8ihaProWRdvQ+MHtKK7A==", "MeJulie");
            public static User NewMeJulie = new User("Me", "Julie", "MeJulie@AliGsCrib.com", "RD8ihaProWRdvQ+MHtKK7A==", "MeJulie");

        }

        public static class Animals
        {
            public static int ChihuahuaAnimalId = 11;
            public static Animal Chihuahua = new Animal(ChihuahuaAnimalId, "Chihuahua - Dog",
                "The perfect little companion and fashion accessory, although be warned they can get yappy.");
        }

        public static class UsersPets
        {
            public static int VersacePetId = 1;
            public static int BurberryPetId = 2;
            public static Pet Versace = new Pet(VersacePetId, Animals.ChihuahuaAnimalId, "Versace");
            public static Pet Burberry = new Pet(BurberryPetId, Animals.ChihuahuaAnimalId, "Burberry");

            public static UserPet MeJuliesVersace = new UserPet(Users.MeJulieUserId, VersacePetId, DateTime.UtcNow);
            public static UserPet MeJuliesBurberry = new UserPet(Users.MeJulieUserId, BurberryPetId, DateTime.UtcNow);

            public static IList<Pet> MeJuliesPets = new List<Pet>()
            {
                Versace,
                Burberry
            };

            public static IList<int> MeJuliesPetIds = new List<int>()
            {
                VersacePetId,
                BurberryPetId
            };

            public static PetMetric VersaceConfidenceMetric = new PetMetric(VersacePetId, AnimalMetrics.ConfidenceMetricId, 0, DateTime.UtcNow);
            public static PetMetric VersaceHungerMetric = new PetMetric(VersacePetId, AnimalMetrics.HungerMetricId, 0, DateTime.UtcNow);

            public static IList<PetMetric> VersacePetMetrics = new List<PetMetric>()
            {
                VersaceConfidenceMetric,
                VersaceHungerMetric
            };

            public static PetMetric BurberryPetMetric = new PetMetric(BurberryPetId, AnimalMetrics.ConfidenceMetricId, 0, DateTime.UtcNow);
            public static PetMetric BurberryHungerMetric = new PetMetric(BurberryPetId, AnimalMetrics.HungerMetricId, 0, DateTime.UtcNow);

            public static IList<PetMetric> BurberryPetMetrics = new List<PetMetric>()
            {
                BurberryPetMetric,
                BurberryHungerMetric
            };
        }

        public static class AnimalMetrics
        {
            public static int HungerMetricId = 10;
            public static Metric Hunger = new Metric(10, "Hunger", "Your pet has to eat regularly or else he will starve!",
                MetricType.IncreasesWithTime, 3);

            public static int ConfidenceMetricId = 20;
            public static Metric Confidence = new Metric(ConfidenceMetricId, "Confidence", "Sometimes your pet just wants reassurance that they are looking on point", MetricType.DecreasesWithTime, 4);

            public static IList<int> ChihuahuaMetricIds = new List<int>()
            {
                ConfidenceMetricId,
                HungerMetricId
            };

            public static AnimalMetric ChihuahuaIncreasingMetric = new AnimalMetric(Animals.Chihuahua.AnimalId, HungerMetricId, RequiredAttentiveness.Daily);
            public static AnimalMetric ChihuahuaDecreasingMetric = new AnimalMetric(Animals.Chihuahua.AnimalId, ConfidenceMetricId, RequiredAttentiveness.Constantly);

            public static IList<AnimalMetric> ChihuahuaMetrics = new List<AnimalMetric>()
            {
                ChihuahuaDecreasingMetric,
                ChihuahuaIncreasingMetric
            };


        }

        public static class Interactions
        {
            public static int FeedMealInteractionId = 100;
            public static int FeedTreatInteractionId = 101;
            public static MetricInteraction HungerFeedMeal = new MetricInteraction(AnimalMetrics.HungerMetricId, FeedMealInteractionId);
            public static MetricInteraction HungerFeedTreat = new MetricInteraction(AnimalMetrics.HungerMetricId, FeedTreatInteractionId);
            public static Interaction FeedMeal = new Interaction(FeedMealInteractionId, "Feed meal", "Don't let your pet starve! (But also don't let them get fat...)", 40, 6, CooldownTimeUnit.Hour);
            public static Interaction FeedTreat = new Interaction(FeedTreatInteractionId, "Feed treat", "Who's a good boy?", 2, 5, CooldownTimeUnit.Minute);

            public static IList<MetricInteraction> HungerMetricInteractions = new List<MetricInteraction>()
            {
                HungerFeedMeal,
                HungerFeedTreat
            };

            public static IList<Interaction> HungerInteractions = new List<Interaction>()
            {
                FeedMeal,
                FeedTreat
            };

            public static int TakeSelfieWithInteractionId = 200;
            public static int BuyNewOutfitForPetInteractionId = 201;
            public static MetricInteraction ConfidenceTakeSelfieWithPet = new MetricInteraction(AnimalMetrics.ConfidenceMetricId, TakeSelfieWithInteractionId);
            public static MetricInteraction ConfidenceBuyNewOutfit = new MetricInteraction(AnimalMetrics.ConfidenceMetricId, BuyNewOutfitForPetInteractionId);

            public static Interaction TakeSelfieWithPet = new Interaction(TakeSelfieWithInteractionId,
                "Take selfie with pet", "Show the world how great your pet is, make sure you put it on Insta", 3, 20,
                CooldownTimeUnit.Minute);

            public static Interaction BuyNewOutfitForPet = new Interaction(BuyNewOutfitForPetInteractionId,
                "Buy a new outfit for your pet",
                "Keep your pet up with the latest fashion, I'm sure they do *really* appreciate it deep down", 100, 4,
                CooldownTimeUnit.Week);

            public static IList<MetricInteraction> ConfidenceMetricInteractions = new List<MetricInteraction>()
            {
                ConfidenceTakeSelfieWithPet,
                ConfidenceBuyNewOutfit
            };

            public static IList<Interaction> ConfidenceInteractions = new List<Interaction>()
            {
                BuyNewOutfitForPet,
                TakeSelfieWithPet
            };

        }

    }
}


using System.Collections.Generic;
using CitizenFX.Core;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Main.Core.Server.Enums;

namespace PoliceMP.Server.Controllers.AiCallouts
{
    public class AiCalloutsPeds : Controller
    {
        public List<PedHash> DomesticAnimals = new List<PedHash>();
        public List<PedHash> FarmAnimals = new List<PedHash>();

        public static List<PedHash> LowLevelCriminal = new List<PedHash>()
        {
            PedHash.Hillbilly01AMM,
            PedHash.Hillbilly02AMM,
            PedHash.ArmGoon01GMM,
            PedHash.ArmGoon02GMY,
            PedHash.MexLabor01AMM,
            PedHash.MethFemale01,
            PedHash.MethMale01,
            PedHash.Methhead01AMY,
            PedHash.MexThug01AMY,
            PedHash.Rurmeth01AFY,
            PedHash.Rurmeth01AMM,
            PedHash.Skidrow01AFM,
            PedHash.Skidrow01AMM,
            PedHash.Tramp01,
            PedHash.Tramp01AFM,
            PedHash.Tramp01AMM,
            PedHash.Tramp01AMO,
            PedHash.TrampBeac01AFM,
            PedHash.TrampBeac01AMM,
            PedHash.Salton01AMO,
            PedHash.Soucent02AMO,
            PedHash.Tattoo01AMO,
            PedHash.BurgerDrug,
            PedHash.RussianDrunk
        };


        public static List<PedHash> FireInspectors = new List<PedHash>()
        {
            PedHash.Golfer01AMM,
            PedHash.Malibu01AMM,
            PedHash.Beach02AMM,
            PedHash.Business01AMY,
            PedHash.Business02AFY,
            PedHash.SmartCasPat01AMY,
            PedHash.Stbla02AMY,
            PedHash.Barry,
            PedHash.Bankman,
            PedHash.Bankman01,
            PedHash.TaoCheng,
            PedHash.Josh,
            PedHash.Milton,
            PedHash.Paper,
            PedHash.TaosTranslator,
            PedHash.TomEpsilon,
            PedHash.Bevhills01AFM,
            PedHash.Bevhills01AFY,
            PedHash.Bevhills01AMY,
            PedHash.Bevhills02AFM,
            PedHash.Bevhills02AFY,
            PedHash.Bevhills02AMY,
            PedHash.Bevhills03AFY,
            PedHash.Bevhills04AFY,
            PedHash.Business01AFY,
            PedHash.Fitness01AFY
        };

        public static List<PedHash> AmbientFemales = new List<PedHash>()
        {
            PedHash.Beach01AFM,
            PedHash.Bevhills01AFM,
            PedHash.Bodybuild01AFM,
            PedHash.Business01AFY,
            PedHash.Downtown01AFM,
            PedHash.Eastsa01AFM,
            PedHash.Eastsa02AFM,
            PedHash.FatBla01AFM,
            PedHash.FatCult01AFM,
            PedHash.FatWhite01AFM,
            PedHash.Ktown01AFM,
            PedHash.Ktown02AFM,
            PedHash.PrologueHostage01AFM,
            PedHash.Salton01AMO,
            PedHash.Skidrow01AFM,
            PedHash.Soucent01AFM,
            PedHash.Soucent02AFM,
            PedHash.Soucent01AFM,
            PedHash.TrampBeac01AFM,
            PedHash.Genstreet01AFO,
            PedHash.Indian01AFO,
            PedHash.Ktown01AFO,
            PedHash.Soucent01AFO,
            PedHash.Beach01AFY,
            PedHash.Bevhills01AFY,
            PedHash.Bevhills02AFY,
            PedHash.Bevhills03AFY,
            PedHash.Bevhills04AFY,
            PedHash.Business01AFY,
            PedHash.Business02AFY,
            PedHash.Business03AFY,
            PedHash.Business04AFY,
            PedHash.Eastsa01AFY,
            PedHash.Eastsa02AFY,
            PedHash.Eastsa03AFY,
            PedHash.Epsilon01AFY,
            PedHash.Fitness01AFY,
            PedHash.Fitness02AFY,
            PedHash.Genhot01AFY,
            PedHash.Golfer01AFY,
            PedHash.Hiker01AFY,
            PedHash.Hippie01AFY,
            PedHash.Hipster01AFY,
            PedHash.Hipster02AFY,
            PedHash.Hipster03AFY,
            PedHash.Hipster04AFY,
            PedHash.Indian01AFY,
            PedHash.Juggalo01AFY,
            PedHash.Runner01AFY,
            PedHash.Scdressy01AFY,
            PedHash.Skater01AFY,
            PedHash.Soucent01AFY,
            PedHash.Soucent02AFY,
            PedHash.Soucent03AFY,
            PedHash.Tennis01AFY,
            PedHash.Topless01AFY,
            PedHash.Tourist02AFY,
            PedHash.Vinewood01AFY,
            PedHash.Vinewood02AFY,
            PedHash.Vinewood03AFY,
            PedHash.Vinewood04AFY,
            PedHash.Yoga01AFY,
            PedHash.SmartCasPat01AMY

        };

        public static List<PedHash> AmbientMales = new List<PedHash>()
        {
            PedHash.AfriAmer01AMM,
            PedHash.Beach01AMM,
            PedHash.Beach02AMM,
            PedHash.Bevhills01AMM,
            PedHash.Bevhills02AMM,
            PedHash.Business01AMM,
            PedHash.Eastsa01AMM,
            PedHash.Eastsa02AMM,
            PedHash.Farmer01AMM,
            PedHash.Fatlatin01AMM,
            PedHash.Genfat01AMM,
            PedHash.Genfat02AMM,
            PedHash.Golfer01AMM,
            PedHash.Hasjew01AMM,
            PedHash.Hillbilly01AMM,
            PedHash.Hillbilly02AMM,
            PedHash.Indian01AMM,
            PedHash.Ktown01AMM,
            PedHash.Malibu01AMM,
            PedHash.MexCntry01AMM,
            PedHash.MexLabor01AMM,
            PedHash.OgBoss01AMM,
            PedHash.Paparazzi01AMM,
            PedHash.Polynesian01AMM,
            PedHash.PrologueHostage01AMM,
            PedHash.Rurmeth01AMM,
            PedHash.Salton01AMM,
            PedHash.Salton02AMM,
            PedHash.Salton03AMM,
            PedHash.Salton04AMM,
            PedHash.Skater01AMM,
            PedHash.Skidrow01AMM,
            PedHash.Socenlat01AMM,
            PedHash.Soucent01AMM,
            PedHash.Soucent02AMM,
            PedHash.Soucent03AMM,
            PedHash.Soucent04AMM,
            PedHash.Stlat02AMM,
            PedHash.Tennis01AMM,
            PedHash.Tourist01AMM,
            PedHash.Tramp01AMM,
            PedHash.TrampBeac01AMM,
            PedHash.Beach01AMO,
            PedHash.Genstreet01AMO,
            PedHash.Ktown01AMO,
            PedHash.Salton01AMO,
            PedHash.Soucent01AMO,
            PedHash.Soucent02AMO,
            PedHash.Soucent03AMO,
            PedHash.Tramp01AMO,
            PedHash.Beach01AMY,
            PedHash.Beach02AMY,
            PedHash.Beach03AMY,
            PedHash.Beachvesp01AMY,
            PedHash.Beachvesp02AMY,
            PedHash.Bevhills01AMY,
            PedHash.Bevhills02AMY,
            PedHash.Breakdance01AMY,
            PedHash.Busicas01AMY,
            PedHash.Business01AMY,
            PedHash.Business02AMY,
            PedHash.Business03AMY,
            PedHash.Cyclist01AMY,
            PedHash.Dhill01AMY,
            PedHash.Downtown01AMY,
            PedHash.Eastsa01AMY,
            PedHash.Eastsa02AMY,
            PedHash.Epsilon01AMY,
            PedHash.Epsilon02AMY,
            PedHash.Gay01AMY,
            PedHash.Gay02AMY,
            PedHash.Genstreet01AMY,
            PedHash.Genstreet02AMY,
            PedHash.Golfer01AMY,
            PedHash.Hasjew01AMY,
            PedHash.Hiker01AMY,
            PedHash.Hippy01AMY,
            PedHash.Hipster01AMY,
            PedHash.Hipster02AMY,
            PedHash.Hipster03AMY,
            PedHash.Indian01AMY,
            PedHash.Jetski01AMY,
            PedHash.Juggalo01AMY,
            PedHash.Ktown01AMY,
            PedHash.Ktown02AMY,
            PedHash.Latino01AMY,
            PedHash.Methhead01AMY,
            PedHash.MexThug01AMY,
            PedHash.Motox01AMY,
            PedHash.Motox02AMY,
            PedHash.Musclbeac01AMY,
            PedHash.Musclbeac02AMY,
            PedHash.Polynesian01AMY,
            PedHash.Roadcyc01AMY,
            PedHash.Runner01AMY,
            PedHash.Runner02AMY,
            PedHash.Salton01AMY,
            PedHash.Skater01AMY,
            PedHash.Skater02AMY,
            PedHash.Soucent01AMY,
            PedHash.Soucent02AMY,
            PedHash.Soucent03AMY,
            PedHash.Soucent04AMY,
            PedHash.Stbla01AMY,
            PedHash.Stbla02AMY,
            PedHash.Stlat01AMY,
            PedHash.Stwhi01AMY,
            PedHash.Stwhi02AMY,
            PedHash.Sunbathe01AMY,
            PedHash.Surfer01AMY,
            PedHash.Vindouche01AMY,
            PedHash.Vinewood01AMY,
            PedHash.Vinewood02AMY,
            PedHash.Vinewood03AMY,
            PedHash.Vinewood04AMY,
            PedHash.Yoga01AMY,
            PedHash.SmartCasPat01AMY
        };

        public List<PedHash> SecurityGuards = new List<PedHash>();
        public List<PedHash> Prisoners = new List<PedHash>();
        public List<PedHash> HighNetFemale = new List<PedHash>();
        public List<PedHash> MediumNetFemale = new List<PedHash>();
        public List<PedHash> LowNetFemale = new List<PedHash>();
        public List<PedHash> ElderlyFemales = new List<PedHash>();
        public List<PedHash> HighNetMale = new List<PedHash>();
        public List<PedHash> MediumNetMale = new List<PedHash>();
        public List<PedHash> LowNetMale = new List<PedHash>();

        public AiCalloutsPeds()
        {
            //Check animals due to DSU overlays
            DomesticAnimals = new List<PedHash>()
            {
                PedHash.Cat,
                PedHash.Pug,
                PedHash.Poodle,
                PedHash.Retriever,
                PedHash.Rottweiler,
                PedHash.Westy,
                PedHash.Chop
            };
            FarmAnimals = new List<PedHash>()
            {
                PedHash.Pig,
                PedHash.Cow,
                PedHash.Deer,
                PedHash.Boar,
                PedHash.Coyote,
                PedHash.Hen
            };
            LowLevelCriminal = new List<PedHash>()
            {
                PedHash.Hillbilly01AMM,
                PedHash.Hillbilly02AMM,
                PedHash.ArmGoon01GMM,
                PedHash.ArmGoon02GMY,
                PedHash.MexLabor01AMM,
                PedHash.MethFemale01,
                PedHash.MethMale01,
                PedHash.Methhead01AMY,
                PedHash.MexThug01AMY,
                PedHash.Rurmeth01AFY,
                PedHash.Rurmeth01AMM,
                PedHash.Skidrow01AFM,
                PedHash.Skidrow01AMM,
                PedHash.Tramp01,
                PedHash.Tramp01AFM,
                PedHash.Tramp01AMM,
                PedHash.Tramp01AMO,
                PedHash.TrampBeac01AFM,
                PedHash.TrampBeac01AMM,
                PedHash.Salton01AMO,
                PedHash.Soucent02AMO,
                PedHash.Tattoo01AMO,
                PedHash.BurgerDrug,
                PedHash.RussianDrunk
            };
            SecurityGuards = new List<PedHash>()
            {
                PedHash.Armoured01,
                PedHash.Armoured01SMM,
                PedHash.Armoured02SMM
            };
            Prisoners = new List<PedHash>()
            {
                PedHash.Rashkovsky,
                PedHash.Prisoner01,
                PedHash.PrisMuscl01SMY,
                PedHash.Prisguard01SMM,
                PedHash.Prisoner01SMY
            };
            HighNetFemale = new List<PedHash>()
            {
                PedHash.Bevhills01AFM,
                PedHash.Bevhills01AFY,
                PedHash.Bevhills01AMY,
                PedHash.Bevhills02AFM,
                PedHash.Bevhills02AFY,
                PedHash.Bevhills02AMY,
                PedHash.Bevhills03AFY,
                PedHash.Bevhills04AFY,
                PedHash.Business01AFY,
                PedHash.Fitness01AFY
            };

            MediumNetFemale = new List<PedHash>()
            {
                PedHash.Eastsa01AFM,
                PedHash.Eastsa02AFM,
                PedHash.Tourist01AFM,
                PedHash.Soucent02AFO,
                PedHash.Business02AFY,
                PedHash.Business03AFY,
                PedHash.Epsilon01AFY,
                PedHash.Fitness01AFY,
                PedHash.Hipster02AFY,
                PedHash.Tourist02AFY,
                PedHash.Vinewood02AFY,
            };

            LowNetFemale = new List<PedHash>()
            {
                PedHash.Downtown01AFM,
                PedHash.Salton01AFM,
                PedHash.Skidrow01AFM,
                PedHash.TrampBeac01AFM,
                PedHash.Salton01AFO,
                PedHash.Rurmeth01AFY,
                PedHash.Maude,
                PedHash.Tonya
            };

            ElderlyFemales = new List<PedHash>()
            {
                PedHash.Genstreet01AFO,
                PedHash.Indian01AFO,
                PedHash.Soucent01AFO,
                PedHash.MrsPhillips,
                PedHash.MrsThornhill,
                PedHash.Patricia,
            };
            HighNetMale = new List<PedHash>()
            {
                PedHash.Golfer01AMM,
                PedHash.Malibu01AMM,
                PedHash.Beach02AMM,
                PedHash.Business01AMY,
                PedHash.Business02AFY,
                PedHash.SmartCasPat01AMY,
                PedHash.Stbla02AMY,
                PedHash.Barry,
                PedHash.Bankman,
                PedHash.Bankman01,
                PedHash.TaoCheng,
                PedHash.Josh,
                PedHash.Milton,
                PedHash.Paper,
                PedHash.TaosTranslator,
                PedHash.TomEpsilon
            };
            
            

            MediumNetMale = new List<PedHash>()
            {
                PedHash.Eastsa02AMM,
                PedHash.Bevhills01AMM,
                PedHash.Bevhills02AMM,
                PedHash.Ktown01AMM,
                PedHash.MexCntry01AMM,
                PedHash.Polynesian01AMM,
                PedHash.Salton02AMM,
                PedHash.Socenlat01AMM,
                PedHash.Beach02AMM,
                PedHash.Beachvesp01AMY,
                PedHash.Beachvesp02AMY,
                PedHash.Bevhills02AMY,
                PedHash.Busicas01AMY,
                PedHash.Eastsa02AMY,
                PedHash.Epsilon01AMY,
                PedHash.Epsilon02AMY,
                PedHash.Gay02AMY,
                PedHash.Genstreet01AMY,
                PedHash.Hipster03AMY,
                PedHash.Hipster01AFY,
                PedHash.Ktown01AMY
            };

            LowNetMale = new List<PedHash>()
            {
                PedHash.Fatlatin01AMM,
                PedHash.Genfat01AMM,
                PedHash.Genfat02AMM,
                PedHash.Hillbilly01AMM,
                PedHash.Hillbilly02AMM,
                PedHash.Salton04AMM,
                PedHash.RampHic,
                PedHash.PrologueDriver,
                PedHash.Hunter,
                PedHash.OldMan1a,
                PedHash.Omega,
                PedHash.OldMan2,
                PedHash.RampHic,
                PedHash.Trucker01SMM,
                PedHash.MilitaryBum
            };
        }
    }
}
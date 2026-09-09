using System.Collections.Generic;

namespace PoliceMP.Server.Controllers.AiCallouts
{
    public class AiCalloutsSpeech
    {
        //@todo This needs adding from web sources
        public static readonly List<string> GunScenes = new()
        {
            "GENERIC_CURSE_HIGH",
            "GENERIC_FUCK_YOU",
            "DYING_HELP",
            "COVER_ME",
            "GENERIC_FRIGHTENED_HIGH",
            "GENERIC_FRIGHTENED_MED",
            "GENERIC_INSULT_HIGH",
            "GENERIC_WAR_CRY",
            "RELOADING",
            "SHOOT",
            "TAKE_COVER",
            "STAY_DOWN",
            "KILLED_ALL",
            "SHOOT"
        };

        //@todo Not needed currently, But Forbs will abuse!
        public static readonly List<string> SexRelated = new()
        {
            "HOOKER_HAD_ENOUGH",
            "HOOKER_LEAVES_ANGRY",
            "SEX_CLIMAX",
            "SEX_FINISHED",
            "SEX_GENERIC",
            "SEX_GENERIC_FEM",
            "SEX_ORAL",
            "SEX_ORAL_FEM",
            "HOOKER_SECLUDED",
            "HOOKER_STORY_SYMPATHETIC_RESP",
            "HOOKER_STORY_SARCASTIC_RESP",
            "HOOKER_STORY_REVULSION_RESP"
        };

        //@todo This needs adding from web sources
        public static readonly List<string> ScaredScenes = new()
        {
            "GENERIC_CURSE_HIGH",
            "GENERIC_FUCK_YOU",
            "DYING_HELP",
            "COVER_ME",
            "GENERIC_FRIGHTENED_HIGH",
            "GENERIC_FRIGHTENED_MED",
            "GENERIC_INSULT_HIGH",
            "GENERIC_WAR_CRY",
            "RELOADING",
            "SHOOT",
            "TAKE_COVER",
            "STAY_DOWN"
        };

        public static readonly List<string> RollerCoaster = new()
        {
            "ROLLERCOASTER_CHAT_EXCITED",
            "ROLLERCOASTER_CHAT_NORMAL"
        };

        public static readonly List<string> ShopArmedRelatedLanguage = new()
        {
            "SHOP_BROWSE",
            "SHOP_BANTER",
            "SHOP_BROWSE_ARMOUR",
            "SHOP_GOODBYE",
            "SHOP_GREET",
            "SHOP_NO_COPS",
            "SHOP_NO_MESSING",
            "SHOP_NO_WEAPON",
            "SHOP_OUT_OF_STOCK",
            "SHOP_SELL_ARMOUR",
            "SHOP_SELL_BULLETPROOF_TYRES",
            "SHOP_SPECIAL_DISCOUNT",
            "SHOUT_THREATEN_PED",
            "SHOP_SHOOTING",
            "SHOP_GREET_UNUSUAL",
            "SHOP_SELL"
        };

        public static readonly List<string> ShopMeleRelatedLanguage = new()
        {
            "SHOP_BROWSE",
            "SHOP_BANTER",
            "SHOP_BROWSE_MELEE",
            "SHOP_GREET",
            "SHOP_GOODBYE",
            "SHOP_NO_COPS",
            "SHOP_NO_MESSING",
            "SHOP_NO_WEAPON",
            "SHOP_OUT_OF_STOCK",
            "SHOP_GREET_UNUSUAL",
            "SHOP_NO_WEAPON",
            "SHOP_SELL"
        };

        public static readonly List<string> ShopVehicleRelatedLanguage = new()
        {
            "SHOP_BROWSE",
            "SHOP_BANTER",
            "SHOP_GREET",
            "SHOP_GOODBYE",
            "SHOP_SELL_COSMETICS",
            "SHOP_SELL_ENGINE_UPGRADE",
            "SHOP_SELL_EXHAUST",
            "SHOP_SELL_HORN",
            "SHOP_SELL_REPAIR",
            "SHOP_SELL_SUSPENSION",
            "SHOP_SELL_TRANS_UPGRADE",
            "SHOP_SELL_TURBO",
            "SHOP_NICE_VEHICLE"
        };

        public static readonly List<string> ShopBarberRelatedLanguage = new()
        {
            "SHOP_BROWSE",
            "SHOP_BANTER",
            "SHOP_GREET",
            "SHOP_GOODBYE",
            "SHOP_CUTTING_HAIR",
            "SHOP_HAIR_WHAT_WANT"
        };
    }
}
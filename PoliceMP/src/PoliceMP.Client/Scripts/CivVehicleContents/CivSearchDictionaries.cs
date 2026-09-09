using System.Collections.Generic;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.CivVehicleContents
{
    public class CivSearchDictionaries : Script
    {
        public static Dictionary<string, int> vehicleSearchPossibleItems = new Dictionary<string, int>
        {
            {"Nothing of Interest", 1},
            {"Smart Phone", 2},
            {"Burner Phone", 4},
            {"Wallet", 8},
            {"Car Keys", 16},
            {"House Keys", 32},
            {"Purse", 64},
            {"Handbag", 128},
            {"Laptop", 256},
            {"Electronic Devices", 512},
            {"Sunglasses", 1024},
            {"Torch", 2048},
            {"Camera", 4096},
            {"Stolen IDs", 8192},
            {"Fake IDs", 16384},
            {"Stolen Mail", 32768},
            {"Stolen Checks", 65536},
            {"Duct Tape", 131072},
            {"Plastic Sheet", 262144},
            {"Lock Pick", 524288},
            {"Counterfeit Money", 1048576},
            {"Bloody Hammer", 2097152},
            {"Documentation", 4194304},
            {"Parking Ticket", 8388608},
            {"Food Wrappers", 16777216},
            {"Waste Bags", 33554432},
            {"Shovel", 67108864}
        };

        public static Dictionary<string, int> vehicleSearchPossibleItems2 = new Dictionary<string, int>
        {
            {"Knife", 1},
            {"Switchblade", 2},
            {"Machete", 4},
            {"Handgun", 8},
            {"Semi-Automatic Rifle", 16},
            {"Fully-Automatic Rifle", 32},
            {"Magazines", 64},
            {"Loose Ammo", 128},
            {"Weapon Parts", 256},
            {"Weapon Casing", 512},
            {"Cocaine Parcel", 1024},
            {"Cannabis Grinder", 2048},
            {"Spliff", 4096},
            {"Briefcase of Drugs", 8192},
            {"Small Bags of Drugs", 16384},
            {"Packet of Cigarettes", 32768},
            {"Packet of Tobacco", 65536},
            {"Vape", 131072},
            {"Chewing Gum", 262144},
            {"Chocolate Bar", 524288},
            {"Crisps", 1048576},
            {"Kinder Eggs", 2097152},
            {"Cash £200", 4194304},
            {"Cash £500", 8388608},
            {"Cash £2000", 16777216},
            {"Cash £10000", 33554432},
        };

        public static Dictionary<string, int> vehicleSearchPossibleItems3 = new Dictionary<string, int>
        {
            {"Gift Card", 1},
            {"Key Maker", 2},
            {"Vehicle Hacking Device", 4},
            {"Fake Pistol", 8},
            {"Credit Card Reader", 16},
            {"Balaclava", 32},
            {"Ski Mask", 64},
            {"Power Drill", 128},
            {"Opened Bottle", 256},
            {"Empty Cans", 512},
            {"Stolen Jewellery", 1024},
            {"Gloves", 2048},
            {"Tracking Device", 4096},
            {"Blueprints", 8192},
            {"Black Market Catalogue", 16384},
            {"Number Plates", 32768},
            {"Radio Scanner", 65536},
        };

        public static Dictionary<string, int> vehicleSearchPossibleItemsSenior = new Dictionary<string, int> 
        {
            {"C55", 1},
            {"CHEMTEX", 2},
            {"Trinitrotoluene (TNT)", 4},
            {"Nitrocellulose", 8},
            {"Gunpowder", 16},
            {"Oxidisers", 32},
            {"RDX", 64},
            {"Nitroglycerine", 128},
            {"Hexamethylene Triperoxide Diamine (HMTD)", 256},
            {"Triacetone Triperoxide (TATP)", 512},
            {"Nitrate Fertilizers", 1024},
            {"Household Chemicals", 2048}
        };
    }
}
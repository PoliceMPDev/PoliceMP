using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Shared.Models;

namespace PoliceMP.Main.Client.Actions
{
    public class Resist
    {
        public static bool Check(Person person, int chanceModifier = 0)
        {
            if (person.Ped().IsCuffed) return false;

            if (person.Ped().GetBoolDecor("HasResisted")) return true;

            var chance = GetPersonChance(person);

            var resists = PoliceMpRandom.Next(100) <= chance + chanceModifier;

            if (resists)
            {
                person.Ped().SetBoolDecor("HasResisted", true);

                OnResist(chance, person);
            }

            return resists;
        }

        public static bool Check(Car car, int chanceModifier = 0)
        {
            if (car.Vehicle().IsWanted) return true;

            var chance = GetCarChance(car);

            var resists = PoliceMpRandom.Next(100) <= chance + chanceModifier;

            if (resists)
            {
                car.Vehicle().IsWanted = true;

                OnResist(chance, car);
            }

            return resists;
        }

        private static void OnResist(int chance, Person person)
        {
            Screen.ShowSubtitle($"~y~{person.FirstName}: ~r~You'll never take me alive!");

            person.Ped().Task.FleeFrom(Game.PlayerPed);
        }

        private static void OnResist(int chance, Car car)
        {
            CarSelector.UnselectCar();

            Screen.ShowSubtitle($"~r~The driver of the {car.Vehicle().LocalizedName} has driven off!");

            var vehicle = car.Vehicle();

            car.Vehicle().Driver.Task.FleeFrom(Game.PlayerPed);

            BaseScript.TriggerServerEvent("PursuitManager:RegisterPursuit",
                vehicle.Driver.NetworkId,
                vehicle.LocalizedName,
                vehicle.Mods.PrimaryColor.ToString(),
                ClientFunctions.GetStreetName(car.Vehicle().Position),
                vehicle.Position.X,
                vehicle.Position.Y,
                vehicle.Position.Z);
        }

        private static int GetPersonChance(Person person)
        {
            var chance = person.Attitude / 4;

            if (person.OnAnyDrugs)
                chance += 10;

            if (person.HasIllegalItems)
                chance += 10;

            if (person.HasMarkers)
                chance += (10 * person.Markers.Count);

            if (person.HasWarrants)
                chance += 10;

            if (person.HasWeapon)
                chance += 10;

            return chance;
        }

        private static int GetCarChance(Car car)
        {
            var chance = 2;

            if (car.HasMarkers)
                chance += (10 * car.Markers.Count);

            if (!car.HasInsurance)
                chance += 10;

            if (!car.HasMot)
                chance += 10;

            if (!car.HasTax)
                chance += 10;

            if (car.HasIllegalItems)
                chance += 10;

            if (car.HasMarker("Fail to Stop"))
                chance += 75;

            return chance;
        }
    }
}
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Shared.Constants;
using PoliceMP.Main.Core.Shared.Extensions;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Main.Client.Actions.People
{
    /// <summary>
    ///     Used to show things like "ped has red eyes" when interacting with them.
    /// </summary>
    public class Observation : BaseScript
    {
        public static readonly string[] CannabisObservations =
        {
            "- Eyes look <span class='text-danger'>red</span><br>",
            "- Smells of <span class='text-danger'>cannabis</span>.<br>",
            "- Co-ordination appears to be <span class='text-danger'>impaired</span><br>",
            "- Appears to be <span class='text-danger'>on edge</span><br>",
            "- Seems to be <span class='text-warning'>laughing at the slightest thing</span><br>",
            "- Talking as if they have an <span class='text-warning'>overly dry mouth</span><br>"
        };

        public static readonly string[] CocaineObservations =
        {
            "- Pupils look <span class='text-danger'>dilated<br>",
            "- Showing <span class='text-danger'>alert</span> and <span class='text-danger'>jumpy behaviour<br>",
            "- Evidence of <span class='text-danger'>white powder</span> around the nose<br>"
        };

        public static readonly string[] HeroinObservations =
        {
            "- Visible indicators of <span class='text-danger'>needle use</span><br>",
            "- Appears to be <span class='text-danger'>drowsy</span><br>"
        };

        public static readonly string[] EcstasyObservations =
        {
            "- Jaw is <span class='text-danger'>swinging</span><br>",
            "- Appears to be <span class='text-warning'>sweating excessively</span><br>",
            "- Appears to be <span class='text-warning'>suspiciously happy</span><br>",
            "- Pupils look <span class='text-danger'>dilated</span><br>",
            "- Appears to be <span class='text-warning'>full of energy</span><br>"
        };

        public static readonly string[] AlcoholObservations =
        {
            "- Appears to have <span class='text-warning'>slurred speech</span><br>",
            "- Smells of <span class='text-warning'>alcohol</span><br>",
            "- Can't seem to <span class='text-warning'>stay still</span><br>",
            "- They are <span class='text-danger'>speaking aggressively</span><br>"
        };

        /// <summary>
        ///     Used to make sure we don't use the same observation string twice, even if
        ///     it exists in multiple of the observation arrays.
        /// </summary>
        private static readonly List<string> UsedObservations = new List<string>();

        [EventHandler(ClientEvents.ON_INTERACT_WITH_DRIVER)]
        private async void OnInteractWithDriver()
        {
            var car = CarSelector.SelectedCar;
            if (car == null || car.Vehicle().Driver == null)
            {
                ClientFunctions.SendErrorMessage("There was an error retrieving the car or driver. Please try again. Report this if it continues.");
                return;
            }
            var person = await PersonRetriever.GetPerson(car.Vehicle().Driver.Handle);
            if (person == null)
            {
                ClientFunctions.SendErrorMessage("There was an error retrieving the person. Please try again. Report this if it continues.");
                return;
            }

            var builder = new StringBuilder();
            builder.Append(
                $"You notice something about the <span class='text-warning'>{car.Vehicle().LocalizedName}</span>...<br><br>");
            builder.Append(person.WearsSeatbelt ? "- <span class='text-success'>Wearing seatbelt.<br>" : "- <span class='text-danger'>Not wearing seatbelt.<br>");

            var onCannabis = ObserveCannabis(person, ref builder);
            var onAlcohol = ObserveAlcohol(person, ref builder);
            var onCocaine = ObserveCocaine(person, ref builder);
            var onEcstasy = ObserveEcstasy(person, ref builder);
            var onHeroin = ObserveHeroin(person, ref builder);
            var carFault = ObserveCar(car, ref builder);

            if (person.WearsSeatbelt && !onCannabis && !onAlcohol && !onCocaine && !onEcstasy && !onHeroin && !carFault)
            {
                ShowNotification($"No observations were made on the <span class='text-warning'>{car.Vehicle().LocalizedName}.</span>");
                return;
            }

            var message = builder.ToString();

            ShowNotification(message);

            UsedObservations.Clear();
        }

        [EventHandler(ClientEvents.ON_PERSON_SELECTED)]
        private void OnPersonSelected()
        {
            var person = PersonSelector.SelectedPerson;
            var builder = new StringBuilder();

            builder.Append("You notice something about the person...<br><br>");

            var onCannabis = ObserveCannabis(person, ref builder);
            var onAlcohol = ObserveAlcohol(person, ref builder);
            var onCocaine = ObserveCocaine(person, ref builder);
            var onEcstasy = ObserveEcstasy(person, ref builder);
            var onHeroin = ObserveHeroin(person, ref builder);
            var hasInjuries = ObserveInjuries(person, ref builder);

            if (!onCannabis && !onAlcohol && !onCocaine && !onEcstasy && !onHeroin && !hasInjuries)
            {
                ShowNotification("No observations were made.");
                return;
            }

            var message = builder.ToString();
            
            ShowNotification(message);

            UsedObservations.Clear();
        }

        public static bool ObserveCar(Car car, ref StringBuilder builder)
        {
            var hasFault = false;

            if (car.Vehicle().IsDamaged)
            {
                builder.Append("- Vehicle is damaged.<br>");
                hasFault = true;
            }

            if (car.Vehicle().IsFrontBumperBrokenOff)
            {
                builder.Append("- Front bumper has broken off.<br>");
                hasFault = true;
            }

            if (car.Vehicle().IsRearBumperBrokenOff)
            {
                builder.Append("- Rear bumper has broken off.<br>");
                hasFault = true;
            }

            if (car.Vehicle().IsLeftHeadLightBroken)
            {
                builder.Append("- Left head light broken.<br>");
                hasFault = true;
            }

            if (car.Vehicle().IsRightHeadLightBroken)
            {
                builder.Append("- Right head light broken.<br>");
                hasFault = true;
            }

            if (car.Vehicle().PassengerCount > 0)
            {
                builder.Append($"- There are <span class='text-warning'>{car.Vehicle().PassengerCount}</span> passengers.<br>");
                hasFault = true;
            }

            if (!car.Vehicle().Windows.AreAllWindowsIntact)
            {
                foreach (var window in car.Vehicle().Windows.GetAllWindows())
                    switch (window.Index)
                    {
                        case VehicleWindowIndex.BackLeftWindow:
                            builder.Append("- Back left window smashed.<br>");
                            break;

                        case VehicleWindowIndex.BackRightWindow:
                            builder.Append("- Back right window smashed.<br>");
                            break;

                        case VehicleWindowIndex.FrontLeftWindow:
                            builder.Append("- Front left window smashed.<br>");
                            break;

                        case VehicleWindowIndex.FrontRightWindow:
                            builder.Append("- Front right window smashed.<br>");
                            break;

                        default:
                            builder.Append("- One or more windows are smashed.<br>");
                            break;
                    }

                hasFault = true;
            }

            if (car.Vehicle().EngineHealth < 800)
            {
                builder.Append("- Engine appears to be damaged.<br>");
                hasFault = true;
            }

            if (car.Vehicle().PetrolTankHealth < 800)
            {
                builder.Append("- Petrol tank appears to be damaged.<br>");
                hasFault = true;
            }

            if (car.Vehicle().BodyHealth < 800)
            {
                builder.Append("- Bodywork appears to be damaged.<br>");
                hasFault = true;
            }

            var burst = false;
            for (var i = 1; i < 5; i++)
                if (API.IsVehicleTyreBurst(car.Vehicle().Handle, i, false))
                    burst = true;

            if (burst)
            {
                builder.Append("- One or more tyres are burst.<br>");
                hasFault = true;
            }

            return hasFault;
        }

        private static bool ObserveCannabis(Person person, ref StringBuilder builder)
        {
            var quantity = PoliceMpRandom.Next(1, 3);

            if (person.SmokedCannabis)
            {
                for (var i = 0; i < quantity; i++)
                {
                    var randomString = GetRandomObservation(CannabisObservations);
                    builder.Append(randomString);
                }

                return true;
            }

            return false;
        }

        private static bool ObserveAlcohol(Person person, ref StringBuilder builder)
        {
            var quantity = PoliceMpRandom.Next(1, 3);

            if (person.AlcoholLevel > Breathalyse.BLOOD_ALCOHOL_LIMIT)
            {
                for (var i = 0; i < quantity; i++)
                {
                    var randomString = GetRandomObservation(AlcoholObservations);
                    builder.Append(randomString);
                }

                return true;
            }

            return false;
        }

        private static bool ObserveCocaine(Person person, ref StringBuilder builder)
        {
            var quantity = PoliceMpRandom.Next(1, 3);

            if (person.OnCocaine)
            {
                for (var i = 0; i < quantity; i++)
                {
                    var randomString = GetRandomObservation(CocaineObservations);
                    builder.Append(randomString);
                }

                return true;
            }

            return false;
        }

        private static bool ObserveEcstasy(Person person, ref StringBuilder builder)
        {
            var quantity = PoliceMpRandom.Next(1, 3);

            if (person.OnEcstasy)
            {
                for (var i = 0; i < quantity; i++)
                {
                    var randomString = GetRandomObservation(EcstasyObservations);
                    builder.Append(randomString);
                }

                return true;
            }

            return false;
        }

        private static bool ObserveHeroin(Person person, ref StringBuilder builder)
        {
            var quantity = PoliceMpRandom.Next(1, 3);

            if (person.OnEcstasy)
            {
                for (var i = 0; i < quantity; i++)
                {
                    var randomString = GetRandomObservation(HeroinObservations);
                    builder.Append(randomString);
                }

                return true;
            }

            return false;
        }

        private static bool ObserveInjuries(Person person, ref StringBuilder builder)
        {
            var isInjured = false;

            if (API.HasEntityBeenDamagedByAnyVehicle(person.EntityId()))
            {
                builder.Append("Looks like they have been <span class='text-warning'>hit by a vehicle.</span><br>");
                isInjured = true;
            }

            if (person.Ped().HasBeenDamagedByAnyWeapon())
            {
                builder.Append("Looks like they have been <span class='text-warning'>hurt with a weapon.</span><br>");
                isInjured = true;
            }

            if (person.Ped().HasBeenDamagedBy(Game.PlayerPed))
            {
                builder.Append("They were hit by <span class='text-warning'>you.</span><br>");
                isInjured = true;
            }

            return isInjured;
        }

        private static string GetRandomObservation(string[] observations)
        {
            string randomString;
            var count = 0;
            do
            {
                var index = PoliceMpRandom.Next(observations.Length - 1);
                randomString = observations[index];
                if (count >= 5)
                {
                    randomString = string.Empty;
                    break;
                }
                count++;
            } while (UsedObservations.Contains(randomString));

            UsedObservations.Add(randomString);

            return randomString;
        }

        private static void ShowNotification(string text)
        {
            ClientFunctions.ShowToast( "Observations", text, "info");
        }
    }
}
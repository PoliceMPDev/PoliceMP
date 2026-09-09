using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Constants;
using System.Collections.Generic;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.CivVehicleContents;

namespace PoliceMP.Client.Actions.SearchPed
{
    public class SearchPedHandler : ActionHandler<SearchPed>
    {
        private readonly INotificationService _notifications;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IAnimationService _anims;
        private readonly ISoundService _sounds;
        private readonly IPedInfoService _pedInfo;
        private readonly ISpeechService _speech;

        public SearchPedHandler(INotificationService notifications,
            IAnimationService anims,
            ISoundService sounds,
            IPedInfoService pedInfo,
            ISpeechService speech,
            INewNotificationOverlay newNotificationOverlay)
        {
            _notifications = notifications;
            _anims = anims;
            _sounds = sounds;
            _pedInfo = pedInfo;
            _newNotificationOverlay = newNotificationOverlay;
            _speech = speech;
        }

        protected override async Task<bool> Handle(SearchPed action)
        {
            if (!action.Subject.IsNearEntity(action.Target, new Vector3(2f, 2f, 2f)))
            {
                _notifications.Error("Person Search", "You are not close enough to the ped.");
                return false;
            }

            if (action.Target.IsInVehicle())
            {
                _notifications.Error("Person Search", "You cannot search someone who is in a vehicle.");
                return false;
            }

            await Search(action);
            return true;
        }

        private async Task Search(SearchPed action)
        {
            action.Subject.Task.TurnTo(action.Target);

            _speech.Say(Game.PlayerPed, "I'm going to search you...");
            if (action.Target.IsPlayer) API.DecorSetBool(action.Target.Handle, "PoliceMP_Ped_BeingSearched", true);

            await Delay(500);
            
            await PlaySearchAnims(action);
            await DisplayResults(action);
        }

        private async Task DisplayResults(SearchPed action)
        {
            if (API.DecorExistOn(action.Target.Handle, "PoliceMP_Ped_Inventory1") || API.DecorExistOn(action.Target.Handle, "PoliceMP_Ped_Inventory2") || API.DecorExistOn(action.Target.Handle, "PoliceMP_Ped_Inventory3") || API.DecorExistOn(action.Target.Handle, "PoliceMP_Ped_Inventory4"))
            {

                var encodedValueSet1 = API.DecorGetInt(action.Target.Handle, "PoliceMP_Ped_Inventory1");
                var encodedValueSet2 = API.DecorGetInt(action.Target.Handle, "PoliceMP_Ped_Inventory2");
                var encodedValueSet3 = API.DecorGetInt(action.Target.Handle, "PoliceMP_Ped_Inventory3");
                var encodedValueSet4 = API.DecorGetInt(action.Target.Handle, "PoliceMP_Ped_Inventory4");

                var skip = false;
                if (encodedValueSet1 == 0 && encodedValueSet2 == 0 && encodedValueSet3 == 0) skip = true;

                if (!skip)
                {
                    List<string> decodedItems = new List<string>();

                    foreach (var vehicleItem in CivSearchDictionaries.vehicleSearchPossibleItems)
                    {
                        if ((encodedValueSet1 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in CivSearchDictionaries.vehicleSearchPossibleItems2)
                    {
                        if ((encodedValueSet2 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in CivSearchDictionaries.vehicleSearchPossibleItems3)
                    {
                        if ((encodedValueSet4 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in CivSearchDictionaries.vehicleSearchPossibleItemsSenior)
                    {
                        if ((encodedValueSet3 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    var decodedString = String.Join(", ", decodedItems);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Person Search", "success", $"Items Found: {decodedString}!", new NewNotificationMessageContent[0]));
                    return;
                }
            }

            var pedInfo = await _pedInfo.GetByNetworkId(action.Target.NetworkId);
            if (pedInfo == null || !pedInfo.Items.Any())
            {
                _notifications.Info("Person Search", "Found nothing of interest.");
                return;
            }

            var builder = new StringBuilder();

            builder.Append(pedInfo.HasIllegalItems
                ? "You found some <span class='text-danger'>illegal</span> item(s)!<br><br>"
                : "You didn't find any illegal items.<br><br>");

            foreach (var item in pedInfo.Items)
            {
                builder.Append(item.IsIllegal 
                        ? $"- <span class='text-danger'>{item.Name}</span><br>" 
                        : $"- {item.Name}<br>");
            }

            _notifications.Info("Person Search", builder.ToString());
        }

        private async Task PlaySearchAnims(SearchPed action)
        {
            var timeOut = DateTime.Now;

            var sequence = new TaskSequence();
            sequence.AddTask.AchieveHeading(action.Subject.Heading);
            await sequence.AddRequired(action.Target);
            sequence.AddTask.StandStill(-1);
            sequence.Close();
            action.Target.AlwaysKeepTask = true;
            action.Target.Task.PerformSequence(sequence);

            while (Math.Abs(action.Target.Heading - action.Subject.Heading) > 3f)
            {
                if ((DateTime.Now - timeOut).TotalSeconds >= 3)
                {
                    action.Target.Heading = action.Subject.Heading;
                    break;
                }
                await Delay(10);
            }

            if (!action.Target.IsCuffed)
                await _anims.HandsUp(action.Target);

            action.Target.IsPositionFrozen = true;

            var offsetPositionFront = API.GetOffsetFromEntityInWorldCoords(action.Target.Handle, 0f, -0.55f, 0f);
            timeOut = DateTime.Now;
            while (!API.IsEntityAtCoord(action.Subject.Handle, offsetPositionFront.X, offsetPositionFront.Y,
                offsetPositionFront.Z, 0.33f, 0.33f, 0.33f, false, true, 0))
            {
                // Just teleport them to the position if it's been 5 seconds and they're still not there
                if ((DateTime.Now - timeOut).TotalSeconds >= 5)
                {
                    action.Target.Position = offsetPositionFront;
                    break;
                }

                API.TaskGoStraightToCoord(action.Subject.Handle, offsetPositionFront.X, offsetPositionFront.Y,
                    offsetPositionFront.Z, 1f, 1000, action.Subject.Handle, 0.5f);
                await Delay(500);
            }

            _sounds.Play(Sounds.PatDown);
            _speech.Do(action.Target, $"Gets searched by {Game.Player.Name}.", 8000);
            action.Subject.Heading = action.Target.Heading;
            action.Subject.IsPositionFrozen = true;
            await _anims.Play(action.Subject, "anim@heists@load_box", "idle", 1.5f, flag: 0);
            await Delay(850);
            await _anims.Play(action.Subject, "anim@heists@box_carry@", "idle", 1.5f, flag: 0);
            await Delay(600);
            await _anims.Play(action.Subject, "missfam5_yoga", "start_pose", 1.5f, flag: 0);
            await Delay(750);

            var position = API.GetOffsetFromEntityInWorldCoords(action.Subject.Handle, 0f, -0.33f, 0f);
            API.TaskGoStraightToCoord(action.Subject.Handle, position.X, position.Y, position.Z, 1f, 1000, action.Subject.Heading, 1f);
            await Script.Delay(1000);
            action.Subject.Heading -= 20f;

            await _anims.Play(action.Subject, "missbigscore2aig_7@driver", "boot_r_loop", 1.3f, flag: 1);
            await Delay(1000);
            await _anims.Play(action.Subject, "mini@yoga", "outro_2", 1.5f, flag: 0);
            await Delay(1500);

            API.ClearPedTasks(action.Subject.Handle);
            action.Subject.Heading += 40f;

            await _anims.Play(action.Subject, "missbigscore2aig_7@driver", "boot_l_loop", 1.3f, flag: 1);
            await Delay(1000);
            await _anims.Play(action.Subject, "mini@yoga", "outro_2", 1.5f, flag: 0);
            await Delay(1500);
            API.ClearPedTasks(action.Subject.Handle);

            action.Subject.Heading -= 20f;
            await Delay(50);

            action.Target.IsCollisionEnabled = true;
            action.Target.IsPositionFrozen = false;
            action.Subject.IsPositionFrozen = false;

            action.Target.Task.ClearAll();
            _anims.Clear(action.Target);
            await Delay(10);

            await action.Target.StandStillFacingPlayer();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.PlayerControllerScript;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared.Commands;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.NetworkMessages.Callouts.Commands;
using PoliceMP.Shared.NetworkMessages.Callouts.Notifications;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;

namespace PoliceMP.Client.Scripts.AiCallouts
{
    public class AiCallouts : Script
    {
        private readonly IClientCommunicationsManager _comms;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private int? _currentCalloutId = null;
        private float _scale = 0.1F * API.GetGameplayCamFov();
        private ITickManager _tickManager;
        private readonly ISpeechService _speech;
        private readonly ILegacyClientCommunicationsManager _legacyComms;
        private readonly ICommandManager _commandManager;
        private readonly IPlayerController _playerController;
        private int _blip;

        private string _currentCalloutTitle = "";
        private string _currentCalloutSubtitle = "";
        private string _currentCalloutBody = "";
        private List<string> _currentCalloutAttendingUnits;

        public AiCallouts(IClientCommunicationsManager comms, INewNotificationOverlay newNotificationOverlay,
            ITickManager tickManager, ISpeechService speech, ILegacyClientCommunicationsManager legacyComms,
            ICommandManager commandManager, IPlayerController playerController)
        {
            _comms = comms;
            _newNotificationOverlay = newNotificationOverlay;
            _tickManager = tickManager;
            _speech = speech;
            _legacyComms = legacyComms;
            _commandManager = commandManager;
            _playerController = playerController;


            legacyComms.On<List<float>>(ClientEvents.StartFire, (pos =>
            {
                API.StartScriptFire(pos[0], pos[1], pos[2], 25, true);

                API.ExecuteCommand("startfire normal 6 3 false");

                API.AddExplosion(pos[0], pos[1], pos[2], 23, 5, true, false, 1);
            }));

            comms.AddNotificationHandler<CalloutCreatedEvent>(CreateNewCalloutCommand);
            comms.AddNotificationHandler<PlayerAttachedToCalloutEvent>(OnAttachedToCallout);
            comms.AddNotificationHandler<PlayerDetachedFromCalloutNotification>(OnDetachedFromCallout);
            comms.AddNotificationHandler<CalloutResolvedEvent>(OnCalloutResolved);
            comms.AddNotificationHandler<ExperiencePointsNofitication>(NewExperiencePointsNotification);
            comms.AddNotificationHandler<PlaySpeechEvent>(AiSpeech);
            comms.AddNotificationHandler<CalloutErrorNotificationEvent>(CalloutError);
            comms.AddNotificationHandler<UserClockedOnNotificationEvent>(AttendeeClockedOn);
            comms.AddNotificationHandler<UserClockedOffNotificationEvent>(AttendeeClockedOff);
            comms.AddNotificationHandler<PlayAmbientSoundEvent>(PlayAmbientSound);
            comms.AddNotificationHandler<CalloutInfoEvent>(OnCalloutInfo);
            // comms.AddRequestHandler<BreakVehicleCommand>(BreakVehicle);

            API.RegisterNuiCallback("GetActiveCallout", new Action<ExpandoObject, CallbackDelegate>(async (_, cb) =>
            {
                if (_currentCalloutId == null) cb("{ \"noCallout\": true }");
                else
                {
                    var json = "";
                    var calloutId = _currentCalloutId.ToString();
                    json += "{";
                    json += $"\"calloutId\": \"{calloutId}\",";
                    json += $"\"title\": \"{_currentCalloutTitle}\",";
                    json += $"\"subtitle\": \"{_currentCalloutSubtitle}\",";
                    json += $"\"body\": \"{_currentCalloutBody}\",";
                    json += "\"attendingUnits\": [";
                    json = _currentCalloutAttendingUnits.Aggregate(json, (current, unit) => current + $"\"{unit}\",");
                    json = json.TrimEnd(',');
                    json += "]";
                    json += "}";
                    cb(json);
                }
            }));
            
            API.RegisterKeyMapping("acceptcallout", "Accept Call", "Q", "keyboard");
        }

        private Task OnCalloutInfo(CalloutInfoEvent @event)
        {
            if (null == @event.CalloutId)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                    "Not attached to incident",
                    "error",
                    "You are not currently attached to an incident.",
                    new NewNotificationMessageContent[0]
                ));
                return Task.FromResult(0);
            }

            // Build notification of callout details
            string message = @event.Title;
            message += "<br><br>";
            if (@event.Subtitle is { Length: > 0 })
            {
                message += @event.Subtitle;
                message += "<br><br>";
            }

            message += @event.Body;

            // Send notification
            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                $"Incident Details:",
                "info",
                message,
                new NewNotificationMessageContent[0],
                30
            ));

            // Build and send notification for attached units
            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                $"Attached Units:",
                "info",
                String.Join("<br>", @event.AttachedUnits),
                new NewNotificationMessageContent[0],
                30
            ));

            return Task.FromResult(0);
        }

        // private void BreakVehicle(BreakVehicleCommand command)
        // {
        //     var vehicle = Entity.FromNetworkId(command.VehicleNetworkId) as Vehicle;
        //     if (vehicle == null)
        //     {
        //         _log.Debug($"Could not find vehicle with network id: {command.VehicleNetworkId}");
        //         return;
        //     }
        //     foreach (var wheel in command.BreakWheels)
        //     {
        //         API.BreakOffVehicleWheel(vehicle.Handle, wheel, false, false, false, false);
        //     }
        // }

        private Task PlayAmbientSound(PlayAmbientSoundEvent @event)
        {
            Ped ped = Entity.FromNetworkId(@event.PedNetworkId) as Ped;
            if (null == ped) return Task.FromResult(0);
            API.PlayPedAmbientSpeechNative(ped.Handle, @event.SpeechName, "SPEECH_PARAMS_STANDARD");

            return Task.FromResult(0);
        }

        private async Task CreateNewCalloutCommand(CalloutCreatedEvent createdEvent)
        {
            // Generate random Ped for the headshot
            IEnumerable<PedHash> pedHashes = Enum.GetValues(typeof(PedHash)).Cast<PedHash>();
            PedHash pedHash = pedHashes.GetRandom();
            Ped ped;
            do
            {
                ped = await World.CreatePed(pedHash, new Vector3((float)-2.94, (float)-0.61, (float)0.03));
            } while (!API.DoesEntityExist(ped.Handle));

            // Get headshot image into NUI
            string texture = await GetHeadshot(ped);
            StringBuilder sb = new StringBuilder();
            sb.Append("https://nui-img/");
            sb.Append(texture);
            sb.Append("/");
            sb.Append(texture);
            sb.Append("?v=");
            sb.Append(DateTime.UtcNow.ToFileTime());

            // Build notification of callout details
            string message = createdEvent.Title;
            message += "<br><br>";
            if (createdEvent.Subtitle is { Length: > 0 })
            {
                message += createdEvent.Subtitle;
                message += "<br><br>";
            }

            message += createdEvent.Body;

            // Send notification with ped headshot attached
            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                $"New Incident:",
                "error",
                message,
                new NewNotificationMessageContent[0],
                20,
                sb.ToString()
            ));

            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                $"/acceptcallout {createdEvent.CalloutId}", "info",
                "To accept the incident", new NewNotificationMessageContent[0], 20));
            
            // Remove the headshot ped
            ped.MarkAsNoLongerNeeded();
            ped.Delete();
        }

        private async Task OnAttachedToCallout(PlayerAttachedToCalloutEvent attachedCalloutEvent)
        {
            if (attachedCalloutEvent.PlayerServerHandle == Player.Local.ServerId)
            {
                // I have attached to the callout
                _currentCalloutId = attachedCalloutEvent.CalloutId;
                _currentCalloutTitle = attachedCalloutEvent.CalloutTitle;
                _currentCalloutSubtitle = attachedCalloutEvent.CalloutSubtitle;
                _currentCalloutBody = attachedCalloutEvent.CalloutBody;
                _currentCalloutAttendingUnits = attachedCalloutEvent.CalloutAttendingUnits;
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Attached to Incident", "success",
                    $"You have now been attached to the Incident.", new NewNotificationMessageContent[0]));
                API.ClearGpsPlayerWaypoint();
                API.SetWaypointOff();

                if (null != attachedCalloutEvent.CalloutBlipRadius)
                {
                    // Radius-based blip
                    var location = attachedCalloutEvent.CalloutLocation;
                    _blip = API.AddBlipForRadius(location.X, location.Y, 5f,
                        attachedCalloutEvent.CalloutBlipRadius.GetValueOrDefault(0));
                    API.SetBlipColour(_blip, 1);
                    API.SetBlipAlpha(_blip, 85);
                }
                else
                {
                    // Single location blip with icon
                    API.SetNewWaypoint(attachedCalloutEvent.CalloutLocation.X, attachedCalloutEvent.CalloutLocation.Y);
                    _blip = API.AddBlipForCoord(attachedCalloutEvent.CalloutLocation.X,
                        attachedCalloutEvent.CalloutLocation.Y, 5f);
                    API.SetBlipSprite(_blip, attachedCalloutEvent.CalloutIcon.GetValueOrDefault(0));
                }
            }
            else
            {
                // Someone else attached to the callout
                // Is it the callout I am attached to? Otherwise I'm not interested
                if (attachedCalloutEvent.CalloutId == _currentCalloutId)
                {
                    _currentCalloutAttendingUnits = attachedCalloutEvent.CalloutAttendingUnits;
                    // Yes its the same callout I am on, let me know who has attached
                    _newNotificationOverlay.SendNotification(
                        new NewNotificationMessage(
                            "New Unit Attached",
                            "info",
                            (null != attachedCalloutEvent.PlayerCallsign
                                ? "[" + attachedCalloutEvent.PlayerCallsign + "] "
                                : "") +
                            attachedCalloutEvent.PlayerName +
                            " has attached to the callout."
                        )
                    );
                }
            }
        }

        private Task OnDetachedFromCallout(PlayerDetachedFromCalloutNotification detachedCalloutEvent)
        {
            // Is it me that detached?
            if (detachedCalloutEvent.PlayerServerHandle == Player.Local.ServerId)
            {
                // I detached from the callout
                _currentCalloutId = null;
                _currentCalloutTitle = null;
                _currentCalloutSubtitle = null;
                _currentCalloutBody = null;
                _currentCalloutAttendingUnits = new();
                API.DeleteWaypoint();
                API.RemoveBlip(ref _blip);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                    "Detached from Incident",
                    "success",
                    "You have now been detached from the incident."
                ));
            }
            else
            {
                // Someone else detached from their callout
                // Is it the callout I am on?
                if (detachedCalloutEvent.CalloutId == _currentCalloutId)
                {
                    _currentCalloutAttendingUnits = detachedCalloutEvent.CalloutAttendingUnits;
                    // Yes its the same callout I am on, let me know who has detached
                    _newNotificationOverlay.SendNotification(
                        new NewNotificationMessage(
                            "Unit Detached",
                            "info",
                            (null != detachedCalloutEvent.PlayerCallsign
                                ? "[" + detachedCalloutEvent.PlayerCallsign + "] "
                                : "") +
                            detachedCalloutEvent.PlayerName +
                            " has detached from the callout."
                        )
                    );
                }
            }

            return Task.FromResult(0);
        }

        private Task NewExperiencePointsNotification(ExperiencePointsNofitication experiencePointsNotification)
        {
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("New Callout", "error",
                $"{experiencePointsNotification.ExperiencePoints:D2}:\n{experiencePointsNotification.Subtitle:D2}:\n{experiencePointsNotification.Body:D2}",
                new NewNotificationMessageContent[0]));

            return Task.FromResult(0);
        }

        private Task OnCalloutResolved(CalloutResolvedEvent calloutResolvedEvent)
        {
            // Is it the callout I am on?
            if (_currentCalloutId == calloutResolvedEvent.CalloutId)
            {
                _currentCalloutId = null;
                _currentCalloutTitle = null;
                _currentCalloutSubtitle = null;
                _currentCalloutBody = null;
                _currentCalloutAttendingUnits = new();
                API.DeleteWaypoint();
                API.RemoveBlip(ref _blip);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                    "Callout Resolved",
                    "success",
                    "The callout has been resolved successfully."
                ));
            }

            return Task.FromResult(0);
        }

        internal async Task<string> GetHeadshot(Ped ped)
        {
            string texture;
            int timeout = 10;
            int headShot = API.RegisterPedheadshot(ped.Handle);

            while (timeout > 0 && (!API.IsPedheadshotReady(headShot) || !API.IsPedheadshotValid(headShot)))
            {
                timeout--;

                await Delay(200);
            }

            texture = API.GetPedheadshotTxdString(headShot);

            API.UnregisterPedheadshot(headShot);

            return texture;
        }

        private async Task AiSpeech(PlaySpeechEvent speechEvent)
        {
            _speech.Say(Entity.FromNetworkId(speechEvent.NetworkId) as Ped, speechEvent.Text);
        }

        private async Task SetFire(StartFireEvent setFireEvent)
        {
            API.StartScriptFire(setFireEvent.LocationX, setFireEvent.LocationY, setFireEvent.LocationZ, 25, true);
        }


        private async Task CalloutError(CalloutErrorNotificationEvent ErrorInCallout)
        {
            if (ErrorInCallout.CalloutId == _currentCalloutId)
            {
                _currentCalloutId = null;
                _currentCalloutTitle = null;
                _currentCalloutSubtitle = null;
                _currentCalloutBody = null;
                _currentCalloutAttendingUnits = new();
                
                API.DeleteWaypoint();
                API.RemoveBlip(ref _blip);
                
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Callout Error", "error",
                    $"There has been a problem detected with the current callout, Callout suspended.",
                    new NewNotificationMessageContent[0]));
            }
        }

        private async Task AttendeeClockedOn(UserClockedOnNotificationEvent UserClockOnNotification)
        {
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Clocked On", "info",
                $"You have now clocked onto the callout system.",
                new NewNotificationMessageContent[0]));
        }

        private async Task AttendeeClockedOff(UserClockedOffNotificationEvent UserClockOffNotification)
        {
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Clocked Off", "info",
                $"You have now clocked off the callout system.",
                new NewNotificationMessageContent[0]));
        }
    }
}
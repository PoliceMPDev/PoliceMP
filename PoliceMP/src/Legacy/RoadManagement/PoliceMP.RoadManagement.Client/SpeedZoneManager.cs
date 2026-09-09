using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.RoadManagement.Shared;

namespace PoliceMP.RoadManagement.Client
{
    public class SpeedZoneManager : BaseScript
    {
        private const float MAX_SIZE = 100f;
        private const float MIN_SIZE = 5f;
        private const float CHANGE_SIZE_SPEED = 0.1f;

        private static List<SpeedZone> _speedZones = new List<SpeedZone>();

        private static bool _isCreatingZone = false;
        private static float _scale = 5f;
        private static float _speed = 0f;

        public SpeedZoneManager()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            //EventHandlers[ClientEvents.UPDATE_SPEED_ZONES] += new Action<dynamic>(UpdateSpeedZones);
            Tick += OnTick;
        }

        private void UpdateSpeedZones(dynamic speedZones)
        {
            SendMessage("Event triggered");
            if (speedZones is List<SpeedZone> zones)
            {
                SendMessage("Yes it is a zone");
                _speedZones = zones;
            }
           
        }

        private async Task OnTick()
        {
            if (_isCreatingZone)
            {
                var playerPos = Game.PlayerPed.Position;
                World.DrawMarker(MarkerType.VerticalCylinder, playerPos, Vector3.Zero, Vector3.Zero, new Vector3(_scale, _scale, 2f), Color.FromArgb(255, 200, 100, 0));

                if (API.IsControlPressed(0, 39)) // [
                {
                    if (_scale < MAX_SIZE)
                        _scale += CHANGE_SIZE_SPEED;
                }
                else if (API.IsControlPressed(0, 40)) // ]
                {
                    if (_scale > MIN_SIZE)
                        _scale -= CHANGE_SIZE_SPEED;
                }
                else if (API.IsControlPressed(0, 23)) // F
                {
                    SendMessage("Pressed");
                    // Create SpeedZone object
                    var speedZone = new SpeedZone()
                    {
                        SpeedZoneId = API.AddSpeedZoneForCoord(playerPos.X, playerPos.Y, playerPos.Z, _scale, _speed, false),
                        BlipId = API.AddBlipForRadius(playerPos.X, playerPos.Y, playerPos.Z, _scale),
                        Scale = _scale
                    };

                    // Set the blip transparancy, colour and sprite
                    API.SetBlipAlpha(speedZone.BlipId, 100);
                    API.SetBlipColour(speedZone.BlipId, 1);
                    API.SetBlipSprite(speedZone.BlipId, 9);

                    // Add it to the speedzones list
                    _speedZones.Add(speedZone);
                    //TriggerServerEvent(ServerEvents.ADD_SPEED_ZONE, speedZone);

                    foreach (var zone in _speedZones)
                    {
                        SendMessage($"Zone {zone.BlipId} scale {zone.Scale} zone {zone.SpeedZoneId}");
                    }
                    
                    // Reset the values
                    _isCreatingZone = false;
                    _scale = 5f;
                    _speed = 0f;

                    SendMessage($"Speedzone of {_speed}kmh created.");
                }
            }
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            API.RegisterCommand("sz_create", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count > 0)
                {
                    int speed = -1;
                    if (!int.TryParse(args[0].ToString(), out speed)) return;

                    _isCreatingZone = true;
                    _speed = speed;

                    SendMessage("Use [ ] keys to change zone size. Press F when done.");
                }
                else SendMessage("Usage: /sz_create [speed]");
            }), false);

            API.RegisterCommand("sz_removeall", new Action<int, List<object>, string>((source, args, raw) =>
            {
                foreach (var speedZone in _speedZones)
                {
                    API.RemoveSpeedZone(speedZone.SpeedZoneId);
                    int blipId = speedZone.BlipId;
                    API.RemoveBlip(ref blipId);
                }

                _speedZones.Clear();

                SendMessage("Speed zones removed.");
            }), false);
        }

        private void SendMessage(string message)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 0, 0, 200 },
                args = new[] { "[Road Management]", message }
            });
        }
    }
}

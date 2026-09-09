using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Extensions;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Commands;
using PoliceMP.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Extensions;
using Prop = CitizenFX.Core.Prop;
using CitizenFX.Core.UI;
using PoliceMP.Core.Client.Communications;

namespace PoliceMP.Client.Scripts.ALP
{

    public class ALPAppliance
    {
        public ALPAppliance() 
        {
            Reset();
        }

        public void Reset()
        {
            applianceNetowrkID = -1;
            p_seat = -1;
            p_base = -1;
            p_outer = -1;
            p_middle = -1;
            p_inner = -1;
            p_end = -1;
            p_end = -1;
            p_cage = -1;
        }

        public int applianceNetowrkID {  get; set; } //NetworkID for the appliance
        public int p_seat { get; set; } //Network ID for the seat
        public float seat_rotation { get; set; }
        public int p_base { get; set; } //Network ID for the base
        public int p_outer { get; set; } //Network ID for the outer piece
        public int p_middle { get; set;} //Network ID for the middle piece
        public int p_inner { get; set; } //Network ID for the inner piece
        public int p_end { get; set; } //Network ID for the end piece
        public int p_cage { get; set;} //Network ID for the cage on the end
    }

    public class ALPScript : Script
    {
        private readonly ILogger<ALPScript> _logger;
        private readonly ICommandManager _commandManager;
        private readonly ITickManager _ticks;
        private readonly IGameInputManager _gameInputManager;
        private readonly IPermissionService _permissionService;

        private string[] ALP_PARTS = { "32m_base", "32m_cage", "32m_end_piece", "32m_inner", "32m_middle", "32m_outer", "32m_seat" };

        private ALPAppliance _activeALP = new ALPAppliance();
        private bool _controllingALP = false;
        private bool _inCage = false;

        private const float SEAT_TURN_RATE = 10f;
        private const float BASE_MAX_PITCH = 70f;
        private const float CAGE_TURN_RATE = 10f;
        private const float CAGE_PITCH_MIN = -90f;
        private const float CAGE_PITCH_MAX = 90f;

        private const float LADDER_EXTENSION_RATE = 2f;
        private const float LADDER_INNER_MAX_EXTENSION = 5f;
        private const float LADDER_MIDDLE_MAX_EXTENSION = 7f;
        private const float LADDER_OUTER_MAX_EXTENSION = 7.3f;
        private const float LADDER_END_MAX_EXTENSION = 2f;
        private const float LADDER_MAX_EXTENSION = LADDER_INNER_MAX_EXTENSION + LADDER_MIDDLE_MAX_EXTENSION + LADDER_OUTER_MAX_EXTENSION + LADDER_END_MAX_EXTENSION;

        // What degrees the seat has to be before the ladder is allowed to go down further (clearing the cab)
        private const float LADDER_LOWER_PITCH_START = 45f;

        private const float LADDER_PITCH_MIN = 0f;

        // How far can the player lower the ladder after it's cleared the cab?
        private const float LADDER_LOWER_PITCH_MIN = -20f;


        private readonly Vector3 _seatOffset = new Vector3(0f, -2.45f, 1.29f);
        private readonly Vector3 _ladderStartOffset = new Vector3(0f, -2.25f, 0.8f);
        private readonly Vector3 _ladderOuterOffset = new Vector3(0f, 4.42f, 0.77f);
        private readonly Vector3 _ladderMiddleOffset = new Vector3(0f, 0.27f, 0.09f);
        private readonly Vector3 _ladderInnerOffset = new Vector3(0f, 1.03f, -0.03f);
        private readonly Vector3 _ladderEndOffset = new Vector3(0f, -0f, 0.13f);
        private readonly Vector3 _ladderCageOffset = new Vector3(0f, 4.33f, -0.66f);

        public ALPScript(ILogger<ALPScript> logger, ICommandManager commandManager, ITickManager ticks, IGameInputManager gameInputManager, IPermissionService permissionService)
        {
            _logger = logger;
            _commandManager = commandManager;
            _ticks = ticks;
            _gameInputManager = gameInputManager;

            _commandManager.Register("alpsetup").WithHandler(async () =>
            {
                if(!CanUseALP()) return;
                SetupALP();
            });

            _commandManager.Register("alpreset").WithHandler(async () =>
            {
                if (!CanUseALP()) return;
                ResetALP();
            });

            _commandManager.Register("alpdelete").WithHandler(async () =>
            {
                if (!CanUseALP()) return;
                DeleteALP();
            });

            _commandManager.Register("alpcontrol").WithHandler(async () =>
            {
                if (!CanUseALP()) return;
                if (!_controllingALP)
                {
                    _controllingALP = true;
                    _ticks.On(ALPControlTick);
                }
                else
                {
                    _controllingALP = false;
                    _activeALP.Reset();
                    _ticks.Off(ALPControlTick);
                }
            });

            _commandManager.Register("alpcage").WithHandler(async () =>
            {
                if (_inCage)
                {
                    _logger.Debug("Already in car");
                    return;
                }

                var partHash = API.GetHashKey("32m_cage");
                Prop cage = null;

                foreach (var prop in World.GetAllProps())
                {
                    if (prop.Model.Hash != partHash)
                    {
                        continue;
                    }

                    if (cage != null)
                    {
                        var playerPos = Game.PlayerPed.Position;
                        var distanceToCage = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, cage.Position.X, cage.Position.Y, cage.Position.Z, true);
                        var distanceToProp = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, prop.Position.X, prop.Position.Y, prop.Position.Z, true);

                        if (distanceToProp < distanceToCage)
                        {
                            cage = prop;
                        }
                        continue;
                    }

                    cage = prop;
                }

                if (cage == null)
                {
                    _logger.Debug("No cage found");
                    return;
                }

                Game.PlayerPed.AttachTo(cage, new Vector3(0f, 0f, 1f));
                _ticks.On(CageControlTick);
            });

            _commandManager.Register("debugpos").WithHandler(async () =>
            {
                Dictionary<string, string> hashMap = new Dictionary<string, string>();
                foreach (var part in ALP_PARTS)
                {
                    _logger.Debug($"{part}: {API.GetHashKey(part)} {(uint)API.GetHashKey(part)}");
                    hashMap.Add(API.GetHashKey(part).ToString(), part);
                }

                var vehicle = Game.PlayerPed.CurrentVehicle;
                List<Entity> toCheck = new List<Entity>();
                foreach (var prop in World.GetAllProps())
                {
                    if (!prop.IsAttachedTo(vehicle))
                    {
                        continue;
                    }
                    toCheck.Add(prop);

                }

                for (int i = 0; i < toCheck.Count; i++)
                {
                    var e = toCheck[i];


                    string modeln = $"{e.Model.Hash}";
                    if (hashMap.ContainsKey(e.Model.Hash.ToString()))
                    {
                        modeln = hashMap[e.Model.Hash.ToString()];
                    }

                    //var offset = e.GetOffsetPosition(vehicle.Position);
                    var offsetrel = API.GetOffsetFromEntityGivenWorldCoords(Game.PlayerPed.CurrentVehicle.Handle, e.Position.X, e.Position.Y, e.Position.Z);
                    _logger.Debug($"Found a scumbag: {e.Handle} {modeln} {offsetrel.X} {offsetrel.Y} {offsetrel.Z}");
                    if (e.IsAttached())
                    {
                        _logger.Debug($"Another entity to check: {e.GetEntityAttachedTo().Handle}");
                        toCheck.Add(e.GetEntityAttachedTo());
                    }
                }


                foreach (var prop in World.GetAllProps())
                {
                    if (hashMap.ContainsKey(prop.Model.Hash.ToString()))
                    {
                        var modeln = hashMap[prop.Model.Hash.ToString()];
                        _logger.Debug($"Found: {modeln}");

                        //var offsetrel = API.GetOffsetFromEntityGivenWorldCoords(prop.GetEntityAttachedTo().Handle, prop.Position.X, prop.Position.Y, prop.Position.Z);
                        //_logger.Debug($"Position: {offsetrel.X} {offsetrel.Y} {offsetrel.Z}");
                        _logger.Debug($"Rot: {prop.Rotation.X} {prop.Rotation.Y} {prop.Rotation.Z}");

                    }
                }
            });
            _permissionService = permissionService;
        }

        private bool CanUseALP()
        {
            if (_permissionService.CurrentUserRole.Branch != UserBranch.Fire)
            {
                return false;
            }

            if (Game.PlayerPed.CurrentVehicle == null)
            {
                return false;
            }

            if (Game.PlayerPed.CurrentVehicle.Model.Hash != API.GetHashKey("lfb32"))
            {
                return false;
            }

            return true;
        }

        private void DeleteALP()
        {
            if (Game.PlayerPed.CurrentVehicle == null)
            {
                return;
            }

            var p_seat = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(Game.PlayerPed.CurrentVehicle.NetworkId, "32m_seat"));
            var p_base = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_seat.NetworkId, "32m_base"));
            var p_outer = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_base.NetworkId, "32m_outer"));
            var p_middle = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_outer.NetworkId, "32m_middle"));
            var p_inner = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_middle.NetworkId, "32m_inner"));
            var p_end = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_inner.NetworkId, "32m_end_piece"));
            var p_cage = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_end.NetworkId, "32m_cage"));

            p_cage.Delete();
            p_end.Delete();
            p_outer.Delete();
            p_middle.Delete();
            p_inner.Delete();
            p_base.Delete();
            p_seat.Delete();
            Game.PlayerPed.CurrentVehicle.Delete();
        }

        private void ResetALP()
        {
            if(Game.PlayerPed.CurrentVehicle == null)
            {
                return;
            }

            var p_seat = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(Game.PlayerPed.CurrentVehicle.NetworkId, "32m_seat"));
            var p_base = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_seat.NetworkId, "32m_base"));
            var p_outer = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_base.NetworkId, "32m_outer"));
            var p_middle = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_outer.NetworkId, "32m_middle"));
            var p_inner = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_middle.NetworkId, "32m_inner"));
            var p_end = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_inner.NetworkId, "32m_end_piece"));
            var p_cage = Entity.FromNetworkId(GetNetworkIDAttachedByPartName(p_end.NetworkId, "32m_cage"));

            p_base.AttachTo(p_seat, _ladderStartOffset);
            p_outer.AttachTo(p_base, _ladderOuterOffset);
            p_middle.AttachTo(p_outer, _ladderMiddleOffset);
            p_inner.AttachTo(p_middle, _ladderInnerOffset);
            p_end.AttachTo(p_inner, _ladderEndOffset);
            p_cage.AttachTo(p_end, _ladderCageOffset, new Vector3(90f, 0f, 0f));

            p_seat.AttachTo(Game.PlayerPed.CurrentVehicle, _seatOffset);
        }

        private Task CageControlTick()
        {
            var cage = Game.PlayerPed.GetEntityAttachedTo();

            if(cage == null)
            {
                _logger.Debug("Cage no exist");
                _inCage = false;
                _ticks.Off(CageControlTick);
                return Task.FromResult(0);
            }

            Screen.ShowSubtitle("~n~~c~NUM -~s~ Exit Cage" +
                                "~n~~c~NUM +~s~ Exit Cage (At Vehicle)"
                                 , 0);

            if (_gameInputManager.IsPressed(Control.VehicleCinematicDownOnly)) //Numpad -
            {
                Game.PlayerPed.Detach();
                _inCage = false;
                _ticks.Off(CageControlTick);
                return Task.FromResult(0);
            }

            if (_gameInputManager.IsPressed(Control.VehicleCinematicUpOnly)) //Numpad +
            {
                var vehicleHashHash = API.GetHashKey("lfb32");
                Vehicle closestVehicle = null;

                foreach (var vehicle in World.GetAllVehicles())
                {
                    
                    if (vehicle.Model.Hash != vehicleHashHash)
                    {
                        continue;
                    }

                    _logger.Debug("found");

                    if (closestVehicle != null)
                    {
                        var playerPos = Game.PlayerPed.Position;
                        var distanceToCloseV = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, closestVehicle.Position.X, closestVehicle.Position.Y, closestVehicle.Position.Z, true);
                        var distanceToVeh = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, true);

                        if (distanceToVeh < distanceToCloseV)
                        {
                            closestVehicle = vehicle;
                        }
                        continue;
                    }

                    closestVehicle = vehicle;
                }
                Game.PlayerPed.Detach();
                if (closestVehicle != null)
                {
                    var offset = API.GetOffsetFromEntityInWorldCoords(closestVehicle.Handle, 0f, -8f, 0f);
                    _logger.Debug($"Setting pos to {offset}");
                    Game.PlayerPed.Position = offset;
                }
                _inCage = false;
                _ticks.Off(CageControlTick);
                return Task.FromResult(0);
            }

            return Task.FromResult(0);
        }

        private Task ALPControlTick()
        {
            if (!Game.PlayerPed.IsInVehicle())
            {
                _activeALP.Reset();
                _controllingALP = false;
                _ticks.Off(ALPControlTick);
                return Task.FromResult(0);
            }

            
            //Get parts attached to the ALP.
            if(_activeALP.applianceNetowrkID == -1)
            {
                _activeALP.applianceNetowrkID = Game.PlayerPed.CurrentVehicle.NetworkId;

                _activeALP.p_seat = GetNetworkIDAttachedByPartName(Game.PlayerPed.CurrentVehicle.NetworkId, "32m_seat");
                _activeALP.p_base = GetNetworkIDAttachedByPartName(_activeALP.p_seat, "32m_base");

                _activeALP.p_outer = GetNetworkIDAttachedByPartName(_activeALP.p_base, "32m_outer");
                _activeALP.p_middle = GetNetworkIDAttachedByPartName(_activeALP.p_outer, "32m_middle");
                _activeALP.p_inner = GetNetworkIDAttachedByPartName(_activeALP.p_middle, "32m_inner");
                _activeALP.p_end = GetNetworkIDAttachedByPartName(_activeALP.p_inner, "32m_end_piece");
                _activeALP.p_cage = GetNetworkIDAttachedByPartName(_activeALP.p_end, "32m_cage");

                _logger.Debug($"{_activeALP.applianceNetowrkID} {_activeALP.p_seat} {_activeALP.p_outer} {_activeALP.p_middle} {_activeALP.p_inner} {_activeALP.p_end} {_activeALP.p_cage}");
            }

            Screen.ShowSubtitle("~n~~c~NUM 4/6~s~ Rotate Seat" +
                                "~n~~c~NUM 5/8~s~ Adjust Height" +
                                "~n~~c~NUM 7/9~s~ Adjust Length" +
                                "~n~~c~NUM +/-~s~ Rotate Basket" 
                                 , 0);


            //Handle controls

            if (_gameInputManager.IsPressed(Control.VehicleFlyRollRightOnly)) //Numpad 6
            {
                AddArmYaw(SEAT_TURN_RATE * Game.LastFrameTime * -1);
            }

            if (_gameInputManager.IsPressed(Control.VehicleFlyRollLeftOnly)) //Numpad 4
            {               
                AddArmYaw(SEAT_TURN_RATE * Game.LastFrameTime);
            }

            if (_gameInputManager.IsPressed(Control.VehicleFlyPitchUpOnly)) //Numpad 8
            {
                AddArmPitch(SEAT_TURN_RATE * Game.LastFrameTime);
            }

            if (_gameInputManager.IsPressed(Control.VehicleFlyPitchDownOnly)) //Numpad 5
            {
                AddArmPitch(SEAT_TURN_RATE * Game.LastFrameTime * -1);
            }

            if (_gameInputManager.IsPressed(Control.VehicleFlySelectTargetRight)) //Numpad 9
            {
                ChangeLadderExtension(LADDER_EXTENSION_RATE * Game.LastFrameTime);
            }

            if (_gameInputManager.IsPressed(Control.VehicleFlySelectTargetLeft)) //Numpad 7
            {
                ChangeLadderExtension(LADDER_EXTENSION_RATE * Game.LastFrameTime * -1);
            }

            if (_gameInputManager.IsPressed(Control.VehicleCinematicUpOnly)) //Numpad -
            {
                AddBoxPitch(CAGE_TURN_RATE * Game.LastFrameTime * -1);
            }

            if (_gameInputManager.IsPressed(Control.VehicleCinematicDownOnly)) //Numpad +
            {
                AddBoxPitch(CAGE_TURN_RATE * Game.LastFrameTime);
            }

            return Task.FromResult(0);
        }

        private void AddBoxPitch(float degrees)
        {
            var part = Entity.FromNetworkId(_activeALP.p_cage);
            var connectedPart = Entity.FromNetworkId(_activeALP.p_end);

            // Calculate the part's rotation in world space
            Quaternion partRelativeRotation = Quaternion.Invert(connectedPart.Quaternion) * part.Quaternion;

            // Create a quaternion representing the rotation around the X axis in world space
            Quaternion rotationX = QuaternionTools.AngleAxis(degrees, Vector3.Right);

            // Apply the rotation to the part's world rotation
            Quaternion newPartRelativeRotation = rotationX * partRelativeRotation;

            // Ensure we don't get any velocity related skews on the roll and yaw
            Vector3 euler = newPartRelativeRotation.ToEulerAngles();
            euler.Z = 0;
            euler.Y = 0;

            euler.X = MathUtil.Clamp(euler.X, CAGE_PITCH_MIN, CAGE_PITCH_MAX);

            //euler.X = MathUtil.Clamp(euler.X, 0f, 360f);

            _logger.Debug($"Rotating: {euler.X} {euler.Y} {euler.Z}");

            // Attach the part using the new relative rotation
            part.AttachTo(connectedPart, _ladderCageOffset, euler);
        }

        private void AddArmYaw(float degrees)
        {
            var part = Entity.FromNetworkId(_activeALP.p_seat);
            var @base = Entity.FromNetworkId(_activeALP.p_base);
            var connectedPart = Entity.FromNetworkId(Game.PlayerPed.CurrentVehicle.NetworkId);

            // Calculate the part's rotation in world space
            Quaternion partRelativeRotation = Quaternion.Invert(connectedPart.Quaternion) * part.Quaternion;

            var currentYaw = partRelativeRotation.ToEulerAngles().Z;

            // Create a quaternion representing the rotation around the Z axis in world space
            Quaternion rotationZ = QuaternionTools.AngleAxis(degrees, Vector3.Up);

            // Apply the rotation to the part's world rotation
            Quaternion newPartRelativeRotation = rotationZ * partRelativeRotation;

            // Ensure we don't get any velocity related skews on the roll and pitch
            Vector3 euler = newPartRelativeRotation.ToEulerAngles();
            euler.X = 0;
            euler.Y = 0;

            _logger.Debug($"Rotating: {euler.X} {euler.Y} {euler.Z}");

            if(Math.Abs(currentYaw) > LADDER_LOWER_PITCH_START && Math.Abs(euler.Z) < LADDER_LOWER_PITCH_START &&
               (Quaternion.Invert(part.Quaternion) * @base.Quaternion).ToEulerAngles().X < LADDER_PITCH_MIN)
            {
                // We're trying to move into the cab. Abort!
                return;
            }

            // Attach the part using the new relative rotation
            part.AttachTo(connectedPart, _seatOffset, euler);
        }

        private void AddArmPitch(float degrees)
        {
            var part = Entity.FromNetworkId(_activeALP.p_base);
            var vehicle = Entity.FromNetworkId(_activeALP.applianceNetowrkID);
            var connectedPart = Entity.FromNetworkId(_activeALP.p_seat);
            
            // Calculate the part's rotation in world space
            Quaternion partRelativeRotation = Quaternion.Invert(connectedPart.Quaternion) * part.Quaternion;

            // Create a quaternion representing the rotation around the X axis in world space
            Quaternion rotationX = QuaternionTools.AngleAxis(degrees, Vector3.Right);

            // Apply the rotation to the part's world rotation
            Quaternion newPartRelativeRotation = rotationX * partRelativeRotation;

            // Ensure we don't get any velocity related skews on the roll and yaw
            Vector3 euler = newPartRelativeRotation.ToEulerAngles();
            euler.Z = 0;
            euler.Y = 0;

            var clampedDegrees = euler.X;

            var seatRot = (Quaternion.Invert(vehicle.Quaternion) * connectedPart.Quaternion).ToEulerAngles().Z;
            var min = Math.Abs(seatRot) > LADDER_LOWER_PITCH_START 
                ? LADDER_LOWER_PITCH_MIN 
                : LADDER_PITCH_MIN;

            euler.X = MathUtil.Clamp(euler.X, min, BASE_MAX_PITCH);

            clampedDegrees -= euler.X;

            _logger.Debug($"Rotating: {euler.X} {euler.Y} {euler.Z}");

            // Attach the part using the new relative rotation
            part.AttachTo(connectedPart, _ladderStartOffset, euler);

            // Add the inverse pitch to the box to keep it level
            AddBoxPitch((degrees - clampedDegrees) * -1);
        }

        private void ChangeLadderExtension(float delta)
        {
            // figure out the current extension
            var vehicle = Entity.FromNetworkId(_activeALP.applianceNetowrkID);
            var seat = Entity.FromNetworkId(_activeALP.p_seat);
            var @base = Entity.FromNetworkId(_activeALP.p_base);
            var inner = Entity.FromNetworkId(_activeALP.p_inner);
            var middle = Entity.FromNetworkId(_activeALP.p_middle);
            var outer = Entity.FromNetworkId(_activeALP.p_outer);
            var end = Entity.FromNetworkId(_activeALP.p_end);

            //var extension = vehicle.State.Get<float>("extension");

            float innerExtension = 0f;
            float middleExtension = 0f;
            float outerExtension = 0f;
            float endExtension = 0f;

            // calculate the extension based on offsets to each parts parent
            var extension = 0f;
            extension += World.GetDistance(@base.GetOffsetPosition(_ladderOuterOffset), outer.Position);
            extension += World.GetDistance(outer.GetOffsetPosition(_ladderMiddleOffset), middle.Position);
            extension += World.GetDistance(middle.GetOffsetPosition(_ladderInnerOffset), inner.Position);
            extension += World.GetDistance(inner.GetOffsetPosition(_ladderEndOffset), end.Position);
            extension += delta;

            extension = MathUtil.Clamp(extension, 0f, LADDER_MAX_EXTENSION);

            // Calculate each part extension from
            float partExtension = extension;

            if (partExtension > 0)
            {
                endExtension = partExtension is > 0 and < LADDER_END_MAX_EXTENSION
                    ? partExtension
                    : LADDER_END_MAX_EXTENSION;

                partExtension -= LADDER_END_MAX_EXTENSION;
            }

            if (partExtension > 0)
            {
                innerExtension = partExtension < LADDER_INNER_MAX_EXTENSION
                    ? partExtension
                    : LADDER_INNER_MAX_EXTENSION;

                partExtension -= LADDER_INNER_MAX_EXTENSION;
            }

            if (partExtension > 0)
            {
                middleExtension = partExtension < LADDER_MIDDLE_MAX_EXTENSION 
                    ? partExtension 
                    : LADDER_MIDDLE_MAX_EXTENSION;

                partExtension -= LADDER_MIDDLE_MAX_EXTENSION;
            }

            if (partExtension > 0)
            {
                outerExtension = partExtension is > 0 and < LADDER_OUTER_MAX_EXTENSION
                    ? partExtension
                    : LADDER_OUTER_MAX_EXTENSION;
            }

            // log all the extension values
            _log.Debug($"Extension: {extension}");
            _log.Debug($"Inner: {innerExtension}");
            _log.Debug($"Middle: {middleExtension}");
            _log.Debug($"Outer: {outerExtension}");
            _log.Debug($"End: {endExtension}");

            end.Detach();
            outer.Detach();
            middle.Detach();
            inner.Detach();

            outer.AttachTo(@base, _ladderOuterOffset + Vector3.Forward * outerExtension, Vector3.Zero);
            middle.AttachTo(outer, _ladderMiddleOffset + Vector3.Forward * middleExtension, Vector3.Zero);
            inner.AttachTo(middle, _ladderInnerOffset + Vector3.Forward * innerExtension, Vector3.Zero);
            end.AttachTo(inner, _ladderEndOffset + Vector3.Forward * endExtension, Vector3.Zero);
        }

        private int GetNetworkIDAttachedByPartName(int entityAttachedTo, string part)
        {
            var partHash = API.GetHashKey(part);
            var entity = Entity.FromNetworkId(entityAttachedTo);

            foreach (var prop in World.GetAllProps())
            {
                if (!prop.IsAttachedTo(entity))
                {
                    continue;
                }    

                if(prop.Model.Hash != partHash)
                {
                    continue;
                }

                return prop.NetworkId;
            }

            return -1;
        }

        private async void SetupALP()
        {
            if(!Game.PlayerPed.IsInVehicle()) 
            {
                _logger.Debug("Not in a motor");
                return;
            }

            var vehicle = Game.PlayerPed.CurrentVehicle;

            var existingSeat = GetNetworkIDAttachedByPartName(Game.PlayerPed.CurrentVehicle.NetworkId, "32m_seat");
            if (existingSeat != -1)
            {
                _logger.Debug("ALP Already setup");
                return;
            }

            foreach (var part in ALP_PARTS)
            {
                if (!API.IsModelValid((uint)API.GetHashKey(part)))
                {
                    _logger.Debug($"Bad part: {part}");
                    continue;
                }

                int breaker = 100;
                while(!API.HasModelLoaded((uint)API.GetHashKey(part)) && breaker > 0)
                {
                    breaker--;
                    API.RequestModel((uint)API.GetHashKey(part));
                    await Delay(0);
                }
            }

            foreach (var part in ALP_PARTS)
            {
                if (!API.HasModelLoaded((uint)API.GetHashKey(part)))
                {
                    _logger.Debug("Parts, not loaded, come back");
                    return;
                }
            }

            //Build the ALP off the vehicle
            var ladderbase = World.CreateProp(new Model("32m_seat"), Game.PlayerPed.Position, true, false).Result;
            var ladderstart = World.CreateProp(new Model("32m_base"), Game.PlayerPed.Position, true, false).Result;

            ladderstart.AttachTo(ladderbase, _ladderStartOffset);

            //Outer
            var ladderouter = World.CreateProp(new Model("32m_outer"), Game.PlayerPed.Position, true, false).Result;
            ladderouter.AttachTo(ladderstart, _ladderOuterOffset);

            //Middle
            var laddermiddle = World.CreateProp(new Model("32m_middle"), Game.PlayerPed.Position, true, false).Result;
            laddermiddle.AttachTo(ladderouter, _ladderMiddleOffset);

            //Inner
            var ladderinner = World.CreateProp(new Model("32m_inner"), Game.PlayerPed.Position, true, false).Result;
            ladderinner.AttachTo(laddermiddle, _ladderInnerOffset);


            //Endpiece
            var ladderend = World.CreateProp(new Model("32m_end_piece"), Game.PlayerPed.Position, true, false).Result;
            ladderend.AttachTo(ladderinner, _ladderEndOffset);

            //Cage
            var laddercage = World.CreateProp(new Model("32m_cage"), Game.PlayerPed.Position, true, false).Result;
            laddercage.AttachTo(ladderend, _ladderCageOffset, new Vector3(90f, 0f, 0f));

            //Attach last prop to the vehicle
            ladderbase.AttachTo(vehicle, _seatOffset);

            // Release the models for GC
            foreach (var part in ALP_PARTS)
            {
                API.SetModelAsNoLongerNeeded((uint)API.GetHashKey(part));
            }
        }

        private static class PartOffsets
        {
            static readonly Vector3 Ladder = new Vector3(0f, -2.45f, 1.29f);
        }
    }
}

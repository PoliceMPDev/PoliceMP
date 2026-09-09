using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using PoliceMP.Client.Extensions;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using Prop = CitizenFX.Core.Prop;

namespace PoliceMP.Client.Scripts.RoadManagement
{
    public class RoadManagement : Script
    {
        #region Services

        private readonly ILogger<RoadManagement> _logger;
        private readonly IPermissionService _permission;
        private readonly ILegacyClientCommunicationsManager _comms;
        private UserAces _access;

        #endregion Services

        #region Variables
        private readonly string ROADDECOR = "PMP_RoadMan_DecorProp";
        private Menu _roadManagementMenu = new("Prop Menu");
        private Menu _civPropMenu = new("Civ Props");
        private Menu PropMenu = new Menu("Props");
        private List<MenuItem> PropCategoriesMenu = new List<MenuItem>();
        private List<PropCategory> PropCategories = new List<PropCategory>();

        private Prop _attachedProp;

        private float _radius = 5.0f;

        private int _marker;

        private Dictionary<PmpVector3, int> _speedZones = new();

        private Dictionary<int, int> _navmeshBlocks = new Dictionary<int, int>();
        private bool _nodeDebug = false;

        #endregion Variables

        public RoadManagement(ILogger<RoadManagement> logger, ICommandManager command, IPermissionService permission, ILegacyClientCommunicationsManager comms, ITickManager tickManager)
        {
            _logger = logger;
            _permission = permission;
            _comms = comms;

            _comms.OnRequest<PmpVector3, PmpVector3>(ServerEvents.FetchNodePosition, FetchNearestRoadNodePosition);
            _comms.On<PmpVector3, bool>(ServerEvents.UpdateNodeAtPosition, OnUpdateNodeAtPosition);
            _comms.On<PmpVector3, int>(ServerEvents.UpdateSpeedAtPosition, OnSetSpeedZoneInArea);
            _comms.On<bool>(ClientEvents.PlayerSpawned, async (firstSpawn) =>
            {
                await GetPropData();
                _logger.Debug($"Received {PropCategories?.Count} props!");
            });

            command.Register("roadmanagement").WithHandler(async () =>
            {
                if (MenuController.IsAnyMenuOpen()) return;
                if (_roadManagementMenu.Visible)
                {
                    _roadManagementMenu.CloseMenu();
                    return;
                }

                await CreateRoadManagementMenu();
            });
            command.Register("debugnodes").WithHandler(() =>
            {
                if (_nodeDebug)
                {
                    _logger.Debug("Turning off Debug Mode");
                    _nodeDebug = false;
                    tickManager.Off(DebugNodeTick);
                }
                else
                {
                    _logger.Debug($"Turning on Debug Nodes");
                    _nodeDebug = true;
                    tickManager.On(DebugNodeTick);
                }
            });
            API.RegisterKeyMapping("roadmanagement", "Road Management", "keyboard", "F5");

            tickManager.On(RotatePropTick);
            tickManager.On(NavmeshBlockingTick);
            //tickManager.On(SpikeStripTick);
        }

        private async Task NavmeshBlockingTick()
        {
            if(_navmeshBlocks != null)
            {
                foreach (var block in _navmeshBlocks.ToArray())
                {
                    bool removeBlock = false;

                    if (!API.DoesEntityExist(block.Key))
                    {
                        removeBlock = true; 
                    }
                    else
                    {
                        var entity = Entity.FromHandle(block.Key);
                        float distance = Vector3.Distance(Game.PlayerPed.Position, entity.Position);

                        if (distance >= 160f)
                        {
                            removeBlock = true;
                        }
                    }


                    if (!removeBlock)
                    {
                        continue;
                    }

                    API.RemoveNavmeshBlockingObject(block.Value);
                    lock (_navmeshBlocks)
                    {
                        _logger.Debug($"Removing Navmesh Block: {block.Key} {block.Value}");
                        _navmeshBlocks.Remove(block.Key);
                    }
                    await Delay(50);
                }
            }

            foreach (var prop in World.GetAllProps())
            {
                if (!API.DecorExistOn(prop.Handle, ROADDECOR))
                {
                    continue;
                }

                if (_navmeshBlocks != null && _navmeshBlocks.ContainsKey(prop.Handle))
                {
                    continue;
                }

                AddNavmeshBlock(prop.Handle);
                await Delay(50);
            }
            await Delay(500);
        }

        protected override async Task OnStartAsync()
        {
            _access = await _permission.GetUserAces();
            _roadManagementMenu = new Menu("Prop Menu", "Main Menu");

            API.DecorRegister(ROADDECOR, 2);
        }

        private async Task DebugNodeTick()
        {
            if (Game.PlayerPed == null) return;

            var nearestNode = await FetchNearestRoadNodePosition(Game.PlayerPed.Position.ToPmpVector3());

            API.DrawMarker(28, nearestNode.X, nearestNode.Y, nearestNode.Z, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 255, 0, 0, 255, false, false, 2, false, null, null, false);

        }


        private async Task GetPropData()
        {
            var obj = await _comms.Request<List<PropCategory>>(ServerEvents.GetPropData);

            PropMenu.ClearMenuItems();
            PropCategoriesMenu.Clear();
            PropCategories.Clear();

            foreach (var pc in obj)
            {
                PropCategories.Add(pc);
            }
        }

        // Class-level (persistent) variables:
private Vector3 _propOffset = new Vector3(0f, -1f, -0.6f);  // initial offset – adjust as needed (local: X=right, Y=forward, Z=up)
private Vector3 _propRotation = Vector3.Zero;                // initial rotation (X=pitch, Y=roll, Z=yaw)
private const float STEPVALUE = 1.0f;  // Rotation step
private const float MOVE_STEPVALUE = 0.10f;  // Move step

private async Task RotatePropTick()
{
    if (_attachedProp == null)
    {
        _propOffset = new Vector3(0f, -1f, -0.6f);
        return;
    }

    bool ctrlPressed = false;
    
    // Show control instructions.
    Screen.ShowSubtitle("~c~Q~s~ Rotate Left" +
                          "~n~~c~E~s~ Rotate Right" +
                          "~n~~c~NUM 7~s~ Rotate Up" +
                          "~n~~c~NUM 9~s~ Rotate Down" +
                          "~n~~c~NUM 8~s~ Forward" +
                          "~n~~c~NUM 4~s~ Left" +
                          "~n~~c~NUM 5~s~ Back" +
                          "~n~~c~NUM 6~s~ Right" +
                          "~n~~c~NUM -~s~ Decrease Height" +
                          "~n~~c~NUM +~s~ Increase Height" +
                          "~n~~c~Z~s~ Floor Snap (Beta)", 0);

    // ----- Floor Alignment Key (Z) -----
    if (Game.IsControlJustPressed(0, (Control)20)) // Z key: auto-align prop to floor
    {
        await AlignPropToFloor();
        return; // skip further movement this tick
    }

    // ----- Rotation Controls (Yaw on Z axis) -----
    if (API.IsControlPressed(0, 44)) // Left bracket key (rotate left/yaw)
    {
        _propRotation.Z += STEPVALUE;
        ctrlPressed = true;
    }
    else if (API.IsControlPressed(0, 38)) // Right bracket key (rotate right/yaw)
    {
        _propRotation.Z -= STEPVALUE;
        ctrlPressed = true;
    }

    // ----- Additional Rotation Controls (Pitch on X axis) -----
    if (Game.IsControlPressed(0, (Control)117)) // NUMPAD 7: Rotate Up (increase pitch)
    {
        _propRotation.X += STEPVALUE;
        ctrlPressed = true;
    }
    else if (Game.IsControlPressed(0, (Control)118)) // NUMPAD 9: Rotate Down (decrease pitch)
    {
        _propRotation.X -= STEPVALUE;
        ctrlPressed = true;
    }

    // Get the player's heading in radians.
    float heading = Game.PlayerPed.Heading * (float)Math.PI / 180f;
    float cos = (float)Math.Cos(heading);
    float sin = (float)Math.Sin(heading);

    // Movement controls relative to player's direction.
    Vector3 moveOffset = Vector3.Zero;
    if (Game.IsControlPressed(0, (Control)111)) // NUMPAD 8: Move Forward
    {
        moveOffset.Y += MOVE_STEPVALUE;
        ctrlPressed = true;
    }
    if (Game.IsControlPressed(0, (Control)108)) // NUMPAD 4: Move Left
    {
        moveOffset.X -= MOVE_STEPVALUE;
        ctrlPressed = true;
    }
    if (Game.IsControlPressed(0, (Control)109)) // NUMPAD 6: Move Right
    {
        moveOffset.X += MOVE_STEPVALUE;
        ctrlPressed = true;
    }
    if (Game.IsControlPressed(0, (Control)110)) // NUMPAD 5: Move Back
    {
        moveOffset.Y -= MOVE_STEPVALUE;
        ctrlPressed = true;
    }
    if (Game.IsControlPressed(0, (Control)97)) // Pg Dwn: Decrease Height
    {
        moveOffset.Z -= MOVE_STEPVALUE;
        ctrlPressed = true;
    }
    if (Game.IsControlPressed(0, (Control)96)) // Pg Up: Increase Height
    {
        moveOffset.Z += MOVE_STEPVALUE;
        ctrlPressed = true;
    }

    if (!ctrlPressed)
    {
        return;
    }
    
    // Rotate the movement offset relative to the player's heading.
    Vector3 rotatedMoveOffset = new Vector3(
        moveOffset.X * cos - moveOffset.Y * sin,  // X
        moveOffset.X * sin + moveOffset.Y * cos,  // Y
        moveOffset.Z                              // Z remains unchanged
    );
    
    // Update the prop offset.
    _propOffset += rotatedMoveOffset;
    
    // Reattach the prop using the updated offset/rotation.
    API.AttachEntityToEntity(
        _attachedProp.Handle,
        Game.PlayerPed.Handle,
        11816,
        _propOffset.X, _propOffset.Y, _propOffset.Z,
        _propRotation.X, _propRotation.Y, _propRotation.Z,
        false, false, false, false, 2, false);
}


/// <summary>
/// Adjusts the prop’s pitch/roll and vertical offset so its “bottom” fits the ground.
/// </summary>
private async Task AlignPropToFloor()
{
    if (_attachedProp == null)
        return;
    
    // Get player's current position and heading (in radians).
    Vector3 playerPos = Game.PlayerPed.Position;
    float playerHeadingRad = Game.PlayerPed.Heading * (float)Math.PI / 180f;
    
    // Calculate the base position for the prop using the current offset.
    float cos = (float)Math.Cos(playerHeadingRad);
    float sin = (float)Math.Sin(playerHeadingRad);
    Vector3 rotatedOffset = new Vector3(
        _propOffset.X * cos - _propOffset.Y * sin,
        _propOffset.X * sin + _propOffset.Y * cos,
        _propOffset.Z
    );
    Vector3 basePropPos = playerPos + rotatedOffset;
    
    // Get the prop’s model dimensions.
    int modelHash = API.GetEntityModel(_attachedProp.Handle);
    Vector3 minDim = Vector3.Zero, maxDim = Vector3.Zero;
    API.GetModelDimensions((uint)modelHash, ref minDim, ref maxDim);
    
    // Define the local bottom corners (using the model’s local coordinates).
    // (Assumes local axes: X = right, Y = forward, Z = up.)
    Vector3 localBackLeft  = new Vector3(minDim.X, minDim.Y, minDim.Z);
    Vector3 localBackRight = new Vector3(maxDim.X, minDim.Y, minDim.Z);
    Vector3 localFrontLeft = new Vector3(minDim.X, maxDim.Y, minDim.Z);
    Vector3 localFrontRight= new Vector3(maxDim.X, maxDim.Y, minDim.Z);
    
    // The total (world) yaw is the sum of the player's heading and the prop’s local yaw.
    float totalYaw = Game.PlayerPed.Heading + _propRotation.Z; // in degrees
    Vector3 totalRotation = new Vector3(_propRotation.X, _propRotation.Y, totalYaw);
    
    // Local function: transform a local point into world space.
    Vector3 TransformLocalToWorld(Vector3 localPoint)
    {
        return basePropPos + RotateVectorByEuler(localPoint, totalRotation);
    }
    
    // Get world positions for each bottom corner.
    Vector3 worldBackLeft  = TransformLocalToWorld(localBackLeft);
    Vector3 worldBackRight = TransformLocalToWorld(localBackRight);
    Vector3 worldFrontLeft = TransformLocalToWorld(localFrontLeft);
    Vector3 worldFrontRight= TransformLocalToWorld(localFrontRight);
    
    // Get ground Z for each corner.
    float groundBL = 0f, groundBR = 0f , groundFL = 0f, groundFR = 0f;
    API.GetGroundZFor_3dCoord(worldBackLeft.X,  worldBackLeft.Y,  worldBackLeft.Z  + 5f, ref groundBL, false);
    API.GetGroundZFor_3dCoord(worldBackRight.X, worldBackRight.Y, worldBackRight.Z + 5f, ref groundBR, false);
    API.GetGroundZFor_3dCoord(worldFrontLeft.X,  worldFrontLeft.Y,  worldFrontLeft.Z  + 5f, ref groundFL, false);
    API.GetGroundZFor_3dCoord(worldFrontRight.X, worldFrontRight.Y, worldFrontRight.Z + 5f, ref groundFR, false);
    
    // Average the ground heights for front/back and left/right.
    float frontAvg = (groundFL + groundFR) / 2f;
    float backAvg  = (groundBL + groundBR) / 2f;
    float leftAvg  = (groundFL + groundBL) / 2f;
    float rightAvg = (groundFR + groundBR) / 2f;
    
    // Use the local (model) dimensions for distance.
    float forwardDist = Math.Abs(maxDim.Y - minDim.Y);
    float sideDist = Math.Abs(maxDim.X - minDim.X);
    
    // Compute the desired pitch (rotation around X) and roll (rotation around Y) in degrees.
    float desiredPitch = (float)(Math.Atan2(frontAvg - backAvg, forwardDist) * 180f / Math.PI);
    float desiredRoll  = (float)(Math.Atan2(rightAvg - leftAvg, sideDist) * 180f / Math.PI);
    
    // Update the prop’s rotation (keeping yaw intact).
    _propRotation.X = desiredPitch;
    _propRotation.Y = desiredRoll;
    
    // Recalculate the total rotation and transform the corners again.
    totalRotation = new Vector3(_propRotation.X, _propRotation.Y, totalYaw);
    worldBackLeft  = TransformLocalToWorld(localBackLeft);
    worldBackRight = TransformLocalToWorld(localBackRight);
    worldFrontLeft = TransformLocalToWorld(localFrontLeft);
    worldFrontRight= TransformLocalToWorld(localFrontRight);
    
    // Get updated ground heights.
    API.GetGroundZFor_3dCoord(worldBackLeft.X,  worldBackLeft.Y,  worldBackLeft.Z  + 5f, ref groundBL, false);
    API.GetGroundZFor_3dCoord(worldBackRight.X, worldBackRight.Y, worldBackRight.Z + 5f, ref groundBR, false);
    API.GetGroundZFor_3dCoord(worldFrontLeft.X,  worldFrontLeft.Y,  worldFrontLeft.Z  + 5f, ref groundFL, false);
    API.GetGroundZFor_3dCoord(worldFrontRight.X, worldFrontRight.Y, worldFrontRight.Z + 5f, ref groundFR, false);
    
    // Determine the vertical gap (difference between prop corner and ground) for each corner.
    float gapBL = worldBackLeft.Z  - groundBL;
    float gapBR = worldBackRight.Z - groundBR;
    float gapFL = worldFrontLeft.Z - groundFL;
    float gapFR = worldFrontRight.Z- groundFR;
    float minGap = Math.Min(Math.Min(gapBL, gapBR), Math.Min(gapFL, gapFR));
    
    // Adjust the prop’s vertical offset so that the lowest bottom corner is flush with the ground.
    _propOffset.Z -= minGap;
    
    // Finally, reattach the prop with the new offset and rotation.
    API.AttachEntityToEntity(
         _attachedProp.Handle,
         Game.PlayerPed.Handle,
         11816,
         _propOffset.X, _propOffset.Y, _propOffset.Z,
         _propRotation.X, _propRotation.Y, _propRotation.Z,
         false, false, false, false, 2, false
    );
}

/// <summary>
/// Rotates a vector by a set of Euler angles (in degrees).
/// Uses GTA’s convention: X = pitch, Y = roll, Z = yaw.
/// </summary>
private Vector3 RotateVectorByEuler(Vector3 vec, Vector3 rotationDegrees)
{
    float pitch = rotationDegrees.X * (float)Math.PI / 180f;
    float roll  = rotationDegrees.Y * (float)Math.PI / 180f;
    float yaw   = rotationDegrees.Z * (float)Math.PI / 180f;
    Quaternion q = CreateFromYawPitchRoll(yaw, pitch, roll);
    return Vector3.Transform(vec, q);
}


/// <summary>
/// Creates a quaternion from yaw, pitch, and roll (all in radians).
/// GTA’s convention: X = pitch, Y = roll, Z = yaw.
/// </summary>
private static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
{
    // Calculate half-angles.
    float halfYaw   = yaw * 0.5f;
    float halfPitch = pitch * 0.5f;
    float halfRoll  = roll * 0.5f;

    // Compute sine and cosine of half-angles.
    float cosYaw   = (float)Math.Cos(halfYaw);
    float sinYaw   = (float)Math.Sin(halfYaw);
    float cosPitch = (float)Math.Cos(halfPitch);
    float sinPitch = (float)Math.Sin(halfPitch);
    float cosRoll  = (float)Math.Cos(halfRoll);
    float sinRoll  = (float)Math.Sin(halfRoll);

    Quaternion q = new Quaternion();
    // These formulas match common implementations (like XNA’s Quaternion.CreateFromYawPitchRoll):
    q.W = cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw;
    q.X = sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw;
    q.Y = cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw;
    q.Z = cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw;

    return q;
}



        
        private async Task SpikeStripTick()
        {
            var props = World.GetAllProps()
                .Where(prop => prop != null && prop.Model.Hash == API.GetHashKey("P_ld_stinger_s")
                                            && prop.HeightAboveGround <= 0.2f)
                .ToArray();

            var vehicles = World.GetAllVehicles()
                .Where(vehicle => vehicle.HasNetworkControl())
                .ToArray();

            await Task.WhenAll(props.SelectMany(prop =>
            {
                var propPosition = prop.Position;
                return vehicles.Where(vehicle => propPosition.Distance(vehicle.Position) <= 3)
                    .Select(async vehicle =>
                    {
                        for (var i = 0; i < 6; i++)
                        {
                            if (API.IsVehicleTyreBurst(vehicle.Handle, i, false)) continue;
                            API.SetVehicleTyreBurst(vehicle.Handle, i, true, 1000);
                        }
                        await Delay(0);
                    });
            }));
        }


        private async Task CreateRoadManagementMenu()
        {
            if (Game.PlayerPed.IsInVehicle()) return;
            MenuController.CloseAllMenus();
            _roadManagementMenu.ClearMenuItems();

            _logger.Debug($"Menu items: {_roadManagementMenu.GetMenuItems().Count}");

            foreach (PropCategory pc in PropCategories)
            {
                if (pc.AllowedBranches.Any(ab => ab.Contains(_permission.CurrentUserRole.Branch.ToString())) || _access.IsAdmin || _access.IsDeveloper)
                {
                    var categoryMenu = new Menu(pc.CategoryName, pc.CategoryName + " Menu");
                    MenuController.AddSubmenu(_roadManagementMenu, categoryMenu);

                    var propMenuItem = new MenuItem(pc.CategoryName) { Label = "→" };
                    _roadManagementMenu.AddMenuItem(propMenuItem);
                    MenuController.BindMenuItem(_roadManagementMenu, categoryMenu, propMenuItem);

                    foreach (var prop in pc.Props)
                    {
                        categoryMenu.AddMenuItem(new MenuItem(prop.PropName)
                        {
                            ItemData = prop.PropFile
                        });
                    }

                    categoryMenu.OnItemSelect += async (_menu, _item, _index) =>
                    {
                        _logger.Debug($"Is Attached Prop Null? {_attachedProp == null}");

                        if (_attachedProp == null)
                        {
                            await CreateLocalProp(_item.ItemData);
                        }
                        else
                        {
                            await CreateServerProp();
                        }
                    };
                }
            }

            _roadManagementMenu.AddMenuItem(new MenuItem("Delete Item")
            {
                LeftIcon = MenuItem.Icon.WARNING
            });

            _roadManagementMenu.AddMenuItem(new MenuItem("Close")
            {
                LeftIcon = MenuItem.Icon.INFO
            });

            MenuController.AddMenu(_roadManagementMenu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);

            _roadManagementMenu.OnItemSelect += RoadManagementMenuOnItemSelect;

            _roadManagementMenu.OpenMenu();
        }

        private async void RoadManagementMenuOnItemSelect(Menu menu, MenuItem menuitem, int itemindex)
        {
            try
            {
                await Script.Delay(1);
                if (menu == _roadManagementMenu)
                {
                    if (menuitem.Text == "Close")
                    {
                        menu.CloseMenu();
                        return;
                    }

                    if (menuitem.Text == "Delete Item")
                    {
                        if (_attachedProp != null)
                        {
                            _attachedProp.Delete();
                            return;
                        }
                        DeleteNearestProp();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Road management", ex);
            }
        }

        private void DeleteNearestProp()
        {
            Prop closestProp = null;
            float closestDistance = float.MaxValue;

            Vector3 playerPosition = Game.Player.Character.Position;

            foreach (var prop in World.GetAllProps())
            {
                if (!API.DecorExistOn(prop.Handle, ROADDECOR))
                {
                    continue;
                }

                float distance = Vector3.Distance(playerPosition, prop.Position);

                if (distance > 2f)
                {
                    continue;
                }

                _logger.Debug($"Close road prop {prop.Handle} {prop.NetworkId} {distance}");

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestProp = prop;
                }
            }

            // If a closest prop is found, delete it
            if (closestProp != null)
            {
                _logger.Debug($"{closestProp.Handle} {closestProp.NetworkId} {closestDistance}");
                closestProp.Delete();
                _comms.ToServer(ServerEvents.RoadsDeleteProp, closestProp.NetworkId);
                return;
            }
            _logger.Debug("No prop");
        }

        private async Task CreateLocalProp(string objectName)
        {
            var model = new Model(objectName);
            if (!await model.Request(1000))
            {
                _logger.Error($"Unable to request Object: {objectName}");
                return;
            }

            var playerPos = Game.PlayerPed.Position;
            var propHandle = API.CreateObjectNoOffset((uint)model.Hash, playerPos.X, playerPos.Y, playerPos.Z, false,
                false, false);
            _attachedProp = new Prop(propHandle);
            
            API.AttachEntityToEntity(_attachedProp.Handle, Game.PlayerPed.Handle, 11816, 0f, 2f, -0.6f,
                Game.PlayerPed.Rotation.X, Game.PlayerPed.Rotation.Y, Game.PlayerPed.Rotation.Z, false, false, false,
                false, 2, false);
            while (!_attachedProp.IsAttachedTo(Game.PlayerPed))
            {
                await Script.Delay(1);
            }
        }

        private async Task CreateServerProp()
        {
            _logger.Debug("Create Server Prop");
            var speed = -1;

            var propSpeedCategory = PropCategories.SelectMany(propCat => propCat.Props).FirstOrDefault(prop => API.GetHashKey(prop.PropFile) == _attachedProp.Model.Hash);

            if (propSpeedCategory != null)
            {
                speed = propSpeedCategory.propSpeedRestriction;
            }

            _attachedProp.Detach();
            API.PlaceObjectOnGroundProperly(_attachedProp.Handle);

            await Script.Delay(100);

            var roadManagementItem = new RoadManagementItem
            {
                ModelHash = _attachedProp.Model.Hash,
                Position = _attachedProp.Position.ToPmpVector3(),
                Rotation = _attachedProp.Rotation.ToPmpVector3(),
                Speed = speed
            };

            _attachedProp.Delete();
            _attachedProp = null;

            await Script.Delay(100);

            var prop = API.CreateObjectNoOffset((uint)roadManagementItem.ModelHash, roadManagementItem.Position.X, roadManagementItem.Position.Y, roadManagementItem.Position.Z, true,
                true, false);

            API.SetEntityRotation(prop, roadManagementItem.Rotation.X, roadManagementItem.Rotation.Y, roadManagementItem.Rotation.Z, 1, true);
            API.FreezeEntityPosition(prop, true);
            API.SetEntityCollision(prop, true, false);

            API.DecorSetBool(prop, ROADDECOR, true);

            AddNavmeshBlock(prop);



            //_comms.ToServer(ServerEvents.CreateRoadPropOnServer, roadManagementItem);
        }

        private void AddNavmeshBlock(int prop)
        {
            if (!API.DoesEntityExist(prop))
            {
                _logger.Debug($"Tried to add a block for no such entity {prop}");
                return;
            }

            if (_navmeshBlocks.ContainsKey(prop))
            {
                _logger.Debug("Tried to create blocker for prop already navmesh blocking.");
                return;
            }

            var entity = Entity.FromHandle(prop);
            float distance = Vector3.Distance(Game.PlayerPed.Position, entity.Position);
            if (distance >= 160f)
            {
                return;
            }


            // Get the dimensions of the prop
            Vector3 min = Vector3.Zero, max = Vector3.Zero;
            API.GetModelDimensions((uint)API.GetEntityModel(prop), ref min, ref max);

            Vector3 propPosition = API.GetEntityCoords(prop, true);
            Vector3 size = max - min;
            Vector3 center = (min + max) / 2.0f;

            // Add a 1f buffer around the size dimensions
            Vector3 bufferedSize = size + new Vector3(5f, 5f, 5f);

            // Calculate the position of the navmesh blocking object
            Vector3 blockerPosition = propPosition + center;

            // Create the navmesh blocking object with the buffered size
            int blocker = API.AddNavmeshBlockingObject(
                blockerPosition.X,
                blockerPosition.Y,
                blockerPosition.Z,
                bufferedSize.X,
                bufferedSize.Y,
                bufferedSize.Z,
                API.GetEntityHeading(prop),
                false, // No need to set dynamic if the object is static
                7
            );

            _logger.Debug($"Attempting to add block for: {prop}");
            if (blocker != -1)
            {
                _logger.Debug($"Navmesh blocker created successfully with handle: {blocker}");
                lock(_navmeshBlocks)
                {
                    _navmeshBlocks.Add(prop, blocker);
                }
                
            }
            else
            {
                _logger.Debug("Failed to create navmesh blocker.");
            }
        }

        private Task<PmpVector3> FetchNearestRoadNodePosition(PmpVector3 position)
        {
            var nodePosition = new CitizenFX.Core.Vector3();
            API.GetClosestVehicleNode(position.X, position.Y, position.Z, ref nodePosition, 1, 0f, 0f);
            _logger.Debug($"Found Position at {nodePosition}");
            return Task.FromResult(nodePosition.ToPmpVector3());
        }

        private void OnUpdateNodeAtPosition(PmpVector3 position, bool nodeEnabled)
        {
            _logger.Debug($"Updating Position: {position} node to: {nodeEnabled}");
            if (nodeEnabled)
            {
                API.SetRoadsBackToOriginal(position.X + _radius, position.Y + _radius, position.Z + _radius,
                    position.X - _radius, position.Y - _radius, position.Z - _radius);
            }
            else
            {
                API.SetRoadsInArea(position.X + _radius, position.Y + _radius, position.Z + _radius, position.X - _radius,
                    position.Y - _radius, position.Z - _radius, nodeEnabled, false);
                API.SetIgnoreSecondaryRouteNodes(true);
            }
        }

        private void OnSetSpeedZoneInArea(PmpVector3 position, int speed)
        {
            _logger.Debug($"Updating Speed in Area: {position} to {speed}");

            foreach (var ped in World.GetAllPeds())
            {
                if (ped.IsHuman) { continue; }
                if (ped.IsPlayer) { continue; }

                if (ped.State.Get($"{ped.NetworkId}") == "invisible")
                {
                    //Must be a rat
                    ped.IsVisible = false;
                }
            }

            if (speed == -1)
            {
                _logger.Debug("Removing Speed Zone");
                var hasSpeedZone = _speedZones.TryGetValue(position, out int oldSpeedZone);
                _logger.Debug($"Found Speed Zone? {hasSpeedZone}");
                if (!hasSpeedZone) return;
                _logger.Debug($"Speed Zone ID: {oldSpeedZone}");
                API.RemoveRoadNodeSpeedZone(oldSpeedZone);
                return;
            }
            var speedZone = API.AddRoadNodeSpeedZone(position.X, position.Y, position.Z, _radius, speed, true);
            if (speedZone == -1 || speedZone == null)
            {
                _logger.Error("could not add speed zone");
                return;
            }
            _speedZones.Add(position, speedZone);
        }
    }
}
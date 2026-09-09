using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using CitizenFX.Core.UI;
using PoliceMP.Client.Actions.HandsUp;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Scripts.HideBlips;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Client.Overlays.NewNotification;
using MenuAPI;
using System.Xml.Linq;
using PoliceMP.Core.Mediator;
using System.Security.Policy;
using System.Linq;

namespace PoliceMP.Client.Scripts.MedicalEquipment
{
    public class MedicalEquipment : Script
    {
        private readonly ICommandManager _commandManager;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<MedicalEquipment> _logger;

        public MedicalEquipment(ICommandManager commandManager, ILogger<MedicalEquipment> logger,
            INewNotificationOverlay newNotificationOverlay, IPermissionService permissionService, ITickManager ticks,
            IPlayerService playerService, ILegacyClientCommunicationsManager comms, ICommonFunctionsService common, IFeatureService featureService)
        {
            _commandManager = commandManager;
            _newNotificationOverlay = newNotificationOverlay;
            _permissionService = permissionService;
            _ticks = ticks;
            _commandManager = commandManager;
            _logger = logger;
            _permissionService = permissionService;
        }

        private bool belsKitWearing = false;
        private int belsComponentID = 2;

        private int medicalPropRight;
        private int medicalPropLeft;
        private int medicalPropBack;
        private int placedPropRight;
        private int placedPropLeft;
        private int placedPropBack;

        private int bone;
        private float xPos = + 0.3f;
        private float yPos;
        private float zPos;
        private float xRot;
        private float yRot = 270f;
        private float zRot;
        private float bagxPos = 0f - 0.3f;
        private float bagyPos = 0f - 0.2f;
        private float bagzPos;
        private float bagxRot;
        private float bagyRot = 450;
        private float bagzRot;

        private uint clearHash = 0;
        private uint belsHash = (uint)API.GetHashKey("prop_bls");
        private uint alsHash = (uint)API.GetHashKey("prop_alsbag");
        private uint alsOpenHash = (uint)API.GetHashKey("prop_alsbagopen");
        private uint appBagHash = (uint)API.GetHashKey("prop_appbag");
        private uint appBagOpenHash = (uint)API.GetHashKey("prop_appbagopen");
        private uint entonoxHash = (uint)API.GetHashKey("prop_entonox");
        private uint hemsBagBlackHash = (uint)API.GetHashKey("prop_hemsbagblack");
        private uint hemsBagOrangeHash = (uint)API.GetHashKey("prop_hemsbagorange");
        private uint lucasHash = (uint)API.GetHashKey("prop_lucas");
        private uint o2cylinderHash = (uint)API.GetHashKey("prop_o2cylinder");
        private uint primaryBagHash = (uint)API.GetHashKey("prop_primarybag");
        private uint primaryBagOpenHash = (uint)API.GetHashKey("prop_primarybagopen");
        private uint zollventHash = (uint)API.GetHashKey("prop_zollvent");
        private uint cprDummyHash = (uint)API.GetHashKey("resuscitation_doll");
        private uint hsRedDefibHash = (uint)API.GetHashKey("prop_idol_case_01");
        

        private List<uint> attachedPropsRight = new List<uint>();
        private List<uint> attachedPropsLeft = new List<uint>();
        private List<uint> attachedPropsBack = new List<uint>();

        private List<uint> placedPropsRight = new List<uint>();
        private List<uint> placedPropsLeft = new List<uint>();
        private List<uint> placedPropsBack = new List<uint>();

        private uint publicLeftHash;
        private uint publicRightHash;
        private uint publicBackHash;

        protected override async Task OnStartAsync()
        {
            var userAces = await _permissionService.GetUserAces();

            _commandManager.Register("dropmedicalkit").WithHandler(DropMedicalKit);
            API.RegisterKeyMapping("dropmedicalkit", "Place Medical Kit", "KEYBOARD", "e");
            API.RegisterKeyMapping("collectmedicalkit", "Pick Up Medical Kit", "KEYBOARD", "e");
            _commandManager.Register("medqmb").WithHandler(async () =>
            {

                MenuController.CloseAllMenus();

                var medicalEquipmentMenu = new Menu("Medical Equipment", "Collect your medical equipment");
                MenuController.EnableMenuToggleKeyOnController = false;
                MenuController.MenuToggleKey = (Control)(-1);
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                MenuController.AddMenu(medicalEquipmentMenu);

                await Delay(0);
                medicalEquipmentMenu.OpenMenu();

                //if (currentUserRole.Division == UserDivision.Afo
                //if (currentUserRole.Branch == UserBranch.Nhs)
                var currentUserRole = _permissionService.CurrentUserRole;

                var clear = new MenuItem("Return Equipment", "Return everything");
                medicalEquipmentMenu.AddMenuItem(clear);

                var bels = new MenuItem("BELS Bag", "Collect or return your BELS Bag");
                var als = new MenuItem("ALS Bag", "Collect or return your ALS Bag");
                var appBag = new MenuItem("APP Bag", "Collect or return your APP Bag");
                var entonox = new MenuItem("Entonox", "Collect or return Entonox");
                var hemsBagBlack = new MenuItem("HEMS Bag Black", "Collect or return HEMS Bag Black");
                var hemsBagOrange = new MenuItem("HEMS Bag Orange", "Collect or return HEMS Bag Orange");
                var lucas = new MenuItem("Avery", "Collect or return Avery");
                var o2Cylinder = new MenuItem("O2 Cylinder", "Collect or return O2 Cylinder");
                var primaryBag = new MenuItem("Primary Bag", "Collect or return Primary Bag");
                var zollVent = new MenuItem("Zoll Vent", "Collect or return Zoll Vent");
                var cprDummy = new MenuItem("Training Dummy", "Collect or return Training Dummy");
                var hsRedDefib = new MenuItem("MRX Patient Monitor", "Collect or return MRX Patient Monitor");


                if (currentUserRole.Division == UserDivision.Afo)
                {
                    medicalEquipmentMenu.AddMenuItem(bels);
                }
                if (currentUserRole.Branch == UserBranch.Nhs)
                {
                    medicalEquipmentMenu.AddMenuItem(als);
                    medicalEquipmentMenu.AddMenuItem(appBag);
                    medicalEquipmentMenu.AddMenuItem(entonox);
                    medicalEquipmentMenu.AddMenuItem(o2Cylinder);
                    medicalEquipmentMenu.AddMenuItem(primaryBag);
                    medicalEquipmentMenu.AddMenuItem(zollVent);
                    medicalEquipmentMenu.AddMenuItem(hsRedDefib);
                }
                if (currentUserRole.Division == UserDivision.Hems || currentUserRole.Division == UserDivision.BeepDoctor && userAces.IsNhsHems) 
                {
                    medicalEquipmentMenu.AddMenuItem(hemsBagBlack);
                    medicalEquipmentMenu.AddMenuItem(hemsBagOrange);
                    medicalEquipmentMenu.AddMenuItem(lucas);
                }
                if (currentUserRole.Division == UserDivision.HemsDoctor || currentUserRole.Division == UserDivision.BeepDoctor && userAces.IsNhsDoctor)
                {
                    medicalEquipmentMenu.AddMenuItem(hemsBagBlack);
                    medicalEquipmentMenu.AddMenuItem(hemsBagOrange);
                    medicalEquipmentMenu.AddMenuItem(lucas);
                }    
                if (userAces.IsNhsClinicalTl || userAces.IsNhsHemsTl)
                {
                    medicalEquipmentMenu.AddMenuItem(cprDummy);
                }
                medicalEquipmentMenu.OpenMenu();

                medicalEquipmentMenu.OnItemSelect += (menu, item, index) =>
                {
                    if (menu != medicalEquipmentMenu) return;

                    if (item == clear) MedicProp("null", clearHash, false);
                    if (item == bels) MedicProp("BELS Bag", belsHash, true);
                    if (item == als) MedicProp("ALS Bag", alsHash, false);
                    if (item == appBag) MedicProp("APP Bag", appBagHash, true);
                    if (item == entonox) MedicProp("Entonox", entonoxHash, false);
                    if (item == hemsBagBlack) MedicProp("HEMS Bag", hemsBagBlackHash, true);
                    if (item == hemsBagOrange) MedicProp("HEMS Bag", hemsBagOrangeHash, true);
                    if (item == lucas) MedicProp("Avery Device", lucasHash, false);
                    if (item == o2Cylinder) MedicProp("O2 Cylinder", o2cylinderHash, false);
                    if (item == primaryBag) MedicProp("Primary Bag", primaryBagHash, true);
                    if (item == zollVent) MedicProp("Zoll Vent", zollventHash, false);
                    if (item == cprDummy) MedicProp("Training Dummy", cprDummyHash, false);
                    if (item == hsRedDefib) MedicProp("MRX Patient Monitor", hsRedDefibHash, false);

                    medicalEquipmentMenu.CloseMenu();
                };
            });
        }


        private async void MedicProp(string name, uint hash, bool isBagProp)
        {
            var player = Game.PlayerPed.Handle;
            var playerPos = API.GetEntityCoords(player, true);
            var bone = (int)Bone.SKEL_R_Hand;

            if (isBagProp)
            {
                bone = (int)Bone.SKEL_Spine3;
                bagyPos = 0f - 0.2f;
                bagxPos = 0 - 0.3f;

                API.RequestAnimDict("clothingtie");
                API.TaskPlayAnim(player, "clothingtie", "try_tie_negative_a", 8.0f, 1.0f, -1, 2, 0, false, false, false);
                await Delay(1000);
                API.ClearPedTasksImmediately(player);
            }

            if (hash == hsRedDefibHash)
            {
                yPos = -0.3f;
            }    

            if (hash == lucasHash)
            {
                xPos = 0 + 0.6f;
            }
            if (hash == entonoxHash || hash == o2cylinderHash)
            {
                xPos = 0 + 0.625f;
            }
            else
            {
                xPos = +0.3f;
            }

            if (!isBagProp)
            {
                API.ExecuteCommand("e mechanic");
                await Delay(1500);
                API.ExecuteCommand("e c");
            }

            if (hash == clearHash)
            {
                API.DeleteObject(ref medicalPropRight);
                API.DeleteObject(ref medicalPropLeft);
                API.DeleteObject(ref medicalPropBack);
                API.DeleteObject(ref placedPropRight);
                API.DeleteObject(ref placedPropLeft);
                API.DeleteObject(ref placedPropBack);
                attachedPropsLeft.Clear();
                attachedPropsRight.Clear();
                attachedPropsBack.Clear();
                placedPropsRight.Clear();
                placedPropsLeft.Clear();
                placedPropsBack.Clear();
                API.SetPedComponentVariation(player, 10, 0, 0, 0);

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", $"You have returned your equipment!", new NewNotificationMessageContent[0]));
                return;
            }

            var wearingBels = false;
            if (API.GetPedDrawableVariation(player, 10) == belsComponentID) wearingBels = true;

            if (attachedPropsRight.Contains(hash) || attachedPropsLeft.Contains(hash) || attachedPropsBack.Contains(hash) || wearingBels)
            {
                if (attachedPropsRight.Contains(hash))
                {
                    API.DeleteObject(ref medicalPropRight);
                    attachedPropsRight.Remove(hash);
                }
                else if (attachedPropsLeft.Contains(hash))
                {
                    API.DeleteObject(ref medicalPropLeft);
                    attachedPropsLeft.Remove(hash);
                }
                else if (attachedPropsBack.Contains(hash))
                {
                    API.DeleteObject(ref medicalPropBack);
                    attachedPropsBack.Remove(hash);
                }
                else if (wearingBels)
                {
                    API.SetPedComponentVariation(player, 10, 0, 0, 0);
                    attachedPropsBack.Remove(hash);
                }
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", $"You have returned your {name}!", new NewNotificationMessageContent[0]));
                return;
            }

            API.RequestModel(hash);

            while (!API.HasModelLoaded(hash))
            {
                await Delay(10);
            }

            if (attachedPropsRight.Count == 0 && !isBagProp)
            {
                API.DeleteObject(ref medicalPropRight);
                medicalPropRight = API.CreateObject((int)hash, playerPos.X, playerPos.Y, playerPos.Z, true, true, false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", $"You have collected your {name}!", new NewNotificationMessageContent[0]));
                API.AttachEntityToEntity(medicalPropRight, player, API.GetPedBoneIndex(player, bone), xPos, yPos, zPos, xRot, yRot, zRot, false, false, false, false, 2, true);
                attachedPropsRight.Add(hash);
                publicRightHash = hash;
                return;
            }

            if (attachedPropsRight.Count != 0 && attachedPropsLeft.Count == 0 && !isBagProp)
            {
                bone = (int)Bone.SKEL_L_Hand;
                API.DeleteObject(ref medicalPropLeft);
                medicalPropLeft = API.CreateObject((int)hash, playerPos.X, playerPos.Y, playerPos.Z, true, true, false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", $"You have collected your {name}!", new NewNotificationMessageContent[0]));
                API.AttachEntityToEntity(medicalPropLeft, player, API.GetPedBoneIndex(player, bone), xPos, yPos, zPos, xRot, yRot, zRot, false, false, false, false, 2, true);
                attachedPropsLeft.Add(hash);
                publicLeftHash = hash;
                return;
            }

            if (isBagProp)
            {
                API.DeleteObject(ref medicalPropBack);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", $"You have collected your {name}!", new NewNotificationMessageContent[0]));
                attachedPropsBack.Add(hash);
                publicBackHash = hash;

                if (hash == belsHash)
                {
                    API.SetPedComponentVariation(player, 10, belsComponentID, 0, 0);
                    return;
                }

                medicalPropBack = API.CreateObject((int)hash, playerPos.X, playerPos.Y, playerPos.Z, true, true, false);
                API.AttachEntityToEntity(medicalPropBack, player, API.GetPedBoneIndex(player, bone), bagxPos, bagyPos, bagzPos, bagxRot, bagyRot, bagzRot, false, false, false, false, 2, true);

                return;
            }

            if (attachedPropsRight.Count == 1 && attachedPropsLeft.Count == 1)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "error", $"Your hands are full! Clear them to collect your {name}", new NewNotificationMessageContent[0]));
                return;
            }

            return;
        }


        private async void DropMedicalKit()
        {
            var player = Game.PlayerPed.Handle;
            if (API.IsPedInAnyVehicle(player, true)) return;
            var playerPos = API.GetEntityCoords(player, true);
            var playerHeading = Game.PlayerPed.Heading;
            API.RequestAnimDict("anim@mp_snowball");
            var hash = (uint)0;

            var rightPos = API.GetEntityCoords(placedPropRight, true);
            var leftPos = API.GetEntityCoords(placedPropLeft, true);
            var backPos = API.GetEntityCoords(placedPropBack, true);

            var distanceFromRight = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, rightPos.X, rightPos.Y, rightPos.Z, false);
            var distanceFromLeft = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, leftPos.X, leftPos.Y, leftPos.Z, false);
            var distanceFromBack = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, backPos.X, backPos.Y, backPos.Z, false);

            var smallestDistance = Math.Min(distanceFromRight, Math.Min(distanceFromLeft, distanceFromBack));

            if (smallestDistance < 1f)
            {
                PickupMedicalKit();
                return;
            }

            if (attachedPropsRight.Count != 0)
            {
                hash = publicRightHash;
                API.TaskPlayAnim(player, "anim@mp_snowball", "pickup_snowball", 8.0f, 1.0f, -1, 2, 0, false, false, false);
                await Delay(1200);
                API.ClearPedTasksImmediately(player);

                while (!API.HasModelLoaded(hash))
                {
                    await Delay(100);
                }

                var openHash = hash;
                placedPropsRight.Add(hash);
                if (hash == alsHash) openHash = alsOpenHash;
                if (hash == appBagHash) openHash = appBagOpenHash;
                if (hash == primaryBagHash) openHash = primaryBagOpenHash;

                API.DeleteObject(ref placedPropRight);
                placedPropRight = API.CreateObject((int)openHash, playerPos.X, playerPos.Y - 0.55f, playerPos.Z, true, true, false);
                API.SetEntityRotation(placedPropRight, 0, 0, playerHeading, 1, true);
                API.DeleteObject(ref medicalPropRight);
                attachedPropsRight.Remove(hash);
                API.PlaceObjectOnGroundProperly(placedPropRight);
                
                if (hash == lucasHash)
                {
                    var raisedZ = 0.3f;
                    var entityCoords = API.GetEntityCoords(placedPropRight, true);
                    API.SetEntityCoordsNoOffset(placedPropRight, entityCoords.X, entityCoords.Y, entityCoords.Z + raisedZ, true, true, true);
                }
                if (hash == entonoxHash | hash == o2cylinderHash)
                {
                    var raisedZ = 0.25f;
                    var entityCoords = API.GetEntityCoords(placedPropRight, true);
                    API.SetEntityCoordsNoOffset(placedPropRight, entityCoords.X, entityCoords.Y, entityCoords.Z + raisedZ, true, true, true);
                }

                API.FreezeEntityPosition(placedPropRight, true);
                API.SetEntityCollision(placedPropRight, true, true);
                placedPropsRight.Add(hash);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", "You have placed your Medical Equipment!", new NewNotificationMessageContent[0]));

            }

            if (attachedPropsLeft.Count != 0)
            {
                hash = publicLeftHash;
                API.TaskPlayAnim(player, "anim@mp_snowball", "pickup_snowball", 8.0f, 1.0f, -1, 2, 0, false, false, false);
                await Delay(1200);
                API.ClearPedTasksImmediately(player);

                while (!API.HasModelLoaded(hash))
                {
                    await Delay(100);
                }

                var openHash = hash;
                placedPropsLeft.Add(hash);
                if (hash == alsHash) openHash = alsOpenHash;
                if (hash == appBagHash) openHash = appBagOpenHash;
                if (hash == primaryBagHash) openHash = primaryBagOpenHash;

                API.DeleteObject(ref placedPropLeft);
                placedPropLeft = API.CreateObject((int)openHash, playerPos.X, playerPos.Y - 0.55f, playerPos.Z, true, true, false);
                API.SetEntityRotation(placedPropLeft, 0, 0, playerHeading, 1, true);
                API.DeleteObject(ref medicalPropLeft);
                attachedPropsLeft.Remove(hash);
                API.PlaceObjectOnGroundProperly(placedPropLeft);

                if (hash == lucasHash)
                {
                    var raisedZ = 0.3f;
                    var entityCoords = API.GetEntityCoords(placedPropLeft, true);
                    API.SetEntityCoordsNoOffset(placedPropLeft, entityCoords.X, entityCoords.Y, entityCoords.Z + raisedZ, true, true, true);
                }
                if (hash == entonoxHash | hash == o2cylinderHash)
                {
                    var raisedZ = 0.25f;
                    var entityCoords = API.GetEntityCoords(placedPropRight, true);
                    API.SetEntityCoordsNoOffset(placedPropRight, entityCoords.X, entityCoords.Y, entityCoords.Z + raisedZ, true, true, true);
                }

                API.FreezeEntityPosition(placedPropLeft, true);
                API.SetEntityCollision(placedPropLeft, true, true);
                placedPropsLeft.Add(hash);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", "You have placed your Medical Equipment!", new NewNotificationMessageContent[0]));
                return;
            }

            if (attachedPropsBack.Count != 0)
            {
                hash = publicBackHash;
                API.TaskPlayAnim(player, "anim@mp_snowball", "pickup_snowball", 8.0f, 1.0f, -1, 2, 0, false, false, false);
                await Delay(1200);
                API.ClearPedTasksImmediately(player);

                while (!API.HasModelLoaded(hash))
                {
                    await Delay(100);
                }

                var openHash = hash;
                placedPropsBack.Add(hash);
                if (hash == alsHash) openHash = alsOpenHash;
                if (hash == appBagHash) openHash = appBagOpenHash;
                if (hash == primaryBagHash) openHash = primaryBagOpenHash;

                API.DeleteObject(ref placedPropBack);
                placedPropBack = API.CreateObject((int)openHash, playerPos.X, playerPos.Y - 0.55f, playerPos.Z, true, true, false);
                API.SetEntityRotation(placedPropBack, 0, 0, playerHeading, 1, true);

                API.DeleteObject(ref medicalPropBack);
                attachedPropsBack.Remove(hash);
                API.PlaceObjectOnGroundProperly(placedPropBack);
                API.FreezeEntityPosition(placedPropBack, true);
                API.SetEntityCollision(placedPropBack, true, true);
                placedPropsBack.Add(hash);

                if (hash == belsHash)
                {
                    API.SetPedComponentVariation(player, 10, 0, 0, 0);
                }

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", "You have placed your Medical Equipment!", new NewNotificationMessageContent[0]));
                return;
            }
        }

        private async void PickupMedicalKit()
        {
            var player = Game.PlayerPed.Handle;
            var playerPos = API.GetEntityCoords(player, true);
            API.RequestAnimDict("anim@mp_snowball");
            var hash = (uint)0;

            if ((placedPropsBack.Count != 0) || (placedPropsLeft.Count != 0) || (placedPropsRight.Count != 0))
            {
                var rightPos = API.GetEntityCoords(placedPropRight, true);
                var leftPos = API.GetEntityCoords(placedPropLeft, true);
                var backPos = API.GetEntityCoords(placedPropBack, true);

                var distanceFromRight = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, rightPos.X, rightPos.Y, rightPos.Z, false);
                var distanceFromLeft = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, leftPos.X, leftPos.Y, leftPos.Z, false);
                var distanceFromBack = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, backPos.X, backPos.Y, backPos.Z, false);

                var smallestDistance = Math.Min(distanceFromRight, Math.Min(distanceFromLeft, distanceFromBack));

                if (smallestDistance > 1f)
                {
                    if (placedPropLeft == null && placedPropRight == null && placedPropBack == null) return;
                    return;
                }

                if (smallestDistance == distanceFromRight)
                {
                    bone = (int)Bone.SKEL_R_Hand;

                    API.TaskPlayAnim(player, "anim@mp_snowball", "pickup_snowball", 8.0f, 1.0f, -1, 2, 0, false, false, false);
                    await Delay(1200);
                    API.ClearPedTasksImmediately(player);

                    API.DeleteObject(ref placedPropRight);
                    placedPropsRight.Remove(hash);
                    hash = publicRightHash;
                    medicalPropRight = API.CreateObject((int)hash, playerPos.X, playerPos.Y, playerPos.Z, true, true, false);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", $"You have picked up your equipment!", new NewNotificationMessageContent[0]));
                    API.AttachEntityToEntity(medicalPropRight, player, API.GetPedBoneIndex(player, bone), xPos, yPos, zPos, xRot, yRot, zRot, false, false, false, false, 2, true);
                    attachedPropsRight.Add(hash);
                    publicRightHash = hash;
                    return;
                }

                if (smallestDistance == distanceFromLeft)
                {
                    bone = (int)Bone.SKEL_L_Hand;

                    API.TaskPlayAnim(player, "anim@mp_snowball", "pickup_snowball", 8.0f, 1.0f, -1, 2, 0, false, false, false);
                    await Delay(1200);
                    API.ClearPedTasksImmediately(player);

                    API.DeleteObject(ref placedPropLeft);
                    hash = publicLeftHash;
                    placedPropsLeft.Remove(hash);
                    medicalPropLeft = API.CreateObject((int)hash, playerPos.X, playerPos.Y, playerPos.Z, true, true, false);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", $"You have picked up your equipment!", new NewNotificationMessageContent[0]));
                    API.AttachEntityToEntity(medicalPropLeft, player, API.GetPedBoneIndex(player, bone), xPos, yPos, zPos, xRot, yRot, zRot, false, false, false, false, 2, true);
                    attachedPropsLeft.Add(hash);
                    publicLeftHash = hash;
                    return;
                }

                if (smallestDistance == distanceFromBack)
                {
                    bone = (int)Bone.SKEL_Spine3;

                    API.TaskPlayAnim(player, "anim@mp_snowball", "pickup_snowball", 8.0f, 1.0f, -1, 2, 0, false, false, false);
                    await Delay(1200);
                    API.ClearPedTasksImmediately(player);

                    API.DeleteObject(ref placedPropBack);
                    hash = publicBackHash;
                    placedPropsBack.Remove(hash);

                    if (hash == belsHash)
                    {
                        API.SetPedComponentVariation(player, 10, belsComponentID, 0, 0);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", $"You have picked up your equipment!", new NewNotificationMessageContent[0]));
                        return;
                    }

                    medicalPropBack = API.CreateObject((int)hash, playerPos.X, playerPos.Y, playerPos.Z, true, true, false);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Medical Equipment", "success", $"You have picked up your equipment!", new NewNotificationMessageContent[0]));
                    API.AttachEntityToEntity(medicalPropBack, player, API.GetPedBoneIndex(player, bone), bagxPos, bagyPos, bagzPos, bagxRot, bagyRot, bagzRot, false, false, false, false, 2, true);
                    attachedPropsBack.Add(hash);
                    publicBackHash = hash;
                    return;
                }
            }
        }
    }
}
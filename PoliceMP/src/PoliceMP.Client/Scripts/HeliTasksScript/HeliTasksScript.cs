using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;
using System.Runtime.CompilerServices;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using MenuAPI;
using System.Security.Policy;

namespace PoliceMP.Client.Scripts.HeliTasksScript
{
    public class HeliTasksScript : Script
    {
        private readonly ITickManager _ticks;
        private readonly ILogger<HeliTasksScript> _logger;
        private INotificationService _notification;
        private ICommandManager _commandManager;
        private readonly IPermissionService _permissionService;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ISoundService _soundService;

        #region Variables

        private int airTug;
        private int gpuProp;

        #endregion Variables

        public HeliTasksScript(ITickManager tick, ICommandManager command, ISoundService soundService, IPermissionService permission, ICommandManager commandManager, INewNotificationOverlay newNotificationOverlay, INotificationService notification, ILogger<HeliTasksScript> logger)
        {
            _notification = notification;
            _logger = logger;
            _ticks = tick;
            _newNotificationOverlay = newNotificationOverlay;
            _commandManager = commandManager;
            _permissionService = permission;
            _soundService = soundService;
        }

        #region Ticks and Commands
        protected override Task OnStartAsync()
        {
            _commandManager.Register("hetsgpu").WithHandler(OnSpawnGPU);

            return Task.FromResult(0);
        }
        #endregion Ticks and Commands

        private async void OnSpawnGPU()
        {
            var airtugHash = (uint)API.GetHashKey("airtug");

            API.RequestModel(airtugHash);
            while (!API.HasModelLoaded(airtugHash))
            {
                await Delay(100);
            }

            var airTugSpawn = new Vector3(-1650.53454f, -3185.893f, 13.42077f);
            var aitTugHeading = 57.2f;
            airTug = API.CreateVehicle(airtugHash, airTugSpawn.X, airTugSpawn.Y, airTugSpawn.Z, aitTugHeading, true, false);

            var gpuHash = API.GetHashKey("prop_air_generator_01");
            var gpuSpawn = new Vector3(-1647.09948f, -3188.00610f, 13.99096f);
            var gpuHeading = 57.2f;
            gpuProp = API.CreateObject(gpuHash, gpuSpawn.X, gpuSpawn.Y, gpuSpawn.Z, true, false, false);

            var bone = API.GetEntityBoneIndexByName(airTug, "bonnet");
            API.AttachEntityToEntity(airTug, gpuProp, bone, 0, 0, 0, 0, 0, 0, false, false, false, false, 2, true);

            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter Maintenance", "success", "Use the tug vehicle to bring the Ground Power Unit over to the aircraft!", new NewNotificationMessageContent[0]));

        }
    }
}
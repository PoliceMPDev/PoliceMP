using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Behaviors.AiCallouts;
using static System.Net.Mime.MediaTypeNames;

namespace PoliceMP.Client.Actions.BootEnterHandler
{
    public class BootEnterHandler : ActionHandler<BootEnter>
    {
        private readonly ITickManager _tickManager;
        private readonly ILogger<BootEnterHandler> _logger;
        private readonly IGameInputManager _gameInputManager;
        public BootEnterHandler(ITickManager tickManager, ILogger<BootEnterHandler> logger, IGameInputManager gameInputManager)
        {
            _tickManager = tickManager;
            _logger = logger;
            _gameInputManager = gameInputManager;
        }

        private int actionVehicleHandle;

        protected override async Task<bool> Handle(BootEnter action)
        {
            actionVehicleHandle = action.Target.Handle;

            var pedBone = API.GetEntityBoneIndexByName(Game.PlayerPed.Handle, "SKEL_ROOT");
            var vehicleBone = API.GetEntityBoneIndexByName(Game.PlayerPed.Handle, "boot");

            API.AttachEntityBoneToEntityBone(Game.PlayerPed.Handle, action.Target.Handle, pedBone, vehicleBone, true, true);

            var boneAttachIndex = API.GetEntityBoneIndexByName(action.Target.Handle, "boot");
            var bone = action.Target.Bones[boneAttachIndex];

            Game.PlayerPed.Task.PlayAnimation("savef_default@", "f_sleep_r_loop");

            var bonePos = API.GetEntityBonePosition_2(action.Target.Handle, boneAttachIndex);
            var offset = API.GetOffsetFromEntityGivenWorldCoords(action.Target.Handle, bonePos.X, bonePos.Y, bonePos.Z);

            offset.Y += 0.2f;
            offset.Z += -0.4f;

            Game.PlayerPed.AttachTo(action.Target, offset, new Vector3(0f, 0f, -90f));

            //Game.PlayerPed.AttachTo(action.Target.Bones[]);


            _tickManager.On(InBootTick);

            return true;
        }

        private Task InBootTick()
        {
            _logger.Debug("Jobbies");

            if (!Game.PlayerPed.IsAttached())
            {
                _tickManager.Off(InBootTick);
                return Task.FromResult(0);
            }

            var vehicle = Game.PlayerPed.GetEntityAttachedTo();

            if (vehicle == null)
            {
                _tickManager.Off(InBootTick);
                return Task.FromResult(0);
            }

            Game.PlayerPed.Task.PlayAnimation("savef_default@", "f_sleep_r_loop");
            Screen.ShowSubtitle("Press F to exit");

            if (_gameInputManager.IsJustPressed(Control.Enter))
            {             
                var exitPos = API.GetOffsetFromEntityInWorldCoords(vehicle.Handle, 0f, -4f, 0f);

                Game.PlayerPed.Detach();
                Game.PlayerPed.Task.ClearAllImmediately();

                Game.PlayerPed.Position = exitPos;

                API.SetEntityCollision(Game.PlayerPed.Handle, true, true);
                API.SetEntityCollision(actionVehicleHandle, true, true);
                //actionVehicleHandle = 0;
                _tickManager.Off(InBootTick);
            }

            return Task.FromResult(0);
        }
    }
}
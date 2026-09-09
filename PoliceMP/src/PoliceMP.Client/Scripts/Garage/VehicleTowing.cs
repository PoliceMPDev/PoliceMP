using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Scripts.Garage
{
    public class VehicleTowing : Script
    {
        private readonly ICommandManager _command;
        private readonly INotificationService _notification;

        private const string BoneName = "misc_attachPoint";
        private Vector3 _pointOffset = new Vector3(0f, -7f, 0f);
        
        public VehicleTowing(ICommandManager command, INotificationService notification)
        {
            _command = command;
            _notification = notification;
        }

        protected override async Task OnStartAsync()
        {
            _command.Register("vehicleOn").WithHandler(AttachVehicle);
            _command.Register("vehicleDeploy").WithHandler(DetachVehicle);
        }

        private async Task AttachVehicle()
        {
            if (!Game.PlayerPed.IsInVehicle() || !Game.PlayerPed.CurrentVehicle.Bones.HasBone(BoneName))
            {
                _notification.Error("Towing", "You must be in an appropriate vehicle to use this.");
                return;
            }
            
            if (Game.PlayerPed.CurrentVehicle.Speed != 0)
            {
                _notification.Error("Towing", "You must be stationary to use this");
                return;
            }

            var pickUpPos = API.GetOffsetFromEntityInWorldCoords(Game.PlayerPed.CurrentVehicle.Handle, _pointOffset.X,
                _pointOffset.Y, _pointOffset.Z);

            Vehicle vehicleToTow = null;
            foreach (var vehicle in World.GetAllVehicles())
            {
                var distance = API.GetDistanceBetweenCoords(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, pickUpPos.X,
                    pickUpPos.Y, pickUpPos.Z, true);

                if (distance >= 3f) continue;
                
                if(vehicle == Game.PlayerPed.CurrentVehicle) continue;
                
                vehicleToTow = vehicle;
            }

            if (vehicleToTow == null)
            {
                _notification.Error("Towing", "No vehicle found to tow.");
                return;
            }
            
            if (vehicleToTow.Driver.Exists() || vehicleToTow.Passengers.Length != 0)
            {
                _notification.Error("Towing", "Vehicle must be empty for you to do this.");
                return;
            }

            await vehicleToTow.TryRequestNetworkEntityControl();

            vehicleToTow.IsCollisionEnabled = false;
            
            var boneAttachIndex = API.GetEntityBoneIndexByName(Game.PlayerPed.CurrentVehicle.Handle, BoneName);
            var bone = Game.PlayerPed.CurrentVehicle.Bones[boneAttachIndex];
            vehicleToTow.AttachTo(bone);
        }
        
        private async Task DetachVehicle()
        {
            if (!Game.PlayerPed.IsInVehicle() || !Game.PlayerPed.CurrentVehicle.Bones.HasBone(BoneName))
            {
                _notification.Error("Towing", "You must be in an appropriate vehicle to use this.");
                return;
            }

            if (Game.PlayerPed.CurrentVehicle.Speed != 0)
            {
                _notification.Error("Towing", "You must be stationary to use this");
                return;
            }

            Vehicle attachedVehicle = null;
            foreach (var vehicle in World.GetAllVehicles())
            {
                if (!vehicle.IsAttachedTo(Game.PlayerPed.CurrentVehicle)) continue;

                attachedVehicle = vehicle;
            }

            if (attachedVehicle == null)
            {
                _notification.Error("Towing", "There is no vehicle in the bay.");
                return;
            }
            
            await attachedVehicle.TryRequestNetworkEntityControl();
            
            attachedVehicle.Detach();
            
            var deployPos = API.GetOffsetFromEntityInWorldCoords(Game.PlayerPed.CurrentVehicle.Handle, _pointOffset.X,
                _pointOffset.Y, _pointOffset.Z);

            attachedVehicle.Position = deployPos;
            attachedVehicle.IsCollisionEnabled = true;
        }
    }
}
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Abstraction;

namespace PoliceMP.Client.Actions.PutPedInCar
{
    public class PutPedInCarHandler : ActionHandler<PutPedInCar>
    {
        private readonly ILogger<PutPedInCarHandler> _logger;
        private readonly IActionManager _actions;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ISpeechService _speech;

        private readonly VehicleDoorIndex[] _validDoors = new[]
        {
            VehicleDoorIndex.BackLeftDoor,
            VehicleDoorIndex.BackRightDoor
        };

        // TODO: Try and find a native for this!
        private readonly Dictionary<VehicleDoorIndex, VehicleSeat> SeatMappings =
            new Dictionary<VehicleDoorIndex, VehicleSeat>()
            {
                { VehicleDoorIndex.BackLeftDoor, VehicleSeat.LeftRear },
                { VehicleDoorIndex.BackRightDoor, VehicleSeat.RightRear }
            };

        public PutPedInCarHandler(ILogger<PutPedInCarHandler> logger,
            IActionManager actions,
            ILegacyClientCommunicationsManager comms,
            ISpeechService speech)
        {
            _logger = logger;
            _actions = actions;
            _comms = comms;
            _speech = speech;
        }

        protected override async Task<bool> Handle(PutPedInCar action)
        {
            if (action.Vehicle.LockStatus != VehicleLockStatus.Unlocked)
                return false;

            if (action.Subject == Game.PlayerPed
                && action.Target.IsAttachedTo(action.Subject))
            {
                _logger.Debug("Un-grabbing ped..");
                await _actions.Execute(new Grab.Grab(action.Target, true), true);
            }

            var doorIndex = action.VehicleDoorIndex;
            if ((int)doorIndex != -1 && !SeatMappings.TryGetValue(doorIndex, out var seat))
            {
                _logger.Warn("Could not find vehicleSeat mapping for provided vehicleSeat!");
            }
            else if (!TryGetClosestDoorIndex(action, out doorIndex, out seat))
            {
                _logger.Warn("Could not find a suitable vehicleSeat for ped!");
            }

            _logger.Debug($"Best door is {doorIndex}. Seat is {seat}.");
            if (!IsVehicleSeatAccessible(action.Target, action.Vehicle, seat, true))
            {
                return false;
            }

            await SubjectOpenDoor(action, doorIndex, seat);
            await Delay(1000);

            if (!IsVehicleSeatAccessible(action.Target, action.Vehicle, seat, true))
            {
                return false;
            }

            await TargetGetInVehicle(action, doorIndex, seat);

            return true;
        }

        private async Task SubjectOpenDoor(PutPedInCar action, VehicleDoorIndex door, VehicleSeat seat)
        {
            var timeout = Game.GameTime + 10000.0f;

            while (API.GetIsTaskActive(Game.PlayerPed.Handle, (int)TaskTypeIndex.CTaskMoveGoToPoint)
                   && Game.GameTime < timeout)
                await Delay(0);

            // Doesn't work with task sequences. Fucker!
            API.TaskOpenVehicleDoor(action.Subject.Handle, action.Vehicle.Handle, 10000, (int)seat, 1.0f);

            while (!action.Vehicle.Doors[door].IsOpen
                   && Game.GameTime < timeout
                   && action.Vehicle.Speed < 10.0f)
                await Delay(0);

            if (action.Vehicle.Speed > 10.0f)
                return;

            await Delay(500);

            // Force fully open
            action.Vehicle.Doors[door].Open();

            _logger.Debug("Moving away from vehicle...");
            var sequence = new TaskSequence();

            var endPosition = API.GetEntryPositionOfDoor(action.Vehicle.Handle, (int)door) +
                              action.Vehicle.ForwardVector * -1f;
            sequence.AddTask.GoTo(endPosition);
            sequence.AddTask.LookAt(action.Target);
            sequence.AddTask.TurnTo(action.Target);
            sequence.Close();

            action.Subject.Task.PerformSequence(sequence);
        }

        private async Task TargetGetInVehicle(PutPedInCar action, VehicleDoorIndex door, VehicleSeat vehicleSeat)
        {
            action.Target.Task.ClearAll();
            var timeout = Game.GameTime + 10000.0f;
            _logger.Debug($"Target getting into vehicle vehicleSeat {vehicleSeat}...");

            var sequence = new TaskSequence();
            sequence.AddTask.GoTo(API.GetEntryPositionOfDoor(action.Vehicle.Handle, (int)door));
            sequence.AddTask.EnterVehicle(action.Vehicle, vehicleSeat);
            sequence.Close();
            action.Target.Task.PerformSequence(sequence);

            while (!action.Target.IsInVehicle(action.Vehicle)
                   && IsVehicleSeatAccessible(action.Target, action.Vehicle, vehicleSeat, true)
                   && Game.GameTime < timeout
                   && action.Vehicle.Speed < 10.0f)
                await Delay(0);

            if (action.Vehicle.Speed > 10.0f ||
                action.Target.CurrentVehicle != action.Vehicle
                || action.Target.SeatIndex != vehicleSeat)
                action.Target.Task.WarpIntoVehicle(action.Vehicle, vehicleSeat);

            action.Vehicle.Doors[door].Close();

            _speech.Do(action.Target, "Gets into vehicle.");

            _logger.Debug($"Target getting into vehicle vehicleSeat {vehicleSeat}... Done!");
        }

        private bool TryGetClosestDoorIndex(PutPedInCar action, out VehicleDoorIndex vehicleDoorIndex,
            out VehicleSeat seat)
        {
            var distance = float.MaxValue;
            vehicleDoorIndex = (VehicleDoorIndex)(-1);
            seat = VehicleSeat.None;

            for (int i = 0; i < _validDoors.Length; i++)
            {
                var doorIndex = _validDoors[i];

                if (!SeatMappings.TryGetValue(doorIndex, out var vehicleSeat))
                {
                    continue;
                }

                _logger.Debug(
                    $"Testing vehicle vehicleSeat {vehicleSeat}: {API.GetPedInVehicleSeat(action.Vehicle.Handle, (int)vehicleSeat)}; Door is {doorIndex}");
                if (action.Vehicle.Doors.HasDoor(doorIndex)
                    && API.GetIsDoorValid(action.Vehicle.Handle, (int)doorIndex)
                    && 0 == API.GetPedInVehicleSeat(action.Vehicle.Handle, (int)vehicleSeat)
                    && IsVehicleSeatAccessible(action.Target, action.Vehicle, vehicleSeat, true))
                {
                    var position = API.GetEntryPositionOfDoor(action.Vehicle.Handle, (int)doorIndex);
                    var doorDistance = World.GetDistance(action.Subject.Position, position);

                    if (doorDistance < distance)
                    {
                        distance = doorDistance;
                        seat = vehicleSeat;
                        vehicleDoorIndex = doorIndex;
                    }
                }
            }

            return (int)vehicleDoorIndex != -1;
        }

        private async Task Ungrab(PutPedInCar action)
        {
            var grabbedPed = action.Target;
            grabbedPed.Detach();
            grabbedPed.IsInvincible = false;

            grabbedPed.Task.ClearAll();
            Game.PlayerPed.Task.ClearAll();

            _speech.Say(Game.PlayerPed, "Stay here");

            API.SetEntityCollision(grabbedPed.Handle, true, true);

            await grabbedPed.StandStill();

            _comms.ToClient(ClientEvents.PedGrabbed, grabbedPed, false);

            await action.Target.StandStillFacingPlayer();
        }

        private bool IsVehicleSeatAccessible(Ped ped, Vehicle vehicle, VehicleSeat seat, bool isEnter)
        {
            var isAccessible = Function.Call<bool>(
                Hash._IS_VEHICLE_SEAT_ACCESSIBLE,
                ped.Handle,
                vehicle.Handle,
                (int)seat,
                false,
                isEnter
            );

            _logger.Debug($"Checking vehicleSeat {seat} accessibility: isAccessible {isAccessible}");
            return isAccessible;
        }
    }
}

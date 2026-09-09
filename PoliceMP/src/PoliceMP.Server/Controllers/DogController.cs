using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Microsoft.Extensions.Options;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Server.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Behaviors.Dog;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.NetworkMessages.Buckets.Notifications;
using PoliceMP.Shared.NetworkMessages.Dog.Commands;
using PoliceMP.Shared.NetworkMessages.Dog.Notifications;
using PoliceMP.Shared.NetworkMessages.Dog.Queries;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;
using PoliceMP.Shared.Options;

namespace PoliceMP.Server.Controllers
{
    public class DogController : Controller
    {
        private readonly ILogger<DogController> _log;
        private readonly ILegacyServerCommunicationsManager _legacyComms;
        private readonly ICommandManager _command;
        private readonly IServerCommunicationsManager _comms;
        private readonly IBehaviorService _behaviors;
        private readonly IOptions<DogOptions> _dogOptions;
        private readonly IBucketService _bucketService;
        private readonly ConcurrentDictionary<Player, Ped> _playerDogs = new ConcurrentDictionary<Player, Ped>();
        private readonly ConcurrentDictionary<Ped, Blackboard<DogBehavior>> _dogBlackboards =
            new ConcurrentDictionary<Ped, Blackboard<DogBehavior>>();

        public DogController(
            ILogger<DogController> log, 
            ILegacyServerCommunicationsManager legacyComms, 
            ICommandManager command, 
            IServerCommunicationsManager comms, 
            IBehaviorService behaviors,
            IOptions<DogOptions> _dogOptions,
            IBucketService bucketService)
        {
            _log = log;
            _legacyComms = legacyComms;
            _command = command;
            _comms = comms;
            _behaviors = behaviors;
            this._dogOptions = _dogOptions;
            _bucketService = bucketService;

            _legacyComms.On<Player, int, DogSound>(ServerEvents.SendDogSoundEventToServer, OnReceiveSoundEventFromClient);
            _legacyComms.On<Player, int, DogFx>(ServerEvents.SendDogParticleFxEventToServer, OnReceiveDogFxEventFromClient);
            
            _comms.AddNotificationClientForwarder<DogTookDownPlayerEvent>();
            _comms.AddNotificationHandler<DogLostOwnerEvent>(DogLostOwner);
            _comms.AddRequestHandler<ServerSpawnDogCommand, ServerSpawnDogCommandResult>(SpawnDogRequestHandler);
            _comms.AddRequestHandler<GetDogOptionsQuery, DogOptions>(GetDogOptionsQueryHandler);
            _comms.AddRequestHandler<SetDogNameCommand>(SetDogNameRequestHandler);

            _comms.AddNotificationHandler<PlayerDroppedEvent>(HandlePlayerDroppedEvent);
            _comms.AddNotificationHandler<PlayerSpawnedEvent>(HandlePlayerSpawnedEvent);

            _comms.AddNotificationHandler<PlayerMovedBucketEvent>(HandlePlayerMovedBucket);

            // command.Register("dogtest").WithHandler(player => DoSpawnDog(player));
            command.Register("dog.setname").HasGreedyArgs().WithHandler(SetDogNameCommandHandler);
        }

        private Task HandlePlayerMovedBucket(PlayerMovedBucketEvent @event)
        {
            foreach (var dog in _playerDogs)
            {
                if (int.Parse(dog.Key.Handle) == @event.PlayerServerHandle)
                {
                    _bucketService.MoveEntity(dog.Value, _bucketService.GetPlayerBucket(Players[@event.PlayerServerHandle]));
                }
            }

            return Task.CompletedTask;
        }

        private Task HandlePlayerSpawnedEvent(Player player, PlayerSpawnedEvent _)
        {
            _log.Trace($"HandlePlayerSpawnedEvent: {player.Name}");
            
            TryDeleteDogForPlayer(player);
            return Task.CompletedTask;
        }

        private Task HandlePlayerDroppedEvent(PlayerDroppedEvent @event)
        {
            _log.Trace($"HandlePlayerDroppedEvent: {@event.PlayerName}");
            
            var player = Players[@event.ServerHandle];
            TryDeleteDogForPlayer(player);
            return Task.CompletedTask;
        }

        private bool TryDeleteDogForPlayer(Player player)
        {
            try
            {
                if (_playerDogs.TryRemove(player, out var dog))
                {
                    _log.Debug($"Deleting dog for player {player.Name}...");
                    API.DeleteEntity(dog.Handle);
                    _dogBlackboards.TryRemove(dog, out _);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _log.Error($"An error occurred while trying to delete dog for player {player.Name}: {ex.Message}");
                return false;
            }
        }

        private void SetDogNameCommandHandler(Player player, string name)
        {
            if (!_playerDogs.TryGetValue(player, out var dog))
            {
                Debug.WriteLine("You do not have a dog deployed!");
                return;
            }
            
            DoSetDogName(player, dog, name);
        }

        private Task SetDogNameRequestHandler(Player player, SetDogNameCommand command)
        {
            if (_playerDogs.TryGetValue(player, out var dog))
            {
                if (dog.NetworkId != command.DogNetworkId)
                {
                    _log.Error($"Player {player.Name} tried to set their dogs name, but there was a network id mismatch!");
                    return Task.CompletedTask;
                }

                if (!_dogBlackboards.TryGetValue(dog, out var bb))
                {
                    _log.Error($"Failed to set dog name for {player.Name}. Could not find blackboard for player dog!");
                    return Task.CompletedTask;
                }
                
            }
            
            return Task.CompletedTask;
        }

        private void DoSetDogName(Player player, Ped dog, string name)
        {
            if (!_dogBlackboards.TryGetValue(dog, out var bb))
            {
                _log.Error($"Failed to set dog name for {player.Name}. Could not find blackboard for player dog!");
                return;
            }
            
            var oldName = bb.Get(b => b.DogName);
            bb.Set(b => b.DogName, name);
            dog.State.Set(PedStates.FullName, name);

            _comms.PublishAll(new DogNameChangedEvent
            {
                DogNetworkId = dog.NetworkId,
                OldName = oldName,
                NewName = name
            });
        }

        private Task<DogOptions> GetDogOptionsQueryHandler(GetDogOptionsQuery _)
        {
            return Task.FromResult(_dogOptions.Value);
        }

        private async Task<ServerSpawnDogCommandResult> SpawnDogRequestHandler(Player player, ServerSpawnDogCommand request)
        {
            _log.Debug($"Spawning dog for player: {player.Name}");
            var ped = await DoSpawnDog(player, request.DogOptionsIndex);
            var isSuccess = ped != null && API.DoesEntityExist(ped.Handle);
            return new ServerSpawnDogCommandResult
            {
                IsSuccess = isSuccess,
                DogNetworkId = isSuccess ? ped!.NetworkId : 0,
                DogOptions = isSuccess ? _dogOptions.Value.Dogs[request.DogOptionsIndex] : null,
            };
        }

        private async Task DogLostOwner(Player currentNetworkOwner, DogLostOwnerEvent @event)
        {
            try
            {
                _log.Debug($"DogLostOwner: {currentNetworkOwner?.Name ?? "Unknown"}");

                var dogNetworkId = @event.DogNetworkId;
                var expectedNetworkId = @event.OwnerNetworkId;

                var dog = Entity.FromNetworkId(dogNetworkId);
                if (dog == null)
                {
                    _log.Warn($"Dog entity not found for network ID {dogNetworkId}");
                    return;
                }

                foreach (var player in Players)
                {
                    if (player?.Character == null)
                        continue;

                    if (player.Character.NetworkId == expectedNetworkId)
                    {
                        var position = player.Character.Position;

                        try
                        {
                            _bucketService.MoveEntity(dog, _bucketService.GetPlayerBucket(player));
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"Error moving dog to player bucket: {ex.Message}");
                        }

                        _comms.PublishToClient(currentNetworkOwner, new DogShouldTeleportEvent
                        {
                            DogNetworkId = @event.DogNetworkId,
                            Position = new PmpVector3(position.X, position.Y, position.Z)
                        });

                        return;
                    }
                }

                _log.Warn($"No player found with character NetworkId {expectedNetworkId}");
            }
            catch (Exception ex)
            {
                _log.Error($"Unhandled exception in DogLostOwner: {ex.Message}");
            }
        }

        private async Task<Ped?> DoSpawnDog(Player player, int dogOptionsIndex)
        {
            var dogs = _dogOptions.Value.Dogs;

            Ped dog;

            var dogOptions = _dogOptions.Value.Dogs[dogOptionsIndex];

            if (_playerDogs.TryRemove(player, out var playerDog))
            {
                if (API.DoesEntityExist(playerDog.Handle))
                {
                    API.DeleteEntity(playerDog.Handle);
                }

                _comms.PublishAll(new PlayerDogRemovedEvent
                {
                    PlayerServerHandle = int.Parse(player.Handle),
                    PlayerPedNetworkId = player.Character.NetworkId
                });
            }

            if (dogs.Count < dogOptionsIndex + 1)
            {
                _log.Error($"Could not spawn dog for {player.Name} as index {dogOptionsIndex} is out of range of options!");
                return null;
            }

            var pos = player.Character.Position;
            var heading = player.Character.Heading;
            dog = Entity.FromHandle(API.CreatePed(0, (uint)API.GetHashKey(dogOptions.Model), pos.X, pos.Y, pos.Z, heading, true, false)) as Ped;

            if (dog is null)
            {
                _log.Debug("Dog is null!");
                return null;
            }

            if (!_playerDogs.TryAdd(player, dog))
            {
                _log.Error("Could not add dog to _playerDogs dict!");
                API.DeleteEntity(dog.Handle);
                return null;
            }

            API.SetPedComponentVariation(dog.Handle, 0, 0, dogOptions.ComponentVariation, 0);
            _bucketService.MoveEntity(dog, _bucketService.GetPlayerBucket(player));

            var bb = await _behaviors.SetPedBehavior<DogBehavior>(dog);
            if (bb is null || !_dogBlackboards.TryAdd(dog, bb))
            {
                _log.Error("Failed to set ped behavior for dog!");
                API.DeleteEntity(dog.Handle);
                if (!_playerDogs.TryRemove(player, out _))
                {
                    _log.Error("Failed to remove dog from _playerDogs dict!");
                }

                return null;
            }

            bb.Set(b => b.OwnerNetworkId, player.Character.NetworkId);
            bb.Set(b => b.State, DogBehaviorState.FollowOwner);
            bb.Set(b => b.Options, dogOptions);

            _comms.PublishAll(new PlayerDogSpawnedEvent
            {
                PlayerServerHandle = int.Parse(player.Handle),
                DogNetworkId = dog.NetworkId
            });

            await BaseScript.Delay(3000);
            // bb.Set(bb => bb.State, DogBehaviorState.TakeDownTarget);

            return dog;
        }

        private void OnReceiveSoundEventFromClient(Player player, int dogNetworkId, DogSound soundType)
        {
            _legacyComms.ToClient(ClientEvents.SendDogSoundEventToClient, dogNetworkId, soundType);
        }

        private void OnReceiveDogFxEventFromClient(Player player, int dogNetworkId, DogFx dogFx)
        {
            _legacyComms.ToClient(ClientEvents.SendDogParticleFxEventToClient, dogNetworkId, dogFx);
        }
        
    }
}
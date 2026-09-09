using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.IoC;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Scripts;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.NetworkMessages.Behaviors.Common.Commands;

namespace PoliceMP.Client.Services
{
    public class BehaviorService : IBehaviorService
    {
        private readonly Dictionary<Type, Type>
            _pedBehaviorImplementations; // Key: PedBehaviorDefinition; Value: PedBehavior<TPedBehaviorDefinition>

        private readonly ITickManager _ticks;
        private readonly IClientCommunicationsManager _comms;
        private readonly ILogger<BehaviorService> _log;
        private readonly IServiceContainer _services;

        private ConcurrentDictionary<int, Stack<PedBehaviorBase>> _behaviors = new();

        public BehaviorService(
            ILogger<BehaviorService> log,
            IServiceContainer services,
            IBehaviorTypeProvider behaviorTypes,
            ITickManager ticks,
            IClientCommunicationsManager comms)
        {
            _log = log;
            _services = services;
            _pedBehaviorImplementations = behaviorTypes.GetBehaviorTypes();
            _ticks = ticks;
            _comms = comms;

            _ticks.On(CheckBrainsTick);
            _ticks.On(CheckRemotePedsTick);

            _comms.AddRequestHandler<SetPedBehaviorCommand, bool>(SetPedBehaviorCommandHandler);
        }

        private async Task<bool> SetPedBehaviorCommandHandler(SetPedBehaviorCommand command)
        {
            Ped thePed = null;
            for (int i = 0; i < 10; i++)
            {
                if (Entity.FromNetworkId(command.PedNetworkId) is Ped entity)
                {
                    thePed = entity;
                    break;
                }

                _log.Debug($"could not find ped with network id {command.PedNetworkId}");
                await BaseScript.Delay(100);
            }

            if (thePed == null)
            {
                _log.Error(
                    $"NetworkId {command.PedNetworkId} is not a ped! it is a {Entity.FromNetworkId(command.PedNetworkId)?.GetType().Name}");
                return false;
            }

            _log.Debug(
                $"Setting brain {command.BehaviorType.Name} for entity {command.PedNetworkId} at server request.");
            SetPedBehaviorInternal(thePed, command.BehaviorType, true);
            return true;
        }

        private void HandleRemoteBrainCreate(Entity entity)
        {
            //_log.Debug($"Taking over brain ownership for NetId {entity.NetworkId}");
            if (entity is not Ped ped || !entity.Exists())
            {
                throw new ScriptManagerException($"Failed to take ownership of the brain for ped {entity.NetworkId}.");
            }

            if (_behaviors.ContainsKey(ped.NetworkId))
            {
                return;
            }

            SetPedBehaviorInternal(ped, GetBehaviorTypes(entity, true).Peek(), false);
        }

        public async Task CheckBrainsTick()
        {
            foreach (var kvp in _behaviors)
            {
                var entity = Entity.FromNetworkId(kvp.Key);
                var brains = kvp.Value;
                for (var i = 0; i < brains.Count; i++)
                {
                    var brain = brains.ToArray()[i];
                    var brainType = brain.GetBehaviorType();
                    var shouldRemove = false;
                    try
                    {
                        if (!API.NetworkDoesNetworkIdExist(kvp.Key)
                            || entity.State.Get<Stack<Type>>(PedStates.AttachedBrain).ToArray()[i] != brainType)
                        {
                            shouldRemove = true;
                        }
                    }
                    catch (Exception _)
                    {
                        shouldRemove = true;
                    }

                    if (shouldRemove)
                    {
                        _log.Debug($"Removing brain from entity {kvp.Key} as their brain is not correct. " +
                                   $"expected: {brainType?.FullName}; " +
                                   $"actual: {entity?.State?.Get<string>(PedStates.AttachedBrain) ?? "None"}");
                        if (_behaviors.TryRemove(kvp.Key, out var deletedBrains))
                        {
                            //_log.Debug($"Removing {deletedBrain.GetType().Name} from entity {entity.NetworkId}");
                            foreach (var deletedBrain in deletedBrains)
                            {
                                deletedBrain.StopInternal();
                            }

                            // Can't continue as the array has been edited. Catch the rest int the next loop
                            break;
                        }
                    }
                }
            }

            await Script.Delay(1000);
        }

        public async Task CheckRemotePedsTick()
        {
            var peds = World.GetAllPeds();

            foreach (var ped in peds)
            {
                if (!ped.Exists())
                {
                    await Script.Delay(0);
                    continue;
                }

                if (ped.State.Get<Stack<Type>>(PedStates.AttachedBrain) != null
                    && !_behaviors.ContainsKey(ped.NetworkId))
                {
                    HandleRemoteBrainCreate(ped);
                }
            }

            await Script.Delay(100);
        }

        public Blackboard<T> SetPedBehavior<T>(Ped ped)
            where T : PedBehaviorDefinition
        {
            var instance = SetPedBehaviorInternal<T>(ped, true);
            return instance.Blackboard;
        }

        public Blackboard<T> AddPedBehavior<T>(Ped ped)
            where T : PedBehaviorDefinition
        {
            var instance = AddPedBehaviorInternal<T>(ped, true);
            return instance.Blackboard;
        }

        public Stack<Type> GetBehaviorTypes(Ped ped)
        {
            return ped.State.Get<Stack<Type>>(PedStates.AttachedBrain);
        }

        public bool RemovePedBehaviors(Ped ped)
        {
            if (!_behaviors.TryRemove(ped.NetworkId, out var behaviors) || behaviors == null)
            {
                return false;
            }

            for (var i = 0; i < behaviors.Count; i++)
            {
                var behavior = behaviors.Pop();
                behavior.StopInternal();
                behavior.Dispose();

                var pedStateType = ped.State.Get<Stack<Type>>(PedStates.AttachedBrain).ToArray()[i];
                var behaviorType = behavior.GetBehaviorType();
                if (pedStateType != null && behaviorType != null && pedStateType == behaviorType)
                {
                    ped.State.Set<Stack<Type>>(PedStates.AttachedBrain, null);
                }
            }


            return true;
        }

        public Stack<Type> GetBehaviorTypes(Entity entity, bool throwOnFail = false)
        {
            var attachedBrains = entity.State.Get<Stack<Type>>(PedStates.AttachedBrain);

            for (var i = 0; i < attachedBrains.Count; i++)
            {
                var attachedBrain = attachedBrains.ToArray()[i];

                if (attachedBrain is null)
                {
                    if (throwOnFail)
                    {
                        throw new ScriptManagerException(
                            $"Failed to find brain type on ped \"{entity.NetworkId}\": {attachedBrain}");
                    }

                    return null;
                }

                if (throwOnFail && !_pedBehaviorImplementations.Keys.Contains(attachedBrain))
                {
                    throw new ScriptManagerException($"Failed to find Type \"{attachedBrain}\"");
                }
            }

            return attachedBrains;
        }

        private PedBehavior<T> AddPedBehaviorInternal<T>(Ped ped, bool isCreating) where T : PedBehaviorDefinition
        {
            return (PedBehavior<T>)AddPedBehaviorInternal(ped, typeof(T), isCreating);
        }

        private PedBehaviorBase AddPedBehaviorInternal(Ped ped, Type behaviorDefinitionType, bool isCreating)
        {
            if (!_pedBehaviorImplementations.TryGetValue(behaviorDefinitionType, out var implementingType))
            {
                throw new ArgumentException(
                    $"There is no implementing type for definition {behaviorDefinitionType.FullName}.",
                    nameof(behaviorDefinitionType));
            }

            var instance = (PedBehaviorBase)_services.GetAdhocRequired(implementingType);
            instance.SetPed(ped, isCreating);
            instance.OnDispose = () => _behaviors.TryRemove(ped.NetworkId, out _);
            var stack = _behaviors.GetOrAdd(ped.NetworkId, new Stack<PedBehaviorBase>());
            stack.Push(instance);
            if (isCreating)
            {
                var typeStack = ped.State.Get<Stack<Type>>(PedStates.AttachedBrain) ?? new Stack<Type>();
                typeStack.Push(behaviorDefinitionType);
                _log.Debug(
                    $"Adding brain state for ped {ped.NetworkId}, now {string.Join(", ", typeStack.Select(t => t.FullName))}");
                ped.State.Set(PedStates.AttachedBrain, typeStack);
                instance.Initialize();
                instance.SetMigrateInformation();
            }

            instance.StartInternal();

            return instance;
        }

        private PedBehavior<T> SetPedBehaviorInternal<T>(Ped ped, bool isCreating) where T : PedBehaviorDefinition
        {
            return (PedBehavior<T>)SetPedBehaviorInternal(ped, typeof(T), isCreating);
        }

        private PedBehaviorBase SetPedBehaviorInternal(Ped ped, Type behaviorDefinitionType, bool isCreating)
        {
            //_log.Debug($"Adding behavior to ped {ped.Handle} ({behaviorDefinitionType}). isCreating: {isCreating}");

            if (!_pedBehaviorImplementations.TryGetValue(behaviorDefinitionType, out var implementingType))
            {
                throw new ArgumentException(
                    $"There is no implementing type for definition {behaviorDefinitionType.FullName}.",
                    nameof(behaviorDefinitionType));
            }

            if (isCreating
                && _behaviors.TryRemove(ped.NetworkId, out var behaviors))
            {
                foreach (var behavior in behaviors)
                {
                    behavior.StopInternal();
                    behavior.Dispose();
                }
            }

            var instance = (PedBehaviorBase)_services.GetAdhocRequired(implementingType);
            instance.SetPed(ped, isCreating);
            instance.OnDispose = () => _behaviors.TryRemove(ped.NetworkId, out _);
            var stack = new Stack<PedBehaviorBase>();
            stack.Push(instance);
            if (_behaviors.TryAdd(ped.NetworkId, stack))
            {
                if (isCreating)
                {
                    var typeStack = new Stack<Type>();
                    typeStack.Push(behaviorDefinitionType);
                    _log.Debug(
                        $"Setting brain state for ped {ped.NetworkId} to {string.Join(", ", typeStack.Select(t => t.FullName))}");
                    ped.State.Set(PedStates.AttachedBrain, typeStack);

                    _log.Debug(ped.State.Get<Stack<Type>>(PedStates.AttachedBrain).ToString());
                    instance.Initialize();
                    instance.SetMigrateInformation();
                }

                instance.StartInternal();
            }
            else
            {
                throw new Exception("Could not add behavior!");
            }

            return instance;
        }
    }
}
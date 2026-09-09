using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Core.Server.Commands
{
    public class CommandManager : ICommandManager
    {
        private readonly ConcurrentDictionary<string, List<Delegate>> _subscriptions;
        private readonly ILogger<CommandManager> _logger;
        private readonly PlayerList _players;

        public CommandManager(ILogger<CommandManager> logger, PlayerList players)
        {
            _subscriptions = new ConcurrentDictionary<string, List<Delegate>>();
            _logger = logger;
            _players = players;
        }

        public ICommandBuilder Register(string command)
        {
            if (string.IsNullOrEmpty(command) || !command.All(c => c == '.' || char.IsLetterOrDigit(c)))
                throw new CommandException($"Failed to register command '{command}'. Command name must be only letters, digits or '.' and not contain spaces.");

            return new CommandBuilder(command, this);
        }

        public void Register(Command command, Delegate handler)
        {
            AddSubscription(command, handler);
            RegisterFiveMHandler(command);
        }

        private void AddSubscription(Command command, Delegate handler)
        {
            if (!_subscriptions.ContainsKey(command.Name))
            {
                if (!_subscriptions.TryAdd(command.Name, new List<Delegate>()))
                    throw new CommandException($"Failed to add subscription {handler.Method.DeclaringType?.Name}.{handler.Method.Name}" +
                        $" to command {command.Name}");
            }

            _subscriptions[command.Name].Add(handler);
            _logger.Trace($"On: {command} attached to {handler.Method.DeclaringType?.Name}.{handler.Method.Name}");
        }

        private void RegisterFiveMHandler(Command command)
        {
            API.RegisterCommand(command.Name, new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (!_subscriptions.TryGetValue(command.Name, out var subscriptions))
                    return;

                var player = _players[source];

                foreach (var subscription in subscriptions)
                {
                    bool methodHasPlayerParam = subscription.Method.GetParameters().FirstOrDefault()?.ParameterType == typeof(Player);
                    int methodInputParamsCount = GetHandlerInputParamsCount(subscription, methodHasPlayerParam);

                    if (!VerifyCommandUsage(command, subscription, args, methodInputParamsCount, methodHasPlayerParam))
                        continue;

                    if (command.HasGreedyArgs)
                    {
                        var greedyArgs = args.Skip(methodInputParamsCount - 1).Select(x => x.ToString()).ToList();
                        args.RemoveRange(methodInputParamsCount - 1, greedyArgs.Count());
                        args.Add(string.Join(" ", greedyArgs));
                    }

                    if (methodHasPlayerParam)
                        args.Insert(0, player);

                    try
                    {
                        subscription.DynamicInvoke(args.ToArray());
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"{e.GetType().Name} error while invoking {command.Name}: {e.Message}");
                        _logger.Error(e.StackTrace);
                    }
                }
            }), command.Restricted);
        }

        private int GetHandlerInputParamsCount(Delegate handler, bool methodHasPlayerParam)
        {
            if (methodHasPlayerParam)
                return handler.Method.GetParameters().Count() - 1;

            return handler.Method.GetParameters().Count();
        }

        private bool VerifyCommandUsage(Command command, Delegate handler, List<object> args, int methodInputParamsCount, bool methodHasPlayerParam)
        {
            if ((!command.HasGreedyArgs && methodInputParamsCount != args.Count())
                || (command.HasGreedyArgs && methodInputParamsCount > args.Count()))
            {
                string commandParams;

                if (methodHasPlayerParam)
                    commandParams = string.Join(" ", handler.Method.GetParameters().Skip(1).Select(p => $"[{p.Name}]"));
                else
                    commandParams = string.Join(" ", handler.Method.GetParameters().Select(p => $"[{p.Name}]"));

                // This will print to server and the client's chat if applicable
                // If we used logger here, the client will see things like the class name
                // So this is why Debug.WriteLine is used here
                // Debug.WriteLine($"USAGE: /{command.Name} {commandParams}");
                return false;
            }

            return true;
        }
    }
}

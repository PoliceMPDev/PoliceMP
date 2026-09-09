using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Core.Client.Commands
{
    public class CommandManager : ICommandManager
    {
        private readonly ConcurrentDictionary<string, List<Delegate>> _subscriptions;
        private readonly ILogger<CommandManager> _logger;

        public CommandManager(ILogger<CommandManager> logger)
        {
            _subscriptions = new ConcurrentDictionary<string, List<Delegate>>();
            _logger = logger;
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

                foreach (var subscription in subscriptions)
                {
                    int methodInputParamsCount = GetHandlerInputParamsCount(subscription);

                    if (!VerifyPolicies(command))
                        continue;

                    if (!VerifyCommandUsage(command, subscription, args, methodInputParamsCount))
                        continue;

                    if (command.HasGreedyArgs)
                    {
                        var greedyArgs = args.Skip(methodInputParamsCount - 1).Select(x => x.ToString()).ToList();
                        args.RemoveRange(methodInputParamsCount - 1, greedyArgs.Count());
                        args.Add(string.Join(" ", greedyArgs));
                    }

                    subscription.DynamicInvoke(args.ToArray());
                }
            }), false);
        }

        private bool VerifyPolicies(Command command)
        {
            // TODO: Policy check
            return true;
        }

        private bool VerifyCommandUsage(Command command, Delegate handler, List<object> args, int methodInputParamsCount)
        {
            if ((!command.HasGreedyArgs && methodInputParamsCount != args.Count())
               || (command.HasGreedyArgs && methodInputParamsCount > args.Count()))
            {
                var commandParams = string.Join(" ", handler.Method.GetParameters().Select(p => $"[{p.Name}]"));
                // Game.Player.SendChatMessage($"/{command.Name} {commandParams}", "USAGE");
                return false;
            }

            return true;
        }

        private int GetHandlerInputParamsCount(Delegate handler)
        {
            return handler.Method.GetParameters().Count();
        }
    }
}

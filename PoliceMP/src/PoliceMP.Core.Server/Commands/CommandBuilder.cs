using CitizenFX.Core;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Shared.Commands;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Server.Commands
{
    public class CommandBuilder : ICommandBuilder
    {
        private readonly Command _command;
        private readonly ICommandManager _commandManager;

        public CommandBuilder(string command, ICommandManager commandManager)
        {
            _command = new Command { Name = command };
            _commandManager = commandManager;
        }

        public ICommandBuilder HasGreedyArgs()
        {
            _command.HasGreedyArgs = true;
            return this;
        }

        public ICommandBuilder Restrict()
        {
            _command.Restricted = true;
            return this;
        }

        public void WithHandler(Action<Player> handler) => _On(handler);

        public void WithHandler(Action<Player, string> handler) => _On(handler);

        public void WithHandler(Action<Player, string, string> handler) => _On(handler);

        public void WithHandler(Action<Player, string, string, string> handler) => _On(handler);

        public void WithHandler(Action<Player, string, string, string, string> handler) => _On(handler);

        public void WithHandler(Action<Player, string, string, string, string, string> handler) => _On(handler);

        public void WithHandler(Action handler) => _On(handler);

        public void WithHandler(Action<string> handler) => _On(handler);

        public void WithHandler(Action<string, string> handler) => _On(handler);

        public void WithHandler(Action<string, string, string> handler) => _On(handler);

        public void WithHandler(Action<string, string, string, string> handler) => _On(handler);

        public void WithHandler(Action<string, string, string, string, string> handler) => _On(handler);


        public void WithHandler(Func<Player, Task> handler) => _On(handler);

        public void WithHandler(Func<Player, string, Task> handler) => _On(handler);

        public void WithHandler(Func<Player, string, string, Task> handler) => _On(handler);

        public void WithHandler(Func<Player, string, string, string, Task> handler) => _On(handler);

        public void WithHandler(Func<Player, string, string, string, string, Task> handler) => _On(handler);

        public void WithHandler(Func<Player, string, string, string, string, string, Task> handler) => _On(handler);

        public void WithHandler(Func<Task> handler) => _On(handler);

        public void WithHandler(Func<string, Task> handler) => _On(handler);

        public void WithHandler(Func<string, string, Task> handler) => _On(handler);

        public void WithHandler(Func<string, string, string, Task> handler) => _On(handler);

        public void WithHandler(Func<string, string, string, string, Task> handler) => _On(handler);

        public void WithHandler(Func<string, string, string, string, string, Task> handler) => _On(handler);

        private void _On(Delegate handler)
        {
            _commandManager.Register(_command, handler);
        }
    }
}

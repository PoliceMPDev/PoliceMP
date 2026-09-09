using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Shared.Commands;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Commands
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

        public ICommandBuilder RequirePolicy(string policy)
        {
            throw new NotImplementedException();
        }

        public ICommandBuilder HasGreedyArgs()
        {
            _command.HasGreedyArgs = true;
            return this;
        }

        public void WithHandler(Action handler) => _On(handler);
        public void WithHandler(Action<string> handler) => _On(handler);
        public void WithHandler(Action<string, string> handler) => _On(handler);
        public void WithHandler(Action<string, string, string> handler) => _On(handler);
        public void WithHandler(Action<string, string, string, string> handler) => _On(handler);
        public void WithHandler(Action<string, string, string, string, string> handler) => _On(handler);

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

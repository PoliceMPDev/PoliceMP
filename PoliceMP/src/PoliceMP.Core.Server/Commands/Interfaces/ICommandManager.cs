using PoliceMP.Core.Shared.Commands;
using System;

namespace PoliceMP.Core.Server.Commands.Interfaces
{
    public interface ICommandManager
    {
        ICommandBuilder Register(string command);
        void Register(Command command, Delegate handler);
    }
}

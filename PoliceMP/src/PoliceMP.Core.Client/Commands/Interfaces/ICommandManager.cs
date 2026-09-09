using PoliceMP.Core.Shared.Commands;
using System;

namespace PoliceMP.Core.Client.Commands.Interfaces
{
    public interface ICommandManager
    {
        ICommandBuilder Register(string command);
        void Register(Command command, Delegate handler);
    }
}

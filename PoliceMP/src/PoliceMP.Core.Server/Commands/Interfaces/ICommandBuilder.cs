using CitizenFX.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PoliceMP.Core.Server.Commands.Interfaces
{
    public interface ICommandBuilder
    {
        // Allows spaces in the last property to expand into the property
        ICommandBuilder HasGreedyArgs();
        // Makes command unrestricted
        ICommandBuilder Restrict();
        void WithHandler(Action handler);
        void WithHandler(Action<string> handler);
        void WithHandler(Action<string, string> handler);
        void WithHandler(Action<string, string, string> handler);
        void WithHandler(Action<string, string, string, string> handler);
        void WithHandler(Action<string, string, string, string, string> handler);
        void WithHandler(Action<Player> handler);
        void WithHandler(Action<Player, string> handler);
        void WithHandler(Action<Player, string, string> handler);
        void WithHandler(Action<Player, string, string, string> handler);
        void WithHandler(Action<Player, string, string, string, string> handler);
        void WithHandler(Action<Player, string, string, string, string, string> handler);

        void WithHandler(Func<Task> handler);
        void WithHandler(Func<string, Task> handler);
        void WithHandler(Func<string, string, Task> handler);
        void WithHandler(Func<string, string, string, Task> handler);
        void WithHandler(Func<string, string, string, string, Task> handler);
        void WithHandler(Func<string, string, string, string, string, Task> handler);
        void WithHandler(Func<Player, Task> handler);
        void WithHandler(Func<Player, string, Task> handler);
        void WithHandler(Func<Player, string, string, Task> handler);
        void WithHandler(Func<Player, string, string, string, Task> handler);
        void WithHandler(Func<Player, string, string, string, string, Task> handler);
        void WithHandler(Func<Player, string, string, string, string, string, Task> handler);
    }
}

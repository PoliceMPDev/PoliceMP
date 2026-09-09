using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PoliceMP.Core.Client.Commands.Interfaces
{
    public interface ICommandBuilder
    {
        // Allows spaces in last string value
        ICommandBuilder HasGreedyArgs();
        void WithHandler(Action handler);
        void WithHandler(Action<string> handler);
        void WithHandler(Action<string, string> handler);
        void WithHandler(Action<string, string, string> handler);
        void WithHandler(Action<string, string, string, string> handler);
        void WithHandler(Action<string, string, string, string, string> handler);

        void WithHandler(Func<Task> handler);
        void WithHandler(Func<string, Task> handler);
        void WithHandler(Func<string, string, Task> handler);
        void WithHandler(Func<string, string, string, Task> handler);
        void WithHandler(Func<string, string, string, string, Task> handler);
        void WithHandler(Func<string, string, string, string, string, Task> handler);
    }
}

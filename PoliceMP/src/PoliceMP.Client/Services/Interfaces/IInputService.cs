using System.Threading.Tasks;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IInputService
    {
        Task<string> ShowKeyboardInput(string textEntry, string exampleText, int maxStringLength);
        bool IsStarted { get; }
        Task StartAsync();
    }
}
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Actions.Interfaces
{
    public interface IActionManager
    {
        bool CanExecute();
        Task<bool> Execute(IAction action, bool force = false);
        void AddHandler(IActionHandler actionHandler);
    }
}
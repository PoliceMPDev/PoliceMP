using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Actions.Interfaces
{
    public interface IActionHandler
    {
        Task Initialise();
        Type GetActionType();
        Task<bool> Handle(IAction action);
    }
}
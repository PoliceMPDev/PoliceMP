using CitizenFX.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IInstructionalButtonsService
    {
        Task AddInstructionalButton(string name, int control);
        Task AddInstructionalButton(string name, Control control);
        Task RemoveInstructionalButton(string name);
        Task RemoveInstructionalButtons(List<string> names);
        bool IsButtonShown(string name);
    }
}
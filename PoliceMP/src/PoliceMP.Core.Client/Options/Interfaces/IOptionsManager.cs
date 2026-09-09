using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Options.Interfaces
{
    public interface IOptionsManager
    {
        PoliceMP.Shared.Options.Options Options { get; }
        Task Initialise();
    }
}
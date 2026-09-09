using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Core.Server.Interfaces.Factories
{
    public interface IPedInfoFactory
    {
        PedInfo Random(int networkId, Gender gender);
    }
}
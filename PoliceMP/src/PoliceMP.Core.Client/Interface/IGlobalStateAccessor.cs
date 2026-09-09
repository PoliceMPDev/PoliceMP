using CitizenFX.Core;

namespace PoliceMP.Core.Client.Interface
{
    public interface IGlobalStateAccessor
    {
        StateBag GlobalState { get; }
    }
}
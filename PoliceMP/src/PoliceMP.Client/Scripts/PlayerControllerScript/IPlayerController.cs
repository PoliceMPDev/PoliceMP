using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.PlayerControllerScript
{
    public interface IPlayerController
    {
        void SetControl(bool hasControl, PlayerControlFlag flags = PlayerControlFlag.None);
        void ResetControl();
        Task GrabPed(Ped ped);
        Task UngrabPed();
        PlayerState State { get; }
        Ped GrabbedPed { get; }
        Entity InteractionTarget { get; }
        Entity LastInteractionTarget { get; }
    }
}
using CitizenFX.Core;
using System.Collections.Generic;

namespace PoliceMP.Client.Scripts.PlayerControllerScript.Interfaces
{
    public interface IContextualActionsProvider
    {
        IList<PlayerAction> GetNormalActions(IPlayerController playerController);
        IList<PlayerAction> GetInteractionActions(IPlayerController playerController, Entity context);
        IList<PlayerAction> GetAttachedActions(IPlayerController playerController, Entity context, Entity adsContext);
        IList<PlayerAction> GetAimDownSightActions(IPlayerController playerController, Entity context);
    }
}

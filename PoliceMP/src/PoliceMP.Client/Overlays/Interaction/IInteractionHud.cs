using PoliceMP.Client.Scripts.PlayerControllerScript;
using PoliceMP.Core.Client.Interface;
using System.Collections.Generic;
using CitizenFX.Core;
using PoliceMP.Core.Client.Overlays;

namespace PoliceMP.Client.Overlays.Interaction
{
    public interface IInteractionHud : IOverlay
    {
        Entity Entity { get; }
        void SetInteractionEntity(Entity entity);
        void SetInteractionActions(IList<PlayerAction> actions);
        void ClearInteractionContext();
    }
}
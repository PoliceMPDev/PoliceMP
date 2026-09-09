using PoliceMP.Core.Mediator;
namespace PoliceMP.Shared.NetworkMessages.Game.Notifications
{
    public class PedCuffedNotification: INotification
    {
        public int cufferNetworkId;

        public int cuffeeNetworkId;
    }
}
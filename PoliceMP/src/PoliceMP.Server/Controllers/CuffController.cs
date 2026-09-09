using System.Threading.Tasks;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;
namespace PoliceMP.Server.Controllers
{
    public class CuffController : Controller
    {
        private readonly IServerCommunicationsManager _comms;
        
        public CuffController(IServerCommunicationsManager comms)
        {
            _comms = comms;
            
            _comms.AddNotificationHandler<PedCuffedNotification>(PedCuffedHandler);
            _comms.AddNotificationHandler<PedUncuffedNotification>(PedUncuffedHandler);
        }

        private async Task PedCuffedHandler(PedCuffedNotification notification)
        {
            _comms.PublishToClients(notification);
        }
        
        private async Task PedUncuffedHandler(PedUncuffedNotification notification)
        {
            _comms.PublishToClients(notification);
        }
    }
}
using PoliceMP.Core.Mediator;
namespace PoliceMP.Shared.NetworkMessages.Game.Notifications
{
    public class PlayAmbientSoundEvent: INotification
    {
        public int PedNetworkId { get; set; }
        
        public string SpeechName { get; set; }
    }
}
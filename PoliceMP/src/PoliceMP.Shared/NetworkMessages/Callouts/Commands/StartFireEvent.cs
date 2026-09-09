using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Callouts.Commands
{
    public class StartFireEvent : INotification
    {
        public float LocationX { get; set; }
        public float LocationY { get; set; }
        public float LocationZ { get; set; }
    }
}
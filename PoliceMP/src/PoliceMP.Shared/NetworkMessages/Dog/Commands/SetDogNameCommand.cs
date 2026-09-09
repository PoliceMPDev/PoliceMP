using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Dog.Commands
{
    public class SetDogNameCommand : IServerRequest
    {
        public int DogNetworkId { get; set; }
        public string DogName { get; set; }
    }
}
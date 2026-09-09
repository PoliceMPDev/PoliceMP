using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Dog.Commands
{
    public class RemovePlayerDogCommand : IServerRequest
    {
        public int DogNetworkId { get; set; }
    }
}
using PoliceMP.Core.Mediator;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Options;

namespace PoliceMP.Shared.NetworkMessages.Dog.Commands
{
    public class ServerSpawnDogCommandResult
    {
        public bool IsSuccess { get; set; }
        public int DogNetworkId { get; set; }
        public DogOptionsEntry DogOptions { get; set; }
    }
    
    public class ServerSpawnDogCommand : IServerRequest<ServerSpawnDogCommandResult>
    {
        public int DogOptionsIndex { get; set; }
    }
}
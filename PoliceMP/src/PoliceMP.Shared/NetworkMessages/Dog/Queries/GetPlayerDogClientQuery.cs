using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Dog.Queries
{
    public class GetPlayerDogQueryResponse
    {
        public int NetworkId { get; set; }
    }
    
    public class GetPlayerDogClientQuery : IClientRequest<GetPlayerDogQueryResponse>
    {
    }
}
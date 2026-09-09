using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.NetworkMessages.Callouts.Queries
{
    public class GetCalloutInfoResponse
    {
        public int CalloutId { get; set; }
        public PmpVector2 BlipLocation { get; set; }
        // public List<int> PlayerServerHandles { get; set; } @todo to do later
    }

    public class GetCalloutInfoByIdQuery : IServerRequest<GetCalloutInfoResponse>
    {
        public int CalloutId { get; set; }
    }
}
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface ICurrentTicketService
    {
        Ticket Ticket { get; }
        void Clear();
    }
}
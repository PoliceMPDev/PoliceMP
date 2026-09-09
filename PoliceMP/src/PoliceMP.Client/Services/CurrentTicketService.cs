using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Services
{
    public class CurrentTicketService : ICurrentTicketService
    {
        public Ticket Ticket { get; private set; } = new Ticket();
        public void Clear() => Ticket = new Ticket();
    }
}
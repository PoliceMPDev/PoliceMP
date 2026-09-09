using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Actions.TicketPed
{
    public class TicketPed : IAction
    {
        public Ped Target { get; }
        public Ticket Ticket { get; }

        public TicketPed(Ped target, Ticket ticket)
        {
            Target = target;
            Ticket = ticket;
        }
    }
}
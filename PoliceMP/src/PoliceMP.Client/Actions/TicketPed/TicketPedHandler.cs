using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions.TicketPed
{
    public class TicketPedHandler : ActionHandler<TicketPed>
    {
        private readonly IPedInfoService _pedInfoService;
        private readonly INotificationService _notifications;
        private readonly ISpeechService _speech;

        public TicketPedHandler(IPedInfoService pedInfoService,
            INotificationService notifications,
            ISpeechService speech)
        {
            _pedInfoService = pedInfoService;
            _notifications = notifications;
            _speech = speech;
        }

        protected override async Task<bool> Handle(TicketPed action)
        {
            var pedInfo = await _pedInfoService.GetByNetworkId(action.Target.NetworkId);

            _speech.Say(Game.PlayerPed, "I'm going to write you a Fixed Penalty Notice...");

            await Delay(1000);

            Game.PlayerPed.Task.PlayAnimation("veh@busted_low", "issue_ticket_cop");

            _speech.Do(Game.PlayerPed, "Writes up a Fixed Penalty Notice");

            await Delay(2000);

            _speech.Do(action.Target, "Facepalms.");

            action.Target.Task.PlayAnimation("anim@mp_player_intcelebrationfemale@face_palm",
                "face_palm");

            Game.PlayerPed.IsPositionFrozen = false;

            var builder = new StringBuilder();

            builder.Append($"<b>Name:</b> {pedInfo?.FullName}<br>");
            builder.Append($"<b>Date:</b> {DateTime.Now:D}<br>");
            builder.Append($"<b>Points:</b> <span class='text-warning'>{action.Ticket.TotalPoints}</span><br>");
            builder.Append($"<b>Fine:</b> <span class='text-warning'>£{action.Ticket.TotalFine}.00</span><br>");

            builder.Append("<br><b>Offences</b><br>");

            foreach (var offence in action.Ticket.Offences)
                builder.Append($"- {offence.Name}<br>");

            if (action.Ticket.TotalPoints >= 12)
                builder.Append(
                    "<br><span class='text-danger'>" +
                    "The vehicle should be towed because driver has received 12 points on their license." +
                    "</span>");

            _notifications.Info("Ticket", builder.ToString());

            await action.Target.StandStillFacingPlayer();
            return true;
        }
    }
}
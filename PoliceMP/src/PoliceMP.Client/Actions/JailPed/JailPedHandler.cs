using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Options.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Options;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions.JailPed
{
    public class JailPedHandler : ActionHandler<JailPed>
    {
        private readonly IOptionsManager _options;
        private readonly INotificationService _notifications;
        private readonly ISpeechService _speech;

        public JailPedHandler(IOptionsManager options,
            INotificationService notifications,
            ISpeechService speech)
        {
            _options = options;
            _notifications = notifications;
            _speech = speech;
        }

        protected override async Task<bool> Handle(JailPed action)
        {
            if (!action.Target.IsCuffed)
            {
                _notifications.Error("Jail", "The person must be cuffed before put in jail.");
                return false;
            }

            var jailPoints = _options.Options.Action.JailPoints;
            JailPoint thisJailPoint = null;

            foreach (var jailPoint in jailPoints)
            {
                if (action.Target.IsCloseEnoughToPoint(jailPoint.RadiusPoint.ToCitizenVector3(), 20f))
                {
                    thisJailPoint = jailPoint;
                    break;
                }
            }

            if (thisJailPoint == null)
            {
                _notifications.Error("Jail", "There is no jail nearby.");
                return false;
            }

            var cell = thisJailPoint.Cells[AppRandom.Next(thisJailPoint.Cells.Count - 1)];

            _speech.Do(action.Target, $"Gets jailed by {Game.Player.Name}.");

            var timeOut = DateTime.Now;
            while ((DateTime.Now - timeOut).TotalSeconds <= 5
                   && !action.Target.IsCloseEnoughToPoint(cell.ToCitizenVector3(), 3f))
            {
                action.Target.Task.GoTo(cell.ToCitizenVector3());
                await Delay(1000);
            }

            await Delay(2500);
            API.NetworkFadeOutEntity(action.Target.Handle, true, false);

            return true;
        }
    }
}
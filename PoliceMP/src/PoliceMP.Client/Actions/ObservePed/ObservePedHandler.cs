using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Options.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions.ObservePed
{
    public class ObservePedHandler : ActionHandler<ObservePed>
    {
        private readonly IPedInfoService _pedInfo;
        private readonly INotificationService _notifications;
        private readonly IOptionsManager _optionsManager;
        private readonly List<string> _usedObservations = new List<string>();

        public ObservePedHandler(IPedInfoService pedInfo,
            INotificationService notifications,
            IOptionsManager optionsManager)
        {
            _pedInfo = pedInfo;
            _notifications = notifications;
            _optionsManager = optionsManager;
        }

        protected override async Task<bool> Handle(ObservePed action)
        {
            var pedInfo = await _pedInfo.GetByNetworkId(action.Target.NetworkId);

            var builder = new StringBuilder();
            builder.Append("You notice something about the person...<br><br>");

            bool onCannabis = ObserveCannabis(pedInfo, builder);
            bool onAlcohol = ObserveAlcohol(pedInfo, builder);
            bool onCocaine = ObserveCocaine(pedInfo, builder);
            bool onEcstasy = ObserveEcstasy(pedInfo, builder);
            bool onHeroin = ObserveHeroin(pedInfo, builder);
            bool hasInjuries = ObserveInjuries(action, builder);

            if (!onCannabis && !onAlcohol && !onCocaine && !onEcstasy && !onHeroin && !hasInjuries)
            {
                _notifications.Info("Observations", "No observations were made.");
                return true;
            }

            _notifications.Info("Observations", builder.ToString());
            return true;
        }

        private bool ObserveCannabis(PedInfo pedInfo, StringBuilder builder)
        {
            int quantity = AppRandom.Next(1, 3);

            if (!pedInfo.IsOnCannabis) return false;

            for (var i = 0; i < quantity; i++)
            {
                string randomObservation = GetRandomObservation(_optionsManager.Options.Action.CannabisObservations);
                builder.Append(randomObservation);
            }

            return true;
        }

        private bool ObserveAlcohol(PedInfo pedInfo, StringBuilder builder)
        {
            int quantity = AppRandom.Next(1, 3);

            if (!(pedInfo.AlcoholLevel > 0.8f)) return false;

            for (var i = 0; i < quantity; i++)
            {
                string randomObservation = GetRandomObservation(_optionsManager.Options.Action.AlcoholObservations);
                builder.Append(randomObservation);
            }

            return true;
        }

        private bool ObserveCocaine(PedInfo pedInfo, StringBuilder builder)
        {
            int quantity = AppRandom.Next(1, 3);

            if (!pedInfo.IsOnCocaine) return false;

            for (var i = 0; i < quantity; i++)
            {
                string randomObservation = GetRandomObservation(_optionsManager.Options.Action.CocaineObservations);
                builder.Append(randomObservation);
            }

            return true;
        }

        private bool ObserveEcstasy(PedInfo pedInfo, StringBuilder builder)
        {
            int quantity = AppRandom.Next(1, 3);

            if (!pedInfo.IsOnEcstasy) return false;

            for (var i = 0; i < quantity; i++)
            {
                string randomObservation = GetRandomObservation(_optionsManager.Options.Action.EcstasyObservations);
                builder.Append(randomObservation);
            }

            return true;
        }

        private bool ObserveHeroin(PedInfo pedInfo, StringBuilder builder)
        {
            int quantity = AppRandom.Next(1, 3);

            if (!pedInfo.IsOnEcstasy) return false;

            for (var i = 0; i < quantity; i++)
            {
                string randomObservation = GetRandomObservation(_optionsManager.Options.Action.HeroinObservations);
                builder.Append(randomObservation);
            }

            return true;
        }

        private bool ObserveInjuries(ObservePed action, StringBuilder builder)
        {
            var isInjured = false;

            if (API.HasEntityBeenDamagedByAnyVehicle(action.Target.Handle))
            {
                builder.Append("Looks like they have been <span class='text-warning'>hit by a vehicle.</span><br>");
                isInjured = true;
            }

            if (action.Target.HasBeenDamagedByAnyWeapon())
            {
                builder.Append("Looks like they have been <span class='text-warning'>hurt with a weapon.</span><br>");
                isInjured = true;
            }

            if (action.Target.HasBeenDamagedBy(action.Subject))
            {
                builder.Append("They were hit by <span class='text-warning'>you.</span><br>");
                isInjured = true;
            }

            return isInjured;
        }

        private string GetRandomObservation(List<string> observations)
        {
            string observation = observations.Except(_usedObservations).GetRandom();
            _usedObservations.Add(observation);
            return observation;
        }
    }
}
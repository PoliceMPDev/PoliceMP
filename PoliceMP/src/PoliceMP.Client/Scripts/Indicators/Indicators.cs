#pragma warning disable CS0618 // Type or member is obsolete
using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.Indicators
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Indicators : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private Tuple<bool, bool> _indicators = new Tuple<bool, bool>(false, false);

        public Indicators(ILegacyClientCommunicationsManager comms, ITickManager ticks)
        {
            _comms = comms;

            ticks.On(OnTick);

            comms.On(ClientEvents.IndicatorsChanged, (int vehicleNetId, Tuple<bool, bool> indicators) =>
            {
                var vehicle = (Vehicle)Entity.FromNetworkId(vehicleNetId);
                if (vehicle == null || !vehicle.Exists()) return;
                vehicle.IsLeftIndicatorLightOn = indicators.Item1;
                vehicle.IsRightIndicatorLightOn = indicators.Item2;
            });
        }

        private Task OnTick()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle == null)
            {
                _indicators = new Tuple<bool, bool>(false, false);
                return Task.FromResult(0);
            }

            var left = vehicle.IsLeftIndicatorLightOn;
            var right = vehicle.IsRightIndicatorLightOn;

            if (left == _indicators.Item1 && right == _indicators.Item2) return Task.FromResult(0);
            _indicators = new Tuple<bool, bool>(left, right);
            _comms.ToServer(ServerEvents.IndicatorsChanged, vehicle.NetworkId, _indicators);

            return Task.FromResult(0);
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete

using System;
using static CitizenFX.Core.Native.API;
using PoliceMP.Core.Client.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Scripts;


namespace PoliceMP.Client.Scripts.EntityFreeze
{
    
    public class EntityFreezeHandler : Script
    {
        private readonly ITickManager _ticks;

        private readonly List<string> _entities = new List<string>()
        {
            "prop_trafficlight",
            "prop_traffic_lightset_01",
            "prop_traffic_03b",
            "prop_traffic_03a",
            "prop_traffic_02b",
            "prop_traffic_02a",
            "prop_traffic_01d",
            "prop_traffic_01b",
            "prop_traffic_01a",
            "prop_traffic_01",
            "prop_streetlight_01b",
            "prop_streetlight_01",
            "prop_streetlight_01b",
            "prop_postbox_01a",
            "prop_bin_01a",
            "prop_sign_road_05a",
        };

        private bool _isFrozen = false;
        
        public EntityFreezeHandler(ITickManager ticks, ICommandManager command)
        {
            _ticks = ticks;
            
            command.Register("entityfreeze").WithHandler(() =>
            {
                _isFrozen = !_isFrozen;
                
                if (_isFrozen)
                {
                    _ticks.On(OnTick);
                }
                else
                {
                    _ticks.Off(OnTick);
                }
            });
        }

        private async Task OnTick()
        {
            var pos = GetEntityCoords(PlayerPedId(), false);

            Parallel.ForEach(_entities, async (entity) =>
            {
                var obj = GetClosestObjectOfType(pos.X, pos.Y, pos.Z, 100, (uint) GetHashKey(entity), false, false,
                    false);

                if (obj > 0 && (IsPedWalking(PlayerPedId()) || IsPedRunning(PlayerPedId()) ||
                                IsPedInAnyVehicle(PlayerPedId(), false)))
                {
                    FreezeEntityPosition(obj, true);
                }
            });

            await Script.Delay(500);
        }
    }
}
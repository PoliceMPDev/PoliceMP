using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Enums;
using System.Drawing;
using PoliceMP.Shared.Behaviors.FailToStop;

namespace PoliceMP.Client.Scripts.Taser
{
    public class TaserPedWritheScript : Script
    {
        private readonly ILogger<TaserPedWritheScript> _log;
        private readonly ITickManager _ticks;
        private readonly IBehaviorService _behaviors;
        private readonly Random _random = new Random();

        public TaserPedWritheScript(ILogger<TaserPedWritheScript> log, ITickManager ticks, IBehaviorService behaviors)
        {
            _log = log;
            _ticks = ticks;
            _behaviors = behaviors;
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(TaserPedWritheTick);
            return Task.FromResult(0);
        }

        protected override Task OnStopAsync()
        {
            _ticks.Off(TaserPedWritheTick);
            return Task.FromResult(0);
        }

        private async Task TaserPedWritheTick()
        {
            if (Game.PlayerPed.Weapons.Current?.Hash == WeaponHash.StunGun)
            {
                int entityHandle = 0;
                API.GetEntityPlayerIsFreeAimingAt(API.GetPlayerIndex(), ref entityHandle);

                if (entityHandle > 0)
                {
                    API.SetPedMinGroundTimeForStungun(entityHandle, 1000);

                    if (API.IsPedBeingStunned(entityHandle, 0))
                    {
                        await DoWrithe(new Ped(entityHandle), Game.Player);
                    }
                }
            }
        }

        private async Task DoWrithe(Ped ped, Player player)
        {
            if (!await ped.TryRequestNetworkEntityControl())
            {
                _log.Error("Could not get network control!");
                return;
            }

            var result = _random.Next(2);

            //_log.Debug($"Taser writhe value: {result}");
            if (result == 0)
            {
                while (ped.IsRagdoll)
                    await Script.Delay(0);

                ped.Task.ClearAllImmediately();
                ped.Heading += 180;
                Function.Call(Hash.TASK_WRITHE, ped.Handle, Game.PlayerPed.Handle, 10, 0, 1, 10000);
                API.SetPedKeepTask(ped.Handle, true);
            }
            else
            {
                _behaviors.SetPedBehavior<FailToStopBehavior>(ped);

                while (ped.IsBeingStunned)
                    await Script.Delay(0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.NoWantedLevel
{
    public class NoWantedLevelScript : Script
    {
        private readonly ITickManager _ticks;

        public NoWantedLevelScript(ITickManager ticks)
        {
            _ticks = ticks;
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(NoWantedTick);

            return Task.FromResult(0);
        }

        private Task NoWantedTick()
        {
            API.SetMaxWantedLevel(0);

            return Task.FromResult(0);
        }
    }
}

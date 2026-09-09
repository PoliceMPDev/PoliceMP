using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.MedicalActions
{
    public class MedicalActionsScript : Script
    {
        private readonly ITickManager _ticks;
        
        public MedicalActionsScript(ITickManager ticks)
        {
            _ticks = ticks;
        }

        protected override async Task OnStartAsync()
        {
            _ticks.On(DownedPedChecker);
        }
        private async Task DownedPedChecker()
        {
            var peds = World.GetAllPeds();

            for (int i = 0; i < peds.Length; i++)
            {
                var ped = peds[i];
                if (ped.IsAlive)
                {
                    await Delay(0);
                    continue;
                }

                var playerPos = Game.PlayerPed.Position;
                if (API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, ped.Position.X, ped.Position.Y,
                    ped.Position.Z, true) <= 2f)
                {
                    
                }
                
            }
            
        }
    }
}
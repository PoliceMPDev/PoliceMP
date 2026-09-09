using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.SyncDeadPlayers
{
    public class SyncDeadPlayersScript : Script
    {
        private readonly ITickManager _ticks;
        private const string PositionStateBagKey = "pmp:SyncDeadPlayersScript:position";

        public SyncDeadPlayersScript(ITickManager ticks)
        {
            _ticks = ticks;
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(SyncDeadPlayersScriptTick);
            return Task.FromResult(0);
        }

        private async Task SyncDeadPlayersScriptTick()
        {
            if (Game.PlayerPed.IsDead)
            {
                Game.PlayerPed.State.Set(PositionStateBagKey, Game.PlayerPed.Position);
                API.ResetPedRagdollTimer(Game.PlayerPed.Handle);
            }
            
            var peds = World.GetAllPeds().Where(p => p.IsPlayer && p != Game.PlayerPed);
            foreach (var ped in peds)
            {
                if (ped.IsDead && ped.Velocity.Length() < 0.5f)
                {
                    var statePos = ped.State.Get<Vector3>(PositionStateBagKey);
                    
                    if (World.GetDistance(ped.Position, statePos) > 1f)
                    {
                        API.SetEntityCoordsWithoutPlantsReset(ped.Handle, statePos.X, statePos.Y, statePos.Z, false, false, false, false);

                        API.ResetPedRagdollTimer(ped.Handle);
                        API.ActivatePhysics(ped.Handle);
                    }
                    
                    await Script.Delay(0);
                }
            }
        }
    }
}
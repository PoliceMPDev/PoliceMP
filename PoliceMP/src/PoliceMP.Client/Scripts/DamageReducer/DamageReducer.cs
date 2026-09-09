using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Interface;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.DamageReducer
{
    public class DamageReducer : Script
    {
        private readonly ITickManager _ticks;
        private readonly string[] _weapons = { "WEAPON_NIGHTSTICK" };

       // private readonly string[] _weapons = { "WEAPON_UNARMED", "WEAPON_NIGHTSTICK" };

        public DamageReducer(ITickManager ticks)
        {
            _ticks = ticks;
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(DamageReducerTick);
            return Task.FromResult(0);
        }

        private Task DamageReducerTick()
        {//API.N_0x4757f00bc6323cfe((uint)API.GetHashKey("WEAPON_UNARMED"), 0.3f);
            API.N_0x4757f00bc6323cfe((uint)API.GetHashKey("WEAPON_TORCH"), 0.1f);
            API.N_0x4757f00bc6323cfe((uint)API.GetHashKey("WEAPON_NIGHTSTICK"), 0.1f);
            API.N_0x4757f00bc6323cfe((uint)API.GetHashKey("WEAPON_TRAININGMP5SEMI"), 0.3f); 
            API.RemoveStealthKill((uint)API.GetHashKey("ACT_stealth_kill_a"), true);
            API.RemoveStealthKill((uint)API.GetHashKey("ACT_stealth_kill_weapon"), true);
            API.RemoveStealthKill((uint)API.GetHashKey("ACT_stealth_kill_b"), true);
            API.RemoveStealthKill((uint)API.GetHashKey("ACT_stealth_kill_c"), true);
            API.RemoveStealthKill((uint)API.GetHashKey("ACT_stealth_kill_d"), true);
            API.RemoveStealthKill((uint)API.GetHashKey("ACT_stealth_kill_a_gardener"), true);

            foreach (var weapon in _weapons)
            {
                API.SetWeaponDamageModifier((uint)API.GetHashKey(weapon), 0f);
            }

            if (API.IsPedArmed(API.PlayerPedId(), 6))
            {
                API.DisableControlAction(1, 140, true);
                API.DisableControlAction(1, 141, true);
                API.DisableControlAction(1, 142, true);
            }

            return Task.FromResult(0);
        }
    }
}
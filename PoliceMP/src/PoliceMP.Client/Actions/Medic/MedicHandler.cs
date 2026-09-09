using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Actions.Medic
{
    public class MedicHandler : ActionHandler<Medic>
    {
        private readonly ILogger<MedicHandler> _logger;
        private readonly INotificationService _notification;
        private readonly ISpeechService _speech;

        private Random _random;
        
        public MedicHandler(ILogger<MedicHandler> logger, INotificationService notification, ISpeechService speech)
        {
            _logger = logger;
            _notification = notification;
            _speech = speech;

            _random = new Random();
        }

        private const string BAG_MODEL = "w_am_als";
        protected override async Task<bool> Handle(Medic action)
        {
            API.RequestAnimDict("rcmextreme3");
            while (!API.HasAnimDictLoaded("rcmextreme3"))
            {
                API.RequestAnimDict("rcmextreme3");
                await Delay(0);
            }

            API.TaskGoToEntity(Game.PlayerPed.Handle, action.Target.Handle, 1000, 0.1f, 1f, 1073741824, 0);
            await Delay(200);

            API.TaskPlayAnim(Game.PlayerPed.Handle, "rcmextreme3", "idle", 8.0f, 2.0f, 10000, 1, 2f, false, false, false);
            await Delay(1000);
            
            var bagPos = Game.PlayerPed.Position;
            bagPos.X += 1f;
            bagPos.Y += 1f;
            var defib = await World.CreateProp(new Model(API.GetHashKey(BAG_MODEL)), bagPos,
                Game.PlayerPed.Rotation, false, true);
            defib.SetNoCollision(action.Target, true);
            
            API.RemoveWeaponFromPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ALS"));
            Game.PlayerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
            
            var sequence = new TaskSequence();
            sequence.AddTask.StandStill(-1);
            sequence.Close();
            action.Target.AlwaysKeepTask = true;
            action.Target.Task.PerformSequence(sequence);
            
            defib.Delete();
            API.GiveWeaponToPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ALS"), 1000, false, true);
            action.Target.Health = 100;

            return true;
        }

    }
}
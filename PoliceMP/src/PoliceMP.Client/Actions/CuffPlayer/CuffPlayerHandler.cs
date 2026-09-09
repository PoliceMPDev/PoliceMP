using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using CitizenFX.Core.UI;
using PoliceMP.Client.Overlays.Legacy.IdCardOverlay;
using PoliceMP.Client.Utils;
using PoliceMP.Shared.Models;
using System.Collections.Generic;
using System.Drawing;

namespace PoliceMP.Client.Actions.CuffPlayer
{
    /*
    public class Cuff : IAction
    {
        public Ped Arrester { get; }
        public Ped Target { get; }

        public Cuff(Ped arrester, Ped target)
        {
            Arrester = arrester;
            Target = target;
        }
    }
    */
    public class CuffPlayerHandler : ActionHandler<CuffPlayer>
    {
        private readonly INotificationService _notifications;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ISpeechService _speech;
        private readonly ILogger<CuffPlayerHandler> _logger;
        private readonly IBehaviorService _behavior;

        public CuffPlayerHandler(INotificationService notifications,
            ILegacyClientCommunicationsManager comms,
            ISpeechService speech, ILogger<CuffPlayerHandler> logger, IBehaviorService behavior)
        {
            _notifications = notifications;
            _comms = comms;
            _speech = speech;
            _logger = logger;
            _behavior = behavior;
        }


        protected override async Task<bool> Handle(CuffPlayer action)
        {
            action.Target.SetBoolDecor(PedStates.CuffPlayerActionActive, true);

            var player = Game.PlayerPed;
            var beingCuffedPlayer = action.Target.Handle;
            var cuffedState = API.DecorGetInt(beingCuffedPlayer, "PoliceMP_CuffPlayer_State");

            Debug.WriteLine("player has attempted to cuff another player");

            switch (cuffedState)
            {
                case 0:
                    API.DecorSetInt(beingCuffedPlayer, "PoliceMP_CuffPlayer_State", 1); //Set the player cuffed
                    Debug.WriteLine("set the player to PoliceMP_CuffPlayer_State : 1");
                    await Delay(100);
                    await player.Task.PlayAnimation("mp_arresting", "idle", 8f, -8f, -1, (AnimationFlags)49, 0);
                    action.Target.SetBoolDecor(PedStates.CuffPlayerActionActive, false);
                    return true;
                case 2:
                    API.DecorSetInt(beingCuffedPlayer, "PoliceMP_CuffPlayer_State", 3); //Set the player cuffed
                    Debug.WriteLine("set the player to PoliceMP_CuffPlayer_State : 3");
                    await Delay(100);
                    await player.Task.PlayAnimation("mp_arresting", "idle", 8f, -8f, -1, (AnimationFlags)49, 0);
                    action.Target.SetBoolDecor(PedStates.CuffPlayerActionActive, false);
                    return true;
                default:
                    action.Target.SetBoolDecor(PedStates.CuffPlayerActionActive, false);
                    return true;
            }
        }
    }
}
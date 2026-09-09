using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Extensions;
using PoliceMP.Client.Overlays.Legacy.IdCardOverlay;
using PoliceMP.Client.Scripts.Afk;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Actions.AnalyseBody
{
    public class AnalyseBodyHandler : ActionHandler<AnalyseBody>
    {
        #region Services

        private readonly ILogger<AnalyseBodyHandler> _logger;
        private readonly INotificationService _notification;
        private readonly IdCardOverlay _idCardOverlay;
        private readonly IPedInfoService _pedInfo;

        #endregion


        public AnalyseBodyHandler(ILogger<AnalyseBodyHandler> logger, INotificationService notification, IdCardOverlay idCardOverlay, IPedInfoService pedInfo)
        {
            _logger = logger;
            _notification = notification;
            _idCardOverlay = idCardOverlay;
            _pedInfo = pedInfo;
        }

        protected override async Task<bool> Handle(AnalyseBody action)
        {

            action.Target.SetBoolDecor(PedStates.AnalyseBodyActionActive, true);

            var player = Game.PlayerPed;
            var causeOfDeath = (WeaponHash)API.GetPedCauseOfDeath(action.Target.Handle);
            var causeOfDeathMessage = Death.GetDeathCauseByWeapon(causeOfDeath).Cause;
            var pedInfo = await _pedInfo.GetByNetworkId(action.Target.NetworkId);

            var drivingLicenseTypes = new List<string>();
            if (pedInfo.HasDrivingLicense) drivingLicenseTypes.Add("Car");

            Screen.ShowSubtitle("~y~You are now examining the ~r~body...", 13000);
            API.ExecuteCommand("e medic2");
            await Delay(4000);
            API.ExecuteCommand("e mechanic");
            await Delay(5500);
            API.ExecuteCommand("e clipboard");
            await Delay(3700);
            Screen.ShowSubtitle($"~y~After analysing the position and injuries of the ~r~body~y~, your investigatiuon concludes that the most likely cause of death was by ~r~{causeOfDeathMessage}! Here is the PNC info that comes up for the injured person.", 10000);

            _idCardOverlay.Show(new IdCard
            {
                FirstName = pedInfo.FirstName,
                LastName = pedInfo.LastName,
                DateOfBirth = pedInfo.DateOfBirth.ToString("dd/MM/yyyy"),
                Height = pedInfo.Height,
                IsMale = true,
                Type = pedInfo.HasDrivingLicense ? IdCardType.Driver : IdCardType.None,
                DrivingLicenseTypes = drivingLicenseTypes
            });

            API.ExecuteCommand("e c"); //Cancels emote

            await Delay(10000);
            _idCardOverlay.Hide();

            action.Target.SetBoolDecor(PedStates.AnalyseBodyActionActive, false);
            return true;
        }
    }
}
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Extensions;
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

namespace PoliceMP.Client.Actions.CollectDNA
{
    public class CollectDNAHandler : ActionHandler<CollectDNA>
    {
        #region Services

        private readonly ILogger<CollectDNAHandler> _logger;
        private readonly INotificationService _notification;

        #endregion

        public CollectDNAHandler(ILogger<CollectDNAHandler> logger, INotificationService notification)
        {
            _logger = logger;
            _notification = notification;
        }

        protected override async Task<bool> Handle(CollectDNA action)
        {

            action.Target.SetBoolDecor(PedStates.CollectDNAActionActive, true);

            var sourceOfDeath = API.GetPedSourceOfDeath(action.Target.Handle);
            if (!API.IsPedAPlayer(sourceOfDeath))
            {
                Screen.ShowSubtitle("~y~No ~r~DNA ~y~relating to a possible ~r~Suspect ~y~can be found on scene!", 10000);
                return true;
            }

            var intPlayerIndex = API.NetworkGetPlayerIndexFromPed(sourceOfDeath);
            //var playerName = API.GetPlayerName(playerIndex);
            var playerServerID = API.GetPlayerServerId(intPlayerIndex);

            API.ExecuteCommand("e medic2");
            await Delay(1000);
            API.ExecuteCommand("e mechanic");
            Screen.ShowSubtitle("~y~You are searching for any ~r~Fingerprints~y~, ~r~DNA ~y~or ~r~Clothing fibres ~y~that may belong to a ~r~Suspect...", 13000);
            Random random = new Random();
            int chancOfFinding = random.Next(1, 101);
            await Delay(15000);

            if (chancOfFinding >= 95)
            {
                Screen.ShowSubtitle("~y~You could not find any ~r~evidence ~y~that might link back to a ~r~Suspect~y~!", 10000);
                return true;
            }

            Screen.ShowSubtitle("~y~You have found a possible ~r~Suspects fingerprints ~y~and ~r~fibres ~y~on scene, you are now collecting them...", 15000);
            API.ExecuteCommand("e clean");
            await Delay(15000);
            Screen.ShowSubtitle("~y~You are now comparing your findings to the ~r~PNC~y~ and ~r~Databases~y~ for a match...", 10000);
            API.ExecuteCommand("e tablet");
            await Delay(10000);
            API.ExecuteCommand("e c");
            Screen.ShowSubtitle($"~y~You have a match! Your potential ~r~Suspect~y~ is...", 10000);
            API.ExecuteCommand($"showid {playerServerID}");
            await Delay(2500);
            API.ExecuteCommand($"showid {playerServerID}");
            await Delay(2500);
            API.ExecuteCommand($"showid {playerServerID}");
            await Delay(2500);

            action.Target.SetBoolDecor(PedStates.CollectDNAActionActive, false);
            return true;
        }
    }
}
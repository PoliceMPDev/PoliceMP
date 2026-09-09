using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.WhatThreeWord
{
    public class WhatThreeWordScript : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ICommandManager _command;

        public WhatThreeWordScript(ILegacyClientCommunicationsManager comms, ICommandManager command)
        {
            _comms = comms;
            _command = command;
            _comms.On<string>(ServerEvents.SendWhatThreeWordToClient, word =>
            {
                BaseScript.TriggerEvent("chat:addMessage", $"^3 ^* [W3P] Your W3P Location: {word}");
            });
            _comms.On<PmpVector3>(ServerEvents.SendWhatThreeWordPosToClient, (position) =>
            {
                API.SetNewWaypoint(position.X, position.Y);
            });
        }

        protected override async Task OnStartAsync()
        {
            _command.Register("gotow3w").HasGreedyArgs().WithHandler(whatWord =>
            {
                _comms.ToServer(ClientEvents.GoToWhatThreeWord, whatWord);
            });
        }
    }
}
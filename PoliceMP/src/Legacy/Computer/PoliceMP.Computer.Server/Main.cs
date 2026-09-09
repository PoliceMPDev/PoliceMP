using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Computer.Server
{
    public class Main : BaseScript
    {
        [EventHandler("PoliceMPComputer:RequestAllPlayersRanks")]
        private void OnRequestPlayerRanks([FromSource] Player sourcePlayer, string requestGuid)
        {
            var dict = new Dictionary<string, List<string>>();

            foreach (var player in Players)
            {
                var ranks = new List<string>();
                // Trained ranks
                if (API.IsPlayerAceAllowed(player.Handle, "Police.afoTrained")) ranks.Add("AFO");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.rpuTrained")) ranks.Add("RPU");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.cidTrained")) ranks.Add("CID");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.npasTrained")) ranks.Add("NPAS");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.mpuTrained")) ranks.Add("MPU");

                // Admin
                if (API.IsPlayerAceAllowed(player.Handle, "Police.adminAuth")) ranks.Add("Admin");

                // Police Ranks
                if (API.IsPlayerAceAllowed(player.Handle, "Police.pc")) ranks.Add("PC");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.sc")) ranks.Add("SC");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.sergeant")) ranks.Add("SGT");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.inspector")) ranks.Add("INSP");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.chiefinspector")) ranks.Add("CI");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.superintendent")) ranks.Add("SI");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.chiefsuperintendent")) ranks.Add("CSI");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.assistantcommissioner")) ranks.Add("AC");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.deputycommissioner")) ranks.Add("DC");
                if (API.IsPlayerAceAllowed(player.Handle, "Police.commissioner")) ranks.Add("CO");

                dict.Add(player.Name, ranks);
            }

            sourcePlayer.TriggerEvent("PoliceMP:ReceiveRequestResult",
                requestGuid,
                JsonConvert.SerializeObject(dict));
        }
    }
}

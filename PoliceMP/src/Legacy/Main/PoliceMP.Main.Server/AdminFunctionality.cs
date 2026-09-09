using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PoliceMP.Main.Server
{
    class AdminFunctionality : BaseScript
    {
        [EventHandler("PoliceMP:AdminFunctionalityYeetGunServerHandle")]
        private void SendYeetToAllClients(int id)
        {
            var entity = API.NetworkGetEntityFromNetworkId(id);
            if (API.DoesEntityExist(entity))
            {
                API.DeleteEntity(entity);
            }
            
            TriggerClientEvent("PoliceMP:AdminFunctionalityClientSideYeeting", id);
        }

        [EventHandler("PoliceMP:requestingSpecWhitelisting")]
        public void requestingSpecWhitelisting([FromSource] Player player, bool _command)
        {
            Debug.WriteLine("PoliceMP:requestingSpecWhitelisting");
            if (_command)
            {
                bool found = false;
                foreach (var id in player.Identifiers)
                {
                    if (id.Contains("127.0.0.1"))
                    {
                        found = true;
                    }
                }
                if (!found) { return; }
            }

            bool AFOStatus = false, RPUStatus = false, CIDStatus = false, AdminAuth = false, NPASStatus = false, MPUStatus = false, DogStatus = false, NHSStatus = false, FireStatus = false, MODstatus = false, DeveloperStatus = false, isWhitelisted = false;

            if (API.IsPlayerAceAllowed(player.Handle, "Police.pc") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.sc") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.sergeant") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.inspector") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.chiefinspector") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.superintendent") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.chiefsuperintendent") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.assistantcommissioner") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.deputycommissioner") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.commissioner") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.modAuth") ||
                API.IsPlayerAceAllowed(player.Handle, "Police.adminAuth")
                )
            {
                isWhitelisted = true;
                Debug.WriteLine("Player: " + player.Name + " is a whitelisted member.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "Police.afoTrained") == true)
            {
                AFOStatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted AFO status.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "Police.rpuTrained") == true)
            {
                RPUStatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted RPU status.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "Police.cidTrained") == true)
            {
                CIDStatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted CID status.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "Police.adminAuth") == true)
            {
                AdminAuth = true;
                AFOStatus = true;
                RPUStatus = true;
                CIDStatus = true;
                NPASStatus = true;
                MPUStatus = true;
                DogStatus = true;
                NHSStatus = true;
                FireStatus = true;
                MODstatus = true;
                isWhitelisted = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted PoliceMPAdmin.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "Police.npasTrained") == true)
            {
                NPASStatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted NPAS status.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "Police.mpuTrained"))
            {
                MPUStatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted MPU status.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "Police.dogTrained"))
            {
                DogStatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted Dog Section status.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "NHS.Trained"))
            {
                NHSStatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted NHS status.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "Fire.Trained"))
            {
                FireStatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted Fire Service status.");
            }

            if (API.IsPlayerAceAllowed(player.Handle, "Police.modAuth"))
            {
                MODstatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted Moderator status.");
            }
            
            if (API.IsPlayerAceAllowed(player.Handle, "Police.developer"))
            {
                DeveloperStatus = true;
                Debug.WriteLine("Player: " + player.Name + " has been granted Developer status.");
            }

            TriggerClientEvent(player, "PoliceMP:recieveSpecWhitelisting", AFOStatus, RPUStatus, CIDStatus, NPASStatus, MPUStatus, AdminAuth, DogStatus, NHSStatus, FireStatus, MODstatus, DeveloperStatus, isWhitelisted);
            Debug.WriteLine("Whitelisting done...");
        }


        [EventHandler("PoliceMP:DogSectionUpdateServer")]
        private void DogSectionUpdate([FromSource] Player player, int _pl, int _switchToNet)
        {
            TriggerClientEvent("PoliceMP:DogSectionUpdateClient", _pl, _switchToNet);
        }
    }
}

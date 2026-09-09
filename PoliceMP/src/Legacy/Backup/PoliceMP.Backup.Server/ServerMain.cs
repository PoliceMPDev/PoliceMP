using System;
using CitizenFX.Core;

namespace PoliceMP.Backup.Server
{
    public class ServerMain : BaseScript
    {
        public ServerMain()
        {

        }

        [EventHandler("PoliceMP:ClientRequestingBackup")]
        private void ClientRequestingBackup([FromSource]Player _pl, string _requesttype)
        {
            Debug.WriteLine(DateTime.Now.ToString() + ": " + _pl.Name + " requested backup " + _requesttype);
            string request = "Unkown request";
            switch (_requesttype)
            {
                case "RESPONSE":
                    request = "~b~" + _pl.Name + "~s~ is requesting Response Backup";
                    TriggerClientEvent("PoliceMP:BackupRequestRecieved", _pl.Handle, request, _requesttype);
                    break;
                case "RPU":
                    request = "~b~" + _pl.Name + "~s~ is requesting RPU Backup";
                    TriggerClientEvent("PoliceMP:BackupRequestRecieved", _pl.Handle, request, _requesttype);
                    break;
                case "AFO":
                    request = "~b~" + _pl.Name + "~s~ is requesting AFO Backup";
                    TriggerClientEvent("PoliceMP:BackupRequestRecieved", _pl.Handle, request, _requesttype);
                    break;
                case "CID":
                    request = "~b~" + _pl.Name + "~s~ is requesting CID Backup";
                    TriggerClientEvent("PoliceMP:BackupRequestRecieved", _pl.Handle, request, _requesttype);
                    break;
                case "SUPERVISOR":
                    request = "~b~" + _pl.Name + "~s~ is requesting Supervisor Backup";
                    TriggerClientEvent("PoliceMP:BackupRequestRecieved", _pl.Handle, request, _requesttype);
                    break;
                case "PANIC":
                    request = "~b~" + _pl.Name + "~r~ has activated their panic button";
                    TriggerClientEvent("PoliceMP:BackupRequestRecieved", _pl.Handle, request, _requesttype);
                    break;                 
            }
        }
    }
}

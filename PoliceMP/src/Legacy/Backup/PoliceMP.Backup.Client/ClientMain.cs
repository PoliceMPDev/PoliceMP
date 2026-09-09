using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using PoliceMP.Main.Core.Client;

namespace PoliceMP.Backup.Client
{
    public class ClientMain : BaseScript
    {
        private Menu backupmenu = new Menu("Request Backup", "Request backup from players.");
        private Menu trackmenu = new Menu("Track Player", "Select a player to track");

        //Holders for whitelisting
        private bool AFOStatus = false;
        private bool RPUStatus = false;
        private bool CIDStatus = false;
        private bool NPASStatus = false;
        private bool MPUStatus = false;
        private bool AdminAuth = false;
        private bool DogStatus = false;
        private bool NHSStatus = false;
        private bool FireStatus = false;
        private bool ModAuth = false;
        private bool WhitelistedMember = false;

        private bool recentlyRequested = false;
        private bool backupToBeAccepted = false;
        private int REQUESTLIMIT = 10000;
        private int TIMETOACCEPTBACKUP = 5000;

        private Ped CurrentlyTracking;
        private bool trackingPlayer = false;

        public ClientMain()
        {
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.AddMenu(backupmenu);

            var cancel = new MenuItem("Cancel Route", "Press this to cancel your backup GPS.");
            var players = new MenuItem("Track Player") { Label = "→→→" };
            MenuController.BindMenuItem(backupmenu, trackmenu, players);
            var bk1 = new MenuItem("Response Backup");
            var bk2 = new MenuItem("Request RPU");
            bk2.RightIcon = MenuItem.Icon.CAR;
            var bk3 = new MenuItem("Request AFO");
            bk3.RightIcon = MenuItem.Icon.GUN;
            var bk4 = new MenuItem("Request CID");
            bk4.RightIcon = MenuItem.Icon.INV_KEYCARD;
            var bk5 = new MenuItem("Request Supervisor");
            var panic = new MenuItem("~r~PANIC BUTTON");
            panic.RightIcon = MenuItem.Icon.WARNING;

            backupmenu.AddMenuItem(cancel);
            backupmenu.AddMenuItem(players);
            backupmenu.AddMenuItem(bk1);
            backupmenu.AddMenuItem(bk2);
            backupmenu.AddMenuItem(bk3);
            backupmenu.AddMenuItem(bk4);
            backupmenu.AddMenuItem(bk5);
            backupmenu.AddMenuItem(panic);

            backupmenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == cancel) StopTracking();
                if (item == players) RefreshPlayerMenuAndOpen();
                if (item == bk1) SendRequest("RESPONSE");
                if (item == bk2) SendRequest("RPU");
                if (item == bk3) SendRequest("AFO");
                if (item == bk4) SendRequest("CID");
                if (item == bk5) SendRequest("SUPERVISOR");
                if (item == panic)
                {
                    SendRequest("PANIC");
                    PanicSound();
                }
            };
        }

        [EventHandler("PoliceMP:recieveSpecWhitelisting")]
        private void recieveSpecWhitelisting(bool afo, bool rpu, bool cid, bool npas, bool mpu, bool admin, bool dog, bool nhs, bool fire, bool mod, bool whitelisted)
        {
            AFOStatus = afo;
            RPUStatus = rpu;
            CIDStatus = cid;
            NPASStatus = npas;
            MPUStatus = mpu;
            AdminAuth = admin;
            DogStatus = dog;
            NHSStatus = nhs;
            FireStatus = fire;
            ModAuth = mod;
            WhitelistedMember = whitelisted;
        }

        [EventHandler("PoliceMP:BackupRequestRecieved")]
        private async void BackupRequestRecieved(int _playerNameRequesting, string _requestText, string _typeRequesting)
        {
            //Player requester = null;
            if(Game.Player.Handle == API.GetPlayerFromServerId(_playerNameRequesting))
            {
                ClientFunctions.ShowToast("Backup", "You have successfully sent a backup request", "info");
                return;
            }

            switch (_typeRequesting)
            {
                case "RESPONSE":
                    ClientFunctions.ShowToast("Backup", _requestText, "info");
                    break;
                case "RPU":
                    if (RPUStatus)
                    {
                        ClientFunctions.ShowToast("Backup", _requestText, "info");
                    }
                    break;
                case "AFO":
                    if (AFOStatus)
                    {
                        ClientFunctions.ShowToast("Backup", _requestText, "info");
                    }
                    break;
                case "CID":
                    if (CIDStatus)
                    {
                        ClientFunctions.ShowToast("Backup", _requestText, "info");
                    }
                    break;
                case "SUPERVISOR":
                    if (AdminAuth)
                    {
                        ClientFunctions.ShowToast("Backup", _requestText, "info");
                    }
                    break;
                case "PANIC":
                    ClientFunctions.ShowToast("Backup", _requestText, "info");
                    await PanicSound();
                    break;
            } 
        }

        private async Task SendRequest(string _backuptype)
        {
            if (recentlyRequested)
            {
                ClientFunctions.ShowToast("Backup", "You have recently requested backup, please wait.", "info");
                return;
            }
            TriggerServerEvent("PoliceMP:ClientRequestingBackup", _backuptype);
            recentlyRequested = true;
            await RequestLimiter();
        }

        private async Task RequestLimiter()
        {
            await Delay(REQUESTLIMIT);
            recentlyRequested = false;
        }

        [Tick]
        private async Task MenuOpen()
        {
            if (backupmenu.Visible) { return; }
            if (API.IsInputDisabled(2) 
                && Game.IsControlJustPressed(0, (Control)289) 
                && !Game.IsControlPressed(0, (Control)21))
            {
                backupmenu.OpenMenu();
            }
        }

        private void StopTracking()
        {
            try
            {
                CurrentlyTracking.AttachedBlip.ShowRoute = false;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("@@ Error stopping track: " + ex);
            }
            trackingPlayer = false;
            CurrentlyTracking = null;
        }

        private async Task PanicSound()
        {
            for(int i=0; i<5; i++)
            {
                Game.PlaySound("CONFIRM_BEEP", "HUD_MINI_GAME_SOUNDSET");
                await Delay(250);
            }
        }

        private void RefreshPlayerMenuAndOpen()
        {
            bool isPlayer = false;
            trackmenu.ClearMenuItems();
            
            for(int i=0; i<=256; i++)
            {
                if (!API.NetworkIsPlayerActive(i)) { continue; }

                if(i == Game.Player.Handle) { continue; }

                var p = new MenuItem("[" + i + "]" + " - " + API.GetPlayerName(i)) { Label = "GPS →→→" };
                
                int handle = API.GetPlayerPed(i);
                Ped playerped = (Ped)Ped.FromHandle(handle);

                if (playerped == null || playerped.Position == null) { continue; }
                if(playerped == Game.PlayerPed) { continue; }

                if (playerped.IsInPoliceVehicle)
                {
                    if (playerped.IsInBoat)
                    {
                        p.RightIcon = MenuItem.Icon.INV_BOAT;
                    }
                    else if (playerped.IsInHeli)
                    {
                        p.RightIcon = MenuItem.Icon.INV_HELI;
                    }
                    else if (playerped.IsOnBike)
                    {
                        p.RightIcon = MenuItem.Icon.BIKE;
                    }
                    else
                    {
                        p.RightIcon = MenuItem.Icon.CAR;
                    }
                }

                trackmenu.AddMenuItem(p);

                trackmenu.OnItemSelect += (menu, item, index) =>
                {
                    if (item == p)
                    {
                        if (trackingPlayer)
                        {
                            ClientFunctions.ShowToast("Backup", "You are already tracking a player", "info");
                            return;
                        }
                        TrackPed(playerped);
                    }
                };
            }
        }

        private void TrackPed(Ped _ped)
        {
            try
            {
                if (trackingPlayer)
                {
                    StopTracking();
                }

                _ped.AttachedBlip.ShowRoute = true;

                CurrentlyTracking = _ped;
                trackingPlayer = true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("@@ Error with tracking: " + ex);
            } 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using MenuAPI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.Callsign
{
    public class CallSignMenuScript : Script
    {
        private readonly IFiveEventManager _fiveEvents;
        private readonly IPermissionService _permission;
        private readonly ILogger<CallSignMenuScript> _logger;
        private readonly IPlayerService _playerService;
        private readonly INotificationService _notification;
        private readonly ICommandManager _command;
        private readonly ITickManager _ticks;
        private readonly IGameInputManager _gameInput;
        private readonly ILegacyClientCommunicationsManager _comms;
        private UserAces _aces;

        private Menu _menu;

        public CallSignMenuScript(IFiveEventManager fiveEvents, ICommandManager command, IPermissionService permission,
            ILogger<CallSignMenuScript> logger, IPlayerService playerService, INotificationService notification,
            ITickManager ticks, IGameInputManager gameInput, ILegacyClientCommunicationsManager comms)
        {
            _fiveEvents = fiveEvents;
            _command = command;
            _permission = permission;
            _logger = logger;
            _playerService = playerService;
            _notification = notification;
            _ticks = ticks;
            _gameInput = gameInput;
            _comms = comms;

            _fiveEvents.On("PoliceMP:MDTChangeCallsign", new Func<string, Task>(MdtChangeCallsign));
        }

        protected override async Task OnStartAsync()
        {
            _command.Register("callsign").WithHandler(CallSignMenu);
            _ticks.On(ShiftEnterPress);
            _aces = await _permission.GetUserAces();
        }

        private async Task MdtChangeCallsign(string callsign)
        {
            await Delay(0);
            _logger.Debug("MDT CHANGE CALLSIGN CALLED, CALLSIGN: " + callsign);
            Game.Player.State.Set(PlayerStates.CallSign, callsign, true);
        }

        private string hoveredCallsign = null;

        private async Task ShiftEnterPress()
        {
            await Delay(0);
            if (_menu == null) return;
            if (hoveredCallsign == null) return;
            if (!_menu.Visible) return;

            if (_gameInput.IsBeingHeld(Control.FrontendAccept))
            {
                _menu.CloseMenu();
                _notification.Success("Callsign", $"You have force assigned the callsign: {hoveredCallsign}");
                Game.Player.State.Set(PlayerStates.CallSign, hoveredCallsign, true);
            }
        }

        private void CallSignMenu()
        {
            _logger.Debug("entry");
            if (_menu != null)
            {
                if (_menu.Visible) return;
            }

            var playerList = _playerService.FetchAllRecentPlayerInfo().Result;


            _menu = new Menu("Callsign Selection");
            MenuController.AddMenu(_menu);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            _logger.Debug("1");


            #region Prefix List

            var prefixList = new List<string>();
            switch (_permission.CurrentUserRole.Branch)
            {
                case UserBranch.Police:
                    switch (_permission.CurrentUserRole.Division)
                    {
                        case UserDivision.Ert:
                            if (_aces.IsBandTwo) prefixList.Add("BX");
                            if (_aces.IsBandThree) prefixList.Add("SX");
                            if (_aces.IsBandThree) prefixList.Add("RI");
                            if (_aces.IsBandTwo || _aces.IsTierTwo) prefixList.Add("RS");
                            if (_aces.IsWhiteListed) prefixList.Add("CW");
                            if (!_aces.IsWhiteListed || _aces.IsTierTwo) prefixList.Add("PCSO");
                            if (_aces.IsTsgTrained) prefixList.Add("Uniform");
                            if (_aces.IsJruTrained) prefixList.Add("JR-");
                            break;
                        case UserDivision.Tsg:
                            if (_aces.IsBandTwo) prefixList.Add("BX");
                            if (_aces.IsBandThree) prefixList.Add("SX");
                            if (_aces.IsBandThree) prefixList.Add("RI");
                            if (_aces.IsBandTwo || _aces.IsTierTwo) prefixList.Add("RS");
                            if (_aces.IsTsgTrained) prefixList.Add("Uniform");
                            break;
                        case UserDivision.Afo:
                            if (_aces.IsBandTwo) prefixList.Add("BX");
                            if (_aces.IsBandThree) prefixList.Add("SX");
                            if (_aces.IsBandThree || _aces.IsDeveloper) prefixList.Add("FI");
                            if (_aces.IsBandTwo || _aces.IsDeveloper || _aces.IsTierTwo) prefixList.Add("FS");
                            prefixList.Add("Trojan");
                            prefixList.Add("FM");
                            prefixList.Add("EXPO-");
                            prefixList.Add("CBRN-");
                            break;
                        case UserDivision.Rpu:
                            if (_aces.IsBandTwo) prefixList.Add("BX");
                            if (_aces.IsBandThree) prefixList.Add("SX");
                            if (_aces.IsBandThree || _aces.IsDeveloper) prefixList.Add("OT");
                            if (_aces.IsBandTwo || _aces.IsDeveloper || _aces.IsTierTwo) prefixList.Add("OS");
                            prefixList.Add("OC");
                            prefixList.Add("Sierra");
                            break;
                        case UserDivision.Cid:
                            if (_aces.IsBandTwo) prefixList.Add("BX");
                            if (_aces.IsBandThree) prefixList.Add("SX");
                            if (_aces.IsBandThree || _aces.IsDeveloper) prefixList.Add("DI");
                            if (_aces.IsBandTwo || _aces.IsDeveloper || _aces.IsTierTwo) prefixList.Add("DS");
                            prefixList.Add("SO");
                            break;
                        case UserDivision.Dsu:
                            if (_aces.IsBandTwo) prefixList.Add("BX");
                            if (_aces.IsBandThree) prefixList.Add("SX");
                            if (_aces.IsBandThree || _aces.IsDeveloper) prefixList.Add("XI");
                            if (_aces.IsBandTwo || _aces.IsDeveloper || _aces.IsTierTwo) prefixList.Add("XS");
                            prefixList.Add("X");
                            break;
                        case UserDivision.Npas:
                            prefixList.Add("NPAS");
                            prefixList.Add("HELIMED");
                            prefixList.Add("DU");
                            prefixList.Add("DAUPHIN");
                            prefixList.Add("RAINBOW");
                            break;
                        default:
                            prefixList.Add("Unit");
                            break;
                    }

                    break;
                case UserBranch.Fire:
                    if (_aces.IsFireBoroughCommander || _aces.IsDeveloper) prefixList.Add("LFRS-IC");
                    if (_aces.IsFireStationCommander || _aces.IsDeveloper) prefixList.Add("JC");
                    if (_aces.IsFireOfficer || _aces.IsDeveloper || _aces.IsTierTwo) prefixList.Add("JO"); // Station Officer (upper band 2)
                    if (_aces.IsFireTrainer || _aces.IsDeveloper || _aces.IsTierTwo) prefixList.Add("JS"); // Sub Officer (band 2)
                    prefixList.Add("FR");
                    prefixList.Add("LW");
                    break;
                case UserBranch.Nhs:
                    if (_aces.IsBandTwo) prefixList.Add("LHS-IC");
                    if (_aces.IsNhsSectionLeader || _aces.IsDeveloper) prefixList.Add("OM"); // Band 3

                    if (_aces.IsNhsClinicalTl || _aces.IsDeveloper || _aces.IsTierTwo)
                        prefixList.Add("TL"); // Clinical Band 2
                    if (_aces.IsNhsHemsTl || _aces.IsDeveloper || _aces.IsTierTwo) prefixList.Add("HE"); // Hems Band 2
                    if (_aces.IsHartTL || _aces.IsDeveloper || _aces.IsTierTwo) prefixList.Add("HA"); // Hart Band 2
                    if (_aces.IsDeveloper || _aces.IsTierTwo || _aces.IsTierOne) prefixList.Add("XC"); // Beep Doctors

                    if (_aces.IsNhsHems || _aces.IsTierTwo) prefixList.Add("HO");
                    if (_aces.IsHartTrained || _aces.IsTierTwo) prefixList.Add("HART-");
                    if (_aces.IsHartTrained || _aces.IsTierTwo) prefixList.Add("SORT-");
                    if (_aces.IsNhsParamedic || _aces.IsTierTwo) prefixList.Add("JR-");
                    if (_aces.IsNhsParamedic || _aces.IsStudentPara || _aces.IsDeveloper) prefixList.Add("NC");
                    prefixList.Add("SD");
                    break;
                case UserBranch.Civ:
                    break;
                case UserBranch.Highways:
                    prefixList.Add("ME");
                    break;
                case UserBranch.Control:
                    prefixList.Add("CONTROL");
                    prefixList.Add("OPs");
                    break;
                default:
                    _logger.Debug("Somehow not logged as a faction");
                    return;
            }

            var menuItemPrefix = new MenuListItem("Prefix", prefixList, 0);
            _menu.AddMenuItem(menuItemPrefix);

            #endregion

            var currentUser = _permission.CurrentUserRole;

            var callSignRange = Enumerable.Range(1, 200).ToList();

            if (currentUser.Division == UserDivision.Npas)
            {
                callSignRange = new List<int>
                {
                    1, 2, 10, 11, 12, 13, 27, 28, 61, 62, 63, 75, 76, 81, 82
                };
            }

            if (currentUser.Branch == UserBranch.Control)
            {
                callSignRange = new List<int>
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
                };
            }

            var callSignSuffixList = callSignRange.ConvertAll<string>(i => i.ToString());

            var menuItemSuffix = new MenuListItem("Suffix", callSignSuffixList, 0);
            _menu.AddMenuItem(menuItemSuffix);

            var menuItemSelect = new MenuItem("Select Callsign") { LeftIcon = MenuItem.Icon.STAR };
            _menu.AddMenuItem(menuItemSelect);

            var menuItems = new List<MenuItem>();
            foreach (var player in playerList)
            {
                if (string.IsNullOrEmpty(player.CallSign))
                {
                    menuItems.Add(new MenuItem($"NO CALLSIGN {player.Name}"));
                }
                else
                {
                    menuItems.Add(new MenuItem($"[{player.CallSign}] {player.Name}")
                    {
                        ItemData = (string)player.CallSign
                    });
                }
            }

            menuItems.Sort((x, y) => x.Text.CompareTo(y.Text));
            foreach (var item in menuItems)
            {
                _menu.AddMenuItem(item);
            }


            _menu.OnItemSelect += async (menu, item, index) =>
            {
                if (item != menuItemSelect)
                {
                    if (item.ItemData == null) return;
                    Game.Player.State.Set(PlayerStates.CallSign, item.ItemData, true);
                    _notification.Success("Callsign", $"You have joined the callsign: {(string)item.ItemData}");
                }
                else
                {
                    var sign = $"{menuItemPrefix.GetCurrentSelection()} {menuItemSuffix.GetCurrentSelection()}";

                    hoveredCallsign = sign;

                    var players = await _playerService.FetchAllRecentPlayerInfo();

                    foreach (var player in players)
                    {
                        _logger.Debug($"Callsign for {player.Name} - {player.CallSign}");
                    }

                    if (players.Any(p => p.CallSign != null && p.CallSign == sign))
                    {
                        _notification.Error("Callsign",
                            "Someone is already using this callsign. Hold Enter to force use this callsign.");
                        return;
                    }

                    Game.Player.State.Set(PlayerStates.CallSign, sign, true);
                    _notification.Success("Callsign", $"You have been assigned the callsign: {sign}");
                    //_comms.ToServer(ServerEvents.OnReceiveCallSign, sign);
                }
            };
            _menu.OpenMenu();
        }
    }
}
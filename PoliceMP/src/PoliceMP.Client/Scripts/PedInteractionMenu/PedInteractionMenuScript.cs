using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Actions.AskForId;
using PoliceMP.Client.Actions.Breathalyse;
using PoliceMP.Client.Actions.Drugalyse;
using PoliceMP.Client.Actions.Follow;
using PoliceMP.Client.Actions.Grab;
using PoliceMP.Client.Actions.HandsUp;
using PoliceMP.Client.Actions.JailPed;
using PoliceMP.Client.Actions.Kneel;
using PoliceMP.Client.Actions.LieDown;
using PoliceMP.Client.Actions.ObservePed;
using PoliceMP.Client.Actions.QuestionPed;
using PoliceMP.Client.Actions.ReleasePed;
using PoliceMP.Client.Actions.SearchPed;
using PoliceMP.Client.Actions.StandUp;
using PoliceMP.Client.Actions.TicketPed;
using PoliceMP.Client.Actions.WarnPed;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Options.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.PedInteractionMenu
{
    public class PedInteractionMenuScript : Script, IPedInteractionMenuScript
    {
        private Ped _targetPed;

        #region Dependencies

        private readonly IActionManager _actions;
        private readonly IPedInfoService _pedInfo;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ITickManager _ticks;
        private readonly IQuestionService _questionService;
        private readonly ISpeechService _speech;
        private readonly IOptionsManager _options;
        private readonly ICurrentTicketService _currentTicket;
        private readonly INotificationService _notifications;
        private readonly ILogger<PedInteractionMenuScript> _logger;

        #endregion Dependencies

        #region Menus

        private Menu _MainMenu;
        private Menu _arrestsMenu;
        private Menu _interactionsMenu;
        private Menu _questionsMenu;
        private Menu _ticketMenu;
        private Menu _offencesMenu;

        #endregion Menus

        #region Arrests Menu Buttons

        private MenuItem _jail;
        private MenuItem _follow;
        private MenuItem _kneel;
        private MenuItem _handsUp;
        private MenuItem _lieDown;
        private MenuItem _stand;

        #endregion Arrests Menu Buttons

        #region Main Menu Buttons

        private MenuItem _warning;
        private MenuItem _release;

        #endregion Main Menu Buttons

        #region Question Menu Buttons

        private List<MenuItem> _questionMenuItems;

        #endregion Question Menu Buttons

        #region Interactions Buttons

        private MenuItem _breathalyse;
        private MenuItem _drugalyse;
        private MenuItem _search;
        private MenuItem _askForId;

        #endregion Interactions Buttons

        #region IPedInteractionMenuScript Interface

        public async Task InteractWith(Ped ped)
        {
            if (ped.CurrentVehicle != null && !await ped.CurrentVehicle.TryRequestNetworkEntityControl())
            {
                return;
            }

            if (!await ped.TryRequestNetworkEntityControl())
            {
                MenuController.DisableBackButton = true;
                return;
            }

            _MainMenu.OpenMenu();

            _targetPed = ped;
            _targetPed.State.Set("isInteracting", true, true);
            if (_targetPed.GetIsNameKnown())
            {
                var pedInfo = await _pedInfo.GetByNetworkId(_targetPed.NetworkId);
                _MainMenu.MenuTitle = pedInfo.FullName;
                _arrestsMenu.MenuTitle = pedInfo.FullName;
                _interactionsMenu.MenuTitle = pedInfo.FullName;
                _questionsMenu.MenuTitle = pedInfo.FullName;
            }
            else
            {
                _MainMenu.MenuTitle = "Unknown Ped";
                _arrestsMenu.MenuTitle = "Unknown Ped";
                _interactionsMenu.MenuTitle = "Unknown Ped";
                _questionsMenu.MenuTitle = "Unknown Ped";
            }

            _speech.Say(Game.PlayerPed, "I'd like a word.");

            _targetPed.CanPlayAmbientAnims(false);
            _targetPed.BlockPermanentEvents = true;
            API.TaskSetBlockingOfNonTemporaryEvents(_targetPed.Handle, true);

            if (!_targetPed.IsInVehicle())
            {
                _targetPed.Task.ClearAll();
                await _targetPed.StandStillFacingPlayer();
                MenuController.DisableBackButton = true;
            }

            await _actions.Execute(new ObservePed(Game.PlayerPed, _targetPed));
            MenuController.DisableBackButton = true;

            await Delay(500);
            MenuController.DisableBackButton = true;
            _speech.SayRandomGreeting(_targetPed);
        }

        public bool IsMenuActive()
        {
            return _MainMenu.Visible ||
                   _interactionsMenu.Visible ||
                   _arrestsMenu.Visible ||
                   _questionsMenu.Visible ||
                   _ticketMenu.Visible ||
                   _offencesMenu.Visible;
        }

        #endregion IPedInteractionMenuScript Interface

        public PedInteractionMenuScript(IActionManager actions,
            IPedInfoService pedInfo,
            ILegacyClientCommunicationsManager comms,
            ITickManager ticks,
            IQuestionService questionService,
            ISpeechService speech,
            IOptionsManager options,
            ICurrentTicketService currentTicket,
            INotificationService notifications,
            ILogger<PedInteractionMenuScript> logger)
        {
            _actions = actions;
            _pedInfo = pedInfo;
            _comms = comms;
            _ticks = ticks;
            _questionService = questionService;
            _speech = speech;
            _options = options;
            _currentTicket = currentTicket;
            _notifications = notifications;
            _logger = logger;
        }

        protected override async Task OnStartAsync()
        {
            await CreateMenuAsync();

            _comms.On<int>(ClientEvents.LearnedName, OnLearnedName);
            _comms.On(ClientEvents.ActionExecuteStart, UpdateButtons);
            _comms.On(ClientEvents.ActionExecuteEnd, UpdateButtons);

            _ticks.On(PedInteractionMenuTick);
        }

        #region Event Handlers

        private async Task OnLearnedName(int networkId)
        {
            if (_targetPed == null || _targetPed.NetworkId != networkId) return;

            var pedInfo = await _pedInfo.GetByNetworkId(networkId);
            if (pedInfo == null) return;

            _MainMenu.MenuTitle = pedInfo.FullName;
            _arrestsMenu.MenuTitle = pedInfo.FullName;
            _interactionsMenu.MenuTitle = pedInfo.FullName;
            _questionsMenu.MenuTitle = pedInfo.FullName;
        }

        #endregion Event Handlers

        #region Ticks

        private async Task PedInteractionMenuTick()
        {
            if (_targetPed != null)
            {
                if (!Game.PlayerPed.IsCloseEnoughToEntity(_targetPed, 5f))
                {
                    _MainMenu.CloseMenu();
                    _arrestsMenu.CloseMenu();
                    _interactionsMenu.CloseMenu();
                    _questionsMenu.CloseMenu();
                    _ticketMenu.CloseMenu();
                    _offencesMenu.CloseMenu();
                }

                await Delay(1000);
            }
        }

        #endregion Ticks

        #region Menus

        private async Task CreateMenuAsync()
        {
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            _MainMenu = new Menu("Person Interaction", "Main Menu");
            MenuController.AddMenu(_MainMenu);

            CreateArrestsSubmenu();
            CreateInteractionsSubmenu();
            await CreateQuestionsSubmenuAsync();
            CreateTicketSubmenu();

            _warning = new MenuItem("Issue Warning", "Issue a warning to the person.");
            _release = new MenuItem("~b~Release", "Release the person.");

            _MainMenu.AddMenuItem(_warning);
            _MainMenu.AddMenuItem(_release);

            _MainMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item == _release)
                {
                    await _actions.Execute(new ReleasePed(Game.PlayerPed, _targetPed));
                    _MainMenu.CloseMenu();
                }
                else if (item == _warning) await _actions.Execute(new WarnPed(Game.PlayerPed, _targetPed));
            };

            _MainMenu.OnMenuOpen += menu =>
            {
                UpdateButtons();
            };

            _MainMenu.OnMenuClose += async menu =>
            {
                await Delay(5);

                if (_arrestsMenu?.Visible == true) return;
                if (_interactionsMenu?.Visible == true) return;
                if (_questionsMenu?.Visible == true) return;
                if (_offencesMenu?.Visible == true) return;
                if (_ticketMenu?.Visible == true) return;

                _currentTicket.Clear();

                // Make sure the ped gets ungrabbed if the menu is closed
                await _actions.Execute(new Grab(_targetPed, true), true);

                //_targetPed?.ReleaseInteractionLock();
                _targetPed.State.Set("isInteracting", false, true);
                _targetPed = null;
            };
        }

        private void CreateInteractionsSubmenu()
        {
            _interactionsMenu = new Menu("Person Interaction", "Interactions");
            MenuController.AddSubmenu(_MainMenu, _interactionsMenu);

            var interactionsMenuBtn = new MenuItem("Interactions", "Open the interactions sub-menu.") { Label = "→→→" };
            _MainMenu.AddMenuItem(interactionsMenuBtn);
            MenuController.BindMenuItem(_MainMenu, _interactionsMenu, interactionsMenuBtn);

            _askForId = new MenuItem("Ask for ID", "Ask the person to provide some form of identification.");
            _breathalyse = new MenuItem("Breathalyse",
                "Breathalyse the person to see whether they are below the drink drive limit.");
            _drugalyse = new MenuItem("Drugalyse",
                "Drugalyse the person to see whether they have consumed any illegal substances.");
            _search = new MenuItem("Search", "Pat the person down to find out what they are carrying.");

            _interactionsMenu.AddMenuItem(_askForId);
            _interactionsMenu.AddMenuItem(_breathalyse);
            _interactionsMenu.AddMenuItem(_drugalyse);
            _interactionsMenu.AddMenuItem(_search);

            _interactionsMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item == _askForId) await _actions.Execute(new AskForId(Game.PlayerPed, _targetPed));
                else if (item == _breathalyse) await _actions.Execute(new Breathalyse(Game.PlayerPed, _targetPed));
                else if (item == _drugalyse) await _actions.Execute(new Drugalyse(Game.PlayerPed, _targetPed));
                else if (item == _search) await _actions.Execute(new SearchPed(Game.PlayerPed, _targetPed));
            };

            _interactionsMenu.OnMenuOpen += menu =>
            {
                UpdateButtons();
            };
        }

        private async Task CreateQuestionsSubmenuAsync()
        {
            var questions = await _questionService.GetAllAsync();
            _questionMenuItems = new List<MenuItem>();

            _questionsMenu = new Menu("Person Interaction", "Questions");
            MenuController.AddSubmenu(_MainMenu, _questionsMenu);

            var questionsMenuBtn = new MenuItem("Questions", "Open the questions sub-menu.") { Label = "→→→" };
            _MainMenu.AddMenuItem(questionsMenuBtn);
            MenuController.BindMenuItem(_MainMenu, _questionsMenu, questionsMenuBtn);

            _questionsMenu.OnMenuOpen += async (Menu menu) =>
            {
                _questionsMenu.ClearMenuItems();
                // Generate the menu on-the-fly
                PedInfo pedInfo = await _pedInfo.GetByNetworkId(_targetPed.NetworkId);
                questions = await _questionService.GetAllAsync(pedInfo.QuestionList);
                _questionMenuItems = new List<MenuItem>();
                foreach (var question in questions)
                {
                    var questionItem = new MenuItem(question.Text);
                    _questionsMenu.AddMenuItem(questionItem);
                    _questionMenuItems.Add(questionItem);
                }
            };

            _questionsMenu.OnItemSelect += async (menu, item, index) =>
            {
                var question = questions.FirstOrDefault(x => x.Text.Equals(item.Text));
                if (question != null)
                {
                    await _actions.Execute(new QuestionPed(question, _targetPed));
                }
            };
        }

        private void CreateArrestsSubmenu()
        {
            _arrestsMenu = new Menu("Person Interaction", "Commands and Arrests");
            MenuController.AddSubmenu(_MainMenu, _arrestsMenu);

            var arrestsMenuBtn = new MenuItem("Commands and Arrests", "Open the arrests sub-menu.") { Label = "→→→" };
            _MainMenu.AddMenuItem(arrestsMenuBtn);
            MenuController.BindMenuItem(_MainMenu, _arrestsMenu, arrestsMenuBtn);

            _jail = new MenuItem("Jail", "Put the person into a nearby jail.");
            _follow = new MenuItem("Follow Me", "Command the person to follow you.");
            _kneel = new MenuItem("Kneel", "Command the person to kneel.");
            _handsUp = new MenuItem("Hands Up", "Command the person to put their hands up.");
            _lieDown = new MenuItem("Lie Down", "Command the person to lie on the ground.");
            _stand = new MenuItem("Stand Up", "Command the person to stand up.");

            _arrestsMenu.AddMenuItem(_jail);
            _arrestsMenu.AddMenuItem(_follow);
            _arrestsMenu.AddMenuItem(_kneel);
            _arrestsMenu.AddMenuItem(_handsUp);
            _arrestsMenu.AddMenuItem(_lieDown);
            _arrestsMenu.AddMenuItem(_stand);

            _arrestsMenu.OnMenuOpen += menu =>
            {
                UpdateButtons();
            };

            _arrestsMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item == _follow) await _actions.Execute(new Follow(Game.PlayerPed, _targetPed));
                else if (item == _kneel) await _actions.Execute(new Kneel(_targetPed));
                else if (item == _handsUp) await _actions.Execute(new HandsUp(_targetPed));
                else if (item == _lieDown) await _actions.Execute(new LieDown(_targetPed));
                else if (item == _stand) await _actions.Execute(new StandUp(_targetPed));
                else if (item == _jail) await _actions.Execute(new JailPed(_targetPed));
            };
        }

        private void CreateTicketSubmenu()
        {
            _ticketMenu = new Menu("Person Interaction", "Fixed Penalty Notice");
            MenuController.AddSubmenu(_MainMenu, _ticketMenu);

            var ticketMenuBtn = new MenuItem("Issue Fixed Penalty Notice", "Issue a Fixed Penalty Notice to the person.") { Label = "→→→" };
            _MainMenu.AddMenuItem(ticketMenuBtn);
            MenuController.BindMenuItem(_MainMenu, _ticketMenu, ticketMenuBtn);

            var fineAmount = new MenuItem("Fine",
                "The amount in pounds that the person will be fined.")
            {
                Label = $"£{_currentTicket.Ticket.TotalFine}.00"
            };

            var pointsAmount = new MenuItem("Points",
                "The amount of points that will be issued to the person.")
            {
                Label = $"{_currentTicket.Ticket.TotalPoints}"
            };

            var issue = new MenuItem("Issue", "Issue the Fixed Penalty Notice.");

            _ticketMenu.AddMenuItem(fineAmount);
            _ticketMenu.AddMenuItem(pointsAmount);
            _ticketMenu.AddMenuItem(issue);

            CreateOffencesSubmenu();

            _ticketMenu.OnMenuOpen += menu =>
            {
                fineAmount.Label = $"£{_currentTicket.Ticket.TotalFine}.00";
                pointsAmount.Label = $"{_currentTicket.Ticket.TotalPoints}";
                issue.Enabled = _currentTicket.Ticket.HasAnyPointsOrFine;
            };

            _ticketMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item != issue) return;

                await _actions.Execute(new TicketPed(_targetPed, _currentTicket.Ticket));
            };
        }

        private void CreateOffencesSubmenu()
        {
            _offencesMenu = new Menu("Fixed Penalty Notice", "Select Offences");
            MenuController.AddSubmenu(_ticketMenu, _offencesMenu);

            var offencesMenuBtn =
                new MenuItem("Select Offences", "Select the offences that are to be given to the person.")
                { Label = "→→→" };
            _ticketMenu.AddMenuItem(offencesMenuBtn);
            MenuController.BindMenuItem(_ticketMenu, _offencesMenu, offencesMenuBtn);

            var categoriesList = new MenuListItem("Category",
                new List<string> { "Endorsable", "Non-Endorsable", "Anti-Social" },
                0, "The category of offence.");

            _offencesMenu.AddMenuItem(categoriesList);

            AddOffenceCheckboxes(categoriesList.ListItems[0]);

            _offencesMenu.OnMenuOpen += menu =>
            {
                AddOffenceCheckboxes(categoriesList.ListItems[0]);
            };

            _offencesMenu.OnListIndexChange += (menu, item, index, selectionIndex, itemIndex) =>
            {
                if (item == categoriesList)
                {
                    AddOffenceCheckboxes(categoriesList.ListItems[selectionIndex]);
                }
            };

            _offencesMenu.OnCheckboxChange += (menu, item, index, newCheckedState) =>
            {
                if (newCheckedState)
                {
                    if (_currentTicket.Ticket.Offences.Count >= _options.Options.Offence.MaxOffences)
                    {
                        _notifications.Error("Fixed Penalty Notice",
                            $"You can only give a maximum of {_options.Options.Offence.MaxOffences} offences.");
                        item.Checked = false;
                        return;
                    }

                    _currentTicket.Ticket.Offences
                        .Add(_options.Options.Offence.Offences
                            .FirstOrDefault(offence => offence.Name == item.Text));
                }
                else
                {
                    _currentTicket.Ticket.Offences.RemoveAll(offence => offence.Name == item.Text);
                }
            };
        }

        private void AddOffenceCheckboxes(string offenceCategory)
        {
            var oldOffenceBoxes = _offencesMenu.GetMenuItems().OfType<MenuCheckboxItem>();
            foreach (var oldOffenceBox in oldOffenceBoxes)
                _offencesMenu.RemoveMenuItem(oldOffenceBox);

            var offences = _options.Options.Offence.Offences
                .Where(o => o.Category.Equals(offenceCategory));

            foreach (var offence in offences)
            {
                var offenceBox = new MenuCheckboxItem(offence.Name)
                {
                    Style = MenuCheckboxItem.CheckboxStyle.Tick,
                    Checked = _currentTicket.Ticket.Offences.Any(x => x.Name == offence.Name)
                };

                _offencesMenu.AddMenuItem(offenceBox);
            }
        }

        #endregion Menus

        private void UpdateButtons()
        {
            if (_targetPed == null) return;

            bool canExecute = _actions.CanExecute();
            bool isCuffed = _targetPed.IsCuffed;
            bool isKneeling = _targetPed.State.Get<bool>(PedStates.IsKneeling);
            bool isLyingDown = _targetPed.State.Get<bool>(PedStates.IsLyingDown);
            bool isFollowingPlayer = _targetPed.State.Get<bool>(PedStates.IsFollowingPlayer);

            _follow.Enabled = canExecute;
            _kneel.Enabled = canExecute;
            _handsUp.Enabled = canExecute;
            _lieDown.Enabled = canExecute;
            _stand.Enabled = canExecute;
            _warning.Enabled = canExecute;
            _release.Enabled = canExecute;
            _breathalyse.Enabled = canExecute;
            _drugalyse.Enabled = canExecute;
            _search.Enabled = canExecute;
            _askForId.Enabled = canExecute;
            _jail.Enabled = canExecute;
            _questionMenuItems.ForEach(x => x.Enabled = canExecute);

            _follow.Text = isFollowingPlayer ? "Stop Following Me" : "Follow Me";

            if (canExecute)
            {
                _follow.Enabled = !isCuffed;
                _handsUp.Enabled = !isCuffed;
                _jail.Enabled = isCuffed;

                _stand.Enabled = !isCuffed && (isKneeling || isLyingDown);
                _kneel.Enabled = !isCuffed && !isKneeling;
                _lieDown.Enabled = !isCuffed && !isLyingDown;
            }
        }
    }
}
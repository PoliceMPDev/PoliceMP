using CitizenFX.Core.Native;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Client.Overlays.Interaction;
using PoliceMP.Client.Overlays.NewNotification;

namespace PoliceMP.Client.Actions.BikePickup
{
    public class BikePickupHandler : ActionHandler<BikePickup>
    {
        private readonly ITickManager _ticks;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly IGameInputManager _gameInputManager;
        private readonly IInstructionalButtonsService _instructionalButtonsService;
        private readonly IInteractionHud _interactionHud;
        private readonly ILogger<BikePickupHandler> _logger;

        private Vehicle _bike;

        public BikePickupHandler(ITickManager ticks, INewNotificationOverlay newNotificationOverlay, IGameInputManager gameInputManager, ILogger<BikePickupHandler> logger, IInstructionalButtonsService instructionalButtonsService, IInteractionHud interactionHud)
        {
            _ticks = ticks;
			_newNotificationOverlay = newNotificationOverlay;
			_gameInputManager = gameInputManager;
            _logger = logger;
            _instructionalButtonsService = instructionalButtonsService;
            _interactionHud = interactionHud;
        }

        protected override async Task<bool> Handle(BikePickup action)
        {
            _logger.Debug("Trying to pick up bike");

            if(action.Target.Driver.Exists())
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Bike Pickup", "error", "This bike has someone on it!", new NewNotificationMessageContent[0]));
                return false;
            }

            await action.Target.TryRequestNetworkEntityControl();

            if (!action.Target.HasNetworkControl())
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Bike Pickup", "error", "Unable to gain control of the bike.", new NewNotificationMessageContent[0]));
                return false;
            }

            _logger.Debug("Got control attaching");

            action.Target.AttachTo(action.Subject, position: new Vector3(0f, -0.2f, 0f),rotation: new Vector3(90f, 90f, 0f));
            action.Target.IsCollisionEnabled = false;

            _bike = action.Target;
            _ticks.On(HoldingBike);

            _logger.Debug("Dunzo");
            return true;
        }

        private async Task HoldingBike()
        {
            
            if (!_bike.Exists())
            {
                _ticks.Off(HoldingBike);
                return;
            }

            if(!_instructionalButtonsService.IsButtonShown("Drop Bike"))
            {
                await _instructionalButtonsService.AddInstructionalButton("Drop Bike", Control.Enter);
            }

            if (!_gameInputManager.IsPressed(Control.Enter))
            {
                return;
            }

            await _instructionalButtonsService.RemoveInstructionalButton("Drop Bike");
            
            _logger.Debug("Ticking");

            _bike.Detach();
            _bike.IsCollisionEnabled = true;
            _ticks.Off(HoldingBike);
        }
    }
}

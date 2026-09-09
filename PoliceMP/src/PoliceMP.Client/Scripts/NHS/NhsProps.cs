using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using CitizenFX.Core.UI;
using MenuAPI;
using PoliceMP.Client.Actions.Defib;
using PoliceMP.Client.Extensions;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Properties;
using PoliceMP.Client.Scripts.Afk;
using PoliceMP.Client.Scripts.Weapons;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.NHS
{
    public class NhsProps : Script
    {
        private ILogger<NhsProps> _logger;
        private ICommandManager _commandManager;
        private IPermissionService _permissionService;
        private UserAces _userAces;
        private ITickManager _tickManager;
        private readonly IPlayerListAccessor _playerListAccessor;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        
        private const string DEFIB_MODEL = "w_am_ecg";


        public NhsProps(ILogger<NhsProps> logger, ICommandManager commandManager, IPermissionService permissionService, ITickManager tickManager, IPlayerListAccessor playerListAccessor, INewNotificationOverlay newNotificationOverlay)
        {
            _logger = logger;
            _commandManager = commandManager;
            _permissionService = permissionService;
            _tickManager = tickManager;
            _playerListAccessor = playerListAccessor;
            _newNotificationOverlay = newNotificationOverlay;

            _tickManager.On(CastProps);

        }

        public async Task CastProps()
        {
            int _defib = API.GetHashKey("DEFIB_MODEL"); // Name of prop thats placed on floor goes here, from this we can determine it allows player to do X medical function (Thinking using vetors to determine if item is close enough to use on patient
            if (!API.DoesEntityExist(_defib))
            {
                return;
            }
            else
            {
              DefibAction();
              await Delay(0);
            }

            int _Pcare = API.GetHashKey("prop_primarybagopen");
            if (!API.DoesEntityExist(_Pcare))
            {
                return;
            }
            else
            {
                PcareAction();
                await Delay(0);
            }

        }

        public void DefibAction()
        {
            _logger.Debug("Defib unit detected");
            // We will need to have a think about how we want to inject contextual actions or run them in here via a menu? 

        }

        public void PcareAction()
        {
            _logger.Debug("Pcare bag detected");

        }


































    }
}

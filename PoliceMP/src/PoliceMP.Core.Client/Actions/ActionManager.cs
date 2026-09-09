using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared.Constants;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Core.Client.Actions
{
    public class ActionManager : IActionManager
    {
        private const int CooldownMs = 2000;
        private readonly ConcurrentDictionary<Type, IActionHandler> _actionHandlers;
        private DateTime _lastActionTime = DateTime.MinValue;
        private volatile bool _isHandlingAction;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<ActionManager> _logger;

        public ActionManager(ILegacyClientCommunicationsManager comms, ILogger<ActionManager> logger)
        {
            _comms = comms;
            _logger = logger;
            _actionHandlers = new ConcurrentDictionary<Type, IActionHandler>();
        }

        public bool CanExecute()
        {
            return /*!IsOnCooldown() && */!_isHandlingAction;
        }

        public async Task<bool> Execute(IAction action, bool force = false)
        {
            if (!force && !CanExecute())
            {
                _logger.Debug($"Failed to execute action {action.GetType()}");
                return false;
            }

            if (!_actionHandlers.TryGetValue(action.GetType(), out var actionHandler))
            {
                throw new ActionException($"Couldn't find the action handler for type {action.GetType()}.");
            }

            _logger.Debug($"Away to handle action {action.GetType()}");
            _lastActionTime = DateTime.Now;
            _isHandlingAction = true;
            _comms.ToClient(ClientEvents.ActionExecuteStart);
            bool result = false;
            try
            {
                result = await actionHandler.Handle(action);
            }
            catch
            {
                _logger.Error($"An error occurred while handling action {action.GetType()}...");
                throw;
            }
            finally
            {
                _isHandlingAction = false;
                _comms.ToClient(ClientEvents.ActionExecuteEnd);
                _logger.Debug($"Finished handling action {action.GetType()}");

            }

            return result;
        }

        public void AddHandler(IActionHandler actionHandler)
        {
            var type = actionHandler.GetActionType();
            if (!_actionHandlers.TryAdd(type, actionHandler))
            {
                throw new ActionException($"Failed to add handler for action {type} because one already exists for it.");
            }
        }

        private bool IsOnCooldown()
        {
            return (DateTime.Now - _lastActionTime).TotalMilliseconds < CooldownMs;
        }
    }
}
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Computer.Client.Controllers;
using PoliceMP.Computer.Client.Handlers;
using PoliceMP.Computer.Client.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoliceMP.Computer.Client
{
    public class Main : BaseScript
    {
        /// <summary>
        /// The instance of Main so it can be used in the static methods
        /// to expose BaseScript methods to be used by non-BaseScript classes.
        /// </summary>
        private static Main _instance;

        /// <summary>
        /// Handles communication with NUI.
        /// </summary>
        private readonly NuiHandler _nui;

        /// <summary>
        /// Handles registering and deregistering ticks for non-BaseScripts.
        /// </summary>
        private readonly TickHandler _ticks;

        /// <summary>
        /// Event system for non-BaseScripts.
        /// </summary>
        private readonly CommunicationsHandler _comms;

        /// <summary>
        /// Handles RPC communications to request data from the server.
        /// </summary>
        private readonly RpcHandler _rpc;

        /// <summary>
        /// For logging stuff.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The main controller.
        /// </summary>
        private readonly DesktopController _desktopController;

        /// <summary>
        /// The callouts app controller.
        /// </summary>
        private readonly CalloutController _calloutController;

        /// <summary>
        /// Creates a new instance of Main.
        /// </summary>
        public Main()
        {
            _instance = this;

            _logger = new Logger(true);

            _nui = new NuiHandler();
            _ticks = new TickHandler(_logger);
            _comms = new CommunicationsHandler(_logger);
            _rpc = new RpcHandler(_logger);

            _desktopController = new DesktopController(_logger, _nui, _ticks, _comms, _rpc);
            _calloutController = new CalloutController(_logger, _nui, _ticks, _comms, _rpc);
        }

        /// <summary>
        /// Called when the client resource starts.
        /// </summary>
        [EventHandler("onClientResourceStart")]
        private void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            _desktopController.Toggle(false);
        }

        /// <summary>
        /// Receives the RPC result.
        /// </summary>
        /// <param name="requestGuid">The request GUID.</param>
        /// <param name="jsonString">The requested JSON data.</param>
        [EventHandler("PoliceMP:ReceiveRequestResult")]
        private void OnReceiveRequestResult(string requestGuid, string jsonString)
        {
            _rpc.ReceiveRequestResult(requestGuid, jsonString);
        }

        /// <summary>
        /// Tell the callout controller to update the callout data when
        /// a new one comes in.
        /// </summary>
        [EventHandler("Callouts:ReceiveCalloutNotification")]
        private async void OnReceiveCalloutNotification(int id, string title, string description, Vector3 location)
        {
            await _calloutController.UpdateCalloutData();
        }

        /// <summary>
        /// Tell the callout controller to update the callout data
        /// when one has ended.
        /// </summary>
        [EventHandler("Callouts:CalloutEnded")]
        private void OnCalloutEnded(int calloutId)
        {
            _calloutController.CalloutEnded(calloutId);
        }

        [EventHandler("Callouts:JoinCallout")]
        private void OnJoinCallout(int calloutId, string title, string description, Vector3 location)
        {
            _logger.Error("OnJoinCallout");
            _calloutController.JoinedCallout(calloutId);
        }

        /// <summary>
        /// Show the computer dev command.
        /// </summary>
        [Command("comp")]
        private void Cmd_Comp()
        {
            _logger.Log("comp command");
            TriggerEvent("PoliceComputer:Toggle", true);
        }

        /// <summary>
        /// Enable/disable the computer display.
        /// </summary>
        /// <param name="enable">Whether to enable the display.</param>
        [EventHandler("PoliceComputer:Toggle")]
        private void OnToggle(bool enable)
        {
            _desktopController.Toggle(enable);

            if (enable)
                _comms.ToClient("ComputerOpened");
            else
                _comms.ToClient("ComputerClosed");
        }

        /// <summary>
        /// Register a NUI callback. This exists so NuiHandler can use it without
        /// inheriting from BaseScript.
        /// </summary>
        /// <param name="eventName">The NUI event name.</param>
        /// <param name="callback">The function to be executed when the event is triggered.</param>
        public static void RegisterNuiCallback(string eventName, Action<IDictionary<string, object>, CallbackDelegate> callback)
        {
            _instance.EventHandlers[$"__cfx_nui:{eventName}"] += callback;
        }

        /// <summary>
        /// Allows non-BaseScripts to add a tick handler. Used by the
        /// TickHandler class.
        /// </summary>
        public static void AddTickHandler(Func<Task> handler)
        {
            _instance.Tick += handler;
        }

        /// <summary>
        /// Allows non-BaseScripts to remove a tick handler. Used by the
        /// TickHandler class.
        /// </summary>
        /// <param name="handler"></param>
        public static void RemoveTickHandler(Func<Task> handler)
        {
            _instance.Tick -= handler;
        }

        public List<Player> GetPlayers()
        {
            return _instance.Players.ToList();
        }
    }
}

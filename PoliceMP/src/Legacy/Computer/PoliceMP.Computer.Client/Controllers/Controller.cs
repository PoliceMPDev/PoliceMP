using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Computer.Client.Handlers;
using PoliceMP.Computer.Client.Util;
using System.Threading.Tasks;

namespace PoliceMP.Computer.Client.Controllers
{
    public abstract class Controller
    {
        protected readonly ILogger _logger;
        protected readonly NuiHandler _nui;
        protected readonly TickHandler _ticks;
        protected readonly CommunicationsHandler _comms;
        protected readonly RpcHandler _rpc;

        public Controller(ILogger logger, NuiHandler nui, TickHandler ticks, CommunicationsHandler comms, RpcHandler rpc)
        {
            _logger = logger;
            _nui = nui;
            _ticks = ticks;
            _comms = comms;
            _rpc = rpc;

            _comms.On("ComputerClosed", OnComputerClosed);
            _comms.On("ComputerOpened", OnComputerOpened);
        }

        private void OnComputerOpened()
        {
            _ticks.On(OnTick);
        }

        private void OnComputerClosed()
        {
            _ticks.Off(OnTick);
        }

        protected virtual async Task OnTick()
        {
        }

        /// <summary>
        /// Send some JSON data to the interface.
        /// </summary>
        /// <param name="data">JSON data.</param>
        protected void Emit(object data)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Set whether to focus the UI.
        /// </summary>
        /// <param name="hasFocus">Whether the UI should be focussed.</param>
        /// <param name="showCursor">Whether to show the cursor.</param>
        protected void Focus(bool hasFocus, bool showCursor)
        {
            API.SetNuiFocus(hasFocus, showCursor);
        }
    }
}

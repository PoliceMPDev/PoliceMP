using CitizenFX.Core;
using PoliceMP.Computer.Client.Handlers;
using PoliceMP.Computer.Client.Util;
using System.Collections.Generic;

namespace PoliceMP.Computer.Client.Controllers
{
    public class DesktopController : Controller
    {
        public DesktopController(ILogger logger, NuiHandler nui, TickHandler ticks, CommunicationsHandler comms, RpcHandler rpc) : base(logger, nui, ticks, comms, rpc)
        {
            _nui.On("hide", OnHide);
        }

        public void Toggle(bool enable)
        {
            _logger.Log($"Toggle: {enable}");

            if (enable)
            {
                Focus(true, true);
                Emit(new
                {
                    cmd = "show"
                });
            }
            else
            {
                Focus(false, false);
                Emit(new
                {
                    cmd = "hide"
                });
            }
        }

        /// <summary>
        /// Called when NUI requests to hide the computer.
        /// </summary>
        private void OnHide(IDictionary<string, object> data, CallbackDelegate callback)
        {
            _logger.Log("OnHide");
            BaseScript.TriggerEvent("PoliceComputer:Toggle", false);
            callback();
        }
    }
}

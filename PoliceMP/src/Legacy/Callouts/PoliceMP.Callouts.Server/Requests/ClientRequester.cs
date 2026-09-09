using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Callouts.Shared.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceMP.Callouts.Server.Requests
{
    public class ClientRequester : BaseScript
    {
        private static Dictionary<Guid, Request> _requests = new Dictionary<Guid, Request>();

        public static async Task<T> Request<T>(Player player, string eventName, params object[] args)
        {
            var request = new Request();
            _requests.Add(request.Guid, request);

            Debug.WriteLine($"[ClientRequester] Request for {eventName} sent");

            player.TriggerEvent(eventName, request.Guid.ToString(), args);

            while (!request.IsCompleted)
            {
                await Delay(50);
                if ((DateTime.Now - request.RequestTime).TotalSeconds > 5)
                {
                    return default;
                }
            }

            var result = JsonConvert.DeserializeObject<T>(request.JsonResult);
            return result;
        }

        [EventHandler(ServerEvents.RECEIVE_REQUEST_RESULT)]
        private void ReceiveRequestResult(string guid, string jsonResult)
        {
            var parsedGuid = Guid.Parse(guid);
            if (_requests.ContainsKey(parsedGuid))
            {
                var request = _requests[parsedGuid];
                request.JsonResult = jsonResult;
                request.IsCompleted = true;

                Debug.WriteLine($"[ClientRequester] Request {guid} received. Result: {jsonResult}");
            }
        }
    }

    public class Request
    {
        public Guid Guid { get; private set; }
        public string JsonResult { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime RequestTime { get; private set; }

        public Request()
        {
            Guid = Guid.NewGuid();
            JsonResult = string.Empty;
            IsCompleted = false;
            RequestTime = DateTime.Now;
        }
    }
}

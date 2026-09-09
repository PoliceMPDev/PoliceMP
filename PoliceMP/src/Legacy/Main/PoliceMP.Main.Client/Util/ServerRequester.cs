using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Main.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Util
{
    public class ServerReqduester : BaseScript
    {
        private static readonly List<ServerRequest> _requests = new List<ServerRequest>();

        [EventHandler(ClientEvents.RECEIVE_SERVER_REQUEST)]
        private void OnReceiveSuccessfulRequest(Guid guid, string json)
        {
            var request = _requests.ToList().FirstOrDefault(r => r.Guid == guid);
            if (request == null)
            {
                return;
            }

            request.JsonResult = json;
            request.IsSuccess = true;
            request.RequestComplete = true;
        }

        [EventHandler(ClientEvents.FAILED_SERVER_REQUEST)]
        private void OnReceiveFailedRequest(Guid guid)
        {
            var request = _requests.ToList().FirstOrDefault(r => r.Guid == guid);
            if (request == null)
            {
                return;
            }

            request.IsSuccess = false;
            request.RequestComplete = true;
        }

        public static async Task<T> Request<T>(string eventName, params object[] args)
        {
            var request = new ServerRequest()
            {
                Guid = Guid.NewGuid(),
                EventName = eventName,
                Args = args,
                RequestComplete = false,
                RequestTime = DateTime.Now

            };

            _requests.Add(request);

            TriggerServerEvent(eventName, request.Guid, args);

            while (!request.RequestComplete)
            {
                await Delay(100);

                var timeSince = DateTime.Now - request.RequestTime;
                if (timeSince.TotalSeconds > 2)
                {
                    request.IsSuccess = false;
                    return default;
                }
            }

            var result = JsonConvert.DeserializeObject<T>(request.JsonResult);

            return result;
        }
    }

    public class ServerRequest
    {
        public Guid Guid { get; set; }
        public string EventName { get; set; }
        public object[] Args { get; set; }

        public bool RequestComplete { get; set; }
        public bool IsSuccess { get; set; }
        public string JsonResult { get; set; }

        public DateTime RequestTime { get; set; }
    }
}

using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Computer.Client.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceMP.Computer.Client.Handlers
{
    public class RpcHandler
    {
        private Dictionary<Guid, RequestData> _requests = new Dictionary<Guid, RequestData>();

        private readonly ILogger _logger;

        public RpcHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<T> Request<T>(string eventName, params object[] args)
        {
            var request = new RequestData();
            _requests.Add(request.Guid, request);

            _logger.Log($"[Rpc] Request for {eventName} sent");

            BaseScript.TriggerServerEvent(eventName, request.Guid.ToString(), args);

            while (!request.IsCompleted)
            {
                await BaseScript.Delay(50);
                if ((DateTime.Now - request.RequestTime).TotalSeconds > 5)
                {
                    return default;
                }
            }

            _logger.Log($"[Rpc] Request received");

            var result = JsonConvert.DeserializeObject<T>(request.JsonResult);
            return result;
        }

        public void ReceiveRequestResult(string guid, string jsonResult)
        {
            var parsedGuid = Guid.Parse(guid);
            if (_requests.ContainsKey(parsedGuid))
            {
                var request = _requests[parsedGuid];
                request.JsonResult = jsonResult;
                request.IsCompleted = true;

                _logger.Log($"[Rpc] Request {guid} received. Result: {jsonResult}");
            }
        }

        private class RequestData
        {
            public Guid Guid { get; private set; }
            public string JsonResult { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime RequestTime { get; private set; }

            public RequestData()
            {
                Guid = Guid.NewGuid();
                JsonResult = string.Empty;
                IsCompleted = false;
                RequestTime = DateTime.Now;
            }
        }
    }
}

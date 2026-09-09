using System;
using System.Linq;
using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Callouts.Server.Enums;
using PoliceMP.Core.Shared.Communications;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class BackupRequestCallout : Callout
    {
        private BackupRequest _backupRequest { get; set; }

        private static DateTime? _lastLfbMobiliseSounds { get; set; } = null;
        
        public BackupRequestCallout(int id, BackupRequest backupRequest) : base(id)
        {
            _backupRequest = backupRequest;

            var attr = _backupRequest.Type.GetCustomAttribute<BackupItemAttribute>();
            
            Title = $"Request for {attr.Description}";
            Grade = attr.Grade;
            Description = $"Requested by {_backupRequest.Name}";
            LocationSetting = LocationSetting.Manual;
            Location = new Vector3(backupRequest.CoordsX, backupRequest.CoordsY, backupRequest.CoordsZ);
        }
    
        public override void Setup()
        {
            base.Setup();
        }
        
        public override void Update()
        {
            if (Status != CalloutStatus.Initial || HasAnyPlayer())
            {
                return;
            }
            
            var secondsSinceCreation = (DateTime.Now - TimeCreated).TotalSeconds;
            
            if (secondsSinceCreation > 60)
            {
                End(CalloutResult.NoPlayerJoined);
            }
        }

        public override void OnPlayerArrived(Player player)
        {
            base.OnPlayerArrived(player);
            ClientEventAPI.ShowSubtitle(player, $"You are now state 6 to this ~y~{_backupRequest.Type}~w~.", 5000);
        }

        public void RpcEmit(string @event, params object[] args)
        {
            Debug.WriteLine($"Emitting {@event} to server.");
            var message = new RpcMessage
            {
                Id = Guid.NewGuid(),
                Event = @event,
                Payload = args.Select(JsonConvert.SerializeObject).ToArray()
            };

            TriggerEvent(RpcConstants.RpcMessage, JsonConvert.SerializeObject(message));
        }
    }
}

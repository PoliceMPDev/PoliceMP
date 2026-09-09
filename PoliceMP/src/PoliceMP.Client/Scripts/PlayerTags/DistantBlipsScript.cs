using CitizenFX.Core;
using PoliceMP.Core.Client.Scripts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using PoliceMP.Client.Scripts.HideBlips;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Scripts.PlayerTags
{
    public class DistantBlipsScript : Script
    {
        private readonly ILogger<DistantBlipsScript> _log;
        private readonly IPermissionService _perms;

        private readonly StateBagProxy<IList<DistantBlip>> _distantBlipState;
        private readonly ConcurrentDictionary<string, Blip> _distantSprites = new ConcurrentDictionary<string, Blip>();

        public DistantBlipsScript(ILogger<DistantBlipsScript> log, IPermissionService perms, IGlobalStateAccessor globalState)
        {
            _log = log;
            _perms = perms;

            _distantBlipState = new GlobalStateBagProxy<IList<DistantBlip>>(globalState, GlobalStates.DistantBlips);
        }

        protected override Task OnStartAsync()
        {
            _distantBlipState.AddStateBagChangeHandler(OnDistantBlipStateChange);
            return Task.FromResult(0);
        }

        private async Task OnDistantBlipStateChange(IList<DistantBlip> _, IList<DistantBlip> distantBlips, bool replicated)
        {
            // CLEANUP
            foreach (var distantSprite in _distantSprites)
            {
                if (distantBlips.Any(c => c.PlayerServerHandle == distantSprite.Key)) continue;
                
                if (_distantSprites.TryRemove(distantSprite.Key, out var distantBlipSprite))
                {
                    distantBlipSprite.Delete();
                }
            }
            
            // UPDATE
            for (int i = 0; i < distantBlips.Count; i++)
            {
                var distantBlip = distantBlips[i];
                var playerHandle = distantBlip.PlayerServerHandle;
                var networkId = distantBlip.PlayerPedNetworkId;
                if (networkId <= 0 || networkId == Game.PlayerPed.NetworkId)
                {
                    continue;
                }
                
                var cloneExists = API.NetworkDoesEntityExistWithNetworkId(networkId);

                // Remove blip if clone exists (player is close)
                // Also remove all blips if player is civ
                if (cloneExists ||
                    _perms.CurrentUserRole?.Branch == UserBranch.Civ)
                {
                    if (_distantSprites.TryRemove(playerHandle, out var existingDistantBlip))
                    {
                        existingDistantBlip.Delete();
                    }

                    await Script.Delay(0);
                    continue;
                }


                if (!_distantSprites.TryGetValue(playerHandle, out var blip))
                {
                    //_log.Debug($"Creating distant blip for player {networkId}");
                    blip = World.CreateBlip(distantBlip.Position.ToCitizenVector3());
                    if (!_distantSprites.TryAdd(playerHandle, blip))
                    {
                        blip.Delete();
                        continue;
                    }
                }

                if (blip.Sprite != (BlipSprite) distantBlip.Sprite)
                    blip.Sprite = (BlipSprite) distantBlip.Sprite;

                if (blip.Color != (BlipColor)distantBlip.Color)
                    blip.Color = (BlipColor)distantBlip.Color;

                if (!blip.IsShortRange)
                    blip.IsShortRange = true;

                blip.Scale = distantBlip.Scale;
                blip.Position = distantBlip.Position.ToCitizenVector3();
                blip.Name = distantBlip.BlipName;

                await Script.Delay(0);
            }
        }
    }
}

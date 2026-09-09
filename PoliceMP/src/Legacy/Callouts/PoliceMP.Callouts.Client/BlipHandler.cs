using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Callouts.Shared.Events;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Callouts.Client
{
    public class BlipHandler : BaseScript
    {
        private static readonly List<Blip> _blips = new List<Blip>();

        [EventHandler(ClientEvents.REMOVE_ALL_BLIPS)]
        public static void RemoveAllBlips()
        {
            foreach (var blip in _blips)
            {
                blip.Delete();
            }

            _blips.Clear();
        }

        [EventHandler(ClientEvents.ADD_BLIP_FOR_COORD)]
        public static void AddBlipForCoord(int blipSprite, Vector3 location, int blipColor, bool showRoute, bool isShortRange, string name)
        {
            int blipId = API.AddBlipForCoord(location.X, location.Y, location.Z);

            var blip = new Blip(blipId)
            {
                Sprite = (BlipSprite)blipSprite,
                Color = (BlipColor)blipColor,
                ShowRoute = showRoute,
                IsShortRange = isShortRange,
                Name = name,
            };

            _blips.Add(blip);
        }

        [EventHandler(ClientEvents.ADD_BLIP_FOR_ENTITY)]
        public static void AddBlipForEntity(int networkId, int blipSprite, int blipColor, bool showRoute, bool isShortRange, string name, float scale)
        {
//            API.NetworkRequestControlOfNetworkId(networkId);

            var entityId = API.NetworkGetEntityFromNetworkId(networkId);

            if (entityId == 0) return;
            
//            API.NetworkRequestControlOfEntity(entityId);

            var blipId = API.AddBlipForEntity(entityId);

            var blip = new Blip(blipId)
            {
                Color = (BlipColor)blipColor,
                ShowRoute = showRoute,
                IsShortRange = isShortRange,
                Name = name,
                Scale = scale
            };

            _blips.Add(blip);
        }

        [EventHandler(ClientEvents.REMOVE_BLIP_FOR_ENTITY)]
        public static void RemoveBlipForEntity(int networkId)
        {
            var entityId = API.NetworkGetEntityFromNetworkId(networkId);

            var blip = _blips.FirstOrDefault(b => b.Handle == entityId);
            if (blip == null) return;

            blip.Delete();
            _blips.Remove(blip);
        }

        [EventHandler(ClientEvents.ADD_BLIP_FOR_RADIUS)]
        public static void AddBlipForRadius(int blipSprite, Vector3 location, float radius, int blipColor, bool showRoute, bool isShortRange, string name)
        {
            var newLoc = location + new Vector3(PoliceMpRandom.Next(10, 200), PoliceMpRandom.Next(10, 200), PoliceMpRandom.Next(10, 200));

            var blipId = API.AddBlipForRadius(newLoc.X, newLoc.Y, newLoc.Z, radius);

            var blip = new Blip(blipId)
            {
                //Sprite = (BlipSprite)blipSprite,
                Color = (BlipColor)blipColor,
                ShowRoute = showRoute,
                IsShortRange = isShortRange,
                Name = name,
                Alpha = 128
            };

            _blips.Add(blip);
        }
    }
}

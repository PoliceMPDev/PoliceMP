using CitizenFX.Core;

namespace PoliceMP.Server.Controllers.AiCallouts
{
    public class AiCalloutBlip
    {
        private readonly Vector2 _location;
        private readonly int? _icon;
        private readonly int? _radius;
        
        private AiCalloutBlip(Vector2 location, int? icon, int? radius)
        {
            _location = location;
            _icon = icon;
            _radius = radius;
        }

        public static AiCalloutBlip ForLocation(Vector2 location, int icon)
        {
            return new AiCalloutBlip(location, icon, null);
        }

        public static AiCalloutBlip ForRadius(Vector2 location, int radius)
        {
            return new AiCalloutBlip(location, null, radius);
        }

        public Vector2 GetLocation()
        {
            return _location;
        }

        public int? GetIcon()
        {
            return _icon;
        }

        public int? GetRadius()
        {
            return _radius;
        }
    }
}
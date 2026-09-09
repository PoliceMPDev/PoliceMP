using CitizenFX.Core.Native;
using PoliceMP.Core.Shared.Models;
using Vector3 = CitizenFX.Core.Vector3;

namespace PoliceMP.Core.Client.Abstraction
{
    public class TextDraw
    {
        // These fields are not set to "readonly" on purpose due
        // to an exception thrown OnTick:
        // "Cannot take the address of a init-only field at 0x000e"
        protected Vector3 _position;
        protected string _text;
        protected Color _color;

        public TextDraw(Vector3 position, string text, Color color = null)
        {
            _position = position;
            _text = text;
            _color = color ?? Color.White;
        }

        public virtual void Draw()
        {
            var screenX = 0f;
            var screenY = 0f;

            bool isOnScreen = API.World3dToScreen2d(
                _position.X,
                _position.Y,
                _position.Z,
                ref screenX,
                ref screenY);

            var gameplayCamCoords = API.GetGameplayCamCoords();

            float distance = API.GetDistanceBetweenCoords(
                gameplayCamCoords.X,
                gameplayCamCoords.Y,
                gameplayCamCoords.Z,
                _position.X,
                _position.Y,
                _position.Z,
                true);

            float scale = 1 / distance;
            float fov = 1 / API.GetGameplayCamFov() * 100;
            scale *= fov;

            if (isOnScreen)
            {
                API.SetTextScale(0f, scale);
                API.SetTextFont(0);
                API.SetTextProportional(true);
                API.SetTextColour(_color.R, _color.G, _color.B, _color.A);
                API.SetTextDropshadow(0, 0, 0, 0, 255);
                API.SetTextOutline();
                API.SetTextEntry("STRING");
                API.SetTextCentre(true);
                API.AddTextComponentString(_text);
                API.DrawText(screenX, screenY);
            }
        }
    }
}
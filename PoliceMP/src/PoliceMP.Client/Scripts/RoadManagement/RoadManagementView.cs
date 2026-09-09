using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils;
using PoliceMP.Client.Utils.CameraUtils;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.RoadManagement
{
    public class RoadManagementView : Script
    {
        private readonly ICommandManager _commands;
        private readonly ILegacyClientCommunicationsManager _legacyComms;
        private readonly INotificationService _notifications;
        private readonly ITickManager _ticks;
        private SpringArmCamera _camera = null;

        public RoadManagementView(
            ICommandManager commands,
            ILegacyClientCommunicationsManager legacyComms,
            INotificationService notifications,
            ITickManager ticks
        )
        {
            _commands = commands;
            _legacyComms = legacyComms;
            _notifications = notifications;
            _ticks = ticks;

            _camera = new SpringArmCamera(
                _ticks,
                Game.PlayerPed.Position,
                new Vector3(-90f, 0f, 0f),
                15f,
                1f,
                ignorePlayerPed: false,
                raycastCheck: false,
                smoothMoveModifier: 3f
            );
            _camera.AttachTo(Game.PlayerPed);

            _commands.Register("startroadview").WithHandler(StartView);
            _commands.Register("stoproadview").WithHandler(StopView);

            _ticks.On(CheckMouseClicked);
        }

        private async Task CheckMouseClicked()
        {
            if (_camera.IsEnabled())
            {
                // We are in the road management view, check if the mouse has been clicked
                if (API.IsControlJustReleased(2, 229))
                {
                    Debug.WriteLine("You clicked!!");
                    // Mouse clicked!
                    float x = API.GetControlNormal(0, 239);
                    float y = API.GetControlNormal(0, 240);
                    Debug.WriteLine($"GetControlNormal: X: {x}, Y: {y}");
                    Vector3 centerPos = ScreenUtils.ScreenRelToWorld(_camera.Camera.GameCamera, new Vector2(-90f, 0f),
                        out Vector3 forwardDirection);

                    int screenWidth = 0;
                    int screenHeight = 0;
                    API.GetActiveScreenResolution(ref screenWidth, ref screenHeight);
                    Debug.WriteLine($"Active Screen Resolution: {screenWidth} x {screenHeight}");

                    x = (centerPos.X - (screenWidth / 2)); // * x;
                    y = (centerPos.Y - (screenHeight / 2)); // * y;

                    Vector3 spawnPos = new Vector3(x, y, Game.PlayerPed.Position.Z);
                    Debug.WriteLine($"You clicked: X: {spawnPos.X}, Y: {spawnPos.Y}, Z: {spawnPos.Z}");

                    // Make a cone appear
                    Model model = new Model("prop_mp_cone_01");
                    if (!await model.Request(1000))
                    {
                        Debug.WriteLine("FAT RIDE");
                        return;
                    }

                    var propHandle = API.CreateObjectNoOffset(
                        (uint)model.Hash,
                        spawnPos.X,
                        spawnPos.Y,
                        spawnPos.Z,
                        true,
                        false,
                        false
                    );
                    Prop prop = new Prop(propHandle);
                    API.PlaceObjectOnGroundProperly(prop.Handle);
                }
            }
        }

        private Vector3? TryTheAiWay()
        {
            Vector3 superSpyCamLocation = API.GetGameplayCamCoord();
            Vector3 superSpyCamRotatorThingy = API.GetGameplayCamRot(2);
            Vector3 superSpyCamGazingDirection = RotationToDirection(superSpyCamRotatorThingy);
            Vector3 aPlaceFarFarAway = superSpyCamLocation + superSpyCamGazingDirection * 10000f;

            OutputArgument thingWeMayHit = new OutputArgument();

            int innocentRaySneakingAround = API.StartShapeTestRay(superSpyCamLocation.X, superSpyCamLocation.Y,
                superSpyCamLocation.Z, aPlaceFarFarAway.X, aPlaceFarFarAway.Y, aPlaceFarFarAway.Z, -1, 0, 7);

            int didInnocentRayHitSomething = 0;
            Vector3 positionWhereInnocentRayLostItsInnocence = new Vector3();
            Vector3 aNormalLoiteringOnTheSurface = new Vector3();

            bool hit = false;
            int entityHit = 0;
            didInnocentRayHitSomething = API.GetShapeTestResult(innocentRaySneakingAround, ref hit,
                ref positionWhereInnocentRayLostItsInnocence, ref aNormalLoiteringOnTheSurface, ref entityHit);

            if (2 == didInnocentRayHitSomething)
            {
                Debug.WriteLine(
                    $"Our innocent ray seemed to have found its soulmate at: X={positionWhereInnocentRayLostItsInnocence.X}, Y={positionWhereInnocentRayLostItsInnocence.Y}, Z={positionWhereInnocentRayLostItsInnocence.Z}");

                return positionWhereInnocentRayLostItsInnocence;
            }

            return null;
        }

        private Vector3 RotationToDirection(Vector3 rotation)
        {
            var goingAroundZInCircles = DegToRad(rotation.Z);
            var goingAroundXInCircles = DegToRad(rotation.X);
            var numbarThatReallyLikesCosine = Math.Abs(Math.Cos(goingAroundXInCircles));

            var newVector = new Vector3
            {
                X = -2 * (float)Math.Sin(goingAroundZInCircles) * (float)numbarThatReallyLikesCosine,
                Y = (float)Math.Cos(goingAroundZInCircles) * (float)numbarThatReallyLikesCosine,
                Z = (float)Math.Sin(goingAroundXInCircles)
            };

            return newVector;
        }

        private double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        private void StartView()
        {
            _camera.AttachTo(Game.PlayerPed);
            _camera.Enable();
            _camera.ResetCamera();

            API.EnterCursorMode();
        }

        private void StopView()
        {
            API.LeaveCursorMode();
            _camera.Disable();
        }
    }
}
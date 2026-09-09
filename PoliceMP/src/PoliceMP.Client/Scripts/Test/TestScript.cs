using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using CitizenFX.Core;
using PoliceMP.Client.Actions.QuestionPed;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using CitizenFX.Core.UI;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Interface;
using Color = System.Drawing.Color;
using Vector3 = CitizenFX.Core.Vector3;
using static System.Net.Mime.MediaTypeNames;
using Text = CitizenFX.Core.UI.Text;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Reflection;
using PoliceMP.Core.Client.Scripts;
using static CitizenFX.Core.UI.Screen;

namespace PoliceMP.Client.Scripts.Test
{
    public class TestScript : Script
    {
        private readonly ILegacyClientCommunicationsManager _legacyComms;
        private readonly ILogger<TestScript> _logger;
        private readonly ICommandManager _commands;
        private readonly INotificationService _notifications;
        private readonly IQuestionService _questionService;
        private readonly IActionManager _actions;
        private readonly ISpeechService _speech;
        private readonly ITickManager _ticks;
        private readonly IGameInputManager _input;
        private readonly IClientCommunicationsManager _comms;

        public TestScript(ILogger<TestScript> logger,
            ICommandManager commands,
            ILegacyClientCommunicationsManager legacyComms,
            INotificationService notifications,
            IQuestionService questionService,
            IActionManager actions,
            ISpeechService speech,
            ITickManager ticks,
            IGameInputManager input,
            IClientCommunicationsManager comms)
        {
            _logger = logger;
            _commands = commands;
            _legacyComms = legacyComms;
            _notifications = notifications;
            _questionService = questionService;
            _actions = actions;
            _speech = speech;
            _ticks = ticks;
            _input = input;
            _comms = comms;

            LoadNorthYankton();
        }

        protected override Task OnStartAsync()
        {
            _legacyComms.On("TestEvent", () =>
            {
                _logger.Debug("TestEvent hit.");
            });

            _legacyComms.On<string, int, bool>("TestEventWithPrimitiveArgs", (s, i, b) =>
            {
                _logger.Debug($"TestEventWithPrimitiveArgs hit: {s} / {i} / {b}");
            });

            _legacyComms.On<Vector3, Vector3>("TestEventWithObjects", (user, vector) =>
            {
                _logger.Debug($"TestEventWithObjects hit: {user} / {vector}");
            });

            _legacyComms.On("TestEventServerToClient", () =>
            {
                _logger.Debug("TestEventServerToClient hit.");
            });

            _legacyComms.On<string, TestModel>("TestEventServerToClientWithArgs", (text, vector) =>
            {
                _logger.Debug($"TestEventServerToClientWithArgs hit: {text} / {vector.Name}");
            });

            _commands.Register("clientcomms").WithHandler(async () =>
            {
                _legacyComms.ToClient("TestEvent");
                _legacyComms.ToClient("TestEventWithPrimitiveArgs", "hello", 123, true);
                _legacyComms.ToClient("TestEventWithObjects", new Vector3(5f, 6f, 7f), new Vector3(1f, 2f, 3f));

                _legacyComms.ToServer("TestEventClientToServer");
                _legacyComms.ToServer("TestEventClientToServerWithArgs", "hello", new Vector3(1f, 2f, 3f));

                var result = await _legacyComms.Request<TestModel>("TestRequestClientToServer", "mate");
                _logger.Debug($"TestRequestClientToServer Result is {result.Name}");
            });

            _legacyComms.OnRequest<string, TestModel>("TestRequestServerToClient", OnTestRequestServerToClient);

            _commands.Register("toast").HasGreedyArgs().WithHandler((message) =>
            {
                Game.Player.SendChatMessage("Show toast");
                _notifications.Success("Success", message);
                _notifications.Info("Info", message);
                _notifications.Error("Error", message);
                _notifications.Warning("Warning", message);

            });

            _commands.Register("questionped").WithHandler(async () =>
            {
                Game.Player.SendChatMessage("questionped");
                var questions = await _questionService.GetAllAsync();
                Game.Player.SendChatMessage($"got {questions.Count} questions");
                await _actions.Execute(new QuestionPed(questions.First(), Game.PlayerPed));
                Game.Player.SendChatMessage("Done");
            });

            _commands.Register("speech").HasGreedyArgs().WithHandler(async (speech) =>
            {
                Game.Player.SendChatMessage($"About to say: {speech}");
                var ped = await World.CreatePed(
                    new Model(PedHash.Abigail),
                    Game.PlayerPed.Position);

                _speech.Say(ped, speech, 30000);

                await Delay(2000);
                _speech.Do(ped, "Fuck up");
            });

            _commands.Register("fleestyle").WithHandler(drivingStyle =>
            {
                if (int.TryParse(drivingStyle, out var intStyle))
                {
                    PedExtensions.DesperateFleeDrivingStyle = intStyle;
                    _notifications.Success("Success", $"Successfully set FleeStyle = {intStyle}");
                }
                else
                    _notifications.Error("Failure", $"Could not parse {drivingStyle} to an int");
            });

            _commands.Register("fuel").WithHandler(() =>
            {
                if(Game.PlayerPed.CurrentVehicle != null)
                    _notifications.Info("Fuel Level", $"Fuel level is currently at {Game.PlayerPed.CurrentVehicle.FuelLevel}%");
                else
                    _notifications.Error("Fuel Level", "You are not in a vehicle!");
            });

            _commands.Register("insult").WithHandler(() => _speech.SayRandomInsult(Game.PlayerPed));
            _commands.Register("farewell").WithHandler(() => _speech.SayRandomFarewell(Game.PlayerPed));
            _commands.Register("greeting").WithHandler(() => _speech.SayRandomGreeting(Game.PlayerPed));

            _commands.Register("players").WithHandler(() =>
            {
                List<dynamic> players = API.GetActivePlayers();

                if (!players.Any())
                {
                    _logger.Error("Fuck");
                }
                else
                {
                    foreach (var player in players)
                    {
                        var id = Convert.ToInt32(player);
                        Debug.WriteLine($"Test: {id}, {API.GetPlayerName(player)}");
                    }
                }
            });

            _ticks.On(TestHeld);
            _ticks.On(TestCollision);
            _ticks.On(NodeTest);
            _ticks.On(() =>
            {
                if (Game.PlayerPed.CurrentVehicle is not null)
                {
                    Game.PlayerPed.CurrentVehicle.TryFlip();
                }

                return Task.FromResult(0);
            });
            
            _ticks.On(() =>
            {
                API.SetFlyThroughWindscreenParams(0f, 0f, 0f, 0f);
                return Task.FromResult(0);
            });

            // _commands.Register("mediator").WithHandler(new Func<Task>(async () =>
            // {
            //     var helloWorld = await _comms.SendToServer(new HelloWorldQuery
            //     {
            //         Name = "Fish"
            //     });
            //
            //     _speech.Say(Game.PlayerPed, helloWorld, replicate: true);
            // }));

            return Task.FromResult(0);
        }

        public static Vector3 RotateBy(Vector3 v, float degrees)
        {
            var radians = MathUtil.DegreesToRadians(degrees);
            var c = (float) System.Math.Cos(radians);
            var s = (float) System.Math.Sin(radians);

            return new Vector3(
                v.X * c - v.Y * s,
                v.X * s + v.Y * c,
                v.Z);
        }

        private Task NodeTest()
        {
            var p = Game.PlayerPed.CurrentVehicle != null ? Game.PlayerPed.CurrentVehicle.Position : Game.PlayerPed.Position;

            Color green = Color.FromArgb(128, 50, 255, 50);
            Color red = Color.FromArgb(128, 255, 0, 0);
            Color blue = Color.FromArgb(128, 50, 50, 255);
            Color orange = Color.FromArgb(128, 255, 90, 0);

            Vector3 pp = Vector3.Zero;
            //for (int i = 1; i < 50; i++)
            //{
            //    float heading = 0f;
            //    int totalLanes = 0;
            //    if (API.GetNthClosestVehicleNodeWithHeading(p.X, p.Y, p.Z, i, ref pp, ref heading, ref totalLanes, 1, 3.0f, 2.5f))
            //    {
            //        Vector3 ppSrc = Vector3.Zero;
            //        Vector3 ppDest = Vector3.Zero;
            //        Vector3 roadsidePos = Vector3.Zero;
            //        int lanesIn = 0;
            //        int lanesOut = 0;
            //        float offset = 0f;
            //        API.GetClosestRoad(pp.X, pp.Y, pp.Z, 1.0f, 0, ref ppSrc, ref ppDest, ref lanesIn, ref lanesOut, ref offset,
            //            false);

            //        pp += Vector3.ForwardLH;
            //        ppSrc += Vector3.ForwardLH;
            //        ppDest += Vector3.ForwardLH;

            //        var right = RotateBy(GameMath.HeadingToDirection(heading), 90f);
            //        World.DrawMarker(MarkerType.DebugSphere, ppSrc, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, green);
            //        //World.DrawMarker(MarkerType.DebugSphere, ppDest, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, blue);
            //        World.DrawLine(ppSrc, ppDest, blue);

            //        //var totalOutSpan = 5.0f * lanesOut + offset;
            //        //var totalInSpan = 5.0f * lanesIn + offset;

            //        //bool isOneWay = lanesIn == 0 || lanesOut == 0;


            //        //var edgeSizeOut = isOneWay ? totalOutSpan / 2 : totalOutSpan;
            //        //var edgeSizeIn = isOneWay ? totalInSpan / 2 : totalInSpan;

            //        //if (lanesOut > 0)
            //        //{
            //        //    var vOffsetEdge = right * edgeSizeOut;
            //        //    World.DrawLine(ppSrc + vOffsetEdge, ppDest + vOffsetEdge, orange);
            //        //    World.DrawLine(ppSrc + vOffsetEdge * -1, ppDest + vOffsetEdge * -1, red);
            //        //}

            //        //if (API.GetRoadSidePointWithHeading(pp.X, pp.Y, pp.Z, heading, ref roadsidePos))
            //        //{
            //        //    World.DrawMarker(MarkerType.DebugSphere, roadsidePos + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.25f, orange);
            //        //}

            //        //if (!isOneWay)
            //        //{
            //        //    var h2 = heading + 180f;
            //        //    if (h2 > 360) h2 -= 360;

            //        //    //if (API.GetRoadSidePointWithHeading(pp.X, pp.Y, pp.Z, h2, ref roadsidePos))
            //        //    //{
            //        //    //    World.DrawMarker(MarkerType.DebugSphere, roadsidePos + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.25f, orange);
            //        //    //}
            //        //}

            //        var sub = new StringBuilder(32);

            //        if (API.GetRoadSidePointWithHeading(pp.X, pp.Y, pp.Z, heading, ref roadsidePos))
            //        {
            //            sub.Append("h; ");
            //            World.DrawMarker(MarkerType.DebugSphere, roadsidePos + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.25f, orange);
            //            World.DrawLine(roadsidePos + Vector3.ForwardLH, roadsidePos + Vector3.ForwardLH + GameMath.HeadingToDirection(heading), orange);
            //        }

            //        if (API.GetPointOnRoadSide(pp.X, pp.Y, pp.Z, 0, ref roadsidePos)
            //            && API.GetPointOnRoadSide(pp.X, pp.Y, pp.Z, 1, ref roadsidePos))
            //        {
            //            sub.Append("0; 1; ");
            //            World.DrawMarker(MarkerType.DebugSphere, roadsidePos + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.25f, green);
            //            World.DrawMarker(MarkerType.DebugSphere, roadsidePos + Vector3.ForwardLH * 2, Vector3.Zero,
            //                Vector3.Zero, Vector3.One * 0.25f, blue);
            //            World.DrawLine(roadsidePos + Vector3.ForwardLH * 1.5f, roadsidePos + Vector3.ForwardLH * 1.5f + GameMath.HeadingToDirection(heading), orange);
            //        }

            //        //if (API.GetPointOnRoadSide(pp.X, pp.Y, pp.Z, 1, ref roadsidePos))
            //        //{
            //        //    sub.Append("1; ");
            //        //    World.DrawMarker(MarkerType.DebugSphere, roadsidePos + Vector3.ForwardLH * 2, Vector3.Zero, Vector3.Zero, Vector3.One * 0.25f, blue);
            //        //}

            //        else if (API.GetPointOnRoadSide(pp.X, pp.Y, pp.Z, -1, ref roadsidePos))
            //        {
            //            sub.Append("-1; ");
            //            World.DrawMarker(MarkerType.DebugSphere, roadsidePos + Vector3.ForwardLH * 3, Vector3.Zero, Vector3.Zero, Vector3.One * 0.25f, red);
            //        }

            //        Screen.ShowSubtitle(sub.ToString());

            //        //for (int j = 0; j < lanesOut; j++)
            //        //{
            //        //    var laneoffset = (5 * (j + 1) + offset);
            //        //    if (lanesIn == 0)
            //        //        laneoffset -= totalOutSpan;

            //        //    World.DrawLine(ppSrc + laneoffset * right, ppDest + laneoffset * right, red);
            //        //}

            //        //for (int j = 0; j < lanesIn; j++)
            //        //{
            //        //    var laneoffset = (5 * (j + 1) + offset);
            //        //    if (isMiddle)
            //        //        laneoffset -= totalInSpan / 2 + 2.5f;

            //        //    World.DrawLine(ppSrc + laneoffset * right, ppDest + laneoffset * right, red);
            //        //}
            //    }
            //}

            //var v1 = Vector3.Zero;
            //var v2 = Vector3.Zero;
            //int lanesIn = 0;
            //int lanesOut = 0;
            //float f1 = 0f;

            //var x = 0;

            //while (API.GetClosestRoad(p.X, p.Y, p.Z, 1f, 1, ref v1, ref v2, ref i1, ref i2, ref f1, false) == 1)
            //{
            //    //Screen.ShowSubtitle($"{test}: v1: {v1}, v2: {v2}, i1: {i1}, i2: {i2}, f1: {f1}");
            //    World.DrawMarker(MarkerType.DebugSphere, v1 + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, good);
            //    World.DrawMarker(MarkerType.DebugSphere, v2 + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, bad);
            //    World.DrawLine(p + Vector3.ForwardLH, v1 + Vector3.ForwardLH, good);
            //    World.DrawLine(v1 + Vector3.ForwardLH, v2 + Vector3.ForwardLH, good);
            //    x++;
            //}

            // p3:
            // OxD = Avoid Highways
            // 0x20 = Shortcut?
            // 0x40 = Random places

            // p4:
            // 1 & 2 = Any Road
            // 3 & 4 = Major Road
            // 5 & 6 = Specific junction and waterways? Potentially for a specific mission

            // p10 = Only Asphalt Road
            //var test = API.GetClosestRoad(p.X, p.Y, p.Z, 1f, 1, ref v1, ref v2, ref lanesIn, ref lanesOut, ref f1, false);

            //Screen.ShowSubtitle($"LanesIn: {lanesIn}, LanesOut: {lanesOut}, f1: {f1}");
            //World.DrawMarker(MarkerType.DebugSphere, v1 + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, good);
            //World.DrawMarker(MarkerType.DebugSphere, v2 + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, bad);
            //World.DrawLine(p + Vector3.ForwardLH, v1 + Vector3.ForwardLH, good);
            //World.DrawLine(v1 + Vector3.ForwardLH, v2 + Vector3.ForwardLH, good);

            //Vector3 pp = Vector3.Zero;
            //Vector3 ppSrc = Vector3.Zero;
            //Vector3 ppDest = Vector3.Zero;
            //float h = 0f;
            //int lanes = 0;
            //int backwardLanes = 0;
            //int forwardLanes = 0;
            //float offset = 0f;
            //if (API.GetNthClosestVehicleNodeWithHeading(p.X, p.Y, p.Z, 1, ref pp, ref h, ref lanes, 9, 3.0f, 2.5f)
            //    && API.GetClosestRoad(pp.X, pp.Y, pp.Z, 1.0f, 1, ref ppDest, ref ppSrc, ref forwardLanes,
            //        ref backwardLanes, ref offset, true) == 1)
            //{
            //    bool oneWay = h < 90f || h > 270f ? lanes == forwardLanes : lanes == backwardLanes;

            //    var j = 1;

            //    World.DrawMarker(MarkerType.DebugSphere, pp + Vector3.ForwardLH * 2, Vector3.Zero, Vector3.Zero,
            //        Vector3.One * 0.5f, blue);
            //    World.DrawMarker(MarkerType.DebugSphere, ppSrc + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero,
            //        Vector3.One * 0.5f, green);
            //    World.DrawMarker(MarkerType.DebugSphere, ppDest + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero,
            //        Vector3.One * 0.5f, red);
            //    World.DrawLine(ppSrc + Vector3.ForwardLH, ppDest + Vector3.ForwardLH, green);
            //    //World.DrawLine(p + Vector3.ForwardLH, ppSrc + Vector3.ForwardLH, green);
            //    World.DrawLine(ppSrc + Vector3.ForwardLH,
            //        ppSrc + Vector3.ForwardLH + GameMath.HeadingToDirection(h) * 3, Color.FromArgb(255, 255, 0, 255));
            //    Screen.ShowSubtitle(
            //        $"Total Lanes: {lanes}; Lanes Forward: {forwardLanes}, Lanes Backwards: {backwardLanes}; width {offset} ");

            //    for (int i = 0; i < backwardLanes; i++)
            //    {
            //        var laneOffset = GameMath.HeadingToDirection(h) * (5 * i + 1);
            //        World.DrawLine(ppSrc + laneOffset, ppDest + laneOffset, Color.FromArgb(255, 255, 90, 0));
            //    }

            //    for (int i = 0; i < forwardLanes; i++)
            //    {
            //        var laneOffset = GameMath.HeadingToDirection(h) * (5 * i + 1) * -1;
            //        World.DrawLine(ppSrc + laneOffset, ppDest + laneOffset, Color.FromArgb(255, 255, 90, 0));
            //    }
            //}

            //API.GetClosestVehicleNodeWithHeading(p.X, p.Y, p.Z, ref pp, ref h, 1, 3.0f, 0);
            //API.GetClosestRoad(pp.X, pp.Y, pp.Z, 0.0f, 2, ref ppSrc, ref ppDest, ref forwardLanes, ref backwardLanes,
            //    ref offset, false);

            //World.DrawMarker(MarkerType.DebugSphere, pp + Vector3.ForwardLH * 2f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, blue);
            //World.DrawMarker(MarkerType.DebugSphere, ppSrc + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, green);
            //World.DrawMarker(MarkerType.DebugSphere, ppDest + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, red);
            //World.DrawLine(ppSrc + Vector3.ForwardLH, ppDest + Vector3.ForwardLH, green);

            //var heading = Game.PlayerPed.Heading / 10;
            //Screen.ShowSubtitle($"{heading}");
            //var j = 1;
            //while (j < 5)
            //{
            //    if (API.GetClosestRoad(pp.X, pp.Y, pp.Z, 0.0f, j, ref ppSrc, ref ppDest,
            //            ref forwardLanes, ref backwardLanes, ref offset, true) == 0)
            //    {
            //        j++; 
            //        continue;
            //    }
            //    World.DrawMarker(MarkerType.DebugSphere, pp + Vector3.ForwardLH * 2f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, blue);
            //    World.DrawMarker(MarkerType.DebugSphere, ppSrc + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, green);
            //    World.DrawMarker(MarkerType.DebugSphere, ppDest + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, red);
            //    World.DrawLine(ppSrc + Vector3.ForwardLH, ppDest + Vector3.ForwardLH, green);

            //    var d = 0;
            //    var iFlags = 0;
            //    var srcProp = API.GetVehicleNodeProperties(ppSrc.X, ppSrc.Y, ppSrc.Z, ref d, ref iFlags);
            //    var srcFlags = (PathNodeFlags)iFlags;
            //    var destProp = API.GetVehicleNodeProperties(ppDest.X, ppDest.Y, ppDest.Z, ref d, ref iFlags);
            //    var destFlags = (PathNodeFlags)iFlags;

            //    var text = new Text(srcFlags.ToString(), Screen.WorldToScreen(ppSrc, false), 0.2f);
            //    text.Draw();

            //    text = new Text(destFlags.ToString(), Screen.WorldToScreen(ppDest, false), 0.2f);
            //    text.Draw();

            //    var median = (ppDest + ppSrc) * 0.5f;
            //    var length = World.GetDistance(ppSrc, ppDest);

            //    text = new Text(j.ToString(CultureInfo.InvariantCulture), Screen.WorldToScreen(median), 0.3f, Color.FromArgb(255, 0, 0, 0));
            //    text.Draw();

            //    text = new Text($"{forwardLanes + backwardLanes}", Screen.WorldToScreen(median) + new SizeF(0f, 20f),
            //        0.3f, Color.FromArgb(255, 255, 50, 50));
            //    text.Draw();

            //    j++;
            //}

            //if (API.GetNthClosestVehicleNodeFavourDirection(v1.X, v1.Y, v1.Z, v2.X, v2.Y, v2.Z, i, ref pp, ref h, 1,
            //        0x40400000, 1))
            //{
            //    World.DrawMarker(MarkerType.DebugSphere, pp + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, blue);
            //}

            //for (int i = 0; i < 100; i++)
            //{
            //    Vector3 pos = Vector3.Zero;
            //    int density = 0;
            //    int flags = 0;
            //    float heading = 0f;
            //    int unk1 = 0;

            //    if (API.GetNthClosestVehicleNodeWithHeading(p.X, p.Y, p.Z, i, ref pos, ref heading, ref unk1, 1, 3f, 2.5f)
            //        && API.GetVehicleNodeProperties(pos.X, pos.Y, pos.Z, ref density, ref flags))
            //    {
            //        var color = blue;
            //        World.DrawMarker(MarkerType.DebugSphere, pos, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, color);

            //        //if (color == green)
            //        //    World.DrawLine(p, pos, color);
            //        var nodeId = API.GetNthClosestVehicleNodeId(pos.X, pos.Y, pos.Z, 1, 1, 3.0f, 0f);
            //        var gpsAllowed = API.GetVehicleNodeIsGpsAllowed(nodeId);
            //        var disabled = API.GetVehicleNodeIsSwitchedOff(nodeId);
            //        var text = new Text($"{(PathNodeFlags)flags & ~PathNodeFlags.WanderTarget}", Screen.WorldToScreen(pos, false), 0.2f);
            //        text.Draw();
            //    }
            //}


            //var isOnRoad = API.IsPointOnRoad(p.X, p.Y, p.Z, API.GetVehiclePedIsIn(Game.PlayerPed.Handle, false));
            //Screen.ShowSubtitle($"Is on road: {isOnRoad}");

            //int dir = 0;
            //float veh = 0;
            //float dist = 0;
            //var pp = Vector3.Zero;
            //var ppp = Vector3.Zero;
            //var t = API.GetClosestRoad(p.X, p.Y, p.Z, 1.0f, 1, ref pp, ref ppp, ref dir, ref dir, ref veh, false);
            //World.DrawMarker(MarkerType.DebugSphere, ppp, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, green);
            //var flags = Convert.ToByte(veh);
            //Screen.ShowSubtitle($"t = {t}, veh={flags}");


            // 0x1      IncludeDisabled
            // 0x2      Only Water Nodes (Required IncludeDisabled)
            // 0x4      No Junctions
            // 0x8      No dead ends (Required IncludeDisabled)

            //foreach (var pos in GetClosestNodesOfType(p, 0x1, 10))
            //{
            //    var c = Color.FromArgb(100, 255, 255, 255);
            //    World.DrawMarker(MarkerType.DebugSphere, pos, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);
            //    //World.DrawLine(p, pos, c);
            //}

            var left = GetClosestNodesOfType(p, 0x1, 50);
            var right = GetClosestNodesOfType(p, 0x1, 50);
            var all = new List<Vector3>(left.Count + right.Count);
            all.AddRange(left);
            all.AddRange(right);

            var bad = Color.FromArgb(255, 255, 0, 0);
            var neutral = Color.FromArgb(255, 255, 128, 0);
            var good = Color.FromArgb(255, 0, 255, 0);
            foreach (var pos in all)
            {
                Color c;
                var leftContains = left.Contains(pos);
                var rightContains = right.Contains(pos);
                if (!rightContains)
                {
                    c = leftContains ? bad : neutral;
                    World.DrawLine(p, pos + Vector3.ForwardLH * 1f * 0.5f, c);
                }
                else
                {
                    c = good;
                }

                int density = 0;
                int iflags = 0;
                Vector3 ppp = Vector3.Zero;
                float heading = 0f;
                int lanes = 0;
                API.GetNthClosestVehicleNodeWithHeading(pos.X, pos.Y, pos.Z, 1, ref ppp, ref heading, ref lanes, 0, 3.0f,
                    0);
                API.GetVehicleNodeProperties(pos.X, pos.Y, pos.Z, ref density, ref iflags);

                var builder = new StringBuilder();
                builder.AppendLine($"flags: {(PathNodeFlags)iflags}");
                builder.AppendLine($"heading: {heading}");
                builder.AppendLine($"lanes: {lanes}");
                var text = new Text(builder.ToString(), Screen.WorldToScreen(pos), 0.2f);
                text.Draw();

                World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 1f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.5f, c);

            }

            foreach (var pos in left)
            {
                Color c;
                if (!right.Contains(pos))
                {
                    c = bad;
                    int density = 0;
                    int iflags = 0;
                    Vector3 ppp = Vector3.Zero;
                    float heading = 0f;
                    int lanes = 0;
                    API.GetVehicleNodeProperties(pos.X, pos.Y, pos.Z, ref density, ref iflags);
                    API.GetNthClosestVehicleNodeWithHeading(pos.X, pos.Y, pos.Z, 1, ref ppp, ref heading, ref lanes, 0, 3.0f,
                        0);
                    var builder = new StringBuilder();
                    builder.AppendLine($"density: {density}");
                    builder.AppendLine($"flags: {(PathNodeFlags)iflags}");
                    builder.AppendLine($"heading: {heading}");
                    builder.AppendLine($"lanes: {lanes}");

                    var text = new Text(builder.ToString(), Screen.WorldToScreen(pos), 0.2f);
                    text.Draw();
                    World.DrawLine(p, pos + Vector3.ForwardLH * 1f * 0.5f, c);
                }
                else
                {
                    c = good;
                }

                World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 1f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);

            }

            //foreach (var pos in GetClosestNodesOfType(p, 0x0, 10))
            //{
            //}

            //foreach (var pos in GetClosestNodesOfType(p, 0x8, 10))
            //{
            //    var c = Color.FromArgb(255, 0, 255, 0);
            //    World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 2f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);
            //    World.DrawLine(p, pos + Vector3.ForwardLH * 2f * 0.5f, c);
            //}

            //foreach (var pos in GetClosestNodesOfType(p, 0x8, 10))
            //{
            //    var c = Color.FromArgb(100, 0, 255, 0);
            //    World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 3f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);
            //    World.DrawLine(p, pos + Vector3.ForwardLH * 3f * 0.5f, c);
            //}

            //foreach (var pos in GetClosestNodesOfType(p, 0x10, 10))
            //{
            //    var c = Color.FromArgb(100, 0, 0, 255);
            //    World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 4f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);
            //    //World.DrawLine(p, pos + Vector3.ForwardLH * 4f * 0.5f, c);
            //}

            //foreach (var pos in GetClosestNodesOfType(p, 0x20, 10))
            //{
            //    var c = Color.FromArgb(100, 255, 0, 255);
            //    World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 5f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);
            //    //World.DrawLine(p, pos + Vector3.ForwardLH * 5f * 0.5f, c);
            //}

            //foreach (var pos in GetClosestNodesOfType(p, 0x40, 10))
            //{
            //    var c = Color.FromArgb(100, 255, 255, 0);
            //    World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 6f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);
            //    //World.DrawLine(p, pos + Vector3.ForwardLH * 6f * 0.5f, c);
            //}

            //foreach (var pos in GetClosestNodesOfType(p, 0x80, 10))
            //{
            //    var c = Color.FromArgb(100, 0, 255, 255);
            //    World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 7f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);
            //    //World.DrawLine(p, pos + Vector3.ForwardLH * 7f * 0.5f, c);
            //}

            //foreach (var pos in GetClosestNodesOfType(p, 0x100, 10))
            //{
            //    var c = Color.FromArgb(100, 0, 100, 255);
            //    World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 8f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);
            //    //World.DrawLine(p, pos + Vector3.ForwardLH * 8f * 0.5f, c);
            //}

            //foreach (var pos in GetClosestNodesOfType(p, 0x200, 10))
            //{
            //    var c = Color.FromArgb(100, 100, 255, 100);
            //    World.DrawMarker(MarkerType.DebugSphere, pos + Vector3.ForwardLH * 9f * 0.5f, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, c);
            //    //World.DrawLine(p, pos + Vector3.ForwardLH * 9f * 0.5f, c);
            //}

            return Task.FromResult(0);

        }

        private List<Vector3> GetClosestNodesOfType(Vector3 p, int nodeType, int count)
        {
            var nodes = new List<Vector3>(count);
            for (int i = 0; i < count; i++)
            {
                var position = Vector3.Zero;

                if (!API.GetNthClosestVehicleNode(p.X, p.Y, p.Z, i, ref position, nodeType, 0x40400000, 0))
                {
                    break;
                }

                nodes.Add(position);
            }

            return nodes;
        }

        private async Task<TestModel> OnTestRequestServerToClient(string wat)
        {
            _logger.Debug($"OnTestRequestServerToClient hit: {wat}");
            await Delay(2500);
            return new TestModel { Name = "Michael" };
        }

        private Task TestHeld()
        {
            if(_input.IsJustBeingHeld(Control.Jump))
            {
                _notifications.Info("Jump held!", "Jump has just been held!");
            }

            return Task.FromResult(0);
        }

        private Task TestCollision()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle != null)
            {
                var p1 = vehicle.Position;
                var p2 = vehicle.Position + vehicle.ForwardVector * 30f;
                API.DrawLine(p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z, 90, 90, 255, 255);
                var result = World.RaycastCapsule(vehicle.Position, vehicle.ForwardVector, 30f, 2f,
                    IntersectOptions.MissionEntities, vehicle);

                if (result.DitHit)
                {
                    World.DrawMarker(MarkerType.DebugSphere, result.HitPosition, Vector3.Zero, Vector3.Zero, Vector3.One, Color.FromArgb(255, 90, 255, 90));
                }
            }

            return Task.FromResult(0);
        }

        public void LoadNorthYankton()
        {
            API.RequestIpl("plg_01");
            API.RequestIpl("prologue01");
            API.RequestIpl("prologue01_lod");
            API.RequestIpl("prologue01c");
            API.RequestIpl("prologue01c_lod");
            API.RequestIpl("prologue01d");
            API.RequestIpl("prologue01d_lod");
            API.RequestIpl("prologue01e");
            API.RequestIpl("prologue01e_lod");
            API.RequestIpl("prologue01f");
            API.RequestIpl("prologue01f_lod");
            API.RequestIpl("prologue01g");
            API.RequestIpl("prologue01h");
            API.RequestIpl("prologue01h_lod");
            API.RequestIpl("prologue01i");
            API.RequestIpl("prologue01i_lod");
            API.RequestIpl("prologue01j");
            API.RequestIpl("prologue01j_lod");
            API.RequestIpl("prologue01k");
            API.RequestIpl("prologue01k_lod");
            API.RequestIpl("prologue01z");
            API.RequestIpl("prologue01z_lod");
            API.RequestIpl("plg_02");
            API.RequestIpl("prologue02");
            API.RequestIpl("prologue02_lod");
            API.RequestIpl("plg_03");
            API.RequestIpl("prologue03");
            API.RequestIpl("prologue03_lod");
            API.RequestIpl("prologue03b");
            API.RequestIpl("prologue03b_lod");
            API.RequestIpl("prologue03_grv_dug");
            API.RequestIpl("prologue03_grv_dug_lod");
            API.RequestIpl("prologue_grv_torch");
            API.RequestIpl("plg_04");
            API.RequestIpl("prologue04");
            API.RequestIpl("prologue04_lod");
            API.RequestIpl("prologue04b");
            API.RequestIpl("prologue04b_lod");
            API.RequestIpl("prologue04_cover");
            API.RequestIpl("des_protree_end");
            API.RequestIpl("des_protree_start");
            API.RequestIpl("des_protree_start_lod");
            API.RequestIpl("plg_05");
            API.RequestIpl("prologue05");
            API.RequestIpl("prologue05_lod");
            API.RequestIpl("prologue05b");
            API.RequestIpl("prologue05b_lod");
            API.RequestIpl("plg_06");
            API.RequestIpl("prologue06");
            API.RequestIpl("prologue06_lod");
            API.RequestIpl("prologue06b");
            API.RequestIpl("prologue06b_lod");
            API.RequestIpl("prologue06_int");
            API.RequestIpl("prologue06_int_lod");
            API.RequestIpl("prologue06_pannel");
            API.RequestIpl("prologue06_pannel_lod");
            API.RequestIpl("prologue_m2_door");
            API.RequestIpl("prologue_m2_door_lod");
            API.RequestIpl("plg_occl_00");
            API.RequestIpl("prologue_occl");
            API.RequestIpl("plg_rd");
            API.RequestIpl("prologuerd");
            API.RequestIpl("prologuerdb");
            API.RequestIpl("prologuerd_lod");
        }
    }
}
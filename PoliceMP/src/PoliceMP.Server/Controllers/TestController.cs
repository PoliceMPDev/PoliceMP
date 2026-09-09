using System;
using CitizenFX.Core;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Data.Entities;
using System.Threading.Tasks;
using Bogus.Extensions;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Options;
using Vector3 = CitizenFX.Core.Vector3;

namespace PoliceMP.Server.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private readonly ICommandManager _commands;
        private readonly ILegacyServerCommunicationsManager _legacyComms;

        private readonly IServerCommunicationsManager _comms;
        //private readonly IUserService _userService;

        public TestController(ILogger<TestController> logger,
            ICommandManager commands,
            ILegacyServerCommunicationsManager legacyComms,
            IServerCommunicationsManager comms
            /*IUserService userService*/)
        {
            _logger = logger;
            _commands = commands;
            _legacyComms = legacyComms;
            _comms = comms;
            //_userService = userService;
        }

        public override Task Started()
        {
            /*
            _comms.On("TestEvent", () =>
            {
                _logger.Debug("TestEvent hit.");
            });

            _comms.On<string, int, bool>("TestEventWithPrimitiveArgs", (s, i, b) =>
            {
                _logger.Debug($"TestEventWithPrimitiveArgs hit: {s} / {i} / {b}");
            });

            _comms.On<User, Vector3>("TestEventWithObjects", (user, vector) =>
            {
                _logger.Debug($"TestEventWithObjects hit: {user.UserId} / {vector}");
            });

            _comms.On<Player>("TestEventClientToServer", async (player) =>
            {
                _logger.Debug($"TestEventClientToServer hit by {player.Name}");

                var result = await _comms.Request<TestModel>(player, "TestRequestServerToClient", "mate");
                _logger.Debug($"TestRequestServerToClient result received: {result.Name}");
            });

            _comms.On<string, TestModel>("TestEventClientToServerWithArgs", (player, text, vector) =>
            {
                _logger.Debug($"TestEventClientToServerWithArgs hit by {player.Name}: {text} / {vector.Name}");
            });

            _comms.OnRequest<string, TestModel>("TestRequestClientToServer", OnTestRequestClientToServer);

            _comms.ToServer("TestEvent");
            _comms.ToServer("TestEventWithPrimitiveArgs", "hello", 123, true);
            _comms.ToServer("TestEventWithObjects", new User { UserId = 123 }, new Vector3(1f, 2f, 3f));

            _commands.Register("servercomms").WithHandler(() =>
            {
                _comms.ToClient("TestEventServerToClient");
                _comms.ToClient("TestEventServerToClientWithArgs", "test", new Vector3(1f, 2f, 3f));
            });


            _commands.Register("vehicle").WithHandler(new Action<Player>(player =>
            {
                var ped = API.GetPlayerPed(player.Handle);

                var pos = API.GetEntityCoords(ped);
                var vehicle = API.CreateVehicle(
                    (uint)API.GetHashKey("addpolgolf"),
                    pos.X,
                    pos.Y,
                    pos.Z,
                    API.GetEntityHeading(ped),
                    true,
                    true);

                API.TaskWarpPedIntoVehicle(ped, vehicle, -1);
            }));
            */

            // _comms.AddRequestHandler<HelloWorldQuery, string>(OnHelloWorld);

            return Task.FromResult(0);
        }

        // private Task<string> OnHelloWorld(HelloWorldQuery query)
        // {
        //     return Task.FromResult($"Hello {query.Name}!");
        // }

        private Task<TestModel> OnTestRequestClientToServer(Player player, string arg)
        {
            _logger.Debug($"OnTestRequestClientToServer hit. arg: {arg}");
            return Task.FromResult(new TestModel { Name = "McMillan" });
        }
    }
}
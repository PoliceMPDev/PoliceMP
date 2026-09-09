using Microsoft.Extensions.Options;
using PoliceMP.Server.Options.Interfaces;
using PoliceMP.Shared.Options;

namespace PoliceMP.Server.Options
{
    public class OptionsManager : IOptionsManager
    {
        public Shared.Options.Options Options { get; }

        public OptionsManager(IOptions<ActionOptions> action,
            IOptions<AuthOptions> auth,
            IOptions<ServerOptions> server,
            IOptions<SpawnOptions> spawn,
            IOptions<VehicleFaultGeneratorOptions> vehicleFaultGenerator,
            IOptions<PlayerControllerOptions> playerController,
            IOptions<SpeedBumpOptions> speedBumps,
            IOptions<SpeechOptions> speech,
            IOptions<OffenceOptions> offence)

        {
            Options = new Shared.Options.Options(
                action.Value,
                auth.Value,
                server.Value,
                spawn.Value,
                vehicleFaultGenerator.Value,
                playerController.Value,
                speedBumps.Value,
                speech.Value,
                offence.Value);
        }
    }
}
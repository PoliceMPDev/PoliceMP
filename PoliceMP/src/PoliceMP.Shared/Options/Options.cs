namespace PoliceMP.Shared.Options
{
    public class Options
    {
        public ActionOptions Action { get; }
        public AuthOptions Auth { get; }
        public ServerOptions Server { get; }
        public SpawnOptions Spawn { get; }
        public VehicleFaultGeneratorOptions VehicleFaultGenerator { get; }
        public PlayerControllerOptions PlayerController { get; }
        public SpeedBumpOptions SpeedBumps { get; }
        public SpeechOptions Speech { get; }
        public OffenceOptions Offence { get; }

        public Options(ActionOptions action,
            AuthOptions auth,
            ServerOptions server,
            SpawnOptions spawn,
            VehicleFaultGeneratorOptions vehicleFaultGenerator,
            PlayerControllerOptions playerController,
            SpeedBumpOptions speedBumps,
            SpeechOptions speech,
            OffenceOptions offence)
        {
            Action = action;
            Auth = auth;
            Server = server;
            Spawn = spawn;
            VehicleFaultGenerator = vehicleFaultGenerator;
            PlayerController = playerController;
            SpeedBumps = speedBumps;
            Speech = speech;
            Offence = offence;
        }
    }
}
namespace PoliceMP.Shared.Options
{
    public class VehicleFaultGeneratorOptions
    {
        public double IllegalStuffChance { get; set; }
        public double SpeedingChance { get; set; }
        public double RunRedLightChance { get; set; }
        public double HeadlightsOffChance { get; set; }
        public double PoorBodyHealthChance { get; set; }
        public double PoorEngineHealthChance { get; set; }
        public double PoorPetrolTankHealthChance { get; set; }
        public double BurstTyreChance { get; set; }
        public double AlarmActiveChance { get; set; }
        public double DodgyDriverChance { get; set; }
        public float SpeedingSpeed { get; set; }
        public int SpeedingDrivingStyle { get; set; }
        public int RunRedLightDrivingStyle { get; set; }
        public int DodgyDriverDrivingStyle { get; set; }
        public int AlarmTimeLeft { get; set; }
        public int PoorBodyMinHealth { get; set; }
        public int PoorBodyMaxHealth { get; set; }
        public int PoorEngineMinHealth { get; set; }
        public int PoorEngineMaxHealth { get; set; }
        public int PoorPetrolTankMinHealth { get; set; }
        public int PoorPetrolTankMaxHealth { get; set; }
    }
}
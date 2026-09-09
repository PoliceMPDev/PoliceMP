namespace PoliceMP.Computer.Server
{
    public class PlayerInfo
    {
        public string Name { get; set; }
        public int ServerId { get; set; }
        public int NetworkId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public int VehicleNetworkId { get; set; }
        
        public PlayerInfo()
        {
                    
        }
    }
}
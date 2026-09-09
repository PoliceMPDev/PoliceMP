namespace PoliceMP.Shared.Models
{
    public class SpeedProp
    {
        public float Speed { get; set; }
        public string ModelName { get; set; }

        public SpeedProp(float speed, string modelName)
        {
            Speed = speed;
            ModelName = modelName;
        }
    }
}
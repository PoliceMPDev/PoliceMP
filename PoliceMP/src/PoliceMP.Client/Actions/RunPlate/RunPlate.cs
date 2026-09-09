using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.RunPlate
{
    public class RunPlate : IAction
    {
        public string Plate { get; set; }

        public RunPlate()
        {
            Plate = string.Empty;
        }

        public RunPlate(string plate)
        {
            Plate = plate;
        }
    }
}
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.RunName
{
    public class RunName : IAction
    {
        public string Name { get; set; }

        public RunName()
        {
            Name = string.Empty;
        }

        public RunName(string name)
        {
            Name = name;
        }
    }
}
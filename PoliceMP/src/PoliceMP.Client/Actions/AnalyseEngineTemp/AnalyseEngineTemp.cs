using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.AnalyseEngineTemp
{
    public class AnalyseEngineTemp : IAction
    {
        public Vehicle Target { get; }

        public AnalyseEngineTemp(Vehicle target)
        {
            Target = target;
        }
    }
}
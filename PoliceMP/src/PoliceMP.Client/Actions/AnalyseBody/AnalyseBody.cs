using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.AnalyseBody
{
    public class AnalyseBody : IAction
    {
        public Ped Target { get; }

        public AnalyseBody(Ped target)
        {
            Target = target;
        }
    }
}
using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.SearchPed
{
    public class SearchPed : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }

        public SearchPed(Ped subject, Ped target)
        {
            Subject = subject;
            Target = target;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.Client.Services.Interfaces
{
    public delegate Task OnShockingEventDelegate(IReadOnlyList<Entity> involvedEntities, Entity sourcePed, IReadOnlyList<object> data);

    public interface IAiEventService
    {
        public void AddShockingEventListener(Ped sensoryPed, string shockingEventName, OnShockingEventDelegate handler);
        public void AddShockingEventListener(int sensoryPedHandle, string shockingEventName, OnShockingEventDelegate handler);

        public void RemoveShockingEventListener(Ped sensoryPed, string shockingEventName, OnShockingEventDelegate handler);
        public void RemoveShockingEventListener(int sensoryPedHandle, string shockingEventName, OnShockingEventDelegate handler);
    }
}

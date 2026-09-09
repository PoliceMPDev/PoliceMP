using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Server.Scripts;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Server.Services.Interfaces
{
    public interface IBehaviorService
    {
        Task<Blackboard<T>?> SetPedBehavior<T>(Ped ped)
            where T : PedBehaviorDefinition;

        Stack<Type> GetBehaviorTypes(Ped ped);

        Task<bool> RemovePedBehaviors(Ped ped);
    }
}
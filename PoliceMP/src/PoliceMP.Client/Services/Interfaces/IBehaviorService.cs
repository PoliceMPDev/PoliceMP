using System;
using System.Collections.Generic;
using CitizenFX.Core;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IBehaviorService
    {
        Blackboard<T> SetPedBehavior<T>(Ped ped)
            where T : PedBehaviorDefinition;
        
        Blackboard<T> AddPedBehavior<T>(Ped ped)
            where T : PedBehaviorDefinition;
        
        Stack<Type> GetBehaviorTypes(Ped ped);

        bool RemovePedBehaviors(Ped ped);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IBehaviorTypeProvider
    {
        Dictionary<Type, Type> GetBehaviorTypes();
    }
}

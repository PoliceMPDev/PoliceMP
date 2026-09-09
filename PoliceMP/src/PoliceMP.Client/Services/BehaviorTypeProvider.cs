using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Client.Services.Interfaces;

namespace PoliceMP.Client.Services
{
    public class BehaviorTypeProvider : IBehaviorTypeProvider
    {
        private readonly Dictionary<Type, Type> _pedBehaviorImplementations;

        public BehaviorTypeProvider(Dictionary<Type, Type> pedBehaviorImplementations)
        {
            _pedBehaviorImplementations = pedBehaviorImplementations;
        }

        public Dictionary<Type, Type> GetBehaviorTypes()
        {
            return _pedBehaviorImplementations;
        }
    }
}

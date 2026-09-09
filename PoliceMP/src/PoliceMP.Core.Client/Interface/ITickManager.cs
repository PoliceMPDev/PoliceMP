using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Interface
{
    public interface ITickManager
    {
        void On(Func<Task> handler);
        void Off(Func<Task> handler);
    }
}
using PoliceMP.Computer.Client.Util;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Computer.Client.Handlers
{
    public class TickHandler
    {
        private readonly ILogger _logger;

        public TickHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void On(Func<Task> handler)
        {
            Main.AddTickHandler(handler);
        }

        public void Off(Func<Task> handler)
        {
            Main.RemoveTickHandler(handler);
        }
    }
}

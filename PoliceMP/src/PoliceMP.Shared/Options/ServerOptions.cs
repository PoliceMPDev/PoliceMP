using PoliceMP.Core.Shared;

namespace PoliceMP.Shared.Options
{
    public class ServerOptions
    {
        public bool IsDevelopment { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Discord { get; set; }
    }
}

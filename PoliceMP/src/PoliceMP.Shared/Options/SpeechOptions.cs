using System.Collections.Generic;

namespace PoliceMP.Shared.Options
{
    public class SpeechOptions
    {
        public float MaxDistance { get; set; }
        public List<string> Greetings { get; set; }
        public List<string> Farewells { get; set; }
        public List<string> Insults { get; set; }
    }
}
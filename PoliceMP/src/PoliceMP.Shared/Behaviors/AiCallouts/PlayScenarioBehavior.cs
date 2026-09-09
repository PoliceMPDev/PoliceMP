using System.Collections.Generic;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.AiCallouts
{
    public class PlayScenarioBehavior : PedBehaviorDefinition
    {
        public string ScenarioName { get; set; } = "";
        public string CurrentScenarioName { get; set; } = "";
        public List<string> Speeches { get; set; } = new List<string>();
        public int SpeechTicksInterval { get; set; } = 0;
        public int AmbientTicksInterval { get; set; } = 100;
        public List<string> AmbientSounds { get; set; } = new List<string>();
        public List<string> CurrentAmbientSounds { get; set; } = new List<string>();
    }
}
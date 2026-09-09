using System;
using System.Collections.Generic;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.AiCallouts
{
    public class PlayAnimationBehavior : PedBehaviorDefinition
    {
        public List<Tuple<string, string>> AnimationsTupleList { get; set; } = new List<Tuple<string, string>>();
        public List<Tuple<string, string>> CurrentAnimationsTupleList { get; set; } = new List<Tuple<string, string>>();
        public List<string> Speeches { get; set; } = new List<string>();
        public int SpeechTicksInterval { get; set; } = 0;
        public int AmbientTicksInterval { get; set; } = 100;
        public List<string> AmbientSounds { get; set; } = new List<string>();
        public List<string> CurrentAmbientSounds { get; set; } = new List<string>();
    }
}
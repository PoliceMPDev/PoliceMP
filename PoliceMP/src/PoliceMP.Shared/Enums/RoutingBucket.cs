using System;
using System.ComponentModel;

namespace PoliceMP.Shared.Enums
{

    public class RoutingBucketAttribute : Attribute
    {
        public string Name { get; }
        public bool PopulationEnabled { get; }

        public RoutingBucketAttribute(string name, bool populationEnabled = true)
        {
            Name = name;
            PopulationEnabled = populationEnabled;
        }
    }

    public enum RoutingBucket
    {
        [RoutingBucketAttribute("Default")]
        Default,

        [RoutingBucketAttribute("Timeout")]
        Timeout,

        [RoutingBucketAttribute("Training 1")]
        Training1,

        [RoutingBucketAttribute("Training 2")]
        Training2,

        [RoutingBucketAttribute("Training 3 (No Pop)", false)]
        Training3,

        [RoutingBucketAttribute("Training 4 (No Pop)", false)]
        Training4,

        [RoutingBucketAttribute("Mod 1 (No Pop)", false)]
        Mod1,

        [RoutingBucketAttribute("Mod 2 (No Pop)", false)]
        Mod2,

        [RoutingBucketAttribute("Mod 3 (No Pop)", false)]
        Mod3,

        [RoutingBucketAttribute("Mod 4 (No Pop)", false)]
        Mod4
    }
}
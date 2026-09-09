using System.Collections.Generic;
using PoliceMP.Core.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global

namespace PoliceMP.Shared.Options
{
    public class DogOptions
    {
        public List<DogKennelEntry> Kennels { get; set; }
        public List<DogOptionsEntry> Dogs { get; set; }
    }

    public class DogOptionsEntry
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public int ComponentVariation { get; set; }
        public string SitScenario { get; set; }
        public DogOptionsAnimations Animations { get; set; }
        public bool CanTakeDownTarget { get; set; }
        public bool CanSniffTarget { get; set; }
    }

    public class DogOptionsAnimations
    {
        public DogOptionsAnimationsEntry Bark { get; set; }
        public DogOptionsAnimationsEntry TakedownFromBackDog { get; set; }
        public DogOptionsAnimationsEntry TakedownFromBackVictim { get; set; }
        public List<DogOptionsAnimationsEntry> Intimidate { get; set; }
    }

    public class DogOptionsAnimationsEntry
    {
        public string Dict { get; set; }
        public string Anim { get; set; }
    }

    public class DogKennelEntry
    {
        public string DisplayName { get; set; }
        public PmpVector3 Position { get; set; }
        public float DisplayRange { get; set; } = 10.0f;
        public float ActivationRange { get; set; } = 1.0f;
    }
}
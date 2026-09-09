using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Shared.Scripts;
using PoliceMP.Shared.Options;

namespace PoliceMP.Shared.Behaviors.Dog
{
    public enum DogBehaviorState
    {
        FollowOwner,
        Wait,
        IntimidateTarget,
        TakeDownTarget,
        SniffTarget,
        InPlayerVehicle
    }

    public enum DogBehaviorVocalization
    {
        Bark,
        Sniff
    }

    public class DogBehavior : PedBehaviorDefinition
    {
        public DogBehaviorState State { get; set; }
        public int OwnerNetworkId { get; set; }
        public int TargetNetworkId { get; set; }
        public float DistanceToSniff { get; set; }
        public float DistanceToTakeDown { get; set; }
        public DogOptionsEntry Options { get; set; }
        public int TimeToRagdollAfterTakedownMs { get; set; }
        public string DogName { get; set; }
        public double LastBarkTime { get; set; }

        public DogBehavior()
        {
            CanMigrate = false;
        }
    }

}
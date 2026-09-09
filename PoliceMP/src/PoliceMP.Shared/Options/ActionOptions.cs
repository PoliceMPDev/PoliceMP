using PoliceMP.Core.Shared.Models;
using System.Collections.Generic;

namespace PoliceMP.Shared.Options
{
    public class ActionOptions
    {
        public float BloodAlcoholLimit { get; set; }
        public List<string> CannabisObservations { get; set; }
        public List<string> CocaineObservations { get; set; }
        public List<string> HeroinObservations { get; set; }
        public List<string> EcstasyObservations { get; set; }
        public List<string> AlcoholObservations { get; set; }
        public List<JailPoint> JailPoints { get; set; }
    }

    public class JailPoint
    {
        public PmpVector3 RadiusPoint { get; set; }
        public List<PmpVector3> Cells { get; set; }
    }
}
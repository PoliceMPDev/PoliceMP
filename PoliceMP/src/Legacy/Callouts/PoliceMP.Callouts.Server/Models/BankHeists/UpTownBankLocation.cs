using CitizenFX.Core;
using System.Collections.Generic;

namespace PoliceMP.Callouts.Server.Models.BankHeists
{
    class UpTownBankLocation : BankHeistLocation
    {
        public UpTownBankLocation()
        {
            BadGuyLocations = new List<Vector3>()
            {
                new Vector3(261.68f, 210.61f, 106.28f),
                new Vector3(256.25f, 214.76f, 106.29f),
                new Vector3(247.68f, 214.86f, 106.29f),
                new Vector3(240.94f, 212.39f, 106.29f),
                new Vector3(234.79f, 223.49f, 106.29f),
                new Vector3(242.5f, 222.22f, 106.29f),
                new Vector3(251.63f, 220.29f, 106.29f),
                new Vector3(254.66f, 224.9f, 106.29f),
                new Vector3(246.85f, 229.02f, 106.29f),
                new Vector3(268f, 222.93f, 103.48f),
                new Vector3(256.14f, 226.21f, 101.88f),
                new Vector3(247.66f, 227.28f, 101.68f),
                new Vector3(261.21f, 214.11f, 110.28f),
                new Vector3(258.96f, 208.49f, 110.28f),
                new Vector3(249.19f, 210.13f, 110.28f),
                new Vector3(236.85f, 223.68f, 110.28f)
            };

            CivilianLocations = new List<Vector3>()
            {
                new Vector3(243.29f, 221.73f, 106.29f),
                new Vector3(252.69f, 218.9f, 106.29f),
                new Vector3(249.14f, 212.78f, 106.29f),
                new Vector3(253.56f, 223.77f, 106.29f),
                new Vector3(244.77f, 227.28f, 106.29f),
                new Vector3(264.6f, 213.87f, 110.29f),
                new Vector3(262.45f, 208.02f, 110.29f)
            };

            ArrivalLocation = new Vector3(205.4f, 194.9f, 105.56f);
        }
    }
}

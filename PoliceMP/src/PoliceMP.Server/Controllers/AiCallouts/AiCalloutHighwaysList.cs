using System;
using System.Collections.Generic;
using CitizenFX.Core;
using PoliceMP.Core.Server.Networking;

namespace PoliceMP.Server.Controllers.AiCallouts
{
    public class AiCalloutHighwaysList : Controller
    {
        public static List<uint> VehicleModels = new List<uint>()
        {
            1871995513,
            223258115,
            1456744817,
            2068293287,
            1923400478,
            1896491931,
            972671128,
            2006667053,
            784565758,
            723973206,
            525509695,
            16646064,
            833469436,
            523724515,
            80636076,
            1934384720,
            1119641113,
            15219735,
            722226637,
            1549126457,
            841808271,
            906642318,
            1723137093,
            886934177,
            1777363799,
            1909141499,
            1123216662,
            704435172,
            75131841,
            819197656,
            1031562256,
            917809321,
            1392481335,
            1093792632,
            1352136073,
            1987142870,
            1426219628,
            1663218586,
            1234311532,
            633712403,
            1939284556,
            272929391,
            2067820283,
            234062309,
            48339065,
            408192225,
            1044193113,
            475220373,
            1337041428,
            1269098716,
            1221512915,
            142944341,
            1203490606,
            914654722,
            666166960,
            850565707,
            3862958888,
            634118882,
            1878062887,
            486987393,
            683047626,
            2006918058,
            884422927,
            1177543287,
            2136773105,
            1177543287,
            330661258,
            1349725314,
            1581459400,
            1348744438,
            873639469,
            893081117,
            1069929536,
            121658888,
            1488164764,
            1876516712,
            699456151,
            1475773103,
            1162065741,
            65402552,
            444171386,
            2072156101,
            1739845664,
            728614474,
            1026149675,
            943752001,
            1132262048,
            296357396,
            1770332643,
            989381445,
            92612664,
            1941029835,
            1283517198,
            1098802077
        };

        public static readonly List<Vector3> MotoCrossLocation = new List<Vector3>()
        {
            new((float)895.5380, (float)3088.0610, (float)40.7704), // 934 Dirt Roads west of Sandy Airstrip
            new((float)2369.7366, (float)4903.3896, (float)42.0104), // 2025 Dirt Road surrounding farmhouse
            new((float)264.9357, (float)6780.5161, (float)15.7907), // 3026 Dirt path outside of paleto
            new((float)1102.3326, (float)2120.6436, (float)53.4040), // 938 Construction site
            new((float)2768.8110, (float)2757.7349, (float)43.6497), // 962 Davis Quartz Quarry
            new((float)2234.3928, (float)1836.1216, (float)108.5627), // 726 Windmill farm dirt tracks 
            new((float)1524.0972, (float)-2048.1135, (float)77.3112), // 54 Oil fields dirt tracks 
            new((float)81.1038, (float)3080.5857, (float)41.2422), // 918 Dirt Track next to zancudo river
            new((float)-684.1599, (float)5312.1455, (float)68.8616) // 3002 Dirt track near lumberyard
        };

        public static readonly List<Vector3> BeachPartyLocations = new List<Vector3>()
        {
            new((float)-1786.7075, (float)-882.5734, (float)6.7890), // 610 opposite yacht
            new((float)-1312.5596, (float)-1708.2024, (float)2.1996), // 311 Near to LFB station 
            new((float)-3245.8406, (float)1196.2988, (float)2.9479), // 902 
            new((float)-3095.5532, (float)613.9554, (float)1.9885), // 803 
            new((float)-2593.9148, (float)3571.8916, (float)5.7154), // 1002 Tents
            new((float)-549.0706, (float)6375.0742, (float)3.2513), // 3009 
            new((float)325.3460, (float)6933.6689, (float)3.9514), // 3026
            new((float)1533.5099, (float)6629.1880, (float)2.4582), // 3028 Secluded Beach
            new((float)3207.6436, (float)5345.9692, (float)7.0263), // 2027 
            new((float)3860.7368, (float)4406.4946, (float)3.2928), // 1053
            new((float)1606.3455, (float)-2720.8052, (float)2.0432), // 56
            new((float)-2116.0227, (float)-490.0324, (float)3.5547), // 604
            new((float)-1587.5515, (float)-1109.0594, (float)4.0599), // 611
            new((float)-1406.7700, (float)-1520.4910, (float)2.5827) // 306
        };

        public static readonly List<Vector3> MissingPersonLocation = new List<Vector3>()
        {
            new((float)-1515.0344, (float)4469.5107, (float)17.8297), // 2002 Canyon dirt track near forest
            new((float)-1172.7668, (float)4654.9810, (float)144.6009), // 2003 Path between cliffs 
            new((float)-874.8124, (float)4797.9663, (float)300.4605), // 3001 Path next to Chilliad rescue center
            new((float)-601.3926, (float)2091.9600, (float)131.3669), // 913 Path near abondoned mineshaft
            new((float)-443.1310, (float)1589.6731, (float)358.1042), //703 Dirt track near  to the burger van
            new((float)925.6339, (float)5639.3252, (float)652.5438), // 3029 Path down mount chilliad
            new((float)3092.6848, (float)5972.8047, (float)142.4781), // 2027 Path next to mount Gordo
            new((float)2881.2336, (float)2245.8518, (float)132.9766), // 960 Path south of Davis Quartz
            new((float)2278.4092, (float)2170.8711, (float)78.1118), // 724 Path through windmill farm
            new((float)2963.8438, (float)5329.7275, (float)100.9349), // 2027 Campsite on Mt. Gordo
            new((float)2813.3079, (float)-606.5860, (float)2.4760), // 450 Beach with dirt road leading to 
            new((float)1834.2070, (float)-2682.6917, (float)2.1140), // 56 Beach with access north of docks
            new((float)1916.5533, (float)442.5359, (float)162.7040), // 723 Tataviam mountains in the dam area
            new((float)-2448.2524, (float)2835.8035, (float)3.5121), // 1003 Path near to the army base and marshland
            new((float)-1848.2993, (float)4395.9878, (float)51.3223) // 2002 Raton Canyon Trail path near bridge
        };


        public static readonly List<Vector3> HospitalBedLocation = new List<Vector3>()
        {
            // St Thomas 201 Hospital Beds
            new((float)322.9680, (float)-582.7566, (float)43.2841),
            new((float)319.1501, (float)-585.5240, (float)43.2840),
            new((float)315.8541, (float)-584.4513, (float)43.2840),
            new((float)312.5653, (float)-583.1465, (float)43.2840),
            new((float)309.0680, (float)-581.7347, (float)43.2840),
            new((float)310.4835, (float)-577.9717, (float)43.2841),
            new((float)312.4567, (float)-579.1171, (float)43.2841),
            new((float)320.0087, (float)-582.2339, (float)43.2841),
            new((float)322.6245, (float)-582.5547, (float)43.2841),

            // Mount Zonah 507 Hospital Beds

            new((float)-468.9680, (float)-285.9259, (float)34.9114),
            new((float)-466.0177, (float)-284.3730, (float)34.9123),
            new((float)-462.0856, (float)-282.6599, (float)34.9133),
            new((float)-458.5534, (float)-281.3787, (float)34.9143),
            new((float)-454.4280, (float)-280.0496, (float)34.9143),
            new((float)-452.1073, (float)-283.6190, (float)34.9122),
            new((float)-455.6661, (float)-284.7028, (float)34.9124),
            new((float)-460.8416, (float)-287.1781, (float)34.9122),
            new((float)-464.1729, (float)-288.4950, (float)34.9122),
            new((float)-467.6928, (float)-289.9973, (float)34.9113),

            // Sandy Hospital 1029
            new((float)1828.3928, (float)3671.6372, (float)34.2805),
            new((float)1825.4586, (float)3669.9465, (float)34.2805),
            new((float)1822.6991, (float)3668.3379, (float)34.2805),
            new((float)1820.6102, (float)3671.7065, (float)34.2805),
            new((float)1826.4092, (float)3674.8787, (float)34.2805),

            // Victoria Medical Centre 3011
            new((float)-253.6003, (float)6313.2607, (float)32.4586),
            new((float)-251.6385, (float)6311.3506, (float)32.4587),
            new((float)-255.2810, (float)6307.5615, (float)32.4587),
            new((float)-257.4031, (float)6309.7002, (float)32.4587),
            new((float)-259.6578, (float)6312.0210, (float)32.4587)
        };

        public static List<Tuple<Vector3, Vector3>> BreakdownLocations = new List<Tuple<Vector3, Vector3>>()
        {
            new Tuple<Vector3, Vector3>(new Vector3(711.95f, -158.28f, 49.28f), new Vector3(715.85f, -159.90f, 49.27f)),
            new Tuple<Vector3, Vector3>(new Vector3(877.78f, 167.67f, 74.21f), new Vector3(869.52f, 171.74f, 74.39f)),
            new Tuple<Vector3, Vector3>(new Vector3(1741.88f, 1528.69f, 84.48f),
                new Vector3(1738.37f, 1529.60f, 84.65f)),
            new Tuple<Vector3, Vector3>(new Vector3(2472.28f, 867.44f, 91.60f), new Vector3(2465.74f, 864.35f, 91.62f)),
            new Tuple<Vector3, Vector3>(new Vector3(2813.35f, 3466.42f, 54.46f),
                new Vector3(2815.10f, 3471.17f, 55.13f)),
            new Tuple<Vector3, Vector3>(new Vector3(1601.80f, 1064.89f, 79.89f),
                new Vector3(1604.45f, 1068.17f, 80.70f)),
            new Tuple<Vector3, Vector3>(new Vector3(760.44f, 14.99f, 63.14f), new Vector3(756.93f, 12.89f, 63.72f)),
            new Tuple<Vector3, Vector3>(new Vector3(892.25f, -1193.87f, 45.93f),
                new Vector3(895.38f, -1192.33f, 46.80f)),
            new Tuple<Vector3, Vector3>(new Vector3(1902.29f, -772.86f, 82.59f),
                new Vector3(1907.44f, -773.33f, 83.59f)),
            new Tuple<Vector3, Vector3>(new Vector3(2366.93f, -338.50f, 84.51f),
                new Vector3(2369.60f, -335.92f, 85.21f)),
            new Tuple<Vector3, Vector3>(new Vector3(2243.78f, 1201.76f, 76.36f),
                new Vector3(2245.18f, 1197.78f, 77.11f)),
            new Tuple<Vector3, Vector3>(new Vector3(2424.86f, 2890.39f, 39.47f),
                new Vector3(2422.88f, 2888.42f, 40.22f)),
            new Tuple<Vector3, Vector3>(new Vector3(2707.49f, 4853.22f, 43.85f),
                new Vector3(2709.13f, 4850.78f, 44.61f)),
            new Tuple<Vector3, Vector3>(new Vector3(2263.18f, 5897.92f, 48.25f),
                new Vector3(2265.01f, 5895.72f, 48.94f)),
            new Tuple<Vector3, Vector3>(new Vector3(1600.73f, 6407.58f, 25.54f),
                new Vector3(1602.85f, 6406.39f, 26.55f)),
            new Tuple<Vector3, Vector3>(new Vector3(1083.59f, 6500.52f, 20.30f),
                new Vector3(1087.19f, 6500.78f, 21.06f)),
            new Tuple<Vector3, Vector3>(new Vector3(1001.01f, 6476.60f, 20.23f),
                new Vector3(997.55f, 6476.68f, 20.98f)),
            new Tuple<Vector3, Vector3>(new Vector3(-2326.75f, 4155.23f, 37.51f),
                new Vector3(-2326.23f, 4158.75f, 38.35f)),
            new Tuple<Vector3, Vector3>(new Vector3(-2643.44f, 2810.08f, 15.94f),
                new Vector3(-2643.87f, 2816.87f, 16.69f)),
            new Tuple<Vector3, Vector3>(new Vector3(1818.09f, 2274.27f, 52.94f),
                new Vector3(1817.30f, 2279.26f, 53.62f)),
            new Tuple<Vector3, Vector3>(new Vector3(-401.50f, -715.13f, 36.43f),
                new Vector3(-403.42f, -716.57f, 37.06f)),
            new Tuple<Vector3, Vector3>(new Vector3(187.96f, -513.01f, 33.14f), new Vector3(190.80f, -512.12f, 33.86f)),
            new Tuple<Vector3, Vector3>(new Vector3(2229.30f, -503.29f, 91.08f),
                new Vector3(2234.96f, -500.46f, 91.60f)),
            new Tuple<Vector3, Vector3>(new Vector3(2055.40f, 1477.51f, 74.83f),
                new Vector3(2051.97f, 1482.40f, 75.53f))
        };
    }
}
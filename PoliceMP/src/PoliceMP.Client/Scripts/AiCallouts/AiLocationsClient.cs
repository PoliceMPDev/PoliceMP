using System.Collections;
using System.Collections.Generic;
using CitizenFX.Core;

namespace PoliceMP.Client.Scripts.AiCallouts
{
    public class AiLocationsClient : IEnumerable<Vector3>
    {
        public static readonly List<Vector3> MeleShopLocations = new List<Vector3>()
        {
            new((float)372.19, (float)326.52, (float)103.56), //574 - Window Shop Till
            new((float)372.93, (float)328.87, (float)103.56), //574 - Shop Next Till
            new((float)378.15, (float)333.16, (float)103.56), //574 - Safe in the Back of Shop

            new((float)-47.66, (float)-1759.49, (float)29.42), //120 - Window Shop Till
            new((float)-46.36, (float)-1758.02, (float)29.42), //120 - Shop Next Till
            new((float)-43.10, (float)-1748.55, (float)29.42), //120 - Safe in the Back of Shop

            new((float)1165.22, (float)-324.35, (float)69.20), //411 - Window Shop Till
            new((float)1165.10, (float)-322.57, (float)69.20), //411 - Shop Next Till
            new((float)1159.85, (float)-313.86, (float)69.20), //411 - Safe in the Back of Shop

            new((float)-1221.83, (float)-908.52, (float)12.32), //333 - Corner Shop Till
            new((float)-1220.61, (float)-915.83, (float)11.32), //333 - Corner Shop Till

            new((float)-1194.16, (float)-895.63, (float)13.99), //333 - McDonalds Far Fight Till
            new((float)-1195.40, (float)-893.83, (float)13.99), //333 - McDonalds Middle Till
            new((float)-1196.65, (float)-891.76, (float)13.99), //333 - McDonalds Left Till

            new((float)1698.25, (float)3781.99, (float)34.73), //1022 - McDonalds Far Right Till
            new((float)1696.95, (float)3781.18, (float)34.73), //1022 - McDonalds Middle Till
            new((float)1695.87, (float)9780.28, (float)34.73), //1022 - McDonalds Left Till

            new((float)-2180.76, (float)4297.38, (float)49.15), //2001 - McDonalds Left Till
            new((float)-2180.01, (float)4298.64, (float)49.15), //2001 - McDonalds Left Till
            new((float)-2182.77, (float)4281.85, (float)49.15), //2001 - McDonalds Right Till
            new((float)-2184.11, (float)4282.57, (float)49.15), //2001 - McDonalds Right Till

            new((float)-1195.46, (float)-767.87, (float)17.31), //341 - Urban Wear - Till Nearest Door
            new((float)-1193.99, (float)-766.81, (float)17.31), //341 - Urban Wear - Middle Till
            new((float)-1192.55, (float)-765.77, (float)17.31), //341 - Urban Wear - Till Near Dressing Room

            new((float)-709.09, (float)-151.19, (float)37.41), //696 - Clothing Store - Till

            new((float)423.04, (float)-811.86, (float)29.49), //208 - Clothing Store - Till
            new((float)426.16, (float)-811.76, (float)29.49), //208 - Clothing Store - Till


            new((float)-622.49, (float)-230.09, (float)38.05), //697 - Jewelery Store - Till

            new((float)24.26, (float)-1347.31, (float)29.49), //125 - Window Shop Till
            new((float)24.38, (float)-1344.95, (float)29.49), //125 - Shop Till
            new((float)28.24, (float)-1339.51, (float)29.49), //125 - Safe in the Back of Shop

            new((float)-705.93, (float)-915.37, (float)19.21), //366 - Window Shop Till
            new((float)-705.95, (float)-913.49, (float)19.21), //366 - Shop Till
            new((float)-709.34, (float)-904.06, (float)19.21), //366 - Safe in the Back of Shop

            new((float)1392.81, (float)3606.59, (float)34.98), //1016 - Weird Dive Bar Till

            new((float)1728.65, (float)6417.39, (float)35.03), //3030 - Shop Till
            new((float)1727.82, (float)6415.17, (float)35.03), //3030 - Window Shop Till
            new((float)1734.69, (float)6420.57, (float)35.03), //3030 - Safe in the back of shop

            new((float)-2184.11, (float)4282.57, (float)49.15), //804 - Shop Till
            new((float)-3041.23, (float)583.62, (float)7.90), //804 - Shop Till
            new((float)-3047.44, (float)585.70, (float)7.90), //804 - Safe in the back of shop
        };

        //Safes that are located inside of stores
        /// <summary>
        /// A static class that contains the locations of Shop Safes.
        /// </summary>
        public static readonly List<Vector3> ShopSafes = new List<Vector3>()
        {
            new((float)-709.34, (float)-904.06, (float)19.21), //366 - Safe in the Back of Shop
            new((float)28.24, (float)-1339.51, (float)29.49), //125 - Safe in the Back of Shop
            new((float)1159.85, (float)-313.86, (float)69.20), //411 - Safe in the Back of Shop
            new((float)-43.10, (float)-1748.55, (float)29.42), //120 - Safe in the Back of Shop
            new((float)378.15, (float)333.16, (float)103.56), //574 - Safe in the Back of Shop
            new((float)1734.69, (float)6420.57, (float)35.03), //3030 - Safe in the back of shop
            new((float)-3047.44, (float)585.70, (float)7.90), //804 - Safe in the back of shop
        };

        //Weird Meth Lab Area in Sandy - 1016
        /// <summary>
        /// This class contains the coordinates of a meth lab in Sandy.
        /// </summary>
        public static readonly List<Vector3> SandyMethLab = new List<Vector3>()
        {
            new((float)1392.32, (float)3606.19, (float)38.94), //1016 Meth Lab around tables
            new((float)1390.10, (float)3608.62, (float)38.94), //1016 Meth Lab around tables
            new((float)1388.87, (float)3605.48, (float)38.94), //1016 Meth Lab around tables
            new((float)1389.42, (float)3603.43, (float)38.91), //1016 Meth Lab around tables
            new((float)1394.32, (float)3601.95, (float)38.94), //1016 Meth Lab around tables
        };


        //Ammunation Stores - Gun Shops
        /// <summary>
        /// Contains the locations of Ammunation stores.
        /// </summary>
        public static readonly List<Vector3> AmmunationStores = new List<Vector3>()
        {
            new((float)-660.93, (float)-933.61, (float)21.82), //366 - Ammunation Till
            new((float)-66.21, (float)-937.90, (float)21.82), //366 - Ammunation Till Left Side

            new((float)23.87, (float)-1105.76, (float)29.79), //200 - Ammunation Till
            new((float)17.34, (float)-1108.33, (float)29.79), //200 - Ammunation Till Left Side

            new((float)1692.74, (float)3762.28, (float)34.70), //1020 - Ammunation Till
            new((float)1692.82, (float)3755.11, (float)34.70), //1020 - Ammunation Till Left Side

            new((float)-331.02, (float)6086.18, (float)31.45), //3007 - Ammunation Till
            new((float)-331.25, (float)6079.09, (float)31.45), //3007 - Ammunation Till Left Side
        };


        //Armed Robbery Spawn Location - For Robberies
        /// <summary>
        /// Stores the locations of alarm panels.
        /// </summary>
        public static readonly List<Vector3> AlarmPanel = new List<Vector3>()
        {
            new((float)-1196.65, (float)-891.76, (float)13.99), //333 - McDonalds Alarm Panel
            new((float)-628.91, (float)-228.00, (float)38.05), //697 - Jewelery Store Alarm Panel
            new((float)-709.97, (float)-906.70, (float)19.21), //366 - Shop Store Alarm Panel / Electric Box
        };


        //Jewelry Store Diamond Counters - Postal 697
        /// <summary>
        /// Location coordinates of the jewelry store counters.
        /// </summary>
        public static readonly List<Vector3> JeweleryStoreCounters = new List<Vector3>()
        {
            //Ones when you first walk in
            new((float)-626.87, (float)-233.12, (float)38.05), //697 - Jewelery Store Counter
            new((float)-627.90, (float)-233.86, (float)38.05), //697 - Jewelery Store Counter
            new((float)-626.76, (float)-235.46, (float)38.05), //697 - Jewelery Store Counter
            new((float)-625.63, (float)-234.75, (float)38.05), //697 - Jewelery Store Counter

            //Ones on the right when you first walk in
            new((float)-626.87, (float)-238.37, (float)38.05), //697 - Jewelery Store Counter
            new((float)-625.67, (float)-237.73, (float)38.05), //697 - Jewelery Store Counter

            //Center stuff in a square sourrounding the tills
            new((float)-623.07, (float)-232.96, (float)38.05), //697 - Jewelery Store Counter
            new((float)-624.58, (float)-231.05, (float)38.05), //697 - Jewelery Store Counter
            new((float)-624.09, (float)-228.14, (float)38.05), //697 - Jewelery Store Counter
            new((float)-620.97, (float)-228.37, (float)38.05), //697 - Jewelery Store Counter
            new((float)-619.49, (float)-230.39, (float)38.05), //697 - Jewelery Store Counter
            new((float)-620.15, (float)-233.38, (float)38.05), //697 - Jewelery Store Counter

            //Fair right corner of the store
            new((float)-619.15, (float)-233.61, (float)38.05), //697 - Jewelery Store Counter
            new((float)-620.31, (float)-234.42, (float)38.05), //697 - Jewelery Store Counter

            //Fair Left corner of the store
            new((float)-624.87, (float)-228.00, (float)38.05), //697 - Jewelery Store Counter
            new((float)-623.68, (float)-227.28, (float)38.05), //697 - Jewelery Store Counter

            //Fair back counters within the store
            new((float)-620.55, (float)-226.80, (float)38.05), //697 - Jewelery Store Counter
            new((float)-619.77, (float)-227.72, (float)38.05), //697 - Jewelery Store Counter
            new((float)-618.37, (float)-229.62, (float)38.05), //697 - Jewelery Store Counter
            new((float)-617.63, (float)-230.65, (float)38.05), //697 - Jewelery Store Counter
        };

        public static readonly List<Vector3> DrugAlleywaysFoot = new List<Vector3>()
        {
            new((float)452.4977, (float)-1559.9529, (float)29.2827), // 151 - Alleyway behind apartments
            new((float)-856.5140, (float)-1094.6940, (float)2.1630), // 354 - Deadend Alley next to canals
            new((float)-1041.1968, (float)-1005.0609, (float)2.1502), // 345 - Beneath building overhang
            new((float)-1172.9926, (float)-1011.4686, (float)2.1502), // 334 - Path by the canal
            new((float)-1270.8197, (float)-136.9025, (float)43.0750), // 657 - Path next to tennis courts
            new((float)94.3746, (float)154.6738, (float)104.7652), // 568 - Alley next to bins
            new((float)1803.5883, (float)4602.8862, (float)37.6828), // 2018 - Next to store#
            new((float)1532.2180, (float)3600.3477, (float)35.2413), // 1019 - Abandoned hotel
            new((float)1794.2769, (float)3390.7454, (float)41.4662), // 1020 - Trailers across from Sandy airstrip
            new((float)2481.5432, (float)3764.0100, (float)41.6043), // 1040 - Alien/Crazy skatepark
            new((float)1902.8296, (float)3908.1714, (float)32.6081), // 1034 - Path between caravans
            new((float)1972.2203, (float)3766.1099, (float)32.1887), // 1036 - Petrol station behind trailers
            new((float)378.7957, (float)2574.9221, (float)43.5195), // 926 - Caravans just off dirt road
            new((float)261.8908, (float)2576.6011, (float)45.0988), // 920 - Behind petrol station
            new((float)209.6519, (float)2750.9434, (float)43.4264), // 919 - Trailer park 
            new((float)-307.1079, (float)6277.6089, (float)31.4923), // 3011 - Behind store 
            new((float)-219.7518, (float)6254.6094, (float)31.4896), // 3012 - Behind pixel petes 
            new((float)-299.2579, (float)6192.6821, (float)31.4894), // 3010 - Alley behind shops next to church
            new((float)-162.5282, (float)6431.8838, (float)31.9098), // 3015 - Seating area next to pool by the motel
            new((float)-87.1912, (float)6390.0566, (float)31.4904), // 3016 - Small Alleyways 
            new((float)-112.3899, (float)6356.7773, (float)31.4904), // 3016 - Behind Motel
            new((float)1682.3881, (float)6434.9482, (float)32.1507), // 3030 - Petrol station
            new((float)1510.0817, (float)6333.8716, (float)23.9198), // 3029 - Caravan/dodgy house
            new((float)-682.9885, (float)5797.6318, (float)17.3310), // 3004 - Behind bayview lodge
            new((float)-736.8797, (float)5559.3687, (float)36.7096), // 3003 - Pala Springs 
            new((float)-2166.4451, (float)4281.5796, (float)48.9572), // 2001 - Behind Hookies 
            new((float)-3187.9451, (float)1035.5757, (float)20.8188), // 908 - Behind stores 
            new((float)-1988.2900, (float)-333.0672, (float)32.0978), // 603 - Alley next to hotel 
            new((float)-1803.5970, (float)-400.5419, (float)44.7257), // 615 - Alley between apartments 
            new((float)-1495.9232, (float)-184.1086, (float)50.3970), // 643 - Corner off the market square
            new((float)-483.6311, (float)-56.5885, (float)39.9942), // 518 - Corner of alleyway next to bins 
            new((float)-493.0799, (float)-26.2965, (float)44.5166), // 518 Deadend alley 
            new((float)-1159.1084, (float)-1555.5221, (float)4.3079), // 308 - Corner of alley 
            new((float)-1338.4346, (float)-1143.7477, (float)4.3433), // 303 - Path between houses 
            new((float)-1861.3983, (float)-611.4549, (float)11.6073), // 303 - Bike rack next to the 
        };


        public static readonly List<Vector3> DrugAlleywaysVehicle = new List<Vector3>()
        {
            new((float)-183.2334, (float)-1294.7688, (float)31.2960), // 101 - Alleys Near Benny's 
            new((float)-50.2034, (float)-1418.4977, (float)29.3273), // 126 - Alleyway behind houses
            new((float)-53.1149, (float)-1508.5887, (float)31.5339), // 117 - Alleyway next to apartments
            new((float)28.5125, (float)-1032.5927, (float)29.3914), // 200 - Alleyway behind parking lot
            new((float)307.6911, (float)-999.1860, (float)29.2565), // 210 - Alleyway off Strawberry Ave
            new((float)-1289.6288, (float)-809.8370, (float)17.5410), // 323 - Large Alleyway
            new((float)-1396.2623, (float)-654.5282, (float)28.6734), // 628 - Alley between 628 & 629
            new((float)-1378.3037, (float)-449.3959, (float)34.4776), // 636 - Alleyway Behind shops
            new((float)-1331.3572, (float)-230.1282, (float)42.8644), // 654 - Alley next to parking lot
            new((float)-783.1383, (float)-192.6352, (float)37.2836), // 682 - Alley behind shops
            new((float)-469.7572, (float)75.8014, (float)58.6615), // 527 - Alley next to parking garage 
            new((float)-458.2228, (float)304.5426, (float)83.2481), // 524 - Alley/Parking spaces 
            new((float)64.9876, (float)149.5397, (float)104.5983), // 568 - Alley parking spaces
            new((float)1341.4174, (float)4372.3345, (float)44.3438), // 2008 - Dirt road near Boat store
            new((float)2654.3083, (float)3454.7595, (float)55.6740), // 956 - Road next to large store
            new((float)2316.3584, (float)2594.2739, (float)46.6758), // 959 - Caravan park at back
            new((float)1982.4641, (float)3782.7117, (float)32.1808), // 1036 - Petrol station parking
            new((float)365.9795, (float)2632.5576, (float)44.4977), // 925 - Parking lot for motel 
            new((float)-263.4249, (float)6065.4653, (float)31.4644), // 3008 - Parking lot next to chicken factory
            new((float)-320.8769, (float)6102.2578, (float)31.4641), // 3007 - Parking of Ammunation
            new((float)-324.9991, (float)6138.5151, (float)31.4913), // 3007 - Parking lot of church 
            new((float)-174.4815, (float)6444.0078, (float)31.5117), // 3015 - Parking of motel
            new((float)-25.7216, (float)6401.3755, (float)31.4904), // 3020 - Parking of Morris & Sons
            new((float)-96.0932, (float)6344.5796, (float)31.4904), // 3016 - Parking of motel 
            new((float)-697.7731, (float)5779.8481, (float)17.3310), // 3004 - Parking of bayview lodge
            new((float)-758.5710, (float)5546.4292, (float)33.4857), // 3003 - Parking lot of pala springs
            new((float)-2197.4980, (float)4244.7861, (float)47.8587), // 3002 - Parking of Hookies next to storage 
            new((float)-3152.8464, (float)1088.5918, (float)20.7042), // 908 - Parking of shops 
            new((float)-2967.6704, (float)60.8354, (float)11.6085), // 811 - Parking of Country club 
            new((float)-1987.9985, (float)-295.5878, (float)48.1058), // 603 - Parking of The Jetty 
            new((float)-1262.0378, (float)-268.0666, (float)38.9405), // 655 - Alley next to parking lot 
            new((float)-1319.0632, (float)-1148.9155, (float)4.4989), // 303 - Parking lot 
            new((float)-1859.9575, (float)-628.7148, (float)11.2301), // 607 - Parking next to footbridge 
        };


        private readonly List<Vector3> MetroStops = new()
        {
            new Vector3(-537.81665039062f, -1265.0319824219f, 25.905330657959f),
            new Vector3(-1088.627f, -2709.362f, -7.137033f),
            new Vector3(1081.309f, 2725.259f, 7.137033f),
            new Vector3(-889.2755f, -2311.825f, -11.45941f),
            new Vector3(-876.7512f, -2323.808f, -11.45609f),
            new Vector3(-545.3138f, -1280.548f, 27.09238f),
            new Vector3(-536.8082f, -1286.096f, 27.08238f),
            new Vector3(270.2029f, -1210.818f, 39.25398f),
            new Vector3(265.3616f, -1198.051f, 39.23406f),
            new Vector3(-286.3837f, -318.877f, 10.33625f),
            new Vector3(-302.6719f, -322.995f, 10.33629f),
            new Vector3(-826.3845f, -134.7151f, 20.22362f),
            new Vector3(-816.7159f, -147.4567f, 20.2231f),
            new Vector3(-1351.282f, -481.2916f, 15.318f),
            new Vector3(-1341.085f, -467.674f, 15.31838f),
            new Vector3(-496.0209f, -681.0325f, 12.08264f),
            new Vector3(-495.8456f, -665.4668f, 12.08244f),
            new Vector3(-218.2868f, -1031.54f, 30.51112f),
            new Vector3(-209.6845f, -1037.544f, 30.50939f),
            new Vector3(112.3714f, -1729.233f, 30.24097f),
            new Vector3(120.0308f, -1723.956f, 30.31433f)
        };

        public IEnumerator<Vector3> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
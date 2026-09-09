using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using CitizenFX.Core.UI;
using PoliceMP.Client.Actions.HandsUp;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Scripts.HideBlips;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Client.Overlays.NewNotification;
using System.Globalization;

namespace PoliceMP.Client.Scripts.CCTV
{
    public class CCTV : Script
    {
        private readonly ICommandManager _commandManager;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<CCTV> _logger;

        private Vector3 startingCoords;
        private Vector3 controlCCTVRoom = new Vector3(1225, -1489, 34);
        private Vector3 highwaysCCTVRoom = new Vector3(1542f, 828f, 77f);
        private Vector3 cidCCTVRoom = new Vector3(-1097.522f, -817.752f, 19.036f);

        public bool cctvCamsActive = false;
        private float startingMumbleProx;
        private int currentCameraIndex = 1;


        //private Dictionary <int, Vector3> cameraPositions;

        private Dictionary<int, int> cameraBlipHandles = new Dictionary<int, int>();

        private Dictionary<int, Vector3> cameraPositions = new Dictionary<int, Vector3>();

        #region Highways Cam Pos
        private readonly Dictionary<int, Vector3> highwaysCameraPositions = new Dictionary<int, Vector3>
        {
            { 1, new Vector3(620.91888427734f, -277.529296875f, 48.944190979004f) }, // 407
            { 2, new Vector3(724.86267089844f, -94.622947692871f, 63.240177154541f) }, // 405
            { 3, new Vector3(900.21417236328f, 203.03997802734f, 87.019462585449f) }, // 400
            { 4, new Vector3(1129.6348876953f, 423.19888305664f, 91.720329284668f) }, // 400
            { 5, new Vector3(1281.5205078125f, 574.19134521484f, 88.74210357666f) }, // 723
            { 6, new Vector3(1533.1782226563f, 813.37426757813f, 94.626747131348f) }, // M25 Base
            { 7, new Vector3(1622.03125f, 1113.1857910156f, 90.922584533691f) }, // 722
            { 8, new Vector3(1696.9979248047f, 1448.4113769531f, 93.960121154785f) }, // 721
            { 9, new Vector3(1746.6407470703f, 1852.6041259766f, 83.747268676758f) }, // 721
            { 10, new Vector3(1953.7932128906f, 2505.0012207031f, 63.211589813232f) }, // 724
            { 11, new Vector3(2184.21875f, 2730.2233886719f, 56.344619750977f) }, // 950
            { 12, new Vector3(2405.9411621094f, 2905.3298339844f, 56.957553863525f) }, // 954
            { 13, new Vector3(2725.4760742188f, 3275.5769042969f, 64.219856262207f) }, // 957
            { 14, new Vector3(2882.630859375f, 3632.0739746094f, 61.394016265869f) }, // 961
            { 15, new Vector3(2845.36328125f, 4251.3071289063f, 59.119045257568f) }, // 1047
            { 16, new Vector3(2752.0053710938f, 4543.5390625f, 55.213062286377f) }, // 1048
            { 17, new Vector3(2626.3247070313f, 5102.6577148438f, 53.215511322021f) }, // 2025
            { 18, new Vector3(2449.4187011719f, 5665.2890625f, 53.59651184082f) }, // 3031
            { 19, new Vector3(2059.7309570313f, 6075.9248046875f, 58.87678527832f) }, // 3030
            { 20, new Vector3(1581.0870361328f, 6395.6157226563f, 35.583709716797f) }, // 3030
            { 21, new Vector3(1031.2681884766f, 6489.6508789063f, 31.182203292847f) }, // 3028
            { 22, new Vector3(478.27407836914f, 6561.9345703125f, 35.766971588135f) }, // 3027
            { 23, new Vector3(-292.0485534668f, 6085.4604492188f, 39.893001556396f) }, // 3007
            { 24, new Vector3(-516.22833251953f, 5792.4301757813f, 45.499977111816f) }, // 3004
            { 25, new Vector3(-941.36828613281f, 5436.3461914063f, 47.399715423584f) }, // 3003
            { 26, new Vector3(-1385.5216064453f, 5106.4663085938f, 71.177574157715f) }, // 3001
            { 27, new Vector3(-1652.2587890625f, 4844.5336914063f, 69.602485656738f) }, // 3001
            { 28, new Vector3(-1996.7144775391f, 4522.1044921875f, 65.408073425293f) }, // 2001
            { 29, new Vector3(-2307.2133789063f, 4130.0307617188f, 45.82250213623f) }, // 2001
            { 30, new Vector3(-2549.2980957031f, 3467.6596679688f, 23.502994537354f) }, // 1002
            { 31, new Vector3(-2658.7468261719f, 2630.4548339844f, 25.438980102539f) }, // 1001
            { 32, new Vector3(-2612.0087890625f, 2321.0590820313f, 36.926029205322f) }, // 1001
            { 33, new Vector3(-3008.9516601563f, 1912.9826660156f, 42.165630340576f) }, // 910
            { 34, new Vector3(-3033.3259277344f, 1460.3560791016f, 40.7038230896f) }, // 909
            { 35, new Vector3(-3138.4128417969f, 905.97204589844f, 27.901445388794f) }, // 906
            { 36, new Vector3(-3018.3889160156f, 249.67915344238f, 25.20269203186f) }, // 810
            { 37, new Vector3(-2781.6354980469f, 12.770753860474f, 28.386144638062f) }, // 811
            { 38, new Vector3(-2470.7021484375f, -217.56637573242f, 26.67804145813f) }, // 601
            { 39, new Vector3(-2005.7021484375f, -441.19848632813f, 19.710063934326f) }, // 604
            { 40, new Vector3(-1911.7828369141f, -517.82092285156f, 20.025062561035f) }, // 605
            { 41, new Vector3(-1706.5053710938f, -683.20129394531f, 19.395065307617f) }, // 609
            { 42, new Vector3(-1107.1706542969f, -634.615234375f, 21.766136169434f) }, // Postal 320
            { 43, new Vector3(-1039.9356689453f, -597.30163574219f, 26.381191253662f) }, // 647
            { 44, new Vector3(-797.64532470703f, -516.80908203125f, 34.345439910889f) }, // 364
            { 45, new Vector3(-444.96646118164f, -513.52465820313f, 33.841442108154f) }, // 509
            { 46, new Vector3(-23.710075378418f, -510.3440246582f, 41.349319458008f) }, // 395
            { 47, new Vector3(210.60090637207f, -510.1770324707f, 42.03532409668f) }, // 396
            { 48, new Vector3(435.40615844727f, -511.89465332031f, 43.996852874756f) }, // 207
            { 49, new Vector3(649.64300537109f, -581.50610351563f, 43.989032745361f) }, // 219
            { 50, new Vector3(846.60217285156f, -685.56744384766f, 51.540775299072f) }, // 419
            { 51, new Vector3(1020.500793457f, -932.53540039063f, 38.581916809082f) }, // 449
            { 52, new Vector3(1053.5606689453f, -1197.4625244141f, 43.077739715576f) }, // 175
            { 53, new Vector3(1067.3548583984f, -1376.5777587891f, 38.903732299805f) }, // 177
            { 54, new Vector3(1086.2392578125f, -1678.7960205078f, 37.431625366211f) }, // 179
            { 55, new Vector3(1167.4307861328f, -1893.3563232422f, 41.614440917969f) }, // 181
            { 56, new Vector3(1256.9903564453f, -2095.671875f, 52.691192626953f) }, // 53
            { 57, new Vector3(1333.8572998047f, -2350.4594726563f, 59.987281799316f) }, // 51
            { 58, new Vector3(1283.0603027344f, -2490.4260253906f, 53.133430480957f) }, // 52
            { 59, new Vector3(964.73260498047f, -2613.2160644531f, 64.117729187012f) }, // 66
            { 60, new Vector3(808.40216064453f, -2612.21484375f, 60.683944702148f) }, // 62
            { 61, new Vector3(606.50799560547f, -2672.1474609375f, 51.722061157227f) }, // 10
            { 62, new Vector3(368.3464050293f, -2675.6018066406f, 28.356731414795f) }, // 22
            { 63, new Vector3(110.39122772217f, -2648.12109375f, 28.104732513428f) }, // 34
            { 64, new Vector3(-141.29907226563f, -2516.1823730469f, 54.544662475586f) }, // 49
            { 65, new Vector3(-438.98150634766f, -2307.7211914063f, 71.326377868652f) }, // 87
            { 66, new Vector3(-744.83618164063f, -2083.9995117188f, 43.786861419678f) }, // 80
            { 67, new Vector3(-880.69036865234f, -2010.6651611328f, 35.825130462646f) }, // 95
            { 68, new Vector3(-914.646484375f, -1840.1641845703f, 43.936740875244f) }, // 94
            { 69, new Vector3(-752.69274902344f, -1744.9578857422f, 47.689258575439f) }, // 387
            { 70, new Vector3(-550.71862792969f, -1693.8641357422f, 46.683586120605f) }, // 386
            { 71, new Vector3(-413.44006347656f, -1528.8143310547f, 46.431587219238f) }, // 385
            { 72, new Vector3(-416.11773681641f, -1368.7825927734f, 45.297592163086f) }, // 384
            { 73, new Vector3(-399.09448242188f, -1221.7631835938f, 43.218601226807f) }, // 375
            { 74, new Vector3(-416.49392700195f, -1069.6594238281f, 45.084255218506f) }, // 383
            { 75, new Vector3(-404.62451171875f, -851.86407470703f, 46.76277923584f) }, // 378
            { 76, new Vector3(-407.05996704102f, -571.4130859375f, 49.771408081055f) }, // 380
            { 77, new Vector3(1868.0745849609f, 2101.9123535156f, 62.965175628662f) }, // 724
            { 78, new Vector3(1929.8555908203f, 1793.1934814453f, 74.103507995605f) }, // 725
            { 79, new Vector3(2086.1479492188f, 1411.0101318359f, 84.531784057617f) }, // 726
            { 80, new Vector3(2457.8645019531f, 984.85125732422f, 95.836288452148f) }, // 402
            { 81, new Vector3(2495.3176269531f, 809.16314697266f, 103.54877471924f) }, // 402
            { 82, new Vector3(2557.189453125f, 531.85534667969f, 117.13557434082f) }, // 402
            { 83, new Vector3(2546.6716308594f, 235.51173400879f, 114.25148010254f) }, // 402
            { 84, new Vector3(2499.3200683594f, -2.1549022197723f, 104.15481567383f) }, // 403
            { 85, new Vector3(2246.1015625f, -467.88092041016f, 98.965476989746f) }, // 403
            { 86, new Vector3(1995.5626220703f, -689.9267578125f, 100.82381439209f) }, // 191
            { 87, new Vector3(1543.3087158203f, -1000.5042114258f, 66.43009185791f) }, // 448
            { 88, new Vector3(1315.6563720703f, -1124.7784423828f, 59.811241149902f) }, // 449
            { 89, new Vector3(1110.2919921875f, -1181.9865722656f, 64.028106689453f) }, // 177
            { 90, new Vector3(793.11157226563f, -1194.2542724609f, 54.928169250488f) }, // 226
            { 91, new Vector3(566.62664794922f, -1203.76953125f, 51.22399520874f) }, // 218
            { 92, new Vector3(93.947975158691f, -1203.3839111328f, 46.36266708374f) }, // 133
            { 93, new Vector3(-199.46331787109f, -1208.9871826172f, 46.36266708374f) }, // 394
            { 94, new Vector3(-1567.2720f, -153.2270f, 64.0445f) }, 
            { 95, new Vector3(-1778.6760f, 54.4134f, 76.5977f) }, 
            { 96, new Vector3(-1963.8763f, 535.2936f, 122.2466f) }, 
            { 97, new Vector3(-1711.1047f, 835.8437f, 149.5526f) },
            { 98, new Vector3(-1648.7223f, 2140.7266f, 107.2531f) },
            { 99, new Vector3(-1304.0452f, 2501.2639f, 30.1210f) },
            { 100, new Vector3(-2151.4258f, 2319.8049f, 46.2625f) },
            { 101, new Vector3(-444.5724f, 2883.1829f, 43.5450f) },
            { 102, new Vector3(-4.2986f, 2804.2644f, 69.1084f) },
            { 103, new Vector3(299.8849f, 2622.4988f, 56.3893f) },
            { 104, new Vector3(1111.4120f, 2704.2346f, 56.7278f) },
            { 105, new Vector3(2035.1931f, 3025.3379f, 55.9852f) },
            { 106, new Vector3(2305.9941f, 2975.6763f, 58.4934f) },
        };
        #endregion

        #region Control Cam Pos
        private readonly Dictionary<int, Vector3> controlCameraPositions = new Dictionary<int, Vector3>
        {
            { 1, new Vector3(148.8306f, -1036.004f, 32.10903f) }, // Legion Bank
            { 2, new Vector3(106.2686f, -1085.547f, 33.14174f) }, // Legion Rear Carpark
            { 3, new Vector3(406.8656f, -1118.922f, 35.39988f) }, // Carpark near Mission Row
            { 4, new Vector3(494.4908f, -1120.785f, 34.17147f) }, // Road Behind Mission Row
            { 5, new Vector3(218.253f, -1459.104f, 35.63839f) }, // Postal 141
            { 6, new Vector3(104.935f,  -1421.315f, 33.85973f) }, // Postal 134 (1)
            { 7, new Vector3(83.9758f, -1384.53f, 33.85973f) }, // Postal 134 (2)
            { 8, new Vector3(143.9474f, -1294.748f, 32.54749f) }, // Postal 133
            { 9, new Vector3(467.6618f, -1271.253f, 33.87864f) }, // Postal 155
            { 10, new Vector3(452.2337f, -1453.483f, 34.73988f) }, // Postal 157
            { 11, new Vector3(288.5907f, -1253.381f, 31.52891f) }, // Postal 148
            { 12, new Vector3(-175.8597f, -1452.827f, 38.14526f) }, // Postal 103
            { 13, new Vector3(-90.8656f, -1353.946f, 35.04f) }, // Postal 115
            { 14, new Vector3(-225.422f, -1159.108f, 25.73917f) }, // Postal 394
            { 15, new Vector3(-27.20386f, -1118.306f, 29.09328f) }, // Postal 200
            { 16, new Vector3(-53.03136f, -963.426f, 34.99089f) }, // Postal 200
            { 17, new Vector3(-163.24995422363f, -877.55554199219f, 34.984024047852f) }, // Postal 391
            { 18, new Vector3(-321.39938354492f, -773.54528808594f, 54.546028137207f) }, // Postal 381 (Carpark)
            { 19, new Vector3(-215.97566223145f, -622.32537841797f, 36.680240631104f) }, // Postal 389
            { 20, new Vector3(-112.86187744141f, -592.66918945313f, 41.649635314941f) }, // Postal 389 (Hotel)
            { 21, new Vector3(54.020908355713f, -760.62438964844f, 49.890949249268f) }, // Postal 398
            { 22, new Vector3(68.236503601074f, -875.85485839844f, 32.516368865967f) }, // Postal 399
            { 23, new Vector3(42.349685668945f, -903.53131103516f, 31.57137298584f) }, // Postal 399 (2)
            { 24, new Vector3(-404.13555908203f, -851.30218505859f, 43.193244934082f) }, // Postal 378 (Motorway)
            { 25, new Vector3(-452.59292602539f, -670.23449707031f, 37.895755767822f) }, // Postal 378 (Motorway)
            { 26, new Vector3(-523.38018798828f, -824.48828125f, 36.166599273682f) }, // Postal 372
            { 27, new Vector3(-555.84704589844f, -909.208984375f, 35.640727996826f) }, // Postal 372 (Weazal)
            { 28, new Vector3(-554.49761962891f, -918.63439941406f, 32.61674118042f) }, // Postal 372 (Weazal 2)
            { 29, new Vector3(-547.4521484375f, -1099.841796875f, 28.177217483521f) }, // Postal 374
            { 30, new Vector3(-468.45779418945f, -1417.7758789063f, 35.196308135986f) }, // Postal 385
            { 31, new Vector3(-623.07019042969f, -1314.1451416016f, 16.508943557739f) }, // Postal 363
            { 32, new Vector3(-821.71966552734f, -1264.5103759766f, 10.786540031433f) }, // Postal 355
            { 33, new Vector3(-838.41961669922f, -1262.6086425781f, 11.794535636902f) }, // Postal 348
            { 34, new Vector3(-735.3583984375f, -1599.0257568359f, 28.480054855347f) }, // Postal 349
            { 35, new Vector3(-762.27789306641f, -1693.4122314453f, 33.994312286377f) }, // Postal 386
            { 36, new Vector3(-747.61071777344f, -1116.3708496094f, 16.669616699219f) }, // Postal 361
            { 37, new Vector3(-825.95739746094f, -1082.9405517578f, 12.088537216187f) }, // Postal 354 (Shop)
            { 38, new Vector3(-900.02325439453f, -1176.3311767578f, 10.96831035614f) }, // Postal 347
            { 39, new Vector3(-864.16198730469f, -1225.8900146484f, 9.88219165802f) }, // Postal 348
            { 40, new Vector3(-1005.6122436523f, -1554.5158691406f, 9.777379989624f) }, // Postal 330
            { 41, new Vector3(-1017.2619628906f, -1645.8391113281f, 9.0179853439331f) }, // Postal 321
            { 42, new Vector3(-1079.0124511719f, -1580.7523193359f, 8.9304246902466f) }, // Postal 320
            { 43, new Vector3(-1103.2154541016f, -1544.9696044922f, 8.804425239563f) }, // Postal 319
            { 44, new Vector3(-1059.8603515625f, -1485.0744628906f, 9.6451034545898f) }, // Postal 319 (2)
            { 45, new Vector3(-1093.3555908203f, -1444.7609863281f, 9.6451034545898f) }, // Postal 318
            { 46, new Vector3(-1119.4985351563f, -1391.3372802734f, 11.157096862793f) }, // Postal 327
            { 47, new Vector3(-1157.4227294922f, -1360.5944824219f, 11.220096588135f) }, // Postal 317
            { 48, new Vector3(-1155.2005615234f, -1320.9019775391f, 11.157096862793f) }, // Postal 327 (2)
            { 49, new Vector3(-1172.8402099609f, -1158.5177001953f, 8.4870271682739f) }, // Postal 325
            { 50, new Vector3(-1251.4163818359f, -1077.1231689453f, 14.336175918579f) }, // Postal 314
            { 51, new Vector3(-1242.0515136719f, -1052.4995117188f, 14.336175918579f) }, // Postal 324
            { 52, new Vector3(-1278.7895507813f, -1091.8791503906f, 10.302366256714f) }, // Postal 314
            { 53, new Vector3(-1307.1593017578f, -1076.0675048828f, 12.988326072693f) }, // Postal 313
            { 54, new Vector3(-1314.2883300781f, -1105.0048828125f, 12.799326896667f) }, // Postal 303
            { 55, new Vector3(-1320.6640625f, -1174.8315429688f, 8.3758401870728f) }, // Postal 303 (2)
            { 56, new Vector3(-1263.4033203125f, -1254.4438476563f, 9.9971237182617f) }, // Postal 315
            { 57, new Vector3(-1267.634765625f, -1286.9276123047f, 9.9971237182617f) }, // Postal 305
            { 58, new Vector3(-1228.6270751953f, -1352.6630859375f, 10.228817939758f) }, // Postal 316
            { 59, new Vector3(-1227.7091064453f, -1387.5329589844f, 10.228817939758f) }, // Postal 306
            { 60, new Vector3(-1160.6401367188f, -1495.1944580078f, 8.8812694549561f) }, // Postal 307
            { 61, new Vector3(-1437.1744384766f, -1043.0500488281f, 5.292543888092f) }, // Postal 300
            { 62, new Vector3(-1473.5026855469f, -1018.1635742188f, 7.8334546089172f) }, // Postal 300 (2)
            { 63, new Vector3(-1443.7412109375f, -1038.2595214844f, 5.9434485435486f) }, // Postal 300 (3)
            { 64, new Vector3(-1504.5909423828f, -1023.1555175781f, 7.7704544067383f) }, // Postal 300 (4)
            { 65, new Vector3(-1316.2087402344f, -1511.8403320313f, 6.31147813797f) }, // Postal 306
            { 66, new Vector3(-1124.8338623047f, -1972.6204833984f, 15.556568145752f) }, // Postal 90
            { 67, new Vector3(-1139.8790283203f, -1972.6026611328f, 15.556568145752f) }, // Postal 90 (2)
            { 68, new Vector3(-910.47235107422f, -2037.1213378906f, 12.379696846008f) }, // Postal 95
            { 69, new Vector3(-949.06359863281f, -2342.5654296875f, 8.2049055099487f) }, // Postal 97
            { 70, new Vector3(-905.79479980469f, -2338.9809570313f, 9.9058980941772f) }, // Postal 97 (2)
            { 71, new Vector3(-1117.8264160156f, -943.32098388672f, 8.5058364868164f) }, // Postal 334
            { 72, new Vector3(-839.00634765625f, -1015.1544799805f, 19.241165161133f) }, // Postal 359
            { 73, new Vector3(-703.99572753906f, -926.38494873047f, 22.667362213135f) }, // Postal 366
            { 74, new Vector3(-860.44293212891f, -852.73913574219f, 25.434921264648f) }, // Postal 357
            { 75, new Vector3(-762.48840332031f, -824.20330810547f, 28.512619018555f) }, // Postal 371
            { 76, new Vector3(-752.13116455078f, -676.80822753906f, 36.38081741333f) }, // Postal 370
            { 77, new Vector3(-656.775390625f, -646.67242431641f, 37.751441955566f) }, // Postal 369
            { 78, new Vector3(-654.17852783203f, -565.90277099609f, 40.840347290039f) }, // Postal 369 (2)
            { 79, new Vector3(-867.79656982422f, -643.01849365234f, 32.081882476807f) }, // Postal 364
            { 80, new Vector3(84.853858947754f, -1502.5144042969f, 35.306255340576f) }, // Postal 135
            { 81, new Vector3(53.506446838379f, -1516.3405761719f, 33.996646881104f) }, // Postal 127
            { 82, new Vector3(-42.220260620117f, -1593.5614013672f, 35.69845199585f) }, // Postal 117
            { 83, new Vector3(84.303932189941f, -1665.7825927734f, 35.234169006348f) }, // Postal 129
            { 84, new Vector3(-142.95195007324f, -1754.8361816406f, 34.889129638672f) }, // Postal 109
            { 85, new Vector3(-109.15312194824f, -142.32215881348f, 74.593460083008f) }, // Postal 539
            { 86, new Vector3(173.66003417969f, -2056.2431640625f, 24.353200912476f) }, // Postal 132
            { 87, new Vector3(-190.25215148926f, -1694.5208740234f, 36.119186401367f) }, // Postal 109
            { 88, new Vector3(-120.00409698486f, -1506.5040283203f, 39.785659790039f) }, // Postal 104
            { 89, new Vector3(-95.556610107422f, -246.74182128906f, 50.802993774414f) }, // Postal 571
            { 90, new Vector3(124.94513702393f, -2028.4769287109f, 24.353200912476f) }, // Postal 114
            { 91, new Vector3(-248.0477142334f, -323.9069519043f, 46.7350730896f) }, // Postal 539 (2)
            { 92, new Vector3(-259.72570800781f, -199.38946533203f, 43.939720153809f) }, // Postal 539 (3)
            { 93, new Vector3(339.69094848633f, -1940.6536865234f, 30.469142913818f) }, // Postal 138
            { 94, new Vector3(-174.13528442383f, -2089.1657714844f, 30.612253189087f) }, // Postal 70
            { 95, new Vector3(-1105.5589599609f, -764.11730957031f, 25.087532043457f) }, // Postal 342
            { 96, new Vector3(549.58551025391f, -1673.5006103516f, 35.014129638672f) }, // Postal 158
            { 97, new Vector3(-132.26615905762f, -2098.4560546875f, 31.43124961853f) }, // Postal 71
            { 98, new Vector3(482.07913208008f, -1653.9145507813f, 33.997940063477f) }, // Postal 158
            { 99, new Vector3(455.41372680664f, -1607.4027099609f, 35.343524932861f) }, // Postal 151
            { 100, new Vector3(-973.52099609375f, -1218.2814941406f, 10.904612541199f) }, // Postal 337
            { 101, new Vector3(402.56390380859f, -1490.8988037109f, 35.469524383545f) }, // Postal 150
            { 102, new Vector3(125.10829162598f, -2028.3571777344f, 24.06028175354f) }, // Postal 114
            { 103, new Vector3(193.63471984863f, -1586.5130615234f, 33.991870880127f) }, // Postal 136
            { 104, new Vector3(-1160.6369628906f, -1494.4543457031f, 9.3812656402588f) }, // Postal 307
            { 105, new Vector3(185.3758392334f, -1784.5037841797f, 36.180896759033f) }, // Postal 130
            { 106, new Vector3(106.24605560303f, -1927.0706787109f, 24.214658737183f) }, // Postal 123
            { 107, new Vector3(188.60153198242f, -2033.7189941406f, 22.863286972046f) }, // Postal 132
            { 108, new Vector3(230.05139160156f, -2087.2453613281f, 24.375280380249f) }, // Postal 132
            { 109, new Vector3(-406.71096801758f, -1840.7961425781f, 25.368967056274f) }, // Postal 88
            { 110, new Vector3(-1005.4154663086f, -1555.1580810547f, 10.359492301941f) }, // Postal 330
            { 111, new Vector3(351.16232299805f, -2189.1940917969f, 18.894304275513f) }, // Postal 73
            { 112, new Vector3(-403.00582885742f, -1780.3884277344f, 25.874433517456f) }, // Postal 388
            { 113, new Vector3(-300.9216003418f, -1415.8266601563f, 37.309013366699f) }, // Postal 384
            { 114, new Vector3(293.97180175781f, -1425.7005615234f, 36.142360687256f) }, // Postal 149
            { 115, new Vector3(366.93374633789f, -2153.3522949219f, 17.193311691284f) }, // Postal 147
            { 116, new Vector3(907.54852294922f, -2452.2170410156f, 34.491039276123f) }, // Postal 62
            { 117, new Vector3(869.48217773438f, -2366.4733886719f, 33.123043060303f) }, // Postal 61
            { 118, new Vector3(761.60540771484f, -2470.4978027344f, 26.258913040161f) }, // Postal 62
            { 119, new Vector3(721.35797119141f, -2082.9245605469f, 32.481021881104f) }, // Postal 77
            { 120, new Vector3(796.40545654297f, -2045.4952392578f, 35.290760040283f) }, // Postal 172
            { 121, new Vector3(758.50830078125f, -1964.4985351563f, 33.685806274414f) }, // Postal 166
            { 122, new Vector3(825.28942871094f, -1988.9008789063f, 34.816379547119f) }, // Postal 172
            { 123, new Vector3(852.50769042969f, -1860.4659423828f, 33.20783996582f) }, // Postal 171 
            { 124, new Vector3(802.18249511719f, -1732.8316650391f, 35.268314361572f) }, // Postal 164
            { 125, new Vector3(921.88726806641f, -1742.783203125f, 36.399375915527f) }, // Postal 170
            { 126, new Vector3(963.21588134766f, -1777.1196289063f, 37.402252197266f) }, // Postal 170
            { 127, new Vector3(975.44091796875f, -1667.1068115234f, 39.408771514893f) }, // Postal 170
            { 128, new Vector3(954.99340820313f, -1894.2735595703f, 37.321243286133f) }, // Postal 171 
            { 129, new Vector3(1107.8515625f, -1876.1665039063f, 43.138675689697f) }, // Postal 173
            { 130, new Vector3(1084.7601318359f, -1758.5754394531f, 41.799640655518f) }, // Postal 173
            { 131, new Vector3(1140.7681884766f, -1705.7613525391f, 41.799640655518f) }, // Postal 181
            { 132, new Vector3(1194.8665771484f, -1839.3452148438f, 43.146190643311f) }, // Postal 181
            { 133, new Vector3(1213.595703125f, -2051.6740722656f, 50.438190460205f) }, // Postal 174
            { 134, new Vector3(1079.0465087891f, -2086.0036621094f, 40.675464630127f) }, // Postal 63
            { 135, new Vector3(1114.814453125f, -2568.2583007813f, 36.16748046875f) }, // Postal 66
            { 136, new Vector3(1380.7933349609f, -1707.3634033203f, 66.277626037598f) }, // Postal 185
            { 137, new Vector3(1358.9084472656f, -1597.4049072266f, 58.778289794922f) }, // Postal 187
            { 138, new Vector3(1335.3162841797f, -1637.4506835938f, 54.71199798584f) }, // Postal 184
            { 139, new Vector3(1130.3131103516f, -1599.3760986328f, 36.254383087158f) }, // Postal 179
            { 140, new Vector3(1155.2413330078f, -1575.0484619141f, 36.3173828125f) }, // Postal 179
            { 141, new Vector3(1272.7718505859f, -1442.6810302734f, 41.332225799561f) }, // Postal 186
            { 142, new Vector3(1206.4307861328f, -1389.5942382813f, 37.6881980896f) }, // Postal 177
            { 143, new Vector3(1064.5101318359f, -1419.1030273438f, 33.713619232178f) }, // Postal 176
            { 144, new Vector3(976.04364013672f, -1454.9299316406f, 33.505340576172f) }, // Postal 176
            { 145, new Vector3(810.86206054688f, -1448.0241699219f, 33.379341125488f) }, // Postal 163
            { 146, new Vector3(768.42291259766f, -1314.4643554688f, 32.409549713135f) }, // Postal 162
            { 147, new Vector3(810.63116455078f, -1223.3103027344f, 32.346549987793f) }, // Postal 162
            { 148, new Vector3(808.26635742188f, -1134.5363769531f, 35.007816314697f) }, // Postal 226
            { 149, new Vector3(903.46087646484f, -1262.3929443359f, 33.173709869385f) }, // Postal 175
            { 150, new Vector3(735.62243652344f, -1359.7425537109f, 30.086563110352f) }, // Postal 163
            { 151, new Vector3(815.14727783203f, -1040.1311035156f, 31.294929504395f) }, // Postal 230
            { 152, new Vector3(764.55316162109f, -998.02001953125f, 32.177753448486f) }, // Postal 224
            { 153, new Vector3(405.32983398438f, -1032.0456542969f, 35.468593597412f) }, // Postal 217
            { 154, new Vector3(409.9748840332f, -940.85461425781f, 35.468593597412f) }, // Postal 216
            { 155, new Vector3(507.45431518555f, -943.30993652344f, 32.94645690918f) }, // Postal 216
            { 156, new Vector3(508.24719238281f, -813.31701660156f, 30.832218170166f) }, // Postal 215
            { 157, new Vector3(413.25378417969f, -833.44195556641f, 35.55001449585f) }, // Postal 208
            { 158, new Vector3(434.38833618164f, -659.81726074219f, 31.638721466064f) }, // Postal 207
            { 159, new Vector3(333.31643676758f, -652.5322265625f, 35.333892822266f) }, // Postal 202
            { 160, new Vector3(263.92196655273f, -618.84240722656f, 48.199516296387f) }, // Postal 202
            { 161, new Vector3(272.5775f, -1207.4119f, 40.7960f) }, // Tube Station 213
            { 162, new Vector3(-252.2855f, -290.9641f, 23.7684f) }, // Tube Station 539
            { 163, new Vector3(-272.4550f, -337.7403f, 20.6191f) }, // Tube Station 510
            { 164, new Vector3(-305.4276f, -365.5545f, 11.7101f) }, // Tube Station 510
            { 165, new Vector3(-284.0183f, -287.3419f, 11.5440f) }, // Tube Station 520
            { 166, new Vector3(-780.0467f, -129.7627f, 21.6443f) }, // Tube Station 681
            { 167, new Vector3(-857.5909f, -149.7600f, 21.7692f) }, // Tube Station 681
            { 168, new Vector3(-857.5909f, -149.7600f, 21.5709f) }, // Tube Station 681
            { 169, new Vector3(-846.3876f, -139.5754f, 29.6163f) }, // Tube Station 681
            { 170, new Vector3(-846.3876f, -139.5754f, 29.7333f) }, // Tube Station 646
            { 171, new Vector3(-1339.2745f, -507.1369f, 16.9042f) }, // Tube Station 636
            { 172, new Vector3(-1374.5366f, -469.4577f, 25.4123f) }, // Tube Station 636
            { 173, new Vector3(-1338.6654f, -513.2401f, 24.8443f) }, // Tube Station 363
            { 174, new Vector3(-537.0624f, -662.2870f, 12.8134f) }, // Tube Station 376
            { 175, new Vector3(-458.0493f, -683.7097f, 13.0283f) }, // Tube Station 378
            { 176, new Vector3(-508.3799f, -695.2774f, 22.1042f) }, // Tube Station 378
            { 177, new Vector3(-464.9427f, -696.4629f, 21.5439f) }, // Tube Station 378
            { 178, new Vector3(-225.8511f, -1046.0271f, 32.4483f) }, // Tube Station 393
            { 179, new Vector3(125.5810f, -1754.1429f, 33.4737f) }, // Tube Station 129
            { 180, new Vector3(-1009.2781f, -2752.0369f, 2.1776f) }, // Tube Station 98
            { 181, new Vector3(-1072.5192f, -2737.2969f, 3.1368f) }, // Tube Station 98
            { 182, new Vector3(-1049.1578f, -2691.3635f, -5.4972f) }, // Tube Station 98
            { 183, new Vector3(-1115.9618f, -2738.1328f, -5.8837f) }, // Tube Station 98
            { 184, new Vector3(-907.3354f, -2354.0591f, -9.8858f) }, // Tube Station 97
            { 185, new Vector3(-860.3007f, -2287.1257f, -10.2956f) }, // Tube Station 97
            { 186, new Vector3(-901.0186f, -2302.7383f, -1.1471f) }, // Tube Station 97
            { 187, new Vector3(-911.4515f, -2358.4114f, -2.3040f) }, // Tube Station 97
            { 188, new Vector3(-527.4031f, -1271.0424f, 30.4645f) }, // Tube Station 375
            
        };
        #endregion

        public CCTV(ICommandManager commandManager, ILogger<CCTV> logger,
			INewNotificationOverlay newNotificationOverlay, IPermissionService permissionService, ITickManager ticks,
            IPlayerService playerService, ILegacyClientCommunicationsManager comms, ICommonFunctionsService common, IFeatureService featureService)
        {
            _commandManager = commandManager;
			_newNotificationOverlay = newNotificationOverlay;
			_permissionService = permissionService;
            _ticks = ticks;
            _commandManager = commandManager;
            _logger = logger;
            _permissionService = permissionService;
        }

        protected override async Task OnStartAsync()
        {
            _ticks.Off(CamKeybinds);
            _ticks.Off(LoadCamerasKey);
            _ticks.On(CCTVBlips);
            _ticks.On(CancelCamsOnChangeRole);
            
            _commandManager.Register("fecctvc").WithHandler(ChangedRole);

            _commandManager.Register("cctv").HasGreedyArgs().WithHandler(async (cameraID) =>
            {
                var currentUserRole = _permissionService.CurrentUserRole;
                if (currentUserRole.Branch != UserBranch.Control && currentUserRole.Branch != UserBranch.Highways && currentUserRole.Division != UserDivision.Cid)
                {
                    if (!cctvCamsActive)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("CCTV Cameras", "error", "To use the CCTV Camera System, you must be at a CCTV viewing station!", new NewNotificationMessageContent[0]));
                        return;
                    }
                }

                var player = Game.PlayerPed.Handle;

                if (!Int32.TryParse(cameraID, out int intCameraID))
                {
                    return;
                }

                if (cameraPositions.TryGetValue(intCameraID, out Vector3 cameraPos))
                {
                    API.DoScreenFadeOut(800);
                    API.DisableControlAction(0, 0, true);
                    API.SetFollowPedCamViewMode(4);
                    await Delay(800);
                    API.SetEntityCoords(player, cameraPos.X, cameraPos.Y, cameraPos.Z, false, false, false, false);
                    currentCameraIndex = intCameraID;
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("CCTV Cameras", "success", $"Camera {intCameraID} has been loaded!", new NewNotificationMessageContent[0]));
					API.SetEntityVisible(player, false, false);
                    API.FreezeEntityPosition(player, true);
                    API.DisableAllControlActions(0);
                    API.SetFollowPedCamViewMode(4);

                    if (!API.HasCollisionLoadedAroundEntity(player))
                    {
                        await Delay(10);
                    }

                    API.DoScreenFadeIn(800);
                    return;
                }

				_newNotificationOverlay.SendNotification(new NewNotificationMessage("CCTV Cameras", "success", $"Camera {intCameraID} could not be viewed!", new NewNotificationMessageContent[0]));
			});

            //_commandManager.Register("switchcam").WithHandler(SwitchCam);
            //_commandManager.Register("exitcams").WithHandler(ExitCams);
        }

        private async void CreateCameraBlips()
        {
            if (cameraBlipHandles.Count == 160) return;

            foreach (var cameraPosition in cameraPositions)
            {
                int blipHandle = API.AddBlipForCoord(cameraPosition.Value.X, cameraPosition.Value.Y, cameraPosition.Value.Z);
                API.SetBlipSprite(blipHandle, 1);
                API.SetBlipScale(blipHandle, 2f);
                API.SetBlipDisplay(blipHandle, 4);
                API.SetBlipColour(blipHandle, 1);
                API.SetBlipAsShortRange(blipHandle, true);

                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentString("CCTV Camera");
                API.EndTextCommandSetBlipName(blipHandle);

                cameraBlipHandles[cameraPosition.Key] = blipHandle;
            }
        }


        private void RemoveCameraBlips()
        {
            foreach (var cameraPosition in cameraPositions)
            {
                int blipHandle = cameraBlipHandles[cameraPosition.Key];
                if (blipHandle != 0)
                {
                    API.RemoveBlip(ref blipHandle);
                    API.DeleteEntity(ref blipHandle);
                }
            }
            cameraBlipHandles.Clear();
        }


        private async Task CCTVBlips()
        {
            if (_permissionService.CurrentUserRole == null) return;
            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole.Branch != UserBranch.Control && currentUserRole.Branch != UserBranch.Highways && currentUserRole.Division != UserDivision.Cid)
            {
                return;
            }

            await Delay(35);

            var player = Game.PlayerPed.Handle;
            var playerCoords = API.GetEntityCoords(player, true);
            var distanceFromControlCCTV = World.GetDistance(playerCoords, controlCCTVRoom);
            var distanceFromHighwaysCCTV = World.GetDistance(playerCoords, highwaysCCTVRoom);
            var distanceFromCidCCTV = World.GetDistance(playerCoords, cidCCTVRoom);

            if (distanceFromControlCCTV < 2.5f)
            {
                _ticks.On(LoadCamerasKey);
                return;
            }

            if (distanceFromHighwaysCCTV < 2.5f)
            {
                _ticks.On(LoadCamerasKey);
                return;
            }

            if (distanceFromCidCCTV < 2.5f)
            {
                _ticks.On(LoadCamerasKey);
                return;
            }

            _ticks.Off(LoadCamerasKey);
        }

        private async Task LoadCamerasKey()
        {
            await Delay(0);
            Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ to use CCTV!");
            if (API.IsControlJustReleased(0, 46))
            {
                CCTVCommand();
                await Delay(500);
                return;
            }
        }


        private async Task CamKeybinds()
        {
            await Delay(0);

            API.SetBlipColour(API.GetMainPlayerBlipId(), 40);

            if (API.IsControlJustPressed(0, 46))
            {
                SwitchCam();
            }
            if (API.IsControlJustPressed(0, 44))
            {
                ExitCams();
                CancelCamsOnChangeRole();
            }
            if (API.IsControlJustPressed(0, 45))
            {
                SwitchCamBack();
            }

            API.DisableControlAction(0, 0, true);
            API.DisableControlAction(0, (int)Control.Phone, true);
            API.DisableControlAction(0, (int)Control.SelectWeapon, true);
            API.DisableControlAction(0, (int)Control.LookUp, true);
            API.DisableControlAction(0, (int)Control.LookDown, true); ;
            API.DisableControlAction(0, (int)Control.MoveUp, true);
            API.DisableControlAction(0, (int)Control.MoveDown, true);
            API.DisableControlAction(0, (int)Control.MoveLeft, true);
            API.DisableControlAction(0, (int)Control.MoveRight, true); ;
            API.DisableControlAction(0, (int)Control.Cover, true);
            API.DisableControlAction(0, (int)Control.WeaponWheelNext, true);
            API.DisableControlAction(0, (int)Control.WeaponWheelPrev, true);
            API.DisableControlAction(0, (int)Control.WeaponSpecial, true);

            Screen.ShowSubtitle("~y~CYCLE: ➡~r~'E' - 'R'⬅️ ~y~~n~EXIT: ~r~'Q'~y~~n~SELECT CAMERA: ~r~'/cctv camID'");
        }

        private async void CCTVCommand()
        {
            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole.Branch == UserBranch.Highways)
            {
                cameraPositions = highwaysCameraPositions;
            }
            else if (currentUserRole.Branch == UserBranch.Control)
            {
                cameraPositions = controlCameraPositions;
            }
            else if (currentUserRole.Division == UserDivision.Cid)
            {
                cameraPositions = controlCameraPositions;
            }
            else
            {
                API.AddTextEntry("FMMC_KEY_TIP1", "Enter Selection ('Control' or 'Highways')");
                API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "Control", "", "", "", 10);

                API.UpdateOnscreenKeyboard();

                while (API.UpdateOnscreenKeyboard() == 0)
                {
                    await Delay(10);
                    API.UpdateOnscreenKeyboard();
                }

                if (API.UpdateOnscreenKeyboard() == 1)
                {
                    string cctvSelection = API.GetOnscreenKeyboardResult();
                    if (cctvSelection == "Control") cameraPositions = controlCameraPositions;
                    else if (cctvSelection == "Highways") cameraPositions = highwaysCameraPositions;
                    else
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                            "CCTV Selection", "error",
                            $"To select a camera dictionary, enter either 'Control' or 'Highways' into the box!",
                            new NewNotificationMessageContent[0]
                        ));
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            var player = Game.PlayerPed.Handle;
            cctvCamsActive = true;

            if (API.IsPedInAnyVehicle(player, true) || API.IsPedInAnyBoat(player)) return;

            currentCameraIndex = 1;
            startingCoords = API.GetEntityCoords(player, true);
            startingMumbleProx = API.MumbleGetTalkerProximity();
            API.MumbleSetTalkerProximity(0f);
            API.DoScreenFadeOut(800);
            await Delay(800);
            API.SetFollowPedCamViewMode(4);
            API.DisableControlAction(0, 0, true);
            API.DisplayHud(false);
            API.DisplayRadar(false);

            _ticks.On(CamKeybinds); // ✅ Enables E/R/Q control handling
            CreateCameraBlips();

            if (cameraPositions.TryGetValue(currentCameraIndex, out Vector3 cameraPos))
            {
                API.SetEntityCoords(player, cameraPos.X, cameraPos.Y, cameraPos.Z, false, false, false, false);
            }

            API.SetEntityVisible(player, false, false);
            API.FreezeEntityPosition(player, true);

            while (!API.HasCollisionLoadedAroundEntity(player))
            {
                await Delay(10);
            }

            API.DoScreenFadeIn(800);
        }

        private async void SwitchCamBack()
        {
            var player = Game.PlayerPed.Handle;
            if (cameraPositions.TryGetValue(currentCameraIndex - 1, out Vector3 cameraPos))
            {
                API.DoScreenFadeOut(800);
                API.DisableControlAction(0, 0, true);
                API.SetFollowPedCamViewMode(4);
                await Delay(800);

                API.SetEntityCoords(player, cameraPos.X, cameraPos.Y, cameraPos.Z, false, false, false, false);
                API.FreezeEntityPosition(player, true);
                currentCameraIndex = (currentCameraIndex - 1);

                API.FreezeEntityPosition(player, true);
                API.DisableAllControlActions(0);
                API.SetFollowPedCamViewMode(4);

                if (!API.HasCollisionLoadedAroundEntity(player))
                {
                    await Delay(10);
                }

                API.DoScreenFadeIn(800);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("CCTV Cameras", "success", $"Now viewing Camera: {currentCameraIndex}", new NewNotificationMessageContent[0]));

            }
        }

        private async void SwitchCam()
        {
            var player = Game.PlayerPed.Handle;
            if (cameraPositions.TryGetValue(currentCameraIndex + 1, out Vector3 cameraPos))
            {
                API.DoScreenFadeOut(800);
                API.DisableControlAction(0, 0, true);
                API.SetFollowPedCamViewMode(4);
                await Delay(800);

                API.SetEntityCoords(player, cameraPos.X, cameraPos.Y, cameraPos.Z, false, false, false, false);
                API.FreezeEntityPosition(player, true);
                currentCameraIndex = (currentCameraIndex + 1);

                API.SetEntityVisible(player, false, false);
                API.DisableAllControlActions(0);
                API.SetFollowPedCamViewMode(4);

                if (!API.HasCollisionLoadedAroundEntity(player))
                {
                    await Delay(10);
                }

                API.DoScreenFadeIn(800);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("CCTV Cameras", "success", $"Now viewing Camera: {currentCameraIndex}", new NewNotificationMessageContent[0]));
            }
        }

        private async Task CancelCamsOnChangeRole()
        {
            if (!cctvCamsActive) return;
            var player = Game.PlayerPed.Handle;

            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole.Branch != UserBranch.Control || API.IsPedDeadOrDying(player, true))
            {
                cctvCamsActive = false;

                foreach (var cameraPosition in cameraPositions)
                {
                    int blipHandle = cameraBlipHandles[cameraPosition.Key];
                    if (blipHandle != 0)
                    {
                        API.RemoveBlip(ref blipHandle);
                        API.DeleteEntity(ref blipHandle);
                    }
                }
                cameraBlipHandles.Clear();

                API.FreezeEntityPosition(player, false);
                _ticks.Off(CamKeybinds);
                _ticks.Off(LoadCamerasKey);
                API.SetBlipColour(API.GetMainPlayerBlipId(), 0);
                API.EnableAllControlActions(0);
                API.DisplayHud(true);
                API.DisplayRadar(true);
                API.FreezeEntityPosition(player, false);
                await Delay(2000);
                API.SetEntityVisible(player, true, false);
            }
        }

        private async void ChangedRole()
        {
            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole.Branch != UserBranch.Control && currentUserRole.Branch != UserBranch.Highways && currentUserRole.Division != UserDivision.Cid) return;
            var returnCoords = new Vector3(0.025599991902709f, -0.0043085883371532f ,-0.78580504655838f);
            var distance = Vector3.Distance(startingCoords, returnCoords);
            if (distance < 20f) return;
            if (startingCoords == returnCoords) return;
            _ticks.Off(LoadCamerasKey);
            cctvCamsActive = false;
            ExitCams();
        }

        private async void ExitCams()
        {
            _ticks.Off(LoadCamerasKey);
            RemoveCameraBlips();
            var player = Game.PlayerPed.Handle;
            cctvCamsActive = false;
            API.DisplayHud(true);
            API.DisplayRadar(true);
            Screen.ShowSubtitle(" ", 100);

            _ticks.Off(CamKeybinds);

            API.DoScreenFadeOut(800);
            await Delay(800);

            API.EnableAllControlActions(0);
            API.SetFollowPedCamViewMode(1);
            API.DisableControlAction(0, 0, false);

            if (cameraPositions.TryGetValue(currentCameraIndex, out Vector3 cameraPos))
            {
                API.SetEntityCoords(player, cameraPos.X, cameraPos.Y, cameraPos.Z, false, false, false, false);
            }

            while (!API.HasCollisionLoadedAroundEntity(player))
            {
                await Delay(10);
            }

            await Delay(800);
            API.DoScreenFadeIn(800);

            API.SetEntityCoords(player, startingCoords.X, startingCoords.Y, startingCoords.Z, false, false, false, false);
            API.MumbleSetTalkerProximity(startingMumbleProx);
            API.FreezeEntityPosition(player, false);
            API.SetEntityVisible(player, true, false);
            API.DisableControlAction(0, (int)Control.LookUp, false);
            API.DisableControlAction(0, (int)Control.LookDown, false);
            API.SetBlipColour(API.GetMainPlayerBlipId(), 0);
        }
    }
}
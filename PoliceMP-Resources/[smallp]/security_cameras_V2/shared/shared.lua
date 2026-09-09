CCTV_Config = {}

CCTV_Config.HideRadar = true -- Controls if the minimap will be disabled when in a camera.
CCTV_Config.HideHUD = true -- Controls if the HUD will be disabled when in a camera. (Reference Utils.ToggleHUD() in Utils.lua)

CCTV_Config.UseAces = false -- Controls if you're using default permissions or not

CCTV_Config.ShowNoPerms = true -- Controls whether or not to display the no permissions error

CCTV_Config.Cameras = { -- All of the logged cameras
    {location = vector4(34.12, -1348.48, 30.5, 59.20), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Innocence Boulevard)", camera = 1},
    {location = vector4(24.22, -1342.64, 30.5, 235.62), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Innocence Boulevard)", camera = 2},
    {location = vector4(30.99, -1341.13, 30.52, 49.42), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Innocence Boulevard)", camera = 3},
    {location = vector4(381.34, 323.01, 104.69, 34.69), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Clinton Avenue)", camera = 1},
    {location = vector4(373.53, 331.02, 104.69, 209.73), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Clinton Avenue)", camera = 2},
    {location = vector4(380.57, 330.62, 104.69, 32.19), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Clinton Avenue)", camera = 3},
    {location = vector4(2558.81, 390.11, 109.85, 143.36), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Palomino Freeway)", camera = 1},
    {location = vector4(2552.54, 380.92, 109.85, 313.33), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Palomino Freeway)", camera = 2},
    {location = vector4(2551.39, 387.73, 109.85, 135.86), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Palomino Freeway)", camera = 3},
    {location = vector4(-3036.09, 585.05, 9.6, 319.68), canRotate = true, cameraType = "low", type = "General Stores", name = "General Store (Inseno Road)", camera = 1},
    {location = vector4(-3040.77, 594.27, 9.08, 158.74), canRotate = true, cameraType = "medium", type = "General Stores", name = "General Store (Inseno Road)", camera = 2},
    {location = vector4(-3043.44, 582.91, 9.08, 336.27), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Inseno Road)", camera = 3},
    {location = vector4(-3046.84, 588.96, 9.08, 158.71), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Inseno Road)", camera = 4},
    {location = vector4(-3240.23, 1009.21, 13.90, 129.43), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Barbereno Road)", camera = 1},
    {location = vector4(-3246.92, 1000.3, 13.90, 311.88), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Barbereno Road)", camera = 2},
    {location = vector4(-3247.78, 1007.18, 14.01, 136.92), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Barbereno Road)", camera = 3},
    {location = vector4(548.98, 2674.68, 43.16, 35.94), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Harmony)", camera = 1},
    {location = vector4(537.4, 2671.29, 43.79, 128.4), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Harmony)", camera = 2},
    {location = vector4(539.14, 2671.23, 43.29, 233.44), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Harmony)", camera = 3},
    {location = vector4(549.85, 2666.71, 43.29, 50.89), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Harmony)", camera = 4},
    {location = vector4(543.36, 2664.36, 43.29, 233.4), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Harmony)", camera = 5},
    {location = vector4(1968.82, 3743.63, 33.49, 77.66), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Alhambra Drive)", camera = 1},
    {location = vector4(1657.69, 3744.0, 33.5, 257.56), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Alhambra Drive)", camera = 2},
    {location = vector4(1962.79, 3748.66, 33.49, 77.66), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Alhambra Drive)", camera = 3},
    {location = vector4(2683.92, 3287.4, 56.32, 107.43), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Senora Freeway)", camera = 1},
    {location = vector4(2673.82, 3281.56, 56.32, 292.39), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Senora Freeway)", camera = 2},
    {location = vector4(2675.99, 3288.17, 56.32, 109.95), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Senora Freeway)", camera = 3},
    {location = vector4(1740.25, 6406.94, 36.15, 201.8), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (East | Great Ocean Highway)", camera = 1},
    {location = vector4(1735.7, 6410.0, 36.15, 21.8), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (East | Great Ocean Highway)", camera = 2},
    {location = vector4(1729.81, 6419.38, 36.08, 199.44), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (East | Great Ocean Highway)", camera = 3},
    {location = vector4(1736.51, 6417.86, 36.08, 21.8), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (East | Great Ocean Highway)", camera = 4},
    {location = vector4(1133.25, -978.84, 47.48, 221.79), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Vespucci Boulevard)", camera = 1},
    {location = vector4(-1224.77, -910.71, 13.43, 335.67), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (San Andreas Avenue)", camera = 1},
    {location = vector4(-1483.36, -380.25, 41.31, 74.71), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Prosperity Street)", camera = 1},
    {location = vector4(-2966.23, 387.35, 16.27, 26.65), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (West | Great Ocean Highway)", camera = 1},
    {location = vector4(1169.49, 2711.16, 39.32, 118.61), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Route 68)", camera = 1},
    {location = vector4(-56.89, -1752.07, 30.54, 275.94), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Davis Avenue)", camera = 1},
    {location = vector4(-43.42, -1755.28, 30.54, 98.44), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Davis Avenue)", camera = 2},
    {location = vector4(-40.0, -1751.34, 30.09, 120.42), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Davis Avenue)", camera = 3},
    {location = vector4(-43.97, -1748.2, 30.12, 188.44), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Davis Avenue)", camera = 4},
    {location = vector4(1153.6, -326.81, 70.33, 325.41), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (West Mirror Drive)", camera = 1},
    {location = vector4(1164.76, -318.51, 70.25, 147.90), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (West Mirror Drive)", camera = 2},
    {location = vector4(1164.05, -313.02, 69.96, 170.47), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (West Mirror Drive)", camera = 3},
    {location = vector4(1159.16, -314.42, 69.81, 237.90), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (West Mirror Drive)", camera = 4},
    {location = vector4(-717.72, -915.68, 20.31, 318.77), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Ginger Street)", camera = 1},
    {location = vector4(-705.42, -909.55, 20.24, 136.27), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Ginger Street)", camera = 2},
    {location = vector4(-705.05, -903.92, 19.98, 161.27), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Ginger Street)", camera = 3},
    {location = vector4(-710.21, -904.43, 19.82, 228.77), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Ginger Street)", camera = 4},
    {location = vector4(-1827.22, 784.84, 139.4, 2.34), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (North Rockford Drive)", camera = 1},
    {location = vector4(-1822.38, 797.85, 139.17, 172.34), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (North Rockford Drive)", camera = 2},
    {location = vector4(-1825.91, 802.22, 138.91, 204.84), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (North Rockford Drive)", camera = 3},
    {location = vector4(-1829.37, 898.26, 138.8, 267.34), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (North Rockford Drive)", camera = 4},
    {location = vector4(1703.03, 4933.52, 43.18, 188.32), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Grapeseed)", camera = 1},
    {location = vector4(1701.05, 4919.84, 43.18, 10.82), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Grapeseed)", camera = 2},
    {location = vector4(1705.21, 4916.74, 42.73, 35.83), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Grapeseed)", camera = 3},
    {location = vector4(1708.04, 4920.94, 42.67, 103.32), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Grapeseed)", camera = 4},
    {location = vector4(-1090.0, -2588.47, 14.98, 104.98), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (LSIA)", camera = 1},
    {location = vector4(-1099.73, -2593.77, 15.08, 289.90), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (LSIA)", camera = 2},
    {location = vector4(-1097.69, -2587.18, 15.08, 107.48), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (LSIA)", camera = 3},
    {location = vector4(264.58, -987.13, 30.49, 296.89), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Strawberry Avenue)", camera = 1},
    {location = vector4(273.45, -979.77, 30.48, 119.43), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Strawberry Avenue)", camera = 2},
    {location = vector4(272.63, -986.63, 30.48, 297.0), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Strawberry Avenue)", camera = 3},
    {location = vector4(166.27, 6634.11, 32.86, 0.07), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Paleto Bay)", camera = 1},
    {location = vector4(163.78, 6644.98, 32.86, 185.2), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Paleto Bay)", camera = 2},
    {location = vector4(169.56, 6641.29, 32.81, 7.58), canRotate = true, cameraType = "highquality", type = "General Stores", name = "General Store (Paleto Bay)", camera = 3},

    -- Highway Traffic Cameras
    
    {location = vector4(-1141.84, -656.03, 15.46, 291.61), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Del Perro Fwy", camera = 1},
   
    {location = vector4(-988.14, -571.27, 25.02, 358.79), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Little Seoul Del Perro Fwy", camera = 1},


    {location = vector4(-523.43, -514.01, 29.7, 267.46), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Little Seoul Del Perro Fwy/La Puerta Fwy", camera = 1},
  

    {location = vector4(-227.05, -513.61, 30.35, 238.64), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Las Lagunas Blv La Puerta Fwy", camera = 1},


    {location = vector4(270.64, -510.93, 38.39, 236.37), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Vinewood Del Perro Fwy/Elgin Ave", camera = 1},
  

    {location = vector4(476.81, -484.67, 40.15, 53.99), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "West Vinewood Los Santos Fwy/Del Perro Fwy", camera = 1},
  

    {location = vector4(579.07, -543.8, 41.66, 223.35), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East La Mesa Los Perro Fwy/Del Perro Fwy", camera = 1},
  

    {location = vector4(1020, -932.81, 34.37, 168.39), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "South Murrieta Heights Del Perro Fwy", camera = 1},
   

    {location = vector4(1064.98, -1448.59, 32.68, 315.01), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East La Mesa Eltsian Fields Fyw/Capital Blvd", camera = 1},
    

    {location = vector4(1229.26, -2020.53, 48.63, 186.99), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "South El Burro Heights Elysian Fields Fwy", camera = 1},
    
    {location = vector4(963.99, -2596.72, 50.74, 90.22), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "West Cypress Flats Elysian Fields Fwy", camera = 1},
   

    {location = vector4(236.31, -2678.4, 22.69, 65.79), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "West Elysian Fields Fwy", camera = 1},
  

    {location = vector4(-536.69, -2239.02, 64.04, 48.86), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "West La Puerta Fwy", camera = 1},

    {location = vector4(-725.75, -2149.05, 17.89, 119.66), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "West Davis Ave/Dutch London", camera = 1},

    {location = vector4(-699.21, -2005.97, 30.76, 352.9), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Davis Ave", camera = 1},

    {location = vector4(-668.8, -1918.78, 31.38, 43.62), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Dutch London St", camera = 1},

    {location = vector4(-324.41, -2102.61, 27.79, 244.81), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Dutch London St/Davs Ave", camera = 1},

    {location = vector4(-492.12, -1886.52, 22.16, 276.21), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East La Puerta/Davis Ave", camera = 1},

    {location = vector4(-753.85, -1796.07, 33.05, 359.85), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Dutch London St/Davis Ave", camera = 1},

    {location = vector4(-650.28, -1724.46, 41.64, 271.86), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East La Puerta Fwy/Mutiny Rd", camera = 1},

    {location = vector4(-429.01, -1565.29, 43.34, 333.89), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North La Puerta/La Puerta Fwy", camera = 1},

    {location = vector4(-403.82, -1373.75, 41.81, 248.17), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North La Puerta Fwy/Olympic Fwy", camera = 1},

    {location = vector4(-404.26, -979.05, 41.64, 348.38), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North La Puerta Fwy/Olympic Fwy", camera = 2},

    {location = vector4(-404.3, -698.54, 41.77, 340.45), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North La Puerta Fwy/Olympic Fwy", camera = 3},

    {location = vector4(-152.9, -1220.86, 51.95, 294.69), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Olympic Fwy", camera = 1},

    {location = vector4(596.12, -1203.31, 45.46, 258.47), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Olympic Fwy", camera = 2},

    {location = vector4(1385.05, -1088.29, 57.75, 289.68), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Palomino Fwy", camera = 1},

    {location = vector4(2127.29, -578.88, 99.76, 302.17), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Palomino Fwy", camera = 2},

    {location = vector4(2444.51, -162.73, 92.7, 318.37), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Palomino Fwy", camera = 3},

    {location = vector4(2592.35, 681.02, 97.84, 9.88), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Palomino Fwy", camera = 4},

    {location = vector4(2354.46, 1170.33, 67.19, 307.91), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "East Palomino Fwy", camera = 5},

    {location = vector4(2526.67, 642.45, 110.76, 175.82), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "West Palomino Fwy", camera = 6},

    {location = vector4(2133.19, 1341.78, 79.89, 18.76), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "West Palomino Fwy", camera = 7},

    {location = vector4(526.95, -357.51, 48.97, 344.28), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Los Santos Freeway", camera = 1},

    {location = vector4(587.81, -326.89, 41.16, 313.77), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Los Santos Freeway", camera = 2},

    {location = vector4(834.75, 90.13, 73.15, 310.47), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Los Santos Freeway", camera = 3},

    {location = vector4(1287.24, 579.15, 84.91, 296.25), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Los Santos Freeway", camera = 4},

    {location = vector4(1606.99, 1070.94, 85.23, 329.8), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Los Santos Freeway", camera = 5},

    {location = vector4(1732.16, 1518.58, 89.14, 323.9), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Los Santos Freeway", camera = 6},

    {location = vector4(1902.79, 2371.38, 59.99, 335.66), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Los Santos Freeway", camera = 7},

     {location = vector4(2003.39, 1590.59, 79.07, 9.69), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 1},

     {location = vector4(2014.28, 2579.03, 59.3, 297.02), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 2},
 
     {location = vector4(2248.09, 2779.26, 48.44, 293.72), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 3},

     {location = vector4(2373.62, 2946.01, 53.6, 340.89), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 4},

     {location = vector4(2772.44, 3350.43, 60.46, 322.44), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 5},

     {location = vector4(2795.45, 4390.78, 53.87, 59.56), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 6},

     {location = vector4(2789.62, 4420.52, 53.43, 346.86), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 7},

     {location = vector4(2627.92, 5099.9, 49.46, 339.29), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 8},

     {location = vector4(2621.93, 5123.63, 49.34, 342.12), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 9},

     {location = vector4(2219.07, 6022.92, 56.43, 45.06), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 10},

     {location = vector4(2118.21, 6026.24, 59.69, 47.04), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 11},

     
     {location = vector4(1783.16, 6355.11, 41.62, 59.11), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Senora Fwy", camera = 12},

     {location = vector4(1031.51, 6504.69, 25.91, 58.03), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 1},

     {location = vector4(183.46, 6546.85, 36.59, 116.53), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 2},

     {location = vector4(-635.6, 5606.54, 43.49, 135.6), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 3},

     {location = vector4(-1230.06, 5271.97, 55.2, 223.17), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 4},

     {location = vector4(-1538.61, 4980.64, 66.88, 147.93), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 5},
     
     {location = vector4(-1913.39, 4623.48, 63.04, 145.38), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 6},

     {location = vector4(-2300.23, 4214.92, 45.76, 317.91), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 7},

    {location = vector4(-2506.03, 3541.91, 19.46, 342.76), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 8},

    {location = vector4(-2622.92, 2975.36, 22.91, 19.98), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 9},
   
    {location = vector4(-2691.93, 2302.06, 24.53, 232.53), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 10},

    {location = vector4(-3090.39, 1201.89, 26.05, 180.48), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 11},

    {location = vector4(-3027.27, 311.33, 19.5, 340.22), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 12},

    {location = vector4(-2225.42, -358.66, 18.12, 275.76), canRotate = true, cameraType = "highquality", type = "Traffic Cameras", name = "North Great Ocean Hwy", camera = 13},














 


















    {location = vector4(112.17, -1312.33, 35.07, 236.35), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 1},
    {location = vector4(144.05, -1295.15, 32.27, 181.35), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 2},
    {location = vector4(145.08, -1292.26, 31.87, 328.85), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 3},
    {location = vector4(132.28, -1293.21, 29.87, 148.85), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 4},
    {location = vector4(120.34, -1289.46, 30.37, 216.92), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 5},
    {location = vector4(117.89, -1282.66, 30.37, 154.42), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 6},
    {location = vector4(110.67, -1281.92, 30.12, 219.42), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 7},
    {location = vector4(133.57, -1275.7, 35.75, 231.92), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 8},
    {location = vector4(132.44, -1274.22, 33.52, 21.92), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 9},
    {location = vector4(131.58, -1275.42, 34.34, 111.92), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 10},
    {location = vector4(100.56, -1280.87, 34.49, 289.42), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 11},
    {location = vector4(98.58, -1281.03, 33.54, 79.42), canRotate = true, cameraType = "highquality", type = "Entertainment", name = "Vanilla Unicorn", camera = 12},
    
    {location = vector4(149.47, -1035.65, 31.08, 308.42), canRotate = true, cameraType = "highquality", type = "Banks", name = "Legion Square", camera = 1},
    {location = vector4(152.69, -1041.89, 29.79, 55.92), canRotate = true, cameraType = "highquality", type = "Banks", name = "Legion Square", camera = 2},
    {location = vector4(146.48, -1038.21, 29.79, 248.42), canRotate = true, cameraType = "highquality", type = "Banks", name = "Legion Square", camera = 3},
    {location = vector4(143.44, -1039.94, 30.04, 315.89), canRotate = true, cameraType = "highquality", type = "Banks", name = "Legion Square", camera = 4},
    {location = vector4(142.93, -1041.93, 30.11, 220.91), canRotate = true, cameraType = "highquality", type = "Banks", name = "Legion Square", camera = 5},
    {location = vector4(145.15, -1043.27, 29.92, 220.92), canRotate = true, cameraType = "highquality", type = "Banks", name = "Legion Square", camera = 6},
    {location = vector4(150.61, -1046.72, 29.91, 58.42), canRotate = true, cameraType = "highquality", type = "Banks", name = "Legion Square", camera = 7},
    {location = vector4(149.90, -1051.08, 30.08, 18.42), canRotate = true, cameraType = "highquality", type = "Banks", name = "Legion Square", camera = 8},
    
    {location = vector4(301.31, -581.95, 44.31, 193.39), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Pillbox Medical Center", camera = 1},
    {location = vector4(311.83, -597.61, 44.46, 21.78), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Pillbox Medical Center", camera = 2},
    {location = vector4(327.98, -591.3, 44.43, 119.31), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Pillbox Medical Center", camera = 3},
    {location = vector4(323.69, -602.56, 44.33, 44.28), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Pillbox Medical Center", camera = 4},
    {location = vector4(305.69, -569.71, 44.41, 213.83), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Pillbox Medical Center", camera = 5},
    {location = vector4(315.33, -579.04, 44.41, 136.33), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Pillbox Medical Center", camera = 6},
    {location = vector4(334.53, -570.0, 44.46, 141.33), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Pillbox Medical Center", camera = 7},
    {location = vector4(336.82, -586.6, 44.48, 211.33), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Pillbox Medical Center", camera = 8},
    {location = vector4(347.93, -602.31, 44.33, 1.33), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Pillbox Medical Center", camera = 9},
    {location = vector4(1836.64, 3671.79, 35.30, 241.95), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Sandy Shores Medical Center", camera = 1},
    {location = vector4(1818.46, 3680.81, 35.17, 61.15), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Sandy Shores Medical Center", camera = 2},
    {location = vector4(1830.45, 3693.33, 35.38, 62.7), canRotate = true, cameraType = "highquality", type = "Hospitals", name = "Sandy Shores Medical Center", camera = 3},
    
    {location = vector4(433.3, -978.43, 32.65, 126.55), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 1},
    {location = vector4(423.99, -996.86, 32.96, 119.5), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 2},
    {location = vector4(438.44, -999.89, 31.51, 212.86), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 3},
    {location = vector4(438.91, -978.64, 31.64, 217.54), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 4},
    {location = vector4(441.1, -987.55, 31.97, 52.74), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 5},
    {location = vector4(444.4, -975.07, 32.22, 137.54), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 6},
    {location = vector4(449.24, -988.52, 31.79, 52.54), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 7},
    {location = vector4(449.3, -981.81, 31.77, 140.04), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 8},
    {location = vector4(460.48, -986.91, 31.45, 77.53), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 9},
    {location = vector4(450.83, -986.85, 31.35, 292.54), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 10},
    {location = vector4(458.93, -986.23, 27.42, 72.54), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 11},
    {location = vector4(465.14, -985.6, 26.77, 170.04), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 12},
    {location = vector4(463.39, -990.86, 26.7, 340.03), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 13},
    {location = vector4(459.23, -987.62, 26.66, 233.64), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 14},
    {location = vector4(465.33, -995.80, 25.31, 131.13), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 15},
    {location = vector4(469.49, -995.14, 25.76, 238.64), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 16},
    {location = vector4(478.41, -996.16, 24.99, 148.63), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 17},
    {location = vector4(487.01, -995.25, 24.56, 51.14), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 18},
    {location = vector4(462.42, -1007.64, 25.46, 341.14), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 19},
    {location = vector4(469.5, -1009.24, 26.86, 171.14), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 20},
    {location = vector4(489.89, -1003.8, 28.71, 302.0), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 21},
    {location = vector4(481.86, -977.89, 29.01, 28.63), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Mission Row", camera = 22},
    
    {location = vector4(1856.12, 3683.28, 35.32, 175.73), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 1},
    {location = vector4(1865.0, 3688.08, 35.08, 247.7), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 2},
    {location = vector4(1851.76, 3682.04, 35.38, 325.73), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 3},
    {location = vector4(1847.48, 3689.51, 35.33, 258.23), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 4},
    {location = vector4(1853.08, 3692.62, 35.33, 188.23), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 5},
    {location = vector4(1855.6, 3690.09, 35.55, 348.23), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 6},
    {location = vector4(1844.85, 3694.13, 35.40, 250.73), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 7},
    {location = vector4(1849.12, 3683.57, 30.82, 155.73), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 8},
    {location = vector4(1850.36, 3684.23, 30.82, 273.23), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 9},
    {location = vector4(1853.0, 3687.34, 30.82, 63.23), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 10},
    {location = vector4(1854.4, 3687.96, 30.75, 343.23), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 11},
    {location = vector4(1859.26, 3686.51, 30.8, 68.23), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 12},
    {location = vector4(1860.23, 3688.5, 30.82, 298.23), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 13},
    {location = vector4(1858.38, 3691.48, 30.82, 300.73), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 14},
    {location = vector4(1855.94, 3694.82, 30.82, 305.73), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 15},
    {location = vector4(1853.61, 3700.67, 35.38, 350.2), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 16},
    {location = vector4(1853.61, 3700.67, 35.38, 350.2), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Sandy Shores", camera = 17},

    {location = vector4(-434.9, 6011.41, 33.42, 341.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 1},
    {location = vector4(-447.43, 6023.68, 33.42, 348.92), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 2},
    {location = vector4(-458.63, 6015.94, 33.24, 11.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 3},
    {location = vector4(-441.21, 5994.84, 33.24, 106.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 4},
    {location = vector4(-442.96, 5983.43, 33.24, 111.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 5},
    {location = vector4(-430.89, 5989.2, 33.24, 261.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 6},
    {location = vector4(-441.45, 6013.17, 32.77, 76.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 7},
    {location = vector4(-447.4, 6007.07, 32.77, 1.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 8},
    {location = vector4(-451.49, 6011.69, 32.84, 293.12), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 9},
    {location = vector4(-439.7, 6013.18, 32.84, 166.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 10},
    {location = vector4(-433.59, 6001.16, 32.84, 358.92), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 11},
    {location = vector4(-440.92, 5999.96, 32.64, 271.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 12},
    {location = vector4(-447.12, 6005.48, 33.73, 256.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 13},
    {location = vector4(-440.15, 6002.3, 33.73, 43.92), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 14},
    {location = vector4(-440.73, 5998.79, 37.25, 1.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 15},
    {location = vector4(-435.49, 6004.22, 37.26, 86.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 16},
    {location = vector4(-444.82, 6006.2, 37.26, 256.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 17},
    {location = vector4(-449.02, 6012.96, 37.26, 178.92), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 18},
    {location = vector4(-441.52, 5999.8, 29.02, 311.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 19},
    {location = vector4(-431.88, 6007.97, 29.02, 111.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 20},
    {location = vector4(-439.14, 5999.31, 29.02, 278.92), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 21},
    {location = vector4(-432.19, 5999.29, 29.02, 93.92), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 22},
    {location = vector4(-443.38, 6007.69, 29.02, 253.92), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 23},
    {location = vector4(-446.74, 6009.03, 29.02, 276.42), canRotate = true, cameraType = "highquality", type = "Police Stations", name = "Paleto Bay", camera = 24}
}

CCTV_Config.TimecycleTypes = {
    ["blackwhite"] = "CAMERA_BW",
    ["clear"] = "eyeINtheSKY",
    ["highquality"] = "heliGunCam"
}
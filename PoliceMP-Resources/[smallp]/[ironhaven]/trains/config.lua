Config = {}

Config.FreightSpawns = {
	{-439.18, 5408.36, 78.79} --Position where the freight can spawn
}

Config.ShowFreightBlip = true --Show the Freight Blip on the Map

Config.FreightSpeed = 20.0 --Speed of the freight

Config.FreightWaitTime = 20.0 --The time the freight waits at the station in seconds

Config.FreightStations = {
    {trainStop = vector3(-147.1022, 6135.2642, 31.5761), exitCoord = vector3(-143.1544, 6143.7515, 32.3351)}, --Train Stop, Is where the freight begins to break, ExitCoord is where the player will be teleported to, when he leaves the freight
    {trainStop = vector3(2610.9351, 1701.4380, 26.7382), exitCoord = vector3(2615.2695, 1676.8164, 27.6008)},
    {trainStop = vector3(669.3027, -889.6052, 22.4004), exitCoord = vector3(674.3228, -897.1289, 22.4254)},
}

Config.ShowMetroBlip = true --Show the Metro Blip on the Map

Config.MetroSpeed = 20.0 --Speed of the metro

Config.MetroWaitTime = 20.0 --The time the metro waits at the station in seconds

Config.MetroStations = {
    {trainStop = vector3(264.02, -1198.38, 38.07), exitCoord = vector3(278.45, -1202.29, 38.89)},
    {trainStop = vector3(-302.09, -320.16, 9.17), exitCoord = vector3(-297.53, -307.77, 10.06)},
    {trainStop = vector3(-287.1221, -330.4218, 9.1736), exitCoord = vector3(-290.5941, -309.9994, 10.0632)},
    {trainStop = vector3(279.59, -1210.05, 38.07), exitCoord = vector3(278.45, -1202.29, 38.89)},
    {trainStop = vector3(-222.9382, -1046.1741, 29.3268), exitCoord = vector3(-212.3363, -1024.9633, 30.1383)},
    {trainStop = vector3(-204.6982, -1022.8315, 29.3234), exitCoord = vector3(-213.2155, -1037.7805, 30.1382)}, -- hm
    {trainStop = vector3(-492.3819, -665.5188, 10.9248), exitCoord = vector3(-510.8294, -668.7107, 11.8090)},
    {trainStop = vector3(-504.8213, -680.7617, 10.9172), exitCoord = vector3(-483.7954, -677.1413, 11.8090)},
    {trainStop = vector3(-1338.7777, -473.1241, 13.6844), exitCoord = vector3(-1352.5997, -454.2373, 15.0453)},
    {trainStop = vector3(-1352.5997, -454.2373, 15.0453), exitCoord = vector3(-1348.3134, -479.0424, 15.0454)}, --hm
    {trainStop = vector3(-807.6087, -141.5098, 18.5721), exitCoord = vector3(-806.6910, -137.8905, 19.9503)},
    {trainStop = vector3(-815.5061, -129.0942, 19.0580), exitCoord = vector3(-815.5061, -129.0942, 19.0580)},
    {trainStop = vector3(114.4560, -1719.4764, 29.1294), exitCoord = vector3(114.4560, -1719.4764, 29.1294)},
    {trainStop = vector3(116.0910, -1731.7980, 29.0567), exitCoord = vector3(112.8400, -1726.1486, 30.1102)},
    {trainStop = vector3(-524.4424, -1257.6945, 25.9175), exitCoord = vector3(-537.5703, -1278.2197, 26.9016)},
    {trainStop = vector3(-557.4183, -1308.2769, 25.9031), exitCoord = vector3(-543.4323, -1285.9001, 26.9016)}, -- hm
    {trainStop = vector3(-869.7905, -2303.8223, -12.6323), exitCoord = vector3(-877.9435, -2314.8721, -11.7328)},
    {trainStop = vector3(-897.0756, -2335.4414, -12.6088), exitCoord = vector3(-887.3155, -2320.4434, -11.7327)},
    {trainStop = vector3(-1067.5961, -2708.5562, -8.2887), exitCoord = vector3(-1081.2272, -2718.5388, -7.4101)},
    {trainStop = vector3(-1102.5308, -2727.1580, -8.3097), exitCoord = vector3(-1089.2284, -2718.1816, -7.4101)},
}

Config.MetroInteractionRadius = 15 --Max Distance a player can get in the Metro

Config.FreightInteractionRadius = 40 --Max Distance a player can get in the Freight

Config.ShouldBeTeleportedBack = true --If a Player Teleports out of the Train, he will be teleported back in the Train

Config.TeleportBackRange = 40.0 --If the player is outside this range, he will get teleported back in the Metro (only if Config.ShouldBeTeleportedBack is true)
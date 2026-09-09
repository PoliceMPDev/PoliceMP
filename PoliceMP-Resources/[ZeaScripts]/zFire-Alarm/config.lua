--@param: Please refer to our resource documentation for assistance with configuring this resource: docs.zeadevelopment.com.

cfg = { };

cfg.createCad = false --@param: Set this option to 'true' if you wish for a CAD to be generated on Albos FMS when an alarm is activated
cfg.autoBookOn = true -- @param:  Set this option as 'true' if you want the whitelisted group to be automatically booked.
cfg.notificationSound = false --@param: Set this option to 'true' if you wish to enable a sound to play when receiving notifications.

-- @param: Here, you have the option to enable automatic fire alarm activations. This will trigger a random alarm automatically after the set interval.
cfg.automaticAlarms = {
   allow = true,
   interval = math.random(120, 600) -- This is the interval, in seconds, during which a random alarm will be selected and triggered. It uses the formula [ math.random(lowest, highest) ] to determine the duration.
};

-- @param: Here you can enable and customize the fire/smoke detection system within buildings.
-- @param: Please note that SmartFires is not currently supported by the resource. We do intend to address this issue, but due to some complications, we cannot provide a specific timeline at this moment.
cfg.fireDetection = { 
   type = 'baseGame', -- [ 'none', 'baseGame' ]
   interval = 50, -- This is the interval at which the alarm system will check for the presence of fire or smoke. ( 1 = 250ms )
};

-- @param: Here, you have the option to enable Discord logging for fire alarm activations.
cfg.discordLog = {
   allow = true,
   webhook = 'https://discord.com/api/webhooks/1175449715748388935/lk09f4tykm_Sk2a7rtBGdo3Touo8vlJ7ThfnEE1Rm4M2LZNnB3zZ7mpJfwAJTmrR4t7S',

   communityName = "Fire Alarm",
   communityLogo = "https://i.imgur.com/sNgjLTM.png",
};

-- @param: In this section, you have the ability to modify all of the resource's whitelist.
cfg.whitelist = {
   acePerms = {
      use = true;
      group = 'zeadev.cmd.firealarm'
   }
}

-- @param: Here you can translate the script's text into other languages.
cfg.translations = {
   alarm_callpoint = 'ALARM ACTIVATION: Call Point ZONE:',
   alarm_panel = 'ALARM ACTIVATION: Alarm Panel',
   alarm_detected = 'ALARM ACTIVATION: Smoke Detected ZONE:',
   alarm_errors = {'ALARM ACTIVATION: System Fault', 'ALARM ACTIVATION: General Fault', 'ALARM ACTIVATION: Power Fault'},

   notification_reset = 'Alarm system has been reset!',
   notification_afa_on = 'AFA Activations have been enabled!',
   notification_afa_off = 'AFA Activations have been disabed!',
   notification_fireduty_on = 'Booked On!',
   notification_fireduty_off = 'Booked Off!',

   not_whitelisted = 'You are not whitelisted!'
};

-- @param: Here you can specify the locations of fire alarm systems.
-- @param: !! Please ensure that each block created has a unique number. If two blocks share the same number, it will result in a fatal error in the script. !!
cfg.alarmSystems = {
   -- BLOCK START --
   [1] = {
      buildingName = 'Life Invader';
      buildingAddress = 'South Boulevard Del Perro - (Postal: 663)';
      ['panelLocation'] = {
         coordinates = vector3(-1050.116, -241.6223, 38.1624);
         rotation = vector3(0.0, 0.0, 117.0737)
      };
      ['alarmSettings'] = {
         alarmSpeaker = vector3(-1066.7649, -241.9629, 44.4512);
         alarmDistance = 35.0
      };
      ['buttonLocations'] = {
         -- BLOCK START --
         {
            label = 'Rear Exit',
            coordinates = vector3(-1046.149, -233.9386, 39.3874);
            rotation = vector3(0.0, 0.0, -151.2564)
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            label = 'Rear Hallway',
            coordinates = vector3(-1058.411, -240.1012, 40.0069);
            rotation = vector3(0.0, 0.0, -151.5557)
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            label = 'Bottom Stairs',
            coordinates = vector3(-1068.090, -248.3892, 39.8143);
            rotation = vector3(0.0, 0.0, -152.7886)
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            label = 'Upper Stair',
            coordinates = vector3(-1073.964, -248.7425, 44.2712);
            rotation = vector3(0.0, 0.0, -62.8282)
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            label = 'Upper Office',
            coordinates = vector3(-1067.130, -245.2674, 44.2314);
            rotation = vector3(0.0, 0.0, 116.1649)
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            label = 'Main Lobby',
            coordinates = vector3(-1080.50, -256.1810, 37.9703);
            rotation = vector3(0.0, 0.0, -151.6642)
         };
         -- BLOCK END --
      }
   };
   -- BLOCK END --
   -- BLOCK START --
   [2] = {
      buildingName = 'Vanilla Unicorn';
      buildingAddress = 'Elgin Ave - (Postal: 133)';
      ['panelLocation'] = {
         coordinates = vector3(98.8397, -1289.871, 29.4662);
         rotation = vector3(0.0, 0.0, -59.9263)
      };
      ['alarmSettings'] = {
         alarmSpeaker = vector3(110.7265, -1288.2395, 33.8080);
         alarmDistance = 25.0
      };
      ['buttonLocations'] = {
         {
            label = 'Entrance',
            coordinates = vector3(130.4539, -1296.641, 29.3406);
            rotation = vector3(0.0, 0.0, -148.4884)
         };
         {
            label = 'Dance Room',
            coordinates = vector3(118.6034, -1299.041, 29.4729);
            rotation = vector3(0.0, 0.0, -58.3580)
         };
         {
            label = 'Main Stage',
            coordinates = vector3(114.4047, -1292.160, 28.5623);
            rotation = vector3(0.0, 0.0, -150.1334)
         };
         {
            label = 'Dressing Room',
            coordinates = vector3(108.1699, -1298.598, 29.1445);
            rotation = vector3(0.0, 0.0, -58.1690)
         };
         {
            label = 'Rear Entrance',
            coordinates = vector3(96.8885, -1285.839, 29.3383);
            rotation = vector3(0.0, 0.0, -60.2665)
         };
      }
   };
   -- BLOCK END --
   -- BLOCK START --
   [3] = {
      buildingName = 'Darnell Bros';
      buildingAddress = 'Vespucci Blvd - (Postal: 224)';
      ['panelLocation'] = {
         coordinates = vector3(705.6427, -962.3000, 30.9324);
         rotation = vector3(0.0, 0.0, 179.8460)
      };
      ['alarmSettings'] = {
         alarmSpeaker = vector3(714.6688, -963.9439, 30.3953);
         alarmDistance = 25.0
      };
      ['buttonLocations'] = {
         {
            label = 'Entrance',
            coordinates = vector3(716.5978, -975.2523, 24.9353);
            rotation = vector3(0.0, 0.0, -179.4516)
         };
         {
            label = 'Office',
            coordinates = vector3(709.8023, -963.3491, 30.5798);
            rotation = vector3(0.0, 0.0, -89.1467)
         };
         {
            label = 'Workshop',
            coordinates = vector3(721.0110, -964.4650, 30.6542);
            rotation = vector3(0.0, 0.0, -88.6336)
         };
      }
   };
   -- BLOCK END --
   -- BLOCK START -- Roger Salvage & Scrap : Mutiny Rd
   [4] = {
      buildingName = 'Roger Salvage & Scrap';
      buildingAddress = 'Mutiny Rd - (Postal: 386)';
      ['panelLocation'] = {
         coordinates = vector3(-615.4808, -1617.170, 33.4026);
         rotation = vector3(0.0, 0.0, -5.3474)
      };
      ['alarmSettings'] = {
         alarmSpeaker = vector3(-594.2596, -1611.0177, 35.6380);
         alarmDistance = 45.0
      };
      ['buttonLocations'] = {
         {
            label = 'Entrance Stairs',
            coordinates = vector3(-608.5797, -1613.082, 27.3405);
            rotation = vector3(0.0, 0.0, -96.8053)
         };
         {
            label = 'Reception Room',
            coordinates = vector3(-621.5739, -1624.397, 33.1870);
            rotation = vector3(0.0, 0.0, 173.2767)
         };
         {
            label = 'Locker Room',
            coordinates = vector3(-595.3268, -1616.180, 33.1674);
            rotation = vector3(0.0, 0.0, -5.7092)
         };
         {
            label = 'Rear Stair Case',
            coordinates = vector3(-561.0879, -1629.592, 29.2451);
            rotation = vector3(0.0, 0.0, -95.3885)
         };
         {
            label = 'Storage Room #1',
            coordinates = vector3(-608.0829, -1627.541, 33.0572);
            rotation = vector3(0.0, 0.0, -4.1601)
         };
         {
            label = 'Storage Room #2',
            coordinates = vector3(-587.1498, -1621.918, 33.0712);
            rotation = vector3(0.0, 0.0, -4.6989)
         };
         {
            label = 'Recyle Room #1',
            coordinates = vector3(-602.3746, -1612.337, 27.1101);
            rotation = vector3(0.0, 0.0, 174.5599)
         };
         {
            label = 'Recyle Room #2',
            coordinates = vector3(-570.7147, -1599.046, 27.0742);
            rotation = vector3(0.0, 0.0, -4.3648)
         };
         {
            label = 'Rear Exit',
            coordinates = vector3(-598.5339, -1628.388, 27.4107);
            rotation = vector3(0.0, 0.0, -2.7309)
         };
      }
   };
   -- BLOCK END --
   -- BLOCK START --
   [5] = {
      buildingName = 'Vangelico';
      buildingAddress = 'Eastbourne Way - (Postal: 697)';
      ['panelLocation'] = {
         coordinates = vector3(-628.6918, -227.8573, 38.5722);
         rotation = vector3(0.0, 0.0, -53.9576)
      };
      ['alarmSettings'] = {
         alarmSpeaker = vector3(-621.3819, -231.2050, 41.8860);
         alarmDistance = 15.0
      };
      ['buttonLocations'] = {
         {
            label = 'Store Floor #1',
            coordinates = vector3(-627.2145, -228.5242, 38.4097);
            rotation = vector3(0.0, 0.0, 89.5626)
         };
         {
            label = 'Store Floor #2',
            coordinates = vector3(-616.9920, -233.1619, 38.3643);
            rotation = vector3(0.0, 0.0, -90.1494)
         };
      }
   };
   -- BLOCK END --
   -- BLOCK START --
   [6] = {
      buildingName = 'MRPD';
      buildingAddress = 'Sinner Street - (Postal: 217)';
      ['panelLocation'] = {
         coordinates = vector3(435.0, -986.8221, 31.0);
         rotation = vector3(0.0, 0.0, 90)
      };
      ['alarmSettings'] = {
         alarmSpeaker = vector3(451.56, -989.88, 35.);
         alarmDistance = 40.0
      };
      ['buttonLocations'] = {
         {
            label = 'Entrance',
            coordinates = vector3(438.68, -979.15, 31.1);
            rotation = vector3(0.0, 0.0, -90.0)
         };
         {
            label = 'Main Lobby',
            coordinates = vector3(449.89, -984.96, 31.1);
            rotation = vector3(0.0, 0.0, -90.0)
         };
         {
            label = 'Custody',
            coordinates = vector3(461.97, -1000.32, 25.2);
            rotation = vector3(0.0, 0.0, 90.0)
         };
      }
   };
   -- BLOCK END --
}
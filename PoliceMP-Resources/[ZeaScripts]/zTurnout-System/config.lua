--@param: Please refer to our resource documentation for assistance with configuring this resource: docs.zeadevelopment.com

cfg = { };

cfg.alboFMS = false --@param Set this option to 'true' if you wish to intergrate Albos Force Management System (FMS).
cfg.zWigWags = true --@param: Set this option to 'true' if you wish to intergrate zWigWags.
cfg.notificationSound = false --@param: Set this option to 'true' if you wish to enable a sound to play when receiving notifications.
cfg.interactKeybind = 38 --@param: This feature allows you to set the keybind for interacting with a printer. [ E as the default option ]

-- @param: Here, you have the option to enable Discord logging.
cfg.discordLog = {
   allow = false,
   webhook = 'WEBHOOK_URL',

   communityName = "Community Name",
   communityLogo = "https://i.imgur.com/hAV6F86.jpeg",
}

-- @param: In this section, you have the ability to modify all of the resource's commands.
cfg.commands = {
   incidentForm = { command = 'incidentForm', helpText = 'Create mobilisation message' },
   tipSheet = { command = 'tipsheet', helpText = 'View tipsheet.' },
   giveSheet = { command = 'giveSheet', helpText = 'Give Id your tipsheet.' },
}

-- @param: In this section, you have the ability to modify all of the resource's whitelist.
cfg.whitelist = {
   acePerms = {
      use = false;
      group = 'zeadev.cmd.incidentform'
   }
}

-- @param: Here you can translate the script's text into other languages.
cfg.translations = {
   in_vehicle = 'Please refrain from checking the tipsheet while operating a vehicle.',
   no_tipsheet = 'You currently do not possess a tipsheet.',
   no_targetid = 'No target id entered! Usage: /giveSheet {TargetId}',
   invalid_target = 'The target ID you have entered is currently inactive.',
   received_sheet = 'You have received a Mobilization sheet from:',
   not_whitelisted = 'You are not whitelisted!'
}

-- @param: !! Please ensure that each block created has a unique number. If two blocks share the same number, it will result in a fatal error in the script. !!
cfg.turnoutSystems = {
   -- BLOCK START --
   [1] = {
      stationName = 'Whitechapel Fire Station';
      stationCallsign = 'LW';
      -- / Station Appliances: 
      ['stationAppliances'] = {
         'LW01'; -- DPL Pumps
         'LW02'; -- DPL Pumps
         'LW03'; -- DPL Pumps      
         'LW10'; -- FRU HGV
         'LW11'; -- FRU HGV
         'LW12'; -- FRU VAN 
         'LW20'; -- INCIDENT CONTROL VAN 
         'LW21'; -- OPERATION SUPPORT VAN / BASU
         'LW22'; -- DETECTION IDENIFICATION & MONITORING VAN (DIM)
         'LW50'; -- RESERVE PUMP
      };
      -- / Countertop Location: 
      ['counterLocation'] = {
         coordinates = vector3(1204.662, -1490.634, 33.8);
         rotation = vector3(0.0, 0.0, -90.0)
      };
      -- / Printer Location: 
      ['printerLocation'] = {
         coordinates = vector3(1205.3, -1490.88, 34.87);
         rotation = vector3(0.0, 0.0, -180.0)
      };
      -- / lollipopSettings Settings: 
      ['lollipopSettings'] = {
         allow = true,
         coordinates = vector3(1190.742, -1475.513, 37.28354);
         rotation = vector3(0.0, 0.0, -93.6966),
            colours = {
               ['BLUE'] = 'LW01',
               ['AMBER'] = 'KF02P2',
               ['RED'] = 'KF02A1',
               ['GREEN'] = 'KF02C1',
               ['WHITE'] = '',
            },
         displayTime = 30,
      },
      -- / Speaker Settings: 
      ['speakerSettings'] = {
         coordinates = vector3(1185.553, -1484.097, 39.31798);
         soundFile = '3101',
         distance = 35.0,
      };
   },
   [2] = {
      stationName = 'Mill Hill Fire Station';
      stationCallsign = 'LM';
      -- / Station Appliances: 
      ['stationAppliances'] = {
         'LM01'; -- DPL Pumps
         'LM02'; -- DPL Pumps
         'LM03'; -- DPL Pumps      
         'LM10'; -- FRU HGV
         'LM11'; -- FRU SUPPORT VAN
         'LM50'; -- RESERVE
      };
      -- / Countertop Location: 
      ['counterLocation'] = {
         coordinates = vector3(0,0,0);
         rotation = vector3(0.0, 0.0, -90.0)
      };
      -- / Printer Location: 
      ['printerLocation'] = {
         coordinates = vector3(1703.30, 3603.31, 35.59);
         rotation = vector3(0.0, 0.0, -60.0)
      };
      -- / lollipopSettings Settings: 
      ['lollipopSettings'] = {
         allow = true,
         coordinates = vector3(1704.45, 3602.70, 37.00);
         rotation = vector3(0.0, 0.0, -150.6966),
            colours = {
               ['BLUE'] = 'LM01,LM02,LM03',
               ['AMBER'] = 'KF02P2',
               ['RED'] = 'KF02A1',
               ['GREEN'] = 'KF02C1',
               ['WHITE'] = '',
            },
         displayTime = 30,
      },
      -- / Speaker Settings: 
      ['speakerSettings'] = {
         coordinates = vector3(1704.45, 3602.70, 37.00);
         soundFile = '3101',
         distance = 35.0,
      };
   },
   [3] = {
      stationName = 'Croyden Fire Station';
      stationCallsign = 'LC';
      -- / Station Appliances: 
      ['stationAppliances'] = {
         'LC01'; -- DPL Pumps
         'LC02'; -- DPL Pumps
         'LC03'; -- DPL Pumps      
         'LC04'; -- DPL Pumps
         'LC05'; -- DPL Pumps
         'LC50'; -- RESERVE
      };
      -- / Countertop Location: 
      ['counterLocation'] = {
         coordinates = vector3(-370.20, 6120.10, 30.46);
         rotation = vector3(0.0, 0.0, -135.0)
      };
      -- / Printer Location: 
      ['printerLocation'] = {
         coordinates = vector3(-369.85, 6119.58, 31.55);
         rotation = vector3(0.0, 0.0, -225.0)
      };
      -- / lollipopSettings Settings: 
      ['lollipopSettings'] = {
         allow = true,
         coordinates = vector3(-369.85, 6119.08, 33.55);
         rotation = vector3(0.0, 0.0, -300.6966),
            colours = {
               ['BLUE'] = 'LM01,LM02,LM03',
               ['AMBER'] = 'KF02P2',
               ['RED'] = 'KF02A1',
               ['GREEN'] = 'KF02C1',
               ['WHITE'] = '',
            },
         displayTime = 30,
      },
      -- / Speaker Settings: 
      ['speakerSettings'] = {
         coordinates = vector3(-369.85, 6119.08, 33.55);
         soundFile = '3101',
         distance = 35.0,
      };
   },
   [4] = {
      stationName = 'East Ham Fire Station';
      stationCallsign = 'LE';
      -- / Station Appliances: 
      ['stationAppliances'] = {
         'LE01'; -- RURAL DPL Pumps
         'LE02'; -- RURAL DPL Pumps
         'LE03'; -- RURAL DPL Pumps      
         'LE04'; -- Small Fire Unit
      };
      -- / Countertop Location: 
      ['counterLocation'] = {
         coordinates = vector3(342.50, 3388.00, 35.50);
         rotation = vector3(0.0, 0.0, -69.0)
      };
      -- / Printer Location: 
      ['printerLocation'] = {
         coordinates = vector3(343.11, 3388.04, 36.59);
         rotation = vector3(0.0, 0.0, -160.0)
      };
      -- / lollipopSettings Settings: 
      ['lollipopSettings'] = {
         allow = true,
         coordinates = vector3(333.09, 3404.12, 39.51);
         rotation = vector3(0.0, 0.0, -80.6966),
            colours = {
               ['BLUE'] = 'LM01,LM02,LM03',
               ['AMBER'] = 'KF02P2',
               ['RED'] = 'KF02A1',
               ['GREEN'] = 'KF02C1',
               ['WHITE'] = '',
            },
         displayTime = 30,
      },
      -- / Speaker Settings: 
      ['speakerSettings'] = {
         coordinates = vector3(-369.85, 6119.08, 33.55);
         soundFile = '3101',
         distance = 35.0,
      };
   };
   -- BLOCK END --
}
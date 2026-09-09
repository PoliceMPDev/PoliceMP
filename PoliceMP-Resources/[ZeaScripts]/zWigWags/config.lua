--@param: Please refer to our resource documentation for assistance with configuring this resource: docs.zeadevelopment.com

cfg = { };

cfg.zTurnoutSystem = true --@param: Set this option to 'true' if you wish to intergrate zTurnout-System.
cfg.notificationSound = false --@param: Set this option to 'true' if you wish to enable a sound to play when receiving notifications.

-- @param: In this section, you have the ability to modify all of the resource's commands.
cfg.commands = {
   name = 'wigwag';
   helpText = 'Initiate the activation of a WigWag Group based on its identification number.';
}

-- @param: In this section, you have the ability to modify all of the resource's whitelist.
cfg.whitelist = {
   acePerms = {
      use = true;
      group = 'zeadev.cmd.wigwag'
   }
}

-- @param: Here, you have the option to enable Discord logging.
cfg.discordLog = {
   allow = false,
   webhook = 'WEBHOOK_URL',

   communityName = "Community Name",
   communityLogo = "https://i.imgur.com/hAV6F86.jpeg",
}

-- @param: Here you can translate the script's text into other languages.
cfg.translations = {
   invalid_groupid = 'No valid GroupId provided.';
   not_whitelisted = 'You are not whitelisted!';
   activated_group = 'groupId activated: '
}

-- @param: !! Please ensure that each block created has a unique number. If two blocks share the same number, it will result in a fatal error in the script. !!
cfg.wigwagGroups = {
   -- BLOCK START -- 
   -- Whitechapel
   [1] = {
      {
         coordinates = vector3(1168.515, -1448.383, 33.7);
         rotation = vector3(0.0, 0.0, -180.8235);
         stopRadius = 40.0
      };
      {
         coordinates = vector3(1215.824, -1427.087, 34.2);
         rotation = vector3(0.0, 0.0, 10);
         stopRadius = 40.0
      }
   };
   -- Mill Hill
   [2] = {
      {
         coordinates = vector3(1714.683, 3590.873, 34.2);
         rotation = vector3(0.0, 0.0, 30);
         stopRadius = 25.0
      };
      {
         coordinates = vector3(1697.484, 3565.304, 34.4);
         rotation = vector3(0.0, 0.0, 210);
         stopRadius = 25.0
      }
   };
   -- Croyden Fire Station
   [3] = {
      {
         coordinates = vector3(-395.266, 6129.459, 30.2);
         rotation = vector3(0.0, 0.0, 200);
         stopRadius = 25.0
      };
      {
         coordinates = vector3(-375.1852, 6171.592, 30.2);
         rotation = vector3(0.0, 0.0, 30);
         stopRadius = 25.0
      }
   };
   -- East Ham Fire Station
   [4] = {
      {
         coordinates = vector3(324.4895, 3431.8, 35.1);
         rotation = vector3(0.0, 0.0, 200);
         stopRadius = 30.0
      };
      {
         coordinates = vector3(369.6492, 3472.769, 34.1);
         rotation = vector3(0.0, 0.0, 20);
         stopRadius = 30.0
      }
   };
}
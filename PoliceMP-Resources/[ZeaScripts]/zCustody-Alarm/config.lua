--@param: Please refer to our resource documentation for assistance with configuring this resource: docs.zeadevelopment.com.

cfg = { };

cfg.createCad = false --@param: Set this option to 'true' if you wish for a CAD to be generated on Albos FMS when an alarm is activated
cfg.autoBookOn = true -- @param:  Set this option as 'true' if you want the whitelisted group to be automatically booked.
cfg.notificationSound = false --@param: Set this option to 'true' if you wish to enable a sound to play when receiving notifications.
cfg.interactionKeybind = 38 --@param: This option enables you to specify the keybind for interacting with objects. [ E as Default ]

-- @param: Here, you have the option to enable Discord logging for alarm activations.
cfg.discordLog = {
   allow = true,
   webhook = 'https://discord.com/api/webhooks/1175449715748388935/lk09f4tykm_Sk2a7rtBGdo3Touo8vlJ7ThfnEE1Rm4M2LZNnB3zZ7mpJfwAJTmrR4t7S',

   communityName = "Custody Alarm",
   communityLogo = "https://i.imgur.com/FZB5i2s.png",
};

-- @param: In this section, you have the ability to modify all of the resource's whitelist.
cfg.whitelist = {
   acePerms = {
      use = true;
      group = 'zeadev.cmd.custodyalarm'
   }
}

-- @param: Here you can translate the script's text into other languages.
cfg.translations = {
   alarm_callpoint = 'ALARM ACTIVATION: ',
   alarm_panel = 'ALARM ACTIVATION: Control Panel',

   notification_reset = 'Alarm system has been reset!',
   notification_duty_on = 'Booked On!',
   notification_duty_off = 'Booked Off!',

   not_whitelisted = 'You are not whitelisted!'
};

-- @param: !! Please ensure that each block created has a unique number. If two blocks share the same number, it will result in a fatal error in the script. !!
cfg.alarmSystems = {
   -- BLOCK START --
   [1] = {
      stationName = 'Sinners Street Police Station';
      stationAddress = 'Sinners Street';
      ['panelLocation'] = {
         coordinates = vector3(460.365, -991.36, 25.510);
         rotation = vector3(0.0, 0.0, -89.8670)
      };
      ['alarmSettings'] = {
         alarmSpeaker = vector3(466.8330, -996.2169, 32.0373);
         alarmDistance = 35.0
      };
      ['activationPoints'] = {
         -- BLOCK START --
         {
            type = 'Button';
            location = 'Custody Desk';
            coordinates = vector3(461.6249, -993.9178, 25.0586);
            rotation = vector3(0.0, 0.0, 179.9283)
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Custody Suite';
            coordinates = vector3(457.0784, -998.0143, 25.005);
            distance = 1.5
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Custody Suite';
            coordinates = vector3(459.4921, -999.7260, 25.005);
            distance = 1.5
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Custody Suite';
            coordinates = vector3(465.4640, -993.4683, 25.005);
            distance = 1.5
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Custody Suite';
            coordinates = vector3(462.8454, -989.3170, 25.1143);
            distance = 1.5
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Custody Suite';
            coordinates = vector3(434.9503, -990.2525, 26.8642);
            distance = 1.5
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Custody Suite';
            coordinates = vector3(469.8267, -998.1408, 24.9150);
            distance = 1.5
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Custody Suite';
            coordinates = vector3(469.8267, -1002.0408, 24.9150);
            distance = 1.5
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Interview Room';
            coordinates = vector3(462.8454, -989.3170, 25.1143);
            distance = 1.5
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Interview Room';
            coordinates = vector3(436.5277, -983.8764, 26.6741);
            distance = 1.5
         };
         -- BLOCK END --
         -- BLOCK START --
         {
            type = 'Alarm Bar';
            location = 'Interview Room';
            coordinates = vector3(434.9768, -986.0126, 26.6776);
            distance = 1.5
         };
         -- BLOCK END --
      }
   };
   -- BLOCK END --
}

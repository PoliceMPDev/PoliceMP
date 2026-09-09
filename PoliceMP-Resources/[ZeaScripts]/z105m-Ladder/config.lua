--@param: Please refer to our resource documentation for assistance with configuring this resource: docs.zeadevelopment.com.

cfg = { };

cfg.notificationSound = false --@param: Set this option to 'true' if you wish to enable a sound to play when receiving notifications.

cfg.distanceMountTop = 1.5;
cfg.distanceMountBottom = 3.5;

-- @param: In this section, you have the ability to modify all of the resource's commands.
cfg.commands = {
   name = '10.5m';
   helpText = 'Collect, Stow ladder.';
   arguments = {'collect', 'stow'}
}

-- @param: In this section, you have the ability to modify all of the resource's keybinds.
cfg.keybinds = {
   climb = 73; --@param: [ X as the default option ]
   pickup = 311 -- @param: [ G as the default option ]
}

-- @param: In this section, you have the ability to modify all of the resource's whitelist.
cfg.whitelist = {
   acePerms = {
      use = true;
      group = 'zeadev.cmd.105'
   }
}

-- @param: Here you can translate the script's text into other languages.
cfg.translations = {
   climb_ladder = '[~y~X~w~] Climb';
   pickup_ladder = '[~y~K~w~] Pickup';
   no_vehicle = 'No Vehicle Found!';
   vehicle_empty = 'Vehicle has no ladders!';
   vehicle_full = 'Vehicle is full!';
   not_carrying = 'You are not carrying a ladder!';
   carrying = 'You are already carrying a ladder!';
   not_whitelisted = 'You are not whitelisted!'
}

-- @param: In this section, you can add vehicles from which the ladder can be retrieved.
cfg.vehicles = {
   -- BLOCK START --
   [GetHashKey('LFB1')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 2;
      distance = 6.2;
   },
   [GetHashKey('electricLFB')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 2;
      distance = 6.2;
   },
   [GetHashKey('lfb5')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('lfbtraining')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('lfb10')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('fire11')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('fire16')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('2004ScaniaMill')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('addpolfiretruk')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('reserve4')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('NBFiretruck')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('fire17')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('fire18')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('2004ScaniaCroy')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('reserve3')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('NBFiretruck1')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('2004ScaniaRom')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('reserve2')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('lfb2')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('manrural')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   },
   [GetHashKey('mprv')] = {
      offset = vector3(0.0, -4.9, 0.6);
      amount = 1;
      distance = 3.2;
   }
   -- BLOCK END --
}
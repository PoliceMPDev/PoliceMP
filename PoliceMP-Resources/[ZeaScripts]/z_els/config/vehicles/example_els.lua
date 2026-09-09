--@param: Please refer to our resource documentation for assistance with configuring this resource: docs.zeadevelopment.com.

return 

------------- # ------------- # -------------

{ 
   ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
   ---@field vehicle: Miscellaneous Settings for the vehicle.

   ['vehicle'] = {
      autoRepairDisabled = true, -- False: This vehicle will automatically repair when an extra is toggled on. True: This vehicle will not automatically repair.
      autoDisabledFrontWhites = false, -- True: This vehicle's front white lights will be disabled at night automatically.
      autoEnabledRearReds = false, -- True: This vehicle's rear red lights will be enabled when entering onscene mode.
      seatsCanControl = {-1}, -- These are the seats which are allowed to control the vehicles lighting and sirens

      soundEffects = {
         ------------- # ------------- # -------------

         primary = '999mode.wav',
         onscene = 'sceneMode.wav',
         reset = 'leaveMode.wav',

         ------------- # ------------- # -------------

         toggle_siren = {
            ['activated'] = 'carHorn.wav',
            ['deactivated'] = 'carHorn.wav'
         },         
         switch_siren = 'carHorn.wav',

         ------------- # ------------- # -------------

         front_whites = {
            ['activated'] = 'optilink.wav',
            ['deactivated'] = 'optilink.wav'
         },
         front_blues = {
            ['activated'] = 'optilink.wav',
            ['deactivated'] = 'optilink.wav'
         },

         ------------- # ------------- # -------------

         rear_reds = {
            ['activated'] = 'optilink.wav',
            ['deactivated'] = 'optilink.wav'
         },
         rear_blues = {
            ['activated'] = 'optilink.wav',
            ['deactivated'] = 'optilink.wav'
         },

         ------------- # ------------- # -------------

         alley_lights = {
            ['activated'] = 'optilink.wav',
            ['deactivated'] = 'optilink.wav'
         },

         message_board = {
            ['activated'] = 'optilink.wav',
            ['deactivated'] = 'optilink.wav'
         }

         ------------- # ------------- # -------------
      },

      userInterface = {
         allow = true, -- False: This vehicle will not be able to use a UI panel. True: This vehicle will be able to use a UI panel.
         type = 'standby_maxi' -- Types: 'standby_maxi' // 'standby_midi' // 'standby_mini'
      }
   },

   ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
   ---@field lighting: Lighting pattern configuration for the vehicle.
   ---@comment If you are using ELS, please refer to the patterns.lua file to find all available patterns. Here you can create new ones and edit existing ones as well.
   ---@comment If you are not using a specific lighting option, please ensure it is left as option = {}.

   ['lighting'] = {
      primary = 'Quadrouple-Flash-Fast',
      onscene = 'Quadrouple-Flash-Slow',

      front_whites = {},
      front_blues = {},

      rear_reds = 'Double-Flash-Slow',
      rear_blues = {},

      alley_lights = {},
      message_board = {'Cycle-Slow', 'Flash-Single'},

      safe_extras = {},
   },

   ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
   ---@field sirens: Siren configuration for the vehicle.

   ['sirens'] = {
      hasSirens = true, -- False: This vehicle will not be able to use sirens. True: This vehicle will be able to use sirens.

      sirenList = {
            -- * Siren Start * --
         {
            name = 'SIREN_NAME',
            soundset = 'DLC_NAME_SOUNDSET'
         },
            -- * Siren End (Ensure that the closing bracket has a comma)* --

            -- * Siren Start * --
         {
            name = 'SIREN_NAME',
            soundset = 'DLC_NAME_SOUNDSET'
         },
            -- * Siren End (Ensure that the closing bracket has a comma)* --

            -- * Siren Start * --
         {
            name = 'SIREN_NAME',
            soundset = 'DLC_NAME_SOUNDSET'
         },
            -- * Siren End (Ensure that the closing bracket has a comma)* --
      },

      hasDualTones = false,
      name = 'SIREN_NAME',
      soundset = 'DLC_NAME_SOUNDSET'
   },

   ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
   ---@field bullhorn: Bullhorn configuration for the vehicle.

   ['bullhorn'] = {
      hasBullhorn = true, -- False: This vehicle will not be able to use the bullhorn. True: This vehicle will be able to use the bullhorn.
      
      bullhorn = {
         sirenInterrupt = true, -- False: This vehicle's bullhorn will not pause the active siren. True: This vehicle's bullhorn will pause the active siren.
         
         name = 'SIREN_9',
         soundset = 'DLC_NAME_SOUNDSET'
      }
   },

   ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
   ---@field envLighting

   ['envLighting'] = {
         -- * Extra Start (extra_1) * --
      [1] = {
         types = {'diagonal_rearRight', 'rear_sideRight'},
         rgb = {0, 0, 255},
         distance = 50.0,
         brightness = 0.9,
         radius = 45.0,
         falloff = 150.0
      },
         -- * Extra End (Ensure that the closing bracket has a comma)* --
         -- * Extra Start (extra_2) * --
      [2] = {
         types = {'diagonal_frontRight', 'front_sideRight'},
         rgb = {0, 0, 255},
         distance = 50.0,
         brightness = 0.9,
         radius = 45.0,
         falloff = 150.0
      },
         -- * Extra End (Ensure that the closing bracket has a comma)* --
         -- * Extra Start (extra_3) * --
      [3] = {
         types = {'diagonal_frontLeft', 'front_sideLeft'},
         rgb = {0, 0, 255},
         distance = 50.0,
         brightness = 0.9,
         radius = 45.0,
         falloff = 150.0
      },
         -- * Extra End (Ensure that the closing bracket has a comma)* --
         -- * Extra Start (extra_4) * --
      [4] = {
         types = {'diagonal_rearLeft', 'rear_sideLeft'},
         rgb = {0, 0, 255},
         distance = 50.0,
         brightness = 0.9,
         radius = 45.0,
         falloff = 150.0
      },
         -- * Extra End (Ensure that the closing bracket has a comma)* --
         -- * Extra Start (extra_5) * --
      [5] = {
         types = {'forward_frontLeft'},
         rgb = {255, 255, 255},
         distance = 25.0,
         brightness = 1.0,
         radius = 25.0,
         falloff = 150.0
      },
         -- * Extra End (Ensure that the closing bracket has a comma)* --
         -- * Extra Start (extra_6) * --
      [6] = {
         types = {'forward_frontRight'},
         rgb = {255, 255, 255},
         distance = 25.0,
         brightness = 1.0,
         radius = 25.0,
         falloff = 150.0
      },
         -- * Extra End (Ensure that the closing bracket has a comma)* --
         -- * Extra Start (extra_7) * --
      [7] = {
         types = {'backward_rearRight'},
         rgb = {255, 0, 0},
         distance = 35.0,
         brightness = 0.9,
         radius = 35.0,
         falloff = 150.0
      },
         -- * Extra End (Ensure that the closing bracket has a comma)* --
         -- * Extra Start (extra_9) * --
      [9] = {
         types = {'backward_rearLeft'},
         rgb = {255, 0, 0},
         distance = 35.0,
         brightness = 0.9,
         radius = 35.0,
         falloff = 150.0
      },
         -- * Extra End (Ensure that the closing bracket has a comma)* --
   },

}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
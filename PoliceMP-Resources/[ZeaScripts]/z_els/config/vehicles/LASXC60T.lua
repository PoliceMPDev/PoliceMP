--@param: Please refer to our resource documentation for assistance with configuring this resource: docs.zeadevelopment.com.

return 

{ 
   ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
   ---@field vehicle: Miscellaneous Settings for the vehicle.

   ['vehicle'] = {
      autoRepairDisabled = true, -- False: This vehicle will automatically repair when an extra is toggled on. True: This vehicle will not automatically repair.
      autoDisabledFrontWhites = true, -- True: This vehicle's front white lights will be disabled at night automatically.
      autoEnabledRearReds = true, -- True: This vehicle's rear red lights will be enabled when entering onscene mode.
      seatsCanControl = {-1, 0}, -- These are the seats which are allowed to control the vehicles lighting and sirens

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
         type = 'standby_midi' -- Types: 'standby_maxi' // 'standby_midi' // 'standby_mini'
      }
   },

   ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
   ---@field lighting: Lighting pattern configuration for the vehicle.
   ---@comment If you are using ELS, please refer to the patterns.lua file to find all available patterns. Here you can create new ones and edit existing ones as well.
   ---@comment If you are not using a specific lighting option, please ensure it is left as option = {}.

   ['lighting'] = {
      primary = {1, 2, 3, 7, 8},
      onscene = {2, 4, 5, 6},

      front_whites = {3},
      front_blues = {1, 7, 8},

      rear_reds = {4},
      rear_blues = {2},

      alley_lights = {},
      message_board = {9},

      safe_extras = {11, 12},
   },

   ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
   ---@field sirens: Siren configuration for the vehicle.

   ['sirens'] = {
      hasSirens = true, -- False: This vehicle will not be able to use sirens. True: This vehicle will be able to use sirens.

      sirenList = {
            -- * Siren Start * --
         {
            name = 'rsg_wail',
            soundset = 'SIRENPACK_ONE_SOUNDSET'
         },
            -- * Siren End (Ensure that the closing bracket has a comma)* --

            -- * Siren Start * --
         {
            name = 'rsg_yelp',
            soundset = 'SIRENPACK_ONE_SOUNDSET'
         },
            -- * Siren End (Ensure that the closing bracket has a comma)* --

            -- * Siren Start * --
         {
            name = 'rsg_twotone',
            soundset = 'SIRENPACK_ONE_SOUNDSET'
         },
            -- * Siren End (Ensure that the closing bracket has a comma)* --
            -- * Siren Start * --
         {
             name = 'rsg_phaser',
            soundset = 'SIRENPACK_ONE_SOUNDSET'
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
         
         name = 'bullhorn_one',
         soundset = 'SIRENPACK_ONE_SOUNDSET'
      }
   }
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
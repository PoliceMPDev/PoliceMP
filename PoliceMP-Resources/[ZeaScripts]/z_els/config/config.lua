--@param: Please refer to our resource documentation for assistance with configuring this resource: docs.zeadevelopment.com.

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------

---@class cfg : Configuration
cfg = {}

---@field indicators boolean
cfg.indicators = true --@comment: Do you wish for z_els to use its inbuilt indicators

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------

---@field vehicles string
---@example: 'police1' or 'firetruk'
cfg.vehicles = {
   'LASXC60C',
   'LASXC60',
   'LASXC60U',
   'LASXC60T',
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------

---@field audioBanks string
---@comment: In this section, you can enter the audio that you wish to utilize within the resource.
---@example: 'DLC_WALSHEY\\SIRENPACK_ONE' or 'DLC_WMSIRENS\\SIRENPACK_ONE'
cfg.audioBanks = {
   'DLC_WALSHEY\\SIRENPACK_ONE',
   'DLC_WMSIRENS\\SIRENPACK_ONE',
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------

---@field commands table
cfg.commands = {
   ['toggleUI'] = {
      disable = false,
      command = 'elspanel',
      description = 'Toggle the ELS UserInterface.',
   }
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------

---@field keybinds table
---@comment: Allow for the customization of keybinds utilized by the resource.
---@comment: Keymappings: https://docs.fivem.net/docs/game-references/input-mapper-parameter-ids/keyboard/
cfg.keybinds = {
   primary_onscene = 'Q',

   toggle_siren = 'LMENU',
   switch_siren = 'R',
   dual_tones = 'G',

   rear_reds = 'K',
   rear_blues = 'NUMPAD8',

   front_whites = 'U',
   front_blues = 'B',

   alley_lights = 'X',
   message_board = 'M',

   right_indicator = 'Right',
   left_indicator = 'Left',
   hazard_lights = 'Back',

   nui_focus = 'CAPSLOCK',

   bullhorn_keyboard = 'TAB', ---@comment: This used core/client/classes/keybinds.lua
   bullhorn_controller = 'CONTROLLER_LEFT_BUMPER', ---@comment: This used core/client/classes/keybinds.lua
}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
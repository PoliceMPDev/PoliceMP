Config = {

  -- List of the player who have access to this location (can be an ESX job), comment it completly if you want everybody to have access
  --WhiteList = {
   -- 'group.heto'
 -- },

  -- Keybinds, you can choose from here : https://docs.fivem.net/docs/game-references/input-mapper-parameter-ids/
  DefaultControls = function()
    RegisterKeyMapping('+winch_start',  'Winch - Menu',         'KEYBOARD',          'R') -- Start selection
    RegisterKeyMapping('+winch_select', 'Winch - Select',   'MOUSE_BUTTON', 'MOUSE_LEFT') -- Validate selection
    RegisterKeyMapping('+winch_wind',   'Winch - Wind',         'KEYBOARD',     'PAGEUP') -- Retract         rope
    RegisterKeyMapping('+winch_unwind', 'Winch - UnWind',       'KEYBOARD',   'PAGEDOWN') -- Extend          rope
    RegisterKeyMapping('+winch_break',  'Winch - Break',        'KEYBOARD',          'B') -- Cut             rope
    RegisterKeyMapping('+winch_prev',   'Winch - Previous',     'KEYBOARD',       'LEFT') -- Select previous rope
    RegisterKeyMapping('+winch_next',   'Winch - Next',         'KEYBOARD',      'RIGHT') -- Select next     rope
    RegisterKeyMapping('+winch_all',    'Winch - All',          'KEYBOARD',     'LSHIFT') -- Select all      ropes
  end,

  -- Maximum number of ropes per user
  MaxRopes = 4,

  -- Configure ropes
  Ropes = {
    MinLength    =  0.5,
    MaxLength    = 15.0,
    RopeType     =    5,
    WindingSpeed =  0.5,
  },

  -- Can you add a rope on :
  Types = {
    Self = false, -- Yourself ?
    Plys = false, -- Other players ?
    Peds = true, -- Pedestrians (non player) ?
    Vehs = true, -- Vehicles ?
    Objs = true, -- Objects ?
  },

  -- List of model you can't add a rope on (type doesn't matter)
  BlackList = {
    [`POLICE`] = true
  },

  Strings = {
    ["rope1"]      = "~g~ 1~w~/2 point saved",
    ["rope_ok"]    = "~g~ 2~w~/2 point saved\n Winch ~g~created",
    ["too_long"]   = "The two entities are ~r~too far~w~ apart",
    ["no_obj"]     = "~r~No object~w~ to attach",
    ["max_rope"]   = "You reach your ~r~rope limit~w~",
    ['Rope_Menu1'] = '~INPUT_41393B98~ Rope (%d/%d) ~INPUT_6E0EFF32~\n',
    ['Rope_Menu2'] = '~INPUT_F90E6096~ Length ~g~%.1f~w~m ~INPUT_A0C60889~\n',
    ['Rope_Menu3'] = '~INPUT_FD4AFE6D~ Cut rope\n~INPUT_4F7D4317~ Control all',
  },
}

function DisplayHelpText(lineOne, lineTwo, lineThree)
  BeginTextCommandDisplayHelp("THREESTRINGS")
  AddTextComponentSubstringPlayerName(lineOne)
  AddTextComponentSubstringPlayerName(lineTwo or "")
  AddTextComponentSubstringPlayerName(lineThree or "")
  EndTextCommandDisplayHelp(0, 0, 0, -1)
end

local prev = nil
function ShowNotification(message)
  if prev ~= nil then
    RemoveNotification(prev)
  end
  AddTextEntry("RopeNotif", Config.Strings[message])
  BeginTextCommandThefeedPost("RopeNotif")
  prev = EndTextCommandThefeedPostTicker(false, false)
end

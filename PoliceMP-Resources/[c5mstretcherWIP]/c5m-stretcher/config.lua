Config = {}

local Keys = {
	["ESC"] = 322, ["F1"] = 288, ["F2"] = 289, ["F3"] = 170, ["F5"] = 166, ["F6"] = 167, ["F7"] = 168, ["F8"] = 169, ["F9"] = 56, ["F10"] = 57, 
	["~"] = 243, ["1"] = 157, ["2"] = 158, ["3"] = 160, ["4"] = 164, ["5"] = 165, ["6"] = 159, ["7"] = 161, ["8"] = 162, ["9"] = 163, ["-"] = 84, ["="] = 83, ["BACKSPACE"] = 177, 
	["TAB"] = 37, ["Q"] = 44, ["W"] = 32, ["E"] = 38, ["R"] = 45, ["T"] = 245, ["Y"] = 246, ["U"] = 303, ["P"] = 199, ["["] = 39, ["]"] = 40, ["ENTER"] = 18,
	["CAPS"] = 137, ["A"] = 34, ["S"] = 8, ["D"] = 9, ["F"] = 23, ["G"] = 47, ["H"] = 74, ["K"] = 311, ["L"] = 182,
	["LEFTSHIFT"] = 21, ["Z"] = 20, ["X"] = 73, ["C"] = 26, ["V"] = 0, ["B"] = 29, ["N"] = 249, ["M"] = 244, [","] = 82, ["."] = 81,
	["LEFTCTRL"] = 36, ["LEFTALT"] = 19, ["SPACE"] = 22, ["RIGHTCTRL"] = 70, 
	["HOME"] = 213, ["PAGEUP"] = 10, ["PAGEDOWN"] = 11, ["DELETE"] = 178,
	["LEFT"] = 174, ["RIGHT"] = 175, ["TOP"] = 27, ["DOWN"] = 173,
	["NENTER"] = 201, ["N4"] = 108, ["N5"] = 60, ["N6"] = 107, ["N+"] = 96, ["N-"] = 97, ["N7"] = 117, ["N8"] = 61, ["N9"] = 118
}

-- Enable and disable a marker at the pickup point of the ambulance
-- Set to false or true
Config.DevMode = false

-- PRECONFIG: Input "yes" if stretcher model contains the C5M stretcher attach dummy (misc_z). Input "no" otherwise.
-- EXTRAS: Set both variables to "0" to prevent toggling extras. To toggle extras, input respective extra numbers.
-- DOORS: "rearpass" (Rear Passenger Doors); "cargodoor" (Trunk and Trunk2) (not yet implemented); "override" (Exclude Opening Doors)

Config.Ambulance = {
	{hash = "ambulance", doors = "rearpass", position = -4.5, positionRange = 2.0,preconfig = "no", depth = 0.0, height = 0.0, offset = 2.0, rotationY = 0.0},
	{hash = "ALSrescue1", doors = "rearpass", position = -4.5, positionRange = 2.0, preconfig = "no"},
    {hash = "addpolambulance2", doors = "rearpass", position = -4.5, positionRange = 2.0, preconfig = "no"},
    {hash = "sprinter22", doors = "rearpass", position = -4.5, positionRange = 2.0, preconfig = "no"},
    {hash = "ducatto", doors = "rearpass", position = -4.5, positionRange = 2.0, preconfig = "no"},
    {hash = "ambulance3", doors = "rearpass", position = -4.5, positionRange = 2.0, preconfig = "no"},
}

Config.Emotes1 = {
    animationName = "Emote 1",
    animationDictionary = "savecouch@",
    animationClip = "t_sleep_loop_couch",
	offsetX = "0.0",
    offsetY = "0.1",
    offsetZ = "1.0",
}
Config.Emotes2 = {
    animationName = "Emote 2",
    animationDictionary = "nil",
    animationClip = "nil",
	offsetX = "0.0",
    offsetY = "0.0",
    offsetZ = "0.0",
}
Config.Emotes3 = {
    animationName = "Emote 3",
    animationDictionary = "nil",
    animationClip = "nil",
	offsetX = "0.0",
    offsetY = "0.0",
    offsetZ = "0.0",
}
Config.Emotes4 = {
    animationName = "Emote 4",
    animationDictionary = "nil",
    animationClip = "nil",
	offsetX = "0.0",
    offsetY = "0.0",
    offsetZ = "0.0",
}
Config.Emotes5 = {
    animationName = "Emote 5",
    animationDictionary = "nil",
    animationClip = "nil",
	offsetX = "0.0",
    offsetY = "0.0",
    offsetZ = "0.0",
}

Config.Control = {
    menu_prompt = Keys["E"], 				--key to open interaction menu
    stretcher_grab = Keys["E"],				--take/stow/unstow stretcher
    stretcher_place = Keys["E"],			--place stretcher flat
    stretcher_placeStanding = Keys["Y"], 	--place stretcher standing
    ambulance_doors = Keys["H"], 			--open ambulance doors
    dismount = Keys["Y"]
}

Config.Interaction = {
    sit_right = 'Sit on right side',
    sit_left = 'Sit on left side',
    lie_down = 'Lay on back',
    lie_downRelaxed = 'Lay on back (Relaxed)',
    lie_downStiff = 'Lay on back (Stiff)',
    sit_up1 = 'Sit on stretcher (1)',
}

Config.Customization = {
    toggle_sides = 'Fold Side Guard',
    toggle_sheet = 'Toggle Sheet',
    toggle_seat = 'Toggle Seat Position',
    toggle_Defib = 'Toggle Defib',
    toggle_o2tank = 'Toggle Oxygen Tank',
}

Config.ai = {
    pickup_closest = 'Pickup Closest Dead Ped',
    empty_stretcher = 'Clear Stretcher',
}

Config.Language = {
    menu_prompt = 'Press ~INPUT_CONTEXT~ to open interaction menu',
    stretcher_grab = 'Press ~INPUT_CONTEXT~ to take stretcher',
	ambulance_prompt = 'Press ~INPUT_VEH_ROOF~ to interact with doors.~n~Press ~INPUT_CONTEXT~ to stow stretcher.',
    ambulance_prompt1 = 'Press ~INPUT_CONTEXT~ to stow stretcher.',
    dismount = 'Press ~INPUT_MP_TEXT_CHAT_TEAM~ to dismount stretcher',
}
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

Config = {}

Config.Hash = {
	{hash = "addnhsmercbox19", detection = 4.8, depth = -1.8, height = -0.20},
	{hash = "sprinter22", detection = 4.8, depth = -1.8, height = -0.40},
	{hash = "ducatto", detection = 4.8, depth = -1.8, height = -0.20},
	{hash = "ambulance3", detection = 4.8, depth = -1.8, height = -0.40},
	{hash = "addpoltransitnhs", detection = 4.8, depth = -1.8, height = -0.20},
	{hash = "addpolambulance2", detection = 4.8, depth = -1.8, height = -0.40},
	{hash = "ambulance8", detection = 4.8, depth = -1.8, height = -0.20},
	{hash = "ambuxc90nil", detection = 4.8, depth = -1.8, height = -0.20},
	{hash = "ambodefrural", detection = 4.8, depth = -1.8, height = 0.15},
	{hash = "ukambu1", detection = 4.8, depth = -1.8, height = -0.20},
		
}

-- detection.. lesser number is closer.. bigger number is further
-- depth.. more positive is closer to cab.. more negative is further from cab
-- height.. lesser number is lower.. higher number is higher

Config.Press = {
	take_bed = Keys["R"],
	do_action = Keys["R"],
	out_vehicle_bed = Keys["R"],
	release_bed = Keys["R"],
	in_vehicle_bed = Keys["R"],
	go_out_bed = Keys["R"],
}


Config.Language = {
	name_hospital = 'NHS Stretcher',
	do_action = 'Press ~INPUT_RELOAD~ to interact with stretcher',
	take_bed = "Press ~INPUT_RELOAD~ to take stretcher",
	release_bed = "Press ~INPUT_RELOAD~ to drop stretcher",
	in_vehicle_bed = "Press ~INPUT_RELOAD~ to stow stretcher",
	out_vehicle_bed = "Press ~INPUT_RELOAD~ to retrieve stretcher",
	anim = {
		lie_back = "Lay on the back",
		sit_right = "Sit stretcher right side",
		sit_left = "Sit stretcher left side",
		pls = "Sit on stretcher",
	}
}
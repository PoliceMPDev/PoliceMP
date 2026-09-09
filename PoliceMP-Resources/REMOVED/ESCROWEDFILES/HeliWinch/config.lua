Config = {} -- Do not touch this line
Config.KeyMappings = {}

-- Control Configuration
-- Use this website to get the Control Index https://docs.fivem.net/docs/game-references/controls/
Config.ExtendButton = 173 -- Down Arrow
Config.RetractButton = 172 -- Up Arrow

Config.HoldModifier = 21 -- Shift

Config.AttachButton = 48 -- Z
Config.AttachButtonName = "~INPUT_HUD_SPECIAL~" -- Name column of the controls table, must keep the ~ either side of it

Config.DetachButton = 73 -- X
Config.DetachButtonName = "~INPUT_VEH_DUCK~"

Config.InBasketButton = 47 -- G
Config.InBasketButtonName = "~INPUT_DETONATE~"

Config.WinchSpeed = 1.0
Config.WinchRetractSpeed = 2.0

Config.AnimationProblems = false

Config.ShowKeybindPrompts = true



Config.HoldModifier = 21 -- Shift

Config.WinchSpeed = 1.0
Config.WinchRetractSpeed = 2.0

Config.AnimationProblems = false

Config.ShowKeybindPrompts = true

Config.DisableAllCommands = false

-- Vehicle Configuration
Config.AllowedVehicles = {
    {["SpawnCode"] = "as332", ["BoneName"] = "rope_attach_a", ["OpenDoorWhenWinchCreated"] = true, ["OpenDoorIndex"] = 3},
    {["SpawnCode"] = "devS92", ["BoneName"] = "rope_attach_a", ["OpenDoorWhenWinchCreated"] = false, ["OpenDoorIndex"] = 3},
    {["SpawnCode"] = "aw139", ["BoneName"] = "rope_attach_a", ["OpenDoorWhenWinchCreated"] = true, ["OpenDoorIndex"] = 3},

}

-- Command Configuration

Config.WinchMenuCommand = "winchmenu"
Config.WinchMenuCommandPrompt = "Opens the winch menu."

Config.WinchCommand = "winch"
Config.WinchCommandAcePerms = true
Config.WinchCommandPrompt = "Create a winch."

Config.WinchRemoveCommand = "winchremove"
Config.WinchRemoveCommandAcePerms = true
Config.WinchRemoveCommandPrompt = "Removes the winch."

Config.WinchAttachCommand = "winchattach"
Config.WinchAttachCommandAcePerms = true
Config.WinchAttachCommandPrompt = "Attaches you to the closest winch hook."

Config.WinchDetachCommand = "winchdetach"
Config.WinchDetachCommandAcePerms = true
Config.WinchDetachCommandPrompt = "Detaches you from the winch hook you are currently attached to."
Config.WinchDetachCommandParamPrompt = "true if you wish to detach into the helicopter, else leave blank."

Config.WinchControlCommand = "winchcontrol"
Config.WinchControlCommandAcePerms = true
Config.WinchControlCommandPrompt = "Transfers control of your current winch to the specified player."

Config.CarryCommand = "winchcarry"
Config.CarryCommandAcePerms = true
Config.CarryCommandPrompt = "Carries the closest player ready to winch them."

Config.LitterCommand = "winchlitter"
Config.LitterCommandAcePerms = true
Config.LitterCommandPrompt = "Attaches litter to the winch you are controlling."

Config.LitterPickupCommand = "winchlitterpickup"
Config.LitterPickupCommandAcePerms = true
Config.LitterPickupCommandPrompt = "Carries the nearest player in a litter."


Config.BasketCommand = "winchbasket"
Config.BasketCommandAcePerms = true
Config.BasketCommandPrompt = "Attaches a rescue basket to the winch you are controlling."

Config.WinchCameraCommand = "winchcamera"
Config.WinchCameraCommandPrompt = "Toggles the winch camera."

-- Language Configuration
Config.ExtendHelp = "Extend Winch"
Config.RetractHelp = "Retract Winch"
Config.ControlOfWinchHelp = "You have been given control of a winch."
Config.ControllingWinchHelp = "You are already controlling a winch, you cannot spawn a new one."
Config.WinchRemovedHelp = "Winch removed."
Config.ControlHandedOverHelp = "Control handed over to Server ID:"
Config.HoverEngaged = "Hover Engaged"
Config.HoverDisengaged = "Hover Disengaged"
Config.NotPilot = "You are not the pilot of this helicopter so you cannot create a winch."


Config.VictimInHeli = "to put victim into helicopter."
Config.GetIntoHeli = "to get into helicopter."
Config.DetachFromLitter = "to detach from litter."
Config.LeaveLitter = "to leave litter."
Config.LeaveBasket = "to leave basket."
Config.DetachFromWinch = "to detach from winch."
Config.AttachToWinch = "to attach to winch."
Config.AttachToLitterOr = "to attach to litter or"
Config.GetInLitterOr = "to get in to litter."
Config.GetInLitter = "to get in litter."
Config.AttachToLitter = "to attach to litter."


Config.KeyMappings.ExtendWinch = "Extend current winch"
Config.KeyMappings.RetractWinch = "Retract current winch"
Config.KeyMappings.WinchAttach = "Attach to current winch"
Config.KeyMappings.LitterAttach = "Attach to current winch litter"
Config.KeyMappings.WinchDetach = "Detach from current winch"

Config.WinchHasLitter = "The current winch has a litter on."
Config.WinchHasPlayer = "The current winch has a player on."
-- Menu Language
Config.Menu = {}
Config.Menu.CreateWinch = "~b~Create Winch"
Config.Menu.CreateWinchDesc = "Creates a winch."

Config.Menu.ToggleBasket = "Toggle Basket"
Config.Menu.ToggleBasketDesc = "Toggles basket on your current winch."

Config.Menu.ToggleLitter = "Toggle Litter"
Config.Menu.ToggleLitterDesc = "Toggles litter on your current winch."

Config.Menu.CarryPerson = "Carry Person"
Config.Menu.CarryPersonDesc = "Carries the nearest person."

Config.Menu.LitterPickup = "Litter Pickup"
Config.Menu.LitterPickupDesc = "Pick up nearest person into litter if attached to one."

Config.Menu.RemoveWinch = "~r~Remove Winch"
Config.Menu.RemoveWinchDesc = "Removes current winch."

Config.Menu.ToggleHover = "Toggle Hover"
Config.Menu.ToggleHoverDesc = "Toggles Helicopter Hover."

Config.Menu.GiveControl = "Give Control"
Config.Menu.GiveControlDesc = "Give winch control to the player."


-- Functions
Function = {}

function Function.ShowNotification( text )
    --EDIT BETWEEN THIS LINE
    SetNotificationTextEntry( "STRING" )
    AddTextComponentString( text )
    DrawNotification( false, false )
    --AND THIS LINE to change notification appearance
end
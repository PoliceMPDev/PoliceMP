main = {
    commandName = "decontent",
    acePermissionsEnabled = false,
    tentModel = `prop_decon_tent`,
    usageDistance = 15.0, -- Distance to turn it on/off
    toggleKey = {0, 38}, -- https://docs.fivem.net/docs/game-references/controls/
    cooldown = 3, -- 3 seconds between turning on/off to prevent spam
    defaultHasPermission = true, -- Determines whether someone can toggle on/off a water monitor and adjust pitch by default. See server event to trigger if false.
    spawnOffset = {0.0, 10.0, 0.0}, -- The position to spawn the tent in relation to the player
    showerIntensity = 2.0, -- Recommended, try changing it if you like
}

translations = {
    setup = "setup",
    remove = "remove",
    commandSuggestion = "Setup or remove a decontamination tent",
    commandHelp = "Setup or remove",
    outsideVehicle = "~r~Error~w~: You must not be inside a vehicle",
    tentRemoved = "~g~Success~w~: Decontamination tent removed",
    tentSetup = "~g~Success~w~: Decontamination tent setup",
    noTentFound = "~r~Error~w~: No water monitor found",
    press = "Press",
    toUse = "to toggle the ~b~decontamination showers",
    keyHelp = "INPUT_PICKUP",
    showerToggled = "~g~Success~w~: Decontamination showers toggled",
    commandError = "~r~Error~w~: Use /decontent setup or /decontent remove",
    tentActive = "~r~Error~w~: This decontamination tent is active and cannot be removed",
}
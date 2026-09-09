--[[
    Sonoran Plugins

    Plugin Configuration

    Put all needed configuration in this file.
]]
local config  = {
    enabled = true,
    pluginName = "callcommands", -- name your plugin here
    pluginAuthor = "SonoranCAD", -- author
    configVersion = "2.1",
    -- put your configuration options below
    callTypes = {
        { command = "999", isEmergency = true, suggestionText = "Sends a emergency call to your SonoranCAD", descriptionPrefix = "" },
        { command = "101", isEmergency = false, suggestionText = "Sends a non-emergency call to your SonoranCAD", descriptionPrefix = "(101)" },
    },
    enablePanic = true,
    -- adds an emergency call when panic button is pressed
    addPanicCall = true,

    usePositionForMetadata = false,
}

if config.enabled then
    Config.RegisterPluginConfig(config.pluginName, config)
end

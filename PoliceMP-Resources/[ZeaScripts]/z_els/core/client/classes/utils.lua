--- @script core/client/classes/utils.lua

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@class section: constants

--- @field utils: Stores utility related functions.
utils = utils or {}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@class section: functions

-- @param level The log level (INFO, WARN, ERROR). Defaults to INFO.
---@param level string
---@param message string
local function debug_print(level, message)
    if init.cache['devMode'] ~= 'true' then 
        return 
    end

    local levels = { ["INFO"] = "INFO", ["WARN"] = "WARNING", ["ERROR"] = "ERROR" }
    local levelStr = levels[level] or "DEBUG"

    local year, month, day, hour, minute, second = GetLocalTime()
    local timestamp = string.format("%04d-%02d-%02d %02d:%02d:%02d", year, month, day, hour, minute, second)

    print(string.format("[%s] [%s]: %s", timestamp, levelStr, tostring(message)))
end

utils.debugPrint = debug_print

---@param soundName string
local function press_sound(soundName, status)
    if soundName == 'none' or soundName == '' then 
        return
    end

    if type(soundName) == 'table' then 
        local toUse = not status and 'activated' or 'deactivated'
        soundName = soundName[toUse]
    end
 
    SendNUIMessage({ 
        type = 'play-sound', 
        load = {
            fileName = soundName
        } 
    })
end

utils.pressSound = press_sound

---@param soundName string
local function toggle_controller(status, interface)
    if interface ~= nil then 
        if not interface.allow then return end
    end

    local type = status and 'show-controller' or 'hide-controller'

    SendNUIMessage({ 
        type = type, 
        load = {
            type = interface and interface.type or ''
        } 
    })
end

utils.toggleController = toggle_controller

---@param data table
local function update_controller(data)
    if not data then 
        return
    end

    local types = {
        ['primary_reset'] = data.stage == 1 and 'rgba(255, 100, 100, 0.3)' or '#808080',
        ['at_scene'] = data.stage == 2 and 'rgba(200, 100, 100, 0.3)' or '#808080',
        ['toggle_siren'] = data.siren ~= 0 and 'rgba(0, 255, 0, 0.3)' or '#808080',
        ['rear_reds'] = data.rear_reds and 'rgb(255 0 0 / 38%)' or '#808080',
        ['rear_blues'] = data.rear_blues and 'rgb(0 0 255 / 38%)' or '#808080',
        ['front_whites'] = data.front_whites and 'rgba(100, 100, 100, 0.3)' or '#808080',
        ['front_blues'] = data.front_blues and 'rgb(0 0 255 / 38%)' or '#808080',
        ['alley_lights'] = data.alley_lights and 'rgba(0, 100, 100, 0.3)' or '#808080',
        ['message_board'] = data.message_board ~= 0 and 'rgba(0, 100, 100, 0.3)' or '#808080',
        ['dual_tones'] = data.dual_tones ~= nil and 'rgba(0, 100, 100, 0.3)' or '#808080',
    }
    
    SendNUIMessage({
        type = "update-controller",
        types = types
    })
end

utils.updateController = update_controller

---@param vector vector3
---@return vector3
local function normalize(vector)
    local length = math.sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z)
    if length == 0 then
        return vector3(0.0, 0.0, 0.0)
    end
    return vector3(vector.x / length, vector.y / length, vector.z / length)
end

utils.normalize = normalize

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

--- Shared extension module for Advanced Roleplay Environment.

---Logs a information
---@param ... unknown
function LogInformation(...)
    print("^7[^5Information^7] ", ...)
end

---Logs a warning
---@param ... unknown
function LogWarning(...)
    print("^7[^3Warning^7] ", ...)
end

---Logs a error
function LogError(...)
    print("^7[^1Critical^7] ", ...)
end

---Logs a debug message
function LogDebug(condition, ...)
    if not condition then condition = true end

    local debugModeEnabled = false

    if IsDuplicityVersion() then
        debugModeEnabled = ServerConfig.m_debugModeEnabled
    else
        debugModeEnabled = ClientConfig.m_debugModeEnabled
    end

    if debugModeEnabled and condition then
        print("^7[^6DebugLog^7] ", ...)
    end
end

---Returns green color code when testCondition is true otherwise red
function GetValidColorCode(testCondition)
    if testCondition then return "^2" end
    return "^1"
end

---Dumps table to console
---@param table any
---@param customName any
function DumpTableToJson(table, customName)
    print(((customName or "") .. ":") or "", json.encode(table, { indent = true }))
end

---Returns true if the bodypart is valid
---@param bodyPart any
---@return boolean
function ValidateBodyPart(bodyPart)
    return bodyPart == "HEAD" or bodyPart == "TORSO" or bodyPart == "LEFT_ARM" or bodyPart == "RIGHT_ARM" or
        bodyPart == "LEFT_LEG" or bodyPart == "RIGHT_LEG"
end

---Rounds a value
---@param num any
---@param numDecimalPlaces any
---@return number
function RoundValue(num, numDecimalPlaces)
    local mult = 10 ^ (numDecimalPlaces or 0)
    return math.floor(num * mult + 0.5) / mult
end

---Calculates linear conversion
---@param oldMin any
---@param oldMax any
---@param oldValue any
---@param newMin any
---@param newMax any
---@return number
function LinearConversion(oldMin, oldMax, oldValue, newMin, newMax)
    if oldValue <= 0 then
        oldValue = newMin
    end

    return ((oldValue - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin
end

---Returns true if the specified table contains the element
---@param table any
---@param element any
---@return boolean
function TableContains(table, element)
    if type(table) ~= "table" then return false end

    for _, value in pairs(table) do
        if value == element then
            return true
        end
    end

    return false
end

---Sorts by keys
---@param t any
---@param f any
---@return function
function PairsByKeys(t, f)
    local a = {}
    for n in pairs(t) do table.insert(a, n) end
    table.sort(a, f)
    local i = 0      -- iterator variable
    local iter = function ()   -- iterator function
        i = i + 1
        if a[i] == nil then return nil
        else return a[i], t[a[i]]
        end
    end
    return iter
end

---Returns true if the string starts by start
---@param string any
---@param start any
---@return boolean
function string.starts(string, start)
    return string.sub(string, 1, string.len(start)) == start
end

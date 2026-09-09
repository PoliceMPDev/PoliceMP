--- @script core/client/classes/exports.lua

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@class section: constants

--- @field exports: Stores export functions and data.
exports = exports or {}

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@class section: functions

---@param vehicleHandle int
---@return boolean
---@usage local status = exports['z_els']:isSirenActive(vehicleHandle)
local function is_siren_active(vehicleHandle)
    if not vehicleHandle or not DoesEntityExist(vehicleHandle) then 
        return false
    end

    local dataStore = init.vehiclePool[vehicleHandle]
    
    if not dataStore then return nil end

    if dataStore.siren == 0 then 
        return false
    end
    
    return true
end

exports("isSirenActive", is_siren_active)

---@param vehicleHandle int
---@return boolean
---@usage local status = exports['z_els']:is999ModeActive(vehicleHandle)
local function is_999mode_active(vehicleHandle)
    if not vehicleHandle or not DoesEntityExist(vehicleHandle) then 
        return false
    end

    local dataStore = init.vehiclePool[vehicleHandle]
    
    if not dataStore then return nil end

    if dataStore.stage == 0 then  
        return false
    end
    
    return true
end

exports("is999ModeActive", is_999mode_active)

---@param vehicleHandle int
---@return boolean
---@usage local status = exports['z_els']:areRearRedsActive(vehicleHandle)
local function are_rear_reds_active(vehicleHandle)
    if not vehicleHandle or not DoesEntityExist(vehicleHandle) then 
        return false
    end

    local dataStore = init.vehiclePool[vehicleHandle]
    
    if not dataStore then return nil end

    return dataStore.rear_reds
end

exports("areRearRedsActive", are_rear_reds_active)

---@param vehicleHandle int
---@return boolean
---@usage local status = exports['z_els']:areSceneLightsActive(vehicleHandle)
local function are_scene_lights_active(vehicleHandle)
    if not vehicleHandle or not DoesEntityExist(vehicleHandle) then 
        return false
    end

    local dataStore = init.vehiclePool[vehicleHandle]
    
    if not dataStore then return nil end
    
    return dataStore.alley_lights
end

exports("areSceneLightsActive", are_scene_lights_active)

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
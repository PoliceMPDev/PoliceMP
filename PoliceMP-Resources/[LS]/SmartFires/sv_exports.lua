function CreateFire(coords, size, type, notify)
    local id = createFireId()
    fires[id] = {coords = coords, size = size + 0.0, type = type, active = false, spreadable = false, original = true, originalId = id, initialSize = size, automatic = {created = false, type=""}}
    TriggerClientEvent("Client:updateFireTable", -1, id, fires[id], false, false)
    local count, playerTable = checkJobsForAutomaticFires()
    if usingJobCheck and notify then
        local count, playerTable = checkJobsForAutomaticFires()
        for k, v in pairs(playerTable) do
            TriggerClientEvent("Client:automaticFireAlert", k, id)
        end
    end
    return id
end

exports("CreateFire", CreateFire)

function StopFireById(id)
    if fires[id] ~= nil then
        local entry = fires[id]
        handleSmokeAfterFire(id, entry)
        fires[id] = nil
        TriggerClientEvent("Client:updateFireTable", -1, id, entry, true, false)
    end
end

function IsFireStillActive(id)
    if fires[id] ~= nil then
        return true
    end
end

exports("IsFireStillActive", IsFireStillActive)

exports("StopFireById", StopFireById)

if main.automaticFires.enabled then
    function TriggerAutomaticFire()
        local id = triggerAutoFire()
        return id
    end

    function ToggleAutomaticFires()
        if automaticFiresEnabled then
            automaticFiresEnabled = false
            TriggerClientEvent("Client:automaticFiresToggle", -1, false)
        else
            automaticFiresEnabled = true
            TriggerClientEvent("Client:automaticFiresToggle", -1, false)
        end
    end
    
    exports("ToggleAutomaticFires", ToggleAutomaticFires)
    
    exports("TriggerAutomaticFire", TriggerAutomaticFire)
end

function StopAllFires()
    fires = {}
    TriggerClientEvent("Client:clearAllFires", -1)
end

exports("StopAllFires", StopAllFires)

function CreateSmoke(coords, size, type)
    local id = createFireId()
    local smokeHandle = {coords = coords, size = size, type = type, active = false, initialSize = size}
    smoke[id] = smokeHandle
    TriggerClientEvent("Client:updateSmokeTable", -1, id, smokeHandle, false)
    return id
end

exports("CreateSmoke", CreateSmoke)

function StopSmokeById(id)
    if smoke[id] ~= nil then
        local entry = smoke[id]
        smoke[id] = nil
        TriggerClientEvent("Client:updateSmokeTable", -1, id, entry, true)
    end
end

exports("StopSmokeById", StopSmokeById)

function StopAllSmoke()
    smoke = {}
    TriggerClientEvent("Client:clearAllSmoke", -1)
end

exports("StopAllSmoke", StopAllSmoke)

-- This event allows you to receive data on new automatic fires
RegisterServerEvent("Server:newAutomaticFire")
AddEventHandler("Server:newAutomaticFire", function(id, coords, description, size)
    
end)

function GetAllFires()
    return fires
end

exports("GetAllFires", GetAllFires)

function GetAllSmokes()
    return smoke
end

exports("GetAllSmokes", GetAllSmokes)

if main.automaticFires.main.clockOnSystem.enabled then
    function ClockOnUser(serverId)
        if serverId == nil then return nil end
        if main.logging.enabled then
            normalLog(serverId, translations.clockedOnLog, "")
        end
        clockedOn[serverId] = true
    end
    
    exports("ClockOnUser", ClockOnUser)
    
    function ClockOffUser(serverId)
        if serverId == nil then return nil end
        if main.logging.enabled then
            normalLog(serverId, translations.clockedOffLog, "")
        end
        clockedOn[serverId] = false
    end
    
    exports("ClockOffUser", ClockOffUser)
    
    -- This event allows you to receive data on users clocking on
    RegisterServerEvent("Server:userClockedOn")
    AddEventHandler("Server:userClockedOn", function(id)
        local serverId = id
    end)

    -- This event allows you to receive data on users clocking off
    RegisterServerEvent("Server:userClockedOff")
    AddEventHandler("Server:userClockedOff", function(id)
        local serverId = id
    end)
end

function changeAOP(areaSelected)
    if main.automaticFires.locations[areaSelected] ~= nil then
        areaOfPatrol = areaSelected
    end
end

exports('changeAOP', changeAOP)
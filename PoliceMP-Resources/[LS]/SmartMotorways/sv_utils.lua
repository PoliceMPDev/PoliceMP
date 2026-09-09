webhook = ""
if config.permissions.vRP.enabled then
    Tunnel = module("vrp", "lib/Tunnel")
    Proxy = module("vrp", "lib/Proxy")
    vRP = Proxy.getInterface("vRP")
    vRPclient = Tunnel.getInterface("vRP","vRP")
end
QBCore = nil
if config.permissions.QBCore.enabled then
    QBCore = exports['qb-core']:GetCoreObject()
end
ESX = nil
if config.permissions.ESX.enabled then
    ESX = exports["es_extended"]:getSharedObject()
end
function userHasPermission(source, location)
    local permission = false
    local usingPermissions = false
    -- ESX Permissions
    if location.ESX.enabled then
        local xPlayer = ESX.GetPlayerFromId(source)
        if location.ESX.checkJob.enabled then
            usingPermissions = true
            for k, v in pairs(location.ESX.checkJob.jobs) do
                if xPlayer.job.name == v then
                    permission = true
                end
            end
        end
    end
    -- vRP Permission
    if location.vRP.enabled then
        if location.vRP.checkPermission.enabled then
            usingPermissions = true
            for k, v in pairs(location.vRP.checkPermission.permissions) do
                if vRP.hasPermission({vRP.getUserId({source}),v}) then
                    permission = true
                end
            end
        end
        if location.vRP.checkGroup.enabled then
            usingPermissions = true
            for k, v in pairs(location.vRP.checkGroup.groups) do
                if vRP.hasGroup({vRP.getUserId({source}),v}) then
                    permission = true
                end
            end
        end
    end
    -- QBCore Permission
    if location.QBCore.enabled then
        local player = QBCore.Functions.GetPlayer(source)
        if location.QBCore.checkJob.enabled then
            usingPermissions = true
            for k, v in pairs(location.QBCore.checkJob.jobs) do
                if player.PlayerData.job.name == v then
                    permission = true
                end
            end
        end
        if location.QBCore.checkPermission.enabled then
            usingPermissions = true
            for k, v in pairs(location.QBCore.checkPermission.permissions) do
                if QBCore.Functions.HasPermission(source, v) then
                    permission = true
                end
            end
        end
    end
    if not usingPermissions then
        permission = true
    end
    return permission
end

exports('getSigns', function()
    return locations
end)

RegisterNetEvent("smartmotorways:updateSign", function(signId, newLaneValues, streetName)
    local source = source
    if userHasPermission(source, config.permissions) then
        if not saveSpeeds then
            locations[signId].speeds = newLaneValues
        else
            locations[signId].defaultSpeeds = newLaneValues
        end
        TriggerClientEvent("smartmotorways:syncSignsClient", -1, signId, newLaneValues)
        SaveResourceFile(GetCurrentResourceName(), "locations.json", json.encode(locations, {indent = true}), -1)
        if config.logging.enabled then
            discordLog(source, signId, newLaneValues, streetName)
        end
    end
end)

RegisterNetEvent("SmartMotorways:apiUpdateSign")
AddEventHandler("SmartMotorways:apiUpdateSign", function(signId, speeds)
    if tonumber(source) == nil then
        locations[signId].speeds = speeds
        TriggerClientEvent("smartmotorways:syncSignsClient", -1, signId, speeds)
    end
end)

RegisterNetEvent("SmartMotorways:apiResetSign")
AddEventHandler("SmartMotorways:apiResetSign", function(signId)
    if tonumber(source) == nil then
        locations[signId].speeds = {}
        TriggerClientEvent("smartmotorways:syncSignsClient", -1, signId)
    end
end)
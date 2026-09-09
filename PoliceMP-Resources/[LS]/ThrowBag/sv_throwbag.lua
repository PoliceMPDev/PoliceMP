local allowed = {}

-- Command registered server side below:
RegisterCommand(main.commandName, function(source, args, rawCommand)
    local source = source
    if (source > 0) then

        -- Add your permission check here, send event if they have permission

        local permission = userHasPermission(source, jobCheck)
        if permission then
            TriggerClientEvent("Client:toggleThrowBag", source)
            allowed[source] = true
        end 
    end
end, main.enableAcePermissions)

if jobCheck.QBCore.enabled then
    QBCore = exports["qb-core"]:GetCoreObject()
    if main.itemEnabled then
        CreateThread(function()
            QBCore.Functions.CreateUseableItem(main.itemName, function(src, item)
                if userHasPermission(src, jobCheck) then
                    TriggerEvent("Server:ToggleThrowbag", src)
                end
            end)

        end)
    end
end

if jobCheck.vRP.enabled then
    Proxy = module("vrp", "lib/Proxy")
    vRP = Proxy.getInterface("vRP")
end

if jobCheck.ESX.enabled then
    ESX = nil
    TriggerEvent('esx:getSharedObject', function(obj) ESX = obj end)
    if main.itemEnabled then
        CreateThread(function()
            ESX.RegisterUsableItem(main.itemName, function(playerId)
                if userHasPermission(playerId, jobCheck) then
                    TriggerEvent("Server:ToggleThrowbag", playerId)
                end
            end)
        end)
    end
end

RegisterNetEvent("Server:ToggleThrowbag", function(src)
    if main.itemEnabled then
        if userHasPermission(src, jobCheck) then
            if jobCheck.QBCore.enabled then
                local Player = QBCore.Functions.GetPlayer(src)
                Player.Functions.RemoveItem(main.itemName, 1)
                allowed[src] = true
                TriggerClientEvent("Client:toggleThrowBag", src)
            elseif jobCheck.ESX.enabled then
                local xPlayer = ESX.GetPlayerFromId(src)
                xPlayer.removeInventoryItem(main.itemName, 1)
                allowed[src] = true
                TriggerClientEvent("Client:toggleThrowBag", src)
            elseif jobCheck.vRP.enabled then
                local userID = vRP.getUserId({src})
                if vRP.tryGetInventoryItem({userID, main.itemName, 1, false}) then
                    allowed[src] = true
                    TriggerClientEvent("Client:toggleThrowBag", src)
                end
            end
        end
        
    end
end)

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


RegisterServerEvent('Server:spawnThrowBag')
AddEventHandler('Server:spawnThrowBag', function(coords, net, net2)
    local source = source
    if allowed[source] ~= nil then
        -- This code prevents the server event being triggered by someone without permission
        -- They are added to the "allowed" table temporarily, and removed from it after the rope has spawned in
        TriggerClientEvent("Client:spawnThrowBag", -1, coords, net, net2)
        allowed[source] = nil
    end
end)
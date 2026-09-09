-- Command registered server side below:
-- These setup the foundations for ESX / vRP / QBCore permissions
QBCore = nil
vRP = nil
ESX = nil

if Permissions.ESX.Enabled or Inventory.ESX then
    ESX = exports["es_extended"]:getSharedObject()
end

if Permissions.vRP.Enabled or Inventory.vRP then
    Proxy = module("vrp", "lib/Proxy")
    vRP = Proxy.getInterface("vRP")
end

if Permissions.QBCore.Enabled or Inventory.qbCore then
    QBCore = exports["qb-core"]:GetCoreObject()
end

local itemEnabled = Inventory.quasarInventory or Inventory.qbCore or Inventory.vRP or Inventory.ESX or Inventory.coreInventory or Inventory.oxInventory

local usedShield = {}

if main.commandEnabled then
    RegisterCommand(main.commandName, function(source, args, rawCommand)
        -- Add your permission check here, send event if they have permission
        local shieldType = ""
        if args[1] == nil then
            shieldType = main.defaultShield
        else
            shieldType = string.lower(args[1])
        end 

        if not isShieldTypeValid(shieldType) then
            shieldType = main.defaultShield
        end

        local permission = UserHasPermission(source, Permissions)

        if permission then
            if itemEnabled then
                if usedShield[source] ~= nil then
                    TriggerClientEvent("Client:toggleShield", source)
                else
                    TriggerEvent("Server:ShieldWithItem", source, shieldType)
                end
            else
                TriggerClientEvent("Client:toggleShield", source, shieldType)
            end
        end
    end, Permissions.AcePermissions.Enabled)
end

function isShieldTypeValid(name)
    for k, v in pairs(shields) do
        if v.name ==  name then
            return true
        end
    end
    return false
end

if itemEnabled then
    RegisterNetEvent("Server:ShieldWithItem", function(src, shieldType)
        if HandleInventoryItems(src, string.lower(shieldType), false) then
            usedShield[src] = string.lower(shieldType)
            TriggerClientEvent("Client:toggleShield", src, string.lower(shieldType))
        else
            TriggerClientEvent("shieldNotification", src, translations.noItem)
        end
    end)
end

RegisterNetEvent("Server:ReturnShieldToPlayer", function()
    local source = source
    if usedShield[source] ~= nil and usedShield[source] ~= false then
        print("trying to return "..usedShield[source])
        HandleInventoryItems(source, usedShield[source], true)
        usedShield[source] = nil
    end
end)

if itemEnabled then
    if Inventory.oxInventory then
        exports('usePoliceShield', function(event, item, inventory, slot, data)
            if event == 'usingItem' then
                usedShield[inventory.player.source] = string.lower(item.server.name)
                TriggerClientEvent("Client:toggleShield", inventory.player.source, string.lower(item.server.name))
            end
        end)
    elseif Inventory.qbCore then
        for k, v in pairs(shields) do
            QBCore.Functions.CreateUseableItem(v.name:lower(), function(source, item)
                TriggerEvent("Server:ShieldWithItem", source, v.name)
            end)
        end
    elseif Inventory.ESX then
        for k, v in pairs(shields) do
            ESX.RegisterUsableItem(v.name:lower(), function(playerId, item)
                TriggerEvent("Server:ShieldWithItem", playerId, v.name)
            end)
        end
    elseif Inventory.vRP then
        RegisterNetEvent("Server:useVRPShield", function(source, shieldType)
            TriggerEvent("Server:ShieldWithItem", source, shieldType)
        end)
    end
end

function HandleInventoryItems(source, itemName, give)
    if Inventory.vRP then
        local userID = vRP.getUserId({source})
        if give then
            vRP.giveInventoryItem({userId, itemName, 1, false})
        else
            if vRP.tryGetInventoryItem({userID, itemName, 1, false}) then
                return true
            else
                return false
            end
        end
    elseif Inventory.qbCore then
        if give then
            exports['qb-inventory']:AddItem(source, itemName, 1, false, false, GetCurrentResourceName())
        else
            local successful = exports['qb-inventory']:RemoveItem(source, itemName, 1, false, GetCurrentResourceName())

            return successful
        end
    elseif Inventory.ESX then
        local xPlayer = ESX.GetPlayerFromId(source)
        if give then
            xPlayer.addInventoryItem(itemName, 1)
        else
            local returnedItem = xPlayer.getInventoryItem(itemName)

            if returnedItem.canRemove then
                xPlayer.removeInventoryItem(itemName, 1)
                return true
            else
                return false
            end
        end
    elseif Inventory.oxInventory then
        if give then
            exports.ox_inventory:AddItem(source, itemName, 1)
        else
            local successful = exports.ox_inventory:RemoveItem(source, itemName, 1)

            return successful
        end
    elseif Inventory.quasarInventory then
        if give then
            exports['qs-inventory']:AddItem(source, itemName, 1)
        else
            local quantity = exports['qs-inventory']:GetItemTotalAmount(source, itemName)
            if quantity > 0 then
                exports['qs-inventory']:RemoveItem(source, itemName, 1)

                return true
            else
                return false
            end
        end
    elseif Inventory.coreInventory then
        if give then
            exports.core_Inventory:addItem(source, itemName, 1)
        else
            local successful = exports.core_Inventory:removeItem(source, itemName, 1)

            return successful
        end
    end
end

function UserHasPermission(source, location)

    if not location.EnablePermissions then
        return true
    end

    if location.AcePermissions.Enabled then
        return true
    end

    -- ESX Permissions
    if location.ESX.Enabled then
        local xPlayer = ESX.GetPlayerFromId(source)
        if location.ESX.CheckJob.Enabled then
            for k, v in pairs(location.ESX.CheckJob.Jobs) do
                if xPlayer.job.name == v then
                    return true
                end
            end
        end
    end

    -- vRP Permission
    if location.vRP.Enabled then
        if location.vRP.CheckPermission.Enabled then
            for k, v in pairs(location.vRP.CheckPermission.Permissions) do
                if vRP.hasPermission({vRP.getUserId({source}),v}) then
                    return true
                end
            end
        end

        if location.vRP.CheckGroup.Enabled then
            for k, v in pairs(location.vRP.CheckGroup.Groups) do
                if vRP.hasGroup({vRP.getUserId({source}),v}) then
                    return true
                end
            end
        end
    end

    -- QBCore Permission
    if location.QBCore.Enabled then
        local player = QBCore.Functions.GetPlayer(source)
        if location.QBCore.CheckJob.Enabled then
            for k, v in pairs(location.QBCore.CheckJob.Jobs) do
                if player.PlayerData.job.name == v then
                    return true
                end
            end
        end
        if location.QBCore.CheckPermission.Enabled then
            for k, v in pairs(location.QBCore.CheckPermission.Permissions) do
                if QBCore.Functions.HasPermission(source, v) then
                    return true
                end
            end
        end
    end

    if location.QBX.Enabled then
        local player = exports.qbx_core:GetPlayer(source)
        if location.QBX.CheckJob.Enabled then
            for k, v in pairs(location.QBX.CheckJob.Jobs) do
                if player.PlayerData.job.name == v then
                    return true
                end
            end
        end
    end

    return false
end
Config = {}
Config.Framework = 'Standalone' -- Standalone / ESX / QB / QBX / Custom

--------------------------------------------------------------
--                         Shared                           --
--------------------------------------------------------------

function GetFrameworkCore(pFramework)
    if Framework ~= nil and pFramework ~= 'Standalone' then -- Leave as is.
        return Framework
    end
    if pFramework == 'ESX' then
        return exports["es_extended"]:getSharedObject()
    elseif pFramework == 'QB' or pFramework == 'QBX' then
        return exports['qb-core']:GetCoreObject()
    elseif pFramework == 'Custom' then
        -- Add the code here to return the frameworks core.
    end
    return nil
end

--------------------------------------------------------------
--                         Client                           --
--------------------------------------------------------------

function CustomNotification(pTitle, pMessage)
    lib.notify({
        title = pTitle,
        description = pMessage,
        type = 'inform',
        duration = 5000,
        position = 'top-right',
    })
end

--------------------------------------------------------------
--                         Server                           --
--------------------------------------------------------------

function RegisterUsableItem(pIsUseCommand, pIsUseItem, pSpawnName, pOnSelect)
    if pIsUseCommand then
        RegisterCommand(pSpawnName, pOnSelect)
    end

    if pIsUseItem then
        if Config.Framework == 'ESX' then
            Framework.RegisterUsableItem(pSpawnName, pOnSelect)
        end

        if Config.Framework == 'QB' or Config.Framework == 'QBX' then
            Framework.Functions.CreateUseableItem(pSpawnName, function(pSource, pItem)
                local xPlayer = Framework.Functions.GetPlayer(pSource)
                if not xPlayer.Functions.GetItemByName(pItem.name) then return end
                pOnSelect(pSource)
            end)
        end

        if Config.Framework == 'Custom' then
            -- Add the code here to create a usable item in your custom framework.
        end
    end
end

function GetUserJob(pSource)
    if Config.Framework == 'ESX' then
        local xPlayer = Framework.GetPlayerFromId(pSource)
        return xPlayer?.job?.name
    end

    if Config.Framework == 'QB' then
        local xPlayer = Framework.Functions.GetPlayer(pSource)
        return xPlayer?.PlayerData?.job?.name
    end

    if Config.Framework == 'QBX' then
        local xPlayer = exports.qbx_core:GetPlayer(pSource)
        return xPlayer?.PlayerData?.job?.name
    end

    if Config.Framework == 'Custom' then
        -- If you are using a Custom Framework, add the relevant code here.
    end

    return nil
end

function GetUserDetails(pSource)
    local details = { firstName = nil, lastName = nil, dob = nil } 

    if Config.Framework == 'ESX' then
        local xPlayer = Framework.GetPlayerFromId(pSource)
        if xPlayer == nil then return details end
        details = { firstName = xPlayer.firstName, lastName = xPlayer.lastName, dob = xPlayer.dob }
    end

    if Config.Framework == 'QB' then
        local xPlayer = Framework.Functions.GetPlayer(pSource)
        if xPlayer == nil then return details end
        details = { firstName = xPlayer.firstName, lastName = xPlayer.lastName, dob = xPlayer.dob }
    end

    if Config.Framework == 'QBX' then
        local xPlayer = exports.qbx_core:GetPlayer(pSource)
        if xPlayer == nil then return details end
        details = { firstName = xPlayer.firstName, lastName = xPlayer.lastName, dob = xPlayer.dob }
    end

    if Config.Framework == 'Custom' then
        -- If you are using a Custom Framework, add the relevant code here.
    end

    return details
end

function DoesUserHaveItem(pSource, pItemName)
    if Config.Framework == 'ESX' then
        local xPlayer = Framework.GetPlayerFromId(pSource)
        if not xPlayer then return false end

        local item = xPlayer.getInventoryItem(pItemName)
        return item and item.count and item.count > 0
    end

    if Config.Framework == 'QB' then
        local xPlayer = Framework.Functions.GetPlayer(pSource)
        if not xPlayer then return false end

        local item = xPlayer.Functions.GetItemByName(pItemName)
        return item ~= nil and item.amount and item.amount > 0
    end

    if Config.Framework == 'QBX' then
        local xPlayer = exports.qbx_core:GetPlayer(pSource)
        if not xPlayer then return false end

        local item = xPlayer.Functions.GetItemByName(pItemName)
        return item ~= nil and item.amount and item.amount > 0
    end

    if Config.Framework == 'Custom' then
        -- Add your custom inventory check logic here
        return true
    end

    return false
end

function RemoveItemFromPlayer(pSource, pItemName, pAmount)
    if Config.Framework == 'ESX' then
        local xPlayer = Framework.GetPlayerFromId(pSource)
        if not xPlayer then return end

        local item = xPlayer.getInventoryItem(pItemName)
        if item and item.count and item.count > 0 then
            xPlayer.removeInventoryItem(pItemName, pAmount)
        end
    end

    if Config.Framework == 'QB' then
        local xPlayer = Framework.Functions.GetPlayer(pSource)
        if not xPlayer then return end

        local item = xPlayer.Functions.GetItemByName(pItemName)
        if item ~= nil and item.amount and item.amount > 0 then
            xPlayer.Functions.RemoveItem(pItemName, pAmount)
        end
    end

    if Config.Framework == 'QBX' then
        local xPlayer = exports.qbx_core:GetPlayer(pSource)
        if not xPlayer then return end

        local item = xPlayer.Functions.GetItemByName(pItemName)
        if item ~= nil and item.amount and item.amount > 0 then
            xPlayer.Functions.RemoveItem(pItemName, pAmount)
        end
    end

    if Config.Framework == 'Custom' then
        -- Add your custom inventory remove logic here
    end
end

function GiveItemToPlayer(pSource, pItemName, pAmount)
    if Config.Framework == 'ESX' then
        local xPlayer = Framework.GetPlayerFromId(pSource)
        if not xPlayer then return end

        xPlayer.addInventoryItem(pItemName, pAmount)
    end

    if Config.Framework == 'QB' then
        local xPlayer = Framework.Functions.GetPlayer(pSource)
        if not xPlayer then return end

        xPlayer.Functions.AddItem(pItemName, pAmount)
    end

    if Config.Framework == 'QBX' then
        local xPlayer = exports.qbx_core:GetPlayer(pSource)
        if not xPlayer then return end

        xPlayer.Functions.AddItem(pItemName, pAmount)
    end

    if Config.Framework == 'Custom' then
        -- Add your custom inventory give logic here
    end
end

-- If pAudioIndex is null, it will use pCoordsOrEntity. pAudioIndex usually only be null if its not using coords.
-- Our audio system does not play globally, just the requesting player. You can intergrate your own audio system which supports global / networked audio.
---@param pSource number
---@param pCoordsOrEntity number | vector3
---@param pAudioFile string
---@param pAudioVolume number
---@param pDistance number
---@param pIsLooped boolean
---@param pIsGlobal boolean
---@param pAudioIndex number | string | nil
-- pAudioFile will have the extension which will be .ogg for all our scripts.
-- pAudioVolume is a number between 0.0 and 1.0. 0.0 is silent, 1.0 is max volume.
function PlayAudio(pSource, pCoordsOrEntity, pAudioFile, pAudioVolume, pDistance, pIsLooped, pIsGlobal, pAudioIndex)
    if pAudioIndex == nil then -- If pAudioIndex is null, it will use pCoordsOrEntity as the index / soundId.
        pAudioIndex = pCoordsOrEntity
    end
    -- This if statement has been implemented to demo the use of determining if its playing from an entity or coords.
    -- This is to help if you are using a custom audio resource which has different exports for entity and coords.
    if type(pCoordsOrEntity) == "number" then -- Entity
        TriggerEvent('OG_Lib:StartAudio', pSource, pCoordsOrEntity, pAudioFile, pAudioVolume, pIsLooped, pAudioIndex)
    else -- Coords
        TriggerEvent('OG_Lib:StartAudio', pSource, pCoordsOrEntity, pAudioFile, pAudioVolume, pIsLooped, pAudioIndex)
    end
end

---@param pSource number
---@param pAudioIndex number | string
---@param pIsGlobal boolean
function StopAudio(pSource, pAudioIndex, pIsGlobal)
    TriggerEvent('OG_Lib:PauseAudio', pSource, pAudioIndex)
end

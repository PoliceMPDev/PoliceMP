function GetPlayerIdentifier(pSource) -- If you don't know what your doing, don't touch this. This is to get the Players Identifier / Name displayed on the Radio.
    local playerIdentifier = nil -- Defining variable
    if pSource == nil then return nil end

    if Config.Framework == 'Standalone' then -- STANDALONE USERS
        if Config.FMS then -- FMS USERS 

            exports["fms"]:getPlayerFmsData(pSource, function(pPlayerData)
                if pPlayerData == nil then return Debug('Player Data null for FMS Get Player Identifier') end 
                playerIdentifier = pPlayerData.forcenumber
            end)

        elseif Config.CMA then -- CMA USERS
        
            -- Documentation not provided, ask Staff at OM Solutions.

        else -- NORMAL STANDALONE USERS

            playerIdentifier = GetPlayerName(pSource)

        end    
    elseif Config.Framework == 'ESX' or Config.Framework == 'ESX Legacy' then -- ESX USERS

        local xPlayer = ESX.GetPlayerFromId(pSource)
        if xPlayer == nil then 
            Debug('User (' .. pSource .. ') has a nil xPlayer, setting as standard game name.')
            playerIdentifier = GetPlayerName(pSource)
        else
            playerIdentifier = xPlayer.getName()
            Debug('Player (' .. pSource .. ') ESX Identifier -> ' .. tostring(playerIdentifier))
        end

    elseif Config.Framework == 'QB' then -- QB CORE USERS
        
        Debug(pSource)

        local xPlayer = QBCore.Functions.GetPlayer(pSource)
        if xPlayer == nil then 
            Debug('User (' .. pSource .. ') has a nil xPlayer, setting as standard game name.')
            playerIdentifier = GetPlayerName(pSource)
        else
            playerIdentifier = xPlayer.PlayerData.name
            Debug('Player (' .. pSource .. ') QB Identifier -> ' .. tostring(playerIdentifier))
        end

    end

    Debug('Player (' .. pSource .. ') Identifier -> ' .. tostring(playerIdentifier))

    return playerIdentifier -- Returning variable
end




function IsPlayerOnDuty(pSource) -- If you don't know what your doing, don't touch this. This is to get the Players Identifier / Name displayed on the Radio.
    local isPlayerOnDuty = false

    if Config.Framework == 'Standalone' then -- STANDALONE USERS

        if Config.FMS then -- FMS USERS 

            if Config.CanCivilianAccessRadio then isPlayerOnDuty = true end

            exports["fms"]:getPlayerFmsData(pSource, function(pPlayerData)
                if pPlayerData == nil then return Debug('Player Data null for FMS Get Player Identifier') end 
                if pPlayerData.currentPatrolBranch == Config.CivilianPatrolBranchName then return end 
                isPlayerOnDuty = true  
            end)

        elseif Config.CMA then -- CMA USERS
            
            if Config.CanCivilianAccessRadio then isPlayerOnDuty = true end

            -- Documentation not provided, ask Staff at OM Solutions.

        else -- NORMAL STANDALONE USERS

            isPlayerOnDuty = true

        end    

    elseif Config.Framework == 'ESX' or Config.Framework == 'ESX Legacy' then -- ESX USERS

        if not Config.RequireJob then isPlayerOnDuty = true end
        local xPlayer = ESX.GetPlayerFromId(pSource)
        if xPlayer == nil then 
            Debug('User (' .. pSource .. ') has a nil xPlayer')
        else
            Debug('Player Job -> ' .. xPlayer.job.name)
            if DoesTableContainItem(Config.AllowedJobs, xPlayer.job.name) then isPlayerOnDuty = true end -- If there job is in the Allowed Jobs in Config.
        end
    elseif Config.Framework == 'QB' then -- QB CORE USERS

        if not Config.RequireJob then isPlayerOnDuty = true end
        local xPlayer = QBCore.Functions.GetPlayer(pSource)
        if xPlayer == nil then 
            Debug('User (' .. pSource .. ') has a nil xPlayer')
        else
            Debug('Player Job -> ' .. xPlayer.PlayerData.job.name)
            if DoesTableContainItem(Config.AllowedJobs, xPlayer.PlayerData.job.name) then isPlayerOnDuty = true end -- If there job is in the Allowed Jobs in Config.
        end
    end

    Debug('Returning Is Player (' .. pSource .. ') On Duty as -> ' .. tostring(isPlayerOnDuty))

    return isPlayerOnDuty -- Returning value.
end




function PanicButton(pSource) -- This will do a job check before hand for ESX / QB users and a Discord Badger API Check for all Frameworks. 
    for _, playerId in ipairs(GetPlayers()) do  

        if not IsPlayerOnDuty(playerId) then goto skip end
        if not CheckBadgerPermissions(playerId) then return end -- If you have BadgerPermissions default to off (false), this will always be true.

        if Config.DisplayPanicChatMessage then 
            TriggerClientEvent('chatMessage', playerId, GetPlayerIdentifier(pSource), {255, 0, 0}, ' has activated there Panic Button.')
        end
        if Config.PlayPanicSound then
            TriggerClientEvent('OGRadio:PlayAudio', playerId, 'Panic')
        end

        ::skip::
    end
end
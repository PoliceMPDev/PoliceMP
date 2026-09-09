/*--------------------------------------
  % Made with ❤️ for: Rytrak Store
  % Author: Rytrak https://rytrak.fr
  % Script documentation: https://docs.rytrak.fr/scripts/advanced-extrication-system
  % Full support on discord: https://discord.gg/k22buEjnpZ
--------------------------------------*/

-- [[ Compatibility init ]]

local jobs = {}
GlobalState.counterJobs = {}

local ESX, QBCore = nil, nil

if Config.LockPeopleInsideCar.ESX then
    ESX = exports["es_extended"]:getSharedObject()
end

if Config.LockPeopleInsideCar.QBCore then
    QBCore = exports['qb-core']:GetCoreObject()
end

Citizen.CreateThread(function()
    while true do
        if Config.LockPeopleInsideCar.ESX then
            local xPlayers = ESX.GetPlayers()
    
            for k,v in pairs(Config.LockPeopleInsideCar.minJobsRequired) do
                jobs[k] = 0
            end
            
            for i=1, #xPlayers, 1 do
                local xPlayer = ESX.GetPlayerFromId(xPlayers[i])
                
                if xPlayer then
                    if Config.LockPeopleInsideCar.minJobsRequired[xPlayer.job.name] then
                        if jobs[xPlayer.job.name] ~= nil then
                            jobs[xPlayer.job.name] = jobs[xPlayer.job.name] + 1

                            GlobalState.counterJobs = jobs
                        end
                    end
                end
            end
        elseif Config.LockPeopleInsideCar.QBCore then
            local xPlayers = QBCore.Functions.GetPlayers()

            for k,v in pairs(Config.LockPeopleInsideCar.minJobsRequired) do
                jobs[k] = 0
            end

            for k,v in pairs(QBCore.Functions.GetPlayers()) do
                local xPlayer = QBCore.Functions.GetPlayer(v)

                if xPlayer then
                    if Config.LockPeopleInsideCar.minJobsRequired[xPlayer.PlayerData.job.name] then
                        if jobs[xPlayer.PlayerData.job.name] ~= nil then
                            jobs[xPlayer.PlayerData.job.name] = jobs[xPlayer.PlayerData.job.name] + 1

                            GlobalState.counterJobs = jobs
                        end
                    end
                end
            end
        end

        Wait(1000)
    end
end)
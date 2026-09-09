/*--------------------------------------
  % Made with ❤️ for: Rytrak Store
  % Author: Rytrak https://rytrak.fr
  % Script documentation: https://docs.rytrak.fr/scripts/firefighter-scba-system
  % Full support on discord: https://discord.gg/k22buEjnpZ
--------------------------------------*/

-- [[ Compatibility init ]]

local ESX = nil

if Config.ESX.enabled then
    ESX = exports["es_extended"]:getSharedObject()
elseif Config.QB.enabled then
    QBdata = {}

    RegisterNetEvent('QBCore:Client:OnPlayerLoaded', function()
        QBdata = exports['qb-core']:GetCoreObject().Functions.GetPlayerData()
    end)

    RegisterNetEvent('QBCore:Client:OnJobUpdate', function(JobInfo)
        QBdata.job = JobInfo
    end)
end

-- [[ Functions ]]

-- Compare the player's job with the one in the script configuration (only if ESX or QB is enabled)
function verifyJobPlayer()
    if Config.ESX.enabled then
        local ESXdata = ESX.GetPlayerData()
        for i = 1, #Config.ESX.jobs do
            if ESXdata.job ~= nil then
                if ESXdata.job.name == Config.ESX.jobs[i] then
                    return true
                end
            end
        end
    elseif Config.QB.enabled then
        for i = 1, #Config.QB.jobs do
            if QBdata.job and QBdata.job.name == Config.QB.jobs[i] then
                return true
            end
        end
    else
        return true
    end

    return false
end

-- You can modify the notification system of the script (do not change the name of the function)
function Hint(message)
    if Config.NUI.VisibleHint then
        AddTextEntry('r_scba', message)
        BeginTextCommandDisplayHelp('r_scba')
        EndTextCommandDisplayHelp(0, 0, Config.NUI.HintSound, -1)
    end
end

-- You can modify the damage inflicted to the player when he has no more air in his bottle
function killPed(player)
    TriggerEvent("addinjury", {"smoke_inhalation", "HEAD", "minor"})
    TriggerEvent("addinjury", {"choking", "HEAD", "medium"})
    TriggerEvent("addinjury", {"seizure", "HEAD", "minor"})
end

-- You can modify the damage when you are near a fire here.
function damageNearFire(player)
    ApplyDamageToPed(player, Config.FireDamage.DamageNearFire)
end

-- [[ Command ]]

-- SCBA command (when CommandEnabled is activated)
if Config.CommandEnabled then
    RegisterCommand(Config.Command, function(_, Args)
        if verifyJobPlayer() then
            fCommand() -- You can use this function to use it elsewhere. (in a context menu for example)
        end
    end)
end
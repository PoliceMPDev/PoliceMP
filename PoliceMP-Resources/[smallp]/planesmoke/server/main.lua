local PlayerSmokeSettings = {}

RegisterServerEvent('pmp:PlaneSmokeSettingsUpdate')
AddEventHandler('pmp:PlaneSmokeSettingsUpdate', function(Data)
    local src = source
    PlayerSmokeSettings[src] = Data
    TriggerClientEvent('pmp:PlaneSmokeSettingsUpdate', -1, PlayerSmokeSettings)
end)

AddEventHandler('playerDropped', function(reason)
    local src = tonumber(source)
    PlayerSmokeSettings[src] = nil
    TriggerClientEvent('pmp:PlaneSmokeSettingsUpdate', -1, PlayerSmokeSettings)
end)

local Permissions = {
    TierTwo = "group.TierTwo"
}

RegisterNetEvent("pmp:CheckPlaneSmokePermission")
AddEventHandler("pmp:CheckPlaneSmokePermission", function()
    local player = source
    local hasPermission = IsPlayerAceAllowed(player, Permissions.TierTwo)
    TriggerClientEvent("pmp:ReceivePlaneSmokePermission", player, hasPermission)
end)

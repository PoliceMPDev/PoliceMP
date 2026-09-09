local isDev = false

Citizen.CreateThread(function()
    TriggerServerEvent("aimcontrol:checkPermission")
end)

RegisterNetEvent("aimcontrol:receivePermission")
AddEventHandler("aimcontrol:receivePermission", function(hasPermission)
    isDev = hasPermission
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        local ped = PlayerPedId()
        if IsPlayerFreeAiming(PlayerId()) and not isDev then
            DisableControlAction(0, 22, true)
        end

        if IsPedReloading(ped) then
            DisableControlAction(0, 22, true)
        end
    end
end)

local zip = false
local HandCuff = nil
local zipback = false
local zipfront = false

RegisterNetEvent('ZIPTIE:PLAYANIM')
AddEventHandler('ZIPTIE:PLAYANIM', function()
    local ped = PlayerPedId()
    RequestAnimDict("mp_arresting")
    FreezeEntityPosition(ped, true)
    while not HasAnimDictLoaded('mp_arresting') do Citizen.Wait(100) end
    TaskPlayAnim(ped, 'mp_arresting', 'a_uncuff', 8.0, -8, 3000, 49, 0, 0, 0, 0)
    Citizen.SetTimeout(3000, function() FreezeEntityPosition(ped, false) end)
end)

RegisterNetEvent('ZIPTIE:PLAYSOUNDD')
AddEventHandler('ZIPTIE:PLAYSOUNDD', function(soundFile, soundVolume)
    SendNUIMessage({transactionType = 'playSound', transactionFile = soundFile, transactionVolume = soundVolume})
end)

RegisterNetEvent('ZIPTIE:BACKZIP')
AddEventHandler('ZIPTIE:BACKZIP', function()
    local player = Closetplayer()
    if not player then return ShowNotification("~y~SYSTEM~s~: There are no players around you!") end
    TriggerServerEvent("ZIPTIE:SZIPBACK", player)
end)

RegisterNetEvent('ZIPTIE:UNZIPUSER')
AddEventHandler('ZIPTIE:UNZIPUSER', function()
    local player = Closetplayer()
    if not player then return ShowNotification("~y~SYSTEM~s~: There are no players around you!") end
    TriggerServerEvent("ZIPTIE:FREEDAUSER", player)
end)

RegisterNetEvent('ZIPTIE:CZIPBACK')
AddEventHandler('ZIPTIE:CZIPBACK', function()
    local ped = PlayerPedId()
    if not zip then
        local coords = GetEntityCoords(ped)
        RequestAnimDict("mp_arresting")
        while not HasAnimDictLoaded("mp_arresting") do Citizen.Wait(100) end
        FreezeEntityPosition(ped, true)
        TaskPlayAnim(ped, "mp_arresting", "idle", 8.0, -8, -1, 49, 0, 0, 0, 0)

        if HandCuff then DetachEntity(HandCuff, true, true); DeleteEntity(HandCuff) end

        SetEnableHandcuffs(ped, true)
        DisablePlayerFiring(ped, true)
        SetCurrentPedWeapon(ped, GetHashKey('WEAPON_UNARMED'), true)
        SetPedCanPlayGestureAnims(ped, false)
        DisplayRadar(false)

        HandCuff = CreateObject(GetHashKey("hei_prop_zip_tie_positioned"), coords.x, coords.y, coords.z, true, true, true)
        AttachEntityToEntity(HandCuff, ped, GetPedBoneIndex(ped, 60309), -0.020, 0.035, 0.06, 0.04, 155.0, 80.0, true, false, false, false, 0, true)

        zip, zipback, zipfront = true, true, false
        Citizen.SetTimeout(3000, function() FreezeEntityPosition(ped, false) end)
    end
end)

RegisterNetEvent('ZIPTIE:BEFREEWEIRDO')
AddEventHandler('ZIPTIE:BEFREEWEIRDO', function()
    local ped = PlayerPedId()
    if HandCuff then DetachEntity(HandCuff, true, true); DeleteEntity(HandCuff) end
    zip, zipback, zipfront = false, false, false
    ClearPedSecondaryTask(ped)
    SetEnableHandcuffs(ped, false)
    DisablePlayerFiring(ped, false)
    DisplayRadar(true)
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        local ped = PlayerPedId()
        if zipback then
            DisableControlAction(0, 24, true); DisableControlAction(0, 257, true)
            DisableControlAction(0, 25, true); DisableControlAction(0, 263, true)
            if not IsEntityPlayingAnim(ped, 'mp_arresting', 'idle', 3) then
                RequestAnimDict('mp_arresting')
                TaskPlayAnim(ped, 'mp_arresting', 'idle', 8.0, -8, -1, 49, 0, false, false, false)
            end
        else
            Citizen.Wait(500)
        end
    end
end)

RegisterNetEvent('ZIPTIE:FRONTZIP')
AddEventHandler('ZIPTIE:FRONTZIP', function()
    local player = Closetplayer()
    if not player then return ShowNotification("~y~SYSTEM~s~: There are no players around you!") end
    TriggerServerEvent("ZIPTIE:SZIPFRONT", player)
end)

RegisterNetEvent('ZIPTIE:CZIPFRONT')
AddEventHandler('ZIPTIE:CZIPFRONT', function()
    local ped = PlayerPedId()
    if not zip then
        RequestAnimDict("anim@move_m@prisoner_cuffed")
        while not HasAnimDictLoaded("anim@move_m@prisoner_cuffed") do Citizen.Wait(100) end
        FreezeEntityPosition(ped, true)
        TaskPlayAnim(ped, "anim@move_m@prisoner_cuffed", "idle", 8.0, -8.0, -1, 49, 0, false, false, false)

        SetEnableHandcuffs(ped, true)
        DisablePlayerFiring(ped, true)
        SetCurrentPedWeapon(ped, GetHashKey('WEAPON_UNARMED'), true)
        SetPedCanPlayGestureAnims(ped, false)
        DisplayRadar(false)

        -- ✅ Create and attach the new front cuff prop
        local coords = GetEntityCoords(ped)
        if HandCuff then DetachEntity(HandCuff, true, true); DeleteEntity(HandCuff) end

        HandCuff = CreateObject(GetHashKey("w_me_speedcuffs"), coords.x, coords.y, coords.z, true, true, true)
        AttachEntityToEntity(HandCuff, ped, GetPedBoneIndex(ped, 60309), -0.080, 0.000, 0.070, -12.0, -5.0, 88.0, true, false, false, false, 0, true)

        zip, zipfront, zipback = true, true, false
        Citizen.SetTimeout(3000, function() FreezeEntityPosition(ped, false) end)
    end
end)


Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        local ped = PlayerPedId()
        if zipfront then
            DisableControlAction(0, 24, true); DisableControlAction(0, 257, true)
            DisableControlAction(0, 25, true); DisableControlAction(0, 263, true)
            if not IsEntityPlayingAnim(ped, "anim@move_m@prisoner_cuffed", "idle", 3) then
                RequestAnimDict("anim@move_m@prisoner_cuffed")
                TaskPlayAnim(ped, "anim@move_m@prisoner_cuffed", "idle", 8.0, -8.0, -1, 49, 0, false, false, false)
            end
        else
            Citizen.Wait(500)
        end
    end
end)

function ShowNotification(msg)
    SetNotificationTextEntry("STRING")
    AddTextComponentSubstringPlayerName(msg)
    DrawNotification(false, false)
end

RegisterNetEvent("ZIPTIE:NOTIFY")
AddEventHandler("ZIPTIE:NOTIFY", function(msg)
    ShowNotification(msg)
end)

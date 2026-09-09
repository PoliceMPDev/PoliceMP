local display = false
local t = {}

RegisterCommand("notes", function(source, args)
    PlaySound(source, "CANCEL", "HUD_MINI_GAME_SOUNDSET", 0, 0, 1)
    ExecuteCommand('e notepad')
    if args[1] == "open" then
        SetGui(true)
    elseif args[1] == "close" then 
        SetGui(false)
    else 
        SetGui(not display)
    end
end)
TriggerEvent('chat:addSuggestion', '/note', 'Write your notes', {
    {name="toggle", help="Either 'open' or 'close', or just toggle with no args."}
})

RegisterNUICallback('exit', function(data)
    updateNotes(t)
    SetGui(false)
    StopNotepadAndPencilAnimation()
end)

RegisterNUICallback('error', function(data)
    updateNotes(t)
    SetGui(false)
    notify("~r~Error:~s~\n"..data.error)
    StopNotepadAndPencilAnimation()
end)

RegisterNUICallback('save', function(data)
    SetGui(false)
    table.insert(t, data.main)
    notify("Saved Note ~h~#"..table.length(t))
    updateNotes(t)
    StopNotepadAndPencilAnimation()
end)

RegisterNUICallback('clear', function(data)
    SetGui(false)
    notify("Cleared ~h~"..table.length(t).."~s~ notes")
    t = {}
    updateNotes(t)
    StopNotepadAndPencilAnimation()
end)

Citizen.CreateThread(function()
    while display do
        Citizen.Wait(0)
            DisableControlAction(0, 1, display) -- LookLeftRight
            DisableControlAction(0, 2, display) -- LookUpDown
            DisableControlAction(0, 142, display) -- MeleeAttackAlternate
            DisableControlAction(0, 18, display) -- Enter
            DisableControlAction(0, 322, display) -- ESC
            DisableControlAction(0, 106, display) -- VehicleMouseControlOverride
    end
end)

function StopNotepadAndPencilAnimation()
    local playerPed = PlayerPedId()
    ClearPedTasks(playerPed)

    local notepadHash = GetHashKey("prop_notepad_01")
    local pencilHash = GetHashKey("prop_pencil_01")

    local notepad = GetClosestObjectOfType(GetEntityCoords(playerPed), 1.0, notepadHash, false, false, false)
    local pencil = GetClosestObjectOfType(GetEntityCoords(playerPed), 1.0, pencilHash, false, false, false)

    if DoesEntityExist(notepad) then
        DetachEntity(notepad, true, true)
        SetEntityAsMissionEntity(notepad, true, true)
        TriggerServerEvent('deleteProps', notepad, notepadHash)
    end

    if DoesEntityExist(pencil) then
        DetachEntity(pencil, true, true)
        SetEntityAsMissionEntity(pencil, true, true)
        TriggerServerEvent('deleteProps', pencil, pencilHash)
    end
end

RegisterNetEvent('syncDeleteProps')
AddEventHandler('syncDeleteProps', function(entityId, propHash)
    if DoesEntityExist(entityId) then
        DeleteObject(entityId)
    end
end)

function dump(o)
    if type(o) == 'table' then
       local s = '{ '
       for k,v in pairs(o) do
          if type(k) ~= 'number' then k = '"'..k..'"' end
          s = s .. '['..k..'] = ' .. dump(v) .. ','
       end
       return s .. '} '
    else
       return tostring(o)
    end
 end


 
function SetGui(enable)
    SetNuiFocus(enable, enable)
    display = enable

    SendNUIMessage({
        type = "ui",
        enable = enable,
        data = t
    })
end

function table.length(tbl)
    local cnt = 0
    for _ in pairs(tbl) do cnt = cnt + 1 end
    return cnt
  end

function notify(string)
    SetNotificationTextEntry("STRING")
    AddTextComponentString("~b~~h~"..GetCurrentResourceName()..":~s~~n~"..string)
    DrawNotification(true, false)
    DrawNotificationWithIcon(1,1,"asd")
end

function updateNotes(tbl)
    SendNUIMessage({
        type = "ui",
        data = json.encode(tbl)
    })
end
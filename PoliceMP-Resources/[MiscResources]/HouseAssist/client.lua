RegisterCommand('hpos', function(...)
      ShowNotification("Copied.")
      local plyPos = GetEntityCoords(GetPlayerPed(-1))
      SendNUIMessage({
        type = "Copy",
        txt  = ""..plyPos.x.."\t"..plyPos.y.."\t"..plyPos.z..""
      })
end)

RegisterNetEvent('PMPCopyToClipboard') -- Define the event
AddEventHandler('PMPCopyToClipboard', function(outputString)
    ShowNotification("Copied.")
    SendNUIMessage({
        type = "Copy",
        txt  = outputString
    })
end)

HelpNotification = function(msg)
  AddTextEntry('help_notification', msg)
  BeginTextCommandDisplayHelp('help_notification')
  EndTextCommandDisplayHelp(0, false, true, -1)
end

ShowNotification = function(msg)
  AddTextEntry('show_notification', msg)
  SetNotificationTextEntry('show_notification')
  DrawNotification(false, true)
end

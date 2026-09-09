RegisterNetEvent('esx:setJob')
AddEventHandler('esx:setJob', function(job)
  TriggerServerEvent("Winch:CheckAccess")
end)

RegisterNetEvent('esx:setJob2')
AddEventHandler('esx:setJob2', function(job)
  TriggerServerEvent("Winch:CheckAccess")
end)

RegisterNetEvent("Winch:CheckAccess")
AddEventHandler("Winch:CheckAccess", function(response)
  Allowed = response
end)

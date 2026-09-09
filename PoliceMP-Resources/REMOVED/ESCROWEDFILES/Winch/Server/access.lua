-- If not using ESX, you can remove the following line
ESX = nil ; TriggerEvent('esx:getSharedObject', function(obj) ESX = obj end)

RegisterServerEvent('Winch:CheckAccess')
AddEventHandler('Winch:CheckAccess', function()
    local _source = source

    -- Check ACE Permissions
    if IsPlayerAceAllowed(_source, "heto.Trained") then
        TriggerClientEvent('Winch:CheckAccess', _source, true)
    else
        -- Deny access if ACE permission is not granted
        TriggerClientEvent('Winch:CheckAccess', _source, false)
    end
end)


--[[ Check which camera the player is allowed to use
RegisterServerEvent('Winch:CheckAccess')
AddEventHandler('Winch:CheckAccess', function()
  local _source = source

  local Access = false

  if Config.WhiteList then
    for k, id in pairs(Config.WhiteList) do

      -- By Identifier
      for k2, id2 in pairs(GetPlayerIdentifiers(_source)) do
        if id == id2 then
          TriggerClientEvent('Winch:CheckAccess', _source, true)
          return
        end
      end

      -- By job with ESX
      if ESX ~= nil then
        while ESX.GetPlayerFromId(_source) == nil or ESX.GetPlayerFromId(_source).job == nil do
          Citizen.Wait(100)
        end
      end
      if ESX and id == ESX.GetPlayerFromId(_source).job.name then
        TriggerClientEvent('Winch:CheckAccess', _source, true)
        return
      end

      -- By a custom method
      -- .......
    end

    -- Response to the client
    TriggerClientEvent('Winch:CheckAccess', _source, false)
  else

    -- If the Config.Whitelist was deleted, everybody has access
    TriggerClientEvent('Winch:CheckAccess', _source, true)
  end
end) ]]--


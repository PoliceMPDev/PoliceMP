TriggerEvent('chat:addSuggestion', '/'..main.commandName, translations.commandSuggestion, {
    { name=translations.setup.."/"..translations.remove, help=translations.help },
})

local hoseEnabled = false


AddEventHandler('Client:HoseCommandEnabled', function(enabled)
    hoseEnabled = true
end)

AddEventHandler('Client:HoseCommandDisabled', function(enabled)
    hoseEnabled = false
end)

function displayHelpText(message)
    SetTextComponentFormat('STRING')
    AddTextComponentString(message)
    DisplayHelpTextFromStringLabel(0, 0, 1, -1)
end

function showNotification(message)
    message = message.."."
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(0,1)
end

function raycast()
    local ped = PlayerPedId()
    local location = GetEntityCoords(ped)
    local offSet = GetOffsetFromEntityInWorldCoords(ped, 0.0, 10.0, 0.0)
    local shapeTest = StartShapeTestCapsule(location.x, location.y, location.z, offSet.x, offSet.y, offSet.z, 10.0, 2, ped, 0)
    local retval, hit, endCoords, surfaceNormal, entityHit = GetShapeTestResult(shapeTest)
    return entityHit
end

Citizen.CreateThread(function()
    while true do
        if hoseEnabled then
            TriggerEvent("processSupplyLine")
        end
        Wait(1000)
    end
end)

RegisterCommand("+zcelltrace", function()
    TriggerServerEvent("celltrace:requestPlayerList")
end)

RegisterNetEvent("celltrace:showPlayerList", function(players)
    if #players == 0 then
        lib.notify({
            title = "Cell Site Ping",
            description = "No players available",
            type = "error",
            position = 'bottom'
        })
        return
    end

    local input = lib.inputDialog("Cell Site Ping", {
        {
            type = 'select',
            label = 'Select Target',
            options = players,
            required = true
        }
    })

    if not input then return end

    local targetServerId = input[1]
    TriggerServerEvent("celltrace:initiateTrace", targetServerId)
end)

RegisterNetEvent("celltrace:requestConfirmation", function()
    local response = lib.alertDialog({
        header = "Incoming Ping Request",
        content = "Someone is attempting to trace your phone. Allow it?",
        centered = true,
        cancel = true,
        labels = {
            cancel = "Phone is Off / No Signal",
            confirm = "Phone is Active - Trackable"
        }
    })

    TriggerServerEvent("celltrace:confirmationResult", response == "confirm")
end)

RegisterNetEvent("celltrace:showBlips", function(blips)
    for _, coords in pairs(blips) do
        local blip = AddBlipForCoord(coords.x, coords.y, coords.z)
        SetBlipSprite(blip, 161)
        SetBlipScale(blip, 1.0)
        SetBlipColour(blip, 1)
        SetBlipFlashes(blip, true)
        SetBlipAsShortRange(blip, false)
        BeginTextCommandSetBlipName("STRING")
        AddTextComponentString("[Cell Site Ping]")
        EndTextCommandSetBlipName(blip)

        Citizen.CreateThread(function()
            Wait(60000)
            RemoveBlip(blip)
        end)
    end
end)

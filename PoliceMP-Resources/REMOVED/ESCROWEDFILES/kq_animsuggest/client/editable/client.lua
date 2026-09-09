local BLOCKED = false

RegisterNetEvent('kq_animsuggest:client:toggleSuggestions')
AddEventHandler('kq_animsuggest:client:toggleSuggestions', function(allow)
    BLOCKED = not allow
end)

function CanUseSuggestions()
    return (not BLOCKED and not IsPlayerUnreachable())
end

--- Main animation triggering functions
function PlayAnim(ped, dict, anim, flag, duration)
    Citizen.CreateThread(function()
        RequestAnimDict(dict)
        local timeout = 0
        while not HasAnimDictLoaded(dict) do
            Citizen.Wait(50)
            timeout = timeout + 1
            if timeout > 100 then
                return
            end
        end
        
        TaskPlayAnim(ped or PlayerPedId(), dict, anim, 1.5, 1.0, duration or -1, flag or 1, 0, false, false, false)
        RemoveAnimDict(dict)
    end)
end

function PlayAnimAdvanced(ped, dict, anim, flag, coords, rotation)
    Citizen.CreateThread(function()
        RequestAnimDict(dict)
        local timeout = 0
        while not HasAnimDictLoaded(dict) do
            Citizen.Wait(50)
            timeout = timeout + 1
            if timeout > 100 then
                return
            end
        end
        
        TaskPlayAnimAdvanced(ped or PlayerPedId(), dict, anim,
                coords.x, coords.y, coords.z,
                rotation.x, rotation.y, rotation.z,
                1.5, 1.0, -1, flag or 1, 0.0, false, false)
        
        RemoveAnimDict(dict)
    end)
end
---

function IsPlayerUnreachable()
    local playerPed = PlayerPedId()
    return IsPedInAnyVehicle(playerPed) or IsPedRagdoll(playerPed) or IsEntityDead(playerPed)
end


function CreateProp(toolData)
    local playerPed = PlayerPedId()
    local toolModel = toolData.model
    local coords = GetEntityCoords(playerPed)
    local boneIndex = GetPedBoneIndex(playerPed, toolData.bone or 28422)

    DoRequestModel(toolModel)
    local object = CreateObject(toolModel, coords, true, true, true)
    AttachEntityToEntity(object, playerPed, boneIndex, toolData.pos.x, toolData.pos.y, toolData.pos.z, toolData.rot.x, toolData.rot.y, toolData.rot.z, true, true, false, true, 1, true)

    return object
end

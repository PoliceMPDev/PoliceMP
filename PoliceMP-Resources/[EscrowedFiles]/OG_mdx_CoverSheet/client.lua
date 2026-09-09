local coverSheetEntity = -1

function ToggleCoverSheet()

    local PlayerPed = GetPlayerPed(-1)
    local PlayerCoords = GetEntityCoords(PlayerPed)

    if coverSheetEntity == -1 then

        LoadAnimation("mdx@holdtarp", true)
        LoadModel("mdx_tarp", true)

        TaskPlayAnim(PlayerPed, 'mdx@holdtarp', 'mdx_clip', 8.0, -8, -1, 49, 0, false, false, false)

        coverSheetEntity = CreateObject("mdx_tarp", PlayerCoords.x, PlayerCoords.y, PlayerCoords.z, true, true, true)
        AttachEntityToEntity(coverSheetEntity, PlayerPed, GetPedBoneIndex(PlayerPed, 23553), -0.05, 0.3, 0.0, 180.0, 90.0, 0.0, true, false, false, false, 0, true)

        LoadAnimation("mdx@holdtarp", false)
        LoadModel('mdx_tarp', false)

    else

        ClearPedTasks(PlayerPed)
        DeleteEntity(coverSheetEntity)
        coverSheetEntity = -1

    end

end

function LoadAnimation(pDict, pLoadAnimation)
    if not pLoadAnimation then return RemoveAnimDict(pDict) end
    while not HasAnimDictLoaded(pDict) do
        RequestAnimDict(pDict)
        Citizen.Wait(0)
    end
end

function LoadModel(pModel, pLoadModel)
    if not pLoadModel then return SetModelAsNoLongerNeeded(pModel) end
    while not HasModelLoaded(pModel) do
        RequestModel(pModel)
        Citizen.Wait(0)
    end
end

RegisterKeyMapping('coversheet', 'Toggle Cover Sheet', 'keyboard', '')
RegisterCommand('coversheet', function(source)
    ToggleCoverSheet()
end)

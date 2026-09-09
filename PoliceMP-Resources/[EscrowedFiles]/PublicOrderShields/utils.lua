function AttachShieldToPlayer(pPropLocation)
    local playerPed = GetPlayerPed(-1)
    LoadModel('mdx_longshield', true)
    ShieldEntity = CreateObject(GetHashKey('mdx_longshield'), 0, 0, 0, true, true, true)
    AttachEntityToEntity(ShieldEntity, playerPed, GetPedBoneIndex(playerPed, 64016), pPropLocation.X, pPropLocation.Y, pPropLocation.Z, pPropLocation.XRot, pPropLocation.YRot, pPropLocation.ZRot, true, true, false, true, 1, true)
    LoadModel('mdx_longshield', false)
end

function ClearShieldActivity()
    DeleteEntity(ShieldEntity)
    ClearPedTasks(GetPlayerPed(-1))
    ShieldEntity = nil
end

function LoadAnimation(pDict, pLoad)
    if pLoad then
        while not HasAnimDictLoaded(pDict) do
            RequestAnimDict(pDict)
            Citizen.Wait(0)
        end
    else
        RemoveAnimDict(pDict)
    end
end

function LoadModel(pModel, pLoad)
    if pLoad then
        while not HasModelLoaded(pModel) do
            RequestModel(pModel)
            Citizen.Wait(0)
        end
    else
        SetModelAsNoLongerNeeded(pModel)
    end
end

function Debug(pMessage)
    if Config.Debug then print(pMessage) end
end

local shieldTypeSuggestions = ''
for key, shieldType in pairs(Config.ShieldTypes) do shieldTypeSuggestions = shieldTypeSuggestions .. key .. ' = hold shield ' .. key .. ', ' end
TriggerEvent('chat:addSuggestion', '/' .. Config.CommandName, 'Deploy your long shield.', { { name= 'Type', help = shieldTypeSuggestions } })

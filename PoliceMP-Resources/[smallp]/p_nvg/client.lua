local helmetPairs = {
    { up = 117, down = 116 },
    { up = 119, down = 118 }
}

local currentVisionState = 0
local spotlightActive = false
local isAuthorized = false

local animDict = "mp_masks@standard_car@ds@"
local animName = "put_on_mask"
local animFlag = 51
local animDur = 600

local spotlightSettings = {
    offset = vector3(-0.15, 0.1, 0.185),
    color = {r = 255, g = 255, b = 255},
    distance = 115.0,
    brightness = 2.0,
    hardness = 0.15,
    radius = 25.0,
    falloff = 35.0
}

local function notify(msg)
    exports.ox_lib:notify({
        title = 'Tactical Helmet',
        description = msg,
        type = 'inform',
        position = 'bottom'
    })
end

RegisterNetEvent("helmetvision:setAuthorized")
AddEventHandler("helmetvision:setAuthorized", function(allowed)
    isAuthorized = allowed
end)

Citizen.CreateThread(function()
    TriggerServerEvent("helmetvision:checkPermission")
end)

local function playHelmetAnim()
    local ped = PlayerPedId()
    RequestAnimDict(animDict)
    while not HasAnimDictLoaded(animDict) do Wait(50) end
    TaskPlayAnim(ped, animDict, animName, 8.0, -8.0, animDur, animFlag, 0.0, false, false, false)
    Wait(animDur)
    ClearPedTasks(ped)
end

local function isWearingDownHelmet()
    local current = GetPedPropIndex(PlayerPedId(), 0)
    for _, pair in ipairs(helmetPairs) do
        if current == pair.down then return true end
    end
    return false
end

local function isWearingUpHelmet()
    local current = GetPedPropIndex(PlayerPedId(), 0)
    for _, pair in ipairs(helmetPairs) do
        if current == pair.up then return true end
    end
    return false
end

RegisterCommand("+helmetToggle", function()
    if not isAuthorized then return end
    if IsPlayerFreeAiming(PlayerId()) then return end

    local ped = PlayerPedId()
    local current = GetPedPropIndex(ped, 0)
    local newProp = nil
    local helmetDown = false

    for _, pair in ipairs(helmetPairs) do
        if current == pair.up then
            newProp = pair.down
            helmetDown = true
            break
        elseif current == pair.down then
            newProp = pair.up
            break
        end
    end

    if not newProp then return end

    playHelmetAnim()
    SetPedPropIndex(ped, 0, newProp, 0, true)

    if helmetDown then
        currentVisionState = 1
        SetNightvision(true)
        spotlightActive = false
        Entity(ped).state:set("helmetSpotlight", false, true)
        notify("NVG Enabled")
    else
        currentVisionState = 0
        SetNightvision(false)
        spotlightActive = false
        Entity(ped).state:set("helmetSpotlight", false, true)
        notify("NVG Disabled")
    end
end, false)

RegisterCommand("-helmetToggle", function() end, false)

RegisterCommand("+spotlightToggle", function()
    if not isAuthorized then return end
    if not isWearingUpHelmet() then
        --notify("Helmet lights only works with NVG diaabled")
        return
    end

    local ped = PlayerPedId()

    spotlightActive = not spotlightActive
    currentVisionState = spotlightActive and 2 or 0
    Entity(ped).state:set("helmetSpotlight", spotlightActive, true)
    notify(spotlightActive and "Helmet Light On" or "Helmet Light Off")
end, false)

RegisterCommand("-spotlightToggle", function() end, false)

RegisterKeyMapping("+helmetToggle", "Toggle NVG Helmet On/Off", "keyboard", "H")
RegisterKeyMapping("+spotlightToggle", "Toggle Helmet Light", "keyboard", "J")

Citizen.CreateThread(function()
    while true do
        Wait(0)
        if spotlightActive then
            local ped = PlayerPedId()
            local boneIndex = GetPedBoneIndex(ped, 65068)
            local bonePos = GetWorldPositionOfEntityBone(ped, boneIndex)
            local boneRot = GetEntityRotation(ped, 2)
            local rotVec = vector3(
                -math.sin(math.rad(boneRot.z)),
                math.cos(math.rad(boneRot.z)),
                math.sin(math.rad(boneRot.x))
            )
            local lightPos = GetOffsetFromCoordAndHeadingInWorldCoords(
                bonePos.x, bonePos.y, bonePos.z,
                GetEntityHeading(ped),
                spotlightSettings.offset.x,
                spotlightSettings.offset.y,
                spotlightSettings.offset.z
            )

            DrawSpotLight(
                lightPos.x, lightPos.y, lightPos.z,
                rotVec.x, rotVec.y, rotVec.z,
                spotlightSettings.color.r,
                spotlightSettings.color.g,
                spotlightSettings.color.b,
                spotlightSettings.distance,
                spotlightSettings.brightness,
                spotlightSettings.hardness,
                spotlightSettings.radius,
                spotlightSettings.falloff
            )
        end
    end
end)

Citizen.CreateThread(function()
    while true do
        Wait(500)
        local ped = PlayerPedId()

        if currentVisionState == 1 and not isWearingDownHelmet() then
            currentVisionState = 0
            SetNightvision(false)
            notify("NVG Disabled (Helmet Off)")
        end

        if currentVisionState == 2 and not isWearingUpHelmet() then
            currentVisionState = 0
            spotlightActive = false
            Entity(ped).state:set("helmetSpotlight", false, true)
            notify("Helmet Light Disabled (Helmet Off)")
        end
    end
end)

Citizen.CreateThread(function()
    while true do
        Wait(0)
        local myPed = PlayerPedId()
        local myCoords = GetEntityCoords(myPed)

        for _, i in ipairs(GetActivePlayers()) do
            local ped = GetPlayerPed(i)
            if ped ~= myPed and DoesEntityExist(ped) then
                local state = Entity(ped).state
                if state and state.helmetSpotlight then
                    local targetCoords = GetEntityCoords(ped)
                    if #(targetCoords - myCoords) <= 200.0 then
                        local boneIndex = GetPedBoneIndex(ped, 65068)
                        local bonePos = GetWorldPositionOfEntityBone(ped, boneIndex)
                        local boneRot = GetEntityRotation(ped, 2)
                        local rotVec = vector3(
                            -math.sin(math.rad(boneRot.z)),
                            math.cos(math.rad(boneRot.z)),
                            math.sin(math.rad(boneRot.x))
                        )
                        local lightPos = GetOffsetFromCoordAndHeadingInWorldCoords(
                            bonePos.x, bonePos.y, bonePos.z,
                            GetEntityHeading(ped),
                            spotlightSettings.offset.x,
                            spotlightSettings.offset.y,
                            spotlightSettings.offset.z
                        )

                        DrawSpotLight(
                            lightPos.x, lightPos.y, lightPos.z,
                            rotVec.x, rotVec.y, rotVec.z,
                            spotlightSettings.color.r,
                            spotlightSettings.color.g,
                            spotlightSettings.color.b,
                            spotlightSettings.distance,
                            spotlightSettings.brightness,
                            spotlightSettings.hardness,
                            spotlightSettings.radius,
                            spotlightSettings.falloff
                        )
                    end
                end
            end
        end
    end
end)

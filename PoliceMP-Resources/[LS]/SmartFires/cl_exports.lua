function IsFireNearby(range)
    if range == nil then range = 15.0 end
    for k, v in pairs(fires) do
        if v.active then
            local distance = #(coords - v.coords)
            if distance < range then
                return true
            end
        end
    end
    return false
end

exports("IsFireNearby", IsFireNearby)

function IsSmokeNearby(range)
    if range == nil then range = 15.0 end
    for k, v in pairs(smoke) do
        if v.active then
            local distance = #(coords - v.coords)
            if distance < range then
                return true
            end
        end
    end
    return false
end

exports("IsSmokeNearby", IsSmokeNearby)

-- This will only work if you are using HoseLS (or an alternative hose)
-- This will not work if you are using SmartHose, our paid hose
function SetFoamEnabled(enabled)
    foam = enabled
end

exports("SetFoam", SetFoamEnabled)

function IsFireStillActive(id)
    if fires[id] ~= nil then
        return true
    end
end

exports("IsFireStillActive", IsFireStillActive)
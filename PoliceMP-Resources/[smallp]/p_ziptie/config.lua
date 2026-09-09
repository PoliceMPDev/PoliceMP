config = {}
config.max_distance = 1 
config.sound_vol = 0.1

config.enable_perms = true
config.ace_perm = "group.TierTwo"
config.ace_permpc = "Police.pc"

-- do not touch below -- 
function Closetplayer()
    local ped = PlayerPedId()
    for _, Player in ipairs(GetActivePlayers()) do
        if GetPlayerPed(Player) ~= PlayerPedId() then
            local Ped2 = GetPlayerPed(Player)
            local x, y, z = table.unpack(GetEntityCoords(ped))
            if (GetDistanceBetweenCoords(GetEntityCoords(Ped2), x, y, z) <  config.max_distance) then
                local playerId = GetPlayerServerId(Player);
                return playerId;
            end
        end
    end
    return false;
end
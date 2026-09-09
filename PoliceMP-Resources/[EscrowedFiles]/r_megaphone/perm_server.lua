-- Do not touch perms for band2 --

RegisterNetEvent("megaphone:tryUse", function()
    local src = source
    if IsPlayerAceAllowed(src, "staff.bandTwo") then
        TriggerClientEvent("megaphone:toggle", src)
    else
        TriggerClientEvent("megaphone:deniedNotification", src)
    end
end)

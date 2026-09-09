local dragging = {}
local dragged = {}

RegisterServerEvent("pmp_DragPeople:sync")
AddEventHandler("pmp_DragPeople:sync", function(targetSrc)
    local source = source
    if targetSrc > 0 and #(GetEntityCoords(GetPlayerPed(source)) - GetEntityCoords(GetPlayerPed(targetSrc))) < 20.0 then
        
        TriggerClientEvent("pmp_DragPeople:syncTarget", targetSrc, source)
        dragging[source] = targetSrc
        dragged[targetSrc] = source
    end
end)

RegisterServerEvent("pmp_DragPeople:stop")
AddEventHandler("pmp_DragPeople:stop", function(targetSrc)
    local source = source

    if dragging[source] then
        TriggerClientEvent("pmp_DragPeople:cl_stop", targetSrc, source)
        dragging[source] = nil
        dragged[targetSrc] = nil
    end
end)

AddEventHandler('playerDropped', function(reason)
    local source = source
    dragging[source] = nil
    dragged[source] = nil
end)

RegisterServerEvent("pmp_DragPeople:checkDragPermission")
AddEventHandler("pmp_DragPeople:checkDragPermission", function()
    local src = source
    if IsPlayerAceAllowed(src, "Police.afoTrained") or IsPlayerAceAllowed(src, "Civ.Trained") then
        TriggerClientEvent("pmp_DragPeople:startDrag", src)
    else
        TriggerClientEvent("pmp_DragPeople:sendErrorNotification", src, "You are not allowed to use this command!")
    end
end)

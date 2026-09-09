RegisterNetEvent("p_torchlight:p_SynClient")
AddEventHandler("p_torchlight:p_SynClient", function()
    local src = source

    TriggerClientEvent("p_torchlight:p_AddSynClient", -1, src)
end)

RegisterNetEvent("p_torchlight:p_PassSynClient")
AddEventHandler("p_torchlight:p_PassSynClient", function()
    local src = source
    TriggerClientEvent("p_torchlight:p_RemSynClient", -1, src)
end)

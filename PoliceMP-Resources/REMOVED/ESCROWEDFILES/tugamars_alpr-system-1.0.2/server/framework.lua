if Config.Framework.Framework == "esx_legacy" then
    ESX = exports["es_extended"]:getSharedObject()

    if(Confi.Framework.AllowItem) then
        ESX.RegisterUsableItem(Config.Framework.ItemName, function(source,item)
            TriggerClientEvent("tugamars:alpr:tablet:open", source);
        end)
    end
end

if Config.Framework.Framework == "qb-core" then
    QBCore = exports['qb-core']:GetCoreObject()

    if(Config.Framework.AllowItem) then
        QBCore.Functions.CreateUseableItem(Config.Framework.ItemName, function(source,item)
            TriggerClientEvent("tugamars:alpr:tablet:open", source);
        end)
    end
end
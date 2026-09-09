local options = {
    {
        type = 'client',
        event = 'kq_detective:investigate',
        icon = 'fas fa-user-injured',
        label = L('Investigate'),
        canInteract = function(entity)
            return IsDead(entity) and CanInvestigate()
        end,
        distance = 1.75
    },
}

function AddPedToTargetting(ped)
    if (Config.target.enabled and Config.target.system) then

        local system = Config.target.system
    
        if system == 'ox_target' or system == 'ox-target' then
            exports[system]:addEntity({NetworkGetNetworkIdFromEntity(ped)}, options)
        else
            exports[system]:AddTargetEntity(ped, {
                options = options,
                distance = 1.75,
            })
        end
    end
end

if (Config.target.enabled and Config.target.system) then
    local system = Config.target.system
    
    if system == 'ox_target' or system == 'ox-target' then
        exports[system]:addGlobalPlayer(options)
    end
end

if Config.Framework.UseEmbedTarget then
    for k,v in pairs(Config.Framework.Locations.Departments) do
        exports['qb-target']:AddBoxZone("DepartmentComputer_"..k, v.coords,v.length, v.width, {
            name = "DepartmentComputer_"..k,
            debugPoly = false,
            minZ = -100.0,
            maxZ = 350.0,
        }, {
            options = {
                {
                    action = function()
                        ALPRGui(true)
                    end,
                    icon = 'fas fa-computer',
                    label = 'Open ALPR System',
                    job = Config.Framework.JobsWithAccess,
                    canInteract = function(entity)
                        if(Config.Framework.Objects.Monitors[GetEntityModel(entity)]) then return true; end
                        return false;
                    end,
                },
            },
            distance = 2.5
        })
    end
end
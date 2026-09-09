Config = {}

--If you use esx enable this
Config.UseESX = false

--If you use qb core enable this
Config.UseQBCore = false

--If you use BJ Core enable this
Config.UseBJCore = false

--Enable This If You Use Target
--To Whitelist Vehicles You Still Need To Add Them Inside The Settings.ini
Config.UseTarget = false
--Set this to true to enable ox target
Config.UseOXTarget = false

--To use inventory spike items
Config.ItemName = "spikespack"

--Whitelisted Jobs To Use The Spikes
--Use This Without QB-Target And Set The Job In The Target Exports Below
Config.UseJobWhitelist = false--Set this to false to keep the 
Config.JobNames = {
    "police",
    "police2"
}

--Standalone identifier/ace whitelist
--Set UseWhitelist To True To Use The Identifier Whitelist And Ace Whitelist (Standalone Only)

Config.UseWhitelist = true

Config.AcePermission = "SpikeScript.Permission.All"

Config.Identifiers = {
    "steam:11000012430xfa",
    "license:1123d12313"
}

-- Target Support
if Config.UseTarget then

    if Config.UseOXTarget then
        RegisterNetEvent('SpikeScript:Target:AddSpike')
        AddEventHandler('SpikeScript:Target:AddSpike', function(prop)
            local id = exports['ox_target']:addBoxZone(
            {
                options = {
                    {
                        icon = 'fas fa-x',
                        name = 'pickupSpike' .. prop,
                        label = 'Pickup Spike',
                        onSelect = function(data)
                            TriggerEvent("SpikeScript:RemoveSpike", prop)
                        end,
                        distance = 10.0
                    },
                    {
                        icon = 'fas fa-x',
                        label = 'Pickup All Spikes',
                        name = 'pickupSpike' .. prop,
                        onSelect = function(data)
                            TriggerEvent("SpikeScript:RemoveSpike", -1)
                        end,
                        distance = 10.0
                    }
                },
                coords = GetEntityCoords(prop),
                size = vector3(1, 1, 1),
                debug = false,
                drawSprite = true
            })

            TriggerEvent("SpikeScript:Target:LinkSpike", prop, id)

        end)

        RegisterNetEvent('SpikeScript:Target:RemoveSpike')
        AddEventHandler('SpikeScript:Target:RemoveSpike', function(id)
            exports['ox_target']:removeZone(id)
        end)

        RegisterNetEvent('SpikeScript:Target:Load')
        AddEventHandler('SpikeScript:Target:Load', function()
            exports['ox_target']:addGlobalVehicle(
            {
                {
                    icon = 'fas fa-mug-hot',
                    label = 'Grab Spikes',
                    name = 'grabSpikes',
                    onSelect = function(data)
                        TriggerEvent("SpikeScript:ToggleHandSpike", true)
                    end,
                    canInteract = function(entity, distance, coords, name, bone)
                        return exports['spikescript']:CanInteract(true)
                    end,
                    bones = {'boot'},
                    distance = 2.5
                },
                {
                    icon = 'fas fa-x',
                    label = 'Place Spikes',
                    onSelect = function(data)
                        TriggerEvent("SpikeScript:ToggleHandSpike", false)
                    end,
                    canInteract = function(entity, distance, coords, name, bone)
                        return exports['spikescript']:CanInteract(false)
                    end,
                    bones = {'boot'},
                    distance = 2.5
                }
            })

        end)
    else

        RegisterNetEvent('SpikeScript:Target:AddSpike')
        AddEventHandler('SpikeScript:Target:AddSpike', function(prop)
            exports['qb-target']:AddEntityZone('pickupSpike' .. prop, prop, {

                name = 'pickupSpike' .. prop,
                debugPoly = false,

            },
            
            {
                options = {
                    {
                        icon = 'fas fa-x',
                        label = 'Pickup Spike',
                        --Add Your Job Here
                        --job='police',
                        action = function()
                            TriggerEvent("SpikeScript:RemoveSpike", prop)
                        end,
                    },
                    {
                        icon = 'fas fa-x',
                        label = 'Pickup All Spikes',
                        --Add Your Job Here
                        --job='police',
                        action = function()
                            TriggerEvent("SpikeScript:RemoveSpike", -1)
                        end,
                    }
                },
                distance = 10.0
            })

            

        end)

        RegisterNetEvent('SpikeScript:Target:RemoveSpike')
        AddEventHandler('SpikeScript:Target:RemoveSpike', function(prop)
            exports['qb-target']:RemoveZone('pickupSpike' .. prop)
        end)

        RegisterNetEvent('SpikeScript:Target:Load')
        AddEventHandler('SpikeScript:Target:Load', function()
            exports['qb-target']:AddTargetBone({'boot'},
            {
                options = {
                    {
                        icon = 'fas fa-mug-hot',
                        label = 'Grab Spikes',
                        action = function()
                            TriggerEvent("SpikeScript:ToggleHandSpike", true)
                        end,
                        canInteract = function(entity, distance, data)
                            return exports['spikescript']:CanInteract(true)
                        end,
                    },
                    {
                        icon = 'fas fa-x',
                        label = 'Place Spikes',
                        action = function()
                            TriggerEvent("SpikeScript:ToggleHandSpike", false)
                        end,
                        canInteract = function(entity, distance, data)
                            return exports['spikescript']:CanInteract(false)
                        end,
                    }
                },
                distance = 2.5
            })

        end)
    end
end
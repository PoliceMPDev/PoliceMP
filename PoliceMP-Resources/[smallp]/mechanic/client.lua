local MenuPool = MenuPool.New()
local mechanicMenu = nil
local isMenuActive = false

function GetClosestVehicle()
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)
    local closestVehicle = nil
    local closestDistance = 5.0

    local vehicles = GetGamePool('CVehicle')
    for _, vehicle in ipairs(vehicles) do
        local vehicleCoords = GetEntityCoords(vehicle)
        local distance = #(playerCoords - vehicleCoords)

        if distance < closestDistance then
            closestVehicle = vehicle
            closestDistance = distance
        end
    end

    return closestVehicle
end

function StartEngineRepairAnimation()
    local playerPed = PlayerPedId()
    local animDict = "mini@repair"
    local animName = "fixing_a_ped"
    
    RequestAnimDict(animDict)
    while not HasAnimDictLoaded(animDict) do
        Citizen.Wait(100)
    end

    TaskPlayAnim(playerPed, animDict, animName, 8.0, -8.0, -1, 49, 0, false, false, false)
    Citizen.Wait(20000)
    ClearPedTasks(playerPed)
end

function StartBodyRepairAnimation()
    local playerPed = PlayerPedId()
    local animDict = "amb@world_human_maid_clean@"
    local animName = "idle_b"

    RequestAnimDict(animDict)
    while not HasAnimDictLoaded(animDict) do
        Citizen.Wait(100)
    end

    TaskPlayAnim(playerPed, animDict, animName, 8.0, -8.0, -1, 33, 0, false, false, false)
    Citizen.Wait(20000)
    ClearPedTasks(playerPed)
end

function StartTireChangeAnimation()
    local playerPed = PlayerPedId()
    local animDict = "anim@amb@business@weed@weed_inspecting_lo_med_hi@"
    local animName = "weed_crouch_checkingleaves_idle_04_inspectorfemale"

    RequestAnimDict(animDict)
    while not HasAnimDictLoaded(animDict) do
        Citizen.Wait(100)
    end

    TaskPlayAnim(playerPed, animDict, animName, 8.0, -8.0, -1, 49, 0, false, false, false)
    Citizen.Wait(20000)
    ClearPedTasks(playerPed)
end

function StartFuelAnimation()
    local playerPed = PlayerPedId()
    local animDict = "timetable@gardener@filling_can"
    local animName = "gar_ig_5_filling_can"
    
    RequestAnimDict(animDict)
    while not HasAnimDictLoaded(animDict) do
        Citizen.Wait(100)
    end

    TaskPlayAnim(playerPed, animDict, animName, 8.0, -8.0, -1, 49, 0, false, false, false)
    Citizen.Wait(15000)
    ClearPedTasks(playerPed)
end

function RefuelVehicle()
    local vehicle = GetClosestVehicle()
    if vehicle and DoesEntityExist(vehicle) then
        local currentFuel = GetVehicleFuelLevel(vehicle)
        local maxFuel = 100.0 
        local newFuel = math.min(currentFuel + 20.0, maxFuel) 

        SetVehicleFuelLevel(vehicle, newFuel)
        print("Vehicle refueled by 20%.")
        drawNotification('Vehicle refueled by 20%!')
    else
        drawNotification('No vehicle nearby!')
    end
end

function RepairBody()
    local vehicle = GetClosestVehicle()
    if vehicle and DoesEntityExist(vehicle) then
        SetVehicleBodyHealth(vehicle, 1000.0)
        SetVehicleDeformationFixed(vehicle)
            
        for i = 0, 5 do
            FixVehicleWindow(vehicle, i)
            RollUpWindow(vehicle, i)
        end
             
        print("Body repaired.")
        drawNotification('Body repaired!')
    else
        drawNotification('No vehicle nearby!')
    end
end

function RepairEngine()
    local vehicle = GetClosestVehicle()
    if vehicle and DoesEntityExist(vehicle) then
        SetVehicleEngineHealth(vehicle, 1000.0)
        print("Engine repaired.")
        drawNotification('Engine repaired!')
    else
        drawNotification('No vehicle nearby!')
    end
end

function ChangeTires()
    local vehicle = GetClosestVehicle()

    if vehicle and DoesEntityExist(vehicle) then
        local tireIndices = {0, 1, 4, 5, 2, 3, 45, 47}

        for _, index in ipairs(tireIndices) do
            if IsVehicleTyreBurst(vehicle, index, false) or IsVehicleTyreBurst(vehicle, index, true) then
                SetVehicleTyreFixed(vehicle, index)
                print("Tire " .. index .. " fixed.")
            else
                print("Tire " .. index .. " is not burst.")
            end
        end
        drawNotification('Tires changed!')
    else
        drawNotification('No vehicle nearby!')
    end
end

function FixDeformation()
    local vehicle = GetClosestVehicle()

    if vehicle and DoesEntityExist(vehicle) then
        SetVehicleFixed(vehicle)
        drawNotification('Broken Wheels fixed!')
    else
        drawNotification('No vehicle nearby!')
    end
end

function IsPlayerInVehicle()
    local playerPed = PlayerPedId()
    return IsPedInAnyVehicle(playerPed, false)
end

RegisterNetEvent('mechanic:openMenu')
AddEventHandler('mechanic:openMenu', function()
    
    local playerPed = PlayerPedId()
    local vehicle = GetClosestVehicle()

    if DoesEntityExist(vehicle) then
        OpenMechanicMenu(vehicle)
    else

    end
end)

function OpenMechanicMenu(vehicle)
    -- Check if the player is wearing pants with drawable 199
    local playerPed = PlayerPedId()
    local pantsDrawable = GetPedDrawableVariation(playerPed, 4) -- 4 is the component ID for pants

    if pantsDrawable ~= 199 then
        
        return
    end

    if isMenuActive then
        CloseMechanicMenu()
    end
    isMenuActive = true

    mechanicMenu = NativeUI.CreateMenu("Mechanic Menu", "~b~Choose an action", 1490.0, 0.5) -- Adjust X and Y
    MenuPool:Add(mechanicMenu)

    -- Option 1: Check vehicle status
    local statusItem = NativeUI.CreateItem("Check Vehicle Status", "Shows the vehicle's engine, fuel, damage, and tire status")
    mechanicMenu:AddItem(statusItem)
    statusItem.Activated = function(sender, item)
        if item == statusItem then
            if IsPlayerInVehicle() then
                drawNotification('You must be outside of a vehicle to check its status!')
            else
                StartEngineRepairAnimation()
                CheckVehicleStatus(vehicle)
            end
        end
    end

    -- Option 2: Repair engine
    local repairEngineItem = NativeUI.CreateItem("Repair Engine", "Fix the vehicle's engine")
    mechanicMenu:AddItem(repairEngineItem)
    repairEngineItem.Activated = function(sender, item)
        if item == repairEngineItem then
            if IsPlayerInVehicle() then
                drawNotification('You must be outside of a vehicle to repair its engine!')
            else
                StartEngineRepairAnimation()
                RepairEngine()
            end
        end
    end

    -- Option 3: Repair body
    local repairBodyItem = NativeUI.CreateItem("Repair Body", "Fix the vehicle's body")
    mechanicMenu:AddItem(repairBodyItem)
    repairBodyItem.Activated = function(sender, item)
        if item == repairBodyItem then
            if IsPlayerInVehicle() then
                drawNotification('You must be outside of a vehicle to repair its body!')
            else
                StartBodyRepairAnimation()
                RepairBody()
            end
        end
    end

    -- Option 4: Change tires
    local changeTiresItem = NativeUI.CreateItem("Repair Tyres", "Replace tyres")
    mechanicMenu:AddItem(changeTiresItem)
    changeTiresItem.Activated = function(sender, item)
        if item == changeTiresItem then
            if IsPlayerInVehicle() then
                drawNotification('You must be outside of a vehicle to change its tyres!')
            else
                StartTireChangeAnimation()
                ChangeTires()
            end
        end
    end

    -- Option 5: Refuel vehicle
    local refuelItem = NativeUI.CreateItem("Refuel Vehicle", "Add 20% fuel to the vehicle")
    mechanicMenu:AddItem(refuelItem)
    refuelItem.Activated = function(sender, item)
        if item == refuelItem then
            if IsPlayerInVehicle() then
                drawNotification('You must be outside of a vehicle to refuel it!')
            else
                StartFuelAnimation()
                RefuelVehicle()
            end
        end
    end

    -- Option 6: Fix deformation
    local fixDeformationItem = NativeUI.CreateItem("Repair Broken Wheels", "Only to be used to fix broken or missing wheels.")
    mechanicMenu:AddItem(fixDeformationItem)
    fixDeformationItem.Activated = function(sender, item)
        if item == fixDeformationItem then
            if IsPlayerInVehicle() then
                drawNotification('You must be outside of a vehicle to fix broken wheels!')
            else
                StartTireChangeAnimation()
                FixDeformation()
            end
        end
    end

    -- Option 7: Deploy Ramp
    local deployRampItem = NativeUI.CreateItem("Deploy Ramp", "Deploy a towing ramp!.")
    mechanicMenu:AddItem(deployRampItem)
    deployRampItem.Activated = function(sender, item)
        if item == deployRampItem then
            if IsPlayerInVehicle() then
                drawNotification('You must be outside a vehicle to deploy a ramp!')
            else
                ExecuteCommand('+xsetramp')
            end
        end
    end

    -- Option 8: Remove Ramp
    local removeRampItem = NativeUI.CreateItem("Remove Ramp", "Remove the towing ramp.")
    mechanicMenu:AddItem(removeRampItem)
    removeRampItem.Activated = function(sender, item)
        if item == removeRampItem then
            if IsPlayerInVehicle() then
                drawNotification('You must be outside a vehicle to remove a ramp!')
            else
                ExecuteCommand('+xrmramp')
            end
        end
    end

    -- Option 9: Attach Vehicle
    local attachVehicleItem = NativeUI.CreateItem("Attach Vehicle", "You must be in the towed vehicle to attach on flat bed.")
    mechanicMenu:AddItem(attachVehicleItem)
    attachVehicleItem.Activated = function(sender, item)
        if item == attachVehicleItem then
            if not IsPlayerInVehicle() then
                drawNotification('You must be inside the towed vehicle to attach on the flat bed!')
            else
                ExecuteCommand('+xattach')
            end
        end
    end

    -- Option 10: Detach Vehicle
    local detachVehicleItem = NativeUI.CreateItem("Detach Vehicle", "Detach the attached vehicle")
    mechanicMenu:AddItem(detachVehicleItem)
    detachVehicleItem.Activated = function(sender, item)
        if item == detachVehicleItem then
            if not IsPlayerInVehicle() then
                drawNotification('You must be inside the towed vehicle to detach!')
            else
                ExecuteCommand('+xdetach')
            end
        end
    end

    mechanicMenu:RefreshIndex()
    mechanicMenu:Visible(true)
    
    MenuPool:RefreshIndex()
    mechanicMenu:RefreshIndex()
    MenuPool:MouseControlsEnabled(false)
    MenuPool:ControlDisablingEnabled(false)
end

function CloseMechanicMenu()
    if not isMenuActive then return end
    isMenuActive = false
    if mechanicMenu then
        mechanicMenu:Visible(false)
        mechanicMenu = nil 

        TriggerServerEvent('mechanic:uiClosed')
    end
end

function CheckVehicleStatus(vehicle)
    local engineHealth = GetVehicleEngineHealth(vehicle)
    local bodyHealth = GetVehicleBodyHealth(vehicle)
    local fuelLevel = GetVehicleFuelLevel(vehicle)

    local engineStatus = (engineHealth > 800 and "Good") or (engineHealth > 400 and "Average") or "Bad"
    local bodyStatus = (bodyHealth > 800 and "Good") or (bodyHealth > 400 and "Average") or "Bad"
    local fuelStatus = string.format("%d%%", math.floor(fuelLevel)) 

    local tireStatus = {}
    for i = 0, 3 do
        if IsVehicleTyreBurst(vehicle, i, false) then
            table.insert(tireStatus, string.format("Tire %d: Burst", i + 1))
        else
            table.insert(tireStatus, string.format("Tire %d: Good", i + 1))
        end
    end

    TriggerEvent('chat:addMessage', {
        args = { '^3[Mechanic]', string.format("Engine: %s | Body: %s | Fuel: %s", engineStatus, bodyStatus, fuelStatus) }
    })

    for _, status in ipairs(tireStatus) do
        TriggerEvent('chat:addMessage', { args = { '^3[Mechanic]', status } })
    end
end

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        MenuPool:ProcessMenus()

        if IsControlJustReleased(1, 168) then 
            TriggerEvent('mechanic:openMenu')
        end

        if isMenuActive and IsControlJustReleased(1, 322) then
            CloseMechanicMenu()
        end
    end
end)

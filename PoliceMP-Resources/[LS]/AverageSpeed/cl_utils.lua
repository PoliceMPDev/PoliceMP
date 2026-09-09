local modelHash = `prop_speed_camera`
detecting = false
time = 0
mph = true
fined = false
if main.useKmh ~= nil then
    if main.useKmh then
        mph = false
    end
end

function tableHas(tableSubmit, value)
    for k in pairs(tableSubmit) do
        if tableSubmit[k] == value then
            return true
        end
    end
    return false
end

Citizen.CreateThread(function()
    while true do
        local ped = PlayerPedId()
        local vehicle = GetVehiclePedIsIn(ped, false)
        if vehicle ~= 0 then
            if GetPedInVehicleSeat(vehicle, -1) == ped then
                local class = GetVehicleClass(vehicle)
                local model = GetEntityModel(vehicle)
                if (main.exemptVehicleClasses == nil or class == nil or not tableHas(main.exemptVehicleClasses, class)) and not tableHas(main.exemptVehicleNames, model) then
                    local coords = GetEntityCoords(ped)
                    if not detecting then
                        for k, v in pairs(config) do
                            local distance = #(coords - v.cameraOne.coords)
                            if distance < main.detectionRadius then
                                TriggerEvent("handleDetection", k)
                                break
                            end
                        end
                    else
                        time = time + 0.05
                        for k, v in pairs(config) do
                            local distance = #(coords - config[k].cameraTwo.coords)
                            if distance < main.detectionRadius then
                                local speed = 0.0
                                if mph then
                                    speed = ((config[k].distance / time) * 2.236936) * main.reduceBy
                                else
                                    speed = ((config[k].distance / time) * 3.6) * main.reduceBy
                                end
                                local percentage = main.addPercentage / 10
                                local limit = main.addLimit
                                if speed > config[k].limit + percentage + limit then
                                    if not fined then
                                        fined = true
                                        Citizen.SetTimeout(5000, function()
                                            fined = false
                                        end)
                                        local roadName = GetStreetNameAtCoord(coords.x, coords.y, coords.z)
                                        local roadNameString = GetStreetNameFromHashKey(roadName)
                                        local numberplate = tostring(GetVehicleNumberPlateText(vehicle))
                                        if framework.ESX.ESXBilling.enabled then
                                            TriggerServerEvent('esx_billing:sendBill', GetPlayerServerId(PlayerId()), framework.ESX.ESXBilling.society, framework.ESX.ESXBilling.name, framework.ESX.ESXBilling.amount)
                                        end
                                        TriggerServerEvent("Server:AverageSpeedDetection", k, speed, roadNameString, numberplate)
                                        if main.flashScreen ~= nil and main.flashScreen then
                                            TriggerEvent("flashScreen")
                                        end
                                    end
                                end
                                detecting = false
                                time = 0
                            end
                        end
                    end      
                end    
            end
        end
        Wait(50)
    end
end)
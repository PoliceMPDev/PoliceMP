local Peds = {}
local repairing = false

function Draw3DText(x, y, z, text)
	local onScreen, _x, _y = World3dToScreen2d(x, y, z) 
	local p = GetGameplayCamCoords()
	local distance = GetDistanceBetweenCoords(p.x, p.y, p.z, x, y, z, 1)
	local scale = (1 / distance) * 0.68
	local fov = (1 / GetGameplayCamFov()) * 100
	local scale = scale * fov 
	if onScreen then
	SetTextScale(0.0, scale)
	SetTextFont(0)
	SetTextProportional(1)
	SetTextColour(255, 255, 255, 228)
	SetTextOutline()
	SetTextEntry("STRING")
	SetTextCentre(1)
	AddTextComponentString(text)  	DrawText(_x, _y)
	end
end

function ShowSubtitle(message, duration)
    BeginTextCommandPrint('STRING')
    AddTextComponentString(message)
    EndTextCommandPrint(duration, true)
end

Utils = {
    CreatePed = function(Model, Data)
      Utils.LoadModel(Model);
  
      local Entity = CreatePed(4, Model, Data.Coords, Data.Heading, false);
    
      SetPedCombatAttributes(Entity, 46, true);                   
      SetPedFleeAttributes(Entity, 0, 0);              
      SetEntityAsMissionEntity(Entity, true, true);
      SetPedRandomComponentVariation(Entity);
      SetPedRandomProps(Entity);
      FreezeEntityPosition(Entity, true);
      SetBlockingOfNonTemporaryEvents(Entity, true);
      SetEntityInvincible(Entity, true);
      SetPedCanRagdoll(Entity, false);
      SetEntityVisible(Entity, true);
      SetEntityCollision(Entity, true);
      SetPedDefaultComponentVariation(Entity);
  
      return Entity
    end,
  
    LoadModel = function(model)
      RequestModel(model);
    
      while not HasModelLoaded(model) do
        Citizen.Wait(5)
      end
    end
  }


Citizen.CreateThread(function()
    while true do 
      local player, sleep15 = PlayerPedId(), 1000
        for i = 1, #Config.AdvanceRepair do
          local distance = #(GetEntityCoords(player) - Config.AdvanceRepair[i].Mechanic.Coords)
          local distanceInteract = #(GetEntityCoords(player) - Config.AdvanceRepair[i].Mechanic.Coords)
          RequestModel(Config.AdvanceRepair[i].Mechanic["Model"])
          if distance < 50.0 and not DoesEntityExist(Peds[i]) then 
            Peds[i] = Utils.CreatePed(Config.AdvanceRepair[i].Mechanic["Model"], Config.AdvanceRepair[i]["Mechanic"])
            FreezeEntityPosition(Peds[i], true)
            SetEntityInvincible(Peds[i], true)
            SetEntityInvincible(Peds[i], true)          
            SetBlockingOfNonTemporaryEvents(Peds[i], true)  
          end
          if distance < 10.0 and not IsPedDeadOrDying(player, true) then
            sleep15 = 1
            Draw3DText(Config.AdvanceRepair[i].Mechanic.Coords.x , Config.AdvanceRepair[i].Mechanic.Coords.y, Config.AdvanceRepair[i].Mechanic.Coords.z + 2.0, Config.AdvanceRepair[i].Mechanic["Name"] .. " " .. Config.AdvanceRepair[i].Mechanic["Job"])
            if distanceInteract < 10.0 and not IsPedDeadOrDying(player, true) and IsPedInAnyVehicle(player, false) and not repairing then
            Draw3DText(Config.AdvanceRepair[i].Mechanic.CarInteractCoord.x, Config.AdvanceRepair[i].Mechanic.CarInteractCoord.y, Config.AdvanceRepair[i].Mechanic.CarInteractCoord.z, Config.AdvanceRepair[i].Mechanic["InteractText"])
            Draw3DText(Config.AdvanceRepair[i].Mechanic.CarInteractCoord.x, Config.AdvanceRepair[i].Mechanic.CarInteractCoord.y, Config.AdvanceRepair[i].Mechanic.CarInteractCoord.z - 0.088, Config.AdvanceRepair[i].Mechanic["DriftTyres"])
               if IsControlJustPressed(0, 38) then 
                    local vehicle = GetVehiclePedIsIn(GetPlayerPed(-1))
                    ShowSubtitle('~y~Repairing ~w~ your vehicle...', 4800)
                    SetEntityCoords(vehicle, Config.AdvanceRepair[i].Mechanic.CarInteractCoord.x, Config.AdvanceRepair[i].Mechanic.CarInteractCoord.y, Config.AdvanceRepair[i].Mechanic.CarInteractCoord.z - 1 )
                    SetEntityHeading(vehicle, Config.AdvanceRepair[i].Mechanic.CarInteractHeading)
                    FreezeEntityPosition(Peds[i], false)
                    TaskPedSlideToCoord(Peds[i], Config.AdvanceRepair[i].Mechanic.CarHoodCoord.x, Config.AdvanceRepair[i].Mechanic.CarHoodCoord.y, Config.AdvanceRepair[i].Mechanic.CarHoodCoord.z, Config.AdvanceRepair[i].Mechanic.CarHoodHeading, 400.0)
                    FreezeEntityPosition(vehicle, true)
                    SetVehicleDoorOpen(vehicle, 4, true, true)
                    local animDict = "anim@amb@clubhouse@tutorial@bkr_tut_ig3@"
                    local animName = "machinic_loop_mechandplayer"
                    Citizen.Wait(2800)
                    RequestAnimDict(animDict)
                    while not HasAnimDictLoaded(animDict) do
                             Citizen.Wait(100)
                    end
                        SetVehicleDoorOpen(vehicle, 4, false, true)
                    TaskPlayAnim(Peds[i], animDict, animName, 8.0, -8.0, -1, 1, 0, false, false, false)
                    Citizen.Wait(8000)
                    ClearPedTasks(Peds[i])
                    TaskPedSlideToCoord(Peds[i], Config.AdvanceRepair[i].Mechanic.Coords.x, Config.AdvanceRepair[i].Mechanic.Coords.y, Config.AdvanceRepair[i].Mechanic.Coords.z, Config.AdvanceRepair[i].Mechanic.Heading, 400.0)
                    SetVehicleFixed(vehicle)
                    SetVehicleDeformationFixed(vehicle)
                    SetVehicleUndriveable(vehicle, false)
                    SetVehicleEngineOn(vehicle, true, true)
                    FreezeEntityPosition(vehicle, false)
                    ShowSubtitle('Your ~y~Vehicle ~w~has been repaired!...', 2800)
                    Citizen.Wait(4000)
                    FreezeEntityPosition(Peds[i], true)
                end            
            end
          end
        end		
      Citizen.Wait(sleep15)
    end
  end)

  function CreateBlipForMechanic(mechanicData)
    local blip = AddBlipForCoord(mechanicData.CarInteractCoord.x, mechanicData.CarInteractCoord.y, mechanicData.CarInteractCoord.z)
    
    SetBlipSprite(blip, 402) 
    SetBlipDisplay(blip, 4)
    SetBlipScale(blip, 1.0)
    SetBlipColour(blip, 3)
    SetBlipAsShortRange(blip, true)
    
    BeginTextCommandSetBlipName("STRING")
    AddTextComponentString(mechanicData.Name)
    EndTextCommandSetBlipName(blip)
end

Citizen.CreateThread(function()
    for _, mechanicData in ipairs(Config.AdvanceRepair) do
        CreateBlipForMechanic(mechanicData.Mechanic)
    end
end)

AddEventHandler('onResourceStop', function(resourceName)
    if (GetCurrentResourceName() ~= resourceName) then
      return
    end
end)




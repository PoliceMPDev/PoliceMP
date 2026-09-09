TriggerEvent('chat:addSuggestion', '/'..main.commandName, translations.commandSuggestion, {
    { name=translations.setup.."/"..translations.remove, help=translations.help },
})

TriggerEvent('chat:addSuggestion', '/'..main.basketCommand, translations.basketCommandSuggestion)

hasPermission = main.defaultPermission
usingRopeRescue = false
usingTripodId = 0
tripodDatabase = {}
ropeDatabase = {}
beingCarriedInBasket = false
atBottom = false
tripod = 0
rope = 0
tripodNetId = 0
carryingBasketInProgress = false
serverIdBasket = 0
serverIdPed = 0
animationStarted = false
basket = 0
playerTopCoords = nil
playerBottomCoords = nil
oldZHeight = 0
zHeight = 0

function showNotification(message)
    message = message.."."
    SetNotificationTextEntry("STRING")
	AddTextComponentString(message)
	DrawNotification(0,1)
end

function displayHelpText(message)
    SetTextComponentFormat('STRING')
    AddTextComponentString(message)
    DisplayHelpTextFromStringLabel(0, 0, 1, -1)
end

RegisterNetEvent("returnTripodPermission")
AddEventHandler("returnTripodPermission", function(permission)
    hasPermission = permission
end)

Citizen.CreateThread(function()
    while true do
        if hasPermission then
            local ped = PlayerPedId()
            local coords = GetEntityCoords(ped)
            if not usingRopeRescue and not beingCarriedInBasket then
                for k,v in pairs(tripodDatabase) do
                    if #(tripodDatabase[k][1] - coords) < 4.0 then
                        displayHelpText(translations.pressToRappel)
                        if IsControlJustPressed(main.usageKey[1], main.usageKey[2]) then
                            usingRopeRescue = true
                            usingTripodId = tripodDatabase[k][2]
                            playerTopCoords = coords
                            atBottom = false
                            local entity = NetworkGetEntityFromNetworkId(tripodDatabase[k][2])
                            TriggerEvent("continueRopeRescue", entity)
                        end
                    else
                        local entity = NetworkGetEntityFromNetworkId(tripodDatabase[k][2])
                        local offSet2 = GetOffsetFromEntityInWorldCoords(entity, toFloat(-2.40), toFloat(0.0), toFloat(-1.0))
                        local ground, groundZ2 = GetGroundZFor_3dCoord(offSet2.x, offSet2.y, offSet2.z, 0)
                        if #(coords - vector3(offSet2.x, offSet2.y, groundZ2)) < 7.0 then
                            displayHelpText(translations.pressToRappel)
                            if IsControlJustPressed(main.usageKey[1], main.usageKey[2]) then
                                usingRopeRescue = true
                                usingTripodId = tripodDatabase[k][2]
                                playerBottomCoords = coords
                                atBottom = true
                                local entity = NetworkGetEntityFromNetworkId(tripodDatabase[k][2])
                                TriggerEvent("continueRopeRescue", entity)
                            end
                        end
                    end
                    
                end
            else
                Wait(1000)
            end
        end
        Wait(0)
    end
end)
local listOn = false
local usersnames = {}
local usersnamesupdated = false

RegisterNetEvent("UpdateUsersNames")
AddEventHandler("UpdateUsersNames", function(updatedtable)
	usersnames = updatedtable
	usersnamesupdated = true
end)

Citizen.CreateThread(function()
    listOn = false
    while true do
        Wait(0)

        if IsControlPressed(0, 100)--[[ INPUT_PHONE ]] then
            if not listOn then
				TriggerServerEvent("GetPlayerNamesScoreboard", GetPlayerServerId(PlayerId()))
				while usersnamesupdated == false do
					Wait(0)
				end
				usersnamesupdated = false
                local players = {}
                for serverid, fmsdata in pairs(usersnames) do
                    local fmsname = "Unknown"
                    local defaultname = "Unknown"
					if fmsdata ~= nil then
                        if fmsdata.fulldisplayname ~= nil then
						    fmsname = fmsdata.fulldisplayname
                        end
                        if fmsdata.defaultname ~= nil then
                            defaultname = fmsdata.defaultname
                        end
					end
                    table.insert(players, 
                    '<tr style=\"color: rgb(0, 0, 0)\"><td>' .. serverid .. '</td><td>' .. fmsname .. '</td><td>' .. defaultname .. '</td></tr>'
                    )
                end
                
                SendNUIMessage({ text = table.concat(players) })

                listOn = true
                while listOn do
                    Wait(0)
                    if(IsControlPressed(0, 100) == false) then
                        listOn = false
                        SendNUIMessage({
                            meta = 'close'
                        })
                        break
                    end
                end
            end
        end
    end
end)
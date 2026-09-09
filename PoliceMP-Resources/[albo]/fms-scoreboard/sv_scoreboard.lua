RegisterServerEvent("GetPlayerNamesScoreboard")
AddEventHandler('GetPlayerNamesScoreboard', function(source)
	local playerid = source
	exports["fms"]:getAllPlayersFmsData(function(playerdata)
		for _, i in ipairs(GetPlayers()) do
			local fmsdata = playerdata[tostring(i)]
			if fmsdata == nil then
				playerdata[tostring(i)] = { defaultname = GetPlayerName(i) }
			else
				playerdata[tostring(i)].defaultname = GetPlayerName(i)
			end
		end
		TriggerClientEvent("UpdateUsersNames", playerid, playerdata)
	end)
end)
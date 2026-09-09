--[[---------------------------------------------------------------------------------------

	Wraith ARS 2X
	Created by WolfKnight

	For discussions, information on future updates, and more, join
	my Discord: https://discord.gg/fD4e6WD

	MIT License

	Copyright (c) 2020 WolfKnight

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.

---------------------------------------------------------------------------------------]] --
-- Although there is only one export at the moment, more may be added down the line.
--[[---------------------------------------------------------------------------------------
	Locks the designated plate reader camera for the given client.

	Parameters:
		clientId:
			The id of the client
		cam:
			The camera to lock, either "front" or "rear"
		beepAudio:
			Play an audible beep, either true or false
		boloAudio:
			Play the bolo lock sound, either true or false
---------------------------------------------------------------------------------------]] --
function TogglePlateLock(clientId, cam, beepAudio, boloAudio)
	TriggerClientEvent('wk:togglePlateLock', clientId, cam, beepAudio, boloAudio)
end

function getVersionData()
	return GetResourceMetadata(GetCurrentResourceName(), 'version')
end

ActiveRadars = {}

RegisterNetEvent('wk_wars2x:ActiveRadarsTable', function(id, state)
	if state then
		table.insert(ActiveRadars, id)
	else
		for i = #ActiveRadars, 1, -1 do
			if ActiveRadars[i] == id then
				table.remove(ActiveRadars, i)
			end
		end
	end
end)

RegisterNetEvent('wk_wars2x:GetActiveRadarsTable', function()
	local src = source
	if #ActiveRadars == 0 then
		TriggerClientEvent('wk_wars2x:ReturnActiveRadarTable', src, {})
	else
		TriggerClientEvent('wk_wars2x:ReturnActiveRadarTable', src, ActiveRadars)
	end
end)

JammedPlates = {}

RegisterNetEvent('wk_wars2x:SendJammedPlate', function(plate)
	if JammedPlates[plate] then
		JammedPlates[plate] = nil
	else
		JammedPlates[plate] = true
	end
	TriggerClientEvent('wk_wars2x:SendJammedListToClient', -1, JammedPlates)
end)

Citizen.CreateThread(function()
	while #JammedPlates > 0 do
		Citizen.Wait(5000)
		TriggerClientEvent('wk_wars2x:SendJammedListToClient', -1, JammedPlates)
	end
end)

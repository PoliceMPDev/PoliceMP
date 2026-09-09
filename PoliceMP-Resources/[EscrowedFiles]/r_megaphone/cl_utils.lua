/*--------------------------------------
  % Made with ❤️ for: Rytrak Store
  % Author: Rytrak https://rytrak.fr
  % Script documentation: https://docs.rytrak.fr/scripts/advanced-megaphone-system
  % Full support on discord: https://discord.gg/k22buEjnpZ
--------------------------------------*/

-- [[ Functions ]]

-- You can modify the notification system of the script (do not change the name of the function)
function Hint(message)
    AddTextEntry('NewTxtEntry', message)
    BeginTextCommandDisplayHelp('NewTxtEntry')
    EndTextCommandDisplayHelp(0, 0, 0, -1)
end

-- [[ Command ]]

-- Megaphone command
if not Config.UseFramework then
	RegisterCommand('megaphone', function()
		TriggerServerEvent("megaphone:tryUse")
	end)
end

RegisterNetEvent("megaphone:toggle", function()
    local ped = PlayerPedId()
    if GetSelectedPedWeapon(ped) == Config.Weapon then
        RemoveWeaponFromPed(ped, Config.Weapon)
    else
        GiveWeaponToPed(ped, Config.Weapon, 10, false, false)
        SetCurrentPedWeapon(ped, Config.Weapon, true)
    end
end)

RegisterNetEvent("megaphone:deniedNotification")
AddEventHandler("megaphone:deniedNotification", function()
    SetNotificationTextEntry("STRING")
    AddTextComponentString("~r~ERROR:~s~ You do not have permission! Nice try")
    DrawNotification(false, true)
end)

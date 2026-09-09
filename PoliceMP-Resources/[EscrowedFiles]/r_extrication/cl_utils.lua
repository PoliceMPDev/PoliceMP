/*--------------------------------------
  % Made with ❤️ for: Rytrak Store
  % Author: Rytrak https://rytrak.fr
  % Script documentation: https://docs.rytrak.fr/scripts/advanced-extrication-system
  % Full support on discord: https://discord.gg/k22buEjnpZ
--------------------------------------*/

-- [[ Functions ]]

function Hint(message)
    AddTextEntry('r_extrication', message)
    BeginTextCommandDisplayHelp('r_extrication')
    EndTextCommandDisplayHelp(0, 0, 0, -1)
end

-- [[ Commands ]]

if not Config.UseFramework then
    RegisterCommand('spreader', function(_, Args)
        if GetSelectedPedWeapon(ped) == `WEAPON_SPREADER` then
            RemoveWeaponFromPed(ped, `WEAPON_SPREADER`)
        else
            GiveWeaponToPed(ped, `WEAPON_SPREADER`, 0, false, false)
            SetCurrentPedWeapon(ped, `WEAPON_SPREADER`, true)
        end
    end, false)
    
    RegisterCommand('cutter', function(_, Args)
        if GetSelectedPedWeapon(ped) == `WEAPON_CUTTER` then
            RemoveWeaponFromPed(ped, `WEAPON_CUTTER`)
        else
            GiveWeaponToPed(ped, `WEAPON_CUTTER`, 0, false, false)
            SetCurrentPedWeapon(ped, `WEAPON_CUTTER`, true)
        end
    end, false)

    RegisterCommand('glassmaster', function(_, Args)
        if GetSelectedPedWeapon(ped) == `WEAPON_GLASSMASTER` then
            RemoveWeaponFromPed(ped, `WEAPON_GLASSMASTER`)
        else
            GiveWeaponToPed(ped, `WEAPON_GLASSMASTER`, 0, false, false)
            SetCurrentPedWeapon(ped, `WEAPON_GLASSMASTER`, true)
        end
    end, false)
end

exports('LockPlayerInsideVehicle', function(bool)
    Config.LockPeopleInsideCar.enabled = bool
end)
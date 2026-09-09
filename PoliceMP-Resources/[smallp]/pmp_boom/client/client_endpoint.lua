endpoint = {
	draw_timer = true, 
	draw_pin = false, 
	plant_animation = { "random@domestic", "pickup_low" },
	placeable_object = "ch_prop_ch_explosive_01a",
	default_defuse_text = "→ Bomb Timer ←\n",
	explosion_damage = true,
	explosion_damage_range = 100,
	max_range = 200,
	explode_on_incorrect = true, 
}

RegisterNetEvent('pmp_boom:plant')
AddEventHandler('pmp_boom:plant', function()
    exports['pmp_boom']:StartPlanting()
end)

RegisterNetEvent('pmp_boom:defuse')
AddEventHandler('pmp_boom:defuse', function()
    exports['pmp_boom']:StartDefusing()
end)

RegisterNetEvent('basicNotification')
AddEventHandler('basicNotification', function(message)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(false, true)
end)

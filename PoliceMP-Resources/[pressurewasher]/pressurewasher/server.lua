ESX = nil

if Config.UseESX then
	TriggerEvent('esx:getSharedObject', function(obj) ESX = obj end)

	RegisterServerEvent('pwasher:pay')
	AddEventHandler('pwasher:pay', function(price)
		local xPlayer = ESX.GetPlayerFromId(source)
		local amount = ESX.Math.Round(price)

		if price > 0 then
			xPlayer.removeMoney(amount)
		end
	end)

	ESX.RegisterServerCallback('pwasher:getCash', function (source, cb)
		local xPlayer = ESX.GetPlayerFromId(source)
		cb(xPlayer.getMoney())
	end)
end
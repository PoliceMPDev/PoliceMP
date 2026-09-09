RegisterNetEvent("playSound")
AddEventHandler("playSound", function(file)
	print("playsound " .. file)
	SendNUIMessage({
		transactionType = "playSound",
		transactionFile = file
	})
end)
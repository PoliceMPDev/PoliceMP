-- This version checks if the user has permission ID 74 on the FMS.
local useWhitelist = true

function isAuthorized(player, callback)
	if not useWhitelist then return callback(true) end
    
    exports["fms"]:checkPlayerPermission({ playerId = player, permissionId = 74, alwaysAllowOutsidePatrols = true }, function(err, hasPermission)
		if (err ~= nil) then
			print("Error! ".. err)
            return callback(false)
		else
			return callback(hasPermission)
		end
	end)
end
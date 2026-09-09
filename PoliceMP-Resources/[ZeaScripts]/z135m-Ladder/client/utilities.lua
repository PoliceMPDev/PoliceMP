util = { }

-- @param: Send a notification to the player.
-- @param: The argument 'type' will return one of two strings: 'error' or 'success.' This value can be used for customizing your notifications.
util.notification = function(string, type)
   if ( cfg.notificationSound ) then 
      PlaySoundFrontend(GetSoundId(), 'Text_Arrive_Tone', 'Phone_SoundSet_Default', true)
   end
   SetNotificationTextEntry('STRING')
   AddTextComponentString(string)
   SetNotificationBackgroundColor(140)
   SetNotificationMessage('CHAR_WE', 'logo', true, 4, GetCurrentResourceName(), 'By Zea Development')
   DrawNotification(false, true)
end

--@param: Draw text within 3d world
util.interaction = function(loc, text)
   local px,py,pz=table.unpack(GetGameplayCamCoords())
   local dist = GetDistanceBetweenCoords(px,py,pz, loc.x, loc.y, loc.z, 1)
   local scale = (1/dist)*10*(1/GetGameplayCamFov())*100
   SetTextScale(0.1*scale, 0.1*scale)
   SetTextFont(4)
   SetTextProportional(1)
   SetTextColour(250, 250, 250, 255)
   SetTextDropshadow(1, 1, 1, 1, 255)
   SetTextEdge(2, 0, 0, 0, 150)
   SetTextDropShadow()
   SetTextOutline()
   SetTextEntry("STRING")
   SetTextCentre(1)
   AddTextComponentString(text)
   SetDrawOrigin(loc.x, loc.y, loc.z, 0) 
   DrawText(0.0, 0.0)
   ClearDrawOrigin()
end

-- @param: Add an entity to the table.
util.addToTable = function(entityid)
   if ( entityid <= 0 or entityid == nil ) then 
      return
   end
   table.insert(world.objects, entityid)
end

-- @param: Load and play an animation.
util.animation = function(dict, anim, wait, int, loop)
   RequestAnimDict(dict)
   while not HasAnimDictLoaded(dict) do
      Citizen.Wait(0)
   end
   if ( not loop ) then 
      TaskPlayAnim(GetPlayerPed(PlayerId()), dict, anim, 8.0, 8.0, int, 49, 0, false, false, false)
         Citizen.Wait(wait)
      StopAnimTask(GetPlayerPed(PlayerId()), dict, anim, 1.5)
   else
      TaskPlayAnim(GetPlayerPed(PlayerId()), dict, anim, 8.0, 8.0, int, 49, 0, false, false, false)
   end
end

-- @param: Load a model.
util.load = function(hash)
   if not HasModelLoaded(hash) then
      RequestModel(hash)
      while not HasModelLoaded(hash) do
         Citizen.Wait(0)
      end
   end
   return ( true )
end

--@param: Request Network Control of Entityid.
util.control = function(entityid)
   local timeout = 0
   NetworkRequestControlOfEntity(entityid)
      while not NetworkRequestControlOfEntity(entityid) do
         NetworkRequestControlOfEntity(entityid)
         timeout = timeout + 1
            if ( timeout >= 1500 ) then
               break
            end
         Citizen.Wait(1)
      end
   return true
end
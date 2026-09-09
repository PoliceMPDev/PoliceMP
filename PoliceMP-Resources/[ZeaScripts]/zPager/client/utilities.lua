util = { }

-- @param: Use this function to create your own whitelist, This function is called upon paging a channel and setting a channel.
util.hasPermission = function(channelId, type)
   if ( type == 'sendPage') then 
      return true
   elseif ( type == 'setChannel') then
      return true
   end
end

-- @param: Send a notification to the player.
-- @param: The argument 'type' will return one of two strings: 'error' or 'success.' This value can be used for customizing your notifications.
util.notification = function(string, type)
   if ( cfg.notificationSound ) then 
      PlaySoundFrontend(GetSoundId(), 'Text_Arrive_Tone', 'Phone_SoundSet_Default', true)
   end
   SetNotificationTextEntry('STRING')
   AddTextComponentString(string)
   SetNotificationBackgroundColor(140)
   -- SetNotificationMessage('CHAR_WE', 'logo', true, 4, 'zPager', ' Zea DevelopByment')
   DrawNotification(false, true)
end

-- @param: Display text input box on screen.
util.textBox = function()
   AddTextEntry('FMMC_KEY_TIP1', cfg.translations.input_channel)
   DisplayOnscreenKeyboard(1, 'FMMC_KEY_TIP1', '', '', '', '', '', 15)
   while ( UpdateOnscreenKeyboard() ~= 1 and UpdateOnscreenKeyboard() ~= 2 ) do
      Citizen.Wait(0)
   end
   if ( UpdateOnscreenKeyboard() ~= 2 ) then
      local result = GetOnscreenKeyboardResult()
         Citizen.Wait(100)
      return result
   else
         Citizen.Wait(100)
      return nil
   end
end
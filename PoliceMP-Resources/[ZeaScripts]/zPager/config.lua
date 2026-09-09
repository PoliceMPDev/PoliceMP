cfg = {}

cfg.notificationSound = true --@param: Set this option to 'true' if you wish to enable a sound to play when receiving notifications.
cfg.audioDistance = 5.0 --@param: Distance which the pager alarm can be heard from

-- @param: In this section, you have the ability to modify all of the resource's commands.
cfg.commands = {
   setChannel = { command = 'setChannel', helpText = 'Set pager channel.' },
   pageChannel = { command = 'page', helpText = 'Page a specificed channel.' },
}

-- @param: Here, you have the option to enable Discord logging for pager activations.
cfg.discordLog = {
   allow = false,
   webhook = 'DISCORD_WEBHOOK_URL',

   communityName = "Community Name",
   communityLogo = "https://i.imgur.com/hAV6F86.jpeg",
}

-- @param: Here you can translate the script's text into other languages.
cfg.translations = {
   page_sent = 'Page sent to channel:',
   channel_set = 'Channel set:',
   input_channel = 'Input Pager Channel',

   already_channel = 'Your are already on this Channel!',
   no_input = 'No channelID inputted!',
   no_perms = 'Insufficient Permissions!',
   invalid_input = 'Invalid channelID inputted!',
}

-- @param: Here, you can set up and configure pager channels.
cfg.channels = {
   -- BLOCK START --
   ['1'] = {
      isWhitelisted = false, --@param: See to client/cl-utilities.lua to setup your own whitelist
   },
   -- BLOCK END --
   -- BLOCK START --
   ['2'] = {
      isWhitelisted = false, --@param: See to client/cl-utilities.lua to setup your own whitelist
   },
   -- BLOCK END --
}
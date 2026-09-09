fx_version 'cerulean'
games {'gta5'}

author 'The Ambitioneers'
description 'Roxwood County Train Package'
version '2.0.0'

files {
  'data/vehicles.meta',
  'data/handling.meta',

  'data/*.meta',

  'stream/configs/trains.xml',
  'stream/configs/traintracks.xml',

  'traintracks/trains13.dat',
  'traintracks/trains14.dat',
  
  'train_enabler.lua',
}

escrow_ignore 
{
    'train_enabler.lua',
  }

data_file 'HANDLING_FILE' 'data/handling.meta'
data_file 'VEHICLE_METADATA_FILE' 'data/vehicles.meta'

-- Client --
client_script 'train_enabler.lua'
lua54 'yes'

dependency '/assetpacks'
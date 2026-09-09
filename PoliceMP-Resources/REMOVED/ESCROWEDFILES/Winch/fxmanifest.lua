fx_version 'bodacious'
game 'gta5'
lua54 'yes'

description 'Nabla Corporation - Winch System'
author 'Nabla Corporation'
version '1.4.2a'

client_script 'Client/*.lua'
server_script 'Server/*.lua'
shared_script 'Shared/*.lua'

escrow_ignore {
  'Shared/*.lua',
  'Server/access.lua',
  'Client/access.lua',
}

dependencies {
  '/server:4752',
}

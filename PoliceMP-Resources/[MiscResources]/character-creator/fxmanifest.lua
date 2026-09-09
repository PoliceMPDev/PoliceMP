fx_version 'bodacious'
game 'gta5'

client_script 'CharacterCreator.net.dll';
server_script 'CharacterCreator_Server.net.dll'

files
{
	'CharacterCreator_Shared.net.dll',
	'Newtonsoft.Json.dll',
	'NativeUI.dll'
}

server_exports 
{
	'startCreation',
	'stopCreation',
	'applyAppearanace'
}

author 'TheFuseGamer'
version '1.0.0'
description 'Fuse-Gaming Character Creator'
# HOW TO INSTALL
1. This is spikes V2 which include inventory capabilities, if you dont want to use the inventory dont do any of the below steps
2. Move the spikespack image to your inventory images
3. If you use QBCORE add thise line to qb-core/shared.lua: 
['spikespack'] 			 = {['name'] = 'spikespack', 				['label'] = 'Spikes Pack', 				['weight'] = 0, 		['type'] = 'item', 		['image'] = 'spikespack.png', 		['unique'] = true, 		['useable'] = true, 	['shouldClose'] = true,	   ['combinable'] = nil,   ['description'] = 'A 5 spikes pack for your needs.'},

* To add the spikes item to qb-shops you can add this line to the shop:
[13] = {
    name = "spikespack",
    price = 100,
    amount = 100,
    info = {},
    type = "item",
    slot = 13,
},

4. If you use esx add these lines to your database:
INSERT INTO `items` (`name`, `label`, `weight`) VALUES
	('spikespack', 'Spikes Pack', 0)
;

* If you use esx_inventoryhud or similar scripts you can make the inventory close once you equip the spikes by adding
the item to Config.CloseUiItems

Example For esx_shops:
		{ name = "spikespack", price = 100, count = -1 },

* Note: You will still be able to equip the spikes from the vehicle trunk

* Note if you use esx/qbcore and you want to test the script make you sure you relog after starting/restarting it or resetting your job!


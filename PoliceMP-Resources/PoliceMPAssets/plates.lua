--- IMAGE CONFIG HERE ---
imageUrl = "https://game-media.policemp.com/reg-1.webp" -- Paste your image URL here (doesn't have to be from imgur)
plateUrl = "https://game-media.policemp.com/reg-2.webp"
plateNormalUrl = "https://game-media.policemp.com/reg-3.png"


-- The actual script --
local textureDic = CreateRuntimeTxd('duiTxd')                                                           -- Create custom texture dictionary only needs to be done once
local object = CreateDui(imageUrl, 540, 300)                                                            -- Load image into object
local handle = GetDuiHandle(object)                                                                     -- Gets DUI handle from object

CreateRuntimeTextureFromDuiHandle(textureDic, "duiTex", handle)                                         -- Creates the texture "duiTex" in the "duiTxd" dictionary
AddReplaceTexture('vehshare', 'plate01', 'duiTxd', 'duiTex')                                            -- Applies "duiTex" from "duiTxd" to "plate01" from "vehshare"
AddReplaceTexture('vehshare', 'plate02', 'duiTxd', 'duiTex')                                            -- Applies "duiTex" from "duiTxd" to "plate02" from "vehshare"
AddReplaceTexture('vehshare', 'plate03', 'duiTxd', 'duiTex')                                            -- Applies "duiTex" from "duiTxd" to "plate03" from "vehshare"
AddReplaceTexture('vehshare', 'plate04', 'duiTxd', 'duiTex')                                            -- Applies "duiTex" from "duiTxd" to "plate04" from "vehshare"
AddReplaceTexture('vehshare', 'plate05', 'duiTxd', 'duiTex')                                            -- Applies "duiTex" from "duiTxd" to "plate05" from "vehshare"

local object = CreateDui('https://i.imgur.com/Q3uw6V7.png', 540, 300)                                   -- this URL doesn't need to be edited, its just the 2d model for the plate -- Load image into object
local handle = GetDuiHandle(object)                                                                     -- Gets DUI handle from object
CreateRuntimeTextureFromDuiHandle(textureDic, "duiTex2", handle)                                        -- Creates the texture "duiTex" in the "duiTxd" dictionary
AddReplaceTexture('vehshare', 'plate01_n', 'duiTxd', 'duiTex2')                                         -- Applies "duiTex2" from "duiTxd" to "plate01_n" from "vehshare"
AddReplaceTexture('vehshare', 'plate02_n', 'duiTxd', 'duiTex2')                                         -- Applies "duiTex2" from "duiTxd" to "plate02_n" from "vehshare"
AddReplaceTexture('vehshare', 'plate03_n', 'duiTxd', 'duiTex2')                                         -- Applies "duiTex2" from "duiTxd" to "plate03_n" from "vehshare"
AddReplaceTexture('vehshare', 'plate04_n', 'duiTxd', 'duiTex2')                                         -- Applies "duiTex2" from "duiTxd" to "plate04_n" from "vehshare"
AddReplaceTexture('vehshare', 'plate05_n', 'duiTxd', 'duiTex2')                                         -- Applies "duiTex2" from "duiTxd" to "plate05_n" from "vehshare"

local plateTextureDic = CreateRuntimeTxd('plates')                                                      -- Create custom texture dictionary only needs to be done once
local plateObject = CreateDui(plateUrl, 256, 128)                                                       -- Load image into object
local plateHandle = GetDuiHandle(plateObject)                                                           -- Gets DUI handle from object
local plateNormalObject = CreateDui(plateNormalUrl, 256, 128)                                           -- Load image into object
local plateNormalHandle = GetDuiHandle(plateNormalObject)                                               -- Gets DUI handle from object
CreateRuntimeTextureFromDuiHandle(plateTextureDic, "vehicle_generic_plate_font", plateHandle)           -- Creates the texture "duiTex" in the "duiTxd" dictionary
CreateRuntimeTextureFromDuiHandle(plateTextureDic, "vehicle_generic_plate_font_n", plateNormalHandle)   -- Creates the texture "duiTex" in the "duiTxd" dictionary

AddReplaceTexture('vehshare', 'vehicle_generic_plate_font', 'plates', 'vehicle_generic_plate_font')     -- Applies "duiTex" from "duiTxd" to "plate05" from "vehshare"
AddReplaceTexture('vehshare', 'vehicle_generic_plate_font_n', 'plates', 'vehicle_generic_plate_font_n') -- Applies "duiTex" from "duiTxd" to "plate05" from "vehshare"

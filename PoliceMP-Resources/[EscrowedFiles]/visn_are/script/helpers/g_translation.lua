--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

local languageCode = ClientConfig.m_languageCode
local languageData

function TranslateText(translateName, ...)
    if languageData == nil then
        LogError("Unknown error while translation:", translateName)
        return
    end

    if languageData.messages[translateName] == nil then
        LogError("Translation \"" .. translateName .. "\" in script/languages/" .. languageCode .. ".json not available.")
        return "Translation \"" .. translateName .. "\" not found."
    end

    return string.format(languageData.messages[translateName], ...)
end

function InitLanguages()
    languageData = LoadResourceFile(GetCurrentResourceName(), "script/languages/" .. languageCode .. '.json')

    if languageData == nil then
        LogError("Language script/languages/" .. languageCode .. ".json doesn't exist.")
        return
    end

    languageData = json.decode(languageData)
    if not languageData then return end
    LogInformation("Language " .. languageData.name .. " (script/languages/" .. languageData.code .. ".json) from " .. languageData.author .. " has been loaded.")

    return languageData
end

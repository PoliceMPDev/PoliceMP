-- client.lua

local messages = {
    CCTV = {},
    ANPR = {}
}
local recentCalls = {}
local currentTab = "CCTV"
local logIndex = 1
local callIndex = 1

local uiOpen = false
local hasFocus = false

RegisterNetEvent('chat:addMessage')
AddEventHandler('chat:addMessage', function(data)
    if not data or not data.args or #data.args == 0 then return end
    local msg = data.args[1]
    if type(msg) ~= "string" then return end

    local stripped = msg:gsub("~.-~", ""):gsub("%^%w", "")

    if stripped:find("^CCTV:") or stripped:find("^ANPR:") then
        TriggerEvent("loghistory:add", stripped)
    end
end)

RegisterNetEvent("loghistory:add", function(raw)
    local text = raw:gsub("~.-~", ""):gsub("%^%w", "")
    local type = nil

    if text:find("^CCTV:") then
        type = "CCTV"
        table.insert(messages.CCTV, 1, text)
        if #messages.CCTV > 10 then table.remove(messages.CCTV) end
    elseif text:find("^ANPR:") then
        type = "ANPR"
        table.insert(messages.ANPR, 1, text)
        if #messages.ANPR > 10 then table.remove(messages.ANPR) end
    end

    if uiOpen and type == currentTab and logIndex == 1 then
        SendNUIMessage({ action = "updateLeft", message = text })
    end
end)

RegisterNetEvent('callhistory:add', function(message)
    local cleanedMessage = message:gsub("~.-~", ""):gsub("%^%w", "")
    table.insert(recentCalls, 1, cleanedMessage)
    if #recentCalls > 10 then table.remove(recentCalls) end

    if uiOpen and callIndex == 1 then
        SendNUIMessage({ action = 'updateRight', message = cleanedMessage })
    end
end)

RegisterCommand('calls', function()
    if uiOpen then
        SendNUIMessage({ action = "hide" })
        SetNuiFocus(false, false)
        uiOpen = false
        hasFocus = false
    else
        logIndex = 1
        callIndex = 1
        SendNUIMessage({
            action = "show",
            left = messages[currentTab][1] or "No logs yet.",
            right = recentCalls[1] or "No calls yet."
        })
        uiOpen = true
        hasFocus = false
        SetNuiFocusKeepInput(false)
    end
end)

RegisterCommand("togglecalls", function()
    if uiOpen then
        hasFocus = not hasFocus
        SetNuiFocus(hasFocus, hasFocus)
        SetNuiFocusKeepInput(hasFocus)
    end
end, false)

RegisterKeyMapping("togglecalls", "Toggle NUI Focus for Calls", "keyboard", "-")

RegisterNUICallback("navigateLeft", function(data, cb)
    local list = messages[currentTab]
    if data.direction == "next" and logIndex > 1 then
        logIndex = logIndex - 1
    elseif data.direction == "prev" and logIndex < #list then
        logIndex = logIndex + 1
    end
    SendNUIMessage({ action = "updateLeft", message = list[logIndex] or "No logs." })
    cb({})
end)

RegisterNUICallback("navigateRight", function(data, cb)
    if data.direction == 'next' and callIndex > 1 then
        callIndex = callIndex - 1
    elseif data.direction == 'prev' and callIndex < #recentCalls then
        callIndex = callIndex + 1
    end
    SendNUIMessage({ action = 'updateRight', message = recentCalls[callIndex] or "No calls." })
    cb({})
end)

RegisterNUICallback("tabLeft", function(data, cb)
    currentTab = data.tab
    logIndex = 1
    SendNUIMessage({ action = "updateLeft", message = messages[currentTab][1] or "No logs." })
    cb({})
end)

RegisterNUICallback('close', function(_, cb)
    SetNuiFocus(false, false)
    SendNUIMessage({ action = 'hide' })
    uiOpen = false
    hasFocus = false
    cb({})
end)

--[[Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        if (IsControlJustPressed(0, 84) or IsControlJustPressed(1, 84)) and uiOpen then
            hasFocus = not hasFocus
            SetNuiFocus(hasFocus, hasFocus)
            SetNuiFocusKeepInput(hasFocus)
        end
    end
end)]]--

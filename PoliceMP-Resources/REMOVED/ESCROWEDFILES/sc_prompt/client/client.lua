local response

local function toggleNuiFrame(shouldShow)
    SetNuiFocus(shouldShow, shouldShow)
    SendReactMessage('setVisible', shouldShow)
end

local function createPromp(title, description, description2, cb)
    response = nil
    toggleNuiFrame(true)
    SendReactMessage('createPrompt', {
        show = true,
        title = title,
        description = description,
        description2 = description2
    })
    if cb == nil then return end
    while response == nil do
        Wait(0)
    end
    cb(response)
end

RegisterNUICallback('prompt:accept', function(_, cb)
    toggleNuiFrame(false)
    response = true
    cb('ok')
end)

RegisterNUICallback('prompt:refuse', function(_, cb)
    toggleNuiFrame(false)
    response = false
    cb('ok')
end)

exports('createPrompt', createPromp)

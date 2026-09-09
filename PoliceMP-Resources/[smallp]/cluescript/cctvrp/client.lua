RegisterCommand('cc', function(source, args, rawCommand)
    local postal = GetUserInput("Enter Postal", "", 5)
    if not postal or postal == "" then
        TriggerEvent('chat:addMessage', { args = {"System", "You must enter a postal!"} })
        return
    end

    local message = GetUserInput("Enter Message", "", 200)
    if not message or message == "" then
        TriggerEvent('chat:addMessage', { args = {"System", "You must enter a message!"} })
        return
    end

    local formattedMessage = string.format("~y~CCTV: [P%s] %s", postal, message)

    TriggerServerEvent('customchat:sendGlobalMessage', formattedMessage)

    -- Log the command usage in txAdmin console
    ExecuteCommand("[LOG] --------------------------------------------- [LOG]")
    ExecuteCommand(string.format("[LOG] CCTV RP Used: [Postal: P%s] %s", postal, message))
    ExecuteCommand("[LOG] --------------------------------------------- [LOG]")
end, false)

RegisterCommand('ping', function(source, args, rawCommand)
    local postal = GetUserInput("Enter Postal", "", 5)
    if not postal or postal == "" then
        TriggerEvent('chat:addMessage', { args = {"System", "You must enter a postal!"} })
        return
    end

    local message = GetUserInput("Enter Road Name / Borough and Targeted player / division", "", 100)
    if not message or message == "" then
        TriggerEvent('chat:addMessage', { args = {"System", "You must enter a message!"} })
        return
    end

    postal = string.upper(postal)
    message = string.upper(message)

    local formattedMessage = string.format("~y~ANPR:  [P%s] %s", postal, message)

    TriggerServerEvent('customchat:sendGlobalMessage', formattedMessage)

    -- Log the command usage in txAdmin console
    ExecuteCommand("[LOG] --------------------------------------------- [LOG]")
    ExecuteCommand(string.format("[LOG] ANPR RP Used: [Postal: P%s] %s", postal, message))
    ExecuteCommand("[LOG] --------------------------------------------- [LOG]")
end, false)


function GetUserInput(windowTitle, defaultText, maxInputLength)
    AddTextEntry('FMMC_KEY_TIP1', windowTitle) -- Sets the window title
    DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP1", "", defaultText, "", "", "", maxInputLength)

    while UpdateOnscreenKeyboard() == 0 do
        Wait(0)
    end

    if GetOnscreenKeyboardResult() then
        return GetOnscreenKeyboardResult()
    else
        return nil
    end
end

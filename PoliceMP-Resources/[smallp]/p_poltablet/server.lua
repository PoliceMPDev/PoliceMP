local OPENROUTER_API_KEY = "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"
local API_URL = "https://openrouter.ai/api/v1/chat/completions"

local activeInvestigations = {} -- Stores the investigation context for each player

--[[RegisterCommand("investigate", function(source)
    local src = source
    if activeInvestigations[src] then
        TriggerClientEvent("poltablet:receiveResponse", src, "Reopening previous investigation...")
        for _, message in ipairs(activeInvestigations[src].chatHistory) do
            if message.role == "assistant" or message.role == "user" then
                TriggerClientEvent("poltablet:receiveResponse", src, message.content)
            end
        end
    else
        TriggerClientEvent("poltablet:receiveResponse", src, "What was the call about?")
        activeInvestigations[src] = nil
    end
end, false)]]--

RegisterServerEvent("poltablet:submitResponse")
AddEventHandler("poltablet:submitResponse", function(playerInput)
    local src = source

    if not playerInput or playerInput == "" then
        TriggerClientEvent("poltablet:receiveResponse", src, "Please enter a valid question.")
        return
    end

    -- If it's the first input, store it as the investigation context
    if not activeInvestigations[src] then
        activeInvestigations[src] = {
            caseContext = playerInput,
            chatHistory = {
                { role = "system", content = "**You are a british MET police constable in a roleplay fivem server, respond each scenario with its relevant legislations laws and its sections. Only provide law Sections and legislation, on each specific crime scenarios, Keep it short and relative to the scenario" },
                { role = "user", content = "Initial case details: " .. playerInput }
            }
        }
        TriggerClientEvent("poltablet:receiveResponse", src, "Proceed to input your scenario.")
        return
    end

    -- Add player's question to the chat history
    table.insert(activeInvestigations[src].chatHistory, { role = "user", content = playerInput })

    local requestData = json.encode({
        model = "deepseek/deepseek-chat-v3-0324:free", -- Change to "gpt-4-turbo" or "claude-3-opus" if needed
        messages = activeInvestigations[src].chatHistory, -- Preserve full chat history
        max_tokens = 500, -- Reduce response length
        temperature = 0.5, -- Keep responses consistent
        top_p = 1
    })

    PerformHttpRequest(API_URL, function(status, resultData, headers)
        if status == 200 then
            local response = json.decode(resultData)
            if response and response.choices and response.choices[1] and response.choices[1].message then
                local aiText = response.choices[1].message.content
                table.insert(activeInvestigations[src].chatHistory, { role = "assistant", content = aiText }) -- Save AI response
                TriggerClientEvent("poltablet:receiveResponse", src, aiText)
            else
                TriggerClientEvent("poltablet:receiveResponse", src, "No response from AI.")
            end
        elseif status == 400 then
            print("^1[ERROR]^7 OpenRouter Request Failed: HTTP 400 (Invalid Request)")
            print("^1[DEBUG]^7 Response: " .. (resultData or "No Response"))
            TriggerClientEvent("poltablet:receiveResponse", src, "Invalid request to OpenRouter. Check API format.")
        else
            print("^1[ERROR]^7 OpenRouter Request Failed: HTTP " .. status)
            print("^1[DEBUG]^7 Response: " .. (resultData or "No Response"))
            TriggerClientEvent("poltablet:receiveResponse", src, "Error contacting AI. Check logs.")
        end
    end, "POST", requestData, {
        ["Content-Type"] = "application/json",
        ["Authorization"] = "Bearer " .. OPENROUTER_API_KEY
    })
end)

RegisterCommand("polreset", function(source)
    local src = source
    activeInvestigations[src] = nil
    TriggerClientEvent("poltablet:receiveResponse", src, "Start a new case scenario.")
end, false)

RegisterServerEvent("poltablet:resetInvestigation")
AddEventHandler("poltablet:resetInvestigation", function()
    local src = source
    activeInvestigations[src] = nil
    TriggerClientEvent("poltablet:receiveResponse", src, "Scenario legislation has been reset.")
end)
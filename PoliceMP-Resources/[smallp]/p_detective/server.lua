local OPENROUTER_API_KEY = "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"
local API_URL = "https://openrouter.ai/api/v1/chat/completions"

local activeInvestigations = {} -- Stores the investigation context for each player

RegisterCommand("investigate", function(source)
    local src = source
    if activeInvestigations[src] then
        TriggerClientEvent("detective:receiveResponse", src, "Reopening previous investigation...")
        for _, message in ipairs(activeInvestigations[src].chatHistory) do
            if message.role == "assistant" or message.role == "user" then
                TriggerClientEvent("detective:receiveResponse", src, message.content)
            end
        end
    else
        TriggerClientEvent("detective:receiveResponse", src, "What was the call about?")
        activeInvestigations[src] = nil
    end
end, false)

RegisterServerEvent("detective:submitResponse")
AddEventHandler("detective:submitResponse", function(playerInput)
    local src = source

    if not playerInput or playerInput == "" then
        TriggerClientEvent("detective:receiveResponse", src, "Please enter a valid question.")
        return
    end

    -- If it's the first input, store it as the investigation context
    if not activeInvestigations[src] then
        activeInvestigations[src] = {
            caseContext = playerInput,
            chatHistory = {
                { role = "system", content = "**You are a roleplay AI police detective in a FiveM server.** Respond as if you are an investigator at a real crime scene. Do not use John doe as a name.. Please use a variety of random names ID names. Only provide details based on facts and evidence found. Never invent locations, suspects, or events, or give next course of action or instructions. Keep responses short and to the point and no point form responses" },
                { role = "user", content = "Initial case details: " .. playerInput }
            }
        }
        TriggerClientEvent("detective:receiveResponse", src, "Proceed with your investigation. Ask questions.")
        return
    end

    -- Add player's question to the chat history
    table.insert(activeInvestigations[src].chatHistory, { role = "user", content = playerInput })

    local requestData = json.encode({
        model = "openai/gpt-4o", -- Change to "gpt-4-turbo" or "claude-3-opus" if needed
        messages = activeInvestigations[src].chatHistory, -- Preserve full chat history
        max_tokens = 100, -- Reduce response length
        temperature = 0.5, -- Keep responses consistent
        top_p = 1
    })

    PerformHttpRequest(API_URL, function(status, resultData, headers)
        if status == 200 then
            local response = json.decode(resultData)
            if response and response.choices and response.choices[1] and response.choices[1].message then
                local aiText = response.choices[1].message.content
                table.insert(activeInvestigations[src].chatHistory, { role = "assistant", content = aiText }) -- Save AI response
                TriggerClientEvent("detective:receiveResponse", src, aiText)
            else
                TriggerClientEvent("detective:receiveResponse", src, "No response from AI.")
            end
        elseif status == 400 then
            print("^1[ERROR]^7 OpenRouter Request Failed: HTTP 400 (Invalid Request)")
            print("^1[DEBUG]^7 Response: " .. (resultData or "No Response"))
            TriggerClientEvent("detective:receiveResponse", src, "Invalid request to OpenRouter. Check API format.")
        else
            print("^1[ERROR]^7 OpenRouter Request Failed: HTTP " .. status)
            print("^1[DEBUG]^7 Response: " .. (resultData or "No Response"))
            TriggerClientEvent("detective:receiveResponse", src, "Error contacting AI. Check logs.")
        end
    end, "POST", requestData, {
        ["Content-Type"] = "application/json",
        ["Authorization"] = "Bearer " .. OPENROUTER_API_KEY
    })
end)

RegisterCommand("+xresetinvestigation", function(source)
    local src = source
    activeInvestigations[src] = nil
    TriggerClientEvent("detective:receiveResponse", src, "Investigation reset. Start a new case with /investigate.")
end, false)
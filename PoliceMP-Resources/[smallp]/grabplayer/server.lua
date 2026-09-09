local L0_1, L1_1, L2_1, L3_1
L0_1 = {}

L1_1 = RegisterServerEvent
L2_1 = "pmp:sendEvent"
L1_1(L2_1)

L1_1 = AddEventHandler
L2_1 = "pmp:sendEvent"
function L3_1(A0_2, A1_2, A2_2, A3_2)
  local L4_2
  L4_2 = source

  if A0_2 == nil or A1_2 == nil then
    --print("[pmp:sendEvent] Missing target1 or target2")
    return
  end

  if IsPlayerAceAllowed(L4_2, "Police.pc") then
    TriggerClientEvent("pmp:sendEvent", A0_2, A1_2, A3_2, true)
    TriggerClientEvent("pmp:sendEvent", A1_2, A0_2, A2_2, false)
  end
end

L1_1(L2_1, L3_1)
L1_1(L2_1, L3_1)
L1_1 = RegisterServerEvent
L2_1 = "pmp:Detach"
L1_1(L2_1)
L1_1 = AddEventHandler
L2_1 = "pmp:Detach"
function L3_1(A0_2)
  local L1_2, L2_2, L3_2
  L1_2 = TriggerClientEvent
  L2_2 = "pmp:Detach"
  L3_2 = A0_2
  L1_2(L2_2, L3_2)
end
L1_1(L2_1, L3_1)
L1_1 = RegisterServerEvent
L2_1 = "pmp:DeadEvent"
L1_1(L2_1)
L1_1 = AddEventHandler
L2_1 = "pmp:DeadEvent"
function L3_1(A0_2)
  local L1_2, L2_2, L3_2
  L1_2 = TriggerClientEvent
  L2_2 = "pmp:DeadEvent"
  L3_2 = A0_2
  L1_2(L2_2, L3_2)
end
L1_1(L2_1, L3_1)
L1_1 = RegisterServerEvent
L2_1 = "pmp:enterPed"
L1_1(L2_1)
L1_1 = AddEventHandler
L2_1 = "pmp:enterPed"
function L3_1(A0_2, A1_2, A2_2)
  local L3_2, L4_2, L5_2, L6_2, L7_2
  L3_2 = TriggerClientEvent
  L4_2 = "pmp:enterPed"
  L5_2 = A0_2
  L6_2 = A1_2
  L7_2 = A2_2
  L3_2(L4_2, L5_2, L6_2, L7_2)
end
L1_1(L2_1, L3_1)
L1_1 = RegisterServerEvent
L2_1 = "pmp:exitPed"
L1_1(L2_1)
L1_1 = AddEventHandler
L2_1 = "pmp:exitPed"
function L3_1(A0_2, A1_2)
  local L2_2, L3_2, L4_2, L5_2
  L2_2 = TriggerClientEvent
  L3_2 = "pmp:exitPed"
  L4_2 = A0_2
  L5_2 = A1_2
  L2_2(L3_2, L4_2, L5_2)
end
L1_1(L2_1, L3_1)
L1_1 = RegisterServerEvent
L2_1 = "pmp:syncVariable"
L1_1(L2_1)
L1_1 = AddEventHandler
L2_1 = "pmp:syncVariable"
function L3_1(A0_2)
  local L1_2, L2_2, L3_2, L4_2
  L1_2 = TriggerClientEvent
  L2_2 = "pmp:syncVariable"
  L3_2 = -1
  L4_2 = A0_2
  L1_2(L2_2, L3_2, L4_2)
  L0_1 = A0_2
end
L1_1(L2_1, L3_1)
L1_1 = Citizen
L1_1 = L1_1.CreateThread
function L2_1()
  local L0_2, L1_2, L2_2
  L0_2 = Config
  L0_2 = L0_2.ESX
  L0_2 = L0_2.enabled
  if L0_2 then
    L0_2 = Config
    L0_2 = L0_2.ESX
    L0_2 = L0_2.exceptionJobs
    if L0_2 then
      ESX = nil
      L0_2 = TriggerEvent
      L1_2 = "esx:getSharedObject"
      function L2_2(A0_3)
        local L1_3
        ESX = A0_3
      end
      L0_2(L1_2, L2_2)
    end
  end
end
L1_1(L2_1)
L1_1 = Config
L1_1 = L1_1.ESX
L1_1 = L1_1.enabled
if L1_1 then
  L1_1 = Config
  L1_1 = L1_1.ESX
  L1_1 = L1_1.exceptionJobs
  if L1_1 then
    L1_1 = RegisterServerEvent
    L2_1 = "pmp:exceptionJobs"
    L1_1(L2_1)
    L1_1 = AddEventHandler
    L2_1 = "pmp:exceptionJobs"
    function L3_1(A0_2, A1_2)
      local L2_2, L3_2, L4_2, L5_2, L6_2, L7_2, L8_2, L9_2, L10_2
      L2_2 = ESX
      L2_2 = L2_2.GetPlayerFromId
      L3_2 = A1_2
      L2_2 = L2_2(L3_2)
      L3_2 = TriggerClientEvent
      L4_2 = "pmp:exceptionJobs"
      L5_2 = A0_2
      L6_2 = false
      L3_2(L4_2, L5_2, L6_2)
      L3_2 = 1
      L4_2 = Config
      L4_2 = L4_2.ESX
      L4_2 = L4_2.jobs
      L4_2 = #L4_2
      L5_2 = 1
      for L6_2 = L3_2, L4_2, L5_2 do
        L7_2 = L2_2.job
        if nil ~= L7_2 then
          L7_2 = L2_2.job
          L7_2 = L7_2.name
          L8_2 = Config
          L8_2 = L8_2.ESX
          L8_2 = L8_2.jobs
          L8_2 = L8_2[L6_2]
          if L7_2 == L8_2 then
            L7_2 = TriggerClientEvent
            L8_2 = "pmp:exceptionJobs"
            L9_2 = A0_2
            L10_2 = true
            L7_2(L8_2, L9_2, L10_2)
          end
        end
      end
    end
    L1_1(L2_1, L3_1)
  end
end

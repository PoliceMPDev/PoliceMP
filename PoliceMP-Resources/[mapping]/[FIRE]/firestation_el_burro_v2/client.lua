------------------------------ CLEAR PEDS FROM AREA ------------------------------
CreateThread(function()
    while true do
       
        ClearAreaOfPeds(1165.61, -1487.12, 38.85, 20.0); 
        ClearAreaOfPeds(1202.41, -1473.18, 34.85, 20.0);
        -- ClearAreaOfVehicles(1452.67, -2605.99, 48.52, 15.0, false, false, false, false, false);
        
        Wait(100)
    end
end)

------------------------------ REMOVE OLD FIRESTATION MLO ------------------------------


CreateThread(function()
    local oldinterior = GetInteriorAtCoordsWithType(1201.39514, -1478.453, 33.8594131, 'v_firedept')
    DisableInterior(oldinterior, true)
    UnpinInterior(oldinterior)
end)
Citizen.CreateThread(function()
    while true do
        if IsControlJustPressed(0, 21) then
            SetRelationshipBetweenGroups(2, 'player', 'player')
        end
        if IsControlJustReleased(0, 21) then
            SetRelationshipBetweenGroups(1, 'player', 'player')
        end
        Citizen.Wait(0)
    end
end)
local previewOffsets = {x = 0.0, y = 0.0, z = 1.0, pitch = 0.0, roll = 0.0, yaw = 0.0}
local previewMode = false
local attachedVehicle = nil
local matrixProps = {}

function notify(msg)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(msg)
    DrawNotification(false, false)
end

function startMatrixPreview()
    local ped = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(ped, false)
    if vehicle == 0 then
        notify("You must be in a vehicle.")
        return
    end

    if previewMode then
        notify("Already in preview mode.")
        return
    end

    local vehNet = VehToNet(vehicle)
    previewMode = true
    attachedVehicle = vehicle
    previewOffsets = {x = 0.0, y = 0.0, z = 1.0, pitch = 0.0, roll = 0.0, yaw = 0.0}

    notify("Preview: Arrows = Move | PgUp/PgDn = Height | ,/. = Rotate | ENTER = Print Coords | SHIFT = Fast | CTRL = Slow")

    RequestModel("prop_matrix_01")
    while not HasModelLoaded("prop_matrix_01") do Wait(10) end

    matrixProps[vehNet] = CreateObject(GetHashKey("prop_matrix_01"), 0, 0, 0, true, true, true)
    FreezeEntityPosition(matrixProps[vehNet], false)

    CreateThread(function()
        while previewMode do
            local speedMult = 1.0
            if IsControlPressed(0, 21) then speedMult = 5.0 end       -- SHIFT
            if IsControlPressed(0, 36) then speedMult = 0.25 end     -- CTRL

            -- Movement
            if IsControlPressed(0, 172) then previewOffsets.y = previewOffsets.y + 0.01 * speedMult end -- Up
            if IsControlPressed(0, 173) then previewOffsets.y = previewOffsets.y - 0.01 * speedMult end -- Down
            if IsControlPressed(0, 174) then previewOffsets.x = previewOffsets.x - 0.01 * speedMult end -- Left
            if IsControlPressed(0, 175) then previewOffsets.x = previewOffsets.x + 0.01 * speedMult end -- Right
            if IsControlPressed(0, 10)  then previewOffsets.z = previewOffsets.z + 0.01 * speedMult end -- PgUp
            if IsControlPressed(0, 11)  then previewOffsets.z = previewOffsets.z - 0.01 * speedMult end -- PgDn

            -- Rotation (Yaw)
            if IsControlPressed(0, 81)  then previewOffsets.yaw = previewOffsets.yaw - 1.0 * speedMult end -- Q
            if IsControlPressed(0, 82)  then previewOffsets.yaw = previewOffsets.yaw + 1.0 * speedMult end -- E

            AttachEntityToEntity(
                matrixProps[vehNet], attachedVehicle, 0,
                previewOffsets.x, previewOffsets.y, previewOffsets.z,
                previewOffsets.pitch, previewOffsets.roll, previewOffsets.yaw,
                false, false, false, false, 2, true
            )

            SetEntityAlpha(matrixProps[vehNet], 150, false)

            -- ENTER key = Finish and print result
            if IsControlJustPressed(0, 191) then
                if matrixProps[vehNet] and DoesEntityExist(matrixProps[vehNet]) then
                    DeleteEntity(matrixProps[vehNet])
                end
                matrixProps[vehNet] = nil
                previewMode = false

                print("[Matrix Preview] Offsets:")
                print(string.format("Offset: vector3(%.3f, %.3f, %.3f)", previewOffsets.x, previewOffsets.y, previewOffsets.z))
                print(string.format("Rotation: vector3(%.3f, %.3f, %.3f)", previewOffsets.pitch, previewOffsets.roll, previewOffsets.yaw))

                notify("Matrix position printed to console.")
                return
            end

            Wait(0)
        end
    end)
end

RegisterCommand("+xmatrixpreview", function()
    startMatrixPreview()
end)

RegisterCommand("matrixSetup", function(source, args, rawCommand)
    -- Command is intercepted and disabled silently
end, false)

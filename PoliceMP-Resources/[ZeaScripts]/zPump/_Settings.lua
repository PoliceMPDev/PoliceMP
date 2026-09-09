_Settings = {
    
    ['Appliances'] = {
    
        --

        ['SAPWNCODE'] = {
          Pump_Offset = {x = 0.0; y = -4.0; z = 0.0};
            ['Pump_Doors'] = {
                Openable = false;            --> Can the rear pump doors open / close <--
                Catergory = "Vehicle_Door"; --> Vehicle_Extra // Vehicle_Door <--
                ['Door_Intergers'] = {
                    DoorInts = { 4, 5 };     --> https://docs.fivem.net/natives/?_0x93D9BD300D7789E5 <--
                    DoorExtra_Open = 1;      --> Extra interger when pump doors are open <--
                    DoorExtra_Close = 3;     --> Extra interger when pump doors are closed <--
                }
            }
        };
        ['LFB1'] = {
          Pump_Offset = {x = 0.0; y = -4.0; z = 0.0};
            ['Pump_Doors'] = {
                Openable = false;            --> Can the rear pump doors open / close <--
                Catergory = "Vehicle_Door"; --> Vehicle_Extra // Vehicle_Door <--
                ['Door_Intergers'] = {
                    DoorInts = { 4, 5 };     --> https://docs.fivem.net/natives/?_0x93D9BD300D7789E5 <--
                    DoorExtra_Open = 1;      --> Extra interger when pump doors are open <--
                    DoorExtra_Close = 3;     --> Extra interger when pump doors are closed <--
                }
            }
        };

        --

    };

    ['Miscellaneous'] = {
        DistanceToPump = 1.2;
        DetectionDistance = 7.0;
        ConsolePrint = false;
    };

    ['Text'] = {
        Open_PumpDoors = "[~g~E~w~] Open";
        Close_PumpDoors = "[~r~E~w~] Close";
        View_PumpUI = "[~y~G~w~] View";
    }

}